using System;
using System.IO;
using System.Text;

namespace MtProto.WM6.TL
{
    public sealed class TLBinaryWriter
    {
        private readonly MemoryStream _stream = new MemoryStream();
        public long Length { get { return _stream.Length; } }
        public void WriteInt(int value) { WriteRaw(BitConverter.GetBytes(value)); }
        public void WriteUInt(uint value) { WriteRaw(BitConverter.GetBytes(value)); }
        public void WriteLong(long value) { WriteRaw(BitConverter.GetBytes(value)); }
        public void WriteULong(ulong value) { WriteRaw(BitConverter.GetBytes(value)); }
        public void WriteRaw(byte[] data) { if (data != null) _stream.Write(data, 0, data.Length); }
        public void WriteBytes(byte[] data)
        {
            if (data == null) data = new byte[0];
            int len = data.Length;
            if (len < 254) _stream.WriteByte((byte)len);
            else { _stream.WriteByte(254); _stream.WriteByte((byte)len); _stream.WriteByte((byte)(len >> 8)); _stream.WriteByte((byte)(len >> 16)); }
            _stream.Write(data, 0, data.Length);
            while (_stream.Length % 4 != 0) _stream.WriteByte(0);
        }
        public void WriteString(string value) { WriteBytes(Encoding.UTF8.GetBytes(value == null ? "" : value)); }
        public void WriteObject(TLObject obj) { WriteInt(obj.ConstructorId); obj.Serialize(this); }
        public void WriteIntVector(int[] values)
        {
            WriteInt(TLTypes.Vector); WriteInt(values == null ? 0 : values.Length);
            if (values != null) for (int i = 0; i < values.Length; i++) WriteInt(values[i]);
        }
        public void WriteLongVector(long[] values)
        {
            WriteInt(TLTypes.Vector); WriteInt(values == null ? 0 : values.Length);
            if (values != null) for (int i = 0; i < values.Length; i++) WriteLong(values[i]);
        }
        public void WriteVectorLong(long[] values)
        {
            WriteLongVector(values);
        }
        public void WriteVectorULong(ulong[] values)
        {
            WriteInt(TLTypes.Vector); WriteInt(values == null ? 0 : values.Length);
            if (values != null) for (int i = 0; i < values.Length; i++) WriteULong(values[i]);
        }
        public void WriteObjectVector(TLObject[] values)
        {
            WriteInt(TLTypes.Vector); WriteInt(values == null ? 0 : values.Length);
            if (values != null) for (int i = 0; i < values.Length; i++) WriteObject(values[i]);
        }
        public byte[] ToArray() { return _stream.ToArray(); }
    }
}
