using System;
using System.Drawing;
using System.Windows.Forms;
using pxNetAdapter;
using pxNetAdapter.Response;
using pxNetAdapter.Request;
using pxNetAdapter.Response.MarketData;
using pxNetAdapter.Response.User;
using System.Reflection;

namespace pxConnectorConsole
{
    public partial class frmMain : Form
    {
        #region Members

        private delegate void DlgSetStatus(string msg, Color? c);
        private delegate void DlgLogConsole(string msg);

        private IConnector m_connector;

	    private string m_token;
	    private string m_userGUID;

        #endregion

        public frmMain()
        {
            InitializeComponent();
        }

        #region Methods

        private void LogConsole(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new DlgLogConsole(LogConsole), msg);
                return;
            }

            listConsole.Items.Insert(0, msg);
        }

        private void SetStatus(string msg, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new DlgSetStatus(SetStatus), msg, color);
                return;
            }

            lblStatus.Text = msg;
            if (color != null)
                lblStatus.BackColor = color.Value;
        }

	    private void OnLogin(IResponse loginResponse)
	    {
			if (loginResponse.Error != null && !string.IsNullOrEmpty(loginResponse.Error.Code))
		    {
				LogConsole("Error login: " + loginResponse.Error.Code + ", " + loginResponse.Error.Message);
				return;
		    }

			LoginResponseData data = (LoginResponseData)loginResponse.Data;
		    LogConsole("Token: " + data.Token);
		    m_token = data.Token;
		    m_userGUID = data.UserInfo.GUID;

		    // Get Quotes
		    //IRequest req = MarketData.SubscribeForQuotes(m_token, m_userGUID);
			IRequest req = TradingApp.GetInitialAppData(m_token, m_userGUID);
			Send(req, response =>
				{
					if (response.Error != null && !string.IsNullOrEmpty(response.Error.Code))
					{
						LogConsole("Error: " + response.Error.Code + ", " + response.Error.Message);
					}
					else
					{
						LogConsole("Success");
					}
				});
	    }

	    private void Send(IRequest request, Action<IResponse> cb = null)
	    {
			try
			{
				LogConsole("Sending " + request.Qualifier);
				m_connector.Send(request, cb);
			}
			catch (Exception ex)
			{
				LogConsole("Error Sending: " + ex.Message);
			}
	    }

	    #endregion

        #region EventHandlers

        private void frmMain_Load(object sender, EventArgs e)
        {
            m_connector = new Connector();
            m_connector.OnConnect += m_connector_OnConnect;
            m_connector.OnReconnect += m_connector_OnReconnect;
            m_connector.OnDisconnect += m_connector_OnDisconnect;
            m_connector.OnMessage += m_connector_OnMessage;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
			//object o = Assembly.LoadFrom("C:\\Program Files\\NinjaTrader 7\\bin\\NinjaTrader.Hosted.dll").CreateInstance("NinjaTrader.Hosted.Loader");
            SetStatus("Connecting...", Color.LightSalmon);
            m_connector.Connect(txtHost.Text, (int)txtPort.Value);
        }

        void m_connector_OnReconnect(object sender, GenericEventArgs<int> args)
        {
            SetStatus(string.Format("Connecting ({0})", args.Args), Color.LightSalmon);
        }

        void m_connector_OnDisconnect(object sender, EventArgs e)
        {
            SetStatus("Disconnected", Color.LightCoral);
        }

        void m_connector_OnConnect(object sender, EventArgs e)
        {
            SetStatus("Connected", Color.LightGreen);
            LogConsole(m_connector.SessionId);
        }

        void m_connector_OnMessage(object sender, GenericEventArgs<IResponse> args)
        {
			IResponse response = args.Args;
			switch (response.Qualifier)
			{ 
				case ResponseTypeEnum.LoginResponse:
					LogConsole("LoginResponse, Token: " + ((LoginResponseData)response.Data).Token);
					break;
				case ResponseTypeEnum.QuoteUpdateResponse:
					LogConsole("Got " + ((QuoteUpdateResponseData)response.Data).Quotes.Keys.Count + " quotes");
					break;
				default:
					LogConsole("Got unknown response: " + response.Qualifier);
					break;
			}
		}

        private void btnSend_Click(object sender, EventArgs e)
        {
			IRequest req = User.Login("w123@ufx.com", "123456");
			Send(req, OnLogin);
        }

        private void listConsole_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(listConsole.SelectedItem.ToString());
        }

        #endregion
    }
}
