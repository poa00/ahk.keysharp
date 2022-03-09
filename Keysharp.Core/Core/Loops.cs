using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Keysharp.Core.Windows;
using Microsoft.Win32;

namespace Keysharp.Core
{
	public static class Loops
	{
		internal static Stack<LoopInfo> loops = new Stack<LoopInfo>();//This probably needs to be made threadstatic//TODO
		private static StringBuilder regsb = new StringBuilder(1024);

		internal static IEnumerable GetSubKeys(LoopInfo info, RegistryKey key, bool k, bool v)
		{
			//try
			{
				if (v)
				{
					foreach (var val in ProcessRegValues(info, key))
						yield return val;
				}

				var subkeynames = key.GetSubKeyNames();

				if (subkeynames?.Length > 0)
				{
					foreach (var keyname in subkeynames.Reverse())
					{
						//try
						{
							using (var key2 = key.OpenSubKey(keyname, false))
							{
								if (k)
								{
									info.index++;
									info.regVal = string.Empty;
									info.regName = key2.Name.Substring(key2.Name.LastIndexOf('\\') + 1);
									info.regKeyName = key2.Name;//The full key path.
									info.regType = Core.Keyword_Key;
									var l = QueryInfoKey(key2);
									var dt = DateTime.FromFileTimeUtc(l);
									info.regDate = Conversions.ToYYYYMMDDHH24MISS(dt);
									yield return info.regKeyName;
								}

								foreach (var val in GetSubKeys(info, key2, k, v))
									yield return val;
							}
						}
						//catch (Exception e)
						//{
						//  //error, do something
						//}
					}
				}
			}
			//catch (Exception e)
			//{
			//  //error, do something
			//}
		}

		public static long Inc() => loops.Count > 0 ? ++loops.Peek().index : 0;

		public static long LoopIndex() => loops.Count > 0 ? loops.Peek().index : 0;

		/// <summary>
		/// Perform a series of commands repeatedly: either the specified number of times or until break is encountered.
		/// </summary>
		/// <param name="n">How many times (iterations) to perform the loop.</param>
		/// <returns></returns>
		public static IEnumerable Loop(params object[] obj)
		{
			var o = obj.L();

			if (o.Count > 0)
			{
				var o0 = o[0];

				if (!(o0 is string ss) || ss != string.Empty)
				{
					var n = Convert.ToInt64(o0);
					var info = Push();

					if (n != -1)
					{
						for (var i = 0L; i < n;)
						{
							info.index = i;
							yield return ++i;
						}
					}
					else
					{
						for (var i = 0L; true;)
						{
							info.index = i;
							yield return ++i;
						}
					}

					//_ = Pop();
					//The caller *MUST* call Pop(). This design is used because this
					//function may exit prematurely if the caller does a goto or break out
					//of the loop. In which case, all code below the yield return statement
					//would not get executed. So the burden is shifted to the caller to pop.//MATT
				}
			}
		}

		/// <summary>
		/// Retrieves each element of an array with its key if any.
		/// </summary>
		/// <param name="array">An array or object.</param>
		/// <returns>The current element.</returns>
		public static IEnumerable LoopEach(object array)
		{
			if (array == null)
				yield break;

			var info = new LoopInfo { type = LoopType.Each };
			loops.Push(info);
			var type = array.GetType();

			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				var dictionary = (IDictionary)array;

				foreach (var key in dictionary.Keys)
				{
					info.result = new[] { key, dictionary[key] };
					info.index++;
					yield return info.result;
				}
			}
			else if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				var enumerator = ((IEnumerable)array).GetEnumerator();

				while (enumerator.MoveNext())
				{
					info.result = new[] { null, enumerator.Current };
					info.index++;
					yield return info.result;
				}
			}

			_ = loops.Pop();
		}

		/// <summary>
		/// Retrieves the specified files or folders, one at a time.
		/// </summary>
		/// <param name="path">The name of a single file or folder, or a wildcard pattern.</param>
		/// <param name="mode">One of the following digits, or blank to use the default:
		/// <list>
		/// <item><code>D</code> Include directories (folders).</item>
		/// <item><code>F</code> Include files. If both F and D are omitted, files are included but not folders.</item>
		/// <item><code>R</code> Recurse into subdirectories (subfolders). All subfolders will be recursed into, not just those whose names match FilePattern. If R is omitted, files and folders in subfolders are not included.</item>
		/// </list>
		/// </param>
		/// <param name="recurse"><code>1</code> to recurse into subfolders, <code>0</code> otherwise.</param>
		/// <returns></returns>
		public static IEnumerable LoopFile(params object[] obj)
		{
			bool d = false, f = true, r = false;
			var o = obj.L();
			var info = Push(LoopType.Directory);
			var mode = string.Empty;
			var path = o[0] as string;
			//Dialogs.MsgBox(Path.GetFullPath(path));
			//Dialogs.MsgBox(Accessors.A_WorkingDir);

			if (o.Count > 1 && o[1] is string s)
				mode = s;

			if (!string.IsNullOrEmpty(mode))
			{
				d = mode.Contains('d', StringComparison.OrdinalIgnoreCase);
				f = mode.Contains('f', StringComparison.OrdinalIgnoreCase);
				r = mode.Contains('r', StringComparison.OrdinalIgnoreCase);
			}

			if (!d && !f)
				f = true;

			var dir = Path.GetDirectoryName(path);
			var pattern = Path.GetFileName(path);
			info.path = dir;

			foreach (var file in GetFiles(dir, pattern, d, f, r))
			{
				info.file = file;
				info.index++;
				yield return file;
			}

			//Caller must call Pop() after the loop exits.
		}

		/// <summary>
		/// Retrieves substrings (fields) from a string, one at a time.
		/// </summary>
		/// <param name="input">The string to parse.</param>
		/// <param name="delimiters">One of the following:
		/// <list>
		/// <item>the word <code>CSV</code> to parse in comma seperated value format;</item>
		/// <item>a sequence of characters to treat as delimiters;</item>
		/// <item>blank to parse each character of the string.</item>
		/// </list>
		/// </param>
		/// <param name="omit">An optional list of characters (case sensitive) to exclude from the beginning and end of each substring.</param>
		/// <returns></returns>
		public static IEnumerable LoopParse(params object[] obj)
		{
			var o = obj.L();
			var input = o[0] as string;
			string delimiters = string.Empty, omit = string.Empty;

			if (o.Count > 1)
			{
				if (o[1] is string s1)
					delimiters = s1;

				if (o.Count > 2 && o[2] is string s2)
					omit = s2;
			}

			var info = Push(LoopType.Parse);

			if (delimiters.ToLowerInvariant() == Core.Keyword_CSV)
			{
				var reader = new StringReader(input);
				var part = new StringBuilder();
				bool str = false, next = false;

				while (true)
				{
					var current = reader.Read();

					if (current == -1)
						goto collect;

					const char tokenStr = '"', tokenDelim = ',';
					var sym = (char)current;

					switch (sym)
					{
						case tokenStr:
							if (str)
							{
								if ((char)reader.Peek() == tokenStr)
								{
									_ = part.Append(tokenStr);
									_ = reader.Read();
								}
								else
									str = false;
							}
							else
							{
								if (next)
									_ = part.Append(tokenStr);
								else
									str = true;
							}

							break;

						case tokenDelim:
							if (str)
								goto default;

							goto collect; // sorry

						default:
							next = true;
							_ = part.Append(sym);
							break;
					}

					continue;
					collect:
					next = false;
					var result = part.ToString();
					part.Length = 0;
					info.result = result;
					info.index++;
					yield return result;

					if (current == -1)
						break;
				}
			}
			else
			{
				string[] parts;

				var remove = omit.ToCharArray();

				if (string.IsNullOrEmpty(delimiters))
					parts = input.ToCharArray().Select(x => x.ToString().Trim(remove)).Where(x => x != string.Empty).ToArray();//MATT
				else
					parts = input.Split(delimiters.ToCharArray(), StringSplitOptions.None).Select(x => x.Trim(remove)).Where(x => x != string.Empty).ToArray();

				foreach (var part in parts)
				{
					info.result = part;
					info.index++;
					yield return part;
				}
			}

			//Caller must call Pop() after the loop exits.
		}

		/// <summary>
		/// Retrieves the lines in a text file, one at a time.
		/// </summary>
		/// <param name="input">The name of the text file whose contents will be read by the loop</param>
		/// <param name="output">The optional name of the file to be kept open for the duration of the loop. If "*", then write to standard output.</param>
		/// <returns>Yield return each line in the input file</returns>
		public static IEnumerable LoopRead(params object[] obj)
		{
			var o = obj.L();
			var input = o[0] as string;
			var info = Push(LoopType.File);
			//Dialogs.MsgBox(Path.GetFullPath(input));

			if (o.Count > 1 && o[1] is string output)
				info.filename = output;

			if (!System.IO.File.Exists(input))
				yield break;

			using (var reader = System.IO.File.OpenText(input))
			{
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					info.line = line;
					info.index++;
					yield return line;
				}
			}

			//Caller must call Pop() after the loop exits.
		}

		/// <summary>
		/// Retrieves the contents of the specified registry subkey, one item at a time.
		/// </summary>
		/// <param name="root">Must be either:
		/// HKEY_LOCAL_MACHINE (or HKLM)
		/// HKEY_USERS (or HKU)
		/// HKEY_CURRENT_USER (or HKCU)
		/// HKEY_CLASSES_ROOT (or HKCR)
		/// HKEY_CURRENT_CONFIG (or HKCC)
		/// HKEY_PERFORMANCE_DATA (or HKPD)
		/// </param>
		/// <param name="key">The name of the key (e.g. Software\SomeApplication). If blank or omitted, the contents of RootKey will be retrieved.</param>
		/// <param name="subkeys">
		/// <list>
		/// <item><code>1</code> subkeys contained within Key are not retrieved (only the values);</item>
		/// <item><code>1</code> all values and subkeys are retrieved;</item>
		/// <item><code>2</code> only the subkeys are retrieved (not the values).</item>
		/// </list>
		/// </param>
		/// <param name="recurse"><code>1</code> to recurse into subkeys, <code>0</code> otherwise.</param>
		/// <returns></returns>
		public static IEnumerable LoopRegistry(params object[] obj)
		{
			bool k = false, v = true, r = false;
			var (keyname, mode) = obj.L().S2();

			if (!string.IsNullOrEmpty(mode))
			{
				k = mode.Contains('k', StringComparison.OrdinalIgnoreCase);
				v = mode.Contains('v', StringComparison.OrdinalIgnoreCase);
				r = mode.Contains('r', StringComparison.OrdinalIgnoreCase);
			}

			if (!k && !v)
				v = true;

			var info = Push(LoopType.Registry);
			var (reg, compname, key) = Conversions.ToRegRootKey(keyname);

			if (reg != null)
			{
				info.regVal = string.Empty;
				info.regName = reg.Name;
				info.regKeyName = keyname;
				info.regType = Core.Keyword_Key;
				var subkey = reg.OpenSubKey(key, false);
				var l = QueryInfoKey(subkey);
				var dt = DateTime.FromFileTimeUtc(l);
				info.regDate = Conversions.ToYYYYMMDDHH24MISS(dt);

				if (r)
				{
					foreach (var val in GetSubKeys(info, subkey, k, v))
						yield return val;
				}
				else
				{
					if (v)
					{
						foreach (var valueName in subkey.GetValueNames().Reverse())
						{
							info.index++;
							info.regVal = subkey.GetValue(valueName, string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames);

							if (info.regVal is byte[] ro)
								info.regVal = BitConverter.ToString(ro).Replace("-", string.Empty);

							info.regName = valueName;
							info.regType = Conversions.GetRegistryTypeName(subkey.GetValueKind(valueName));
							yield return valueName;
						}
					}

					if (k)
					{
						foreach (var subKeyName in subkey.GetSubKeyNames().Reverse())//AHK spec says the subkeys and values are returned in reverse.
						{
							using (var tempKey = subkey.OpenSubKey(subKeyName, false))
							{
								info.index++;
								info.regVal = string.Empty;
								info.regName = subKeyName.Substring(subKeyName.LastIndexOf('\\') + 1);
								info.regKeyName = tempKey.Name;//The full key path.
								info.regType = Core.Keyword_Key;
								l = QueryInfoKey(tempKey);
								dt = DateTime.FromFileTimeUtc(l);
								info.regDate = Conversions.ToYYYYMMDDHH24MISS(dt);
								yield return info.regKeyName;
							}
						}
					}

					info.regDate = string.Empty;//Date is empty outside of keys.
				}
			}

			//Caller must call Pop() after the loop exits.
		}

		internal static LoopInfo Peek() => loops.PeekOrNull();

		internal static LoopInfo Peek(LoopType looptype)
		{
			foreach (var l in loops)
				if (l.type == looptype)
					return l;

			return null;
		}

		public static LoopInfo Pop()
		{
			var info = loops.Count > 0 ? loops.Pop() : null;

			if (info != null && info.type == LoopType.File && info.sw != null)
				info.sw.Close();

			return info;
		}

		public static LoopInfo Push(LoopType t = LoopType.Normal)
		{
			var info = new LoopInfo { type = t };
			loops.Push(info);
			return info;
		}

		internal static LoopInfo GetDirLoop()
		{
			if (loops.Count > 0)
			{
				foreach (var l in loops)
				{
					switch (l.type)
					{
						case LoopType.Directory:
							return l;
					}
				}
			}

			return null;
		}

		internal static string GetDirLoopFilename()
		{
			if (loops.Count == 0)
				return string.Empty;

			foreach (var l in loops)
			{
				switch (l.type)
				{
					case LoopType.Directory:
						return l.file as string;
				}
			}

			return null;
		}

		/// <summary>
		/// From https://stackoverflow.com/questions/325931/getting-actual-file-name-with-proper-casing-on-windows-with-net
		/// </summary>
		/// <param name="pathName"></param>
		/// <returns></returns>
		internal static string GetExactPath(string pathName)
		{
			if (!(System.IO.File.Exists(pathName) || Directory.Exists(pathName)))
				return pathName;

			var di = new DirectoryInfo(pathName);

			if (di.Parent != null)
			{
				return Path.Combine(
						   GetExactPath(di.Parent.FullName),
						   di.Parent.GetFileSystemInfos(di.Name)[0].Name);
			}
			else
			{
				return di.Name.ToUpper();
			}
		}

		internal static string GetShortPath(string filename)
		{
			var buffer = new StringBuilder(1024);
			_ = WindowsAPI.GetShortPathName(filename, buffer, buffer.Capacity);
			return buffer.ToString();
		}

		internal static IEnumerable ProcessRegValues(LoopInfo info, RegistryKey key)
		{
			var valuenames = key.GetValueNames();

			if (valuenames?.Length > 0)
			{
				info.regDate = string.Empty;

				foreach (var valueName in valuenames.Reverse())
				{
					info.index++;
					info.regVal = key.GetValue(valueName, string.Empty, RegistryValueOptions.None);

					if (info.regVal is byte[] ro)
						info.regVal = BitConverter.ToString(ro).Replace("-", string.Empty);

					info.regName = valueName;
					info.regType = Conversions.GetRegistryTypeName(key.GetValueKind(valueName));
					yield return valueName;
				}
			}
		}

		private static IEnumerable<string> GetFiles(string path, string pattern, bool d, bool f, bool r)
		{
			var queue = new Queue<string>();
			queue.Enqueue(path);
			var enumopts = new EnumerationOptions
			{
				AttributesToSkip = FileAttributes.Normal,
				IgnoreInaccessible = true,
				MatchCasing = MatchCasing.CaseInsensitive,
				RecurseSubdirectories = r
			};

			while (queue.Count > 0)
			{
				path = queue.Dequeue();

				if (d)
				{
					IEnumerable<string> subdirs = null;

					try
					{
						subdirs = Directory.EnumerateDirectories(path, pattern, enumopts);
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine(ex);
					}

					if (subdirs != null)
					{
						foreach (var subdir in subdirs)
						{
							queue.Enqueue(subdir);
							yield return subdir;
						}
					}
				}

				if (f)
				{
					IEnumerable<string> files = null;

					try
					{
						files = Directory.EnumerateFiles(path, pattern, enumopts);
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine(ex);
					}

					if (files != null)
					{
						foreach (var file in files)
						{
							yield return file;
						}
					}
				}

				if (!enumopts.RecurseSubdirectories)
					break;
			}
		}

		private static long QueryInfoKey(RegistryKey regkey)
		{
			var classSize = (uint)(regsb.Capacity + 1);
			WindowsAPI.RegQueryInfoKey(
				regkey.Handle,
				regsb,
				ref classSize,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
				out var l);
			return l;
		}
	}

	public class LoopInfo
	{
		public object file;
		public string filename = string.Empty;
		public long index = -1;
		public object line;
		public string path;
		public object regDate;
		public string regKeyName;
		public string regName;
		public string regType;
		public object regVal;
		public object result;
		public TextWriter sw;

		public LoopType type = LoopType.Normal;

		public LoopInfo()
		{
		}
	}

	public enum LoopType
	{
		Normal,
		Registry,
		Directory,
		Parse,
		File,
		Each,
	}
}