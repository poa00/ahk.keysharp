;#Include %A_ScriptDir%/header.ahk
x = 

If x != 
	FileAppend, fail, *
else
	FileAppend, pass, *
	
If x = 
	FileAppend, pass, *
else
	FileAppend, fail, *