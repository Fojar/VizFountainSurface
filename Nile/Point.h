#pragma once

namespace Nile
{

	public value class Point
	{
	public:

		double X;
		double Y;


		Point(double X, double Y);

		void MoveToward(Point p, double fraction);
		void MoveAway(Point p, double fraction);

		property double Length
		{
			double get()
			{
				return sqrt(X * X + Y * Y);
			}
		}

		static Point operator * (Point p, double d);
		static Point operator / (Point p, double d);

		static Point operator + (Point a, Point b);
		static Point operator - (Point a, Point b);

		static void operator += (Point a, Point b);
		static void operator -= (Point a, Point b);

	};


}
