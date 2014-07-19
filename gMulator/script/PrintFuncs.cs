using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using gMulator;
using GSharp;

namespace gMulator.script {
	public class PrintFuncs : IModule {
		public void Load(Emulator Emul, IntPtr L, dynamic G) {
			G.Msg = new LuaFunc((LL) => {
				int ArgC = Lua.GetTop(LL);
				for (int i = ArgC; i >= 1; i--) {
					int Ty = Lua.Type(LL, -i);

					// TODO: Fix fucking metatable __tostring
					/*if (Ty != Lua.TSTRING && Lua.GetMetatable(LL, -i)) {
						Lua.GetField(LL, -i, "__tostring");
						if (Lua.Type(LL, -1) != Lua.TNIL) {
							Lua.Call(LL, 0, 1);
							Console.Write(Lua.ToString(LL, -1));
							Lua.Pop(LL, 2);
						} else {
							Lua.Pop(LL, 2);
							goto DO_SWITCH;
						}
						goto END_SWITCH;
					} else
						goto DO_SWITCH;*/

				DO_SWITCH:
					switch (Ty) {
						case Lua.TNUMBER:
						case Lua.TSTRING:
							Console.Write(Lua.ToString(LL, -i));
							break;
						case Lua.TNIL:
							Console.Write("nil");
							break;
						case Lua.TBOOLEAN:
							Console.WriteLine(Lua.ToBoolean(LL, -i));
							break;
						default:
							Console.Write("{0}: 0x{1:x8}", Lua.TypeName(LL, Ty), Lua.ToPointer(LL, -i).ToInt32());
							break;
					}

				END_SWITCH:
					if (i != 1)
						Console.Write("    ");
				}
				return 0;
			});

			G.print = new LuaFunc((LL) => {
				G.Msg(LL);
				Console.WriteLine();
				return 0;
			});
		}

	}
}