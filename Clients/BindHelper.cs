using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace opcode4.wcf.Clients
{
    public static class BindHelper
    {
        public static BasicHttpBinding BasicHttp
        {
            get
            {
                return new BasicHttpBinding(BasicHttpSecurityMode.None)
                {
                    CloseTimeout = TimeSpan.FromMinutes(1),
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(5),
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    MaxBufferPoolSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    MessageEncoding = WSMessageEncoding.Text,
                    TextEncoding = Encoding.UTF8,
                    UseDefaultWebProxy = false,
                    AllowCookies = false,
                    ReaderQuotas = new XmlDictionaryReaderQuotas
                    {
                        MaxDepth = 32,
                        MaxStringContentLength = 2147483647,
                        MaxArrayLength = 2147483647,
                        MaxBytesPerRead = 4096,
                        MaxNameTableCharCount = 2147483647
                    }
                };
            }
        }

        public static WSHttpBinding WsHttp
        {
            get
            {
                return new WSHttpBinding(SecurityMode.None, false)
                           {
                               CloseTimeout = TimeSpan.FromMinutes(1),
                               OpenTimeout = TimeSpan.FromMinutes(1),
                               ReceiveTimeout = TimeSpan.FromMinutes(10),
                               SendTimeout = TimeSpan.FromMinutes(5),
                               BypassProxyOnLocal = false,
                               TransactionFlow = false,
                               HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                               MaxBufferPoolSize = 2147483647,
                               MaxReceivedMessageSize = 2147483647,
                               MessageEncoding = WSMessageEncoding.Text,
                               TextEncoding = Encoding.UTF8,
                               UseDefaultWebProxy = false,
                               AllowCookies = false,
                               ReaderQuotas = new XmlDictionaryReaderQuotas
                                   {
                                       MaxDepth = 32,
                                       MaxStringContentLength = 2147483647,
                                       MaxArrayLength = 2147483647,
                                       MaxBytesPerRead = 4096,
                                       MaxNameTableCharCount = 2147483647
                                   },
                           };
            }
        }

        public static CustomBinding CustomHttp
        {
            get
            {
                var binding = new CustomBinding
                {
                    CloseTimeout = TimeSpan.FromMinutes(1),
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(5)
                };

                binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8));
                binding.Elements.Add(new HttpTransportBindingElement
                {
                    KeepAliveEnabled = false,
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    MaxBufferPoolSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    UseDefaultWebProxy = false,
                    AllowCookies = false,
                });

                return binding;
            }
        }

    }
}
