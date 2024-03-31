using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer
{
    public class VarIntUtil
    {
        /// <summary>
        /// Decodes the VarInt
        /// </summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <returns>The decoded VarInt as an int</returns>
        /// <exception cref="Exception"></exception>
        public static int DecodeVarInt(byte[] bytes)
        {
            int value = 0;
            int position = 0;
            int byteIndex = 0;
            byte currentByte;

            while (true)
            {
                currentByte = bytes[byteIndex];
                value |= (currentByte & 127) << position;

                position += 7;
                byteIndex++;

                if ((currentByte & 128) == 0) break;

                if (byteIndex > 4)
                {
                    throw new Exception("VarInt has too many bytes");
                }
            }

            return value;
        }
        /// <summary>
        /// Encodes the VarInt
        /// </summary>
        /// <param name="value">Int to encode</param>
        /// <returns>Bytes of the encoded VarInt</returns>
        public static byte[] EncodeVarInt(int value)
        {
            List<byte> bytes = new List<byte>();

            while (true)
            {
                if ((value & ~127) == 0)
                {
                    bytes.Add((byte)value);
                    break;
                }
                bytes.Add((byte)((value & 127) | 128));

                value >>>= 7;
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes the VarInt
        /// </summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <returns>An array with the first element being the int and the second the amount of bytes used</returns>
        /// <exception cref="Exception"></exception>
        public static int[] DecodeVarIntIncludeLength(byte[] bytes)
        {
            int value = 0;
            int position = 0;
            int byteIndex = 0;
            byte currentByte;

            while (true)
            {
                currentByte = bytes[byteIndex];
                value |= (currentByte & 127) << position;

                position += 7;
                byteIndex++;

                if ((currentByte & 128) == 0) break;

                if (byteIndex > 4)
                {
                    throw new Exception("VarInt has too many bytes");
                }
            }

            return new int[] { value, byteIndex };
        }
    }
}
