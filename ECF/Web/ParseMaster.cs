using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ECF.Web
{
	/// <summary>
	/// 解析大师
	/// </summary>
	public class ParseMaster
    {
		/// <summary>
		/// 匹配组计算器
		/// </summary>
		/// <param name="match">匹配.</param>
		/// <param name="offset">位置.</param>
		/// <returns></returns>
		public delegate string MatchGroupEvaluator(Match match, int offset);
		/// <summary>
		/// 
		/// </summary>
		private class Pattern
        {
			/// <summary>
			/// The expression
			/// </summary>
			public string expression;
			/// <summary>
			/// The replacement
			/// </summary>
			public object replacement;
			/// <summary>
			/// The length
			/// </summary>
			public int length;
			/// <summary>
			/// Returns a <see cref="System.String" /> that represents this instance.
			/// </summary>
			/// <returns>
			/// A <see cref="System.String" /> that represents this instance.
			/// </returns>
			public override string ToString()
            {
                return "(" + this.expression + ")";
            }
        }
		/// <summary>
		/// The groups
		/// </summary>
		private Regex GROUPS = new Regex("\\(");
		/// <summary>
		/// The su b_ replace
		/// </summary>
		private Regex SUB_REPLACE = new Regex("\\$");
		/// <summary>
		/// The indexed
		/// </summary>
		private Regex INDEXED = new Regex("^\\$\\d+$");
		/// <summary>
		/// The escape
		/// </summary>
		private Regex ESCAPE = new Regex("\\\\.");
		/// <summary>
		/// The quote
		/// </summary>
		private Regex QUOTE = new Regex("'");
		/// <summary>
		/// The deleted
		/// </summary>
		private Regex DELETED = new Regex("\\x01[^\\x01]*\\x01");
		/// <summary>
		/// The ignore case
		/// </summary>
		private bool ignoreCase = false;
		/// <summary>
		/// The escape character
		/// </summary>
		private char escapeChar = '\0';
		/// <summary>
		/// The patterns
		/// </summary>
		private ArrayList patterns = new ArrayList();
		/// <summary>
		/// The escaped
		/// </summary>
		private StringCollection escaped = new StringCollection();
		/// <summary>
		/// The unescape index
		/// </summary>
		private int unescapeIndex = 0;
		/// <summary>
		/// Gets or sets a value indicating whether [ignore case].
		/// </summary>
		/// <value>
		///   <c>true</c> if [ignore case]; otherwise, <c>false</c>.
		/// </value>
		public bool IgnoreCase
        {
            get
            {
                return this.ignoreCase;
            }
            set
            {
                this.ignoreCase = value;
            }
        }
		/// <summary>
		/// Gets or sets the escape character.
		/// </summary>
		/// <value>
		/// The escape character.
		/// </value>
		public char EscapeChar
        {
            get
            {
                return this.escapeChar;
            }
            set
            {
                this.escapeChar = value;
            }
        }
		/// <summary>
		/// Deletes the specified match.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="offset">The offset.</param>
		/// <returns></returns>
		private string DELETE(Match match, int offset)
        {
            return "\u0001" + match.Groups[offset].Value + "\u0001";
        }
		/// <summary>
		/// Adds the specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public void Add(string expression)
        {
            this.Add(expression, string.Empty);
        }
		/// <summary>
		/// Adds the specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="replacement">The replacement.</param>
		public void Add(string expression, string replacement)
        {
            if (replacement == string.Empty)
            {
                this.add(expression, new ParseMaster.MatchGroupEvaluator(this.DELETE));
            }
            this.add(expression, replacement);
        }
		/// <summary>
		/// Adds the specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="replacement">The replacement.</param>
		public void Add(string expression, ParseMaster.MatchGroupEvaluator replacement)
        {
            this.add(expression, replacement);
        }
		/// <summary>
		/// Executes the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public string Exec(string input)
        {
            return this.DELETED.Replace(this.unescape(this.getPatterns().Replace(this.escape(input), new MatchEvaluator(this.replacement))), string.Empty);
        }
		/// <summary>
		/// Adds the specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="replacement">The replacement.</param>
		private void add(string expression, object replacement)
        {
            ParseMaster.Pattern pattern = new ParseMaster.Pattern();
            pattern.expression = expression;
            pattern.replacement = replacement;
            pattern.length = this.GROUPS.Matches(this.internalEscape(expression)).Count + 1;
            if (replacement is string && this.SUB_REPLACE.IsMatch((string)replacement))
            {
                string text = (string)replacement;
                if (this.INDEXED.IsMatch(text))
                {
                    pattern.replacement = int.Parse(text.Substring(1)) - 1;
                }
            }
            this.patterns.Add(pattern);
        }
		/// <summary>
		/// Gets the patterns.
		/// </summary>
		/// <returns></returns>
		private Regex getPatterns()
        {
            StringBuilder stringBuilder = new StringBuilder(string.Empty);
            IEnumerator enumerator = this.patterns.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    stringBuilder.Append(((ParseMaster.Pattern)current).ToString() + "|");
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase);
        }
		/// <summary>
		/// Replacements the specified match.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		private string replacement(Match match)
        {
            int num = 1;
            int num2 = 0;
            ParseMaster.Pattern pattern;
            while ((pattern = (ParseMaster.Pattern)this.patterns[(num2++)]) != null)
            {
                if (match.Groups[num].Value != string.Empty)
                {
                    object replacement = pattern.replacement;
                    if (replacement is ParseMaster.MatchGroupEvaluator)
                    {
                        return ((ParseMaster.MatchGroupEvaluator)replacement)(match, num);
                    }
                    if (replacement is int)
                    {
                        return match.Groups[(int)replacement + num].Value;
                    }
                    return this.replacementString(match, num, (string)replacement, pattern.length);
                }
                else
                {
                    num += pattern.length;
                }
            }
            return match.Value;
        }
		/// <summary>
		/// Replacements the string.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="replacement">The replacement.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		private string replacementString(Match match, int offset, string replacement, int length)
        {
            while (length > 0)
            {
                replacement = replacement.Replace("$" + length--, match.Groups[offset + length].Value);
            }
            return replacement;
        }
		/// <summary>
		/// Escapes the specified string.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns></returns>
		private string escape(string str)
        {
            if (this.escapeChar == '\0')
            {
                return str;
            }
            Regex regex = new Regex("\\" + this.escapeChar + "(.)");
            return regex.Replace(str, new MatchEvaluator(this.escapeMatch));
        }
		/// <summary>
		/// Escapes the match.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		private string escapeMatch(Match match)
        {
            this.escaped.Add(match.Groups[1].Value);
            return "" + this.escapeChar;
        }
		/// <summary>
		/// Unescapes the specified string.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns></returns>
		private string unescape(string str)
        {
            if (this.escapeChar == '\0')
            {
                return str;
            }
            Regex regex = new Regex("\\" + this.escapeChar);
            return regex.Replace(str, new MatchEvaluator(this.unescapeMatch));
        }
		/// <summary>
		/// Unescapes the match.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		private string unescapeMatch(Match match)
        {
            return "\\" + this.escaped[this.unescapeIndex++];
        }
		/// <summary>
		/// Internals the escape.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns></returns>
		private string internalEscape(string str)
        {
            return this.ESCAPE.Replace(str, "");
        }
    }
}
