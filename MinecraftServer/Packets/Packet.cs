using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MinecraftServer.Minecraft;

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
            byte[] length = VarIntUtil.EncodeVarInt(bytesWritten + packetIdBytes.Length00);

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
            byte[] bytes = new byte[9];
            Array.Copy(data, bytesRead, bytes, 0, Math.Min(bytes.Length, data.Length - bytesRead));

            long[] results = VarIntUtil.DecodeVarLongIncludeLength(bytes);

            bytesRead += (int)results[1];

            return results[0];
        }

        protected void WriteVarLong(long value)
        {
            byte[] byteArray = VarIntUtil.EncodeVarLong(value);
            Array.Copy(byteArray, 0, data, bytesWritten, byteArray.Length);

            bytesWritten += byteArray.Length;
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

        protected void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            Array.Copy(bytes, 0, data, bytesWritten, bytes.Length);
            bytesWritten += bytes.Length;
        }

        protected long ReadLong()
        {
            byte[] bytes = new byte[8];
            Array.Copy(data, bytesRead, bytes, 0, bytes.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            long value = BitConverter.ToInt64(bytes);

            bytesRead += 8;

            return value;
        }

        protected void WritePosition(BlockPosition position)
        {
            long pos = ((position.x & 0x3FFFFFF) << 38) | ((position.z & 0x3FFFFFF) << 12) | (position.y & 0xFFF);

            WriteLong(pos);
        }

        protected BlockPosition ReadPosition()
        {
            long pos = ReadLong();

            int x = (int)pos >> 38;
            int y = (int)pos << 52 >> 52;
            int z = (int)pos << 26 >> 38;

            return new BlockPosition(x, y, z);
        }
    }
}