using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MinutesToMidnight
{
	public class UILayer
	{
		public bool active;
		protected Texture2D background;
		protected Vector2 upperLeft;
		protected Vector2 lowerRight;

		public UILayer ()
		{
			active = false;
		}

		public void mouseDelegate(object sender, MouseArgs e){
		}

		public void Draw()
		{
		}
	}
}

