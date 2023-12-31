﻿namespace Keysharp.Core.Common.Window
{
	internal abstract class ControlManagerBase
	{
		internal abstract long ControlAddItem(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlChooseIndex(int n, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlChooseString(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlClick(object ctrlorpos, object title, string text, string whichButton, int clickCount, string options, string excludeTitle, string excludeText);

		internal abstract void ControlDeleteItem(int n, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlFindItem(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlFocus(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetChecked(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract string ControlGetChoice(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract string ControlGetClassNN(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetEnabled(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetExStyle(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetFocus(object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetHwnd(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetIndex(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract Array ControlGetItems(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract Keysharp.Core.Map ControlGetPos(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetStyle(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract string ControlGetText(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long ControlGetVisible(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlHide(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlHideDropDown(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlMove(int x, int y, int width, int height, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSend(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSendText(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSetChecked(object val, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSetEnabled(object val, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSetExStyle(object val, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSetStyle(object val, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlSetText(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlShow(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void ControlShowDropDown(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long EditGetCurrentCol(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long EditGetCurrentLine(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract string EditGetLine(int n, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long EditGetLineCount(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract string EditGetSelectedText(object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void EditPaste(string str, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract object ListViewGetContent(string options, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract void MenuSelect(object title, string text, string menu, string sub1, string sub2, string sub3, string sub4, string sub5, string sub6, string excludeTitle, string excludeText);

		internal abstract void PostMessage(int msg, int wparam, int lparam, object ctrl, object title, string text, string excludeTitle, string excludeText);

		internal abstract long SendMessage(int msg, object wparam, object lparam, object ctrl, object title, string text, string excludeTitle, string excludeText, int timeout);
	}
}