using System.Runtime.Serialization;

namespace opcode4.wcf.Results
{
    [DataContract]
    public class ServiceFaultResult : SvcResult
    {
        [DataMember]
        public int Code { set; get; }

        [DataMember]
        public string CodeName { set; get; }

        [DataMember]
        public string Message { set; get; }
    }
}
