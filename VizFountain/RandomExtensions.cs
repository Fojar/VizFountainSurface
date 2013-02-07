using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Surface.Core;

using Nile;

namespace VizFountain
{
	public static class Extensions
	{
		public static double NextDouble(this Random RNG, double min, double max)
		{
			double range = max - min;
			return min + RNG.NextDouble() * range;
		}

		public static double NextDouble(this Random RNG, double range)
		{
			return RNG.NextDouble() * range;
		}

		public static Point NextPoint(this Random RNG, Point range)
		{
			return new Point(RNG.NextDouble() * range.X, RNG.NextDouble() * range.Y);
		}

		public static Point ToPoint(this TouchPoint t)
		{
			return new Point(t.CenterX, t.CenterY);
		}

		//-----------------------------------------------------------------------------------------
		public static float ToAngle(this Point vec)
		{
			return (float)Math.Atan2(vec.Y, vec.X);
		}

	}
}
