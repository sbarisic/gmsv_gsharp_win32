using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

using GSharp;

namespace GSharp.Dynamic {
	class LuaState : DynamicObject {
		IntPtr L;

		public LuaState(IntPtr L) {
			this.L = L;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			result = GetIndex(binder.Name).Invoke(args);
			return true;
		}

		LuaObject GetIndex(string Name) {
			Lua.GetGlobal(L, Name);
			return new LuaObject(L);
		}

		void SetIndex(string Name, object Obj) {
			Lua.Push(L, Obj);
			Lua.SetGlobal(L, Name);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			result = GetIndex(binder.Name);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value) {
			SetIndex(binder.Name, value);
			return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
			if (indexes.Length != 1)
				throw new Exception("Invalid index count: " + indexes.Length);
			result = GetIndex(indexes[0].ToString());
			return true;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) {
			if (indexes.Length != 1)
				throw new Exception("Invalid index count: " + indexes.Length);
			SetIndex(indexes[0].ToString(), value);
			return true;
		}
	}

	class LuaObject : DynamicObject {
		IntPtr L;
		int RefID;

		public LuaObject(IntPtr L) {
			this.L = L;
			Ref();
		}

		/*~LuaObject() {
			Lua.Unref(L, RefID);
		}*/

		public void Ref() {
			RefID = Lua.Ref(L, Lua.REGISTRYINDEX);
		}

		public LuaObject Deref() {
			Lua.RawGetI(L, Lua.REGISTRYINDEX, RefID);
			return this;
		}

		public void Deref(Action A) {
			int Top = Lua.GetTop(L);
			Deref();

			A();
			Lua.Pop(L);
			if (Lua.GetTop(L) != Top)
				throw new Exception("Stack imbalance");
		}

		public object Invoke(object[] args) {
			object Ret = null;
			Deref(() => {
				for (int i = 0; i < args.Length; i++)
					Lua.Push(L, args[i]);
				if (Lua.PCall(L, args.Length, 1, 0) != 0)
					throw new Exception(Lua.ToString(L, -1));
				Ret = Lua.To(L);
			});
			return Ret;
		}

		public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
			result = Invoke(args);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			object Ret = null;
			GetIndex(binder.Name).Deref(() => {
				for (int i = 0; i < args.Length; i++)
					Lua.Push(L, args[i]);
				if (Lua.PCall(L, args.Length, 1, 0) != 0)
					throw new Exception(Lua.ToString(L, -1));
				Ret = Lua.To(L);
			});
			result = Ret;
			return true;
		}

		LuaObject GetIndex(string Name) {
			LuaObject Ret = null;
			Deref(() => {
				Lua.GetField(L, -1, Name);
				Ret = new LuaObject(L);
			});
			return Ret;
		}

		void SetIndex(string Name, object Obj) {
			Deref(() => {
				Lua.PushString(L, Name);
				Lua.Push(L, Obj);
				Lua.SetTable(L, -3);
			});
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			result = GetIndex(binder.Name);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value) {
			SetIndex(binder.Name, value);
			return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
			if (indexes.Length != 1)
				throw new Exception("Invalid index count: " + indexes.Length);
			result = GetIndex(indexes[0].ToString());
			return true;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) {
			if (indexes.Length != 1)
				throw new Exception("Invalid index count: " + indexes.Length);
			SetIndex(indexes[0].ToString(), value);
			return true;
		}

		public override string ToString() {
			string TypeName = "";
			string ToStringString = "";
			Deref(() => {
				TypeName = Lua.TypeName(L, Lua.Type(L, -1));
				ToStringString = Lua.ToString(L, -1);
			});
			return "(" + TypeName + ": " + ToStringString + ")";
		}
	}
}