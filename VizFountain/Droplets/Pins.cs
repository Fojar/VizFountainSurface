using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Surface.Core;

using Nile;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace VizFountain.Droplets
{

	class Pins : Droplet
	{

		class Node
		{
			public Point anchor;
			public Point displacement;

			public Node(Point p)
			{
				anchor = p;
			}

			public void Draw()
			{
				Canvas.DrawLine(anchor, anchor + displacement);
				double radius = 2 + displacement.Length * .4;
				Canvas.FillCircle(anchor + displacement, radius);
			}
		}


		List<Node> nodes = new List<Node>();


		public Pins()
		{
			Debug.WriteLine("SurfaceTest3.ctor");
			Canvas.Clear();
			Canvas.Blit();

			const int X_SEPARATION = 60;
			const int Y_SEPARATION = 60;

			for (int x = 0; x < Canvas.Size.X; x += X_SEPARATION)
			{
				for (int y = 0; y < Canvas.Size.Y; y += Y_SEPARATION)
				{
					nodes.Add(new Node(new Point(x, y)));
				}
			}

			Canvas.Source = new Color(0, .5, 1, .2);

		}

		ConcurrentDictionary<int, TouchPoint> touches = new ConcurrentDictionary<int, TouchPoint>();

		public override void Input(ConcurrentDictionary<int, TouchPoint> touches)
		{
			this.touches = touches;
		}


		public override void Update()
		{
			foreach (var n in nodes)
			{
				n.anchor += new Point(.1, .1);
				if (n.anchor.X > 1920) n.anchor.X -= 1920;
				if (n.anchor.Y > 1080) n.anchor.Y -= 1080;


				n.displacement = new Point();

				foreach (var t in touches.Values)
				{
					var p = t.ToPoint();
					var v = n.anchor - p;
					var l = v.Length;
					var f = Math.Pow(50000 / (l + 1), .5);
					n.displacement += new Point(v.X / l * f, v.Y / l * f);
				}
			}
		}


		public override void Draw()
		{
			Canvas.Clear();

			foreach (var n in nodes) n.Draw();

			//foreach (var t in touches.Values) Canvas.DrawCircle(t.ToPoint(), 20);

			Canvas.Blit();
		}

	}
}
