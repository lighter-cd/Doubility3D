using UnityEngine;
using System;
using System.Collections;

namespace LeapMotionWebsocket
{

	public class Controller : MonoBehaviour
	{
		// options
		public string host = "127.0.0.1";
		public int port = 6437;
		public bool enableGestures = false;
		public bool background = false;
		public bool optimizeHMD = false;

		Connection connection;
		CircularObjectBuffer<Frame> history = new CircularObjectBuffer<Frame> (200);
		Frame lastFrame;
		Frame lastValidFrame;
		Frame lastConnectionFrame;

		public delegate void FrameEventHandler (object sender, Frame frame);
		public delegate void HandEventHandler (object sender, Hand hand);

		public event FrameEventHandler frameEvent;
		public event HandEventHandler handEvent;
		public event EventHandler connect;
		public event EventHandler ready;
		public event EventHandler disconnect;

		// Use this for initialization
		void Start ()
		{
			connection = new Connection (host, Connection.defaultScheme, port, Connection.defaultProtocolVersion, enableGestures, background, optimizeHMD);		
			setupConnectionEvents ();
			connection.Connect ();
		}
		void OnDestory()
		{
			connection.Disconnect (false);
			connection.messageEvent -= OnConnectionMessageEvent;
			connection.connect -= OnConnectionConnectedEvent;
			connection.ready -= OnConnectionRedyEvent;
			connection.disconnect -= OnConnectionDisconnectedEvent;

		}
	
		// Update is called once per frame
		void LateUpdate ()
		{
			if (lastConnectionFrame != null) {
				processFinishedFrame (lastConnectionFrame);
			}
			connection.Update (Time.deltaTime);
		}

		void setupConnectionEvents ()
		{
			connection.messageEvent += OnConnectionMessageEvent;
			connection.connect += OnConnectionConnectedEvent;
			connection.ready += OnConnectionRedyEvent;
			connection.disconnect += OnConnectionDisconnectedEvent;
		}

		void OnConnectionMessageEvent (object sender, object obj)
		{
			if (obj is Frame) {
				ProcessFrame (obj as Frame);			
			} else if (obj is Protocol) {
				Protocol protocol = connection.Protocol;
				protocol.afterFrameCreated += (s, data, frame) => {};
				protocol.beforeFrameCreated += (s, data, frame) => {};
			}
		}
		void OnConnectionConnectedEvent(object sender, EventArgs e){
			connect (this, null);
		}
		void OnConnectionRedyEvent(object sender, EventArgs e){
			connection.ReportFocus (true);
			ready (this, null);
		}
		void OnConnectionDisconnectedEvent(object sender, EventArgs e){
			disconnect (this, null);
		}

		void ProcessFrame(Frame f){
			lastConnectionFrame = f;
		}
		void processFinishedFrame(Frame f){
			lastFrame = f;
			if (f.valid) {
				lastValidFrame = f;
			}
			history.Put (f);
			frameEvent (this, f);
			for (int i = 0; i < f.hands.Length; i++) {
				handEvent(this,f.hands[i]);
			}
		}
	}
}
