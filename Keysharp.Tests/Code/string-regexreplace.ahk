;#Include %A_ScriptDir%/header.ahk
match := RegExReplace("abc123123", "123$", "xyz")

if (match == "abc123xyz")
	FileAppend, pass, *
else
	FileAppend, fail, *

match := RegExReplace("abc123", "i)^ABC")

if (match == "123")
	FileAppend, pass, *
else
	FileAppend, fail, *

match := RegExReplace("abcXYZ123", "abc(.*)123", "aaa$1zzz")

if (match == "aaaXYZzzz")
	FileAppend, pass, *
else
	FileAppend, fail, *

match := RegExReplace("abc123abc456", "abc\d+", "")

if (match == "")
	FileAppend, pass, *
else
	FileAppend, fail, *
	
if (match.OutputVarCount == 2)
	FileAppend, pass, *
else
	FileAppend, fail, *