using NinjaTrader.Adapter;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using pxNetAdapter;
using pxNetAdapter.Request;
using pxNetAdapter.Response;
using pxNetAdapter.Model.Assets;
using pxNetAdapter.Model.MarketData;
using pxNetAdapter.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using pxNetAdapter.Response.Data;

namespace NinjaTrader.Hosted
{
    public class PXAdapter : IAdapter, IMarketData, NinjaTrader.Adapter.IOrder
    {
        private Connection m_NTconnection;
        private Connector m_pxConnector;
        private ConnectionStateEnum m_state;
		private bool m_firstConnect = true;
		
		// Key is exchange code
		private IDictionary<string, Asset> m_supportesAssets;
		private IDictionary<string, Data.MarketData> m_subscriptions;

		// Key is symbol value is exchange code
		private IDictionary<string, string> m_symbolExchangeCodeMap;
		IDictionary<string, Quote> m_lastQuotes;

		private IDictionary<string, string> m_ntOrderPxOrder;

		// This will hold only positions that were opened from this connection, this is good for the POC only.
		IList<pxNetAdapter.Model.Trading.Position> m_positions;

		private UserInfo m_userInfo;
        private string m_token;

        public PXAdapter(Connection connection)
        {
            m_state = ConnectionStateEnum.Disconnected;
            m_NTconnection = connection;

            m_pxConnector = new Connector();
			m_supportesAssets = new Dictionary<string, Asset>();
			m_subscriptions = new Dictionary<string, Data.MarketData>();
			m_symbolExchangeCodeMap = new Dictionary<string, string>();
			m_lastQuotes = new Dictionary<string, Quote>();
			m_positions = new List<pxNetAdapter.Model.Trading.Position>();
			m_ntOrderPxOrder = new Dictionary<string, string>();

            m_pxConnector.OnConnect += m_pxConnector_OnConnect;
            m_pxConnector.OnDisconnect += m_pxConnector_OnDisconnect;
            m_pxConnector.OnReconnect += m_pxConnector_OnReconnect;
			m_pxConnector.OnMessage += m_pxConnector_OnMessage;
        }

        #region IAdapter

        public void Connect()
        {
			if (m_state != ConnectionStateEnum.Disconnected)
			{
				LogEventArgs.ProcessEventArgs(new LogEventArgs("Connect was called but PXAdapter state is " + m_state, NinjaTrader.Cbi.LogLevel.Warning));
				return;
			}

			m_pxConnector.Connect("dev-site01.toyga.local", 10300);
        }

        public void Disconnect()
        {
			m_pxConnector.Disconnect();
        }

        public void InstrumentLookup(Instrument instrumentTemplate)
        {
        }

        #endregion

		#region IMarketData

		public void Subscribe(Data.MarketData marketData)
		{
			Asset asset = GetPxAsset(marketData.Instrument);
			if (asset == null)
			{
				LogEventArgs.ProcessEventArgs(new LogEventArgs(string.Format("{0}: Failed to subscribe to instrument '{1}': empty symbol map", m_NTconnection.Options.Name, marketData.Instrument.FullName), NinjaTrader.Cbi.LogLevel.Error, LogCategory.Connection));
				return;
			}

			if (m_subscriptions.ContainsKey(asset.ExchangeCode))
				return;

			m_subscriptions.Add(asset.ExchangeCode, marketData);

			// If this is the first time subscribe for quotes
			if (m_subscriptions.Count == 1)
			{
				m_pxConnector.Send(pxNetAdapter.Request.MarketData.SubscribeForQuotes(m_token, m_userInfo.GUID), res => {
					if (res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
					{
						LogEventArgs.ProcessEventArgs(new LogEventArgs("Unable to subscribe to L1 data for instrument '" + marketData.Instrument.FullName + "': " + res.Error.Message, NinjaTrader.Cbi.LogLevel.Error));
					}
				});
			}
		}

		public void Unsubscribe(Data.MarketData marketData)
		{
			string symbol = marketData.Instrument.MasterInstrument.GetProviderName(NinjaTrader.Cbi.Provider.Provider10).TrimStart(new char[] { '@' });
			if (m_subscriptions.ContainsKey(symbol))
			{
				m_subscriptions.Remove(symbol);
			}
		}

		#endregion

		#region IOrder

		public void Cancel(Order order)
		{
			string orderGUID;
			if (!m_ntOrderPxOrder.TryGetValue(order.OrderId, out orderGUID) || string.IsNullOrEmpty(orderGUID))
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToCancelOrder, "Unable to cancel order '" + order.OrderId + "': Could not find PX order GUID", order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
				return;
			}

			Asset asset = GetPxAsset(order.Instrument);
			if (asset == null)
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToCancelOrder, "Unable to cancel order '" + order.OrderId + "': Could not convert Instrument to Asset", order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
				return;
			}

			if (!m_lastQuotes.ContainsKey(asset.Symbol))
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToCancelOrder, "Unable to cancel order '" + order.OrderId + "': No active quote found", order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
				return;
			}

			pxNetAdapter.Model.Trading.Order pxOrder = new pxNetAdapter.Model.Trading.Order();
			pxOrder.Type = pxNetAdapter.Model.Trading.TypeEnum.Limit;
			pxOrder.AccountGUID = order.Account.Name;
			pxOrder.QuoteGUID = m_lastQuotes[asset.Symbol].GUID;

			m_pxConnector.Send(Trading.ClosePosition(m_token, orderGUID, pxOrder), res =>
			{
				if (res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
				{
					m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToCancelOrder, "Unable to cancel order '" + order.OrderId + "': " + res.Error.Message, order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
					return;
				}

				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, OrderState.Cancelled, m_NTconnection.Now));
			});
			
			m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, OrderState.PendingCancel, m_NTconnection.Now));
		}

		public void Remap(Order oldOrder, Order newOrder)
		{
			throw new NotImplementedException();
		}

		public void Submit(Order order)
		{
			Asset asset = GetPxAsset(order.Instrument);
			if (asset == null)
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': Could not convert Instrument to Asset", order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
				return;
			}

			if (!m_lastQuotes.ContainsKey(asset.Symbol))
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': No active quote found", order.OrderId, order.LimitPrice, order.StopPrice, order.Quantity, order.AvgFillPrice, order.Filled, order.OrderState, m_NTconnection.Now));
				return;
			}

			pxNetAdapter.Model.Trading.Order pxOrder = new pxNetAdapter.Model.Trading.Order();
			pxOrder.Symbol = asset.Symbol;
			pxOrder.Type = order.OrderType == OrderType.Limit ? pxNetAdapter.Model.Trading.TypeEnum.Limit : pxNetAdapter.Model.Trading.TypeEnum.Market;
			pxOrder.Side = order.Long ? pxNetAdapter.Model.Trading.SideEnum.Buy : pxNetAdapter.Model.Trading.SideEnum.Sell;
			pxOrder.Quantity = order.Quantity;
			pxOrder.AccountGUID = order.Account.Name;
			pxOrder.QuoteGUID = m_lastQuotes[asset.Symbol].GUID;
			pxOrder.Price = order.OrderType == OrderType.Limit ? order.LimitPrice : order.Long ? m_lastQuotes[asset.Symbol].Ask : m_lastQuotes[asset.Symbol].Bid;
			
			if (order.Name == "Close")
				ClosePosition(order, pxOrder);
			else
				OpenPosition(order, pxOrder);

			m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.PendingSubmit, m_NTconnection.Now));
		}

		#endregion

		#region Methods

		private void Login()
        {
            m_pxConnector.Send(User.Login(m_NTconnection.Options.User, m_NTconnection.Options.Password), res => {
                if (res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
                {
                    m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.LoginFailed, res.Error.Message, ConnectionStatus.Disconnected, ConnectionStatus.Disconnected));
                    return;
                }

                LoginResponseData data = res.Data as LoginResponseData;
                if (data == null || string.IsNullOrEmpty(data.Token) || data.UserInfo == null || string.IsNullOrEmpty(data.UserInfo.GUID))
                {
                    m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.LoginFailed, "Invalid LoginResponseData", ConnectionStatus.Disconnected, ConnectionStatus.Disconnected));
					return;
                }

				m_token = data.Token;
				m_userInfo = data.UserInfo;
				GetInitialAppData();
            });
        }

		private void GetInitialAppData(bool isLogin=true)
		{
			m_pxConnector.Send(TradingApp.GetInitialAppData(m_token, m_userInfo.GUID), res => {
				if (isLogin && res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
				{
					m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.LoginFailed, res.Error.Message, ConnectionStatus.Disconnected, ConnectionStatus.Disconnected));
					return;
				}

				InitialAppDataResponseData data = res.Data as InitialAppDataResponseData;
				if (isLogin && data == null)
				{
					m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.LoginFailed, "Failed fetching InitialAppData", ConnectionStatus.Disconnected, ConnectionStatus.Disconnected));
					return;
				}

				if (isLogin)
					AfterSuccessLogin(data);
			});
		}

		private void OpenPosition(Order order, pxNetAdapter.Model.Trading.Order pxOrder)
		{
			m_pxConnector.Send(Trading.OpenPosition(m_token, pxOrder), res =>
			{
				if (res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
				{
					m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': " + res.Error.Message, order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.Rejected, m_NTconnection.Now));
					return;
				}

				PositionResponseData resData = res.Data as PositionResponseData;
				if (resData == null || string.IsNullOrEmpty(resData.Position.GUID))
				{
					m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': Invalid response data" + res.Error.Message, order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.Rejected, m_NTconnection.Now));
					return;
				}

				m_ntOrderPxOrder[order.OrderId] = resData.Position.GUID;

				int qty = resData.Position.Side == pxNetAdapter.Model.Trading.SideEnum.Buy ? (int)resData.Position.BuyQty : (int)resData.Position.SellQty;
				int filled = resData.Position.Type == pxNetAdapter.Model.Trading.TypeEnum.Limit ? 0 : qty;
				OrderState state = resData.Position.Type == pxNetAdapter.Model.Trading.TypeEnum.Limit ? OrderState.Working : OrderState.Filled;
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, order.OrderId, resData.Position.Price, order.StopPrice, qty, resData.Position.Price, filled, state, m_NTconnection.Now));

				MarketPosition mp = resData.Position.Side == pxNetAdapter.Model.Trading.SideEnum.Buy ? MarketPosition.Long : MarketPosition.Short;
				m_NTconnection.ProcessEventArgs(new PositionUpdateEventArgs(m_NTconnection, ErrorCode.NoError, "", Operation.Insert, order.Account, order.Instrument, mp, qty, NinjaTrader.Cbi.Currency.UsDollar, resData.Position.Price));
				m_positions.Add(resData.Position);
			});
		}

		private void ClosePosition(Order order, pxNetAdapter.Model.Trading.Order pxOrder)
		{
			// We need to find the position, if order is long it means we closing a SHORT position
			pxNetAdapter.Model.Trading.Position pxPosition = order.Long ? m_positions.FirstOrDefault(p => p.Symbol == pxOrder.Symbol && p.Side == pxNetAdapter.Model.Trading.SideEnum.Sell && order.Quantity == p.SellQty) :
																		  m_positions.FirstOrDefault(p => p.Symbol == pxOrder.Symbol && p.Side == pxNetAdapter.Model.Trading.SideEnum.Buy && order.Quantity == p.BuyQty);
			if (pxPosition == null)
			{
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': Could not find the Position to close", order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.Rejected, m_NTconnection.Now));	
				return;
			}
			
			// Calc the pnl
			double pnl = pxPosition.Side == pxNetAdapter.Model.Trading.SideEnum.Buy ? pxOrder.Price - pxPosition.Price : pxPosition.Price - pxOrder.Price;
			pnl *= pxOrder.Quantity;
			pxOrder.PNL = Math.Round(pnl, 2);
			pxOrder.PNLCurrency = "USD";

			m_pxConnector.Send(Trading.ClosePosition(m_token, pxPosition.GUID, pxOrder), res =>
			{
				if (res.Error != null && !string.IsNullOrEmpty(res.Error.Code))
				{
					m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': " + res.Error.Message, order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.Rejected, m_NTconnection.Now));
					return;
				}

				PositionResponseData resData = res.Data as PositionResponseData;
				if (resData == null || string.IsNullOrEmpty(resData.Position.GUID))
				{
					m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.UnableToSubmitOrder, "Unable to submit order '" + order.OrderId + "': Invalid response data" + res.Error.Message, order.OrderId, pxOrder.Price, order.StopPrice, (int)pxOrder.Quantity, order.AvgFillPrice, order.Filled, OrderState.Rejected, m_NTconnection.Now));
					return;
				}

				m_ntOrderPxOrder[order.OrderId] = resData.Position.GUID;

				int qty = resData.Position.Side == pxNetAdapter.Model.Trading.SideEnum.Buy ? (int)resData.Position.BuyQty : (int)resData.Position.SellQty;
				int filled = qty;
				OrderState state = OrderState.Filled;
				m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, order.OrderId, resData.Position.Price, order.StopPrice, qty, resData.Position.Price, filled, state, m_NTconnection.Now));

				MarketPosition mp = MarketPosition.Flat;
				m_NTconnection.ProcessEventArgs(new PositionUpdateEventArgs(m_NTconnection, ErrorCode.NoError, "", Operation.Remove, order.Account, order.Instrument, mp, qty, NinjaTrader.Cbi.Currency.UsDollar, resData.Position.Price));
			});
		}

		private void AfterSuccessLogin(InitialAppDataResponseData data = null)
		{
			m_state = ConnectionStateEnum.Connected;
			m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.NoError, "", ConnectionStatus.Connected, ConnectionStatus.Connected));

			if (m_firstConnect)
			{
				m_firstConnect = false;
				m_NTconnection.AccountItems = new AccountItem[] { AccountItem.CashValue, AccountItem.RealizedProfitLoss };

				m_NTconnection.Actions = new OrderAction[] { OrderAction.Buy, OrderAction.BuyToCover, OrderAction.Sell, OrderAction.SellShort };
				m_NTconnection.Currencies = new NinjaTrader.Cbi.Currency[] { NinjaTrader.Cbi.Currency.Unknown, NinjaTrader.Cbi.Currency.UsDollar };
				m_NTconnection.Exchanges = new Exchange[] { Exchange.Default };
				m_NTconnection.Features = new Feature[] { Feature.SynchronousInstrumentLookup, Feature.MarketData, Feature.Order, Feature.OrderChange };
				m_NTconnection.InstrumentTypes = new[] { InstrumentType.Currency, InstrumentType.Stock, InstrumentType.Index, InstrumentType.Future };
				m_NTconnection.MarketDataTypes = new MarketDataType[] { MarketDataType.Ask, MarketDataType.Bid, MarketDataType.DailyHigh, MarketDataType.DailyLow, MarketDataType.Last, MarketDataType.Opening };
				m_NTconnection.MarketPositions = new MarketPosition[] { MarketPosition.Flat, MarketPosition.Long, MarketPosition.Short };
				m_NTconnection.OrderStates = new OrderState[] { OrderState.Cancelled, OrderState.Filled, OrderState.Working };
				m_NTconnection.OrderTypes = new OrderType[] { OrderType.Limit, OrderType.Market };
				m_NTconnection.TimeInForces = new TimeInForce[] { TimeInForce.Gtc };
			}

			// After we reported about the login update the accounts
			if (m_userInfo.Accounts.Count > 0 && !string.IsNullOrEmpty(m_userInfo.Accounts[0].GUID))
			{
				m_NTconnection.ProcessEventArgs(new AccountEventArgs(m_NTconnection, ErrorCode.NoError, "", m_userInfo.Accounts[0].GUID, Mode.Live));
				UpdateAccountStatus(m_userInfo.Accounts[0]);
			}

			UpdatePositionsUpdate(data.Positions, data.Orders);
			UpdateMarketData(data.Quotes, data.Assets);
		}

		private void UpdateAccountStatus(pxNetAdapter.Model.User.Account acct)
		{
			NinjaTrader.Cbi.Account account = m_NTconnection.Accounts.FindByName(acct.GUID);
			if (account != null)
			{
				m_NTconnection.ProcessEventArgs(new AccountUpdateEventArgs(m_NTconnection, ErrorCode.NoError, "", account, AccountItem.TotalCashBalance, Currency.UsDollar, (double)acct.Balance, m_NTconnection.Now));
			}
		}

		private void UpdateMarketData(IDictionary<string, Quote> quotes, IDictionary<string, Asset> assets)
		{
			// Update the supported assets
			foreach (Asset a in assets.Values)
			{
				if (a.Tradable)
				{
					m_supportesAssets[a.ExchangeCode] = a;
					m_symbolExchangeCodeMap[a.Symbol] = a.ExchangeCode;
				}
				else if (m_supportesAssets.ContainsKey(a.ExchangeCode))
				{
					m_supportesAssets.Remove(a.ExchangeCode);
					m_symbolExchangeCodeMap.Remove(a.Symbol);
					// Todo: unsubscribe from it
				}
			}

			string exCode;
			Data.MarketData md;
			foreach (Quote q in quotes.Values.Where(qt => m_symbolExchangeCodeMap.ContainsKey(qt.Symbol)))
			{
				// Save the quote
				m_lastQuotes[q.Symbol] = q;

				// Check if subscribed to this quote
				exCode = m_symbolExchangeCodeMap[q.Symbol];
				if (!m_subscriptions.ContainsKey(exCode))
					continue;

				md = m_subscriptions[exCode];
				m_NTconnection.ProcessEventArgs(new MarketDataEventArgs(md, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, MarketDataType.Ask, q.Ask, 1, m_NTconnection.Now));
				m_NTconnection.ProcessEventArgs(new MarketDataEventArgs(md, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, MarketDataType.Bid, q.Bid, 1, m_NTconnection.Now));
				m_NTconnection.ProcessEventArgs(new MarketDataEventArgs(md, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, MarketDataType.Last, q.Mid, 1, m_NTconnection.Now));

                if (md.DailyLow == null || (Math.Abs(md.DailyLow.Price - q.Low) > double.Epsilon))
					m_NTconnection.ProcessEventArgs(new MarketDataEventArgs(md, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, MarketDataType.DailyLow, q.Low, 1, m_NTconnection.Now));

				if (md.DailyHigh == null || (Math.Abs(md.DailyHigh.Price - q.High) > double.Epsilon))
					m_NTconnection.ProcessEventArgs(new MarketDataEventArgs(md, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, MarketDataType.DailyHigh, q.High, 1, m_NTconnection.Now));
			}
		}

		private void UpdatePositionsUpdate(IList<pxNetAdapter.Model.Trading.Position> positions, IList<pxNetAdapter.Model.Trading.Position> orders)
		{
			//foreach (pxNetAdapter.Response.Trading.Position p in positions)
			//{
			//	//this.connection.ProcessEventArgs(new OrderStatusEventArgs(order, NinjaTrader.Cbi.ErrorCode.NoError, string.Empty, row.OrderID, rate, stopPrice, num5, executionRate, filled, OrderState.Accepted, time));
			//	m_NTconnection.ProcessEventArgs(new OrderStatusEventArgs(
			//}
		}

		private Asset GetPxAsset(Instrument instrument)
		{
			// Try FXCM in case this is currency
			string exchangeCode;
			if (instrument.MasterInstrument.InstrumentType == InstrumentType.Currency)
				exchangeCode = instrument.MasterInstrument.GetProviderName(NinjaTrader.Cbi.Provider.Provider10).TrimStart(new char[] { '@' });
			else
				exchangeCode = instrument.MasterInstrument.GetProviderName(NinjaTrader.Cbi.Provider.ESignal).TrimStart(new char[] { '@' });

			if (string.IsNullOrEmpty(exchangeCode))
				return null;

			return m_supportesAssets.ContainsKey(exchangeCode) ? m_supportesAssets[exchangeCode] : null;
		}

        #endregion

        #region EventHandlers

        void m_pxConnector_OnReconnect(object sender, GenericEventArgs<int> args)
        {
            m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.NoError, "Disconnected from ParagonEX, reconnecting...", ConnectionStatus.Connecting, ConnectionStatus.Connecting));
			m_state = ConnectionStateEnum.Connecting;
        }

        void m_pxConnector_OnDisconnect(object sender, EventArgs e)
        {
            m_state = ConnectionStateEnum.Disconnected;
            m_NTconnection.ProcessEventArgs(new ConnectionStatusEventArgs(m_NTconnection, ErrorCode.Panic, "Disconnected from ParagonEX, can\'t reconnect", ConnectionStatus.Disconnected, ConnectionStatus.Disconnected));
        }

        void m_pxConnector_OnConnect(object sender, EventArgs e)
        {
            Login();
        }

		void m_pxConnector_OnMessage(object sender, GenericEventArgs<IResponse> e)
		{
			switch (e.Args.Qualifier)
			{ 
				case ResponseTypeEnum.QuoteUpdateResponse:
					QuoteUpdateResponseData data = e.Args.Data as QuoteUpdateResponseData;
					UpdateMarketData(data.Quotes, data.Assets);
					break;
			}
		}

        #endregion
	}
}
