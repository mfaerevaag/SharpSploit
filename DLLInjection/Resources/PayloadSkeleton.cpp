#include <tchar.h>
#include <Windows.h>
#pragma comment (lib, "User32.lib")

static volatile int exitThreads = 0;
static HANDLE hThread = NULL;

DWORD WINAPI PayloadThread(LPVOID)
{
	HANDLE  hStdout = NULL;
	if ((hStdout = GetStdHandle(STD_OUTPUT_HANDLE)) == INVALID_HANDLE_VALUE) {
		return 1;
	}

	while (!exitThreads)
	{
		REPLACE_BODY

		Sleep(REPLACE_SLEEP);
	}

	return 0;
}

BOOL WINAPI DllMain(
	HINSTANCE hinstDLL,  // handle to DLL module
	DWORD fdwReason,     // reason for calling function
	LPVOID lpReserved)   // reserved
{
	DWORD threadId;

	// Perform actions based on the reason for calling.
	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		// Initialize once for each new process.
		// Return FALSE to fail DLL load.
		hThread = CreateThread(NULL, 0, &PayloadThread, NULL, 0, &threadId);
		if (!hThread)
			return false;
		break;

	case DLL_THREAD_ATTACH:
		// Do thread-specific initialization.
		break;

	case DLL_THREAD_DETACH:
		// Do thread-specific cleanup.            
		break;

	case DLL_PROCESS_DETACH:
		// Perform any necessary cleanup.
		exitThreads = 1;
		if (hThread) {
			WaitForSingleObject(hThread, INFINITE);
			hThread = NULL;
		}
		break;
	}

	return TRUE;
}