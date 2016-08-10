using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Xml;
using opcode.log;
using opcode4.core.Helpers;
using opcode4.core.Model.Identity;
using opcode4.core.Model.Log;
using opcode4.utilities;
using opcode4.wcf.Interfaces;

namespace opcode4.wcf.Clients
{
    public class LogSvcClient : BaseClientData
    {
        private static ChannelFactory<ILogService> _factory;
        private readonly string _serverId = ConfigUtils.ServerID;

        public LogSvcClient()
        {
            Address = ConfigUtils.ReadString("ILogService");
            if (Binding != null)
                return;

            Binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
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


        private ILogService CreateChannel()
        {
            if (_factory == null)
            {
                lock (Locker)
                {
                    _factory = new ChannelFactory<ILogService>(Binding, new EndpointAddress(Address));
                }
            }

            return _factory.CreateChannel();
        }

        public void Error(string message, params object[] args)
        {

            var logEntity = new LogEntity
            {
                EventType = LogEventType.Error,
                Message = args == null ? message : string.Format(message, args)
            };

            Send(logEntity);
        }

        public void Info(string message, params object[] args)
        {
            var logEntity = new LogEntity
            {
                EventType = LogEventType.Info,
                Message = args == null ? message : string.Format(message, args)
            };

            Send(logEntity);
        }

        public void CriticalError(string message, params object[] args)
        {
            var logEntity = new LogEntity
            {
                EventType = LogEventType.CriticalError,
                Message = args == null ? message : string.Format(message, args)
            };

            Send(logEntity);
        }

        public void Debug(string message, params object[] args)
        {
            var logEntity = new LogEntity
            {
                EventType = LogEventType.Debug,
                Message = args == null ? message : string.Format(message, args)
            };

            Send(logEntity);
        }

        public void Warning(string message, params object[] args)
        {
            var logEntity = new LogEntity
            {
                EventType = LogEventType.Warning,
                Message = args == null ? message : string.Format(message, args)
            };

            Send(logEntity);
        }

        public void Send(LogEntity logEntity)
        {
            if (!Thread.CurrentPrincipal.ShouldLog(logEntity.EventType))
                return;

            var channel = CreateChannel();
            try
            {
                var identity = Thread.CurrentPrincipal.Identity as CustomIdentity ?? new CustomIdentity();
                logEntity.ActorId = identity.Id;
                logEntity.ActorName = identity.Name;

                if (string.IsNullOrEmpty(logEntity.FormalMessage))
                    logEntity.FormalMessage = "WCFClient.FE";

                logEntity.ServerID = _serverId;
                logEntity.EventDate = DateTime.Now;

                channel.AddEvent(logEntity);
                ((IChannel)channel).Close();
            }
            catch (Exception e)
            {
                ((IChannel)channel).Abort();
                TextLogger.Critical(e.Message);
            }
        }

    }
}
