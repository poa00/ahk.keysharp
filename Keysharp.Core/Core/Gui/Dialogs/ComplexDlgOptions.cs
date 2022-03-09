﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Keysharp.Core
{
	public class ComplexDlgOptions
	{
		public bool AlwaysOnTop = true;
		public Color? BackgroundColor = null;
		public bool Borderless = false;
		public bool CenterMainText = true;
		public bool CenterSubText = true;
		public int GUIID = 1;
		public bool Hide = false;
		public string MainText = "";
		public Font MainTextFont = new Font(FontFamily.GenericSansSerif, 10);
		public Rectangle ObjectGeometry = new Rectangle(0, 0, 0, 0);
		public string Param1; //Progress percent or Image path or off
		public bool ShowInTaskBar = false;
		public string SubText = "";
		public Font SubTextFont = new Font(FontFamily.GenericSansSerif, 8);
		public Color TextColor = Color.Black;
		public Rectangle WindowGeometry = new Rectangle(0, 0, 0, 0);
		public string WinTitle = "";

		public ComplexDlgOptions()
		{
		}

		public void AppendShowHideTo(IComplexDialog complexDlg)
		{
			if (Param1.ToLowerInvariant() == Core.Keyword_Off)
			{
				complexDlg.Close();
				complexDlg.Dispose();

				if (complexDlg is SplashDialog)
				{
					_ = Dialogs.SplashDialogs.Remove(GUIID);
				}
				else if (complexDlg is ProgressDialog)
				{
					_ = Dialogs.ProgressDialgos.Remove(GUIID);
				}

				return;
			}

			if (Param1.ToLowerInvariant() == Core.Keyword_Show)
			{
				if (!complexDlg.Visible)
					complexDlg.Show();
			}
		}

		/// <summary>
		/// Append the given Options here to a IComplexDialoge.
		/// </summary>
		/// <param name="complexDlg"></param>
		public void AppendTo(IComplexDialog complexDlg)
		{
			//ToDo: Implement the missing Options
			if (complexDlg.InvokeRequired)
			{
				_ = complexDlg.Invoke(new Dialogs.AsyncComplexDialoge(AppendTo), complexDlg);
			}

			complexDlg.TopMost = AlwaysOnTop;

			if (WinTitle != "")
				complexDlg.Title = WinTitle;

			if (SubText != "")
				complexDlg.SubText = SubText;

			if (MainText != "")
				complexDlg.MainText = MainText;

			if (!Hide)
			{
				complexDlg.Show();
			}
		}

		public void ParseComplexOptions(string options)
		{
			var optsItems = new Dictionary<string, Regex>();
			optsItems.Add(Core.Keyword_NotAlwaysOnTop, new Regex("(" + Core.Keyword_NotAlwaysOnTop + ")"));
			optsItems.Add(Core.Keyword_Borderless, new Regex("(" + Core.Keyword_Borderless + ")"));
			optsItems.Add(Core.Keyword_ProgressStartPos, new Regex(Core.Keyword_ProgressStartPos + @"(\d*)"));
			optsItems.Add(Core.Keyword_ProgressRange, new Regex(Core.Keyword_ProgressRange + @"(\d*-\d*)"));
			optsItems.Add(Core.Keyword_ShowInTaskbar, new Regex("(" + Core.Keyword_ShowInTaskbar + ")"));
			optsItems.Add(Core.Keyword_X, new Regex(Core.Keyword_X + @"(\d*)"));
			optsItems.Add(Core.Keyword_Y, new Regex(Core.Keyword_Y + @"(\d*)"));
			optsItems.Add(Core.Keyword_W, new Regex(Core.Keyword_W + @"(\d*)"));
			optsItems.Add(Core.Keyword_H, new Regex(Core.Keyword_H + @"(\d*)"));
			optsItems.Add(Core.Keyword_Hide, new Regex("(" + Core.Keyword_Hide + ")"));
			optsItems.Add(Core.Keyword_Centered, new Regex(Core.Keyword_Centered + @"([0,1]{2})"));
			optsItems.Add(Core.Keyword_ZH, new Regex(Core.Keyword_ZH + @"([-,\d]*)"));
			optsItems.Add(Core.Keyword_ZW, new Regex(Core.Keyword_ZW + @"([-,\d]*)"));
			optsItems.Add(Core.Keyword_ZX, new Regex(Core.Keyword_ZX + @"(\d*)"));
			optsItems.Add(Core.Keyword_ZY, new Regex(Core.Keyword_ZY + @"(\d*)"));
			//ToDo
			//ToDo
			var DicOptions = Options.ParseOptionsRegex(ref options, optsItems, true);
			AlwaysOnTop = (DicOptions[Core.Keyword_NotAlwaysOnTop]?.Length == 0);
			Borderless = (DicOptions[Core.Keyword_Borderless] != "");
			ShowInTaskBar = DicOptions[Core.Keyword_ShowInTaskbar] != "";

			try
			{
				if (DicOptions[Core.Keyword_X] != "")
					WindowGeometry.X = int.Parse(DicOptions[Core.Keyword_X]);

				if (DicOptions[Core.Keyword_Y] != "")
					WindowGeometry.Y = int.Parse(DicOptions[Core.Keyword_Y]);

				if (DicOptions[Core.Keyword_W] != "")
					WindowGeometry.Width = int.Parse(DicOptions[Core.Keyword_W]);

				if (DicOptions[Core.Keyword_H] != "")
					WindowGeometry.Height = int.Parse(DicOptions[Core.Keyword_H]);
			}
			catch (FormatException)
			{
				WindowGeometry = new Rectangle(0, 0, 0, 0);
			}

			Hide = DicOptions[Core.Keyword_Hide] != "";

			if (DicOptions[Core.Keyword_Centered] != "")
			{
				CenterMainText = DicOptions[Core.Keyword_Centered].Substring(0, 1) == "1";
				CenterSubText = DicOptions[Core.Keyword_Centered].Substring(1, 1) == "1";
			}

			try
			{
				if (DicOptions[Core.Keyword_ZX] != "")
					ObjectGeometry.X = int.Parse(DicOptions[Core.Keyword_ZX]);

				if (DicOptions[Core.Keyword_ZY] != "")
					ObjectGeometry.Y = int.Parse(DicOptions[Core.Keyword_ZY]);

				if (DicOptions[Core.Keyword_ZW] != "")
					ObjectGeometry.Width = int.Parse(DicOptions[Core.Keyword_ZW]);

				if (DicOptions[Core.Keyword_ZH] != "")
					ObjectGeometry.Height = int.Parse(DicOptions[Core.Keyword_ZH]);
			}
			catch (FormatException)
			{
				ObjectGeometry = new Rectangle(0, 0, 0, 0);
			}
		}

		public void ParseGuiID(string Param1)
		{
			var RegEx_IsGUIID = new Regex("([0-9]{1,}):");
			var RegEx_GUIID = new Regex("([0-9]*):(.*)");

			if (RegEx_IsGUIID.IsMatch(Param1))
			{
				GUIID = int.Parse(RegEx_GUIID.Match(Param1).Groups[1].Captures[0].ToString());
				this.Param1 = RegEx_GUIID.Match(Param1).Groups[2].Captures[0].ToString();
			}
			else
			{
				GUIID = 1;
				this.Param1 = Param1;
			}
		}
	}
}