using System;
using LitJson;

namespace LeapMotionWebsocket
{
    class Header
    {
        public string serviceVersion = "";
        public int version = 0;
    }


    class Event
    {
		public class State{
			public bool attached;
			public string id;
			public bool streaming;
			public string type;	
		}
		public State state;
		public string type;
    }

	public class HandleJson
	{
		public long id;
		public string type; // one of "right", "left"
		public float timeVisible;

		public float[][] r;
		public float s;
		public float[] t;

		public float[][] armBasis;
		public float armWidth;
		public float confidence;
		public float[] direction;
		public float[] elbow;
		public float grabStrength;
		public float[] palmNormal;
		public float[] palmPosition;
		public float[] palmVelocity;
		public float pinchStrength;
		public float[] sphereCenter;
		public float sphereRadius;
		public float[] stabilizedPalmPosition;
		public float[] wrist;
	}

	public class InteractionBoxJson{
		public float[] center;
		public float[] size;
	}

	public class Pointable{
		public long id;
		public long handId;
		public long type; // 0 is thumb; 4 is pinky
		public float timeVisible;

		public float length;
		public float width;

		public bool extended;
		public bool tool;

		public float[][] bases; // the 3 basis vectors for each bone, in index order, wrist to tip, (array of vectors).
		public float[] btipPosition; // the position of the tip of the distal phalanx as an array of 3 floats.
		public float[] carpPosition; // the position of the base of metacarpal bone as an array of 3 floats.
		public float[] dipPosition; // the position of the base of the distal phalanx as an array of 3 floats.
		public float[] direction;
		public float[] mcpPosition; // a position vector as an array of 3 floating point numbers
		public float[] pipPosition; // a position vector as an array of 3 floating point numbers
		public float[] stabilizedTipPosition;
		public float[] tipPosition;
		public float[] tipVelocity;
		public float touchDistance;
		public string touchZone; // one of "none", "hovering", "touching"
	}

    public class FrameJson
    {
		public float currentFrameRate;
		public long id;
		public long timestamp;
		public float[][] r;
		public float s;
		public float[] t;
		// devices[]

		public HandleJson[] handles;
		public InteractionBoxJson interactionBox;
		public Pointable pointbles;
    }

    internal class Protocol
    {
        Header header;

        public delegate void FrameEventHandler(object sender, FrameJson obj, Frame frame);
        public event FrameEventHandler beforeFrameCreated;
        public event FrameEventHandler afterFrameCreated;

        public static Protocol ChooseProtocol(string message)
        {
            Protocol protocol = null;
            Header header = JsonMapper.ToObject<Header>(message);
            if(header.version >=1 && header.version <= 7) {
                protocol = new Protocol(header);
            }else{
                throw new Exception("unrecognized version");
            }
            return protocol;
        }
        public Protocol(Header header)
        {
            this.header = header;
        }
        public object Decode(string message)
        {
            if (message.IndexOf("event") >= 0)
            {
                Event e = JsonMapper.ToObject<Event>(message);
                return e;
            }else{
                
                FrameJson data = JsonMapper.ToObject<FrameJson>(message);
                beforeFrameCreated(this, data,null);
                Frame frame = new Frame(data);
                afterFrameCreated(this, data, frame);
                return frame;
            }
        }
        public Header Header{
            get {return header;}
        }

        static public string EncodeEnableGestures(bool b)
        {
            return EncodeBool("enableGestures", b);
        }
        static public string EncodeBackground(bool b)
        {
            return EncodeBool("background", b);
        }
        static public string EncodeOptimizeHMD(bool b)
        {
            return EncodeBool("optimizeHMD", b);
        }
        static public string EncodeFocused(bool b)
        {
            return EncodeBool("focused", b);
        }

        static string EncodeBool(string var, bool b)
        {
            return string.Format("{\"{0}\":{1}}",var,b);
        }
    }
}
