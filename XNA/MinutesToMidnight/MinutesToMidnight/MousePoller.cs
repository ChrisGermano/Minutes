using System;
using Microsoft.Xna.Framework.Input;

namespace MinutesToMidnight
{

	public delegate void MouseEventHandler(object sender, MouseArgs e);

	public class MousePoller
	{
		private static MousePoller m_instance;

		public event MouseEventHandler handler;

		private int oldX;
		private int oldY;
		private bool oldPressed;

		private MousePoller ()
		{
			oldX = 0;
			oldY = 0;
			oldPressed = false;
		}

		public static MousePoller Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new MousePoller ();
				}
				return m_instance;
			}
		}

		public void update(){
			MouseState ms = Mouse.GetState ();

			if (ms.X != oldX || ms.Y != oldY || (ms.LeftButton == ButtonState.Pressed) != oldPressed) {

				bool pressChange = false;

				if ((ms.LeftButton == ButtonState.Pressed) != oldPressed) {
					pressChange = true;
				}

				oldX = ms.X;
				oldY = ms.Y;
				oldPressed = (ms.LeftButton == ButtonState.Pressed);



				if (handler != null) {
					handler(this, new MouseArgs(oldX, oldY, oldPressed, pressChange));
				}
			}
		}
	}
}

