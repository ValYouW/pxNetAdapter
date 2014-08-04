using pxNetAdapter.Request;
using pxNetAdapter.Response;
using System;

namespace pxNetAdapter
{
    public interface IConnector
    {
        event EventHandler OnConnect;
        event EventHandler<int> OnReconnect;
        event EventHandler OnDisconnect;
		event EventHandler<IResponse> OnMessage;

		string SessionId { get; }

        void Connect(string host, int port);
        void Disconnect();
		void Send(IRequest request, Action<IResponse> onResponse);
    }
}

