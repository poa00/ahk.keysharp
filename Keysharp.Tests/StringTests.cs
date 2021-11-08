﻿using System;
using System.Text;
using Keysharp.Core;
using Keysharp.Scripting;
using NUnit.Framework;
using static Keysharp.Core.Strings;

namespace Keysharp.Tests
{
	public partial class Scripting
	{
		//[Test]
		//public void TestHotstringCaps()
		//{
		//  var kbh = new Keysharp.Core.Windows.KeyboardHook();
		//  var str = kbh.ApplyCase("werent", "weren't");
		//  Assert.AreEqual(str, "weren't");
		//  str = kbh.ApplyCase("Werent", "weren't");
		//  Assert.AreEqual(str, "Weren't");
		//  str = kbh.ApplyCase("WerEnt", "weren't");
		//  Assert.AreEqual(str, "WerEn't");
		//  str = kbh.ApplyCase("WerEnT", "weren't");
		//  Assert.AreEqual(str, "WerEn'T");
		//  str = kbh.ApplyCase("WERENT", "weren't");
		//  Assert.AreEqual(str, "WEREN'T");
		//}

		[Test, Category("String")]
		public void Chr()
		{
			var ord = Strings.Chr(116);
			Assert.AreEqual("t", ord);
			Assert.IsTrue(TestScript("string-chr", true));
		}

		[Test, Category("String")]
		public void CompareCaseEx() => Assert.IsTrue(TestScript("string-compare-case-ex", true));

		[Test, Category("String")]
		public void Escape() => Assert.IsTrue(TestScript("string-escape", true));

		[Test, Category("String")]
		public void ConcatEx() => Assert.IsTrue(TestScript("string-concat-ex", true));

		[Test, Category("String")]
		public void Format()
		{
			var s = Strings.Format("{1}", 123);
			Assert.AreEqual(s, "123");
			s = Strings.Format("{1}", 123.456);
			Assert.AreEqual(s, "123.456");
			Assert.IsTrue(TestScript("string-format", true));
		}

		[Test, Category("String")]
		public void FormatTime()
		{
			var dt = Conversions.ToDateTime("2020");
			Assert.IsTrue(dt == new DateTime(2020, 1, 1));
			dt = Conversions.ToDateTime("202007");
			Assert.IsTrue(dt == new DateTime(2020, 7, 1));
			dt = Conversions.ToDateTime("20200704");
			Assert.IsTrue(dt == new DateTime(2020, 7, 4));
			dt = Conversions.ToDateTime("2020070420");
			Assert.IsTrue(dt == new DateTime(2020, 7, 4, 20, 0, 0));
			dt = Conversions.ToDateTime("202007042030");
			Assert.IsTrue(dt == new DateTime(2020, 7, 4, 20, 30, 0));
			dt = Conversions.ToDateTime("20200704203040");
			Assert.IsTrue(dt == new DateTime(2020, 7, 4, 20, 30, 40));
			var str = "20200704";
			var dtstr = Strings.FormatTime(str, "d");
			Assert.AreEqual("4", dtstr);
			dtstr = Strings.FormatTime(str, "dd");
			Assert.AreEqual("04", dtstr);
			dtstr = Strings.FormatTime(str, "ddd");
			Assert.AreEqual("Sat", dtstr);
			dtstr = Strings.FormatTime(str, "dddd");
			Assert.AreEqual("Saturday", dtstr);
			dtstr = Strings.FormatTime(str, "M");
			Assert.AreEqual("7", dtstr);
			dtstr = Strings.FormatTime(str, "MM");
			Assert.AreEqual("07", dtstr);
			dtstr = Strings.FormatTime(str, "MMM");
			Assert.AreEqual("Jul", dtstr);
			dtstr = Strings.FormatTime(str, "MMMM");
			Assert.AreEqual("July", dtstr);
			dtstr = Strings.FormatTime(str, "y");
			Assert.AreEqual("20", dtstr);
			dtstr = Strings.FormatTime(str, "yy");
			Assert.AreEqual("20", dtstr);
			str = "20020704";
			dtstr = Strings.FormatTime(str, "y");
			Assert.AreEqual("2", dtstr);
			dtstr = Strings.FormatTime(str, "yy");
			Assert.AreEqual("02", dtstr);
			str = "20200704";
			dtstr = Strings.FormatTime(str, "yyyy");
			Assert.AreEqual("2020", dtstr);
			dtstr = Strings.FormatTime(str, "gg");
			Assert.AreEqual("AD", dtstr);
			str = "20200704070809";
			dtstr = Strings.FormatTime(str, "h");
			Assert.AreEqual("7", dtstr);
			dtstr = Strings.FormatTime(str, "hh");
			Assert.AreEqual("07", dtstr);
			dtstr = Strings.FormatTime(str, "H");
			Assert.AreEqual("7", dtstr);
			dtstr = Strings.FormatTime("20200704200809", "HH");
			Assert.AreEqual("20", dtstr);
			dtstr = Strings.FormatTime(str, "m");
			Assert.AreEqual("8", dtstr);
			dtstr = Strings.FormatTime(str, "mm");
			Assert.AreEqual("08", dtstr);
			dtstr = Strings.FormatTime(str, "s");
			Assert.AreEqual("9", dtstr);
			dtstr = Strings.FormatTime(str, "ss");
			Assert.AreEqual("09", dtstr);
			dtstr = Strings.FormatTime(str, "t");
			Assert.AreEqual("A", dtstr);
			dtstr = Strings.FormatTime(str, "tt");
			Assert.AreEqual("AM", dtstr);
			dtstr = Strings.FormatTime(str, "time");
			Assert.AreEqual("7:08 AM", dtstr);
			dtstr = Strings.FormatTime(str, "ShortDate");
			Assert.AreEqual("7/4/2020", dtstr);
			dtstr = Strings.FormatTime(str, "LongDate");
			Assert.AreEqual("Saturday, July 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str, "YDay");
			Assert.AreEqual("186", dtstr);
			dtstr = Strings.FormatTime("20200101", "YDay0");
			Assert.AreEqual("001", dtstr);
			dtstr = Strings.FormatTime(str, "WDay");
			Assert.AreEqual("7", dtstr);
			dtstr = Strings.FormatTime(str, "YWeek");
			Assert.AreEqual("202027", dtstr);
			dtstr = Strings.FormatTime(str, "");
			Assert.AreEqual("7:08 AM Saturday, July 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str);
			Assert.AreEqual("7:08 AM Saturday, July 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str + " R");
			Assert.AreEqual("Saturday, July 4, 2020 7:08 AM", dtstr);
			dtstr = Strings.FormatTime(str + " LSys", "");
			Assert.AreEqual("7:08 AM Saturday, July 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str + " L1033", "");
			Assert.AreEqual("7:08 AM Saturday, July 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str + " L2058", "");
			Assert.AreEqual("7:08 a. m. sábado, julio 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str + " L0x80A", "");
			Assert.AreEqual("7:08 a. m. sábado, julio 4, 2020", dtstr);
			dtstr = Strings.FormatTime(str, "yyyy");
			Assert.AreEqual("2020", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyM");
			Assert.AreEqual("20207", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMM");
			Assert.AreEqual("202007", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMM");
			Assert.AreEqual("2020Jul", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMM");
			Assert.AreEqual("2020July", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMd");
			Assert.AreEqual("2020July4", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMdd");
			Assert.AreEqual("2020July04", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMddd");
			Assert.AreEqual("2020JulySat", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMdddd");
			Assert.AreEqual("2020JulySaturday", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMdddd");
			Assert.AreEqual("2020JulySaturday", dtstr);
			str = "20200704200809";
			dtstr = Strings.FormatTime(str, "yyyyMMMMdddd hh");
			Assert.AreEqual("2020JulySaturday 08", dtstr);
			dtstr = Strings.FormatTime(str, "yyyyMMMMdddd HH");
			Assert.AreEqual("2020JulySaturday 20", dtstr);
			dtstr = Strings.FormatTime(str, "'Date:' yyyyMMMMdddd");
			Assert.AreEqual("Date: 2020JulySaturday", dtstr);
			dtstr = Strings.FormatTime(str, "'Date:' yyyyMMMMdddd ''''");
			Assert.AreEqual("Date: 2020JulySaturday '", dtstr);
			dtstr = Strings.FormatTime(str, "'Date:' yyyyMMMMdddd \"''\"");
			Assert.AreEqual("Date: 2020JulySaturday '", dtstr);
			Assert.IsTrue(TestScript("string-formattime", true));
		}

		[Test, Category("String")]
		public void InStr()
		{
			var x = "the string to searchz";
			var y = "the";
			var z = Strings.InStr(x, y);
			Assert.AreEqual(1, z);
			y = "z";
			z = Strings.InStr(x, y);
			Assert.AreEqual(x.Length, z);
			y = "Z";
			z = Strings.InStr(x, y, 1);
			Assert.AreEqual(0, z);
			z = Strings.InStr(x, y, "on");
			Assert.AreEqual(0, z);
			y = "g";
			z = Strings.InStr(x, y, 0, 12);
			Assert.AreEqual(0, z);
			y = "s";
			z = Strings.InStr(x, y, 0, 1, 2);
			Assert.AreEqual(15, z);
			y = "z";
			z = Strings.InStr(x, y, 0, -1);
			Assert.AreEqual(x.Length, z);
			y = "h";
			z = Strings.InStr(x, y, 0, -1, 2);
			Assert.AreEqual(2, z);
			y = "t";
			z = Strings.InStr(x, y, 0, -1, 3);
			Assert.AreEqual(1, z);
			Assert.IsTrue(TestScript("string-instr", true));
		}

		[Test, Category("String")]
		public void LTrim()
		{
			var x = " test\t";
			var y = Strings.LTrim(x);
			Assert.AreEqual("test\t", y);
			x = "test";
			y = Strings.LTrim(x);
			Assert.AreEqual("test", y);
			x = "\ttest ";
			y = Strings.LTrim(x);
			Assert.AreEqual("test ", y);
			x = " \ttest\t ";
			y = Strings.LTrim(x);
			Assert.AreEqual("test\t ", y);
			Assert.IsTrue(TestScript("string-ltrim", true));
		}

		[Test, Category("String")]
		public void Ord()
		{
			var chr = Strings.Ord("t");
			Assert.AreEqual(116, chr);
			chr = Strings.Ord("te", 2);
			Assert.AreEqual(101, chr);
			Assert.IsTrue(TestScript("string-ord", true));
		}

		[Test, Category("String")]
		public void RegExMatch()
		{
			var match = Strings.RegExMatch("abc123abc456", "abc\\d+", 1);
			Assert.AreEqual(match[0], "abc123");
			Assert.AreEqual(match.Pos(), 1);
			match = Strings.RegExMatch("abc123abc456", "456", -1);
			Assert.AreEqual(match[0], "456");
			Assert.AreEqual(match.Pos(), 10);
			match = Strings.RegExMatch("abc123abc456", "abc", -1);
			Assert.AreEqual(match[0], "abc");
			Assert.AreEqual(match.Pos(), 7);
			match = Strings.RegExMatch("abc123abc456", "abc", -15);
			Assert.AreEqual(match[0], "abc");
			Assert.AreEqual(match.Pos(), 7);
			match = Strings.RegExMatch("abc123abc456", "abc", -5);
			Assert.AreEqual(match[0], "abc");
			Assert.AreEqual(match.Pos(), 1);
			match = Strings.RegExMatch("abc123abc456", "abc\\d+", 2);
			Assert.AreEqual(match[0], "abc456");
			Assert.AreEqual(match.Pos(), 7);
			match = Strings.RegExMatch("abc123123", "123$", 1);
			Assert.AreEqual(match.Pos(), 7);
			match = Strings.RegExMatch("xxxabc123xyz", "abc.*xyz", 1);
			Assert.AreEqual(match.Pos(), 4);
			match = Strings.RegExMatch("abc123123", "123$");
			Assert.AreEqual(match.Pos(), 7);
			match = Strings.RegExMatch("abc123", "i)^ABC");
			Assert.AreEqual(match.Pos(), 1);
			match = Strings.RegExMatch("abcXYZ123", "abc(.*)123");
			Assert.AreEqual(match[1], "XYZ");
			Assert.AreEqual(match.Pos(1), 4);
			match = Strings.RegExMatch("abcXYZ123", "abc(?<testname>.*)123");
			Assert.AreEqual(match["testname"], "XYZ");
			Assert.AreEqual(match.Pos("testname"), 4);
			Assert.AreEqual(match.Name("testname"), "testname");
			match = Strings.RegExMatch(@"C:\Foo\Bar\Baz.txt", @"\w+$");
			Assert.AreEqual(match[0], "txt");
			Assert.AreEqual(match.Pos(), 16);
			match = Strings.RegExMatch("Michiganroad 72", @"(.*) (?<nr>\d+)");
			Assert.AreEqual(match.Count, 2);
			Assert.AreEqual(match[1], "Michiganroad");
			Assert.AreEqual(match.Name(2), "nr");
			Assert.AreEqual(match[2], "72");
			Assert.IsTrue(TestScript("string-regex", true));
		}

		[Test, Category("String")]
		public void RTrim()
		{
			var x = " test\t";
			var y = Strings.RTrim(x);
			Assert.AreEqual(" test", y);
			x = "test";
			y = Strings.RTrim(x);
			Assert.AreEqual("test", y);
			x = "\ttest ";
			y = Strings.RTrim(x);
			Assert.AreEqual("\ttest", y);
			x = " \ttest\t ";
			y = Strings.RTrim(x);
			Assert.AreEqual(" \ttest", y);
			Assert.IsTrue(TestScript("string-rtrim", true));
		}

		[Test, Category("String")]
		public void Sort()
		{
			var x = "Z,X,Y,F,D,B,C,A,E";
			var y = Strings.Sort(x, "D,");
			Assert.AreEqual("A,B,C,D,E,F,X,Y,Z", y);
			y = Strings.Sort(x, "D, r");
			Assert.AreEqual("Z,Y,X,F,E,D,C,B,A", y);
			x = "Z,X,Y,F,D,B,C,A,E,a,b,c,d,e";
			y = Strings.Sort(x, "D,");
			Assert.AreEqual("A,a,B,b,C,c,D,d,E,e,F,X,Y,Z", y);
			y = Strings.Sort(x, "D, r");
			Assert.AreEqual("Z,Y,X,F,e,E,d,D,c,C,b,B,a,A", y);
			y = Strings.Sort(x, "D, c");
			Assert.AreEqual("A,B,C,D,E,F,X,Y,Z,a,b,c,d,e", y);
			y = Strings.Sort(x, "D, c r");
			Assert.AreEqual("e,d,c,b,a,Z,Y,X,F,E,D,C,B,A", y);
			x = "200,100,300,500,600,111,222,1010";
			y = Strings.Sort(x, "D, n");
			Assert.AreEqual("100,111,200,222,300,500,600,1010", y);

			for (var i = 0; i < 10; i++)
			{
				var z = Strings.Sort(x, "D, n random");
				Keysharp.Scripting.Script.OutputDebug(z);
				Assert.AreNotEqual(z, y);
				y = z;
			}

			y = Strings.Sort(x, "D, n r");
			Assert.AreEqual("1010,600,500,300,222,200,111,100", y);
			x = "RED\nGREEN\nBLUE\n";
			y = Strings.Sort(x);
			Assert.AreEqual("BLUE\nGREEN\nRED", y);
			y = Strings.Sort(x, "z");
			Assert.AreEqual("\nBLUE\nGREEN\nRED", y);
			x = "C:\\AAA\\BBB.txt,C:\\BBB\\AAA.txt";
			y = Strings.Sort(x, "D, \\");
			Assert.AreEqual("C:\\BBB\\AAA.txt,C:\\AAA\\BBB.txt", y);
			x = "/usr/bin/AAA/BBB.txt,/usr/bin/BBB/AAA.txt";
			y = Strings.Sort(x, "D, /");
			Assert.AreEqual("/usr/bin/BBB/AAA.txt,/usr/bin/AAA/BBB.txt", y);
			x = "co-op,comp,coop";
			y = Strings.Sort(x, "D, CL");
			Assert.AreEqual("comp,co-op,coop", y);
			x = "Ä,Ü,A,a,B,b,u,U";
			y = Strings.Sort(x, "D, CL");
			Assert.AreEqual("A,a,Ä,B,b,u,U,Ü", y);
			x = "AZB,BYX,CWM,LMN";
			y = Strings.Sort(x, "D, P2");
			Assert.AreEqual("LMN,CWM,BYX,AZB", y);
			Assert.IsTrue(TestScript("string-sort", true));
		}

		[Test, Category("String")]
		public void StrCompare()
		{
			var x = "a";
			var y = "b";
			var z = Strings.StrCompare(x, y);
			Assert.AreEqual(-1, z);
			x = "a";
			y = "a";
			z = Strings.StrCompare(x, y);
			Assert.AreEqual(0, z);
			x = "b";
			y = "a";
			z = Strings.StrCompare(x, y);
			Assert.AreEqual(1, z);
			x = "a";
			y = "B";
			z = Strings.StrCompare(x, y);
			Assert.AreEqual(-1, z);
			x = "A";
			y = "a";
			z = Strings.StrCompare(x, y);
			Assert.AreEqual(0, z);
			x = "b";
			y = "A";
			z = Strings.StrCompare(x, y);
			Assert.AreEqual(1, z);
			x = "A";
			y = "a";
			z = Strings.StrCompare(x, y, "On");
			Assert.IsTrue(z < 0);
			x = "A11";
			y = "A100";
			z = Strings.StrCompare(x, y, "On");
			Assert.IsTrue(z > 0);
			z = Strings.StrCompare(x, y, "logical");
			Assert.IsTrue(z < 0);
			Assert.IsTrue(TestScript("string-strcompare", true));
		}

		[Test, Category("String")]
		public void String()
		{
			object x = 123;
			var y = Strings.String(x);
			Assert.AreEqual(y, "123");
			x = "123";
			y = Strings.String(x);
			Assert.AreEqual(y, "123");
			x = 1.234;
			y = Strings.String(x);
			Assert.AreEqual(y, "1.234");
			Assert.IsTrue(TestScript("string-string", true));
		}

		[Test, Category("String")]
		public void StrLen()
		{
			var x = "test";
			var y = Strings.StrLen(x);
			Assert.AreEqual(y, 4);
			x = "";
			y = Strings.StrLen(x);
			Assert.AreEqual(y, 0);
			Assert.IsTrue(TestScript("string-strlen", true));
		}

		[Test, Category("String")]
		public void StrLower()
		{
			var x = "ALL CAPS";
			var y = Strings.StrLower(x);
			Assert.AreEqual(y, "all caps");
			x = "AlL CaPs";
			y = Strings.StrLower(x);
			Assert.AreEqual(y, "all caps");
			x = "all caps";
			y = Strings.StrLower(x);
			Assert.AreEqual(y, "all caps");
			x = "";
			y = Strings.StrLower(x);
			Assert.AreEqual(y, "");
			x = "ALL CAPS";
			y = Strings.StrLower(x, "T");
			Assert.AreEqual(y, "ALL CAPS");
			x = "all caps";
			y = Strings.StrLower(x, "T");
			Assert.AreEqual(y, "All Caps");
			x = "All Caps";
			y = Strings.StrLower(x, "T");
			Assert.AreEqual(y, "All Caps");
			Assert.IsTrue(TestScript("string-strlower", true));
		}

		[Test, Category("String")]
		public void StrPutStrGet()
		{
			var buf1 = Script.Buffer(32);
			var s = "tester";
			//Unicode test.
			var testlen = StrPut(s);
			var lenwritten = StrPut(s, buf1);
			Assert.AreEqual(testlen, lenwritten);
			var gotten = StrGet(buf1, -s.Length);
			Assert.AreEqual(s, gotten);
			//ASCII test.
			testlen = StrPut(s, null, null, "ASCII");
			lenwritten = StrPut(s, buf1, null, "ASCII");
			Assert.AreEqual(testlen, lenwritten);
			gotten = StrGet(buf1, s.Length, "ASCII");
			Assert.AreEqual(s, gotten);
			//Substring test.
			gotten = StrGet(buf1, s.Length - 2, "ASCII");
			Assert.AreEqual(s.Substring(0, s.Length - 2), gotten);
			Assert.IsTrue(TestScript("string-strputstrget", true));
		}

		[Test, Category("String")]
		public void StrReplace()
		{
			var x = "a,b,c,d,e,f";//Can't test the ref var which holds the count here because those must be global. However it is tested in the script.
			var y = Strings.StrReplace(x, ",");
			Assert.AreEqual("abcdef", y);
			y = Strings.StrReplace(x, ",", "");
			Assert.AreEqual("abcdef", y);
			y = Strings.StrReplace(x, ",", ".");
			Assert.AreEqual("a.b.c.d.e.f", y);
			y = Strings.StrReplace(x, ",", ".", "On");
			Assert.AreEqual("a.b.c.d.e.f", y);
			y = Strings.StrReplace(x, ",", ".");
			Assert.AreEqual("a.b.c.d.e.f", y);
			y = Strings.StrReplace(x, ",", ".", null, null, 3);
			Assert.AreEqual("a.b.c.d,e,f", y);
			y = Strings.StrReplace(x, "");
			Assert.AreEqual("", y);
			y = Strings.StrReplace(x, "a", "A", "On");
			Assert.AreEqual("A,b,c,d,e,f", y);
			y = Strings.StrReplace(x, "a", "A", "On", null, 9);
			Assert.AreEqual("A,b,c,d,e,f", y);
			Assert.IsTrue(TestScript("string-strreplace", false));//Don't test func version because the ref var must be global.
		}

		[Test, Category("String")]
		public void StrSplit()
		{
			var x = "a,b,c,d";
			var y = Strings.StrSplit(x, ",");
			var exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "c", "d" });
			Assert.AreEqual(exp, y);
			x = "abcd";
			y = Strings.StrSplit(x);
			Assert.AreEqual(exp, y);
			x = "	a, b,c ,d	";
			y = Strings.StrSplit(x, ",", "\t ");
			Assert.AreEqual(exp, y);
			x = "	a, b-c _d	";
			y = Strings.StrSplit(x, new string[] { ",", "-", "_" }, "\t ");
			Assert.AreEqual(exp, y);
			y = Strings.StrSplit(x, new Core.Array(new string[] { ",", "-", "_" }), "\t ");
			Assert.AreEqual(exp, y);
			x = "abcd";
			y = Strings.StrSplit(x, null, null, 1);
			exp = Keysharp.Scripting.Script.Array(new string[] { "abcd" });
			Assert.AreEqual(exp, y);
			y = Strings.StrSplit(x, null, null, 2);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "bcd" });
			Assert.AreEqual(exp, y);
			y = Strings.StrSplit(x, null, null, 3);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "cd" });
			Assert.AreEqual(exp, y);
			y = Strings.StrSplit(x, null, null, 4);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "c", "d" });
			Assert.AreEqual(exp, y);
			y = Strings.StrSplit(x, null, null, 5);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "c", "d" });
			Assert.AreEqual(exp, y);
			x = "a,b,c,d";
			y = Strings.StrSplit(x, ",", null, 3);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "c,d" });
			Assert.AreEqual(exp, y);
			x = "	a, b-c _d	";
			y = Strings.StrSplit(x, new string[] { ",", "-", "_" }, "\t ", 3);
			exp = Keysharp.Scripting.Script.Array(new string[] { "a", "b", "c _d" });
			Assert.AreEqual(exp, y);
			Assert.IsTrue(TestScript("string-strsplit", true));
		}

		[Test, Category("String")]
		public void StrUpper()
		{
			var x = "ALL CAPS";
			var y = Strings.StrUpper(x);
			Assert.AreEqual(y, "ALL CAPS");
			x = "AlL CaPs";
			y = Strings.StrUpper(x);
			Assert.AreEqual(y, "ALL CAPS");
			x = "all caps";
			y = Strings.StrUpper(x);
			Assert.AreEqual(y, "ALL CAPS");
			x = "";
			y = Strings.StrUpper(x);
			Assert.AreEqual(y, "");
			x = "ALL CAPS";
			y = Strings.StrUpper(x, "T");
			Assert.AreEqual(y, "ALL CAPS");
			x = "all caps";
			y = Strings.StrUpper(x, "T");
			Assert.AreEqual(y, "All Caps");
			x = "All Caps";
			y = Strings.StrUpper(x, "T");
			Assert.AreEqual(y, "All Caps");
			Assert.IsTrue(TestScript("string-strupper", true));
		}

		[Test, Category("String")]
		public void SubStr()
		{
			var x = "abcdefghijkl";
			var y = Strings.SubStr(x);
			Assert.AreEqual(x, y);
			y = Strings.SubStr(x, 1, 1);
			Assert.AreEqual("a", y);
			y = Strings.SubStr(x, 1, 5);
			Assert.AreEqual("abcde", y);
			y = Strings.SubStr(x, 1, 11);
			Assert.AreEqual("abcdefghijk", y);
			y = Strings.SubStr(x, 1, -1);
			Assert.AreEqual("abcdefghijk", y);
			y = Strings.SubStr(x, 1, -11);
			Assert.AreEqual("a", y);
			y = Strings.SubStr(x, 1, -12);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, 1, -13);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, 4, -3);
			Assert.AreEqual("defghi", y);
			y = Strings.SubStr(x, 6, -6);
			Assert.AreEqual("f", y);
			y = Strings.SubStr(x, 7, -6);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, 7, -7);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, 0);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, x.Length + 1);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, 2, 1);
			Assert.AreEqual("b", y);
			y = Strings.SubStr(x, 2, 1);
			Assert.AreEqual("b", y);
			y = Strings.SubStr(x, 4, 3);
			Assert.AreEqual("def", y);
			y = Strings.SubStr(x, 10, 3);
			Assert.AreEqual("jkl", y);
			y = Strings.SubStr(x, 12, 1);
			Assert.AreEqual("l", y);
			y = Strings.SubStr(x, -1);
			Assert.AreEqual("l", y);
			y = Strings.SubStr(x, -5);
			Assert.AreEqual("hijkl", y);
			y = Strings.SubStr(x, -12);
			Assert.AreEqual(x, y);
			y = Strings.SubStr(x, -13);
			Assert.AreEqual(x, y);
			y = Strings.SubStr(x, -5, 5);
			Assert.AreEqual("hijkl", y);
			y = Strings.SubStr(x, -5, 3);
			Assert.AreEqual("hij", y);
			y = Strings.SubStr(x, -5, -3);
			Assert.AreEqual("hi", y);
			y = Strings.SubStr(x, -5, -5);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, -5, -6);
			Assert.AreEqual("", y);
			y = Strings.SubStr(x, -5, -13);
			Assert.AreEqual("", y);
			Assert.IsTrue(TestScript("string-substr", true));
		}

		[Test, Category("String")]
		public void Trim()
		{
			var x = " test\t";
			var y = Strings.Trim(x);
			Assert.AreEqual("test", y);
			x = "test";
			y = Strings.Trim(x);
			Assert.AreEqual("test", y);
			x = "\ttest ";
			y = Strings.Trim(x);
			Assert.AreEqual("test", y);
			x = " \ttest\t ";
			y = Strings.Trim(x);
			Assert.AreEqual("test", y);
			Assert.IsTrue(TestScript("string-trim", true));
		}
	}
}