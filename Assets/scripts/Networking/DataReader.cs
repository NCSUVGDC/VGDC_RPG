using System;
using System.Text;

namespace VGDC_RPG.Networking
{
    public class DataReader
    {
        public int Length { get; private set; }
        public byte[] Buffer { get; private set; }
        private int i = 0;

        public DataReader(byte[] buffer, int length)
        {
            Length = length;
            Buffer = buffer;
        }

        public byte ReadByte()
        {
            return Buffer[i++];
        }

        public ushort ReadUInt16()
        {
            var r = BitConverter.ToUInt16(Buffer, i);
            i += 2;
            return r;
        }

        public uint ReadUInt32()
        {
            var r = BitConverter.ToUInt32(Buffer, i);
            i += 4;
            return r;
        }

        public ulong ReadUInt64()
        {
            var r = BitConverter.ToUInt64(Buffer, i);
            i += 8;
            return r;
        }

        public short ReadInt16()
        {
            var r = BitConverter.ToInt16(Buffer, i);
            i += 2;
            return r;
        }

        public int ReadInt32()
        {
            var r = BitConverter.ToInt32(Buffer, i);
            i += 4;
            return r;
        }

        public long ReadInt64()
        {
            var r = BitConverter.ToInt64(Buffer, i);
            i += 8;
            return r;
        }

        public float ReadSingle()
        {
            var r = BitConverter.ToSingle(Buffer, i);
            i += 4;
            return r;
        }

        public double ReadDouble()
        {
            var r = BitConverter.ToDouble(Buffer, i);
            i += 4;
            return r;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] r = new byte[length];
            System.Buffer.BlockCopy(Buffer, i, r, 0, length);
            i += length;
            return r;
        }

        public string ReadString()
        {
            var c = ReadInt16();
            var r = Encoding.ASCII.GetString(Buffer, i, c);
            i += c;
            return r;
        }
    }
}
