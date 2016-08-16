using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using opcode4.utilities;
using opcode4.wcf.Security;

namespace opcode4.wcf.Clients
{
    public class BaseWcfClient<TClient> : BaseClientData
    {
        private static ChannelFactory<TClient> _factory;

        public BaseWcfClient()
        {
            Address = ConfigUtils.ReadString(typeof(TClient).Name);
            if (Binding != null)
                return;

            switch (ConfigUtils.ReadStringDef("WCF.BINDING.TYPE", "http").ToLower())
            {
                case "ws":
                    Binding = BindHelper.WsHttp;
                    break;
                case "custom":
                    Binding = BindHelper.CustomHttp;
                    break;
                default:
                    Binding = BindHelper.BasicHttp;
                    break;
            }
        }

        private TClient CreateChannel()
        {
            if (_factory == null)
            {
                lock (Locker)
                {
                    _factory = new ChannelFactory<TClient>(Binding, new EndpointAddress(Address));
                    _factory.Endpoint.Behaviors.Add(new IdentityEndpointBehavior());
                }
            }

            return _factory.CreateChannel();
        }

        public void Send(Action<TClient> code)
        {
            var channel = CreateChannel();

            try
            {
                code(channel);
                ((IChannel)channel).Close();
            }
            catch (Exception)
            {
                ((IChannel)channel).Abort();
                throw;
            }
        }

        public TReturn Get<TReturn>(Func<TClient, TReturn> code)
        {
            var channel = CreateChannel();
            try
            {
                var result = code(channel);
                ((IChannel)channel).Close();
                return result;

            }
            catch (Exception)
            {
                ((IChannel)channel).Abort();
                throw;
            }
        }
    }
}
