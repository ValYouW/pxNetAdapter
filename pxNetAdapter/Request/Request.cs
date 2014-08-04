using System.Collections.Generic;

namespace pxNetAdapter.Request
{
	public class Request : IRequest
	{
		public string Qualifier { get; private set; }
		public string RequestId { get; set; }
		public IRequestData Data { get; set; }

		public Request(string qualifier)
		{
			Qualifier = qualifier;
			RequestId = "";
		}

		public virtual IDictionary<string, object> ToDictionary()
		{
			IDictionary<string, object> result = new Dictionary<string, object>();
			result["qualifier"] = Qualifier;
			result["requestId"] = RequestId;
			
			IDictionary<string, object> data = new Dictionary<string, object>();
			result["data"] = data;
			Data.FillData(data);

			return result;
		}
	}
}
