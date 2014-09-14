module dllmain;

import std.c.windows.windows;
import core.sys.windows.dll;

import std.stdio;
import std.c.windows.windows;

__gshared HINSTANCE g_hInst;

extern (Windows) BOOL DllMain(HINSTANCE hInstance, ULONG ulReason, LPVOID pvReserved) {
	final switch (ulReason) {
		case DLL_PROCESS_ATTACH:
			g_hInst = hInstance;
			dll_process_attach( hInstance, true );
			break;

		case DLL_PROCESS_DETACH:
			dll_process_detach( hInstance, true );
			break;

		case DLL_THREAD_ATTACH:
			dll_thread_attach( true, true );
			break;

		case DLL_THREAD_DETACH:
			dll_thread_detach( true, true );
			break;
	}

	return true;
}

alias extern (C) int function(void*) luaFunc;
alias immutable(char)* cStr;

extern (C) export int gmod13_open(void* L) {
	HMODULE dll = LoadLibraryA("lua_shared.dll");

	extern (C) void function(void*, int, cStr) lua_getfield =
		cast(void function(void*, int, cStr))GetProcAddress(dll, "lua_getfield");
	extern (C) void function(void*, cStr) lua_pushstring =
		cast(void function(void*, cStr))GetProcAddress(dll, "lua_pushstring");
	extern (C) void function(void*, luaFunc, int) lua_pushcclosure =
		cast(void function(void*, luaFunc, int))GetProcAddress(dll, "lua_pushcclosure");
	extern (C) void function(void*, int) lua_settable = cast(void function(void*, int))GetProcAddress(dll, "lua_settable");

	lua_getfield(L, -10002, ("_G").ptr);
	lua_pushstring(L, ("DTest").ptr);
	lua_pushcclosure(L, function(LL) {
		writeln("Hello D world!");
		return 0;
	}, 0);
	lua_settable(L, -3);

	return 0;
}

extern (C) export int gmod13_close(void* L) {
	return 0;
}