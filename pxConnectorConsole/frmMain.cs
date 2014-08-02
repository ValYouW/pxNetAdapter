using System;
using System.Drawing;
using System.Windows.Forms;
using pxNetAdapter;
using pxNetAdapter.Response;
using pxNetAdapter.Request;

namespace pxConnectorConsole
{
    public partial class frmMain : Form
    {
        #region Members

        private delegate void DlgSetStatus(string msg, Color? c);
        private delegate void DlgLogConsole(string msg);

        private Connector m_connector;

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
            SetStatus("Connecting...", Color.LightSalmon);
            m_connector.Connect(txtHost.Text, (int)txtPort.Value);
        }

        void m_connector_OnReconnect(object sender, int attempt)
        {
            SetStatus(string.Format("Connecting ({0})", attempt), Color.LightSalmon);
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

        void m_connector_OnMessage(object sender, IResponse response)
        {
			switch (response.Qualifier)
			{ 
				case "LoginResponse":
					LogConsole("LoginResponse, Token: " + ((LoginResponse)response).Data.Token);
					break;
				default:
					LogConsole("Got unknown response: " + response.Qualifier);
					break;
			}
		}

        private void btnSend_Click(object sender, EventArgs e)
        {
			IRequest req = new LoginRequest("udiyqa", "udiudi");
			req.RequestId = "4";
			try
            {
                m_connector.Send(req);
            }
            catch (Exception ex)
            {
                LogConsole("Error Sending: " + ex.Message);
            }
        }

        private void listConsole_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(listConsole.SelectedItem.ToString());
        }

        #endregion
    }
}
