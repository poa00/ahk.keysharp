;#Include %A_ScriptDir%/header.ahk

monget := MonitorGetCount()

if (monget >= 0)
	FileAppend, pass, *
else
  	FileAppend, fail, *