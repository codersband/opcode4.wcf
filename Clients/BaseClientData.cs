using System.ServiceModel.Channels;

namespace opcode4.wcf.Clients
{
    public abstract class BaseClientData
    {
        protected static readonly object Locker = new object();
        protected static Binding Binding = null;
        protected static string Address;
    }
}
