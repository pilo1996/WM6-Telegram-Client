using System.Collections;
using MtProto.WM6.TL;

namespace MtProto.WM6.Runtime
{
    public sealed class ContainerMessage
    {
        public long MsgId;
        public int SeqNo;
        public byte[] Body;
    }

    public sealed class MessageContainer
    {
        public ArrayList Messages = new ArrayList();

        public static MessageContainer Deserialize(TLBinaryReader r)
        {
            MessageContainer c = new MessageContainer();
            int count = r.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ContainerMessage m = new ContainerMessage();
                m.MsgId = r.ReadLong();
                m.SeqNo = r.ReadInt();
                int len = r.ReadInt();
                m.Body = r.ReadRaw(len);
                c.Messages.Add(m);
            }
            return c;
        }
    }
}
