using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nile;

using Microsoft.Surface;
using Microsoft.Surface.Core;
using System.Diagnostics;

using VizFountain.Droplets;
using System.Collections.Concurrent;

namespace VizFountain
{
	class Program : ProgramBase
	{
		Droplet CurrentDroplet;

		public override void Initialize()
		{
			Droplet.program = this;

			Console.WriteLine("Program.Initialize()");
			InitializeSurfaceInput();

			Droplets.Add(new Droplets.Sketchy());
			CurrentDroplet = Droplets[0];
		}


		ConcurrentDictionary<int, TouchPoint> touchDict = new ConcurrentDictionary<int, TouchPoint>();

		ConcurrentQueue<TouchPoint> touchQueue = new ConcurrentQueue<TouchPoint>();

		/// <summary>
		/// Initializes the surface input system. This should be called after any window
		/// initialization is done, and should only be called once.
		/// </summary>
		private void InitializeSurfaceInput()
		{

			// Create a target for surface input.

			IntPtr handle;
			unsafe
			{
				handle = (IntPtr)GetWindowHandle();
			}
			touchTarget = new TouchTarget(handle, EventThreadChoice.OnBackgroundThread);
			touchTarget.EnableInput();

			touchTarget.TouchTapGesture += new EventHandler<TouchEventArgs>(touchTarget_TouchTapGesture);
			touchTarget.TouchMove += new EventHandler<TouchEventArgs>(touchTarget_TouchMove);
			touchTarget.TouchDown += new EventHandler<TouchEventArgs>(touchTarget_TouchDown);
			touchTarget.TouchUp += new EventHandler<TouchEventArgs>(touchTarget_TouchUp);

		}

		void touchTarget_TouchUp(object sender, TouchEventArgs e)
		{
			TouchPoint t;
			touchDict.TryRemove(e.TouchPoint.Id, out t);
		}

		void touchTarget_TouchDown(object sender, TouchEventArgs e)
		{
			touchQueue.Enqueue(e.TouchPoint);
			touchDict[e.TouchPoint.Id] = e.TouchPoint;
		}

		void touchTarget_TouchMove(object sender, TouchEventArgs e)
		{
			int id = e.TouchPoint.Id;
			var oldPoint = touchDict[id].ToPoint();
			var newPoint = e.TouchPoint.ToPoint();

			if ((oldPoint - newPoint).Length > 10)
			{
				touchQueue.Enqueue(e.TouchPoint);
				touchDict[e.TouchPoint.Id] = e.TouchPoint;
			}
		}

		void touchTarget_TouchTapGesture(object sender, TouchEventArgs e)
		{
		}

		private TouchTarget touchTarget;


		List<Droplet> Droplets = new List<Droplet>();



		public override void Update()
		{
			Point TOP_LEFT = new Point(0, 0);

			TouchPoint t;
			var start = DateTime.Now;
			TimeSpan MAX_INTERVAL = TimeSpan.FromMilliseconds(10);

			while ((DateTime.Now - start) < MAX_INTERVAL && touchQueue.TryDequeue(out t))
			{
				Point p = t.ToPoint();

				if ((p - TOP_LEFT).Length < 20) Exit();
				else CurrentDroplet.TouchTap(t);
			}

			CurrentDroplet.Input(touchDict);
			CurrentDroplet.Update();
		}

		public override void Draw()
		{
			CurrentDroplet.Draw();
		}

		static void Main()
		{
			new Program().Run();
		}
	}
}
