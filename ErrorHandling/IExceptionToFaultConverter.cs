using System;

namespace opcode4.wcf.ErrorHandling
{
    public interface IExceptionToFaultConverter
    {
        object ConvertExceptionToFaultDetail(Exception error);
    }
}
