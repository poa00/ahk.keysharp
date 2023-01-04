﻿using static Keysharp.Core.Accessors;
using static Keysharp.Core.Core;
//using static Keysharp.Core.Common.Window.WindowItemBase;
using static Keysharp.Core.Common.Keyboard.HotkeyDefinition;
using static Keysharp.Core.Common.Keyboard.HotstringDefinition;
using static Keysharp.Core.Dialogs;
using static Keysharp.Core.Dir;
using static Keysharp.Core.Drive;
using static Keysharp.Core.DllHelper;
using static Keysharp.Core.Env;
using static Keysharp.Core.File;
using static Keysharp.Core.Flow;
using static Keysharp.Core.Function;
using static Keysharp.Core.GuiHelper;
using static Keysharp.Core.Images;
using static Keysharp.Core.ImageLists;
using static Keysharp.Core.Ini;
using static Keysharp.Core.Input;
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

[assembly: Keysharp.Scripting.AssemblyBuildVersionAttribute("0.0.0.1")]

namespace Keysharp.CompiledMain
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
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


	public class program
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
				CreateTrayMenu();
				UserMainCode();
				Keysharp.Core.Flow.Sleep(-2);
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
				var ex = mainex.InnerException ?? mainex;

				if (ex is Keysharp.Core.Error kserr)
				{
					if (ErrorOccurred(kserr))
					{
						MsgBox("Uncaught Keysharp exception:\r\n" + kserr);
					}
				}
				else
				{
					MsgBox("Uncaught exception:\r\n" + "Message: " + ex.Message + "\r\nStack: " + ex.StackTrace);
				}

				;

				ExitApp(1);

				return 1;
			}
		}

		public static void UserMainCode()
		{
			x = 11L;
			y11 = 123L;
			z = Vars[string.Concat<object>(new object[]
			{
				"y",
				x
			})];

			if (IfTest(Operate(IdentityEquality, z, 123L)))
			{
				FileAppend("pass", "*");
			}
			else
			{
				FileAppend("fail", "*");
			}
		}

		public static object x { get; set; }

		public static object y11 { get; set; }

		public static object z { get; set; }
	}
}
