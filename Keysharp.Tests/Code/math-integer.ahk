;#Include %A_ScriptDir%/header.ahk

if (-1 == Integer(-1))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"

if (-2 == Integer(-2.1))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"

if (0 == Integer(0))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"

if (0 == Integer(-0))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"

if (0 == Integer(0.5))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"
	
if (1 == Integer(1.000001))
	FileAppend, "pass", "*"
else
	FileAppend, "fail", "*"
