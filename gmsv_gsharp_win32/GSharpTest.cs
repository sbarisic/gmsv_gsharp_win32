using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using RGiesecke.DllExport;
using System.Threading;
using System.Runtime.CompilerServices;
using dotOmegle;

namespace gmsv_gsharp_win32 {
	public static class Program {
		public static int Open(IntPtr L) {
			dynamic _G = new LuaObject(L);

			_G.Thread = new LuaTable(L);
			_G.Thread.Spawn = new LuaFunc((LL) => {
				IntPtr K = Lua.NewThread(LL);
				Lua.Pop(LL);
				Lua.XMove(LL, K, 2);
				new Thread(() => {
					if (Lua.PCall(K, 0, 0, 0) != 0)
						GMod.Print(Lua.ToString(K, -1));
				}).Start();
				return 0;
			});

			_G.Thread.Sleep = new LuaFunc((LL) => {
				int N = (int)Lua.ToNumber(LL, -1);
				Thread.Sleep(N);
				return 0;
			});

			List<string> StrList = new List<string>();

			_G.Thread.Print = new LuaFunc((LL) => {
				StrList.Add(Lua.ToString(LL, -1));
				return 0;
			});

			_G.Thread.Flush = new LuaFunc((LL) => {
				foreach (var S in StrList)
					Console.WriteLine(S);
				StrList.Clear();
				return 0;
			});

			GMod.Print("G# Loaded!");
			return 0;
		}

		public static int Close(IntPtr L) {
			GMod.Print("G# Unloaded!");
			return 0;
		}
	}
}