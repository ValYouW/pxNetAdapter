using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Model.Trading
{
	public enum StatusEnum
	{
		None,
		Open,
		Closed,
		Pending,
		Canceled
	}

	public enum SideEnum
	{
		None,
		Buy,
		Sell
	}

	public enum TypeEnum
	{
		None,
		Market,
		Limit
	}
}
