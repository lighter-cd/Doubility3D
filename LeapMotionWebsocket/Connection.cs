using System;
using UnityEngine;
using WebSocket4Net;

namespace LeapMotionWebsocket
{
    internal class MessageEventArgs : EventArgs
    {
        public object obj;
        public MessageEventArgs(object o)
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

        // events
        public delegate void MessageEventHandler(object sender, object obj);
        public event EventHandler connect;
        public event EventHandler ready;
        public event MessageEventHandler messageEvent;

		public Connection (string host = "127.0.0.1",string scheme = defaultScheme, int port = defaultPort,
			int requestProtocolVersion = defaultProtocolVersion,
			bool enableGestures = false,bool background = false, bool optimizeHMD = false)
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

            ready += delegate(object sender, System.EventArgs e)
            {
                this.EnableGestures = enableGestures;
                this.Background = background;
                this.OptimizeHMD = optimizeHMD;
            };

			Debug.Log ("opening ws...");

			ws.Open (); 

			Debug.Log ("init done.");
		}

		public string Host{
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

		public bool EnableGestures{
			get { return enableGestures;}
			set {
				enableGestures = value;
				if (ws != null && ws.State == WebSocketState.Open) {
					ws.Send (Protocol.EncodeEnableGestures(enableGestures));
				}
			}
		}
		public bool Background{
			get { return background;}
			set {
				background = value;
				if (ws != null && ws.State == WebSocketState.Open) {
					ws.Send (Protocol.EncodeEnableGestures(background));
				}
			}			
		}
		public bool OptimizeHMD{
			get { return optimizeHMD;}
			set {
                optimizeHMD = value;
				if (ws != null && ws.State == WebSocketState.Open) {
                    ws.Send(Protocol.EncodeEnableGestures(optimizeHMD));
				}
			}					
		}

		void handleMessage (object sender, MessageReceivedEventArgs e)
		{
			Debug.Log ("ws message received!");
            object obj = null;
            if (protocol == null)
            {
                protocol = Protocol.ChooseProtocol(e.Message);
                obj = protocol.Header;
                protocolVersionVerified = true;
                ready(this, null);
            }
            else
            {
                obj = protocol.Decode(e.Message);
            }
            messageEvent(this, obj);
		}


        void handleOpen (object sender, System.EventArgs e) {
            Debug.Log("ws opened.");
            if (!connected)
            {
                connected = true;
                connect(this, null);
            }
        }
        void handleClose (object sender, System.EventArgs e)
        {
            Debug.Log("ws closed.");
            if (!connected) 
                return;
            //disconnect();

            // 1001 - an active connection is closed
            // 1006 - cannot connect
            int code = (e as ClosedEventArgs).Code;
            if (code == 1001 && requestProtocolVersion > 1) {
                if (protocolVersionVerified) {
                    protocolVersionVerified = false;
                }else{
                    requestProtocolVersion--;
                }
            }
            //startReconnection();
        }

        void startReconnection()
        {
            // 500毫秒后重连
            
        }
        void stopReconnection()
        {
            // 停止计时
        }


        /* 
         * BaseConnection.prototype.startReconnection = function() {
          var connection = this;
          if(!this.reconnectionTimer){
            (this.reconnectionTimer = setInterval(function() { connection.reconnect() }, 500));
          }
        }

        BaseConnection.prototype.stopReconnection = function() {
          this.reconnectionTimer = clearInterval(this.reconnectionTimer);
        }

        // By default, disconnect will prevent auto-reconnection.
        // Pass in true to allow the reconnection loop not be interrupted continue
        BaseConnection.prototype.disconnect = function(allowReconnect) {
          if (!allowReconnect) this.stopReconnection();
          if (!this.socket) return;
          this.socket.close();
          delete this.socket;
          delete this.protocol;
          delete this.background; // This is not persisted when reconnecting to the web socket server
          delete this.optimizeHMD;
          delete this.focusedState;
          if (this.connected) {
            this.connected = false;
            this.emit('disconnect');
          }
          return true;
        }

        BaseConnection.prototype.reconnect = function() {
          if (this.connected) {
            this.stopReconnection();
          } else {
            this.disconnect(true);
            this.connect();
          }
        }
 
        BaseConnection.prototype.connect = function() {
          if (this.socket) return;
          this.socket = this.setupSocket();
          return true;
        }

        BaseConnection.prototype.send = function(data) {
          this.socket.send(data);
        }

        BaseConnection.prototype.reportFocus = function(state) {
          if (!this.connected || this.focusedState === state) return;
          this.focusedState = state;
          this.emit(this.focusedState ? 'focus' : 'blur');
          if (this.protocol && this.protocol.sendFocused) {
            this.protocol.sendFocused(this, this.focusedState);
          }
        } 
         */

    }
}

