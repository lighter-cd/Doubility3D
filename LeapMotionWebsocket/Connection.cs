using System;
using UnityEngine;
using WebSocket4Net;
using RSG;

namespace LeapMotionWebsocket
{
	internal class MessageEventArgs : EventArgs
	{
		public object obj;

		public MessageEventArgs (object o)
		{
			obj = o;
		}
	}

	internal class Connection
	{
		public const int defaultProtocolVersion = 6;
		public const string defaultScheme = "ws";
		public const int defaultPort = 6437;

		// option
		string host;
		string scheme;
		int port;
		int requestProtocolVersion;

		bool enableGestures;
		bool background;
		bool optimizeHMD;

		// objects
		WebSocket ws;
		Protocol protocol;
        
		// states
		bool connected;
		bool focused;
		bool protocolVersionVerified;
		PromiseTimer promiseTimer;

		// events
		public delegate void MessageEventHandler (object sender, object obj);

		public delegate void FocuseEventHandler (object sender, bool focused);

		public event EventHandler connect;
		public event EventHandler ready;
		public event EventHandler disconnect;
		public event FocuseEventHandler focuseEvent;
		public event MessageEventHandler messageEvent;

		public Connection (string host = "127.0.0.1", string scheme = defaultScheme, int port = defaultPort,
		                   int requestProtocolVersion = defaultProtocolVersion,
		                   bool enableGestures = false, bool background = false, bool optimizeHMD = false)
		{
			this.host = host;
			this.scheme = scheme;
			this.port = port;
			this.requestProtocolVersion = requestProtocolVersion;

			string url = scheme + "://" + host + ":" + port + "/v" + requestProtocolVersion + ".json";
			ws = new WebSocket (url, WebSocketVersion.Rfc6455);
			ws.AllowUnstrustedCertificate = true;
			ws.MessageReceived += handleMessage;
			ws.Opened += handleOpen;
			ws.Closed += handleClose;

			ws.DataReceived += delegate(object sender, DataReceivedEventArgs e) {
				Debug.Log ("ws data received!");
			};
			ws.Error += delegate(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
				Debug.Log ("ws error!");
			};

			ready += delegate(object sender, System.EventArgs e) {
				this.EnableGestures = enableGestures;
				this.Background = background;
				this.OptimizeHMD = optimizeHMD;
			};
		}

		public string Host {
			get { return host; }
		}

		public string Scheme {
			get { return scheme; }
		}

		public int Port {
			get { return port; }
		}

		public int RequestProtocolVersion {
			get { return requestProtocolVersion; }
		}

		public bool EnableGestures {
			get { return enableGestures; }
			set {
				enableGestures = value;
				if (ws != null && ws.State == WebSocketState.Open) {
					ws.Send (Protocol.EncodeEnableGestures (enableGestures));
				}
			}
		}

		public bool Background {
			get { return background; }
			set {
				background = value;
				if (ws != null && ws.State == WebSocketState.Open) {
					ws.Send (Protocol.EncodeEnableGestures (background));
				}
			}			
		}

		public bool OptimizeHMD {
			get { return optimizeHMD; }
			set {
				optimizeHMD = value;
				if (ws != null && ws.State == WebSocketState.Open) {
					ws.Send (Protocol.EncodeEnableGestures (optimizeHMD));
				}
			}					
		}

		void handleMessage (object sender, MessageReceivedEventArgs e)
		{
			Debug.Log ("ws message received!");
			object obj = null;
			if (protocol == null) {
				protocol = Protocol.ChooseProtocol (e.Message);
				obj = protocol;
				protocolVersionVerified = true;
				ready (this, null);
			} else {
				obj = protocol.Decode (e.Message);
			}
			messageEvent (this, obj);
		}


		void handleOpen (object sender, System.EventArgs e)
		{
			Debug.Log ("ws opened.");
			if (!connected) {
				connected = true;
				connect (this, null);
			}
		}

		void handleClose (object sender, System.EventArgs e)
		{
			Debug.Log ("ws closed.");
			if (!connected)
				return;
			Disconnect (false);

			// 1001 - an active connection is closed
			// 1006 - cannot connect
			int code = (e as ClosedEventArgs).Code;
			if (code == 1001 && requestProtocolVersion > 1) {
				if (protocolVersionVerified) {
					protocolVersionVerified = false;
				} else {
					requestProtocolVersion--;
				}
			}
			StartReconnection ();
		}

		void StartReconnection ()
		{
			// 500毫秒后重连
			Connection connection = this;
			promiseTimer = new PromiseTimer ();
			promiseTimer.WaitFor (0.5f).Then (() => {
				connection.promiseTimer = null;
				connection.Reconnect ();
			});
            
		}

		void StopReconnection ()
		{
			// 停止计时
			promiseTimer = null;
		}

		public void Update (float deltaTime)
		{
			if (promiseTimer != null) {
				promiseTimer.Update (deltaTime);
			}
		}

		public void Disconnect (bool allowReconnect)
		{
			if (!allowReconnect)
				StopReconnection ();
			if (ws == null)
				return;
			ws.Close ();

			protocol = null;
			if (connected) {
				connected = false;
				disconnect (this, null);
			}
		}

		public void Reconnect ()
		{
			if (connected) {
				StopReconnection ();
			} else {
				Disconnect (true);
				Connect ();
			}		
		}

		public void Connect ()
		{
			Debug.Log ("opening ws...");
			ws.Open (); 
			Debug.Log ("init done.");
		}

		public void Send (string message)
		{
			ws.Send (message);
		}

		public void ReportFocus (bool focuse)
		{
			if (!connected || focused == focuse)
				return;
			focused = focuse;
			focuseEvent (this,focused);
			if (ws != null && ws.State == WebSocketState.Open) {
				ws.Send (Protocol.EncodeFocused (focused));
			}
		}

		public Protocol Protocol {get{return protocol;}}
	}
}

