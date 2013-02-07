#include "StdAfx.h"

#include "Color.h"


namespace Nile
{

	Color::Color(double Red, double Green, double Blue)
		: Red(Red), Green(Green), Blue(Blue), Alpha(1)
	{}

	Color::Color(double Red, double Green, double Blue, double Alpha)
		: Red(Red), Green(Green), Blue(Blue), Alpha(Alpha)
	{}


	Color Color::FromHSVA(double Hue, double Saturation, double Value, double Alpha)
	{
		if (Saturation > 1.0) Saturation = 1.0;
		if (Saturation < 0.0) Saturation = 0.0;

		if (Value > 1.0) Value = 1.0;
		if (Value < 0.0) Value = 0.0;

		// achromatic
		if (Saturation == 0) return Color(Value, Value, Value, Alpha);

		// Hue is given in radians. It could take any value, allowing for wraparound.
		// We must first constrain it to the range [0, TAU)...

		double h = fmod(Hue, TAU);
		if (h < 0) h += TAU;
	
		// ...and then scale it to the range [0, 6)
		h *= 6 / TAU;

		// get the integral and fractional portions of this new scaled Hue
		int i = (int)floor(h);
		double f = h - i;

		// if the integral portion is even, set f to be 1 - f 
		if (!(i & 1)) f = 1 - f;

		double m, n;
		m = Value * (1 - Saturation);
		n = Value * (1 - Saturation * f);

		switch (i)
		{
		case 0:
			return Color(Value, n, m, Alpha);
		case 1:
			return Color(n, Value, m, Alpha);
		case 2:
			return Color(m, Value, n, Alpha);
		case 3:
			return Color(m, n, Value, Alpha);
		case 4:
			return Color(n, m, Value, Alpha);
		case 5:
			return Color(Value, m, n, Alpha);
		}
	}

	Color Color::FromHSV(double Hue, double Saturation, double Value)
	{
		return FromHSVA(Hue, Saturation, Value, 1);
	}
	


}