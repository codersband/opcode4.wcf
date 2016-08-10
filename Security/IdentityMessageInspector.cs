using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;
using opcode4.core.Model.Identity;

namespace opcode4.wcf.Security
{
    class IdentityMessageInspector : IDispatchMessageInspector, IClientMessageInspector 
    {
        #region IDispatchMessageInspector
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            CustomIdentity identity = null;

            if (OperationContext.Current.IncomingMessageHeaders.FindHeader("Identity", "http://codersband.com/actor_identity") != -1)
                identity = OperationContext.Current.IncomingMessageHeaders.GetHeader<CustomIdentity>("Identity", "http://codersband.com/actor_identity");

            new CustomPrincipal(identity ?? new CustomIdentity());

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        { return; }

        #endregion

        #region IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var identity = Thread.CurrentPrincipal.Identity as CustomIdentity;

            if (identity != null)
            {
                var typedHeader = new MessageHeader<CustomIdentity>(identity);
                var untypedHeader = typedHeader.GetUntypedHeader("Identity", "http://codersband.com/actor_identity");
         
                request.Headers.Add(untypedHeader);
            }
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        { return; }

        #endregion

    }
}
