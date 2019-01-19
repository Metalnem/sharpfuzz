using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace SharpFuzz
{
	// Method class performs the afl-fuzz instrumentation on a single method.
	// All the work is actually done in the constructor, but the static
	// instrumentation method is exposed for ease of use.
	internal sealed class Method
	{
		private readonly MemberRef sharedMem;
		private readonly MemberRef prevLocation;

		private readonly CilBody body;
		private readonly List<Instruction> instructions;
		private readonly Dictionary<Instruction, Instruction> instrumented;

		private Method(MemberRef sharedMem, MemberRef prevLocation, MethodDef method)
		{
			this.sharedMem = sharedMem;
			this.prevLocation = prevLocation;

			body = method.Body;
			instructions = body.Instructions.ToList();
			instrumented = new Dictionary<Instruction, Instruction>();

			body.SimplifyBranches();
			body.Instructions.Clear();

			FindInstrumentationTargets();
			Instrument();
			UpdateBranchTargets();
			UpdateExceptionHandlers();

			body.OptimizeBranches();
		}

		public static void Instrument(MemberRef sharedMem, MemberRef prevLocation, MethodDef method)
		{
			new Method(sharedMem, prevLocation, method);
		}

		// Find all the locations that we want to instrument. These are:
		// 1) Function entry points
		// 2) Branch destinations
		// 3) First instructions after conditional branches (else blocks)
		// 4) Catch blocks (implicit jump destinations)
		private void FindInstrumentationTargets()
		{
			instrumented.Add(instructions[0], null);

			for (int i = 0; i < instructions.Count; ++i)
			{
				var ins = instructions[i];
				var flowControl = ins.OpCode.FlowControl;

				if (flowControl == FlowControl.Cond_Branch)
				{
					instrumented[instructions[i + 1]] = null;
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

		// Regenerate the IL for the method. If some instruction was
		// previously marked as an instrumentation target, generate
		// the instrumentation code and put it before the instruction.
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
						body.Instructions.Add(it.Current);

						while (it.MoveNext())
						{
							body.Instructions.Add(it.Current);
						}
					}
				}

				body.Instructions.Add(ins);
			}
		}

		// Generates the instrumentation instructions for a branch
		// destination. It's equivalent to the following C# code:
		// var id = IdGenerator.Next();
		// SharpFuzz.Common.Trace.SharedMem[id ^ SharpFuzz.Common.Trace.PrevLocation]++;
		// SharpFuzz.Common.Trace.PrevLocation = id >> 1;
		private IEnumerable<Instruction> GenerateInstrumentationInstructions()
		{
			int id = IdGenerator.Next();

			yield return Instruction.Create(OpCodes.Ldsfld, sharedMem);
			yield return Instruction.Create(OpCodes.Ldc_I4, id);
			yield return Instruction.Create(OpCodes.Ldsfld, prevLocation);
			yield return Instruction.Create(OpCodes.Xor);
			yield return Instruction.Create(OpCodes.Add);
			yield return Instruction.Create(OpCodes.Dup);
			yield return Instruction.Create(OpCodes.Ldind_U1);
			yield return Instruction.Create(OpCodes.Ldc_I4_1);
			yield return Instruction.Create(OpCodes.Add);
			yield return Instruction.Create(OpCodes.Conv_U1);
			yield return Instruction.Create(OpCodes.Stind_I1);
			yield return Instruction.Create(OpCodes.Ldc_I4, id >> 1);
			yield return Instruction.Create(OpCodes.Stsfld, prevLocation);
		}

		// Change all branch destinations to point to the first instruction
		// in the instrumentation code instead of the original branch target.
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

		// Once the instrumentation is completed, locations of try/catch/finally
		// blocks could become out of date. For example, if the beginning of the
		// catch block is instrumented, the end of the corresponding try block
		// should point to the first instruction in the instrumentation code,
		// not the instruction that was previously the first in the catch block.
		// This function updates all exception handlers with the correct locations
		// for start/end instructions.
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

				if (handler.HandlerEnd != null && instrumented.TryGetValue(handler.HandlerEnd, out var handlerEnd))
				{
					handler.HandlerEnd = handlerEnd;
				}
			}
		}
	}
}
