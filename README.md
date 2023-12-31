# Keysharp #

## How do I get set up? ##

### Installing on Windows ###
* If .NET 7 is not installed on your machine, you need to download and run the ".NET Desktop Runtime" installer from [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
* Download and run the Keysharp installer from the [Downloads](https://bitbucket.org/mfeemster/keysharp/downloads/) page.
	+ The install path can be optionally added to the $PATH varible, so you can run it from the command line from anywhere.
		+ The path entry will be removed upon uninstall.
	+ It also registers Keysharp.exe as the default program to open .ks files. So after installing, double click any .ks file to run it.
	
### Portable Run on Windows ###
* Download and unzip the zip file from the [Downloads](https://bitbucket.org/mfeemster/keysharp/downloads/) page.
	+ CD to the unzipped folder.
	+ Run `.\Keysharp.exe yourfilename.ahk`
	
### Building From Source on Windows ###
* Download Visual Studio 2022
	+ This should install .NET 7. If it doesn't, you need to install it manually from the link above.
* Open Keysharp.sln
* Build all (building the installer is not necessary)
* CD to bin\release\net7.0-windows
* Run `.\Keysharp.exe yourtestfile.ahk`
	
## Overview ##

Keysharp is a fork and improvement of the abandoned IronAHK project, which itself was a C# re-write of the C++ AutoHotkey project.

The intent is for Keysharp to run on Windows, Linux and eventually Mac. For now, only Windows is supported.

This project is in an extremely early state and should not be used on production systems by anyone.

Some general notes about Keysharp's implementation of the [AutoHotkey V2 specification](https://www.autohotkey.com/docs/v2/):

* The syntax is V2 style. While some remnants of V1 will work, it's unintentional and only V2 is supported.

* The operation of Keysharp is different than AHK. While AHK is an interpreted scripting language, Keysharp actually creates a compiled .NET executable and runs it.

* The process for reading and running a script is:
	+ Pass the script to Keysharp.exe which parses it and generates a DOM tree.
	+ The DOM generates C# code for a single program.
	+ The C# program code is compiled into an in-memory executable.
	+ The executable is ran in memory as a new process.
	+ Optionally output the generated C# code to a .cs file for debugging purposes.
	+ Optionally output the generated executable to an .exe file for running standalone in the future.

* Keysharp supports files with the .ahk extension, however installing it will not register it with that extension. Instead, it will register the other extension it supports, `.ks`.

* In addition to Keysharp.exe, there is another executable that ships with the installer named Keyview.exe. This program can be used to see the C# code that is generated from the corresponding script code.
	+ It gives real-time feedback so you can see immediately when you have a syntax error.
	+ It is recommended that you use this to write code.
	+ The editor is very primitive at the moment, and help improving it would be greatly appreciated.

Despite our best efforts to remain compatible with the AHK spec, there are differences. Some of these differences are a reduction in functionality, and others are an increase. There are also slight syntax changes.

## Differences: ##

###	Behaviors/Functionality: ###
* Keysharp follows the .NET memory model.
* There is no variable caching with strings vs numbers. All variables are C# objects.
* AHK says about the inc/dec ++/-- operators on empty variables: "Due to backward compatibility, the operators ++ and -- treat blank variables as zero, but only when they are alone on a line".
	+ Keysharp breaks this and will instead create a variable, initialize it to zero, then increment it.
	+ For example, a file with nothing but the line `x++` in it, will end with a variable named x which has the value of 1.
* Function objects will need to be created passing the name of the function as a string to `Func()`. They are not all created automatically on script startup.
	+ This is done by calling `Func("FunctionName" [, object, paramCount])`
* Exception classes aren't, and can't be, derived from KeysharpObject.
	+ That is because for the Exception mechanics to work in C#, all exception objects must be derived from the base `System.Exception` class, and multiple inheritance is not allowed.
* `CallbackCreate()` does not support the `CDecl/C` option because the program will be run in 64-bit mode.
	+ The `ParamCount` parameter is unused. The callback that gets created supports passing up to 31 parameters and the number that actually gets passed is adjusted internally.
	+ Passing string pointers to `DllCall()` when passing a created callback is strongly recommended against. This is because the string pointer cannot remain pinned, and is likely to crash the program if the pointer gets moved by the GC.
	+ Usage of the created callback will be extremely inefficient, so usage of `CallbackCreate()` is discouraged.
* Deleting a tab via `GuiCtrl.Delete()` does not reassociate the controls that it contains with the next tab. Instead, they are all deleted.
* The size and positioning of some GUI components will be slightly different than AHK because WinForms uses different defaults.
* Never place a key in a `Map` with the name of `"default"` because it will get confused with the Map's property named `Default`.
	+ This applies to any other properties of `Map` as well.
* The class name for statusbar/statusstrip objects created by Keysharp is "WindowsForms10.Window.8.app.0.2b89eaa_r3_ad1". However, for accessing a statusbar created by another, non .NET program, the class name is still "msctls_statusbar321".
* Using the class name with `ClassNN` on .NET controls gives long, version specific names such as "WindowsForms10.Window.8.app.0.2b89eaa_r3_ad1" for a statusbar/statusstrip.
	+ This is because a simpler class names can't be specified in code the way they can in AHK with calls to `CreatWindowEx()`.
	+ These long names may change from machine to machine, and may change for the same GUI if you edit its code.
	+ There is an new `NetClassNN` property alongside `ClassNN`.
	+ All GUI controls created in Keysharp are prefixed with the string "Keysharp", eg: `KeysharpButton`, `KeysharpEdit` etc...
	+ `NetClassNN` will give values like 'KeysharpButton6' (note that the final digit is the same for the `ClassNN` and the `NetClassNN`).
	+ Due to the added simplicity, `NetClassNN` is preferred over `ClassNN` for WinForms controls created with Keysharp.
	+ This is used internally in the index operator for the Gui class, where if a control with a matching `ClassNN` is not found, then controls are searched for their `NetClassNN` values.
* Tooltips function slightly differently.
	+ When specifying a coordinate for a ToolTip, it will attempt to show it relative to the currently focused Keysharp form.
	+ If there is no focused form, it will attempt to show it relative to the last form which was created. If none are found, it will use the main form shown when double clicking the tray icon.
	+ If the form is minimized, then it will attempt to use the RestoreBounds property of the form. This may not work sometimes, so the ToolTip may never show in that case.
	+ Tooltips cannot be used if the script is not persistent (meaning, it has no main window). This is because C# tooltips require a parent control or form.
* `TrayTip()` functions slightly differently.
	+ Muting the sound played by the tip is not supported with the `Mute` option. The sound will be whatever the user has configured in their system settings.
	+ The option `4` to use the program's tray icon is not supported. It is always shown in the title of the tip.
	+ The option `32` to use the large version of the program's tray icon is not supported. Windows will always show the small version.
* `Sleep()` works, but uses `Application.DoEvents()` internally which is not a good programming practice and can lead to hard to solve bugs.
	+ For this reason, it's recommended that users use timers for repeated execution rather than a loop with calls to `Sleep()`.
* The Optimization section of the `#HotIf` documentation doesn't apply to Keysharp because it uses compiled code, thus the expressions are never re-evaluated.
* For transparent controls which are intended to be overlaid over other controls, special steps must be taken.
	+ Specify `BackgroundTrans` in the options for the transparent control.
	+ Then set the `.Parent` property of the transparent control to the one it's laid over.
	+ This makes the x and y coordinates of the control be relative to its parent, which may be different than the overall form if it's a nested control.
* The `#ErrorStdOut` directive will not print to the console unless piping is used. For example:
	+ `.\Keysharp.exe .\test.ahk | more`
	+ `.\Keysharp.exe .\test.ahk | more > out.txt`
* `AddStandard()` detects menu items by string, instead of ID, because WinForms doesn't expose the ID.
* Static function variables are initialized on program startup, rather than the first time the function is called. This is because C# does not support static function variables.
* The built in class methods `__Init()` and `__New()` are not static. They are instance methods so they can access static and instance member variables.
* Function objects are much slower than direct function calls due to the need to use reflection. So for repeated function calls, such as those involving math, it's best to use the functions directly.
* Internally, all vk and sc related variables are treated as int, unlike AHK where some are byte and others are ushort. Continually casting back and forth is probably bad for performance, so everything relating to keys is made to be int across the board.
* The `File` object is internally named `KeysharpFile` so that it doesn't conflict with `System.IO.File`.
* When creating a reference to an enumerator with a call to `obj.OwnProps()`, you must pass `true` to the call to make it return both the name and value of each returned property.
	+ This is done implicitly when calling `obj.OwnProps()` in a `for` loop declaration based on the number of variables declared. i.e. `Name` is name only, `Name,Val` is name and value.
	+ `ObjOwnProps()` takes an optional second argument which is a boolean. Passing `True` means return name and value, passing `False` or empty means return name only.
* Menu items, whether shown or not, have no impact on threading.
* If a `ComObject` with `VarType` of `VT_DISPATCH` and a null pointer value is assinged a non-null pointer value, its type does not change. The Ptr member remains available.

###	Syntax: ###
* The syntax used in `Format()` is exactly that of `string.Format()` in C#, except with 1-based indexing. Traditional AHK style formatting is not supported.
	+ Full documentation for the formatting rules can be found [here](https://learn.microsoft.com/en-us/dotnet/api/system.string.format).
* The default name for the array of parameters in a variadic function is `args`, instead of `params`. This is due to `params` being a reserved word in C#.
* `DllCall()` has the following caveats:
	+ An alternative to passing a `Buffer` object with type `Ptr` to a function which will allocate and place string data into the buffer, the caller can instead use a `StringBuffer` object to hold the new string. `wsprintf()` is one such example.
		+ `StringBuffer` internally uses a `StringBuilder` which is how C# P/Invoke handles string pointers.
		+ This relieves the caller of having to call `StrGet()` on the new string data.
	+ Also use `Ptr` and `StringBuffer` for double pointer parameters such as `LPTSTR*`.
	+ When using type `Str` for string data the function will modify, but not reallocate, the passed in string argument must be passed by `&` reference. `msvcrt.dll\_wcsrev()` is one such example.
		+ This is also supported for strings passed as `AStr`.
	* Passing `GetCommandLine` to `DllCall()` won't work exactly as the examples show. Instead, the type must be `Ptr` and the result must be wrapped in `StrGet()` like:
	+ `StrGet(DllCall("GetCommandLine", "ptr"))`
		+ This holds true for any function which returns a pointer to memory which was allocated inside of a Dll.
* In AHK, when applied to a power operation, the unary operators apply to the entire result. So `-x**y` really means `-(x**y)`.
	+ In Keysharp, this behavior is different due to an inability to resolve bugs in the original code. So follow these rules instead:
	+ To negate the result of a power operation, use parentheses: `-(x**y)`.
	+ To negate one term of a power operation before applying, use parentheses around the term: `(-x)**y` or `-(x)**y`.	
* A leading plus sign on numeric values, such as `+123` or `+0x123` is not supported. It has no effect anyway, so just omit it.
* AHK does not support null, but Keysharp uses it in some cases to determine if a variable has ever been assigned to, such as with `IsSet()`.
* Most operator rules work, but statements like this one from the documentation will not due to the evaluation order of arguments: `++var := x` is evaluated as `++(var := x)`
	+ Use `var := x, ++var` instead.
* Variables used as function call reference arguments cannot be defined and initialized inline like:
	+ `DllCall("QueryPerformanceFrequency", "Int64*", &freq := 0)`
	+ Instead do:
```
		freq := 0
		DllCall("QueryPerformanceFrequency", "Int64*", &freq)
```
* Leading spaces and tabs are not omitted from the strings in continuation strings. They will be parsed as is, according to the options specified. Trailing spaces and tabs will not be trimmed unless `RTrim` is specified.
* In continuation statements, the smart behavior logic for left trimming each line is disabled. Lines are not left trimmed by default and are only left trimmed if `LTrim` is specified.
* Ternary operators with multiple statements in a branch are not supported. Use an `if/else` statement instead if such functionality is needed.
* Quotes in strings cannot be escaped with double quotes, they must use the escape character, \`.
* Dynamic variable references like %x% can only refer to a global variable. There is no way to access a local variable in C# via reflection.
* `Goto` statements cannot use any type of variables. They must be labels known at compile time and function just like goto statements in C#.
* `Goto` statements being called as a function like `Goto("Label")` are not supported. Instead, just use `goto Label`.
* The underlying function object class is called `FuncObj`. This was named so, instead of `Func`, because C# already contains a built in class named `Func`.
	+ `Func()` or `FuncObj()` can still be used to create an instance of `FuncObj`, by passing the name of the desired function as a string, and optionally an object and a parameter count.
* Optional function parameters can be specified using the `?` suffix, however it is not needed or supported when referring to that parameter inside of the function, for example:
```
	myfunc(a, b, c?, d?)
	{
		e := a
		f := b
		g := c ; No question mark needed for c or d.
		h := d
	}
```
* The `#Requires` directive differs in the following ways:
	+ In addition to supporting `AutoHotkey`, it also supports `Keysharp`.
	+ Sub versions such as -alpha and -beta are not supported, only the four numerical values values contained in the assembly version in the form of `0.0.0.0` are supported.	
* Global variables can be accessed from within class methods by using the `program.` prefix: `program.a := 123`.
* Accessing class member variables within member functions does not require the `this.` prefix.
	+ Instead, just reference the member variable using `global`, and that will distinguish it between a local function variable of the same name.
	+ Using `this.` is still supported, but is slower, so avoid using it if possible.
* If a class and sublcass both have properties with the same name, the following rules apply when accessing the properties within a member function in the base class:
	+ `global propname` refers to the property defined in the base class.
	+ `this.propname` refers to the property in the most derived subclass.
	+ To avoid confusion, it is best not to give properties the same name between base and sub classes.
* For any `__Enum()` class method, it should have a parameter value of 2 when returning `Array` or `Map`, since their enumerators have two fields.
* `WinGetPos()` does not take reference parameters. Instead, it takes 4 optional arguments and returns a `Map` with the following entries: `OutX`, `OutY`, `OutWidth`, `OutHeight`.
* Regex does not use Perl Compatible Regular Expressions. Instead, it uses the built in C# RegEx library. This results in the following changes from AHK:
	+ The following options are different:
		+ -A: Forces the pattern to be anchored; that is, it can match only at the start of Haystack. Under most conditions, this is equivalent to explicitly anchoring the pattern by means such as `^`.
			+ -This is not supported, instead just use `^` or `\A` in your regex string.

		+ -C: Enables the auto-callout mode.
			+ -This is not supported. C# regular expressions don't support calling an event handler for each match. You must manually iterate through the matches yourself.
			
		+ -D: Forces dollar-sign ($) to match at the very end of Haystack, even if Haystack's last item is a newline. Without this option, $ instead matches right before the final newline (if there is one). Note: This option is ignored when the `m` option is present.
			+ -This is not supported, instead just use `$`. However, this will only match `\n`, not `\r\n`. To match the `CR/LF` character combination, include `\r?$` in the regular expression pattern.

		+ -J: Allows duplicate named subpatterns.
			+ -This is not supported.

		+ -S: Studies the pattern to try improve its performance.
			+ -This is not supported. All RegEx objects are internally created with the `RegexOptions.Compiled` option specified, so performance should be reasonable.

		+ -U: Ungreedy.	
			+ -This is not supported, instead use `?` after: `*, ?, +, and {min,max}`.

		+ -X: Enables PCRE features that are incompatible with Perl.
			+ -This is not supported because it's Perl specific.

		+ ``` `a `n `r ```: Causes specific characters to be recognized as newlines.
			+ -This is not supported.

		+ `\K` is not supported, instead, try using `(?<=abc)`
			
	+ PCRE exceptions are not thrown when there is an error, instead C# regex exceptions are thrown.
	+ To learn more about C# regular expressions, see [here](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions).
				
###	Additions/Improvements: Keysharp has added/improved the following: ###
* Add new methods to `Array`:
	+ `Add(value) => Integer` which should be more efficient than `Push(values*)` when adding a single item because it is not variadic. It also returns the length of the array after the add completes.
	+ `Filter(callback: (value [, index]) => Boolean) => Array`
	+ `FindIndex(callback: (value [, index]) => Boolean, start_index := 1) => Integer`
		+ If `start_index` is less than 0 then do a reverse lookup.
	+ `IndexOf(val_to_find, start_index := 1) => Integer`
	+ `Join(separator := ',') => String`
	+ `MapTo(callback: (value [, index]) => Any) => Array`
	+ `Sort(callback: (a, b) => Integer) => $this`. Sort in place. The callback should use the usual logic of returning -1 when `a < b`, 0 when `a == b` and 1 when `a > b`.
* A new function `Atan2(y, x) => Double` while AHK only supports `Atan(value)`.
* Hyperbolic versions of the trigonometric functions:
	+ `Sinh(value) => Double`
	+ `Cosh(value) => Double`
	+ `Tanh(value) => Double`
* A new property `A_LoopRegValue` which makes it easy to get a registry value when using `Loop Reg`.
* `Run/RunWait()` can take an extra string for the argument instead of appending it to the program name string. However, the original functionality still works too.
	+ The new signature is: `Run/RunWait(Target [, WorkingDir, Options, Args])`.
* `ListView` supports a new method `DeleteCol(col) => Boolean` to remove a column. The value returned indicates whether the column was found and deleted.
* `TabControl` supports a new method `SetTabIcon(tabIndex, imageIndex)` to relieve the caller of having to use `SendMessage()`.
* `Menu` supports several new methods:
	+ `HideItem()`, `ShowItem()` and `ToggleItemVis()` which can show, hide or toggle the visibility of a specific menu item.
	+ `MenuItemId()` to get the name of a menu item, rather than having to use `DllCall()`.
	+ `SetForeColor()` to set the fore (text) color of a menu item.
* The 40 character limit for hotstring abbreviations has been removed. There is no limit to the length.
* `FileGetSize()` supports `G` and `T` for gigabytes and terabytes.
* `TreeView` supports a new method `GetNode(nodeIndex) => TreeNode` which retrieves a raw winforms TreeNode object based on a passed in ID.
* `SubStr()` uses a default of 1 for the second parameter, `StartingPos`, to relieve the user of always having to specify it.
* New string functions:
	+ `NormalizeEol(str, eol) => String` to take in a string and make all line endings match the value passed in, or the default for the current environment.
	+ `StartsWith(value, token [,comparison]) => Boolean` and `EndsWith(value, token [,comparison]) => Boolean` to determine if the beginning or end of a string start/end with a given string.
	+ 'Join(separator, params*) => String` to join each parameter together as a string, separated by `separator`.
		+ Pass params as `params*` if it's a collection.
* The v1 `Map` methods `MaxIndex()` and `MinIndex()` are still supported. They are also supported for `Array`.
* New function `GetScreenClip(x, y, width, height [, filename]) => Bitmap` can be used to return a bitmap screenshot of an area of the screen and optionally save it to file.
* Rich text boxes are supported by passing `RichEdit` to `Gui.Add()`. The same options from `Edit` are supported with the following caveats:
	+ `Multiline` is true by default.
	+ `WantReturn` and `Password` are not supported.
	+ `Uppercase` and `Lowercase` are supported, but only for key presses, not for pasting.
* Loading icons from .NET DLLs is supported by passing the name of the icon resource in place of the icon number.
* A new accessor `A_KeysharpCorePath` provides the full path to the Keysharp.Core.dll file.
* A new function `CopyImageToClipboard(filename [,options])` is supported which copies an image to the clipboard.
	+ Uses the same arguments as `LoadPicture()`.
	+ This is a fully separate copy and does not share any handle, or perform any file locking with the original image being read.
* When sending a string through `SendMessage()` using the `WM_COPYDATA` message type, the caller is no longer responsible for creating the special `COPYDATA` struct.
	+ Instead, just pass `WM_COPYDATA (0x4A)` as the message type and the string as the `lparam`, and `SendMessage()` will handle it internally.
* A new function `Collect()` which calls `GC.Collect()` to force a memory collection.
	+ This rarely ever has to be used in properly written code.
* In addition to using `#ClipboardTimeout`, a new accessor named `A_ClipboardTimeout` can be used at any point in the program to get or set that value.
* AHK does not support reloading a compiled script, however Keysharp does.
* `A_EventInfo` is not limited to positive values when reporting the mouse wheel scroll amount.
	+ When scrolling up, the value will be positive, and negative when scrolling down.
* New accessors:
	+ `A_AllowTimers` returns whether timers are allowed or not. It's also easier to set this value rather than call `Thread("NoTimers")`.
	+ `A_CommandLine` returns the command line string. This is preferred over passing `GetCommandLine` to `DllCall()` as noted above.
	+ `A_HotstringNoMouse` returns whether mouse clicks are prevented from resetting the hotstring recognizer because `#Hotstring NoMouse` was specified.
	+ `A_MaxThreads` returns the value `n` specified with `#MaxThreads n`.
	+ `A_NoTrayIcon` returns whether the tray icon was hidden with #NoTrayIcon.
	+ `A_UseHook` returns the value `n` specified with `#UseHook n`.
	+ `A_SuspendExempt` returns whether subsequent hotkeys and hotstrings will be exmpt from suspension because `#SuspendExempt true` was specified.
	+ `A_WinActivateForce` returns whether the forceful method of activating a window is in effect because `#WinActivateForce` was specified.
	+ `A_DefaultHotstringCaseSensitive` returns the default hotstring case sensitivity mode.
	+ `A_DefaultHotstringConformToCase` returns the default hotstring case conformity mode.
	+ `A_DefaultHotstringDetectWhenInsideWord` returns the default hotstring word detection mode.
	+ `A_DefaultHotstringDoBackspace` returns the default hotstring backspacing mode.
	+ `A_DefaultHotstringDoReset` returns the default hotstring resetting mode.
	+ `A_DefaultHotstringEndCharRequired` returns the default hotstring ending character mode.
	+ `A_DefaultHotstringEndChars` returns the default hotstring ending characters.
	+ `A_DefaultHotstringKeyDelay` returns the default hotstring key delay length in milliseconds.
	+ `A_DefaultHotstringOmitEndChar` returns the default hotstring ending character replacement mode.
	+ `A_DefaultHotstringPriority` returns the default hotstring priority.
	+ `A_DefaultHotstringSendMode` returns the default hotstring sending mode.
	+ `A_DefaultHotstringSendRaw` returns the default hotstring raw sending mode.
* `Log(number, base := 10)` is by default base 10, but you can pass a double as the second parameter to specify a custom base.
* In `SetTimer()`, the priority is not in the range -2147483648 and 2147483647, instead it is only 0-4.
	+ The callback is passed the function object as the first argument, and the date/time the timer was triggered as a YYYYMMDDHH24MISS string for the second argument.
	+ This allows the handler to alter the timer by passing the function object back to another call to `SetTimer()`.
	+ Timers are not disabled when the program menu is shown.
* Using an underscore `_` to discard the result of an expression is supported the same way it is in C# like:
	+ `_ := myfunc()`
* `super` is not restricted to being used within a class's code. It can be accessed outside of the class like so:
```
	classobj := myclass()
	classobj.super.a := 123
```
* Reference parameters for functions using `&` are supported with the following improvements and caveats:
	+ Passing class members, array indexes and map values by reference is supported.
		+ `func(&classobj.classprop)`
		+ `func(&myarray[5])`
		+ `func(&mymap["mykey"])`
	+ Reference parameters in functions work for class methods, global functions, built in functions, lambdas and function objects.
		+ Lambdas with a single reference parameter can be declared with no parentheses:
			+ `lam := &a => a := (a * 2)`
	+ For an argument to be passed as a reference, the function parameter in that position must be declared as a reference:
		+ `func(&p1) { }`
	+ When passing a class member variable as a dynamic reference to a function from within another function of that same class, the `this` prefix must be used:
```
	class myclass
	{
		x := 11
		y11 := 0
				
		myclassreffunc(&val)
		{
		}
				
		callmyclassreffunc()
		{
			myclassreffunc(&this.y%x%) ; Use this.
		}
	}
```
* New function to encrypt or decrypt an object using the AES algorithm: `AES(value, key, decrypt := false) => Array`.
* New functions to generate hash values using various algorithms: `MD5(value) => String`, `SHA1(value) => String`, `SHA256(value) => String`, `SHA384(value) => String`, `SHA512(value) => String`.
* New function to calculate the CRC32 polynomial of an object: `CRC32(value) => Integer`.
* New function to generate a secure cryptographic random number: `SecureRandom(min, max) => Decimal`
* New class and functions for managing real threads which are not related to green threads that are used for the rest of the project.
	+ A `RealThread` is created by calling `StartRealThread()`.
```
	class RealThread
	{
		RealThread(Task)
		RealThread ContinueWith(funcobj[, params*]) ; Call `funcobj` after the task completes, optionally passing `params` to it and return a new `RealThread` object for the continuation thread.
		Wait([timeout]) ; Wait until the thread object which was passed to the constructor completes. Optionally return after a specified timeout period in milliseconds elapses.
	}
```
	+ `StartRealThread(funcobj [, params*])` ; Call `funcobj` in a real thread, optionally passing `params` to it, and return a `RealThread` object.
	+ `LockRun(lockobj, funcobj [, params*])` ; Call `funcobj` inside of a lock on `lockobj`, optionally passing `params` to it.
		+ `lockobj` must be initialized to some value, such as an empty string.

###	Removals: ###
* COM is only partially implemented.
* Nested classes are not supported.
* Nested functions are not supported.
* `VarSetStrCapacity()` and `ObjGet/SetCapacity()` have been removed because C# manages its own memory internally.
* `ListLines()` is omitted because C# doesn't support it.
* `ObjPtr()` is not implemented because objects can be moved by the GC.
* There is no such thing as dereferencing in C#, so the `*` dereferencing operator is not supported.		
* The `R`, `Dn` or `Tn` parameters in `FormatDateTime()` are not supported, except for 0x80000000 to disallow user overrides.
	+ If you want to specify a particular format or order, do it in the format argument. There is no need or reason to have one argument alter the other.
	+ [Here](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) is a list of the C# style DateTime formatters which are supported.
* Static text controls do not send the Windows `API WM_CTLCOLORSTATIC (0x0138)` message to their parent controls like they do in AHK.
* `IsAlpha()`, `IsUpper()`, `IsLower()` do not accept a locale parameter because all strings are Unicode.
* Renaming Keysharp.exe to run a specific script by default will not work.
* Double click handlers for buttons are not supported.
* Spin boxes with paired buddy controls are not supported. Just use the regular spin box in C#.
* `IL_Create()` only takes one parameter: `LargeIcons`. `InitialCount` and `GrowCount` are no longer needed because memory is handled internally.
* For slider events, the second parameter passed to the event handler will always be `0` because it's not possible to retrieve the method by which the slider was moved in C#.
* `PixelGetColor()` does not accept a mode as its third parameter.
* `SoundGetInterface()` is not implemented because it's COM.
* The sound functions don't have the concept of a `component` because the underlying NAudio library does not group hardware into components.
	+ However, the component parameter of the sound functions is kept for backward compatibility. Just use device name or index instead.
* The `3` and `5` options for `DirSelect()` don't apply in C#.
* Only `Tab3` is supported, no older tab functionality is present.
* When adding a `ListView`, the `Count` option is not supported because C# can't preallocate memory for a `ListView`.
* Function references are supported, but the VarRef object is not supported.
* The address of a variable cannot be taken using the reference operator except when passing an argument to a function.
	+ `x := &var ; not supported`
	+ `functhattakesref(&x) ; supported`
* `OnMessage()` doesn't observe any of the threading behavior mentioned in the documentation because threading has not been implemented yet. Instead, the handlers are called inline.
	+ The third parameter is just used to specify if the handler should be inserted, added or removed from the list of handlers for the specified message.
	+ A GUI object is required for `OnMessage()` to be used.
* Pausing a script is not supported because a Keysharp script is actually a running program.
	+ The pause menu item has been removed.
* `ObjAddRef()` and `ObjPtrAddRef()` do not have an effect for non-COM objects. Instead, use the following:
	+ `newref := theobj` ; adds 1 to the reference count
	+ `newref := ""` ; subtracts 1 from the reference count
* `#Warn` to enable/disable compiler warnings is not supported yet.
* The `/script` option for compiled scripts does not apply and is therefore not implemented.
* The Help and Window Spy menu items are not implemented yet.
* `Download()` only supports the `*0` option, and not any other numerical values.
* Static class variables cannot be overridden in subclasses. So regardless of the class used to access the variable, they all refer to the same static member variable.
* Static class member variable initializers like static `x.y := 42` are not supported. Instead, just initialize in one step like static `x := { y, 42 }`.
* Within a class, a property and a method cannot have the same name. However, they can if one is in a base class and the other is in a subclass.
* The concept of a class prototype is not supported, because it doesn't exist in C# classes. Thus, there is no `.Prototype` member.
* Properties other than `__Item[]` cannot take parameters. If you need to pass a parameter, use a method instead.
	+ This also applies to properties which have been dynamically defined with `DefineProp()`.
* Static `__Item[]` properties are not allowed, only instance `__Item[]` properties. This is because C# does not support static indexers.
* The built in classes `Array` and `Map` do not have a property named `__Item[]` because in C#, the only properties which can have an index passed to them are the `this[]` properties.
	+ Just use the brackets directly. However, when overriding, using `__Item[]` will work if you derive from `Array` or `Map`.
* When passing `"Interrupt"` as the second argument to `Thread()`, the third argument for `LineCount` is not supported because Keysharp does not support line level awareness.

## Who do I talk to? ##

Please make an account here and post a ticket.