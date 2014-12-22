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

using GSharp;
using GSharp.Dynamic;

namespace gmsv_gsharp_win32 {
	public static class Program {
		public static dynamic print;

		public static void F(string Str, double D) {
			print(string.Format("String: {0}\nDouble: {1}", Str, D));
		}

		public static double add(double A, double B) {
			return A + B;
		}

		public static void wotwat() {
			print("Hello wotwat world!");
		}

		[DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
		public static int Open(IntPtr L) {
			dynamic Env = new LuaState(L);
			print = Env.print;

			Env.func = new Action<string, double>(F);
			Env.add = new Func<double, double, double>(add);

			Env.array = new double[] { 2, 3, Env.add(1, 3) };
			Env.dict = new Dictionary<string, Action>() {
				{ "wotwat", wotwat }
			};

			print("GSharp loaded!");
			return 0;
		}

		[DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
		public static int Close(IntPtr L) {
			return 0;
		}
	}
}