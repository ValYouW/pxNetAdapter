using System;

namespace pxNetAdapter
{
    public interface IConnector
    {
        event EventHandler OnConnect;
        event EventHandler<int> OnReconnect;
        event EventHandler OnDisconnect;
        event EventHandler<string> OnMessage;

        void Connect(string host, int port);
        void Disconnect();
        void Send(string request);
    }
}

