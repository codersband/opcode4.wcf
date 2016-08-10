using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using opcode.log;
using opcode4.core.Helpers;
using opcode4.core.Model.Log;
using opcode4.utilities;

namespace opcode4.wcf.ErrorHandling
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorLoggingBehaviorAttribute : Attribute, IServiceBehavior
    {
        public const string IsLoggedKey = "IsLoggedByErrorLoggingBehavior";
        public bool LogActorActions { set; get; }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IOperationBehavior behavior = new LoggingOperationBehavior(LogActorActions);
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                foreach (OperationDescription operation in endpoint.Contract.Operations)
                {
                    if (!operation.Behaviors.Any(d => d is LoggingOperationBehavior))
                    {
                        operation.Behaviors.Add(behavior);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }
    }

    public class LoggingOperationBehavior : IOperationBehavior
    {
        public bool LogActorActions { set; get; }

        public LoggingOperationBehavior(bool logActorActions = false)
        {
            LogActorActions = logActorActions;
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new LoggingOperationInvoker(dispatchOperation.Invoker, dispatchOperation, LogActorActions);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }
    }


    public class LoggingOperationInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _baseInvoker;
        private readonly string _operationName;
        private readonly string _controllerName;

        public bool LogActorActions { set; get; }

        public LoggingOperationInvoker(IOperationInvoker baseInvoker, DispatchOperation operation, bool logActorAction = false)
        {
            _baseInvoker = baseInvoker;
            _operationName = operation.Name;
            _controllerName = operation.Parent.Type == null ? "[WCFService]" : operation.Parent.Type.FullName;
            LogActorActions = logActorAction;
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = null;
            try
            {
                if(!LogActorActions)
                    return _baseInvoker.Invoke(instance, inputs, out outputs);

                var invoker = _baseInvoker.Invoke(instance, inputs, out outputs);
                if (ConfigUtils.IsDebugMode && Thread.CurrentPrincipal.ShouldLog(LogEventType.Debug))
                    DBLogger.Debug(BuildReportMessage(inputs, outputs, invoker));
                
                return invoker;

            }
            catch (Exception ex)
            {
                var info = "\r\nIN: " + (inputs.Length > 0 ? inputs.ToJSON(): string.Empty);
                if (outputs != null && outputs.Length > 0)
                    info += "\r\nOUT: " + outputs.ToJSON();

                var dbgmsg = ConfigUtils.IsDebugMode ? "\r\n" + ex.StackTrace : "";
                DBLogger.Error("[{0}.{1}] {2}{3}:{4}", _controllerName, _operationName, ex.Message, dbgmsg, info);
                ex.Data[ErrorLoggingBehaviorAttribute.IsLoggedKey] = true;
                throw;
            }
        }

        private string BuildReportMessage(object[] inputs, object[] outputs, object invoker)
        {
            var info = $"\r\nIN: {(inputs.Length > 0 ? inputs.ToJSON() : "none")}\r\nOUT: " +
                       $"{(outputs != null && outputs.Length > 0 ? outputs.ToJSON() : invoker != null? invoker.ToJSON(): "none")}";
            
            return $"[{_controllerName}.{_operationName}] {info}";
        }

        public object[] AllocateInputs() { return _baseInvoker.AllocateInputs(); }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            //_myLog.Log("Method " + _operationName + " of class " + _controllerName + " called");
            return _baseInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result) { return _baseInvoker.InvokeEnd(instance, out outputs, result); }

        public bool IsSynchronous { get { return _baseInvoker.IsSynchronous; } }
    }
}
