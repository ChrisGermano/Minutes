using System;

namespace MinutesToMidnight
{
	public delegate void UpEventHandler(object sender, EventArgs e);

	public class EventManager
	{
		private static EventManager m_instance;

		private EventManager (){

		}

		public static EventManager Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new EventManager ();
				}
				return m_instance;
			}
		}
	}
}

