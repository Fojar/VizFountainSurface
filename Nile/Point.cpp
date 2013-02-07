#include "StdAfx.h"

#include "Point.h"


namespace Nile
{

	Point::Point(double X, double Y)
		: X(X), Y(Y)
	{}


	void Point::MoveToward(Point p, double fraction)
	{
		X += (p.X - X) * fraction;
		Y += (p.Y - Y) * fraction;
	}

	void Point::MoveAway(Point p, double fraction)
	{
		X -= (p.X - X) * fraction;
		Y -= (p.Y - Y) * fraction;
	}

	Point Point::operator * (Point p, double d)
    {
		return (Point(p.X * d, p.Y * d));
    }

	Point Point::operator / (Point p, double d)
    {
		return (Point(p.X / d, p.Y / d));
    }

	Point Point::operator + (Point a, Point b)
    {
		return (Point(a.X + b.X, a.Y + b.Y));
    }

	Point Point::operator - (Point a, Point b)
    {
		return (Point(a.X - b.X, a.Y - b.Y));
    }

	void Point::operator += (Point a, Point b)
    {
		a.X += b.X;
		a.Y += b.Y;
    }

	void Point::operator -= (Point a, Point b)
    {
		a.X -= b.X;
		a.Y -= b.Y;
    }


}