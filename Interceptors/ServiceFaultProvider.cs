using System;
using System.ServiceModel;
using opcode.log;
using opcode4.core.Exceptions;
using opcode4.core.Model.Log;
using opcode4.wcf.ErrorHandling;
using opcode4.wcf.Results;

namespace opcode4.wcf.Interceptors
{
    public class ServiceFaultProvider : IExceptionToFaultConverter
    {
        #region IExceptionToFaultConverter Members

        public object ConvertExceptionToFaultDetail(Exception error)
        {
            var operationContext = OperationContext.Current;
            var actionName = operationContext.IncomingMessageHeaders.Action;
            var serviceName = operationContext.Host.Description.Name;

            //DispatchOperation operation = operationContext.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o =>
            //        o.Action == action);
            // Insert your own error-handling here if (operation == null)
            //Type hostType = operationContext.Host.Description.ServiceType;
            //MethodInfo method = hostType.GetMethod(operation.Name);
            //Type hostType = operationContext.Host.Description.ServiceType;

            var metaInfo = $"{serviceName}.{actionName}";
            var isLogged = error.Data.Contains(ErrorLoggingBehaviorAttribute.IsLoggedKey);
            
            var exception = error as CustomException;
            if (exception != null)
            {
                if (!isLogged)
                    DBLogger.Error("[{0}] Message: {1}", metaInfo, exception.Message);
                return new ServiceFaultResult
                    {
                        Code = (int)exception.Code,
                        Message = exception.Message,
                        CodeName = $"{exception.Code}"
                    };
            }

            var msg = $"[{metaInfo}] Message: {error.Message}\r\n{error.StackTrace}\r\n{error.InnerException}";
            if (!isLogged)
            {
                var logEntity = new LogEntity
                    {
                        EventType = LogEventType.Error,
                        Message = msg,
                        FormalMessage = "[SYSTEMTYPE]"
                    };
                DBLogger.AddEvent(logEntity);
            }

            return new ServiceFaultResult
            {
                Code = (int)ExceptionCode.INTERNAL_ERROR,
                Message = msg,
                CodeName = ExceptionCode.INTERNAL_ERROR.ToString()
            };
        }

        #endregion
    }
}
