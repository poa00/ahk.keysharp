;#Include %A_ScriptDir%/header.ahk

l :=
t :=
r :=
b :=
monget := MonitorGet(, &l, &t, &r, &b)

if (l >= 0 && r >= 0 && t >= 0 && b >= 0 && monget > 0)
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"