using System;
using System.CodeDom;
using System.Globalization;
using static Keysharp.Core.Core;

namespace Keysharp.Scripting
{
	public partial class Parser
	{
		internal static bool IsIdentifier(char symbol) => char.IsLetterOrDigit(symbol) || VarExt.IndexOf(symbol) != -1;

		private bool IsDynamicReference(string code)
		{
			var d = false;

			for (var i = 0; i < code.Length; i++)
			{
				var sym = code[i];

				if (sym == Resolve)
				{
					if (d)
						if (code[i - 1] == Resolve)
							return false;

					d = !d;
				}
				else if (!IsIdentifier(sym))
					return false;
			}

			return code.Length != 0;
		}

		private bool IsExpressionIf(string code)
		{
			code = code.TrimStart(Spaces);
			var i = 0;

			if (code.Length == 0)
				return true;

			if (code[0] == ParenOpen)
				return true;

			while (i < code.Length && IsIdentifier(code[i])) i++;

			if (i == 0 || IsKeyword(code.Substring(0, i)))
				return true;

			while (i < code.Length && IsSpace(code[i])) i++;

			if (i == 0 || i == code.Length)
				return false;

			switch (code[i])
			{
				case Equal:
				case Not:
				case Greater:
				case Less:
					return false;
			}

			return true;
		}

		private bool IsExpressionParameter(string code)
		{
			code = code.TrimStart(Spaces);
			var z = code.IndexOf(Resolve);
			return z == 0 && (code.Length == 1 || IsSpace(code[1]));
		}

		private bool IsIdentifier(string token) => IsIdentifier(token, false);

		private bool IsIdentifier(string token, bool dynamic)
		{
			if (string.IsNullOrEmpty(token))
				return false;

			if (token[0] == TernaryA && (token.Length == 1 || token.Length == 2 && token[1] == TernaryA))
				return false;

			foreach (var sym in token)
			{
				if (!IsIdentifier(sym))
				{
					if (dynamic && sym == Resolve)
						continue;

					return false;
				}
			}

			if (double.TryParse(token, out var _))//Need to ensure it's not a number, because identifiers can't be numbers.
				return false;

			if (token.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase) &&
					int.TryParse(token.AsSpan(2), NumberStyles.HexNumber, culture, out var _))
				return false;

			return true;
		}

		private bool IsKeyword(string code)
		{
			switch (code.ToLowerInvariant())
			{
				case AndTxt:
				case OrTxt:
				case NotTxt:
				case TrueTxt:
				case FalseTxt:
				case NullTxt:
				case IsTxt:
				case FlowBreak:
				case FlowContinue:
				case FlowCase:
				case FlowClass:
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

				default:
					return false;
			}
		}

		private bool IsKeyword(char symbol)
		{
			switch (symbol)
			{
				case TernaryA:
					return true;

				default:
					return false;
			}
		}

		private bool IsLegacyIf(string code)
		{
			var part = code.TrimStart(Spaces).Split(Spaces, 3);

			if (part.Length < 2 || !IsIdentifier(part[0]))
				return false;

			switch (part[1].ToLowerInvariant())
			{
				case NotTxt:
				case BetweenTxt:
				case InTxt:
				case ContainsTxt:
				case IsTxt:
					return true;
			}

			return false;
		}

		private bool IsPrimitiveObject(string code, out object result)
		{
			result = null;

			if (string.IsNullOrEmpty(code))
				return true;

			switch (code.ToLowerInvariant())
			{
				case TrueTxt:
					//result = 1L;
					result = true;//Althought the AHK documentation says true/false are really just 1/0 under the hood, that causes problems here.
					return true;//Particularly with the Force[type]() functions used in Operate(). Having them be a bool type makes it much easier to determine the caller's intent when comparing values.

				case FalseTxt:
					//result = 0L;
					result = false;
					return true;

				case NullTxt:
					result = null;
					return true;
			}

			//Mono incorrectly determines "." as a numeric value.
			if (code.Length == 1 && code[0] == Concatenate)
				return false;

			var codeTrim = code.Trim(Spaces);
			var longresult = codeTrim.ParseLong(false, false);//Also supports hex, but do not consider raw hex, because then a variable name like a would be returned as 10.

			if (longresult.HasValue)
			{
				result = longresult.Value;
				goto exp;
			}

			if (double.TryParse(codeTrim, NumberStyles.Any, culture, out var d))//This will make any number be a double internally. Not sure if this is what AHK does.
			{
				result = d;
				goto exp;
			}

			result = null;
			return false;
			exp:
			return true;
		}

		private CodeExpression PrimitiveToExpression(string code)
		{
			if (IsPrimitiveObject(code, out var result))
			{
				if (result != null)
				{
					if (result is long ll)
						return new CodeSnippetExpression($"{ll}L");
					else
						return new CodePrimitiveExpression(result);
				}
				else
					return new CodePrimitiveExpression(result);
			}

			return null;
		}

		private bool IsPrimitiveObject(string code) => IsPrimitiveObject(code, out var result);

		private bool IsRemap(string code)//This is totally wrong and has nothing to do with how remaps are done in v2.//TODO
		{
			code = code.Trim(Spaces);

			if (code.Length == 0)
				return false;

			if (IsSpace(code[0]))
				return false;

			for (var i = 1; i < code.Length; i++)
			{
				if (IsCommentAt(code, i))
					return true;
				else if (!IsSpace(code[i]))
					return false;
			}

			return true;
		}

		private bool IsVariable(string code) => IsIdentifier(code, true)&& !IsKeyword(code);
	}
}