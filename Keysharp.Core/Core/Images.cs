﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Keysharp.Core.Common;

namespace Keysharp.Core
{
	public static class Images
	{
		public static void CopyImageToClipboard(object obj0, object obj1 = null)
		{
			var filename = obj0.As();
			var options = obj1.As();
			var opts = Options.ParseOptions(options);
			var width = int.MinValue;
			var height = int.MinValue;
			var icon = "";
			object iconnumber = 0;

			foreach (var opt in opts)
			{
				if (Options.TryParse(opt, "w", ref width)) { }
				else if (Options.TryParse(opt, "h", ref height)) { }
				else if (Options.TryParseString(opt, "icon", ref icon)) { iconnumber = ImageHelper.PrepareIconNumber(icon); }
			}

			var ext = System.IO.Path.GetExtension(filename).ToLower();

			if (ext == ".cur")
			{
				using (var cur = new Cursor(filename))
				{
					Clipboard.SetImage(Keysharp.Core.Common.ImageHelper.ConvertCursorToBitmap(cur));
				}
			}
			else if (ImageHelper.LoadImage(filename, width, height, iconnumber) is Bitmap bmp)
			{
				Clipboard.SetImage(new Bitmap(bmp));
			}
		}

		public static object LoadPicture(object obj0)
		{
			object obj = null;
			return LoadPicture(obj0, null, ref obj);
		}

		public static object LoadPicture(object obj0, object obj1)
		{
			object obj = null;
			return LoadPicture(obj0, obj1, ref obj);
		}

		public static object LoadPicture(object obj0, object obj1, ref object obj2)
		{
			var filename = obj0.As();
			var options = obj1.As();
			var handle = IntPtr.Zero;
			var opts = Options.ParseOptions(options);
			var width = int.MinValue;
			var height = int.MinValue;
			var icon = "";
			object iconnumber = 0;
			var disposeHandle = false;

			foreach (var opt in opts)
			{
				if (Options.TryParse(opt, "w", ref width)) { }
				else if (Options.TryParse(opt, "h", ref height)) { }
				else if (Options.TryParseString(opt, "icon", ref icon)) { iconnumber = ImageHelper.PrepareIconNumber(icon); }
			}

			var ext = System.IO.Path.GetExtension(filename).ToLower();

			if (ext == ".cur")
			{
				var cur = new Cursor(filename);
				handle = cur.Handle;
				obj2 = 2L;
			}
			else if (ImageHelper.LoadImage(filename, width, height, iconnumber) is Bitmap bmp)
			{
				//Calling GetHbitmap() and GetHicon() creates a persistent handle that keeps the bitmap in memory, and must be destroyed later.
				if (ImageHelper.IsIcon(filename))
				{
					handle = bmp.GetHicon();
					disposeHandle = true;
					obj2 = 1L;
				}
				else
				{
					handle = bmp.GetHbitmap();
					disposeHandle = true;
					obj2 = 0L;
				}
			}

			return new GdiHandleHolder(handle, disposeHandle);
		}
	}
}