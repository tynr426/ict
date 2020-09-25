

namespace ECF.Json
{
	/// <summary>
	///  ParserToken.cs
	///    Internal representation of the tokens used by the lexer and the parser.
	/// 
	///  The authors disclaim copyright to this source code. For more details, see
	///  the COPYING file included with this distribution.
	/// </summary>
	internal enum ParserToken
    {
		// Lexer tokens (see section A.1.1. of the manual)
		/// <summary>
		/// The none
		/// </summary>
		None = System.Char.MaxValue + 1,
		/// <summary>
		/// The number
		/// </summary>
		Number,
		/// <summary>
		/// The true
		/// </summary>
		True,
		/// <summary>
		/// The false
		/// </summary>
		False,
		/// <summary>
		/// The null
		/// </summary>
		Null,
		/// <summary>
		/// The character seq
		/// </summary>
		CharSeq,
		// Single char
		/// <summary>
		/// The character
		/// </summary>
		Char,

		// Parser Rules (see section A.2.1 of the manual)
		/// <summary>
		/// The text
		/// </summary>
		Text,
		/// <summary>
		/// The object
		/// </summary>
		Object,
		/// <summary>
		/// The object prime
		/// </summary>
		ObjectPrime,
		/// <summary>
		/// The pair
		/// </summary>
		Pair,
		/// <summary>
		/// The pair rest
		/// </summary>
		PairRest,
		/// <summary>
		/// The array
		/// </summary>
		Array,
		/// <summary>
		/// The array prime
		/// </summary>
		ArrayPrime,
		/// <summary>
		/// The value
		/// </summary>
		Value,
		/// <summary>
		/// The value rest
		/// </summary>
		ValueRest,
		/// <summary>
		/// The string
		/// </summary>
		String,

		// End of input
		/// <summary>
		/// The end
		/// </summary>
		End,

		// The empty rule
		/// <summary>
		/// The epsilon
		/// </summary>
		Epsilon
	}
}
