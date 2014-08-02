using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pxNetAdapter.Response
{
    public interface IResponse
    {
        string Qualifier { get; }
        string RequestId { get; }
    }

    public interface IError
    {
        string Code { get; }
        string Message { get; }
    }

    public interface IResponseData
    { 
    }
}
