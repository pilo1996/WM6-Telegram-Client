namespace MtProto.WM6.Transport
{
    public interface IMtProtoTransport
    {
        void Connect(string host, int port);
        void Send(byte[] payload);
        byte[] Receive();
        void Close();
    }
}
