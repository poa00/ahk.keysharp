using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;
using Keysharp.Core;

namespace Keysharp.Scripting
{
	public partial class Script
	{
		public static Core.Array Array(params object[] obj)
		{
			if (obj.Length == 0)
				return new Core.Array(64);

			var arr = new Core.Array(obj.Length);
			arr.Push(obj);
			return arr;
		}

		public static Keysharp.Core.Buffer Buffer(object obj0, object obj1 = null) => new (obj0, obj1);

		public static void Collect() => GC.Collect();

		public static Map Dictionary(object[] keys, object[] values)
		{
			var table = new Map();

			for (var i = 0; i < keys.Length; i++)
			{
				var name = keys[i];
				var entry = i < values.Length ? values[i] : null;

				if (entry == null)
				{
					if (table.Has(name))
						_ = table.Delete(name);
				}
				else
					table[name] = entry;
			}

			return table;
		}

		public static Error Error(params object[] obj) => new (obj);

		public static bool ErrorOccurred(Error err)
		{
			object result = null;

			if (OnErrorHandlers != null)
			{
				foreach (var handler in OnErrorHandlers)
				{
					result = handler.Call(err, err.ExcType);

					if (err.ExcType == Keyword_Return)
					{
						if (result.ParseLong() != -1)
							Flow.Exit(0);
					}
					else if (err.ExcType == Keyword_Exit)
						Flow.Exit(0);
					else if (err.ExcType == Keyword_ExitApp)
						_ = Flow.ExitAppInternal(Flow.ExitReasons.Error);

					if (result.IsCallbackResultNonEmpty() && result.ParseLong(false) == 1L)
						return false;
				}
			}

			return true;
		}

		public static FuncObj FuncObj(object obj0, object obj1 = null) => new FuncObj(obj0.As(), obj1);

		public static (object, MethodPropertyHolder) GetStaticMethodT<T>(object name) => Reflections.FindAndCacheMethod(typeof(T), name.ToString()) is MethodPropertyHolder mph&& mph.mi != null&& mph.IsStaticFunc
		? (null, mph)
		: throw new MethodError($"Attempting to get method {name} failed.");

		public static Gui Gui(object obj0 = null, object obj1 = null, object obj2 = null) => new (obj0, obj1, obj2);

		public static long HasBase(object obj0, object obj1) => obj1.GetType().IsAssignableFrom(obj0.GetType()) ? 1L : 0L;

		public static object Index(object item, params object[] index) => item == null ? null : IndexAt(item, index);

		public static IndexError IndexError(params object[] obj) => new (obj);

		/// <summary>
		/// Differs in that locale is always considered.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long IsAlnum(object obj)
		{
			var s = obj.As();
			return s?.Length == 0 || s.All(ch => char.IsLetter(ch) || char.IsNumber(ch)) ? 1 : 0;
		}

		/// <summary>
		/// Differs in that locale is always considered.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long IsAlpha(object obj)
		{
			var s = obj.As();
			return s?.Length == 0 || s.All(char.IsLetter) ? 1 : 0;
		}

		public static long IsDate(object obj) => IsTime(obj);

		public static long IsDigit(object obj)
		{
			var s = obj.As();
			return s?.Length == 0 || s.All(char.IsDigit) ? 1 : 0;
		}

		public static long IsFloat(object obj)
		{
			var o = obj;

			if (o is double || o is float || o is decimal)
				return 1;

			var val = o.ParseDouble(false, true);
			return val.HasValue ? 1 : 0;
		}

		/// <summary>
		/// Checks if a function is defined.
		/// </summary>
		/// <param name="name">The name of a function.</param>
		/// <returns><c>true</c> if the specified function exists in the current scope, <c>false</c> otherwise.</returns>
		public static long IsFunc(object name) => Reflections.FindMethod(name.ToString()) is MethodPropertyHolder mph&& mph.mi != null ? 1L : 0L;

		public static long IsInteger(object obj)
		{
			var o = obj;

			if (o is long || o is int || o is uint || o is ulong)
				return 1L;

			if (o is double || o is float || o is decimal)
				return 0L;

			var val = o.ParseLong(false);
			return val.HasValue ? 1L : 0L;
		}

		public static long IsLabel(object name) => throw new Error("C# does not allow querying labels at runtime.");

		/// <summary>
		/// Differs in that locale is always considered.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long IsLower(object obj)
		{
			var s = obj.As();
			return s?.Length == 0 || s.All(ch => char.IsLetter(ch) && char.IsLower(ch)) ? 1 : 0;
		}

		public static long IsNumber(object obj) => IsInteger(obj) | IsFloat(obj);

		public static long IsObject(object obj) => obj is KeysharpObject ? 1 : 0;

		public static long IsSet(object obj) => obj != UnsetArg.Default&& obj != null ? 1 : 0;

		public static long IsSpace(object obj)
		{
			foreach (var ch in obj.As())
				if (!Keyword_Spaces.Any(ch2 => ch2 == ch))
					return 0L;

			return 1L;
		}

		public static long IsTime(object obj)
		{
			var s = obj.As();
			DateTime dt;

			try
			{
				dt = Conversions.ToDateTime(s);

				if (dt == DateTime.MinValue)
					return 0L;
			}
			catch
			{
				return 0L;
			}

			int[] t = { DateTime.Now.Year / 100, DateTime.Now.Year % 100, 1, 1, 0, 0, 0, 0 };
			var tempdt = new DateTime(t[1], t[2], t[3], t[4], t[5], t[6], System.Globalization.CultureInfo.CurrentCulture.Calendar);//Will be wrong this if parsing totally failed.
			return dt != tempdt ? 1L : 0L;
		}

		/// <summary>
		/// Differs in that locale is always considered.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long IsUpper(object obj)
		{
			var s = obj.As();
			return s?.Length == 0 || s.All(ch => char.IsLetter(ch) && char.IsUpper(ch)) ? 1 : 0;
		}

		public static long IsXDigit(object obj)
		{
			var s = obj.As();
			var sp = s.AsSpan();

			if (sp.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase))
				sp = sp.Slice(2);

			foreach (var ch in sp)
				if (!((ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f')))
					return 0L;

			return 1L;
		}

		public static KeyError KeyError(params object[] obj) => new (obj);

		public static Map Map(params object[] obj) => Object(obj);

		public static MemberError MemberError(params object[] obj) => new (obj);

		public static MemoryError MemoryError(params object[] obj) => new (obj);

		public static Menu Menu() => new ();

		public static MenuBar MenuBar() => new ();

		public static object MenuFromHandle(object obj)
		{
			var handle = new IntPtr(obj.Al());
			var menu = System.Windows.Forms.MenuStrip.FromHandle(handle);

			if (menu != null)
				return menu;

			if ((menu = System.Windows.Forms.ContextMenuStrip.FromHandle(handle)) != null)
				return menu;

			return "";
		}

		public static MethodError MethodError(params object[] obj) => new (obj);

		public static object ObjSetCapacity(object obj0, object obj1) => throw new Keysharp.Core.Error("ObjSetCapacity() not implemented.");

		public static void OnError(object obj0, object obj1 = null)
		{
			var e = obj0;
			var i = obj1.Al(1L);
			var del = Function.GetFuncObj(e, null, true);

			if (OnErrorHandlers == null)
				OnErrorHandlers = new List<IFuncObj>();

			OnErrorHandlers.ModifyEventHandlers(del, i);
		}

		public static OSError OSError(params object[] obj) => new (obj);

		public static PropertyError PropertyError(params object[] obj) => new (obj);

		public static object SetObject(object value, object item, params object[] index)
		{
			var key = index[0];

			if (item is Map map2)//This function has been redesigned to handle assigning a map key/value pair, or assigning a value to an array index. It is NOT for setting properties.
			{
				map2[key] = value;
				return value;
			}

			var position = (int)ForceLong(key);

			if (item is Core.Array al)
			{
				al[position] = value;
				return value;
			}
			else if (item is Core.Buffer buf)
			{
				throw new IndexError("Cannot call SetObject() on a Buffer object.");
			}
			else if (item == null)
			{
				return null;
			}

			foreach (var mi in item.GetType().GetMethods().Where(m => m.Name == "set_Item"))//Try to see if this is the indexer property __Item.
			{
				var parameters = mi.GetParameters();

				if (parameters.Length > 1)
					return mi.Invoke(item, index.Concat(new object[] { value }));// new object[] { index, value });
				var p = parameters[0];

				if (p.ParameterType == typeof(object))
					return mi.Invoke(item, new object[] { key, value });
				else if (key is long l && p.ParameterType == typeof(long))//Subtract one because arrays are 1-indexed, negative numbers are reverse indexed.
					return mi.Invoke(item, new object[] { l - 1, value });
				else if (key is int i && p.ParameterType == typeof(int))
					return mi.Invoke(item, new object[] { i - 1, value });
			}

			//These are probably never used.
			if (item is object[] objarr)
			{
				var actualindex = position < 0 ? objarr.Length + position : position - 1;
				return objarr[actualindex];
			}
			else if (item is System.Array array)
			{
				var actualindex = position < 0 ? array.Length + position : position - 1;
				return array.GetValue(actualindex);
			}

			throw new MemberError($"Attempting to set index {key} of object {item} to value {value} failed.");
		}

		public static Keysharp.Core.StringBuffer StringBuffer(object obj0, object obj1 = null) => new StringBuffer(obj0.As(), obj1.Ai(256));

		public static TargetError TargetError(params object[] obj) => new (obj);

		public static TimeoutError TimeoutError(params object[] obj) => new (obj);

		public static string Type(object t) => t.GetType().Name;

		public static TypeError TypeError(params object[] obj) => new (obj);

		public static UnsetItemError UnsetItemError(params object[] obj) => new (obj);

		public static ValueError ValueError(params object[] obj) => new (obj);

		public static ZeroDivisionError ZeroDivisionError(params object[] obj) => new (obj);

		public object ObjGetCapacity(params object[] obj) => throw new Keysharp.Core.Error("ObjGetCapacity() is not supported for dictionaries in C#");

		public long ObjHasOwnProp(params object[] obj) => 1L;

		public long ObjOwnPropCount(params object[] obj) => 1L;

		public object ObjOwnProps(params object[] obj) => true;

		public void ObjSetBase(params object[] obj) => throw new Exception(Any.BaseExc);

		public object ObjSetCapacity(params object[] obj) => true;

		private static object IndexAt(object item, params object[] index)
		{
			var key = index == null || index.Length == 0 ? 0 : index[0];

			if (item is Map table)
				return table[key];

			var position = (int)ForceLong(key);

			//The most common is going to be a string, array, map or buffer.
			if (item is string s)
			{
				var actualindex = position < 0 ? s.Length + position : position - 1;
				return s[actualindex];
			}
			else if (item is Core.Array al)
				return al[position];
			else if (item is Core.Buffer buf)
				return buf[position];

			foreach (var mi in item.GetType().GetMethods().Where(m => m.Name == "get_Item"))
			{
				var parameters = mi.GetParameters();

				if (parameters.Length > 1)
					return mi.Invoke(item, index);

				var p = parameters[0];

				if (p.ParameterType == typeof(object))
					return mi.Invoke(item, new object[] { key });
				else if (key is long l && p.ParameterType == typeof(long))//Subtract one because arrays are 1-indexed, negative numbers are reverse indexed.
					return mi.Invoke(item, new object[] { l - 1 });
				else if (key is int i && p.ParameterType == typeof(int))
					return mi.Invoke(item, new object[] { i - 1 });
			}

			//These are probably never used.
			if (item is object[] objarr)
			{
				var actualindex = position < 0 ? objarr.Length + position : position - 1;
				return objarr[actualindex];
			}
			else if (item is System.Array array)
			{
				var actualindex = position < 0 ? array.Length + position : position - 1;
				return array.GetValue(actualindex);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(item.GetType()))
			{
				var ienum = (IEnumerable)item;
				var enumerator = ienum.GetEnumerator();
				var i = 0;
				var len = 0;
				var tempenum = ienum.GetEnumerator();

				while (tempenum.MoveNext())
					len++;

				var actualindex = position < 0 ? len + position : position - 1;

				while (enumerator.MoveNext())
				{
					if (i == actualindex)
						return enumerator.Current;

					i++;
				}

				return null;
			}

			throw new IndexError($"Attempting to get index of {key} on item {item} failed.");
		}

		private static Map Object(params object[] obj)
		{
			if (obj.Length == 0)
				return new Map();

			var dkt = new Map();
			dkt.Set(obj);
			return dkt;
		}
	}
}