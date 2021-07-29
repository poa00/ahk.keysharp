using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Keysharp.Scripting;
using NUnit.Framework;

namespace Keysharp.Tests
{
	public partial class Scripting
	{
		private const string ext = ".ahk";
		private string path = string.Format("..{0}..{0}..{0}Keysharp.Tests{0}Code{0}", Path.DirectorySeparatorChar);

		public bool TestScript(string source, bool testfunc)
		{
			var b1 = false;
			var b2 = true;
			//Task[] taskArray = { Task.Factory.StartNew(() => b1 = HasPassed(RunScript(string.Concat(path, source, ext), source, true))),
			//                   Task.Factory.StartNew(() =>
			//{
			//  if (testfunc)
			//      b2 = HasPassed(RunScript(string.Concat(path, source, ext), source + "_func", true, true));
			//})
			//                 };
			//Task.WaitAll(taskArray);
			b1 = HasPassed(RunScript(string.Concat(path, source, ext), source, true));

			if (testfunc && b1)
				b2 = HasPassed(RunScript(string.Concat(path, source, ext), source + "_func", true, true));

			return b1 && b2;
		}

		public bool ValidateScript(string source, string name)
		{
			RunScript(string.Concat(path, source, ext), name, false);
			return true;
		}

		public bool HasPassed(string output)
		{
			if (string.IsNullOrEmpty(output))
				return false;

			const string pass = "pass";

			foreach (var remove in new[] { pass, " ", "\n" })
					output = output.Replace(remove, string.Empty);
			return output.Length == 0;
		}

		public string WrapInFunc(string source)
		{
			var sb = new StringBuilder();
			sb.AppendLine("func()");
			sb.AppendLine("{");

			using (var sr = new StringReader(source))
			{
				string line;

				while ((line = sr.ReadLine()) != null)
					if (!line.StartsWith("#") && !line.StartsWith(";"))
						_ = sb.AppendLine("\t" + line);
			}

			sb.AppendLine("}");
			sb.AppendLine("func()");
			return sb.ToString();
		}

		public string RunScript(string source, string name, bool execute, bool wrapinfunction)
		{
			return RunScript(WrapInFunc(File.ReadAllText(source)), name, execute);
		}

		public void TestException(Action func)
		{
			var excthrown = false;

			try
			{
				func();
			}
			catch (Exception e)
			{
				excthrown = true;
			}

			Assert.IsTrue(excthrown);
		}

		public string RunScript(string source, string name, bool execute)
		{
			Compiler.Debug(Environment.CurrentDirectory);
			var ch = new CompilerHelper();
			var (domunits, domerrs) = ch.CreateDomFromFile(source);

			if (domerrs.HasErrors)
			{
				foreach (CompilerError err in domerrs)
					Compiler.Debug(err.ErrorText);

				return string.Empty;
			}

			var (code, exc) = ch.CreateCodeFromDom(domunits);

			if (exc is Exception e)
			{
				Compiler.Debug(e.Message);
				return string.Empty;
			}

			code = CompilerHelper.UsingStr + Parser.TrimParens(code);

			using (var sourceWriter = new StreamWriter("./" + name + ".cs"))
			{
				sourceWriter.WriteLine(code);
			}

			Assembly compiledasm;
#if !WINDOWS
			var (results, compileexc) = ch.Compile(code, string.Empty);

			if (compileexc != null)
			{
				Compiler.Debug(compileexc.Message);
				return string.Empty;
			}
			else if (results == null)
			{
				return string.Empty;
			}
			else if (results.Errors.HasErrors)
			{
				foreach (CompilerError err in results.Errors)
					Compiler.Debug(err.ErrorText);

				return string.Empty;
			}

			compiledasm = results.CompiledAssembly;
#else
			var (results, ms, compileexc) = ch.Compile(code, name);

			if (compileexc != null)
			{
				Compiler.Debug(compileexc.Message);
				return string.Empty;
			}
			else if (results == null)
			{
				return string.Empty;
			}
			else if (results.Success)
			{
				ms.Seek(0, SeekOrigin.Begin);
				var arr = ms.ToArray();
				compiledasm = Assembly.Load(arr);
			}
			else
			{
				return string.Empty;
			}

#endif
			var buffer = new StringBuilder();
			var output = string.Empty;

			if (execute)
			{
				using (var writer = new StringWriter(buffer))
				{
					try
					{
						Console.SetOut(writer);
						GC.Collect();
						GC.WaitForPendingFinalizers();

						if (compiledasm == null)
							throw new Exception("Compilation failed.");

						//Environment.SetEnvironmentVariable("SCRIPT", script);
						var program = compiledasm.GetType("Keysharp.Main.Program");
						var main = program.GetMethod("Main");
						var temp = new string[] { };
						_ = main.Invoke(null, new object[] { temp });
					}
					catch (Exception ex)
					{
						if (ex is TargetInvocationException)
							ex = ex.InnerException;

						var error = new StringBuilder();
						_ = error.AppendLine("Execution error:\n");
						_ = error.AppendLine($"{ex.GetType().Name}: {ex.Message}");
						_ = error.AppendLine();
						_ = error.AppendLine(ex.StackTrace);
						var msg = error.ToString();
						Compiler.Debug(msg);//Will write to writer.
						Assert.IsTrue(false);
					}
					finally
					{
						writer.Flush();
						output = buffer.ToString();

						using (var console = Console.OpenStandardOutput())
						{
							var stdout = new StreamWriter(console);
							stdout.AutoFlush = true;
							Console.SetOut(stdout);
						}
					}
				}
			}

			return output;
		}
	}
}