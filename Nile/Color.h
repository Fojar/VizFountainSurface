#pragma once

namespace Nile
{

	public value class Color
	{
	public:

		double Red;
		double Green;
		double Blue;
		double Alpha;


		Color(double Red, double Green, double Blue);
		Color(double Red, double Green, double Blue, double Alpha);

		static Color FromHSV(double Hue, double Saturation, double Value);
		static Color FromHSVA(double Hue, double Saturation, double Value, double Alpha);

	};


}