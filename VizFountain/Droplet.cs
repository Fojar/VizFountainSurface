using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Core;
using System.Collections.Concurrent;

namespace VizFountain
{
	abstract class Droplet
	{
		public static Program program;

		protected static Random RNG = new Random();
		protected const double TAU = Math.PI * 2;

		public virtual void Update() { }
		public virtual void Draw() { }

		public virtual void TouchDown(TouchPoint t) { }
		public virtual void TouchUp(TouchPoint t) { }
		public virtual void TouchTap(TouchPoint t) { }
		public virtual void TouchMove(TouchPoint t) { }

		public virtual void Input(ConcurrentDictionary<int, TouchPoint> touches) { }

		public virtual void TopRightTap() { }

		public void Exit() { program.Exit(); }
	}
}
