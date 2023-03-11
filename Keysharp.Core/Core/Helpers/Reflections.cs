using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Windows.Forms;

namespace Keysharp.Core
{
	internal class UnloadableAssemblyLoadContext : AssemblyLoadContext
	{
		private readonly AssemblyDependencyResolver resolver;

		public UnloadableAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true) => resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);

		protected override Assembly Load(AssemblyName name)
		{
			var assemblyPath = resolver.ResolveAssemblyToPath(name);
			return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
		}
	}

	public static class Reflections
	{
		internal static readonly Dictionary<string, Dictionary<Type, MethodInfo>> stringToTypeBuiltInMethods = new Dictionary<string, Dictionary<Type, MethodInfo>>(sttcap, StringComparer.OrdinalIgnoreCase);
		internal static readonly Dictionary<string, Dictionary<Type, MethodInfo>> stringToTypeLocalMethods = new Dictionary<string, Dictionary<Type, MethodInfo>>(sttcap / 10, StringComparer.OrdinalIgnoreCase);
		internal static readonly Dictionary<string, Dictionary<Type, MethodInfo>> stringToTypeMethods = new Dictionary<string, Dictionary<Type, MethodInfo>>(sttcap, StringComparer.OrdinalIgnoreCase);
		internal static readonly Dictionary<string, Dictionary<Type, PropertyInfo>> stringToTypeProperties = new Dictionary<string, Dictionary<Type, PropertyInfo>>(sttcap, StringComparer.OrdinalIgnoreCase);
		internal static readonly int sttcap = 1000;
		internal static readonly Dictionary<Type, Dictionary<string, MethodInfo>> typeToStringBuiltInMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>(sttcap / 10);
		internal static readonly Dictionary<Type, Dictionary<string, MethodInfo>> typeToStringLocalMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>(sttcap / 10);
		internal static readonly Dictionary<Type, Dictionary<string, MethodInfo>> typeToStringMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>(sttcap / 5);
		internal static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> typeToStringProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>(sttcap / 5);
		//private static Dictionary<Guid, Dictionary<string, MethodInfo>> ExtensionMethods = new Dictionary<Guid, Dictionary<string, MethodInfo>>(sttcap / 20);
		internal static Dictionary<string, Assembly> loadedAssemblies;

		private static Dictionary<string, Assembly> GetLoadedAssemblies()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var dkt = new Dictionary<string, Assembly>(assemblies.Length);

			foreach (var assembly in assemblies)
			{
				try
				{
					if (!assembly.IsDynamic)
						dkt[assembly.Location] = assembly;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			return dkt;
		}

		static Reflections() => Initialize();

		/// <summary>
		/// This must be manually called before any program is run.
		/// Normally we'd put this kind of init in the static constructor, however it must be able to be manually called
		/// when running unit tests. Once upon init, then again within the unit test's auto generated program so it can find
		/// any locally declared methods inside.
		/// Also note that when running a script from Keysharp.exe, this will get called once when the parser starts in Keysharp, then again
		/// when the script actually runs. On the second time, there will be an extra assembly loaded, which is the compiled script itself. More system assemblies will also be loaded.
		/// </summary>
		public static void Initialize()
		{
			loadedAssemblies = GetLoadedAssemblies();
			CacheAllMethods();
			CacheAllProperties();
		}

		internal static void CacheAllMethods()
		{
			List<Assembly> assemblies;
			var loadedAssembliesList = loadedAssemblies.Values;
			stringToTypeLocalMethods.Clear();
			typeToStringLocalMethods.Clear();
			stringToTypeBuiltInMethods.Clear();
			typeToStringBuiltInMethods.Clear();

			if (AppDomain.CurrentDomain.FriendlyName == "testhost")//When running unit tests, the assembly names are changed for the auto generated program.
				assemblies = loadedAssembliesList.ToList();
			else if (Assembly.GetEntryAssembly().FullName.StartsWith("Keysharp,"))//Running from Keysharp.exe which compiled this script and launched it as a dynamically loaded assembly.
				assemblies = loadedAssembliesList.Where(assy => assy.Location.Length == 0 || assy.FullName.StartsWith("Keysharp.")).ToList();//The . is important, it means only inspect Keysharp.Core because Keysharp, is the main Keysharp program, which we don't want to inspect. An assembly with an empty location is the compiled exe.
			else//Running as a standalone executable.
				assemblies = loadedAssembliesList.Where(assy => assy.FullName.StartsWith("Keysharp.") ||
														(assy.EntryPoint != null &&
																assy.EntryPoint.DeclaringType != null &&
																assy.EntryPoint.DeclaringType.Namespace == "Keysharp.CompiledMain"
														)).ToList();

			//_ = MessageBox.Show(string.Join('\n', assemblies.Select(assy => assy.FullName)));

			foreach (var asm in assemblies)
				foreach (var type in asm.GetTypes())
					if (type.IsClass && type.IsPublic && type.Namespace != null && (type.Namespace.StartsWith("Keysharp.Core") || type.Namespace.StartsWith("Keysharp.CompiledMain") || type.Namespace.StartsWith("Keysharp.Tests")))//Allow tests so we can use function objects inside of unit tests.
						_ = FindAndCacheMethod(type, "");

			foreach (var typekv in typeToStringMethods)
				foreach (var methkv in typekv.Value)
				{
					_ = stringToTypeMethods.GetOrAdd(methkv.Key).GetOrAdd(typekv.Key, methkv.Value);

					if (typekv.Key.FullName.StartsWith("Keysharp.CompiledMain", StringComparison.OrdinalIgnoreCase) || typekv.Key.FullName.StartsWith("Keysharp.Tests", StringComparison.OrdinalIgnoreCase))//Need to include Tests so that unit tests will work.
					{
						_ = stringToTypeLocalMethods.GetOrAdd(methkv.Key).GetOrAdd(typekv.Key, methkv.Value);
						_ = typeToStringLocalMethods.GetOrAdd(typekv.Key, () => new Dictionary<string, MethodInfo>(typekv.Value.Count, StringComparer.OrdinalIgnoreCase)).GetOrAdd(methkv.Key, methkv.Value);
					}
					else
					{
						_ = stringToTypeBuiltInMethods.GetOrAdd(methkv.Key).GetOrAdd(typekv.Key, methkv.Value);
						_ = typeToStringBuiltInMethods.GetOrAdd(typekv.Key, () => new Dictionary<string, MethodInfo>(typekv.Value.Count, StringComparer.OrdinalIgnoreCase)).GetOrAdd(methkv.Key, methkv.Value);
					}
				}
		}

		internal static void CacheAllProperties()
		{
			typeToStringProperties.Clear();
			stringToTypeProperties.Clear();

			foreach (var item in loadedAssemblies.Values.Where(assy => assy.FullName.StartsWith("Keysharp.Core,")))
				foreach (var type in item.GetTypes())
					if (type.IsClass && type.IsPublic && type.Namespace != null && type.Namespace.StartsWith("Keysharp.Core"))
						_ = FindAndCacheProperty(type, "");

			foreach (var typekv in typeToStringProperties)
				foreach (var propkv in typekv.Value)
					_ = stringToTypeProperties.GetOrAdd(propkv.Key).GetOrAdd(typekv.Key, propkv.Value);
		}

		internal static MethodInfo FindAndCacheMethod(Type t, string name)
		{
			do
			{
				if (typeToStringMethods.TryGetValue(t, out var dkt))
				{
				}
				else
				{
					var meths = (MethodInfo[])t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

					if (meths.Length > 0)
					{
						foreach (var meth in meths)
							typeToStringMethods.GetOrAdd(meth.DeclaringType, () => new Dictionary<string, MethodInfo>(meths.Length, StringComparer.OrdinalIgnoreCase)).Add(meth.Name, meth);
					}
					else//Make a dummy entry because this type has no methods. This saves us additional searching later on when we encounter a type derived from this one. It will make the first Dictionary lookup above return true.
					{
						typeToStringMethods[t] = dkt = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
						t = t.BaseType;
						continue;
					}
				}

				if (dkt == null && !typeToStringMethods.TryGetValue(t, out dkt))
				{
					t = t.BaseType;
					continue;
				}

				if (dkt.TryGetValue(name, out var mi))//Since the Dictionary was created above with StringComparer.OrdinalIgnoreCase, this will be a case insensitive match.
					return mi;

				t = t.BaseType;
			} while (t.Assembly == typeof(Any).Assembly);//Traverse down to the base, but only do it for types that are part of this library. Once a base crosses the library boundary, the loop stops.

			return null;
		}

		internal static PropertyInfo FindAndCacheProperty(Type t, string name)
		{
			try
			{
				do
				{
					if (typeToStringProperties.TryGetValue(t, out var dkt))
					{
					}
					else//Property on this type has not been used yet, so get all properties and cache.
					{
						var props = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

						if (props.Length > 0)
						{
							foreach (var prop in props)
								typeToStringProperties.GetOrAdd(prop.DeclaringType, () => new Dictionary<string, PropertyInfo>(props.Length, StringComparer.OrdinalIgnoreCase)).Add(prop.Name, prop);
						}
						else//Make a dummy entry because this type has no properties. This saves us additional searching later on when we encounter a type derived from this one. It will make the first Dictionary lookup above return true.
						{
							typeToStringProperties[t] = dkt = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
							t = t.BaseType;
							continue;
						}
					}

					if (dkt == null && !typeToStringProperties.TryGetValue(t, out dkt))
					{
						t = t.BaseType;
						continue;
					}

					if (dkt.TryGetValue(name, out var pi))//Since the Dictionary was created above with StringComparer.OrdinalIgnoreCase, this will be a case insensitive match.
						return pi;

					t = t.BaseType;
				} while (t != typeof(object));
			}
			catch (Exception)// e)
			{
				throw;
			}

			return null;
		}

		internal static MethodInfo FindBuiltInMethod(string name)
		{
			//foreach (var item in AppDomain.CurrentDomain.GetAssemblies().Where(assy => assy.FullName.StartsWith("Keysharp.Core,")))
			//  foreach (var type in item.GetTypes())
			//      if (type.IsClass && type.IsPublic && type.Namespace.StartsWith("Keysharp.Core"))
			//          if (FindMethod(type, name) is MethodInfo mi)
			//              return mi;
			if (stringToTypeBuiltInMethods.TryGetValue(name, out var meths))
				if (meths.Count > 0)
					return meths.Values.First();

			return null;
		}

		/*
		        internal static MethodInfo FindExtensionMethod(Type t, string meth)
		        {
		            //if (typeof(IDictionary).IsAssignableFrom(t))
		            //  if (ExtensionMethods.TryGetValue(typeof(IDictionary).GUID, out var idkt))
		            //      if (idkt.TryGetValue(meth, out var mi))
		            //          return mi;
		            if (ExtensionMethods.TryGetValue(t.GUID, out var dkt))
		                if (dkt.TryGetValue(meth, out var mi))
		                    return mi;

		            return null;
		        }
		*/

		internal static MethodInfo FindLocalMethod(string name)
		{
			if (stringToTypeLocalMethods.TryGetValue(name, out var meths))
				if (meths.Count > 0)
					return meths.Values.First();

			//var stack = new StackTrace(false).GetFrames();
			//for (var i = 0; i < stack.Length; i++)
			//{
			//  var type = stack[i].GetMethod().DeclaringType;
			//  if (type.FullName.StartsWith("Keysharp.Main", StringComparison.OrdinalIgnoreCase))
			//      return FindMethod(type, name);
			//}
			return null;
		}

		internal static MethodInfo FindLocalRoutine(string name) => FindLocalMethod(Keysharp.Scripting.Parser.LabelMethodName(name));

		internal static MethodInfo FindMethod(string name)
		{
			if (FindLocalMethod(name) is MethodInfo mi)
				return mi;

			return FindBuiltInMethod(name);
		}

		internal static string GetVariableInfo()
		{
			var sb = new StringBuilder(2048);
			var stack = new StackTrace(false).GetFrames();

			for (var i = stack.Length - 1; i >= 0; i--)
			{
				if (stack[i] != null &&
						stack[i].GetMethod() != null &&
						stack[i].GetMethod().DeclaringType.Attributes.HasFlag(TypeAttributes.Public))//Public is the script, everything else should be hidden.
				{
					if (stack[i].GetMethod().DeclaringType.Namespace != null &&
							stack[i].GetMethod().DeclaringType.Namespace.StartsWith("Keysharp"))
					{
						var meth = stack[i].GetMethod();
						_ = sb.AppendLine($"Class: {meth.ReflectedType.Name}");
						_ = sb.AppendLine();

						foreach (var v in meth.ReflectedType.GetProperties(BindingFlags.Public | BindingFlags.Static))
						{
							var val = v.GetValue(null);
							var varstr = $"\t{val?.GetType()} {v.Name}: ";

							if (val is string s)
								varstr += $"[{s.Length}] {s.Substring(0, Math.Min(s.Length, 60))}";
							else if (val is Keysharp.Core.Array arr)
							{
								var ct = Math.Min(100, arr.Count);
								var tempsb = new StringBuilder(ct * 100);

								for (var a = 1; a <= ct; a++)
								{
									var tempstr = arr[a].ToString();
									_ = tempsb.Append(tempstr.Substring(0, Math.Min(tempstr.Length, 60)));

									if (a < ct)
										_ = tempsb.Append(", ");
								}

								varstr += tempsb.ToString();
							}
							else if (val is Keysharp.Core.Map map)
							{
								var ct = Math.Min(100, map.Count);
								var a = 0;
								var tempsb = new StringBuilder(ct * 100);
								_ = tempsb.Append('{');

								foreach (var kv in map.map)
								{
									var tempstr = kv.Value.ToString();
									_ = tempsb.Append($"{kv.Key} : {tempstr.Substring(0, Math.Min(tempstr.Length, 60))}");

									if (++a < ct)
										_ = tempsb.Append(", ");
								}

								_ = tempsb.Append('}');
								varstr += tempsb.ToString();
							}
							else if (val == null)
								varstr += "null";
							else
								varstr += val.ToString();

							_ = sb.AppendLine(varstr);
						}

						_ = sb.AppendLine("");
						_ = sb.AppendLine($"Method: {meth.Name}");
						var mb = stack[i].GetMethod().GetMethodBody();

						foreach (var lvi in mb.LocalVariables)
							_ = sb.AppendLine($"\t{lvi.LocalType}");

						_ = sb.AppendLine("--------------------------------------------------");
						_ = sb.AppendLine();
					}
				}
			}

			return sb.ToString();
		}

		/*
		    public MethodInfo BestMatch(string name, int length)
		    {
		    MethodInfo result = null;
		    var last = int.MaxValue;

		    foreach (var writer in this)
		    {
		        // find method with same name (case insensitive)
		        if (!name.Equals(writer.Name, StringComparison.OrdinalIgnoreCase))
		            continue;

		        var param = writer.GetParameters().Length;

		        if (param == length) // perfect match when parameter count is the same
		        {
		            return writer;
		        }
		        else if (param > length && param < last) // otherwise find a method with the next highest number of parameters
		        {
		            result = writer;
		            last = param;
		        }
		        else if (result == null) // return the first method with excess parameters as a last resort
		            result = writer;
		    }

		    return result;
		    }
		*/

		internal static T SafeGetProperty<T>(object item, string name) => (T)item.GetType().GetProperty(name, typeof(T))?.GetValue(item);

		internal static void SafeSetProperty(object item, string name, object value) => item.GetType().GetProperty(name, value.GetType())?.SetValue(item, value, null);

		/// <summary>
		/// This Methode extends the System.Type-type to get all extended methods. It searches hereby in all assemblies which are known by the current AppDomain.
		/// </summary>
		/// <remarks>
		/// Insired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
		/// </remarks>
		/// <returns>returns MethodInfo[] with the extended Method</returns>
		private static List<MethodInfo> GetExtensionMethods(this Type t, List<Type> types)
		{
			var query = from type in types
						where type.IsSealed && /*!type.IsGenericType &&*/ !type.IsNested
						from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
						where method.IsDefined(typeof(ExtensionAttribute), false)
						where method.GetParameters().Length > 0 && method.GetParameters()[0].ParameterType.Name == t.Name
						select method;
			return query.Select(m => m.IsGenericMethod ? m.MakeGenericMethod(t.GenericTypeArguments) : m).ToList();
		}
	}
}