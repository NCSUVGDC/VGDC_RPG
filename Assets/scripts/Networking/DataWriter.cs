using System;
using System.Text;

namespace VGDC_RPG.Networking
{
    public class DataWriter
    {
        public int Length { get; private set; }
        public byte[] Buffer { get; private set; }

        public DataWriter(int length)
        {
            Buffer = new byte[length];
            Length = 0;
        }

        public DataWriter(byte[] buffer)
        {
            Buffer = buffer;
            Length = 0;
        }

        public void Write(byte v)
        {
            Buffer[Length++] = v;
        }

        public void Write(ushort v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
        }

        public void Write(uint v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
        }

        public void Write(ulong v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
            Buffer[Length++] = b[4];
            Buffer[Length++] = b[5];
            Buffer[Length++] = b[6];
            Buffer[Length++] = b[7];
        }

        public void Write(short v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
        }

        public void Write(int v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
        }

        public void Write(long v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
            Buffer[Length++] = b[4];
            Buffer[Length++] = b[5];
            Buffer[Length++] = b[6];
            Buffer[Length++] = b[7];
        }

        public void Write(float v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
        }

        public void Write(double v)
        {
            var b = BitConverter.GetBytes(v);
            Buffer[Length++] = b[0];
            Buffer[Length++] = b[1];
            Buffer[Length++] = b[2];
            Buffer[Length++] = b[3];
            Buffer[Length++] = b[4];
            Buffer[Length++] = b[5];
            Buffer[Length++] = b[6];
            Buffer[Length++] = b[7];
        }

        public void Write(byte[] v)
        {
            System.Buffer.BlockCopy(v, 0, Buffer, Length, v.Length);
            Length += v.Length;
        }

        public void Write(string v)
        {
            var b = Encoding.ASCII.GetBytes(v);
            Write((short)b.Length);
            Write(b);
        }
    }
}
