using pxNetAdapter.Model.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Response.Data
{
	public class PositionResponseData : IResponseData
	{
		public PositionResponseData(IDictionary<string, object> data)
		{
			if (data == null || !data.ContainsKey("position"))
				return;

			Position = new Position(data["position"] as IDictionary<string, object>);
		}

		public Position Position { get; private set; }
	}
}
