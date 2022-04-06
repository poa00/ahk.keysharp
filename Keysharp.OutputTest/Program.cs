﻿using static Keysharp.Core.Accessors;
using static Keysharp.Core.Core;
//using static Keysharp.Core.Common.Window.WindowItemBase;
using static Keysharp.Core.Common.Keyboard.HotstringDefinition;
using static Keysharp.Core.Dialogs;
using static Keysharp.Core.Disk;
using static Keysharp.Core.DllHelper;
using static Keysharp.Core.Env;
using static Keysharp.Core.Flow;
using static Keysharp.Core.Function;
using static Keysharp.Core.GuiHelper;
using static Keysharp.Core.Images;
using static Keysharp.Core.ImageLists;
using static Keysharp.Core.Ini;
using static Keysharp.Core.Keyboard;
using static Keysharp.Core.KeysharpObject;
using static Keysharp.Core.Loops;
using static Keysharp.Core.Maths;
using static Keysharp.Core.Menu;
using static Keysharp.Core.Misc;
using static Keysharp.Core.Monitor;
using static Keysharp.Core.Mouse;
using static Keysharp.Core.Network;
using static Keysharp.Core.Options;
using static Keysharp.Core.Processes;
using static Keysharp.Core.Registrys;
using static Keysharp.Core.Screen;
using static Keysharp.Core.Security;
using static Keysharp.Core.SimpleJson;
using static Keysharp.Core.Sound;
using static Keysharp.Core.Strings;
using static Keysharp.Core.ToolTips;
using static Keysharp.Core.Window;
using static Keysharp.Core.Windows.WindowsAPI;
using static Keysharp.Scripting.Script;
using static Keysharp.Scripting.Script.Operator;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: System.CLSCompliantAttribute(true)]
[assembly: Keysharp.Scripting.AssemblyBuildVersionAttribute("0.0.0.1")]

namespace Keysharp.Main
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using Keysharp.Core;
	using Keysharp.Scripting;
	using Array = Keysharp.Core.Array;
	using Buffer = Keysharp.Core.Buffer;


	public sealed class Program
	{

		[System.STAThreadAttribute()]

		public static int Main(string[] args)
		{
			try
			{
				string name = @"*";
				Keysharp.Scripting.Script.Variables.InitGlobalVars();
				Keysharp.Scripting.Script.SetName(name);
				HandleSingleInstance(name, eScriptInstance.Off);
				HandleCommandLineParams(args);
				SetProcessDPIAware();
				CreateTrayMenu();
				var fo = new FuncObj("label_4F98480", null);
				arr = new Keysharp.Core.Array(new object[]
				{
					1L,
					2L,
					3L
				});
				Invoke(GetMethodOrProperty(arr, "Add"), 123L);
				var len = Index(arr, "Count");
				val = "utf-8";
				A_FileEncoding = val;
				ExitApp(0);
				return 0;
			}
			catch (Keysharp.Core.Error kserr)
			{
				if (ErrorOccurred(kserr))
				{
					MsgBox("Uncaught Keysharp exception:\r\n" + kserr);
				}

				ExitApp(1);
				return 1;
			}
			catch (System.Exception mainex)
			{
				MsgBox("Uncaught exception:\r\n" + "Message: " + mainex.Message + "\r\nStack: " + mainex.StackTrace);
				ExitApp(1);
				return 1;
			}
		}

		public static object arr;

		public static object val;

		/*  public static int Main(string[] args)
		    {
		    try
		    {
		        string name = @"*";
		        Keysharp.Scripting.Script.Variables.InitGlobalVars();
		        Keysharp.Scripting.Script.SetName(name);
		        HandleSingleInstance(name, eScriptInstance.Off);
		        HandleCommandLineParams(args);
		        SetProcessDPIAware();
		        CreateTrayMenu();
		        //var val = Maths.Abs(new object[] { -1 });
		        //val = Maths.Abs(new ArrayList { -1 });
		        //val = Maths.Abs(new Keysharp.Core.Array { -1 });
		        //var mi1 = Reflections.FindBuiltInMethod("MsgBox");
		        //var mi2 = Reflections.FindBuiltInMethod("MsgBox");
		        //var mi3 = Reflections.FindBuiltInMethod("MsgBox");
		        //mi3.Invoke(null, new object[] { "asdf" });
		        var fo = new FuncObj("label_4F98480", null);
		        //AddHotstring("::btw", null, ":btw", "btw", "by the way", false);
		        //AddHotstring("::btw", new FuncObj("label_4F98480", null), ":btw", "btw", "", false);
		        AddHotstring("::btw", fo, ":btw", "btw", "", false);
		        //Task.Factory.StartNew(() =>
		        //{
		        //  var mi3 = Reflections.FindBuiltInMethod("MsgBox");
		        //});
		        RunMainWindow(name);
		        ExitApp(0);
		        return 0;
		    }
		    catch (Keysharp.Core.Error kserr)
		    {
		        if (ErrorOccurred(kserr))
		        {
		            MsgBox("Uncaught Keysharp exception:\r\n" + kserr);
		        }

		        ExitApp(1);
		        return 1;
		    }
		    catch (System.Exception mainex)
		    {
		        MsgBox("Uncaught exception:\r\n" + "Message: " + mainex.Message + "\r\nStack: " + mainex.StackTrace);
		        ExitApp(1);
		        return 1;
		    }
		    }

		    public static object x = 123;

		    }*/
		public static object label_4F98480(params object[] args)
		{
			//Hotstring(":X:btw", "MsgBox");
			return string.Empty;
		}

		public static object afunc(params object[] args)

		{
			//MsgBox("MsgBox");
			return string.Empty;
		}
	}
}
