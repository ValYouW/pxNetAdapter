using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace pxNetAdapter.Response
{
	public abstract class Response : IResponse
	{
		public Response(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Qualifier = Utils.GetValue<string>(data, "qualifier", "");
			RequestId = Utils.GetValue<string>(data, "requestId", "");
			if (data.ContainsKey("error"))
				Error = new Error(data["error"] as IDictionary<string, object>);
		}

		public string Qualifier { get; private set; }
		public string RequestId { get; private set; }
		public IError Error { get; private set; }
	}

	public class Error : IError
	{
		public Error(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Code = data.ContainsKey("code") ? data["code"] as string : "";
			Message = data.ContainsKey("message") ? data["message"] as string : "";
		}

		public string Code { get; protected set; }
		public string Message { get; protected set; }
	}
}
