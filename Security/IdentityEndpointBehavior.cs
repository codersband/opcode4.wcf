using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace opcode4.wcf.Security
{
    [AttributeUsage(AttributeTargets.All)]
    public class IdentityEndpointBehavior : Attribute, IEndpointBehavior, IServiceBehavior 
    {
        #region IEndpointBehavior

        public void Validate(ServiceEndpoint endpoint)
        { return; }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { return; }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher == null) return;
            foreach (var ed in channelDispatcher.Endpoints)
            {
                var inspector = new IdentityMessageInspector();
                ed.DispatchRuntime.MessageInspectors.Add(inspector);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector = new IdentityMessageInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }
        #endregion

        #region IServiceBehaviour

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { return; }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        { return; }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var eDispatcher in cDispatcher.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.Add(new IdentityMessageInspector());
                }
            }
        }

        #endregion

    }
}
