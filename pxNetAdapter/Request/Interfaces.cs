using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pxNetAdapter.Request
{
	public interface IRequest
	{
		string Qualifier { get; }
		string RequestId { get; set; }
	}
}
