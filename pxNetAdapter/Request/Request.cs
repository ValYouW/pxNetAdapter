using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace pxNetAdapter.Request
{
	public abstract class Request : IRequest
	{
		public string Qualifier { get; private set; }
		public string RequestId { get; set; }

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
			return result;
		}
	}
}
