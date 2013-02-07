using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nile;
using System.Threading;

namespace VizFountain.Droplets
{
	class Boxer : Droplet
	{
		static Color BLACK = new Color(0, 0, 0);

		//-----------------------------------------------------------------------------------------------
		class Line
		{
			double Y;
			double Z;

			List<Box> boxes;

			public Line(double z)
			{
				Y = RNG.NextDouble() * Canvas.Height;
				Z = z;

				boxes = new List<Box>();
				boxes.Add(new Box(Y, Z));
			}


			public void Draw()
			{
				Canvas.StrokeWidth = Z * .2;
				Canvas.Source = BLACK;
				//Canvas.DrawLine(0, Y, Canvas.Width, Y);

				foreach (var b in boxes) b.Draw();
			}


			public void Update()
			{
				foreach (Box b in boxes) b.Update();

				// get the last box
				if (boxes.Count > 0)
				{
					Box last = boxes[boxes.Count - 1];
					if (last.pos.X - last.size.X > 0) boxes.Add(new Box(last));
				}

				boxes.RemoveAll(b => b.pos.X > Canvas.Width + b.size.X / 2);

			}
		}


		//-----------------------------------------------------------------------------------------------
		class Box
		{
			double Z;

			public Point pos;
			public Point size;

			public double speed;

			public Color color;

			public Box(double Y, double Z)
			{
				this.Z = Z;

				size = new Point(80 * Z, 80 * Z);

				pos.Y = Y;
				pos.X = -size.X;

				speed = Z * 5;

				color = Color.FromHSVA(RNG.NextDouble() * TAU / 6 + (7 * TAU / 12), Z, 1, .5);

				pos.Y = RNG.NextDouble() * Canvas.Height;
			}

			public Box(Box last)
			{
				Z = last.Z;
				pos = last.pos;
				size = last.size;
				speed = last.speed;
				//speed += size.X * .01;
				color = last.color;

				pos.X = pos.X - (1.5 + RNG.NextDouble() * 5.5) * size.X;

				pos.Y = RNG.NextDouble() * Canvas.Height;
			}

			public void Draw()
			{
				
				Canvas.Source = color;
				Canvas.FillRectangle(pos.X - size.X / 2, pos.Y - size.Y / 2, size.X, size.Y);

				//Canvas.FillCircle(pos, size.X / 2);

				Canvas.StrokeWidth = Z * 2;
				Canvas.Source = new Color(0, 0, 0, 1);
				Canvas.DrawRectangle(pos.X - size.X / 2, pos.Y - size.Y / 2, size.X, size.Y);
			}

			public void Update()
			{
				pos.X += speed;
			}
		}



		List<Line> lines;



		//-----------------------------------------------------------------------------------------------
		public Boxer()
		{
			lines = new List<Line>();

			const int total_lines = 30;
			for (int i = 0; i < total_lines; i++)
			{
				double f = (double)i / total_lines;

				f = Math.Pow(f, 4);

				const double min = .1;
				f = f * (1 - min) + min;

				lines.Add(new Line(f));
			}

			Canvas.Clear();
		}



		public override void Draw()
		{
			Canvas.Clear();

			foreach (var l in lines) l.Draw();

			Canvas.Blit();
		}

		public override void Update()
		{
			foreach (var l in lines) l.Update();
		}



	}
}
