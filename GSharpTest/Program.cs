using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using GSharp;
using GSharp.Dynamic;

namespace GSharpTest {
	public unsafe class Program {
		static IntPtr L;

		static void ErrorCheck(int Ret) {
			if (Ret != 0)
				throw new Exception(Lua.ToString(L, -1));
		}

		static Func<string, double, double> Act = null;
		public static int LuaF(IntPtr L) {
			Lua.Push(L, Act(Lua.CheckString(L, -2, IntPtr.Zero), Lua.CheckNumber(L, -1)));
			return 0;
		}

		public static void F(string Str, double D, int I) {
			Console.WriteLine("String: {0}\nDouble: {1}\nInt: {2}", Str, D, I);
		}

		public static double add(double A, double B) {
			return A + B;
		}

		public static void wotwat() {
			Console.WriteLine("Hello wotwat world!");
		}

		static void Init() {
			dynamic Env = new LuaState(L);

			Env.func = new Action<string, double, int>(F);
			Env.add = new Func<double, double, double>(add);

			Env.array = new double[] { 2, 3, Env.add(1, 3) };
			Env.dict = new Dictionary<string, Action>() {
				{ "wotwat", wotwat }
			};
		}

		static void Main(string[] args) {
			Console.Title = "GSharp Test";
			//Console.WriteLine("GShap Test");

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

			ErrorCheck(Lua.LoadString(L, "function helloworld() print(\"Hello World!\") return helloworld end"));
			ErrorCheck(Lua.PCall(L, 0, 0, 0));
			ErrorCheck(Lua.LoadString(L, "function printt(t) for k,v in pairs(t) do print(tostring(k) .. \" - \" .. tostring(v)) end end"));
			ErrorCheck(Lua.PCall(L, 0, 0, 0));

			//try {
				Init();
			/*} catch (Exception) {
				throw;
			}//*/
			GSharp.Dynamic.Delegates.Dump();
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