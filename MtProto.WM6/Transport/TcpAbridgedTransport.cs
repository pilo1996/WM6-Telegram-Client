using System;
using System.IO;
using System.Net.Sockets;

namespace MtProto.WM6.Transport
{
    public sealed class TcpAbridgedTransport : IMtProtoTransport
    {
        private TcpClient _client;
        private NetworkStream _stream;
        public void Connect(string host, int port)
        {
            _client = new TcpClient();
            _client.Connect(host, port);
            _stream = _client.GetStream();
            _stream.WriteByte(0xef);
        }
        public void Send(byte[] payload)
        {
            int len = payload.Length / 4;
            if (len < 127) _stream.WriteByte((byte)len);
            else { _stream.WriteByte(127); _stream.WriteByte((byte)len); _stream.WriteByte((byte)(len >> 8)); _stream.WriteByte((byte)(len >> 16)); }
            _stream.Write(payload, 0, payload.Length);
        }
        public byte[] Receive()
        {
            int first = _stream.ReadByte();
            if (first < 0) throw new IOException("Connection closed");
            int len = first;
            if (len == 127) len = _stream.ReadByte() | (_stream.ReadByte() << 8) | (_stream.ReadByte() << 16);
            int size = len * 4;
            byte[] buffer = new byte[size];
            int read = 0;
            while (read < size)
            {
                int n = _stream.Read(buffer, read, size - read);
                if (n <= 0) throw new IOException("Connection closed");
                read += n;
            }
            return buffer;
        }
        public void Close()
        {
            if (_stream != null) _stream.Close();
            if (_client != null) _client.Close();
        }
    }
}
