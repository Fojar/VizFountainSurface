using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nile;

namespace VizFountain.Droplets
{
	class Anemone : Droplet
	{
		class Circle
		{
			Point position;
			Point velocity;

			double alpha;
			double spin;

			double speed;

			Color color;

			double angle = RNG.NextDouble() * TAU;

			public Circle(double speed)
			{
				this.speed = speed;
				
				position = new Point(Canvas.Width / 2, Canvas.Height / 2);
				velocity = new Point(speed, speed);
				
				alpha = 1;

				spin = (RNG.NextDouble() - .5) * .4;
				
			}

			public void Draw()
			{
				Canvas.Source = new Color(1, .5, 0, alpha * .5 + (RNG.NextDouble() * .4 - .2));

				double a = alpha * .3 + (RNG.NextDouble() * .1 - .05);

				a = alpha * .3;
				a = 1;

				double size = speed * 5 * (Math.Sin(angle * 3) + 2) / 3; //(RNG.NextDouble() * .5 + .5);

				//Canvas.Source = new Color(1, 1, 1);
				//Canvas.DrawCircle(new Point(position.X, position.Y + 1), size);

				Canvas.Source = Color.FromHSVA(angle, .3, .8, a);
				//Canvas.Source = new Color(1, 1, 1);
				Canvas.FillCircle(position, size);

				Canvas.Source = new Color(0, 0, 0, .25);
				Canvas.StrokeWidth = speed * .2;
				Canvas.DrawCircle(position, size);
				
			}

			public void Update()
			{
				angle += spin;

				velocity = new Point(Math.Cos(angle) * speed, Math.Sin(angle) * speed);

				position += velocity;

				alpha -= .01;
				//speed *= .983;

				//velocity.X += (RNG.NextDouble() - .5) * 15.5;
				//velocity.Y += (RNG.NextDouble() - .5) * 15.5;
			}
		}




		List<Circle> circles;


		// Constructor
		public Anemone()
		{
			circles = new List<Circle>();

			for (int i = 0; i < 200; i++) circles.Add(new Circle(i / 20.0));

			Canvas.Clear();
			Canvas.Source = new Color(0, 0, 0, .1);

			
		}



		public override void Draw()
		{
			//Canvas.Clear();

			foreach (var item in circles) item.Update();
			foreach (var item in circles) item.Draw();

			Canvas.Blit();	// show on screen
		}

		public override void Update()
		{
			
		}



	}
}
