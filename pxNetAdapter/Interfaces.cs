using pxNetAdapter.Request;
using pxNetAdapter.Response;
using System;

namespace pxNetAdapter
{
    public interface IConnector
    {
        event EventHandler OnConnect;
        event EventHandler<GenericEventArgs<int>> OnReconnect;
        event EventHandler OnDisconnect;
		event EventHandler<GenericEventArgs<IResponse>> OnMessage;

		string SessionId { get; }

        void Connect(string host, int port);
        void Disconnect();
		void Send(IRequest request, Action<IResponse> onResponse);
    }
}

