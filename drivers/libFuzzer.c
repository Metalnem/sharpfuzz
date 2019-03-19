#include "errno.h"
#include "stddef.h"
#include "stdint.h"
#include "stdio.h"
#include "stdlib.h"
#include "string.h"
#include "unistd.h"
#include <sys/shm.h>

#define MAP_SIZE (1 << 16)
#define DATA_SIZE (1 << 20)

#define CTL_FD 198
#define ST_FD 199
#define LEN_FLD_SIZE 4

#define SHM_ID_VAR "__LIBFUZZER_SHM_ID"

__attribute__((weak, section("__libfuzzer_extra_counters")))
uint8_t extra_counters[MAP_SIZE];

static const char *target_path_name = "--target_path";
static const char *target_arg_name = "--target_arg";

static const char *target_path;
static const char *target_arg;

static int ctl_fd;
static int st_fd;
static int shm_id;
static uint8_t *trace_bits;
static pid_t child_pid;

static void die(const char *msg)
{
	printf("%s\n", msg);
	exit(1);
}

static void die_sys(const char *msg)
{
	printf("%s: %s\n", msg, strerror(errno));
	exit(1);
}

static void remove_shm()
{
	shmctl(shm_id, IPC_RMID, NULL);
}

static const char *read_flag_value(const char *param, const char *name)
{
	size_t len = strlen(name);

	if (strstr(param, name) == param && param[len] == '=' && param[len + 1])
	{
		return &param[len + 1];
	}

	return NULL;
}

static void parse_flags(int argc, char **argv)
{
	for (int i = 0; i < argc; ++i)
	{
		char *param = argv[i];

		if (!target_path)
		{
			target_path = read_flag_value(param, target_path_name);
		}

		if (!target_arg)
		{
			target_arg = read_flag_value(param, target_arg_name);
		}
	}
}

int LLVMFuzzerInitialize(int *argc, char ***argv)
{
	parse_flags(*argc, *argv);

	if (!target_path)
	{
		die("You must specify the target path by using the --target_path command line flag.");
	}

	int ctl_pipe[2];
	int st_pipe[2];

	if (pipe(ctl_pipe) || pipe(st_pipe))
	{
		die_sys("pipe() failed");
	}

	shm_id = shmget(IPC_PRIVATE, MAP_SIZE + DATA_SIZE, IPC_CREAT | IPC_EXCL | 0600);

	if (shm_id < 0)
	{
		die_sys("shmget() failed");
	}

	atexit(remove_shm);

	trace_bits = shmat(shm_id, NULL, 0);

	if (trace_bits == (void *)-1)
	{
		die_sys("shmat() failed");
	}

	child_pid = fork();

	if (child_pid < 0)
	{
		die_sys("fork() failed");
	}

	if (!child_pid)
	{
		if (dup2(ctl_pipe[0], CTL_FD) < 0 || dup2(st_pipe[1], ST_FD) < 0)
		{
			die_sys("dup() failed");
		}

		close(ctl_pipe[0]);
		close(ctl_pipe[1]);
		close(st_pipe[0]);
		close(st_pipe[1]);

		char shm_str[12];
		sprintf(shm_str, "%d", shm_id);

		if (setenv(SHM_ID_VAR, shm_str, 1))
		{
			die_sys("setenv() failed");
		}

		if (target_arg)
		{
			execlp(target_path, "", target_arg, NULL);
		}
		else
		{
			execlp(target_path, "", NULL);
		}

		die_sys("execlp() failed");
	}
	else
	{
		close(ctl_pipe[0]);
		close(st_pipe[1]);

		ctl_fd = ctl_pipe[1];
		st_fd = st_pipe[0];

		int result;
		int status;

		while ((result = read(st_fd, &status, LEN_FLD_SIZE)) == -1 && errno == EINTR)
		{
			continue;
		}

		if (result != LEN_FLD_SIZE)
		{
			die_sys("read() failed");
		}
	}

	return 0;
}

int LLVMFuzzerTestOneInput(const uint8_t *data, size_t size)
{
	if (size > DATA_SIZE)
	{
		die("Size of the input data must not exceed 1 MiB.");
	}

	memset(trace_bits, 0, MAP_SIZE);
	memcpy(trace_bits + MAP_SIZE, data, size);

	int result;

	while ((result = write(ctl_fd, &size, LEN_FLD_SIZE)) == -1 && errno == EINTR)
	{
		continue;
	}

	if (result != LEN_FLD_SIZE)
	{
		die_sys("write() failed");
	}

	int status;

	while ((result = read(st_fd, &status, LEN_FLD_SIZE)) == -1 && errno == EINTR)
	{
		continue;
	}

	if (result != LEN_FLD_SIZE)
	{
		die_sys("read() failed");
	}

	memcpy(extra_counters, trace_bits, MAP_SIZE);

	if (status)
	{
		__builtin_trap();
	}

	return 0;
}
