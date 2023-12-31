﻿using System;
using System.Runtime.InteropServices;

namespace Keysharp.Core.Linux.X11.Events
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct XExposeEvent
	{
		internal XEventName type;
		internal IntPtr serial;
		internal bool send_event;
		internal IntPtr display;
		internal IntPtr window;
		internal int x;
		internal int y;
		internal int width;
		internal int height;
		internal int count;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct XGraphicsExposeEvent
	{
		internal XEventName type;
		internal IntPtr serial;
		internal bool send_event;
		internal IntPtr display;
		internal IntPtr drawable;
		internal int x;
		internal int y;
		internal int width;
		internal int height;
		internal int count;
		internal int major_code;
		internal int minor_code;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct XNoExposeEvent
	{
		internal XEventName type;
		internal IntPtr serial;
		internal bool send_event;
		internal IntPtr display;
		internal IntPtr drawable;
		internal int major_code;
		internal int minor_code;
	}
}