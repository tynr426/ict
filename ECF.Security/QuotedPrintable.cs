using System.Text;

namespace ECF.Security
{
	/// <summary>
	/// FullName： <see cref="ECF.Security.QuotedPrintable"/>
	/// Summary ： 主要用于邮件收取时编码和解码 
	/// Version： 1.0.0.0 
	/// DateTime： 2012/5/27 12:24 
	/// CopyRight (c) by shaipe
	/// </summary>
	public class QuotedPrintable
    {
        private const byte EQUALS = 61;
        private const byte CR = 13;
        private const byte LF = 10;
        private const byte SPACE = 32;
        private const byte TAB = 9;

        #region QuotedPrintable 编码

        /// <summary>
        /// QuotedPrintable 编码
        /// </summary>
        /// <param name="_ToEncode">String to encode</param>
        /// <param name="charset">The charset.</param>
        /// <returns>
        /// QuotedPrintable encoded string
        /// </returns>
        public static string Encode(string _ToEncode, string charset)
        {
            try
            {
                StringBuilder Encoded = new StringBuilder();
                string hex = string.Empty;
                byte[] bytes = Utils.GetEncoding(charset).GetBytes(_ToEncode);
                int count = 0;

                for (int i = 0; i < bytes.Length; i++)
                {
                    //these characters must be encoded
                    if ((bytes[i] < 33 || bytes[i] > 126 || bytes[i] == EQUALS) && bytes[i] != CR && bytes[i] != LF && bytes[i] != SPACE)
                    {
                        if (bytes[i].ToString("X").Length < 2)
                        {
                            hex = "0" + bytes[i].ToString("X");
                            Encoded.Append("=" + hex);
                        }
                        else
                        {
                            hex = bytes[i].ToString("X");
                            Encoded.Append("=" + hex);
                        }
                    }
                    else
                    {
                        //check if index out of range
                        if ((i + 1) < bytes.Length)
                        {
                            //if TAB is at the end of the line - encode it!
                            if ((bytes[i] == TAB && bytes[i + 1] == LF) || (bytes[i] == TAB && bytes[i + 1] == CR))
                            {
                                Encoded.Append("=0" + bytes[i].ToString("X"));
                            }
                            //if SPACE is at the end of the line - encode it!
                            else if ((bytes[i] == SPACE && bytes[i + 1] == LF) || (bytes[i] == SPACE && bytes[i + 1] == CR))
                            {
                                Encoded.Append("=" + bytes[i].ToString("X"));
                            }
                            else
                            {
                                Encoded.Append(System.Convert.ToChar(bytes[i]));
                            }
                        }
                        else
                        {
                            Encoded.Append(System.Convert.ToChar(bytes[i]));
                        }
                    }
                    if (count == 75)
                    {
                        Encoded.Append("=\r\n"); //insert soft-linebreak
                        count = 0;
                    }
                    count++;
                }

                return Encoded.ToString();
            }
            catch (System.Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }

        #endregion

        #region QuotedPrintable 解码

        /// <summary>
        /// Decode  QuotedPrintable
        /// </summary>
        /// <param name="_ToDecode">The encoded string to decode</param>
        /// <param name="charset">The charset.</param>
        /// <returns>
        /// Decoded string
        /// </returns>
        public static string Decode(string _ToDecode, string charset)
        {
            try
            {
                //remove soft-linebreaks first
                _ToDecode = _ToDecode.Replace("=\r\n", "");

                char[] chars = _ToDecode.ToCharArray();

                byte[] bytes = new byte[chars.Length];

                int bytesCount = 0;

                for (int i = 0; i < chars.Length; i++)
                {
                    // if encoded character found decode it
                    if (chars[i] == '=')
                    {
                        bytes[bytesCount++] = System.Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));

                        i += 2;
                    }
                    else
                    {
                        bytes[bytesCount++] = System.Convert.ToByte(chars[i]);
                    }
                }

                return Utils.GetEncoding(charset).GetString(bytes, 0, bytesCount);
            }
            catch (System.Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }


        #endregion

       


    }
}
