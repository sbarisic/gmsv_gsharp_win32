using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using gMulator;
using GSharp;

namespace gMulator.script {
	public class Modules : IModule {
		[DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpLibFileName);
		[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
		static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string lpProcName);
		[DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
		static extern bool FreeLibrary(IntPtr hModule);

		static List<int> ModHandles = new List<int>();

		public void Load(Emulator Emul, IntPtr L, dynamic G) {
			G.require = new LuaFunc((LL) => {
				Lua.CheckType(LL, -1, Lua.TSTRING);
				string ModName = Path.GetFileNameWithoutExtension(Lua.ToString(LL, -1));

				if (File.Exists(ModName)) {
				} else if (File.Exists(ModName + ".dll"))
					ModName += ".dll";
				else if (File.Exists(ModName + "_win32.dll"))
					ModName += "_win32.dll";
				else if (File.Exists("gmcl_" + ModName + "_win32.dll"))
					ModName = "gmcl_" + ModName + "_win32.dll";
				else if (File.Exists("gmsv_" + ModName + "_win32.dll"))
					ModName = "gmsv_" + ModName + "_win32.dll";
				else {
					Lua.Error(LL, "Module '" + ModName + "' could not be found");
					return 0;
				}

				if (!ModName.EndsWith("_win32.dll"))
					return 0;

				IntPtr Module = LoadLibrary(ModName);
				if (Module == IntPtr.Zero) {
					Lua.Error(LL, "Module '" + ModName + "' could not be loaded");
					return 0;
				}

				IntPtr gmod13_open_addr = GetProcAddress(Module, "gmod13_open");
				if (gmod13_open_addr == IntPtr.Zero) {
					Lua.Error(LL, "gmod13_open entry point not found in '" + ModName + "'");
					return 0;
				}

				if (ModHandles.Contains(Module.ToInt32())) {
					Emul.Print(LL, "Skipping '{0}' 0x{1:X}", ModName, Module.ToInt32());
					Lua.PushInteger(LL, Module.ToInt32());
					return 1;
				}
				Emul.Print(LL, "Loading '{0}' 0x{1:X}", ModName, Module.ToInt32());
				ModHandles.Add(Module.ToInt32());
				try {
					LuaFunc gmod13_open = (LuaFunc)Marshal.GetDelegateForFunctionPointer(gmod13_open_addr, typeof(LuaFunc));
					Lua.PushInteger(LL, Module.ToInt32());
					int R = gmod13_open(LL);
					if (R != -1)
						return R + 1;
					return R;
				} catch (Exception E) {
					Lua.Error(LL, "[ERROR] Module failure\n" + E.Message);
				}

				return 0;
			});

			G.unrequire = new LuaFunc((LL) => {
				int MNum = (int)Lua.CheckInt(LL, -1);
				if (ModHandles.Contains(MNum))
					ModHandles.Remove(MNum);
				else
					return 0;

				IntPtr ModH = new IntPtr(MNum);
				IntPtr gmod13_close_addr = GetProcAddress(ModH, "gmod13_close");

				if (gmod13_close_addr != IntPtr.Zero) {
					LuaFunc gmod13_close = (LuaFunc)Marshal.GetDelegateForFunctionPointer(gmod13_close_addr, typeof(LuaFunc));
					gmod13_close(LL);
				} else
					Emul.Print(LL, "[WARNING] gmod13_close not found in 0x{0:X}", MNum);


				if (FreeLibrary(ModH)) {
					Emul.Print(LL, "Module 0x{0:X} free'd", MNum);
				} else
					Emul.Print(LL, "Failed 0x{0:X}", Marshal.GetLastWin32Error());
				return 0;
			});

			G.require_all = new LuaFunc((LL) => {
				string[] Modules = Directory.GetFiles(".", "*.dll");
				foreach (var Module in Modules) {
					Lua.GetGlobal(LL, "require");
					Lua.PushString(LL, Module);
					Emul.ErrorCheck(LL, Lua.PCall(L, 1, 0, 0));
				}
				return 0;
			});

			G.unrequire_all = new LuaFunc((LL) => {
				int[] MHand = ModHandles.ToArray();
				foreach (var M in MHand) {
					Lua.GetGlobal(LL, "unrequire");
					Lua.PushInteger(LL, M);
					Emul.ErrorCheck(LL, Lua.PCall(L, 1, 0, 0));
				}
				return 0;
			});
		}
	}
}