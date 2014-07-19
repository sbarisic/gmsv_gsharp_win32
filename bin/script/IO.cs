using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using gMulator;
using GSharp;

namespace gMulator.script {
	public class Modules : IModule {
		public void Load(Emulator Emul, IntPtr L, dynamic G) {
			G.dofile = new LuaFunc((LL) => {
				Lua.CheckType(LL, -1, Lua.TSTRING);
				string Src = "";
				try {
					Src = File.ReadAllText(Lua.ToString(LL, -1));
				} catch (Exception E) {
					Lua.Error(LL, E.Message);
					return 0;
				}
				Lua.LoadString(LL, Src);
				if (Lua.PCall(LL, 0, 0, 0) != 0)
					Lua.Error(LL, Lua.ToString(LL, -1));
				return 0;
			});

			G.exit = new LuaFunc((LL) => {
				Environment.Exit(Lua.OptInt(LL, -1, 0));
				return 0;
			});
		}
	}
}