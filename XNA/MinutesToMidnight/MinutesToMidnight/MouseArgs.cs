using System;

namespace MinutesToMidnight
{
	public class MouseArgs : EventArgs
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public bool down { get; private set; }
		public bool change { get; private set; }

		public MouseArgs (int _x, int _y, bool _d, bool _c) {
			x = _x;
			y = _y;
			down = _d;
			change = _c;
		}
	}
}

