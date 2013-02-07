using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nile;

namespace VizFountain.Droplets
{

	class Polygons : Droplet
	{

		class Polygon
		{
			public List<Point> points;
			public double area;
			Color color;

			static double angle = RNG.NextDouble(TAU);

			public Polygon(List<Point> points)
			{
				this.points = points;
				CalculateArea();
				color = Color.FromHSVA(angle, 1, 1, .05 + RNG.NextDouble() * .25);
			}

			public Polygon()
			{
				points = new List<Point>();
				color = Color.FromHSVA(angle, 1, 1, .05 + RNG.NextDouble() * .25);
			}

			public void CalculateArea()
			{
				area = 0;

				int j = points.Count - 1;		// The last vertex is the 'previous' one to the first

				for (int i = 0; i < points.Count; i++)
				{
					area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
					j = i;
				}

				area /= 2;
				area = Math.Abs(area);
			}


			public void Draw()
			{
				if (points.Count == 0) return;

				Canvas.MoveTo(points[0]);
				for (int i = 1; i < points.Count; i++) Canvas.LineTo(points[i]);
				Canvas.ClosePath();

				Canvas.Source = color;
				Canvas.FillPreserve();

				Canvas.Source = new Color(0, 0, 0, .2);
				Canvas.Stroke();

			}

		}

		List<Polygon> polygons = new List<Polygon>();

		public Polygons()
		{
			Canvas.StrokeWidth = 1;
			Canvas.Clear(new Color(1, 1, 1));

			polygons.Add(new Polygon(new List<Point> {
			    new Point(0, 0),
			    new Point(Canvas.Width, 0),
			    new Point(Canvas.Width, Canvas.Height),
			    new Point(0, Canvas.Height)
			}));

			polygons[0].CalculateArea();
		}

		class Node
		{
			public int index;
			public double length;
		}


		void SubDivide(Polygon root)
		{
			List<Node> nodes = new List<Node>();

			for (int i = 0; i < root.points.Count; i++)
			{
				var c = root.points[i];
				var d = root.points[(i + 1) % root.points.Count];

				double dx = d.X - c.X;
				double dy = d.Y - c.Y;
				double distance = Math.Sqrt(dx * dx + dy * dy);

				nodes.Add(new Node { index = i, length = distance });
			}

			nodes.Sort((aa, bb) => aa.length == bb.length ? 0 : (aa.length - bb.length) < 0 ? 1 : -1);

			int totalPoints = root.points.Count;

			// pick one edge at random
			var i1 = RNG.Next(totalPoints);
			i1 = nodes[0].index;
			var i2 = (i1 + 1) % totalPoints;


			var v1 = root.points[i1];
			var v2 = root.points[i2];

			// pick another edge

			int i3;
			do i3 = RNG.Next(totalPoints); while (i3 == i1);
			i3 = nodes[1].index;
			var i4 = (i3 + 1) % totalPoints;

			var v3 = root.points[i3];
			var v4 = root.points[i4];

			double q = RNG.NextDouble() * .5 + .25;
			Point a = (v1 * q + v2 * (1 - q));
			q = RNG.NextDouble();
			Point b = (v3 * q + v4 * (1 - q));

			Polygon first = new Polygon();
			Polygon second = new Polygon();

			first.points.Add(a);
			first.points.Add(b);
			// now all the points from v4 up to v1
			for (int i = i4; i % totalPoints != (i1 + 1) % totalPoints; i++) first.points.Add(root.points[i % totalPoints]);


			second.points.Add(b);
			second.points.Add(a);
			// now all the points from v2 up to v3
			for (int i = i2; i % totalPoints != (i3 + 1) % totalPoints; i++) second.points.Add(root.points[i % totalPoints]);

			polygons.Remove(root);

			first.CalculateArea();
			second.CalculateArea();
			polygons.Add(first);
			polygons.Add(second);
		}

		public override void Update()
		{
		}

		public override void Draw()
		{
			int limit = 1 + (int)Math.Sqrt(polygons.Count);
			for (int i = 0; i < limit; i++)
			{
				//polygons.Sort((a, b) => (a.area - b.area < 0 ? -1 : 1));
				//SubDivide(polygons[0]);

				var p = polygons[RNG.Next(polygons.Count)];

				if (p.area > 1000) SubDivide(p);
				else Console.WriteLine(p.area);
			}



			Canvas.Clear();
			foreach (var p in polygons) p.Draw();

			Canvas.Blit();
		}
	}
}
