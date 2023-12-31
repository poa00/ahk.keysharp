﻿HtmlVar := "
(
I remember candy bars for 1 €. In England they were less than a £.
What happened?
This text is © 2022 by Anonymous.
"Quote marks" are encoded. 'Single quotes'are not encoded.
Some random symbols: † š ‰ 
)"

MsgBox(EncodeHTML(HtmlVar, 2))
MsgBox(EncodeHTML(HtmlVar, 1))
MsgBox(EncodeHTML(HtmlVar, 3))
MsgBox(EncodeHTML(HtmlVar))

; HTML Entities Encoding
; https://www.autohotkey.com
; Similar to the Transform's HTML sub-command, this function converts a
; string into its HTML equivalent by translating characters whose ASCII
; values are above 127 to their HTML names (e.g. £ becomes &pound;). In
; addition, the four characters "&<> are translated to &quot;&amp;&lt;&gt;.
; Finally, each linefeed (`n) is translated to <br>`n (i.e. <br> followed
; by a linefeed).

; In addition of the functionality above, Flags can be zero or a
; combination (sum) of the following values. If omitted, it defaults to 1.

; - 1: Converts certain characters to named expressions. e.g. € is
;      converted to &euro;
; - 2: Converts certain characters to numbered expressions. e.g. € is
;      converted to &#8364;

; Only non-ASCII characters are affected. If Flags is the number 3,
; numbered expressions are used only where a named expression is not
; available. The following characters are always converted: <>"& and `n
; (line feed).

/*
    i := 1 ; changed
    char := "" ; changed
    out  := ""
    split := ""
    code := ""
*/


EncodeHTML(strg, Flags := 1)
{
    TRANS_HTML_NAMED := 1
    TRANS_HTML_NUMBERED := 2
    ansi := ["euro", "#129", "sbquo", "fnof", "bdquo", "hellip", "dagger", "Dagger", "circ", "permil", "Scaron", "lsaquo", "OElig", "#141", "#381", "#143", "#144", "lsquo", "rsquo", "ldquo", "rdquo", "bull", "ndash", "mdash", "tilde", "trade", "scaron", "rsaquo", "oelig", "#157", "#382", "Yuml", "nbsp", "iexcl", "cent", "pound", "curren", "yen", "brvbar", "sect", "uml", "copy", "ordf", "laquo", "not", "shy", "reg", "macr", "deg", "plusmn", "sup2", "sup3", "acute", "micro", "para", "middot", "cedil", "sup1", "ordm", "raquo", "frac14", "frac12", "frac34", "iquest", "Agrave", "Aacute", "Acirc", "Atilde", "Auml", "Aring", "AElig", "Ccedil", "Egrave", "Eacute", "Ecirc", "Euml", "Igrave", "Iacute", "Icirc", "Iuml", "ETH", "Ntilde", "Ograve", "Oacute", "Ocirc", "Otilde", "Ouml", "times", "Oslash", "Ugrave", "Uacute", "Ucirc", "Uuml", "Yacute", "THORN", "szlig", "agrave", "aacute", "acirc", "atilde", "auml", "aring", "aelig", "ccedil", "egrave", "eacute", "ecirc", "euml", "igrave", "iacute", "icirc", "iuml", "eth", "ntilde", "ograve", "oacute", "ocirc", "otilde", "ouml", "divide", "oslash", "ugrave", "uacute", "ucirc", "uuml", "yacute", "thorn", "yuml"]
    unicode := {0x20AC:1, 0x201A:3, 0x0192:4, 0x201E:5, 0x2026:6, 0x2020:7, 0x2021:8, 0x02C6:9, 0x2030:10, 0x0160:11, 0x2039:12, 0x0152:13, 0x2018:18, 0x2019:19, 0x201C:20, 0x201D:21, 0x2022:22, 0x2013:23, 0x2014:24, 0x02DC:25, 0x2122:26, 0x0161:27, 0x203A:28, 0x0153:29, 0x0178:32}
    split := StrSplit(strg) ; changed
    for (i, char in split) ; changed
    {
        code := Ord(char)
        switch code
        {
            case 10: out .= "<br>`n"
            case 34: out .= "&quot;"
            case 38: out .= "&amp;"
            case 60: out .= "&lt;"
            case 62: out .= "&gt;"
            default:
            if (code >= 160 && code <= 255)
            {
                if (Flags & TRANS_HTML_NAMED)
                    out .= "&" ansi[code-127] ";"
                else if (Flags & TRANS_HTML_NUMBERED)
                    out .= "&#" code ";"
                else
                    out .= char
            }
            else if (code > 255)
            {
                if (Flags & TRANS_HTML_NAMED && unicode[code]) ; changed
                    out .= "&" ansi[unicode[code]] ";" ; changed
                else if (Flags & TRANS_HTML_NUMBERED)
                    out .= "&#" code ";"
                else
                    out .= char
            }
            else
            {
                if (code >= 128 && code <= 159)
                    out .= "&" ansi[code-127] ";"
                else
                    out .= char
            }
        }
    }
    return out
}