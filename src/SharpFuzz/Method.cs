using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace SharpFuzz
{
	internal sealed class Method
	{
		private readonly FieldReference sharedMem;
		private readonly FieldReference prevLocation;

		private readonly MethodBody body;
		private readonly ILProcessor il;
		private readonly List<Instruction> instructions;
		private readonly Dictionary<Instruction, Instruction> instrumented;
		private readonly TypeReference byteType;

		private Method(FieldReference sharedMem, FieldReference prevLocation, MethodDefinition method)
		{
			this.sharedMem = sharedMem;
			this.prevLocation = prevLocation;

			body = method.Body;
			il = body.GetILProcessor();
			instructions = body.Instructions.ToList();
			instrumented = new Dictionary<Instruction, Instruction>();
			byteType = method.Module.ImportReference(typeof(byte));

			body.SimplifyMacros();
			body.Instructions.Clear();

			FindInstrumentationTargets();
			Instrument();
			UpdateBranchTargets();
			UpdateExceptionHandlers();

			body.OptimizeMacros();
		}

		public static void Instrument(FieldReference sharedMem, FieldReference prevLocation, MethodDefinition method)
		{
			new Method(sharedMem, prevLocation, method);
		}

		private void FindInstrumentationTargets()
		{
			instrumented.Add(instructions[0], null);

			foreach (var ins in instructions)
			{
				var flowControl = ins.OpCode.FlowControl;

				if (flowControl == FlowControl.Cond_Branch)
				{
					instrumented[ins.Next] = null;
				}

				if (ins.OpCode == OpCodes.Switch)
				{
					foreach (var target in (Instruction[])ins.Operand)
					{
						instrumented[target] = null;
					}
				}
				else if (flowControl == FlowControl.Branch || flowControl == FlowControl.Cond_Branch)
				{
					instrumented[(Instruction)ins.Operand] = null;
				}
			}

			foreach (var handler in body.ExceptionHandlers)
			{
				instrumented[handler.HandlerStart] = null;
			}
		}

		private void Instrument()
		{
			foreach (var ins in instructions)
			{
				if (instrumented.ContainsKey(ins))
				{
					using (var it = GenerateInstrumentationInstructions().GetEnumerator())
					{
						it.MoveNext();
						instrumented[ins] = it.Current;
						il.Append(it.Current);

						while (it.MoveNext())
						{
							il.Append(it.Current);
						}
					}
				}

				il.Append(ins);
			}
		}

		private IEnumerable<Instruction> GenerateInstrumentationInstructions()
		{
			int id = IdGenerator.Next();

			yield return il.Create(OpCodes.Ldsfld, sharedMem);
			yield return il.Create(OpCodes.Ldc_I4, id);
			yield return il.Create(OpCodes.Ldsfld, prevLocation);
			yield return il.Create(OpCodes.Xor);
			yield return il.Create(OpCodes.Ldelema, byteType);
			yield return il.Create(OpCodes.Dup);
			yield return il.Create(OpCodes.Ldind_U1);
			yield return il.Create(OpCodes.Ldc_I4_1);
			yield return il.Create(OpCodes.Add);
			yield return il.Create(OpCodes.Conv_U1);
			yield return il.Create(OpCodes.Stind_I1);
			yield return il.Create(OpCodes.Ldc_I4, id >> 1);
			yield return il.Create(OpCodes.Stsfld, prevLocation);
		}

		private void UpdateBranchTargets()
		{
			foreach (var ins in instructions)
			{
				var flowControl = ins.OpCode.FlowControl;

				if (ins.OpCode == OpCodes.Switch)
				{
					var targets = (Instruction[])ins.Operand;

					for (int i = 0; i < targets.Length; ++i)
					{
						targets[i] = instrumented[targets[i]];
					}
				}
				else if (flowControl == FlowControl.Branch || flowControl == FlowControl.Cond_Branch)
				{
					ins.Operand = instrumented[(Instruction)ins.Operand];
				}
			}
		}

		private void UpdateExceptionHandlers()
		{
			foreach (var handler in body.ExceptionHandlers)
			{
				if (instrumented.TryGetValue(handler.TryStart, out var tryStart))
				{
					handler.TryStart = tryStart;
				}

				if (instrumented.TryGetValue(handler.TryEnd, out var tryEnd))
				{
					handler.TryEnd = tryEnd;
				}

				if (instrumented.TryGetValue(handler.HandlerStart, out var handlerStart))
				{
					handler.HandlerStart = handlerStart;
				}

				if (instrumented.TryGetValue(handler.HandlerEnd, out var handlerEnd))
				{
					handler.HandlerEnd = handlerEnd;
				}
			}
		}
	}
}
