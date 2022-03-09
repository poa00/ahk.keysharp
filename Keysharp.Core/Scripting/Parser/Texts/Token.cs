using System;
using System.IO;
using System.Linq;
using static Keysharp.Core.Core;

namespace Keysharp.Scripting
{
	public partial class Parser
	{
		private Token GetToken(string code)
		{
			code = code.TrimStart(Spaces);

			if (code.Length == 0)
				return Token.Unknown;

			if (IsFlowOperator(code))
				return Token.Flow;
			else if (IsLabel(code))
				return Token.Label;
			else if (IsHotkeyLabel(code) || IsHotstringLabel(code))
				return Token.Hotkey;
			else if (IsAssignment(code))
				return Token.Assign;
			else if (IsDirective(code))
				return Token.Directive;
			else return IsCommand(code) ? Token.Command : Token.Expression;
		}

		private bool IsAssignment(string code, bool checkexprassign = false)
		{
			var i = 0;

			while (i < code.Length && (IsIdentifier(code[i]) || code[i] == Resolve)) i++;

			if (i == 0 || i == code.Length)
				return false;

			while (IsSpace(code[i])) i++;//This should really use Array.IndexOf(Spaces, code);//MATT

			if (i < code.Length && code[i] == Equal)
				return true;

			//The statement above only checks for simple =, not :=. For some reason, checking for := completely throws off other parts of the parsing logic.
			//My hunch is that := is intended to be treated as an expression, not an assignment.
			//So it's better to leave this out for now, and figure out what would be ideal in the future if needed.
			//if (checkexprassign)
			//  if (i < code.Length - 1 && (code[i] == AssignPre && code[i + 1] == Equal))
			//      return true;
			return false;
		}

		private bool IsCommand(string code)
		{
			var i = 0;

			while (i < code.Length && IsIdentifier(code[i])) i++;

			if (i == 0)
				return false;
			else if (i == code.Length)
				return true;
			else if (code[i] == Multicast)
				return true;
			else if (IsSpace(code[i]))
			{
				i++;

				while (i < code.Length && IsSpace(code[i])) i++;

				if (i < code.Length && code[i] == Equal)
					return false;
				else if (IsCommentAt(code, i))
					return true;

				if (IsIdentifier(code[i]))
					return !IsKeyword(code[i]);

				int y = i + 1, z = i + 2;

				if (y < code.Length)
				{
					if (code[y] == Equal)
						return false;
					else if (z + 1 < code.Length && code[i] == code[y] && code[y] == code[z] && code[z + 1] == Equal)
						return false;
					else if (z < code.Length && code[i] == code[y] && code[z] == Equal)
						return false;
					else if (LaxExpressions)
					{
						if (IsOperator(code.Substring(i, 1)) && code.Contains(" ? "))
							return false;
					}
				}

				var pre = code.Substring(0, i).TrimEnd(Spaces);
				return !IsPrimitiveObject(pre);
			}
			else
				return false;
		}

		private bool IsDirective(string code) => code.Length > 2 && code[0] == Directive;

		private bool IsFlowOperator(string code)
		{
			const int offset = 4;
			var delimiters = new char[Spaces.Length + offset];
			delimiters[0] = Multicast;
			delimiters[1] = BlockOpen;
			delimiters[2] = ParenOpen;
			delimiters[3] = HotkeyBound;//Need ':' colon for default: statements. Unsure if this breaks anything else.//MATT
			Spaces.CopyTo(delimiters, offset);
			var word = code.Split(delimiters, 2)[0].ToLowerInvariant();

			switch (word)
			{
				case FlowBreak:
				case FlowContinue:
				case FlowCase:
				case FlowDefault:
				case FlowFor:
				case FlowElse:
				case FlowGosub:
				case FlowGoto:
				case FlowIf:
				case FlowLoop:
				case FlowReturn:
				case FlowWhile:
				case FunctionLocal:
				case FunctionGlobal:
				case FunctionStatic:
				case FlowTry:
				case FlowCatch:
				case FlowFinally:
				case FlowUntil:
				case FlowSwitch:
				case Throw:
					return true;
			}

			return false;
		}

		private bool IsFunction(string code, string next)
		{
			if (code.Length == 0 || code[0] == ParenOpen)
				return false;

			var stage = 0;
			var str = false;

			for (var i = 0; i < code.Length; i++)
			{
				var sym = code[i];

				switch (stage)
				{
					case 0:
						if (sym == ParenOpen)
							stage++;
						else if (!IsIdentifier(sym))
							return false;

						break;

					case 1:
						if (sym == StringBound)
							str = !str;
						else if (!str && sym == ParenClose)
							stage++;

						break;

					case 2:
						if (sym == BlockOpen)
							return true;
						else if (IsCommentAt(code, i))
							goto donext;
						else if (!IsSpace(sym))
							return false;

						break;
				}
			}

			donext:

			if (next.Length == 0)
				return false;

			var reader = new StringReader(next);

			while (reader.Peek() != -1)
			{
				var sym = (char)reader.Read();

				if (sym == BlockOpen)
					return true;
				else if (!IsSpace(sym))
					return false;
			}

			return false;
		}

		private bool IsHotkeyLabel(string code)
		{
			var z = code.IndexOf(HotkeySignal);

			if (z == -1)
				return false;

			var p = false;

			for (var i = 0; i < z; i++)
			{
				var sym = code[i];

				switch (sym)
				{
					case '#':
					case '!':
					case '^':
					case '+':
					case '<':
					case '>':
					case '*':
					case '~':
					case '$':
						break;

					case '&':
						p = false;
						break;

					default:
						if (!IsSpace(sym) && !char.IsLetterOrDigit(sym))
						{
							if (p)
								return false;
							else
								p = true;
						}

						break;
				}
			}

			return true;
		}

		private bool IsHotstringLabel(string code) => code.Length > 0 && code[0] == HotkeyBound&& code.Contains(HotkeySignal)&& code.Count(ch => ch == HotkeyBound) >= 4;

		private bool IsLabel(string code)
		{
			for (var i = 0; i < code.Length; i++)
			{
				var sym = code[i];

				if (IsIdentifier(sym))
					continue;

				switch (sym)
				{
					case HotkeyBound:
						if (i == 0)
							return false;
						else if (i == code.Length - 1)
							return true;
						else
						{
							var sub = StripCommentSingle(code.Substring(i));
							return sub.Length == 0 || IsSpace(sub);
						}

					case ParenOpen:
					case ParenClose:
						break;

					default:
						return false;
				}
			}

			return false;
		}

		private bool IsSpace(char sym) => Array.IndexOf(Spaces, sym) != -1;

		private bool IsSpace(string code)
		{
			foreach (var sym in code)
				if (!IsSpace(sym))
					return false;

			return true;
		}

		private enum Token
		{ Unknown, Assign, Command, Label, Hotkey, Flow, Throw, Expression, Directive }
	}
}