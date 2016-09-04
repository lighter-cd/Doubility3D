using System;
using System.Collections.Generic;

namespace LeapMotionWebsocket
{
	public enum HandType{
		Left,
		Right
	}

	public class Hand
	{
		public bool valid;

		public long id;
		public HandType type;
		public float timeVisible;

		private Vector _translation;
		private Matrix _rotation;
		private float _scaleFactor;

		public Vector direction;	// 手掌到手指的方向
		public Vector palmPosition;	// 手掌位置
		public Vector palmVelocity;	
		public Vector palmNormal;	// 手掌的朝向
		public Vector stabilizedPalmPosition;

		public Bone arm;
		public List<Finger> fingers = new List<Finger>();
		public List<Pointable> tools = new List<Pointable>();
		public List<Pointable> pointables = new List<Pointable>();

		public Vector sphereCenter;
		public float sphereRadius;
		public float confidence;	// 数据质量信赖程度
		public float grabStrength;	// 抓握强度
		public float pinchStrength;	// 

		public Hand (HandJson data)
		{
			valid = true;
			id = data.id;
			type = data.type == "left" ? HandType.Left : HandType.Right;
			timeVisible = data.timeVisible;

			_translation = new Vector (data.t [0], data.t [1], data.t [2]);
			_rotation = new Matrix (
				data.r [0] [0], data.r [0] [1], data.r [0] [2],
				data.r [1] [0], data.r [1] [1], data.r [1] [2],
				data.r [2] [0], data.r [2] [1], data.r [2] [2]);
			_scaleFactor = data.s;

			direction = new Vector (data.direction [0], data.direction [1], data.direction [2]);
			palmPosition = new Vector (data.palmPosition [0], data.palmPosition [1], data.palmPosition [2]);
			palmVelocity = new Vector (data.palmVelocity [0], data.palmVelocity [1], data.palmVelocity [2]);
			palmNormal = new Vector (data.palmNormal [0], data.palmNormal [1], data.palmNormal [2]);
			stabilizedPalmPosition = new Vector (data.stabilizedPalmPosition [0], data.stabilizedPalmPosition [1], data.stabilizedPalmPosition [2]);

			Vector elbow = new Vector (data.elbow [0], data.elbow [1], data.elbow [2]);
			Vector wrist = new Vector (data.wrist [0], data.wrist [1], data.wrist [2]);
			arm = new Bone (elbow, wrist, data.armWidth, Bone.BoneType.TYPE_ARM, data.armBasis);

			sphereCenter = new Vector (data.sphereCenter [0], data.sphereCenter [1], data.sphereCenter [2]);
			sphereRadius = data.sphereRadius;
			confidence = data.confidence;
			grabStrength = data.grabStrength;
			pinchStrength = data.pinchStrength;
		}

		public Vector Translation(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid) 
				return Vector.Zero;
			Hand sinceHand = sinceFrame.hand(this.id);
			if(!sinceHand.valid) 
				return Vector.Zero;

			return _translation - sinceHand._translation;
		}
		public float ScaleFactor(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid) 
				return 1.0f;
			Hand sinceHand = sinceFrame.hand(this.id);
			if(!sinceHand.valid) 
				return 1.0f;

			return (float)Math.Exp(_scaleFactor - sinceHand._scaleFactor);
		}
		public Matrix Rotation(Frame sinceFrame){
			if (!this.valid || !sinceFrame.valid)
				return Matrix.Identity;
			Hand sinceHand = sinceFrame.hand (this.id);
			if (!sinceHand.valid)
				return Matrix.Identity;

			return _rotation * sinceHand._rotation;
		}
	}
}

