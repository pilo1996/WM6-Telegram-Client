using System;
using System.Reflection;

namespace MtProto.WM6.ClientApp
{
    public static class ResultText
    {
        public static string ToShort(object x)
        {
            if (x == null) return "null";
            if (x is byte[]) return "byte[" + ((byte[])x).Length + "]";
            return x.GetType().FullName;
        }
        public static string ExtractPhoneCodeHash(object x)
        {
            if (x == null) return "";
            try
            {
                FieldInfo f = x.GetType().GetField("PhoneCodeHash");
                if (f != null && f.GetValue(x) != null) return f.GetValue(x).ToString();
                PropertyInfo p = x.GetType().GetProperty("PhoneCodeHash");
                if (p != null && p.GetValue(x, null) != null) return p.GetValue(x, null).ToString();
            }
            catch { }
            return "";
        }
    }
}
