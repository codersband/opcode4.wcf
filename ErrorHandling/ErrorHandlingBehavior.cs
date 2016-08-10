using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace opcode4.wcf.ErrorHandling
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorHandlingBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type _exceptionToFaultConverterType;

        public bool EnforceFaultContract { get; set; }
        public Type ExceptionToFaultConverter
        {
            get
            {
                return _exceptionToFaultConverterType;
            }
            set
            {
                if (!typeof(IExceptionToFaultConverter).IsAssignableFrom(value))
                    throw new ArgumentException("Fault converter doesn't implement IExceptionToFaultConverter.", "value");
                _exceptionToFaultConverterType = value;
            }
        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase chanDispBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = chanDispBase as ChannelDispatcher;
                channelDispatcher?.ErrorHandlers.Add(new ErrorHandler(this));
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}
