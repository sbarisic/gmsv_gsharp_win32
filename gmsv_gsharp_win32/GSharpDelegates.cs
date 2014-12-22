using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using CCnv = System.Runtime.InteropServices.CallingConvention;

namespace GSharp.Dynamic {
	static class Delegates {
		internal static AssemblyBuilder AB;
		internal static ModuleBuilder DefMod;
		internal static Dictionary<string, Type> DelegateTypes;
		internal static Dictionary<string, Type> DelegateInvokers;

		static Delegates() {
			AB = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("GSharp.Dynamic.Delegates.dll"),
			AssemblyBuilderAccess.RunAndSave);
			DefMod = AB.DefineDynamicModule("GSharp.Dynamic.Delegates.Module", "GSharp.Dynamic.Delegates.Module.dll");

			DelegateTypes = new Dictionary<string, Type>();
			DelegateInvokers = new Dictionary<string, Type>();
		}

		public static void Dump() {
			AB.Save("GSharp.Dynamic.dll");
		}

		public static string CreateDelegateName(Type ReturnType, Type[] Args) {
			string Name = "Delegate?" + ReturnType.Name + "??";
			foreach (var PType in Args)
				Name += PType.Name;
			return Name;
		}

		public static Delegate ToDelegate(this MethodInfo MI) {
			if (MI == null)
				throw new ArgumentException("MethodInfo MI cannot be null", "MI");
			return Delegate.CreateDelegate(CreateDelegateType(MI.ReturnType, MI.GetParamTypes()), MI);
		}

		public static Type[] GetParamTypes(this MethodInfo MI) {
			ParameterInfo[] PI = MI.GetParameters();
			List<Type> Types = new List<Type>();
			if (MI.CallingConvention.HasFlag(CallingConventions.HasThis))
				Types.Add(MI.ReflectedType);
			for (int i = 0; i < PI.Length; i++)
				Types.Add(PI[i].ParameterType);
			return Types.ToArray();
		}

		public static LuaFunc CreateLuaDelegateInvoker(this Delegate D, IntPtr L) {
			Type[] Params = D.Method.GetParamTypes();
			Type Ret = D.Method.ReturnType;

			string DelegateName = CreateDelegateName(Ret, Params);
			if (DelegateInvokers.ContainsKey(DelegateName)) {
				MethodInfo MI = DelegateInvokers[DelegateName].GetMethod("Invoker");
				return (LuaFunc)Delegate.CreateDelegate(typeof(LuaFunc), MI);
			}

			TypeBuilder TB = DefMod.DefineType(DelegateName, TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
				TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Abstract);
			TB.SetCustomAttribute(new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor
				(new[] { typeof(CCnv) }), new object[] { CCnv.Cdecl }));

			MethodBuilder InvB = TB.DefineMethod("Invoker", MethodAttributes.HideBySig | MethodAttributes.Public |
				MethodAttributes.Static, typeof(int), new Type[] { typeof(IntPtr) });

			ILGenerator ILGen = InvB.GetILGenerator();
			if (Ret != typeof(void))
				ILGen.Emit(OpCodes.Ldarg_0);

			for (int i = 1; i < Params.Length + 1; i++)
				Check(ILGen, Params[i - 1], i);
			ILGen.Emit(OpCodes.Call, D.Method);

			if (Ret == typeof(void))
				ILGen.Emit(OpCodes.Ldc_I4_0);
			else {
				ILGen.Emit(OpCodes.Box, Ret);
				ILGen.Emit(OpCodes.Call, typeof(Lua).GetMethod("Push"));
				ILGen.Emit(OpCodes.Ldc_I4_1);
				ILGen.Emit(OpCodes.Ret);
			}
			ILGen.Emit(OpCodes.Ret);

			Type T = TB.CreateType();
			DelegateInvokers.Add(DelegateName, T);
			return CreateLuaDelegateInvoker(D, L);//*/
		}

		static void Check(ILGenerator ILGen, Type T, int Pos) {
			ILGen.Emit(OpCodes.Ldarg_0);
			ILGen.Emit(OpCodes.Ldc_I4, Pos);
			if (T == typeof(string)) {
				ILGen.Emit(OpCodes.Ldsfld, typeof(IntPtr).GetField("Zero"));
				ILGen.EmitCall(OpCodes.Call, typeof(Lua).GetMethod("CheckString"), null);
			} else if (T == typeof(int))
				ILGen.EmitCall(OpCodes.Call, typeof(Lua).GetMethod("CheckInt"), null);
			else if (T == typeof(double))
				ILGen.EmitCall(OpCodes.Call, typeof(Lua).GetMethod("CheckNumber"), null);
			else
				throw new Exception("Unsupported type: " + T.FullName);
		}

		public static Type CreateDelegateType(Type ReturnType, Type[] Args, CCnv CConv = CCnv.Cdecl) {
			string DelegateName = CreateDelegateName(ReturnType, Args);
			if (DelegateTypes.ContainsKey(DelegateName))
				return DelegateTypes[DelegateName];

			TypeBuilder TB = DefMod.DefineType(DelegateName, TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
				TypeAttributes.Sealed);
			TB.SetParent(typeof(MulticastDelegate));
			TB.SetCustomAttribute(new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor
				(new[] { typeof(CCnv) }), new object[] { CConv }));

			TB.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(Object), typeof(IntPtr) })
				.SetImplementationFlags(MethodImplAttributes.Runtime);

			TB.DefineMethod("BeginInvoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
				MethodAttributes.Virtual, CallingConventions.HasThis, typeof(IAsyncResult),
				Args.Concat<Type>(new[] { typeof(AsyncCallback), typeof(object) }).ToArray())
				.SetImplementationFlags(MethodImplAttributes.Runtime);

			TB.DefineMethod("EndInvoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
				MethodAttributes.Virtual, CallingConventions.HasThis, ReturnType, new[] { typeof(AsyncCallback) })
				.SetImplementationFlags(MethodImplAttributes.Runtime);

			TB.DefineMethod("Invoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
				MethodAttributes.Virtual, CallingConventions.HasThis, ReturnType, Args)
				.SetImplementationFlags(MethodImplAttributes.Runtime);

			Type T = TB.CreateType();
			DelegateTypes.Add(DelegateName, T);
			return T;
		}
	}
}