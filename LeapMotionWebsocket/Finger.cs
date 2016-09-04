using System;

namespace LeapMotionWebsocket
{
	public enum FingerType
	{
		Thumb = 0,		// 拇指
		Index_Finger,
		Middle_Finger,
		Ring_Finger,
		Pinky			// 小指
	}

	public class Finger : Pointable
	{
		public FingerType type; // 0 is thumb; 4 is pinky
		public bool extended;

		public Vector carpPosition; // the position of the base of metacarpal bone as an array of 3 floats.
		public Vector mcpPosition; // a position vector as an array of 3 floating point numbers
		public Vector pipPosition; // a position vector as an array of 3 floating point numbers
		public Vector dipPosition; // the position of the base of the distal phalanx as an array of 3 floats.

		public Vector[] positions;
		public Bone[] bones; 

		public Finger (PointableJson data) : base(data)
		{
			type = (FingerType)data.type;
			extended = data.extended;

			carpPosition = new Vector(data.carpPosition[0],data.carpPosition[1],data.carpPosition[2]);
			mcpPosition = new Vector(data.mcpPosition[0],data.mcpPosition[1],data.mcpPosition[2]);
			pipPosition = new Vector(data.pipPosition[0],data.pipPosition[1],data.pipPosition[2]);
			dipPosition = new Vector(data.dipPosition[0],data.dipPosition[1],data.dipPosition[2]);

			positions = new Vector[]{this.carpPosition, this.mcpPosition, this.pipPosition, this.dipPosition, this.tipPosition};

			bones = new Bone[4];
			bones [0] = new Bone (carpPosition, mcpPosition, width, Bone.BoneType.TYPE_METACARPAL, data.bases [0]);
			bones [1] = new Bone (mcpPosition, pipPosition, width, Bone.BoneType.TYPE_PROXIMAL, data.bases [1]);
			bones [2] = new Bone (pipPosition, dipPosition, width, Bone.BoneType.TYPE_INTERMEDIATE, data.bases [2]);
			bones [3] = new Bone (dipPosition, tipPosition, width, Bone.BoneType.TYPE_DISTAL, data.bases [3]);
		}
	}
}

