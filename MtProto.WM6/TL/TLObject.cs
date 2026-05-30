namespace MtProto.WM6.TL
{
    public abstract class TLObject
    {
        public abstract int ConstructorId { get; }
        public abstract void Serialize(TLBinaryWriter writer);
    }
}
