using System;
using System.Collections;
using System.IO;

namespace MtProto.WM6.TL
{
    public delegate object TLDeserializer(TLBinaryReader reader);

    public sealed class TLGenericObject
    {
        public int ConstructorId;
        public ArrayList Fields = new ArrayList();
        public string Name;
    }

    public static class TLRegistry
    {
        private static readonly Hashtable _readers = new Hashtable();
        private static readonly Hashtable _names = new Hashtable();

        public static void Register(int constructorId, string name, TLDeserializer reader)
        {
            _readers[constructorId] = reader;
            _names[constructorId] = name;
        }

        public static object Deserialize(byte[] data)
        {
            return Deserialize(new TLBinaryReader(data));
        }

        public static object Deserialize(TLBinaryReader reader)
        {
            int constructorId = reader.ReadInt();
            return DeserializeKnown(reader, constructorId);
        }

        public static object DeserializeKnown(TLBinaryReader reader, int constructorId)
        {
            TLDeserializer d = (TLDeserializer)_readers[constructorId];
            if (d != null) return d(reader);
            TLGenericObject o = new TLGenericObject();
            o.ConstructorId = constructorId;
            o.Name = (string)_names[constructorId];
            while (!reader.End) o.Fields.Add(reader.ReadInt());
            return o;
        }

        public static string GetName(int constructorId)
        {
            string name = (string)_names[constructorId];
            return name == null ? "0x" + constructorId.ToString("x8") : name;
        }
    }
}
