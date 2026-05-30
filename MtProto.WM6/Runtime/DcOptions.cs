namespace MtProto.WM6.Runtime
{
    public sealed class DcEndpoint
    {
        public int DcId;
        public string Host;
        public int Port;
        public DcEndpoint(int dcId, string host, int port) { DcId = dcId; Host = host; Port = port; }
    }

    public static class DcOptions
    {
        public static DcEndpoint Production(int dcId)
        {
            switch (dcId)
            {
                case 1: return new DcEndpoint(1, "149.154.175.50", 443);
                case 2: return new DcEndpoint(2, "149.154.167.51", 443);
                case 3: return new DcEndpoint(3, "149.154.175.100", 443);
                case 4: return new DcEndpoint(4, "149.154.167.91", 443);
                case 5: return new DcEndpoint(5, "91.108.56.130", 443);
                default: return new DcEndpoint(dcId, "149.154.175.50", 443);
            }
        }

        public static DcEndpoint Test(int dcId)
        {
            switch (dcId)
            {
                case 1: return new DcEndpoint(1, "149.154.175.10", 443);
                case 2: return new DcEndpoint(2, "149.154.167.40", 443);
                case 3: return new DcEndpoint(3, "149.154.175.117", 443);
                default: return new DcEndpoint(dcId, "149.154.175.10", 443);
            }
        }
    }
}
