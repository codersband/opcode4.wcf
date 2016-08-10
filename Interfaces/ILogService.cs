using System;
using System.Collections.Generic;
using System.ServiceModel;
using opcode4.core.Model.Log;

namespace opcode4.wcf.Interfaces
{
    [ServiceContract(Namespace = "Commons")]
    //[ServiceKnownType(typeof(LogEventsByActorNameCriteria))]
    public interface ILogService
    {
        [OperationContract(IsOneWay = true)]
        void AddEvent(LogEntity entity);

        [OperationContract(IsOneWay = true)]
        void AddEvents(List<LogEntity> entities);

        [OperationContract]
        List<LogEntity> GetEvents();

        [OperationContract]
        List<LogEntity> GetEventsByActorId(ulong actorId, DateTime from, DateTime? till, LogEventType? type);

        [OperationContract]
        LogDetailEntity GetDetails(ulong id);
    }
}
