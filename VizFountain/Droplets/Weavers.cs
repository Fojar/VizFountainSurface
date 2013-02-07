using System.Collections.Generic;
using Nile;
using System;

namespace VizFountain.Droplets
{
	class Weavers : Droplet
	{

#region INNER_CLASSES

		public class WeaverPoint
		{
			public Point previous;
			public Point current;

			public WeaverPoint(Point p)
			{
				current = p;
				previous = p;
			}
		}

		class WeaverSet
		{
			public static Random RNG;

			public List<WeaverPoint> points;

			public double angle;

			public WeaverSet()
			{
				points = new List<WeaverPoint>();

				int w = Canvas.Width;
				int h = Canvas.Height;

				for (int i = 0; i < 10; i++)
				{
					points.Add(new WeaverPoint(new Point(RNG.NextDouble() * w, RNG.NextDouble() * h)));
				}

				angle = RNG.NextDouble() * Math.PI * 2;
			}

			public void Draw()
			{
				int max = points.Count;
				Canvas.Source = Color.FromHSVA(angle, 1, 1, .1);
				//Canvas.StrokeWidth = 5;

				if (true)
					for (int i = 0; i < max; i++) Canvas.DrawLine(points[(i + 1) % max].current, points[i].current);
				else CurvePoly(points);


				//for (int i = 0; i < max; i++) Canvas.FillCircle(points[i].current, 4);
			}

			public void Update()
			{
				int max = points.Count;
				for (int i = 0; i < max; i++) points[i].previous = points[i].current;
				for (int i = 0; i < max; i++) points[i].current.MoveToward(points[(i + 1) % max].previous, .01);
				for (int i = 0; i < max; i++) points[i].current.MoveAway(points[(i - 1 + max) % max].previous, .007);
			}

		}

#endregion

		List<WeaverSet> weaversets;

		public void Create()
		{
			WeaverSet.RNG = new Random();
			weaversets = new List<WeaverSet>();
			for (int i = 0; i < 3; i++) weaversets.Add(new WeaverSet());

			Canvas.Clear();

		}

		public Weavers()
		{
			Create();

			Canvas.Clear();
			Canvas.Blit();

			Canvas.StrokeWidth = .3;
		}

		int frame = 0;
		int imgnum = 0;

		override public void Draw()
		{
			foreach (var w in weaversets) w.Update();
			foreach (var w in weaversets) w.Draw();

			if (++frame == 10)
			{
				Canvas.Blit();
				imgnum++;

				if (imgnum == 8)
				{
					String s = String.Format("smooth{0:000}.png", smooth_value * 100);
					Console.WriteLine(s);
					//Canvas.SavePNG(s);

					//Create();
					//Canvas.Clear();
					//imgnum = 0;
				}

				frame = 0;
			}
		}


		static double smooth_value = 1;

		static public void CurvePoly(List<WeaverPoint> points)
		{
			int n = points.Count;
			Canvas.MoveTo(points[0].current);

			for (int i = 0; i < n; i++)
			{
				Point p0 = points[(i + n - 1) % n].current;
				Point p1 = points[i].current;
				Point p2 = points[(i + 1) % n].current;
				Point p3 = points[(i + 2) % n].current;

				Point c1 = (p0 + p1) / 2;
				Point c2 = (p1 + p2) / 2;
				Point c3 = (p2 + p3) / 2;
				
				double l1 = (p1 - p0).Length;
				double l2 = (p2 - p1).Length;
				double l3 = (p3 - p2).Length;

				double k1 = 11 / (l1 + l2);
				double k2 = l2 / (l2 + l3);

				Point m1 = c1 + (c2 - c1) * k1;
				Point m2 = c2 + (c3 - c2) * k2;

				Point h1 = m1 + (c2 - m1) * smooth_value + p1 - m1;
				Point h2 = m2 + (c2 - m2) * smooth_value + p2 - m2;
				
				Canvas.CurveTo(h1, h2, p2);
			}

			Canvas.ClosePath();
			Canvas.Stroke();

		}
	}
}
