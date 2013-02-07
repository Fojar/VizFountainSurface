using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Surface.Core;

using Nile;
using System.Diagnostics;


namespace VizFountain.Droplets
{

	class Button
	{
		public double x, y;
		public double w, h;

		Action action;

		protected string text;

		protected Button() { }

		public Button(double x, double y, double w, double h, string text, Action action)
		{
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
			this.text = text;
			this.action = action;
		}


		public void Draw()
		{
			Canvas.SaveState();

			Canvas.Source = new Color(1, 1, 1, .5);
			Canvas.FillRectangle(x, y, w, h);

			Canvas.StrokeWidth = .1;
			Canvas.Source = new Color(0, 0, 0, .5);
			Canvas.DrawRectangle(x, y, w, h);

			Canvas.MoveTo(x + 5, y + h * .67);
			Canvas.ShowText(text);

			Canvas.RestoreState();
		}

		public virtual bool CheckAndHandleTap(TouchPoint t)
		{
			if (IsUnderneath(t))
			{
				action();
				return true;
			}
			else return false;
		}


		public bool IsUnderneath(TouchPoint t)
		{
			var tx = t.CenterX;
			var ty = t.CenterY;

			return (tx > x && tx < x + w && ty > y && ty < y + h);
		}


	}

	class Sketchy : Droplet
	{
		class Node
		{
			public Point pos;
			public Point vel;

			public Node(Point pos)
			{
				this.pos = pos;
			}

			public void Update()
			{
				const double noise = .1;

				vel.X += RNG.NextDouble(-noise, noise);
				vel.Y += RNG.NextDouble(-noise, noise);

				vel *= .99;

				pos += vel;
			}

		}

		List<Node> nodes = new List<Node>();
		List<Button> buttons = new List<Button>();
		Dictionary<string, Color> colors = new Dictionary<string, Color>();
		Color currentColor;

		public Sketchy()
		{
			Canvas.Clear();
			Canvas.Blit();
			Canvas.StrokeWidth = 1;
			Canvas.Source = new Color(0, 0, .3);

			colors.Add("RED", new Color(.5, 0, 0, .1));
			colors.Add("YELLOW", new Color(.5, .5, 0, .1));
			colors.Add("GREEN", new Color(0, .5, 0, .1));
			colors.Add("CYAN", new Color(0, .5, .5, .1));
			colors.Add("BLUE", new Color(0, 0, .5, .1));
			colors.Add("MAGENTA", new Color(.5, 0, .5, .1));
			colors.Add("BLACK", new Color(0, 0, 0, .1));
			colors.Add("WHITE", new Color(1, 1, 1, .1));

			currentColor = colors["BLACK"];

			int y = 50;
			foreach (var c in colors)
			{
				string name = c.Key;
				Color color = c.Value;

				buttons.Add(new Button(5, y, 60, 40, name, () =>
				{
					currentColor = color;
				}
				));

				y += 60;
			}

			const int SEPARATION = 80;

			y += SEPARATION;
			buttons.Add(new Button(5, y, 60, 40, "DETACH", () => nodes.Clear()));
			y += SEPARATION;
			buttons.Add(new Button(5, y, 60, 40, "CLEAR", () => { Canvas.Clear(); nodes.Clear(); }));
			y += SEPARATION;
			buttons.Add(new Button(5, y, 60, 40, "SAVE", () => SavePNG()));
			y += SEPARATION;
			buttons.Add(new Button(5, y, 60, 40, "EXIT", () => Exit()));

		}

		private void SavePNG()
		{
			string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			Canvas.SavePNG("Sketchy_" + timestamp + ".png");
		}

		public override void TouchTap(TouchPoint t)
		{
			if (HandleButtons(t)) return;

			Point a = t.ToPoint();

			Canvas.Source = currentColor;
			Canvas.FillCircle(a, 2);

			foreach (var b in nodes)
			{
				var v = a - b.pos;

				const double DISTANCE_LIMIT = 200;
				if (v.Length < DISTANCE_LIMIT)
				{
					Canvas.Source = new Color(currentColor.Red, currentColor.Green, currentColor.Blue, (1 - (v.Length / DISTANCE_LIMIT)) * .5);
					Canvas.DrawLine(a, b.pos);
				}
			}

			nodes.Add(new Node(a));

		}

		private bool HandleButtons(TouchPoint t)
		{
			bool handled = false;

			foreach (var b in buttons) if (b.CheckAndHandleTap(t)) handled = true;
			return handled;
		}


		public override void Draw()
		{
			foreach (var b in buttons) b.Draw();
			Canvas.Blit();
		}

	}
}
