; #Include %A_ScriptDir%/header.ahk

Suspend, On

if (A_IsSuspended == 1) 
	FileAppend, pass, *
else
	FileAppend, fail, *

Suspend, Off

Critical, On
x := A_IsCritical

if (x == 1) 
	FileAppend, pass, *
else
	FileAppend, fail, *

Critical, Off
x := A_IsCritical

if (x == 0) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, 1

if (A_TitleMatchMode == 1) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, 2

if (A_TitleMatchMode == 2) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, 3

if (A_TitleMatchMode == 3) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, RegEx

if (A_TitleMatchMode == "regex") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, dummy

if (A_TitleMatchMode == "regex") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, fast

if (A_TitleMatchModeSpeed == "fast") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, slow

if (A_TitleMatchModeSpeed == "slow") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetTitleMatchMode, dummy

if (A_TitleMatchModeSpeed == "slow") 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_TitleMatchMode == "regex") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenWindows, 0

if (A_DetectHiddenWindows == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenWindows, Off

if (A_DetectHiddenWindows == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenWindows, 1

if (A_DetectHiddenWindows == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenWindows, On

if (A_DetectHiddenWindows == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenWindows, dummy

if (A_DetectHiddenWindows == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenText, 0

if (A_DetectHiddenText == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenText, Off

if (A_DetectHiddenText == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenText, 1

if (A_DetectHiddenText == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenText, On

if (A_DetectHiddenText == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

DetectHiddenText, dummy

if (A_DetectHiddenText == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

StringCaseSense, 0

if (A_StringCaseSense == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

StringCaseSense, Off

if (A_StringCaseSense == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

StringCaseSense, 1

if (A_StringCaseSense == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

StringCaseSense, On

if (A_StringCaseSense == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

StringCaseSense, dummy

if (A_StringCaseSense == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, utf-8

if (A_FileEncoding == "utf-8") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, utf-8-raw

if (A_FileEncoding == "utf-8-raw") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, unicode

if (A_FileEncoding == "utf-16") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, utf-16

if (A_FileEncoding == "utf-16") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, utf-16-raw

if (A_FileEncoding == "utf-16-raw") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, ascii

if (A_FileEncoding == "us-ascii") 
	FileAppend, pass, *
else
	FileAppend, fail, *

FileEncoding, dummy

if (A_FileEncoding == "utf-16") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SendLevel, 0

if (A_SendLevel == 0) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SendLevel, -1

if (A_SendLevel == 0) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SendLevel, 1

if (A_SendLevel == 1) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SendLevel, 100

if (A_SendLevel == 100) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SendLevel, 101

if (A_SendLevel == 100) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetStoreCapsLockMode, 0

if (A_StoreCapsLockMode == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetStoreCapsLockMode, Off

if (A_StoreCapsLockMode == "off") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetStoreCapsLockMode, 1

if (A_StoreCapsLockMode == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetStoreCapsLockMode, On

if (A_StoreCapsLockMode == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetStoreCapsLockMode, dummy

if (A_StoreCapsLockMode == "on") 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetKeyDelay, 10

if (A_KeyDelay == 10) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetKeyDelay, 20, 30

if (A_KeyDelay == 20) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_KeyPressDuration == 30) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetKeyDelay, , 40

if (A_KeyDelay == 20) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_KeyPressDuration == 40) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetKeyDelay, 50, 60, Play

if (A_KeyDelay == 20) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_KeyPressDuration == 40) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_KeyDelayPlay == 50) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_KeyPressDurationPlay == 60) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetWinDelay, 200

if (A_WinDelay == 200) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetControlDelay, 200

if (A_ControlDelay == 200) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetMouseDelay, 200

if (A_MouseDelay == 200) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetMouseDelay, 300, Play

if (A_MouseDelay == 200) 
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_MouseDelayPlay == 300) 
	FileAppend, pass, *
else
	FileAppend, fail, *

SetDefaultMouseSpeed, 500

if (A_DefaultMouseSpeed == 500)
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, ToolTip, Screen

if (A_CoordModeToolTip == "Screen")
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, Pixel, Client

if (A_CoordModePixel == "Client")
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, Mouse, Window

if (A_CoordModeMouse == "Window")
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, Caret, Screen

if (A_CoordModeCaret == "Screen")
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, Menu, Screen

if (A_CoordModeMenu == "Screen")
	FileAppend, pass, *
else
	FileAppend, fail, *

CoordMode, Menu, Dummy

if (A_CoordModeMenu == "Window")
	FileAppend, pass, *
else
	FileAppend, fail, *

SetRegView, 32

if (A_RegView == 32)
	FileAppend, pass, *
else
	FileAppend, fail, *

SetRegView, 64

if (A_RegView == 64)
	FileAppend, pass, *
else
	FileAppend, fail, *

SetRegView, 100

if (A_RegView == 64)
	FileAppend, pass, *
else
	FileAppend, fail, *

; need to test with Menu() function here. TODO

if (A_IconHidden == 0)
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_IconTip == "")
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_IconFile == "")
	FileAppend, pass, *
else
	FileAppend, fail, *

if (A_IconNumber == 1)
	FileAppend, pass, *
else
	FileAppend, fail, *
