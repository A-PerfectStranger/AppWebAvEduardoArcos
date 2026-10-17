Option Explicit
Dim fso, root, source, config, library, lines, line, cleaned, failures
Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(fso.GetParentFolderName(WScript.ScriptFullName))
config = LoadCode(root & "\includes\config.asp")
library = LoadCode(root & "\includes\lib.asp")
cleaned = ""
For Each line In Split(config, vbLf)
    If Left(Trim(line), 9) <> "Response." And Left(Trim(line), 8) <> "Session." Then cleaned = cleaned & line & vbCrLf
Next
ExecuteGlobal cleaned & vbCrLf & library
failures = 0
Check "Entero valido", Eval("ToInt(""42"", 0) = 42")
Check "Entero invalido", Eval("ToInt(""texto"", 7) = 7")
Check "Desbordamiento entero", Eval("ToInt(""2147483648"", 7) = 7")
Check "Bisiesto valido", Eval("ValidBirthDate(""2000-02-29"")")
Check "Bisiesto invalido", Not Eval("ValidBirthDate(""2001-02-29"")")
Check "Mes invalido", Not Eval("ValidBirthDate(""2000-13-01"")")
Check "Formato regional rechazado", Not Eval("ValidBirthDate(""31/12/2000"")")
Check "Formato numerico no ISO rechazado", Not Eval("ValidBirthDate(""2e03-01-01"")")
Check "Fecha vacia permitida", Eval("ValidBirthDate("""")")
Check "Fecha anterior a SQL datetime rechazada", Not Eval("ValidBirthDate(""1700-01-01"")")
Check "Fecha futura rechazada", Not Eval("ValidBirthDate(""9999-01-01"")")
Check "Carpeta canonica", Eval("MapFolder(""songfiles"") = ""SongFiles""")
Check "Ruta externa rechazada", Eval("MapFolder(""../../"") = """"")
If failures > 0 Then WScript.Quit 1
WScript.Echo "13 comprobaciones correctas."
WScript.Quit 0

Sub Check(name, passed)
    If passed Then
        WScript.Echo "OK: " & name
    Else
        failures = failures + 1
        WScript.Echo "ERROR: " & name
    End If
End Sub

Function LoadCode(file)
    Dim stream, text, matches, match, re, result
    Set stream = CreateObject("ADODB.Stream")
    stream.Type = 2
    stream.Charset = "utf-8"
    stream.Open
    stream.LoadFromFile file
    text = stream.ReadText
    stream.Close
    Set re = New RegExp
    re.Pattern = "<%([\s\S]*?)%>"
    re.Global = True
    result = ""
    Set matches = re.Execute(text)
    For Each match In matches
        If Left(match.SubMatches(0), 1) <> "@" Then result = result & match.SubMatches(0) & vbCrLf
    Next
    LoadCode = result
End Function
