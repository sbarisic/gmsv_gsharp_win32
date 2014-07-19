using GSharp;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace gMulator {
	public interface IModule {
		void Load(Emulator E, IntPtr L, dynamic G);
	}

	public class Emulator {
		public bool ErrorCheck(IntPtr L, int I) {
			if (I != 0) {
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(Lua.ToString(L, -1));
				Lua.Pop(L);
				Console.ResetColor();
				return true;
			}
			return false;
		}

		public void Print(IntPtr L, object F, params object[] O) {
			string S = F.ToString();
			if (O != null)
				S = string.Format(S, O);
			GMod.Print(S);
		}
	}

	class Program {
		const string Prompt = "\n>>";

		static void ExecFile(string Path, IntPtr L, LuaObject G, Emulator Emul) {
			var CSC = new CSharpCodeProvider();
			var Params = new CompilerParameters(new[] { "mscorlib.dll", "System.dll", "System.Core.dll", "Microsoft.CSharp.dll", "gMulator.exe" });
			Params.GenerateExecutable = false;
			var Res = CSC.CompileAssemblyFromSource(Params, File.ReadAllText(Path));
			Res.Errors.Cast<CompilerError>().ToList().ForEach(error => {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(error.ErrorText);
				Console.ResetColor();
			});

			if (Res.Errors.Count > 0)
				return;

			var AsmTypes = Res.CompiledAssembly.GetTypes();
			foreach (var T in AsmTypes) {
				var Typ = T.GetInterface("gMulator.IModule");
				if (Typ != null) {
					IModule Mod = (IModule)Res.CompiledAssembly.CreateInstance(T.FullName);
					if (Mod != null)
						Mod.Load(Emul, L, G);
				}
			}
		}

		static void Main(string[] args) {
			IntPtr L = Lua.NewState();
			Lua.OpenLibs(L);
			Lua.AtPanic(L, (LL) => {
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("\n\nPANIC!");
				Console.ResetColor();
				Console.WriteLine(Lua.ToString(LL, -1));
				Console.ReadKey();
				Environment.Exit(0);
				return 0;
			});
			GMod.Init(L);
			dynamic G = new LuaObject(L);
			Emulator Emul = new Emulator();

			FileSystemWatcher CSWatch = new FileSystemWatcher("script", "*.cs");
			CSWatch.Changed += (S, E) => {
				ExecFile(E.FullPath, L, G, Emul);
				Console.WriteLine(Prompt);
			};
			CSWatch.IncludeSubdirectories = true;
			CSWatch.EnableRaisingEvents = true;

			string[] CSFiles = Directory.GetFiles("script", "*.cs");
			foreach (var CSFile in CSFiles)
				ExecFile(CSFile, L, G, Emul);

			if (File.Exists("script/autorun.lua")) {
				FileSystemWatcher FSW = new FileSystemWatcher("script", "autorun.lua");
				FSW.Changed += (S, E) => {
					Console.Clear();
					Lua.GetGlobal(L, "dofile");
					Lua.PushString(L, "script/autorun.lua");
					Emul.ErrorCheck(L, Lua.PCall(L, 1, 0, 0));
					Console.Write(Prompt);
				};
				FSW.EnableRaisingEvents = true;

				Lua.GetGlobal(L, "dofile");
				Lua.PushString(L, "script/autorun.lua");
				Emul.ErrorCheck(L, Lua.PCall(L, 1, 0, 0));
			}

			G.test = new LuaFunc((LL) => {

				Lua.PrintStack(LL, "L");

				return 0;
			});

			while (true) {
				Console.Write(Prompt);
				if (!Emul.ErrorCheck(L, Lua.LoadString(L, Console.ReadLine())))
					try {
						Emul.ErrorCheck(L, Lua.PCall(L, 0, 0, 0));
					} catch (Exception E) {
						Console.WriteLine("\n[{0}]\n{1}", E.GetType().Name, E.Message);
					}
			}
		}
	}
}