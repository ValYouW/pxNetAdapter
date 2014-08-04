using System.Collections.Generic;

namespace pxNetAdapter.Request
{
	public interface IRequest
	{
		string Qualifier { get; }
		string RequestId { get; set; }
		IRequestData Data { get; set; }
	}

	public interface IRequestData
	{
		void FillData(IDictionary<string, object> data);
	}
}
