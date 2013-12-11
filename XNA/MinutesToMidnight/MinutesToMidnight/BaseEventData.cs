using System;

namespace MinutesToMidnight
{
	public class BaseEventData : EventArgs
	{
		public int hashVal { get; protected set; }
		public BaseEventData ()
		{
			//hashVal = System.String ("base");
		}
	}
}

