
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace ECF.Json
{
	/// <summary>
	/// 
	/// </summary>
	internal enum Condition
    {
		/// <summary>
		/// The in array
		/// </summary>
		InArray,
		/// <summary>
		/// The in object
		/// </summary>
		InObject,
		/// <summary>
		/// The not a property
		/// </summary>
		NotAProperty,
		/// <summary>
		/// The property
		/// </summary>
		Property,
		/// <summary>
		/// The value
		/// </summary>
		Value
	}

	/// <summary>
	/// FullName: <see cref="ECF.Json.WriterContext"/>
	/// Summary : Writer context
	/// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	internal class WriterContext
    {
		/// <summary>
		/// The count
		/// </summary>
		public int  Count;
		/// <summary>
		/// The in array
		/// </summary>
		public bool InArray;
		/// <summary>
		/// The in object
		/// </summary>
		public bool InObject;
		/// <summary>
		/// The expecting value
		/// </summary>
		public bool ExpectingValue;
		/// <summary>
		/// The padding
		/// </summary>
		public int  Padding;
    }

	/// <summary>
	/// FullName: <see cref="ECF.Json.JsonWriter"/>
	/// Summary : Json writer
	///  JsonWriter.cs
	///    Stream-like facility to output JSON text.
	/// 
	///  The authors disclaim copyright to this source code. For more details, see
	///  the COPYING file included with this distribution.
	/// /// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	public class JsonWriter
    {
		#region Fields
		/// <summary>
		/// The number_format
		/// </summary>
		private static NumberFormatInfo number_format;

		/// <summary>
		/// The context
		/// </summary>
		private WriterContext        context;
		/// <summary>
		/// The ctx_stack
		/// </summary>
		private Stack<WriterContext> ctx_stack;
		/// <summary>
		/// The has_reached_end
		/// </summary>
		private bool                 has_reached_end;
		/// <summary>
		/// The hex_seq
		/// </summary>
		private char[]               hex_seq;
		/// <summary>
		/// The indentation
		/// </summary>
		private int                  indentation;
		/// <summary>
		/// The indent_value
		/// </summary>
		private int                  indent_value;
		/// <summary>
		/// The inst_string_builder
		/// </summary>
		private StringBuilder        inst_string_builder;
		/// <summary>
		/// The pretty_print
		/// </summary>
		private bool                 pretty_print;
		/// <summary>
		/// The validate
		/// </summary>
		private bool                 validate;
		/// <summary>
		/// The writer
		/// </summary>
		private TextWriter           writer;
		#endregion


		#region Properties
		/// <summary>
		/// Gets or sets the indent value.
		/// </summary>
		/// <value>
		/// The indent value.
		/// </value>
		public int IndentValue {
            get { return indent_value; }
            set {
                indentation = (indentation / indent_value) * value;
                indent_value = value;
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether [pretty print].
		/// </summary>
		/// <value>
		///   <c>true</c> if [pretty print]; otherwise, <c>false</c>.
		/// </value>
		public bool PrettyPrint {
            get { return pretty_print; }
            set { pretty_print = value; }
        }

		/// <summary>
		/// Gets the text writer.
		/// </summary>
		/// <value>
		/// The text writer.
		/// </value>
		public TextWriter TextWriter {
            get { return writer; }
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="JsonWriter"/> is validate.
		/// </summary>
		/// <value>
		///   <c>true</c> if validate; otherwise, <c>false</c>.
		/// </value>
		public bool Validate {
            get { return validate; }
            set { validate = value; }
        }
		#endregion


		#region Constructors
		/// <summary>
		/// Initializes the <see cref="JsonWriter"/> class.
		/// </summary>
		static JsonWriter ()
        {
            number_format = NumberFormatInfo.InvariantInfo;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		public JsonWriter ()
        {
            inst_string_builder = new StringBuilder ();
            writer = new StringWriter (inst_string_builder);

            Init ();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="sb">The sb.</param>
		public JsonWriter (StringBuilder sb) :
            this (new StringWriter (sb))
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <exception cref="ArgumentNullException">writer</exception>
		public JsonWriter (TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException ("writer");

            this.writer = writer;

            Init ();
        }
		#endregion


		#region Private Methods
		/// <summary>
		/// Does the validation.
		/// </summary>
		/// <param name="cond">The cond.</param>
		/// <exception cref="JsonException">
		/// A complete JSON symbol has already been written
		/// or
		/// Can't close an array here
		/// or
		/// Can't close an object here
		/// or
		/// Expected a property
		/// or
		/// Can't add a property here
		/// or
		/// Can't add a value here
		/// </exception>
		private void DoValidation (Condition cond)
        {
            if (! context.ExpectingValue)
                context.Count++;

            if (! validate)
                return;

            if (has_reached_end)
                throw new JsonException (
                    "A complete JSON symbol has already been written");

            switch (cond) {
            case Condition.InArray:
                if (! context.InArray)
                    throw new JsonException (
                        "Can't close an array here");
                break;

            case Condition.InObject:
                if (! context.InObject || context.ExpectingValue)
                    throw new JsonException (
                        "Can't close an object here");
                break;

            case Condition.NotAProperty:
                if (context.InObject && ! context.ExpectingValue)
                    throw new JsonException (
                        "Expected a property");
                break;

            case Condition.Property:
                if (! context.InObject || context.ExpectingValue)
                    throw new JsonException (
                        "Can't add a property here");
                break;

            case Condition.Value:
                if (! context.InArray &&
                    (! context.InObject || ! context.ExpectingValue))
                    throw new JsonException (
                        "Can't add a value here");

                break;
            }
        }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Init ()
        {
            has_reached_end = false;
            hex_seq = new char[4];
            indentation = 0;
            indent_value = 4;
            pretty_print = false;
            validate = true;

            ctx_stack = new Stack<WriterContext> ();
            context = new WriterContext ();
            ctx_stack.Push (context);
        }

		/// <summary>
		/// Ints to hexadecimal.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="hex">The hexadecimal.</param>
		private static void IntToHex (int n, char[] hex)
        {
            int num;

            for (int i = 0; i < 4; i++) {
                num = n % 16;

                if (num < 10)
                    hex[3 - i] = (char) ('0' + num);
                else
                    hex[3 - i] = (char) ('A' + (num - 10));

                n >>= 4;
            }
        }

		/// <summary>
		/// Indents this instance.
		/// </summary>
		private void Indent ()
        {
            if (pretty_print)
                indentation += indent_value;
        }


		/// <summary>
		/// Puts the specified string.
		/// </summary>
		/// <param name="str">The string.</param>
		private void Put (string str)
        {
            if (pretty_print && ! context.ExpectingValue)
                for (int i = 0; i < indentation; i++)
                    writer.Write (' ');

            writer.Write (str);
        }

		/// <summary>
		/// Puts the newline.
		/// </summary>
		private void PutNewline ()
        {
            PutNewline (true);
        }

		/// <summary>
		/// Puts the newline.
		/// </summary>
		/// <param name="add_comma">if set to <c>true</c> [add_comma].</param>
		private void PutNewline (bool add_comma)
        {
            if (add_comma && ! context.ExpectingValue &&
                context.Count > 1)
                writer.Write (',');

            if (pretty_print && ! context.ExpectingValue)
                writer.Write ('\n');
        }

		/// <summary>
		/// Puts the string.
		/// </summary>
		/// <param name="str">The string.</param>
		private void PutString (string str)
        {
            Put (String.Empty);

            writer.Write ('"');

            int n = str.Length;
            for (int i = 0; i < n; i++) {
                switch (str[i]) {
                case '\n':
                    writer.Write ("\\n");
                    continue;

                case '\r':
                    writer.Write ("\\r");
                    continue;

                case '\t':
                    writer.Write ("\\t");
                    continue;

                case '"':
                case '\\':
                    writer.Write ('\\');
                    writer.Write (str[i]);
                    continue;

                case '\f':
                    writer.Write ("\\f");
                    continue;

                case '\b':
                    writer.Write ("\\b");
                    continue;
                }

                if ((int) str[i] >= 32 && (int) str[i] <= 126) {
                    writer.Write (str[i]);
                    continue;
                }

                // Default, turn into a \uXXXX sequence
                IntToHex ((int) str[i], hex_seq);
                writer.Write ("\\u");
                writer.Write (hex_seq);
            }

            writer.Write ('"');
        }

		/// <summary>
		/// Unindents this instance.
		/// </summary>
		private void Unindent ()
        {
            if (pretty_print)
                indentation -= indent_value;
        }
		#endregion


		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString ()
        {
            if (inst_string_builder == null)
                return String.Empty;

            return inst_string_builder.ToString ();
        }

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset ()
        {
            has_reached_end = false;

            ctx_stack.Clear ();
            context = new WriterContext ();
            ctx_stack.Push (context);

            if (inst_string_builder != null)
                inst_string_builder.Remove (0, inst_string_builder.Length);
        }

		/// <summary>
		/// Writes the specified boolean.
		/// </summary>
		/// <param name="boolean">if set to <c>true</c> [boolean].</param>
		public void Write (bool boolean)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            Put (boolean ? "true" : "false");

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write (decimal number)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            Put (Convert.ToString (number, number_format));

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write (double number)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            string str = Convert.ToString (number, number_format);
            Put (str);

            if (str.IndexOf ('.') == -1 &&
                str.IndexOf ('E') == -1)
                writer.Write (".0");

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write (int number)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            Put (Convert.ToString (number, number_format));

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write (long number)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            Put (Convert.ToString (number, number_format));

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="str">The string.</param>
		public void Write (string str)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            if (str == null)
                Put ("null");
            else
                PutString (str);

            context.ExpectingValue = false;
        }

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        //[CLSCompliant(false)]
        public void Write (ulong number)
        {
            DoValidation (Condition.Value);
            PutNewline ();

            Put (Convert.ToString (number, number_format));

            context.ExpectingValue = false;
        }

		/// <summary>
		/// Writes the array end.
		/// </summary>
		public void WriteArrayEnd ()
        {
            DoValidation (Condition.InArray);
            PutNewline (false);

            ctx_stack.Pop ();
            if (ctx_stack.Count == 1)
                has_reached_end = true;
            else {
                context = ctx_stack.Peek ();
                context.ExpectingValue = false;
            }

            Unindent ();
            Put ("]");
        }

		/// <summary>
		/// Writes the array start.
		/// </summary>
		public void WriteArrayStart ()
        {
            DoValidation (Condition.NotAProperty);
            PutNewline ();

            Put ("[");

            context = new WriterContext ();
            context.InArray = true;
            ctx_stack.Push (context);

            Indent ();
        }

		/// <summary>
		/// Writes the object end.
		/// </summary>
		public void WriteObjectEnd ()
        {
            DoValidation (Condition.InObject);
            PutNewline (false);

            ctx_stack.Pop ();
            if (ctx_stack.Count == 1)
                has_reached_end = true;
            else {
                context = ctx_stack.Peek ();
                context.ExpectingValue = false;
            }

            Unindent ();
            Put ("}");
        }

		/// <summary>
		/// Writes the object start.
		/// </summary>
		public void WriteObjectStart ()
        {
            DoValidation (Condition.NotAProperty);
            PutNewline ();

            Put ("{");

            context = new WriterContext ();
            context.InObject = true;
            ctx_stack.Push (context);

            Indent ();
        }

		/// <summary>
		/// Writes the name of the property.
		/// </summary>
		/// <param name="property_name">The property_name.</param>
		public void WritePropertyName (string property_name)
        {
            DoValidation (Condition.Property);
            PutNewline ();

            PutString (property_name);

            if (pretty_print) {
                if (property_name.Length > context.Padding)
                    context.Padding = property_name.Length;

                for (int i = context.Padding - property_name.Length;
                     i >= 0; i--)
                    writer.Write (' ');

                writer.Write (": ");
            } else
                writer.Write (':');

            context.ExpectingValue = true;
        }
    }
}
