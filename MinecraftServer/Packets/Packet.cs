using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MinecraftServer.Packets
{
    public abstract class Packet
    {
        protected byte[] data;
        protected int bytesRead;
        protected int bytesWritten;

        public Packet(byte[] data)
        {
            this.data = data;
            bytesRead = 0;
            bytesWritten = 0;
            ReadVarInt(); // Read the packet ID
        }

        public Packet()
        {
            data = new byte[8192];
            bytesRead = 0;
            bytesWritten = 0;
        }

        /// <summary>
        /// Gets Packets's ID
        /// </summary>
        public abstract int GetID();
        /// <summary>
        /// The state of the connection in which this packet is sent/received
        /// </summary>
        public abstract ConnectionState GetTargetState();
        /// <summary>
        /// Creates the packet header and sends the packet
        /// </summary>
        public void Send(NetworkStream stream)
        {
            byte[] packetIdBytes = VarIntUtil.EncodeVarInt(GetID());
            byte[] length = VarIntUtil.EncodeVarInt(bytesWritten + packetIdBytes.Length);

            Console.WriteLine(bytesWritten + packetIdBytes.Length);

            List<byte> data = new List<byte>();
            data.AddRange(length);
            data.AddRange(packetIdBytes);
            data.AddRange(data);

            stream.Write(data.ToArray());
        }

        protected int ReadVarInt()
        {
            byte[] bytes = new byte[5];
            Array.Copy(data, bytesRead, bytes, 0, Math.Min(bytes.Length, data.Length - bytesRead));

            int[] values = VarIntUtil.DecodeVarIntIncludeLength(bytes);

            bytesRead += values[1];

            return values[0];
        }

        protected void WriteVarInt(int value)
        {
            byte[] byteArray = VarIntUtil.EncodeVarInt(value);
            Array.Copy(byteArray, 0, data, bytesWritten, byteArray.Length);

            bytesWritten += byteArray.Length;
        }
        protected string ReadString()
        {
            int length = ReadVarInt();

            byte[] stringBytes = new byte[length];
            Array.Copy(data, bytesRead, stringBytes, 0, length);

            bytesRead += stringBytes.Length;

            return Encoding.UTF8.GetString(stringBytes);
        }

        protected void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt(bytes.Length);

            Array.Copy(bytes, 0, data, bytesWritten, bytes.Length);
            bytesWritten += bytes.Length;
        }

        protected ushort ReadUShort()
        {
            byte[] bytes = new byte[2];
            Array.Copy(data, bytesRead, bytes, 0, 2);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            bytesRead += 2;

            return BitConverter.ToUInt16(bytes);
        }

        protected void WriteUShort(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            Array.Copy(bytes, 0, data, bytesWritten, bytes.Length);
        }

        protected long ReadVarLong()
        {
            byte[] bytes = new byte[5];
            Array.Copy(data, bytesRead, bytes, 0, Math.Min(bytes.Length, data.Length - bytesRead));

            long value = 0;
            int position = 0;
            int byteIndex = 0;
            byte currentByte;

            while (true)
            {
                currentByte = bytes[byteIndex];
                value |= (long)(currentByte & 127) << position;

                position += 7;
                byteIndex++;

                if ((currentByte & 128) == 0) break;

                if (byteIndex > 4)
                {
                    throw new Exception("VarInt has too many bytes");
                }
            }

            bytesRead += byteIndex;

            return value;
        }

        protected void WriteVarLong(long value)
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
            byte[] byteArray = bytes.ToArray();
            Array.Copy(byteArray, 0, data, bytesWritten, byteArray.Length);

            bytesWritten += bytes.Count;
        }

        protected void WriteBoolean(bool value)
        {
            data[bytesWritten] = (byte)(value ? 0x01 : 0x00);
            bytesWritten++;
        }

        protected bool ReadBoolean()
        {
            bool value = data[bytesRead] == 0x01;
            bytesRead++;
            return value;
        }

        protected void WriteByte(byte value)
        {
            data[bytesWritten] = value;
            bytesWritten++;
        }

        protected byte ReadByte()
        {
            byte value = data[bytesRead];
            bytesRead++;
            return value;
        }

        protected void WriteSByte(sbyte value)
        {
            data[bytesWritten] = (byte)value;
            bytesWritten++;
        }

        protected sbyte ReadSByte()
        {
            sbyte value = (sbyte)data[bytesRead];
            bytesRead++;
            return value;
        }

        protected void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            Array.Copy(bytes, 0, data, bytesWritten, bytes.Length);
            bytesWritten += 4;
        }

        protected int ReadInt()
        {
            byte[] bytes = new byte[4];
            Array.Copy(data, bytesRead, bytes, 0, bytes.Length);
            bytesRead += 4;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes);
        }
    }
}