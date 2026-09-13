using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EBM2x.Modules
{
    public static class Base32
    {
        private const string cBase32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string cBase32Pad = "=";

        public static string ToBase32String(this byte[] Data, bool IncludePadding = true)
        {
            string RetVal = "";
            var Segments = new List<long>();

            // Divide the input data into 5 byte segments
            int Index = 0;
            while (Index < Data.Length)
            {
                long CurrentSegment = 0;
                int SegmentSize = 0;

                while (Index < Data.Length & SegmentSize < 5)
                {
                    CurrentSegment <<= 8;
                    CurrentSegment += Data[Index];

                    Index += 1;
                    SegmentSize += 1;
                }

                // If the size of the last segment was less than 5 bytes, pad with zeros
                CurrentSegment <<= 8 * (5 - SegmentSize);
                Segments.Add(CurrentSegment);
            }

            // Convert each 5 byte segment into 8 character strings
            foreach (long CurrentSegment in Segments)
            {
                for (int x = 0; x <= 7; x++)
                    RetVal += Convert.ToString(cBase32Alphabet[Convert.ToInt32(CurrentSegment >> (7 - x) * 5 & 0x1F)]);
            }

            // Correct the end of the string (where the input wasn't a multiple of 5 bytes)
            int LastSegmentUsefulDataLength = Convert.ToInt32(Math.Ceiling(Data.Length % 5 * 8 / (double)5));
            RetVal = RetVal.Substring(0, RetVal.Length - (8 - LastSegmentUsefulDataLength));

            // Add the padding characters 
            if (IncludePadding)
                RetVal += new string(Convert.ToChar(cBase32Pad), 8 - LastSegmentUsefulDataLength);

            return RetVal;
        }

        public static byte[] FromBase32ToByte(this string Data)
        {
            var RetVal = new List<byte>();
            var Segments = new List<long>();

            // Remove any trailing padding
            Data = Data.TrimEnd(new char[] { cBase32Pad[0] });

            // Process the string 8 characters at a time
            int Index = 0;
            while (Index < Data.Length)
            {
                long CurrentSegment = 0;
                int SegmentSize = 0;

                while (Index < Data.Length & SegmentSize < 8)
                {
                    CurrentSegment <<= 5;
                    CurrentSegment = CurrentSegment | (long)cBase32Alphabet.IndexOf(Data[Index]);

                    Index += 1;
                    SegmentSize += 1;
                }

                // If the size of the last segment was less than 40 bits, pad it
                CurrentSegment <<= 5 * (8 - SegmentSize);
                Segments.Add(CurrentSegment);
            }

            // Break the 5 byte segments back down into individual bytes
            foreach (long CurrentSegment in Segments)
            {
                for (int x = 0; x <= 4; x++)
                    RetVal.Add(Convert.ToByte(CurrentSegment >> (4 - x) * 8 & 0xFF));
            }

            // Remove any bytes of padding from the output
            int BytesToRemove = Convert.ToInt32(5 - Math.Ceiling(Math.Ceiling(3 * 8 / (double)5) / 2));
            RetVal.RemoveRange(RetVal.Count - BytesToRemove, BytesToRemove);

            return RetVal.ToArray();
        }


        private static string BytesToString(byte[] Input)
        {
            var Result = new StringBuilder(Input.Length * 2);
            string Part;

            foreach (byte b in Input)
            {
                Part = Convert.ToString(b);
                if (Part.Length == 1)
                    Part = "0" + Part;
                Result.Append(Part);
            }

            string r = Result.ToString();
            // MsgBox(r.Substring(0, 64))
            if (r.Length == 66 && Convert.ToString(r.ElementAt(65)) == "0" && Convert.ToString(r.ElementAt(64)) == "0")
                return r.Substring(0, 64);
            return Result.ToString();
        }


        private static object AESE(string plaintext, string key)
        {
            var AES = new RijndaelManaged();
            // Dim AES As New System.Security.Cryptography.AesCryptoServiceProvider
            // XFZSPGWGISLRRI5PNIGVKN4KM4
            // XFZSPGWGISLRRI5PNIGVKN4KM4
            var SHA256 = System.Security.Cryptography.SHA256.Create();
            string ciphertext = "";
            try
            {
                // AES.GenerateIV()
                // AES.Key = SHA256.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(key))
                // AES.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key)
                AES.Key = HexToByteArray(key, 31);
                AES.Padding = PaddingMode.None;
                AES.Mode = CipherMode.ECB;
                // AES.BlockSize = 128

                var DESEncrypter = AES.CreateEncryptor();
                // Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(plaintext)
                var Buffer = HexToByteArray(plaintext, 15);
                ciphertext = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                return DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length);
            }
            // Return Convert.ToBase64String(AES.IV) & Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static object HashString(string data, string hashkey)
        {
            var myencoder = new UTF8Encoding();
            // Dim key() As Byte = myencoder.GetBytes(hashkey)

            var key = HexToByteArray(hashkey, 31);
            var text = myencoder.GetBytes(data);
            var myHMAC = new HMACSHA1(key);
            var hashcode = myHMAC.ComputeHash(text);
            // Dim hash As String = Replace(BitConverter.ToString(hashcode), "-", "")
            return hashcode;
        }

        public static string DecodeHexString(string sText)
        {
            int intLength = sText.Length;
            if (intLength == 0)
                return "";
            int intCount = 0;
            var sb = new StringBuilder(Convert.ToInt32(intLength / (double)2));
            try
            {
                var loopTo = sText.Length - 1;
                for (intCount = 0; intCount <= loopTo; intCount += 2)
                    sb.Append(Convert.ToChar(byte.Parse(sText.Substring(intCount, 2), System.Globalization.NumberStyles.HexNumber)));
            }
            catch (Exception ex)
            {
                return "";
            }
            return sb.ToString();
        }

        // public string signKey { get; set; }  // Signature Key
        public static object Receipt_signature(string data, string key)
        {
            byte[] DataToEncode;
            var CopiedData = new byte[11];

            string hexaKey = BytesToString(key.FromBase32ToByte());

            DataToEncode = (byte[])HashString(data, hexaKey);

            string Base32;
            Array.Copy(DataToEncode, CopiedData, 10);
            Base32 = CopiedData.ToBase32String();

            if (Base32.Length == 16)
                return Base32;
            else if (Convert.ToString(Base32[17]) == "A" || Convert.ToString(Base32[17]) == "=")
                return Base32.Substring(0, 16);
            else
                return "Error in Signature Encryption";
        }

        // public string intrlKey { get; set; }  // Internal Key
        public static string Internal_data(string data, string key)
        {
            byte[] enc;
            // Console.WriteLine(data)
            // Console.WriteLine(key)
            string hexaKey = BytesToString(key.FromBase32ToByte());
            enc = (byte[])AESE(data, hexaKey);

            // ByteArrayToHex(enc)

            string enc_base32;
            enc_base32 = enc.ToBase32String();
            //Console.WriteLine("enc base32:" + enc_base32);
            if (enc_base32.Length == 26)
                return enc_base32;
            else if (Convert.ToString(enc_base32[27]) == "A" || Convert.ToString(enc_base32[27]) == "=")
                return enc_base32.Substring(0, 26);
            else if (enc_base32.Length > 27)
                return enc_base32.Substring( 0, 26);
            else
                return "Error in Internal Encryption";
        }

        public static object longToHex(long lval, int nbByte)
        {
            string hexString = null;

            // convert into hexadecimal 
            hexString = Convert.ToString(lval, 16);
            int nbChar = hexString.Length;

            if (nbChar < nbByte * 2)
            {
                for (int i = nbChar + 1, loopTo = nbByte * 2; i <= loopTo; i++)
                    hexString = "0" + hexString;
            }

            return hexString;
        }

        private static byte[] HexToByteArray(string hex, int n)
        {
            int NumberChars = hex.Length;
            if (NumberChars % 2 != 0)
            {
                hex = "0" + hex;
                NumberChars = hex.Length;
            }

            var bytes = new byte[n + 1];

            int j = 0;
            for (int i = 0, loopTo = NumberChars - 1; i < loopTo; i += 2)
            {
                bytes[j] = Convert.ToByte(hex.Substring(i, 2), 16);
                j = j + 1;

                if (j > n) break;
            }

            return bytes;
        }

        public static object nb_days(DateTime exp_date)
        {
            DateTime D1 = DateTime.Now;
            if(exp_date > D1)
            {
                TimeSpan nod = (exp_date - D1);
                return nod.TotalDays;
            }
            else
            {
                TimeSpan nod = (D1 - exp_date);
                return nod.TotalDays;
            }
        }

        public static string setRounding(decimal tot)
        {
            string v;
            int ind, leng;
            string dec = "";
            int decval = 0;
            string intval = "";
            string fval = "";
            v = tot.ToString();

            if (v.Contains("."))
            {
                v = v.Replace(",", "");
                ind = v.IndexOf(".");
                leng = v.Length;
                intval = v.Substring(0, ind);

                //Interaction.MsgBox(leng - ind);

                if (leng - ind == 2)
                    fval = v + "0";
                else if (leng - ind == 1)
                    fval = v + "00";
                else if (leng - ind >= 3)
                {
                    dec = v.Substring(ind + 1, 3);
                    decval = Convert.ToInt32(Convert.ToDouble(dec) + 5);
                    dec = decval.ToString().Substring(0, 2);
                    fval = intval + "." + dec;
                }
            }
            else
            {
                v = v.Replace(",", "");
                fval = v + ".00";
            }
            return fval;
        }

        public static string setRound(decimal tot)
        {
            double c1;
            double c2;

            c1 = Convert.ToDouble(tot.ToString());

            c2 = Math.Round(c1, 2);
            return c2.ToString("0.00");
        }
    }
}
