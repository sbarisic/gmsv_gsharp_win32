using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using GSharp;

namespace GSharpTest {
	unsafe class Program {
		static IntPtr L;

		static void ErrorCheck(int Ret) {
			if (Ret != 0)
				Console.WriteLine("error: {0}", Lua.ToString(L, -1));
		}

		static void Main(string[] args) {
			Console.Title = "GSharp Test";
			Console.WriteLine("GShap Test");

			L = Lua.NewState();
			Lua.OpenLibs(L);

			LuaFunc AtPanicOld = null;
			AtPanicOld = Lua.AtPanic(L, (LL) => {
				AtPanicOld(LL);
				throw new Exception();
			});

			Lua.PushCFunction(L, (LL) => {
				int Top = Lua.GetTop(LL);
				StringBuilder SB = new StringBuilder();
				for (int i = 1; i < Top + 1; i++)
					SB.Append(Lua.ToString(LL, i)).Append("\t");
				Console.WriteLine(SB.ToString().Trim());
				return 0;
			});
			Lua.SetGlobal(L, "print");

			Lua.PushCFunction(L, (LL) => {
				Lua.Close(LL);
				Environment.Exit(0);
				return 0;
			});
			Lua.SetGlobal(L, "quit");

			Lua.PushCFunction(L, (LL) => {
				Lua.Close(LL);
				Main(args);
				return 0;
			});
			Lua.SetGlobal(L, "restart");



			while (true) {
				Console.Write(">> ");
				try {
					ErrorCheck(Lua.LoadString(L, Console.ReadLine()));
					ErrorCheck(Lua.PCall(L, 0, 0, 0));
				} catch (Exception E) {
					Console.WriteLine("exception: {0}", E.Message);
				}

			}
		}
	}
}