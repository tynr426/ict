using System;


namespace ECF.Json
{
	/// <summary>
	/// FullName: <see cref="ECF.Json.JsonException"/>
	/// Summary : Json exception
	/// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	public class JsonException : ECFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		public JsonException() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="token">The token.</param>
		internal JsonException(ParserToken token) :
			base(String.Format(
					"Invalid token '{0}' in input string", token))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="inner_exception">The inner_exception.</param>
		internal JsonException(ParserToken token,
								Exception inner_exception) :
			base(String.Format(
					"Invalid token '{0}' in input string", token),
				inner_exception)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="c">The c.</param>
		internal JsonException(int c) :
			base(String.Format(
					"Invalid character '{0}' in input string", (char)c))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="c">The c.</param>
		/// <param name="inner_exception">The inner_exception.</param>
		internal JsonException(int c, Exception inner_exception) :
			base(String.Format(
					"Invalid character '{0}' in input string", (char)c),
				inner_exception)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public JsonException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner_exception">The inner_exception.</param>
		public JsonException(string message, Exception inner_exception) :
			base(message, inner_exception)
		{
		}
	}
}
