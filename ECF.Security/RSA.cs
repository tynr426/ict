using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ECF.Security
{
	/// <summary>
	/// FullName: <see cref="ECF.Security.RSA"/>
	/// Summary : RSA 加解密
	/// Version: 2.1
	/// DateTime: 2015/5/11 
	/// CopyRight (c) Shaipe
	/// </summary>
	public class RSA
	{
		
		/// <summary>
		/// FullName: <see cref="ECF.Security.RSA.Public"/>
		/// Summary : 公钥加解密
		/// Version: 2.1
		/// DateTime: 2015/5/11 
		/// CopyRight (c) Shaipe
		/// </summary>
		public class Public
		{
			/// <summary>
			/// 公钥加密字符串
			/// </summary>
			/// <param name="content">明文内容</param>
			/// <param name="publicKey">公钥字符串</param>
			/// <param name="input_charset">明文内容编码</param>
			/// <returns></returns>
			public static string Encrypt(string content, string publicKey, string input_charset = "utf-8")
			{
				byte[] Modules, Exponent;
				if (parseKey(publicKey, out Modules, out Exponent))
				{
					return RSA.encrypt(content, toBigInteger(Modules), toBigInteger(Exponent), input_charset, Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 公钥加密字节数组
			/// </summary>
			/// <param name="data">明文数据</param>
			/// <param name="publicKey">公钥字符串</param>
			/// <returns></returns>
			private static byte[] encrypt(byte[] data, string publicKey)
			{
				byte[] Modules, Exponent;
				if (parseKey(publicKey, out Modules, out Exponent))
				{
					return RSA.encrypt(data, toBigInteger(Modules), toBigInteger(Exponent), Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 公钥解密字符串
			/// </summary>
			/// <param name="content">密文字符串</param>
			/// <param name="publicKey">公钥字符串</param>
			/// <param name="input_charset">明文内容编码</param>
			/// <returns></returns>
			public static string Decrypt(string content, string publicKey, string input_charset = "utf-8")
			{
				byte[] Modules, Exponent;
				if (parseKey(publicKey, out Modules, out Exponent))
				{
					return RSA.decrypt(content, toBigInteger(Modules), toBigInteger(Exponent), input_charset, Modules.Length);
				}
				return null;
			}
			/// <summary>
			/// 公钥解密字节数组
			/// </summary>
			/// <param name="data">密文数据</param>
			/// <param name="publicKey">公钥字符串</param>
			/// <returns></returns>
			private static byte[] decrypt(byte[] data, string publicKey)
			{
				byte[] Modules, Exponent;
				if (parseKey(publicKey, out Modules, out Exponent))
				{
					return RSA.decrypt(data, toBigInteger(Modules), toBigInteger(Exponent), Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 解析公钥
			/// </summary>
			/// <param name="pk">公钥字符串</param>
			/// <param name="Modules">MODULES参数</param>
			/// <param name="Exponent">EXPONENT参数</param>
			/// <returns></returns>
			private static bool parseKey(String pk, out byte[] Modules, out byte[] Exponent)
			{
				byte[] pkcs8pulickey = Convert.FromBase64String(pk);
				if (pkcs8pulickey != null)
				{
					return DecodeKeyInfo(pkcs8pulickey, out Modules, out Exponent);
				}
				else
				{
					Modules = null;
					Exponent = null;
					return false;
				}
			}

			private static bool DecodeKeyInfo(byte[] pkcs8, out byte[] Modules, out byte[] Exponent)
			{
				Modules = Exponent = null;
				byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
				byte[] seq = new byte[15];

				MemoryStream mem = new MemoryStream(pkcs8);
				int lenstream = (int)mem.Length;
				BinaryReader binr = new BinaryReader(mem);	  //wrap Memory Stream with BinaryReader for easy reading
				//byte bt = 0;
				ushort twobytes = 0;

				try
				{
					twobytes = binr.ReadUInt16();
					if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
						binr.ReadByte();	//advance 1 byte
					else if (twobytes == 0x8230)
						binr.ReadInt16();	//advance 2 bytes
					else
						return false;

					seq = binr.ReadBytes(15);		//read the Sequence OID
					if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
						return false;

					twobytes = binr.ReadUInt16();
					if (twobytes != 0x8130)	//expect an Octet string 
						binr.ReadByte();
					else if (twobytes != 0x8230)
						binr.ReadUInt16();
					else
						return false;

					//------ at this stage, the remaining sequence should be the RSA private key
					binr.ReadByte();
					byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position + 1));
					return DecodeRSAKey(rsaprivkey, out Modules, out Exponent);
				}

				catch (Exception)
				{
					return false;
				}

				finally { binr.Close(); }
			}
			private static bool DecodeRSAKey(byte[] key, out byte[] Modules, out byte[] Exponent)
			{
				Modules = Exponent = null;
				// ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
				MemoryStream mem = new MemoryStream(key);
				BinaryReader binr = new BinaryReader(mem);	  //wrap Memory Stream with BinaryReader for easy reading
				//byte bt = 0;
				ushort twobytes = 0;
				int elems = 0;
				try
				{
					twobytes = binr.ReadUInt16();
					if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
						binr.ReadByte();	//advance 1 byte
					else if (twobytes == 0x8230)
						binr.ReadInt16();	//advance 2 bytes
					else
						return false;

					elems = GetIntegerSize(binr);
					Modules = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					Exponent = binr.ReadBytes(elems);

					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally { binr.Close(); }
			}
		}

		/// <summary>
		/// FullName: <see cref="ECF.Security.RSA.Private"/>
		/// Summary : 私钥加解密
		/// Version: 2.1
		/// DateTime: 2015/5/11 
		/// CopyRight (c) Shaipe
		/// </summary>
		public class Private
		{
			/// <summary>
			/// 私钥加密字符串
			/// </summary>
			/// <param name="content">明文内容</param>
			/// <param name="privateKey">私钥字符串</param>
			/// <param name="input_charset">明文内容编码</param>
			/// <returns></returns>
			public static string Encrypt(string content, string privateKey, string input_charset = "utf-8")
			{
				byte[] Modules, D;
				if (ParseKey(privateKey, out Modules, out D))
				{
					return RSA.encrypt(content, toBigInteger(Modules), toBigInteger(D), input_charset, Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 私钥加密字节数组
			/// </summary>
			/// <param name="data">明文数据</param>
			/// <param name="privateKey">私钥字符串</param>
			/// <returns></returns>
			private static byte[] encrypt(byte[] data, string privateKey)
			{
				byte[] Modules, D;
				if (ParseKey(privateKey, out Modules, out D))
				{
					return RSA.encrypt(data, toBigInteger(Modules), toBigInteger(D), Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 私钥解密字符串
			/// </summary>
			/// <param name="content">密文字符串</param>
			/// <param name="privateKey">私钥字符串</param>
			/// <param name="input_charset">明文内容编码</param>
			/// <returns></returns>
			private static string Decrypt(string content, string privateKey, string input_charset = "utf-8")
			{
				byte[] Modules, D;
				if (ParseKey(privateKey, out Modules, out D))
				{
					return RSA.decrypt(content, toBigInteger(Modules), toBigInteger(D), input_charset, Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 私钥解密字节数组
			/// </summary>
			/// <param name="data">密文数据</param>
			/// <param name="privateKey">私钥字符串</param>
			/// <returns></returns>
			private static byte[] decrypt(byte[] data, string privateKey)
			{
				byte[] Modules, D;
				if (ParseKey(privateKey, out Modules, out D))
				{
					return RSA.decrypt(data, toBigInteger(Modules), toBigInteger(D), Modules.Length);
				}
				return null;
			}

			/// <summary>
			/// 解析私钥字符串
			/// </summary>
			/// <param name="pk">私钥字符串</param>
			/// <param name="Modules">MODULES参数</param>
			/// <param name="D">D参数</param>
			/// <returns></returns>
			private static bool ParseKey(String pk, out byte[] Modules, out byte[] D)
			{
				byte[] pkcs8pulickey = Convert.FromBase64String(pk);
				if (pkcs8pulickey != null)
				{
					return DecodeKeyInfo(pkcs8pulickey, out Modules, out D);
				}
				else
				{
					Modules = null;
					D = null;
					return false;
				}
			}

			private static bool DecodeKeyInfo(byte[] pkcs8, out byte[] Modules, out byte[] D)
			{
				Modules = D = null;
				byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
				byte[] seq = new byte[15];

				MemoryStream mem = new MemoryStream(pkcs8);
				int lenstream = (int)mem.Length;
				BinaryReader binr = new BinaryReader(mem);	  //wrap Memory Stream with BinaryReader for easy reading
				byte bt = 0;
				ushort twobytes = 0;

				try
				{
					twobytes = binr.ReadUInt16();
					if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
						binr.ReadByte();	//advance 1 byte
					else if (twobytes == 0x8230)
						binr.ReadInt16();	//advance 2 bytes
					else
						return false;

					bt = binr.ReadByte();
					if (bt != 0x02)
						return false;

					twobytes = binr.ReadUInt16();

					if (twobytes != 0x0001)
						return false;

					seq = binr.ReadBytes(15);		//read the Sequence OID
					if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
						return false;

					bt = binr.ReadByte();
					if (bt != 0x04)	//expect an Octet string 
						return false;

					bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
					if (bt == 0x81)
						binr.ReadByte();
					else
						if (bt == 0x82)
						binr.ReadUInt16();
					//------ at this stage, the remaining sequence should be the RSA private key

					byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
					return DecodeRSAKey(rsaprivkey, out Modules, out D);
				}

				catch (Exception)
				{
					return false;
				}

				finally { binr.Close(); }
			}
			private static bool DecodeRSAKey(byte[] key, out byte[] Modules, out byte[] D)
			{
				Modules = D = null;
				byte[] E, P, Q, DP, DQ, IQ;


				// ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
				MemoryStream mem = new MemoryStream(key);
				BinaryReader binr = new BinaryReader(mem);	  //wrap Memory Stream with BinaryReader for easy reading
				byte bt = 0;
				ushort twobytes = 0;
				int elems = 0;
				try
				{
					twobytes = binr.ReadUInt16();
					if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
						binr.ReadByte();	//advance 1 byte
					else if (twobytes == 0x8230)
						binr.ReadInt16();	//advance 2 bytes
					else
						return false;

					twobytes = binr.ReadUInt16();
					if (twobytes != 0x0102)	//version number
						return false;
					bt = binr.ReadByte();
					if (bt != 0x00)
						return false;


					//------  all private key components are Integer sequences ----
					elems = GetIntegerSize(binr);
					Modules = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					E = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					D = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					P = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					Q = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					DP = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					DQ = binr.ReadBytes(elems);

					elems = GetIntegerSize(binr);
					IQ = binr.ReadBytes(elems);

					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally { binr.Close(); }
			}
		}

		#region private Mothed
		/// <summary>
		/// Encrypts the specified content.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="M">The m.</param>
		/// <param name="K">The k.</param>
		/// <param name="input_charset">The input_charset.</param>
		/// <param name="kLen">Length of the k.</param>
		/// <returns></returns>
		private static string encrypt(string content, BigInteger M, BigInteger K, string input_charset, int kLen = 128)
		{

			byte[] data = Encoding.GetEncoding(input_charset).GetBytes(content);
			byte[] result = encrypt(data, M, K, kLen);
			return Convert.ToBase64String(result);
		}

		/// <summary>
		/// Encrypts the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="M">The m.</param>
		/// <param name="K">The k.</param>
		/// <param name="kLen">Length of the k.</param>
		/// <returns></returns>
		private static byte[] encrypt(byte[] data, BigInteger M, BigInteger K, int kLen = 128)
		{
			byte[] buffer = new byte[kLen];
			MemoryStream msInput = new MemoryStream(data);
			MemoryStream msOutput = new MemoryStream();
			int bufferSize = kLen - 11;
			int readLen = msInput.Read(buffer, 0, bufferSize);
			Random r = new Random((int)DateTime.Now.ToBinary());
			while (readLen > 0)
			{
				byte[] dataToEnc = new byte[kLen];
				Array.Copy(buffer, 0, dataToEnc, kLen - readLen, readLen);
				for (int i = 0; i < kLen - readLen - 3; i++)
				{
					dataToEnc[i + 2] = (byte)r.Next(1, 255);
				}
				dataToEnc[0] = dataToEnc[kLen - readLen - 1] = 0;
				dataToEnc[1] = 2;

				BigInteger source = new BigInteger(dataToEnc.Reverse().ToArray());
				BigInteger dest = BigInteger.ModPow(source, K, M);

				dataToEnc = fromBigInteger(dest, kLen);
				msOutput.Write(dataToEnc, 0, dataToEnc.Length);
				readLen = msInput.Read(buffer, 0, bufferSize);
			}



			byte[] result = msOutput.ToArray();	   //得到加密结果
			msInput.Close();
			msOutput.Close();

			return result;
		}

		/// <summary>
		/// Froms the big integer.
		/// </summary>
		/// <param name="v">The v.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		private static byte[] fromBigInteger(BigInteger v, int length)
		{
			byte[] r = v.ToByteArray().Reverse().ToArray();
			if (r.Length < length)
			{
				byte[] t = new byte[length];
				Array.Copy(r, 0, t, length - r.Length, r.Length);
				return t;
			}
			return r;
		}

		/// <summary>
		/// To the big integer.
		/// </summary>
		/// <param name="v">The v.</param>
		/// <returns></returns>
		private static BigInteger toBigInteger(byte[] v)
		{
			byte[] r = new byte[v.Length + 1];
			Array.Copy(v.Reverse().ToArray(), 0, r, 0, v.Length);
			return new BigInteger(r);
		}

		/// <summary>
		/// Decrypts the specified content.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="M">The m.</param>
		/// <param name="K">The k.</param>
		/// <param name="input_charset">The input_charset.</param>
		/// <param name="kLen">Length of the k.</param>
		/// <returns></returns>
		private static string decrypt(string content, BigInteger M, BigInteger K, string input_charset, int kLen = 128)
		{

			byte[] data = Convert.FromBase64String(content);
			byte[] result = decrypt(data, M, K, kLen);
			if (result != null)
			{
				char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(result, 0, result.Length)];
				Encoding.GetEncoding(input_charset).GetChars(result, 0, result.Length, asciiChars, 0);
				return new string(asciiChars);
			}
			return null;
		}

		/// <summary>
		/// Decrypts the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="M">The m.</param>
		/// <param name="K">The k.</param>
		/// <param name="kLen">Length of the k.</param>
		/// <returns></returns>
		private static byte[] decrypt(byte[] data, BigInteger M, BigInteger K, int kLen = 128)
		{
			byte[] buffer = new byte[kLen];
			MemoryStream msInput = new MemoryStream(data);
			MemoryStream msOutput = new MemoryStream();

			int readLen = msInput.Read(buffer, 0, kLen);
			while (readLen > 0)
			{
				byte[] dataToEnc = new byte[readLen];
				Array.Copy(buffer, 0, dataToEnc, 0, readLen);
				dataToEnc = fromBigInteger(BigInteger.ModPow(toBigInteger(dataToEnc), K, M), kLen);
				if (dataToEnc[0] != 0 && dataToEnc[1] != 2)
				{
					msInput.Close();
					msOutput.Close();
					return null;
				}
				int i = 2;
				while (dataToEnc[i++] != 0) ;

				msOutput.Write(dataToEnc, i, dataToEnc.Length - i);
				readLen = msInput.Read(buffer, 0, kLen);
			}



			byte[] result = msOutput.ToArray();	   //得到加密结果
			msInput.Close();
			msOutput.Close();

			return result;
		}

		/// <summary>
		/// Compares the bytearrays.
		/// </summary>
		/// <param name="a">a.</param>
		/// <param name="b">The b.</param>
		/// <returns></returns>
		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				return false;
			int i = 0;
			foreach (byte c in a)
			{
				if (c != b[i])
					return false;
				i++;
			}
			return true;
		}

		/// <summary>
		/// Gets the size of the integer.
		/// </summary>
		/// <param name="binr">The binr.</param>
		/// <returns></returns>
		private static int GetIntegerSize(BinaryReader binr)
		{
			byte bt = 0;
			byte lowbyte = 0x00;
			byte highbyte = 0x00;
			int count = 0;
			bt = binr.ReadByte();
			if (bt != 0x02)		//expect integer
				return 0;
			bt = binr.ReadByte();

			if (bt == 0x81)
				count = binr.ReadByte();	// data size in next byte
			else
				if (bt == 0x82)
			{
				highbyte = binr.ReadByte();	// data size in next 2 bytes
				lowbyte = binr.ReadByte();
				byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
				count = BitConverter.ToInt32(modint, 0);
			}
			else
			{
				count = bt;		// we already have the data size
			}



			while (binr.ReadByte() == 0x00)
			{	//remove high order zeros in data
				count -= 1;
			}
			binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
			return count;
		} 
		#endregion
	}
}
