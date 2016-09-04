using System;

namespace LeapMotionWebsocket
{
	public enum TouchZone
	{
		None = 0,
		Hovering,
		Touching,
	}

	public class Pointable
	{
		public bool valid;

		public long id;			// 唯一的ID,手指丢失后,下次出现会被赋予新的ID
		public long handId;
		public float timeVisible;

		public float length;	// 手指长度 毫米
		public float width;

		public bool tool;

		public float touchDistance;
		public TouchZone touchZone; // one of "none", "hovering", "touching"

		public Vector direction;				// 手指指向的方向
		public Vector stabilizedTipPosition;	// 指尖位置(毫米)	基于速度的精确位置
		public Vector tipPosition;				// 指尖位置(毫米)
		public Vector tipVelocity;				// 指尖速度

		public Pointable (PointableJson data)
		{
			valid = true;

			id = data.id;
			handId = data.handId;
			timeVisible = data.timeVisible;

			length = data.length;
			width = data.width;

			tool = data.tool;

			touchDistance = data.touchDistance;
			if (data.touchZone == "hovering") {
				touchZone = TouchZone.Hovering;
			} else if (data.touchZone == "touching") {
				touchZone = TouchZone.Touching;
			}

			direction = new Vector (data.direction [0], data.direction [1], data.direction [2]);
			stabilizedTipPosition = new Vector (data.stabilizedTipPosition [0], data.stabilizedTipPosition [1], data.stabilizedTipPosition [2]);
			tipPosition = new Vector (data.tipPosition [0], data.tipPosition [1], data.tipPosition [2]);
			tipVelocity = new Vector (data.tipVelocity [0], data.tipVelocity [1], data.tipVelocity [2]);
		}
	}
}

