using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nile;

namespace VizFountain.Droplets
{
	class Sparkles : Droplet
	{



		class Spark
		{
			public static Sparkles parent;
			public static bool Spawn = true;

			Point pos;
			double speed;

			double angle;
			double curl;

			Color color, faintcolor;

			public Spark(Point pos)
			{
				this.pos = pos;
				color = Color.FromHSVA(RNG.NextDouble(0, TAU), .5, .5, .03);
				faintcolor = new Color(color.Red, color.Green, color.Blue, .02);

				Init();
			}

			public Spark(Spark other)
			{
				pos = other.pos;
				color = other.color;
				faintcolor = other.faintcolor;

				Init();
			}

			void Init()
			{
				angle = RNG.NextDouble(0, TAU);
				speed = RNG.NextDouble(.4, .9);
				curl = RNG.NextDouble(-1, 1) * .01;
			}

			public void Draw()
			{
				Update();

				Canvas.Source = color;
				if (RNG.NextDouble() < .5) Canvas.FillCircle(pos, 1);
			}

			public void Update()
			{
				curl *= 1.002;
				angle += curl;
				//speed *= .999;

				pos.X += Math.Cos(angle) * speed;
				pos.Y += Math.Sin(angle) * speed;

				if (Spawn && RNG.NextDouble() < .005)
				{
					Canvas.Source = faintcolor;
					Canvas.FillCircle(pos, 35);
					parent.sparks.Add(new Spark(this));
				}
			}

			public bool IsDead
			{
				get { return Math.Abs(curl) > 100; }
			}
		}




		List<Spark> sparks;

		public Sparkles()
		{
			Spark.parent = this;

			sparks = new List<Spark>();
			for (int i = 0; i < 5; i++) sparks.Add(new Spark(new Point(RNG.NextDouble() * Canvas.Width, RNG.NextDouble() * Canvas.Height)));

			Canvas.Clear(new Color(0, 0, 0));
			Canvas.BlendingMode = BlendingModes.ADDITIVE;
		}


		public override void Draw()
		{
			//Canvas.Clear();

			for (int i = 0; i < sparks.Count; i++) sparks[i].Draw();

			sparks.RemoveAll(s => s.IsDead);

			if (sparks.Count > 200) Spark.Spawn = false;
			else if (sparks.Count == 0)
			{
				sparks.Add(new Spark(new Point(RNG.NextDouble() * Canvas.Width, RNG.NextDouble() * Canvas.Height)));
				Spark.Spawn = true;
			}
			
			Canvas.Blit();
		}





	}
}
