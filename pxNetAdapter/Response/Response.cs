using System;
using System.Collections.Generic;

namespace pxNetAdapter.Response
{
	public class Response : IResponse
	{
		private ResponseTypeEnum m_qualifier;
		public Response(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			if (!Enum.TryParse(Utils.GetValue(data, "qualifier", ""), true, out m_qualifier))
				m_qualifier = ResponseTypeEnum.None;

			RequestId = Utils.GetValue(data, "requestId", "");
			Error = null;
			if (data.ContainsKey("error"))
				Error = new Error(data["error"] as IDictionary<string, object>);
		}

		public ResponseTypeEnum Qualifier { get { return m_qualifier; } }
		public string RequestId { get; private set; }
		public IError Error { get; private set; }
		public IResponseData Data { get; set; }
	}

	public class Error : IError
	{
		public Error(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Code = data.ContainsKey("code") ? data["code"].ToString() : "";
			Message = data.ContainsKey("message") ? data["message"].ToString() : "";
		}

		public string Code { get; private set; }
		public string Message { get; private set; }
	}
}
