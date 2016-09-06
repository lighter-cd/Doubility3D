using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapMotionWebsocket
{
    public class Frame
    {
		public bool valid;
		public float currentFrameRate;
		public long id;
		public long timestamp;

		private Vector _translation;
		private Matrix _rotation;
		private float _scaleFactor;

		public Pointable[] pointables;
		public List<Finger> fingers = new List<Finger>();
		public List<Pointable> tools = new List<Pointable>();
		public Hand[] hands;

		public Dictionary<long,Hand> handsMap = new Dictionary<long, Hand> ();
		public Dictionary<long,Pointable> pointablesMap = new Dictionary<long, Pointable> ();

		public InteractionBox interactionBox;

		public Frame(){
			valid = false;
		}
        public Frame(FrameJson data)
        {
			valid = true;
			currentFrameRate = data.currentFrameRate;
			id = data.id;
			timestamp = data.timestamp;

			_translation = new Vector (data.t [0], data.t [1], data.t [2]);
			_rotation = new Matrix (
				data.r [0] [0], data.r [0] [1], data.r [0] [2],
				data.r [1] [0], data.r [1] [1], data.r [1] [2],
				data.r [2] [0], data.r [2] [1], data.r [2] [2]);
			_scaleFactor = data.s;

			interactionBox = new InteractionBox (new Vector (data.interactionBox.center), new Vector (data.interactionBox.size));

			hands = new Hand[data.hands.Length];
			for (int i = 0; i < data.hands.Length; i++) {
				hands [i] = new Hand (data.hands [i]);
				handsMap [data.hands [i].id] = hands [i];
			}

			Array.Sort (data.pointbles, new Comparison<PointableJson> ((x, y) => {
				return (int)(x.id - y.id);
			}));

			pointables = new Pointable[data.pointbles.Length];
			for(int i=0;i<data.pointbles.Length;i++){
				pointables[i] = data.pointbles[i].tool?  new Pointable(data.pointbles[i]) : new Finger(data.pointbles[i]);
				if (data.pointbles [i].tool) {
					tools.Add (pointables [i]);
					handsMap[data.pointbles[i].handId].tools.Add(pointables [i]);
				} else {
					fingers.Add(pointables [i] as Finger);
					handsMap[data.pointbles[i].handId].fingers.Add(pointables [i] as Finger);
				}
				handsMap[data.pointbles[i].handId].pointables.Add(pointables [i]);
				pointablesMap [data.pointbles [i].id] = pointables [i];
			}
        }

		public Hand hand(long id){
			Hand hand = null;
			handsMap.TryGetValue (id, out hand);
			return hand;
		}
		public Vector Translation(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid) 
				return Vector.Zero;
			return _translation - sinceFrame._translation;
		}
		public float ScaleFactor(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid) 
				return 1.0f;
			return (float)Math.Exp(_scaleFactor - sinceFrame._scaleFactor);
		}
		public Matrix Rotation(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid)
				return Matrix.Identity;
			return _rotation * sinceFrame._rotation;
		}
    }
}
