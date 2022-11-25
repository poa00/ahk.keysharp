﻿using static Keysharp.Core.Accessors;
using static Keysharp.Core.Core;
using static Keysharp.Core.Common.Keyboard.HotstringDefinition;
using static Keysharp.Core.Dialogs;
using static Keysharp.Core.Drive;
using static Keysharp.Core.Dir;
using static Keysharp.Core.DllHelper;
using static Keysharp.Core.Env;
using static Keysharp.Core.File;
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

[assembly: Keysharp.Scripting.AssemblyBuildVersionAttribute("0.0.0.1")]

namespace Keysharp.Benchmark
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	using BenchmarkDotNet.Analysers;
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Configs;
	using BenchmarkDotNet.Exporters;
	using BenchmarkDotNet.Loggers;
	using BenchmarkDotNet.Running;
	using Keysharp.Core;
	using Keysharp.Scripting;
	using Array = Keysharp.Core.Array;
	using Buffer = Keysharp.Core.Buffer;

	[MemoryDiagnoser]
	public class MapReadBenchmark
	{
		private Dictionary<object, object> dkt = new Dictionary<object, object>();
		private Map map = Keysharp.Scripting.Script.Map(), mapScript = Keysharp.Scripting.Script.Map();
		private List<string> strings = new List<string>();

		[Params(10000)]
		public int Size { get; set; }

		[Benchmark]
		public void Map()
		{
			foreach (var s in strings)
				_ = map[s];
		}

		[Benchmark]
		public void MapScript()
		{
			foreach (var s in strings)
				_ = Index(mapScript, s);
		}

		[Benchmark(Baseline = true)]
		public void NativeDictionaryTryGet()
		{
			foreach (var s in strings)
				_ = dkt.TryGetValue(s, out _);
		}

		[GlobalSetup]
		public void Setup()
		{
			Keysharp.Scripting.Script.Variables.InitGlobalVars();
			map = Keysharp.Scripting.Script.Map();
			mapScript = Keysharp.Scripting.Script.Map();
			dkt = new Dictionary<object, object>();
			strings = new List<string>();

			for (var i = 0; i < Size; i++)
			{
				var s = i.ToString();
				map[s] = i;
				mapScript[s] = i;
				dkt[s] = i;
				strings.Add(s);
			}
		}
	}

	[MemoryDiagnoser]
	public class MapWriteBenchmark
	{
		private Dictionary<object, object> dkt = new Dictionary<object, object>();
		private Map map = Keysharp.Scripting.Script.Map(), mapScript = Keysharp.Scripting.Script.Map();
		private List<string> strings = new List<string>();

		public MapWriteBenchmark()
		{
			Keysharp.Scripting.Script.Variables.InitGlobalVars();
			map = Keysharp.Scripting.Script.Map();
			mapScript = Keysharp.Scripting.Script.Map();
			dkt = new Dictionary<object, object>();
			strings = new List<string>();
		}

		[Params(10000)]
		public int Size { get; set; }

		[Benchmark]
		public void Map()
		{
			map.Clear();

			for (var i = 0; i < Size; i++)
				map[strings[i]] = i;
		}

		[Benchmark]
		public void MapScript()
		{
			mapScript.Clear();

			for (var i = 0; i < Size; i++)
				_ = SetObject(strings[i], mapScript, System.Array.Empty<object>(), i);
		}

		[Benchmark(Baseline = true)]
		public void NativeDictionary()
		{
			dkt.Clear();

			for (var i = 0; i < Size; i++)
				dkt[strings[i]] = i;
		}

		[GlobalSetup]
		public void Setup()
		{
			for (var i = 0; i < Size; i++)
				strings.Add(i.ToString());
		}
	}
}