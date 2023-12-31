; Changing MsgBox's Button Names
; https://www.autohotkey.com
; This is a working example script that uses a timer to change
; the names of the buttons in a message box. Although the button
; names are changed, the MsgBox's return value still requires that the
; buttons be referred to by their original names.

#SingleInstance
SetTimer("ChangeButtonNames", 50)
Result := MsgBox("Choose a button:", "Add or Delete", 4)
if (Result == "Yes")
    MsgBox("You chose Add", "Add something")
else
    MsgBox("You chose Delete", "Delete something")

ExitApp()

ChangeButtonNames()
{
    if (!WinExist("Add or Delete"))
        return  ; Keep waiting.
    SetTimer( , 0) 
    WinActivate()
    ControlSetText("&Add", "Button1")
    ControlSetText("&Delete", "Button2")
}

