using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

namespace GSharp {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int LuaFunc(IntPtr L);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int LuaWriter(IntPtr L, IntPtr Data, int Size, IntPtr P);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IntPtr LuaReader(IntPtr L, IntPtr Data, IntPtr Size);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IntPtr LuaAlloc(IntPtr UD, IntPtr Ptr, int OSize, int NSize);

	public static unsafe class Lua {
		static List<LuaFunc> LuaFuncs = new List<LuaFunc>();
		const string LIBNAME = "lua_shared.dll";
		const CallingConvention CConv = CallingConvention.Cdecl;
		const CharSet CSet = CharSet.Auto;

		public const int TNONE = -1;
		public const int TNIL = 0;
		public const int TBOOLEAN = 1;
		public const int TLIGHTUSERDATA = 2;
		public const int TNUMBER = 3;
		public const int TSTRING = 4;
		public const int TTABLE = 5;
		public const int TFUNCTION = 6;
		public const int TUSERDATA = 7;
		public const int TTHREAD = 8;

		public const int GCSTOP = 0;
		public const int GCRESTART = 1;
		public const int GCCOLLECT = 2;
		public const int GCCOUNT = 3;
		public const int GCCOUNTB = 4;
		public const int GCSTEP = 5;
		public const int GCSETPAUSE = 6;
		public const int GCSETSTEPMUL = 7;

		public const int REGISTRYINDEX = -10000;
		public const int ENVIRONINDEX = -10001;
		public const int GLOBALSINDEX = -10002;

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_getmetafield")]
		public static extern int GetMetaField(IntPtr State, int I, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_getmetafield")]
		public static extern int GetMetaField(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_callmeta")]
		public static extern int CallMeta(IntPtr State, int I, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_callmeta")]
		public static extern int CallMeta(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_newstate")]
		public static extern IntPtr NewState();

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_newstate")]
		public static extern IntPtr NewState(LuaAlloc F, IntPtr UD);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_openlibs")]
		public static extern void OpenLibs(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_base")]
		public static extern int OpenBase(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_bit")]
		public static extern int OpenBit(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_debug")]
		public static extern int OpenDebug(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_jit")]
		public static extern int OpenJit(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_math")]
		public static extern int OpenMath(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_os")]
		public static extern int OpenOS(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_package")]
		public static extern int OpenPackage(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_string")]
		public static extern int OpenString(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_table")]
		public static extern int OpenTable(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_typerror")]
		public static extern int TypeError(IntPtr State, int I, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_typerror")]
		public static extern int TypeError(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_argerror")]
		public static extern int ArgError(IntPtr State, int I, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_argerror")]
		public static extern int ArgError(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checklstring")]
		public static extern IntPtr CheckLString(IntPtr State, int I, int L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optlstring")]
		public static extern IntPtr OptLString(IntPtr State, int I, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checknumber")]
		public static extern double CheckNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optnumber")]
		public static extern double OptNumber(IntPtr State, int I, double Def);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkinteger")]
		public static extern int CheckInt(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optinteger")]
		public static extern int OptInt(IntPtr State, int I, int Def);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkstack")]
		public static extern int CheckStack(IntPtr State, int Extra, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checktype")]
		public static extern void CheckType(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkany")]
		public static extern void CheckAny(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_newmetatable")]
		public static extern int NewMetatable(IntPtr State, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkudata")]
		public static extern IntPtr CheckUData(IntPtr State, int I, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_where")]
		public static extern void Where(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_error")]
		public static extern int Error(IntPtr State, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_error")]
		public static extern int Error(IntPtr State, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_ref")]
		public static extern int Ref(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_unref")]
		public static extern void Unref(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadfile")]
		public static extern int LoadFile(IntPtr State, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadfile")]
		public static extern int LoadFile(IntPtr State, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadbuffer")]
		public static extern int LoadBuffer(IntPtr State, string S, int I, string S2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadstring")]
		public static extern int LoadString(IntPtr State, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadstring")]
		public static extern int LoadString(IntPtr State, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_gsub")]
		public static extern IntPtr GSub(IntPtr State, string S, string S2, string S3);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_findtable")]
		public static extern IntPtr FindTable(IntPtr State, int I, string S, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_close")]
		public static extern void Close(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_newthread")]
		public static extern IntPtr NewThread(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_atpanic")]
		[return: MarshalAs(UnmanagedType.FunctionPtr)]
		public static extern LuaFunc AtPanic(IntPtr State, LuaFunc F);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gettop")]
		public static extern int GetTop(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_settop")]
		public static extern void SetTop(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushvalue")]
		public static extern void PushValue(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_remove")]
		public static extern void Remove(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_insert")]
		public static extern void Insert(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_replace")]
		public static extern void Replace(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_xmove")]
		public static extern void XMove(IntPtr State, IntPtr L, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isnumber")]
		public static extern bool IsNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isstring")]
		public static extern bool IsString(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_iscfunction")]
		public static extern bool IsCFunction(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isuserdata")]
		public static extern bool IsUserdata(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_type")]
		public static extern int Type(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_typename")]
		static extern IntPtr _TypeName(IntPtr State, int I);

		public static string TypeName(IntPtr State, int I = TNONE) {
			return Marshal.PtrToStringAnsi(_TypeName(State, I));
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_equal")]
		public static extern int Equal(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawequal")]
		public static extern int RawEqual(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_lessthan")]
		public static extern int LessThan(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_load")]
		public static extern int Load(IntPtr State, LuaReader R, IntPtr Data, IntPtr ChunkName);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_load")]
		public static extern int Load(IntPtr State, LuaReader R, IntPtr Data, [MarshalAs(UnmanagedType.AnsiBStr)]string ChunkName);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tonumber")]
		public static extern double ToNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tointeger")]
		public static extern int ToInteger(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_toboolean")]
		public static extern bool ToBoolean(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tolstring")]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr ToLString(IntPtr State, int I, IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_objlen")]
		public static extern int ObjLen(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tocfunction")]
		public static extern LuaFunc ToCFunction(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_touserdata")]
		public static extern IntPtr ToUserdata(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tothread")]
		public static extern IntPtr ToThread(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_topointer")]
		public static extern IntPtr ToPointer(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushnil")]
		public static extern void PushNil(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushnumber")]
		public static extern void PushNumber(IntPtr State, double D);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushinteger")]
		public static extern void PushInteger(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushlstring")]
		public static extern void PushLString(IntPtr State, string S, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushstring")]
		public static extern void PushString(IntPtr State, IntPtr S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushstring")]
		public static extern void PushString(IntPtr State, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushcclosure")]
		static extern void _PushCClosure(IntPtr State, LuaFunc F, int N = 0);

		public static void PushCClosure(IntPtr State, LuaFunc F, int N = 0) {
			if (!LuaFuncs.Contains(F))
				LuaFuncs.Add(F);
			_PushCClosure(State, F, N);
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushboolean")]
		public static extern void PushBoolean(IntPtr State, int B);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushlightuserdata")]
		public static extern void PushLightUserdata(IntPtr State, IntPtr P);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushthread")]
		public static extern int PushThread(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gettable")]
		public static extern void GetTable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getfield")]
		public static extern void GetField(IntPtr State, int I, IntPtr S);

		public static void GetGlobal(IntPtr State, IntPtr S) {
			GetField(State, GLOBALSINDEX, S);
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getfield")]
		public static extern void GetField(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		public static void GetGlobal(IntPtr State, string S) {
			GetField(State, GLOBALSINDEX, S);
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawget")]
		public static extern void RawGet(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawgeti")]
		public static extern void RawGetI(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_createtable")]
		public static extern void CreateTable(IntPtr State, int NArr, int NRec);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_newuserdata")]
		public static extern IntPtr NewUserdata(IntPtr State, int Size);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getmetatable")]
		public static extern int GetMetatable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getfenv")]
		public static extern void GetFEnv(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_dump")]
		public static extern int Dump(IntPtr State, LuaWriter Writer, IntPtr P);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_settable")]
		public static extern void SetTable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getallocf")]
		[return: MarshalAs(UnmanagedType.FunctionPtr)]
		public static extern LuaAlloc GetAllocF(IntPtr State, IntPtr UD);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setfield")]
		public static extern void SetField(IntPtr State, int I, IntPtr S);

		public static void SetGlobal(IntPtr State, IntPtr S) {
			SetField(State, GLOBALSINDEX, S);
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setfield")]
		public static extern void SetField(IntPtr State, int I, [MarshalAs(UnmanagedType.AnsiBStr)]string S);

		public static void SetGlobal(IntPtr State, string S) {
			SetField(State, GLOBALSINDEX, S);
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawset")]
		public static extern void RawSet(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawseti")]
		public static extern void RawSetI(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setmetatable")]
		public static extern int SetMetatable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setfenv")]
		public static extern int SetFEnv(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_call")]
		public static extern void Call(IntPtr State, int NArgs, int NResults);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pcall")]
		public static extern int PCall(IntPtr State, int NArgs, int NResults, int ErrorFunc);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_cpcall")]
		public static extern int CPCall(IntPtr State, LuaFunc F, IntPtr P);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_yield")]
		public static extern int Yield(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_resume_real")]
		public static extern int Resume(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_status")]
		public static extern int Status(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gc")]
		public static extern int GC(IntPtr State, int What, int Data);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_error")]
		public static extern int Error(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_next")]
		public static extern int Next(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_concat")]
		public static extern void Concat(IntPtr State, int N);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getupvalue")]
		public static extern IntPtr GetUpValue(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gethookmask")]
		public static extern int GetHookMask(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gethookcount")]
		public static extern int GetHookCount(IntPtr State);

		// Custom

		public static void NewTable(IntPtr State) {
			CreateTable(State, 0, 0);
		}

		public static bool Is(IntPtr State, int Idx, int Type) {
			return Lua.Type(State, Idx) == Type;
		}

		public static bool IsNoneOrNil(IntPtr State, int Idx) {
			return Is(State, Idx, TNONE) || Is(State, Idx, TNIL);
		}

		public static string ToString(IntPtr State, int I) {
			return Marshal.PtrToStringAnsi(Lua.ToLString(State, I, IntPtr.Zero));
		}

		public static void Pop(IntPtr State, int I = 1) {
			SetTop(State, -(I) - 1);
		}

		public static void PushCFunction(IntPtr State, LuaFunc F) {
			PushCClosure(State, F, 0);
		}

		public static void Push(IntPtr L, object O) {
			if (O == null)
				Lua.PushNil(L);
			else if (O is string)
				Lua.PushString(L, O.ToString());
			else if (O is int)
				Lua.PushInteger(L, (int)O);
			else if (O is float)
				Lua.PushNumber(L, (double)(float)O);
			else if (O is double)
				Lua.PushNumber(L, (double)O);
			else if (O is LuaFunc)
				Lua.PushCFunction(L, (LuaFunc)O);
			else if (O is bool)
				Lua.PushBoolean(L, (bool)O ? 1 : 0);
			else if (O is Dynamic.LuaObject)
				((Dynamic.LuaObject)O).Deref();
			else if (O is Array) {
				Array A = (Array)O;
				Lua.CreateTable(L, A.Length, 0);
				for (int i = 0; i < A.Length; i++) {
					Push(L, i);
					Push(L, A.GetValue(i));
					Lua.SetTable(L, -3);
				}
			} else if (O is IDictionary) {
				IDictionary D = (IDictionary)O;
				Lua.CreateTable(L, 0, D.Count);
				foreach (var K in D.Keys) {
					Push(L, K);
					Push(L, D[K]);
					Lua.SetTable(L, -3);
				}
			} else
				throw new Exception("Invalid type " + O.GetType().FullName);
		}

		public static T To<T>(IntPtr L, int I = -1) {
			return (T)To(L, I);
		}

		public static object To(IntPtr L, int I = -1) {
			int T = Lua.Type(L, I);

			switch (T) {
				case TSTRING:
					return Lua.ToString(L, I);
				case TNUMBER:
					return Lua.ToNumber(L, I);
				case TFUNCTION: {
						if (!IsCFunction(L, I))
							return new Dynamic.LuaObject(L).Deref();
						LuaFunc LF = Lua.ToCFunction(L, I);
						if (!LuaFuncs.Contains(LF))
							LuaFuncs.Add(LF);
						return LF;
					}
				case TBOOLEAN:
					return Lua.ToBoolean(L, I);
				case TNIL:
					return null;
				case TTABLE:
					return new Dynamic.LuaObject(L).Deref();
			}

			return null;
		}

		public static void RegisterCFunction(IntPtr State, string TableName, string FuncName, LuaFunc F) {
			GetField(State, GLOBALSINDEX, TableName);
			if (!Is(State, -1, TTABLE)) {
				CreateTable(State, 0, 1);
				SetField(State, GLOBALSINDEX, TableName);
				Pop(State);

				GetField(State, GLOBALSINDEX, TableName);
			}

			PushString(State, FuncName);
			PushCFunction(State, F);
			SetTable(State, -3);
			Pop(State);
		}

		public static string[] GetStack(IntPtr State) {
			int Cnt = Lua.GetTop(State);
			List<string> R = new List<string>();

			for (int i = Cnt; i >= 1; i--)
				R.Add(string.Format("{0}, {1} = {2}", -i, Cnt - i + 1, Lua.TypeName(State, Lua.Type(State, -i))));

			return R.ToArray();
		}

		public static void PrintStack(IntPtr State, string Title = "") {
			string[] St = GetStack(State);
			Console.WriteLine("{0} {1}", Title, "{");
			foreach (var S in St)
				Console.WriteLine("   {0}", S);
			Console.WriteLine("{0}", "}");
		}
	}

	public static unsafe class GMod {
		private static IntPtr State;
		public static object Lock = new object();

		public static void Init(IntPtr State) {
			GMod.State = State;
		}

		public static void Print(object O) {
			lock (Lock) {
				Lua.GetGlobal(State, "print");
				Lua.PushString(State, O != null ? O.ToString() : "NULL");
				Lua.Call(State, 1, 0);
			}
		}

		public static void MsgC(int R, int G, int B, string Msg) {
			lock (Lock) {
				Lua.GetField(State, Lua.GLOBALSINDEX, "MsgC");

				Lua.CreateTable(State, 0, 3);
				Lua.PushString(State, "r");
				Lua.PushNumber(State, R);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "g");
				Lua.PushNumber(State, G);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "b");
				Lua.PushNumber(State, B);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "a");
				Lua.PushNumber(State, 255);
				Lua.SetTable(State, -3);

				Lua.PushString(State, Msg);
				Lua.Call(State, 2, 0);
			}
		}
	}
}