using System;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using opcode4.core;
using opcode4.core.Helpers;
using opcode4.core.Model.Log;
using opcode4.utilities;

namespace opcode4.wcf.Interceptors
{
    public class LogOperationAttribute : Attribute, IParameterInspector, IOperationBehavior
    {
        private string _input = "";
        private Stopwatch _stopwatch;
        private string _elapsed = "";

        public object BeforeCall(string operationName, object[] inputs)
        {
            if (ConfigUtils.IsDebugMode || Thread.CurrentPrincipal.ShouldLog(LogEventType.Debug))
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();

                if (inputs != null && inputs.Length > 0)
                    _input = inputs.ToJSON();
            }
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            if (ConfigUtils.IsDebugMode || Thread.CurrentPrincipal.ShouldLog(LogEventType.Debug))
            {
                if (_stopwatch != null)
                {
                    _stopwatch.Stop();
                    _elapsed = _stopwatch.Elapsed.ToString(@"\:m\:s\:fff");
                }

                var message = "\r\nRequest: \t" + _input;

                if (returnValue != null)
                    message += "\r\nResponse: \t" + returnValue.ToJSON();

                var logEntity = new LogEntity
                {
                    ActorName = Thread.CurrentPrincipal.Identity.Name,
                    ActorId = TActorUtils.ActorId,
                    EventDate = DateTime.Now,
                    ServerID = ConfigUtils.ServerID,
                    EventType = LogEventType.Debug,
                    FormalMessage = "CALLINTERCEPT",
                    Message = $"[SERVICE.{operationName}] \t{_elapsed} {message}"
                };

                IoCFactory.DAO.Using(repository => repository.Set(logEntity));
            }
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}
