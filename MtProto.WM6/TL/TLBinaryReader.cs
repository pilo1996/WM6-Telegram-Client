using System;
using System.IO;
using System.Text;

namespace MtProto.WM6.TL
{
    public sealed class TLBinaryReader
    {
        private readonly MemoryStream _stream;
        public TLBinaryReader(byte[] data) { _stream = new MemoryStream(data); }
        public long Position { get { return _stream.Position; } }
        public long Length { get { return _stream.Length; } }
        public bool End { get { return _stream.Position >= _stream.Length; } }
        public int ReadInt() { return BitConverter.ToInt32(ReadRaw(4), 0); }
        public uint ReadUInt() { return BitConverter.ToUInt32(ReadRaw(4), 0); }
        public long ReadLong() { return BitConverter.ToInt64(ReadRaw(8), 0); }
        public ulong ReadULong() { return BitConverter.ToUInt64(ReadRaw(8), 0); }
        public byte[] ReadRaw(int count)
        {
            byte[] b = new byte[count];
            int n = _stream.Read(b, 0, count);
            if (n != count) throw new EndOfStreamException();
            return b;
        }
        public byte[] ReadBytes()
        {
            int len = _stream.ReadByte();
            if (len < 0) throw new EndOfStreamException();
            int header = 1;
            if (len == 254) { byte[] l = ReadRaw(3); len = l[0] | (l[1] << 8) | (l[2] << 16); header = 4; }
            byte[] data = ReadRaw(len);
            int pad = (4 - ((len + header) % 4)) % 4;
            if (pad > 0) ReadRaw(pad);
            return data;
        }
        public string ReadString()
        {
            byte[] data = ReadBytes();
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }
        public int[] ReadIntVector()
        {
            int c = ReadInt();
            if (c != TLTypes.Vector) throw new InvalidDataException("Expected vector constructor.");
            int count = ReadInt();
            int[] r = new int[count];
            for (int i = 0; i < count; i++) r[i] = ReadInt();
            return r;
        }
        public long[] ReadLongVector()
        {
            int c = ReadInt();
            if (c != TLTypes.Vector) throw new InvalidDataException("Expected vector constructor.");
            int count = ReadInt();
            long[] r = new long[count];
            for (int i = 0; i < count; i++) r[i] = ReadLong();
            return r;
        }
        public ulong[] ReadULongVector()
        {
            int c = ReadInt();
            if (c != TLTypes.Vector) throw new InvalidDataException("Expected vector constructor.");
            int count = ReadInt();
            ulong[] r = new ulong[count];
            for (int i = 0; i < count; i++) r[i] = ReadULong();
            return r;
        }
        public object[] ReadObjectVector()
        {
            int c = ReadInt();
            if (c != TLTypes.Vector) throw new InvalidDataException("Expected vector constructor.");
            int count = ReadInt();
            object[] r = new object[count];
            for (int i = 0; i < count; i++) r[i] = TLRegistry.Deserialize(this);
            return r;
        }
    }
}
