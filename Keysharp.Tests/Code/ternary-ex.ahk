;#Include %A_ScriptDir%/header.ahk

z := 3 > 2 ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 3 > 2 ? -1 : 10

if (z = -1)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := 2 > 3 ? 1 : 10

if (z = 10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 2 > 3 ? 1 : -10

if (z = -10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3 > 2 ? 1 : 10

if (z = 10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3 < 2 ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 3 > -2 ? -1 : 10

if (z = -1)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := -3 > -2 ? -1 : -10

if (z = -10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3 < -2 ? -1 : -10

if (z = -1)
	FileAppend, pass, *
else
	FileAppend, fail, *


z := 3.1 > 2.1 ? 1.5 : 10.3

if (z = 1.5)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 3.1 > 2.1 ? -1.5 : 10.3

if (z = -1.5)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := 2.1 > 3.1 ? 1.5 : 10.3

if (z = 10.3)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 2.1 > 3.1 ? 1.5 : -10.3

if (z = -10.3)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3.1 > 2.1 ? 1.5 : 10.3

if (z = 10.3)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3.1 < 2.1 ? 1.5 : 10.3

if (z = 1.5)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := 3.1 > -2.1 ? -1.5 : 10.3

if (z = -1.5)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := -3.1 > -2.1 ? -1.5 : -10.3

if (z = -10.3)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := -3.1 < -2.1 ? -1.5 : -10.3

if (z = -1.5)
	FileAppend, pass, *
else
	FileAppend, fail, *


x := 2
y := 3
z := y > x ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := y < x ? 1 : 10

if (z = 10)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := y > x ? -1 : 10

if (z = -1)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := y < x ? 1 : -10

if (z = -10)
	FileAppend, pass, *
else
	FileAppend, fail, *


z := -y > x ? 1 : 10

if (z = 10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := y > -x ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
z := -y > x ? -1 : 10

if (z = 10)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := y < -x ? 1 : -10

if (z = -10)
	FileAppend, pass, *
else
	FileAppend, fail, *


x := 2
y := 4
z := y = (x * x) ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

x := 2
y := -4
z := y = -(x * x) ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

x := 2
y := 4
z := y = 4 ? (x * x * x) : 10

if (z = 8)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
x := 2
y := 5
z := y = 4 ? 1 : (x * x * x)

if (z = 8)
	FileAppend, pass, *
else
	FileAppend, fail, *

x := 2
y := 4
z := y == (x * x) ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

x := 2
y := -4
z := y == -(x * x) ? 1 : 10

if (z = 1)
	FileAppend, pass, *
else
	FileAppend, fail, *

x := 2
y := 4
z := y == 4 ? (x * x * x) : 10

if (z = 8)
	FileAppend, pass, *
else
	FileAppend, fail, *
	
x := 2
y := 5
z := y == 4 ? 1 : (x * x * x)

if (z = 8)
	FileAppend, pass, *
else
	FileAppend, fail, *

z := x > 0 ? (x := 22) : (y := 33)

if (x = 22)
	FileAppend, pass, *
else
	FileAppend, fail, *
