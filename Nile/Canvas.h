#pragma once

#include "Point.h"
#include "Color.h"

using namespace System;

namespace Nile
{
	const int CANVAS_WIDTH = 1920;
	const int CANVAS_HEIGHT = 1080;

	public enum class BlendingModes
	{
		SOURCE_OVER = CAIRO_OPERATOR_OVER,
		ADDITIVE = CAIRO_OPERATOR_ADD
	};

	public ref class Canvas abstract sealed
	{

	public:

		// This won't be suitable for variable screen sizes.
		static const int Width = CANVAS_WIDTH;
		static const int Height = CANVAS_HEIGHT;

		static const int MaxX = Width - 1;
		static const int MaxY = Height - 1;

		static property Point Size
		{
			Point get()
			{
				return size;
			}

		}

		static property double StrokeWidth
		{
			double get()
			{
				return cairo_get_line_width(cr);
			}
		
			void set(double width)
			{
				cairo_set_line_width(cr, width);
			}
		}

		static property Color Source
		{
			void set(Color color)
			{
				cairo_set_source_rgba(cr, color.Red, color.Green, color.Blue, color.Alpha);
			}
		}

		static property BlendingModes BlendingMode
		{
			void set(BlendingModes mode)
			{
				cairo_set_operator(cr, (cairo_operator_t)mode);
			}
		}


		static void DrawLine(double x1, double y1, double x2, double y2);
		static void DrawLine(Point a, Point b);
		
		static void DrawCircle(double x, double y, double radius);
		static void DrawCircle(Point center, double radius);

		static void FillCircle(double x, double y, double radius);
		static void FillCircle(Point center, double radius);

		static void DrawRectangle(double x, double y, double width, double height);
		static void FillRectangle(double x, double y, double width, double height);

		static void MoveTo(Point a);
		static void MoveTo(double x, double y);

		static void LineTo(Point a);
		static void LineTo(double x, double y);

		static void RelativeMoveTo(Point a);
		static void RelativeMoveTo(double x, double y);

		static void RelativeLineTo(Point a);
		static void RelativeLineTo(double x, double y);


		static void CurveTo(Point b, Point c, Point d);

		static void ClosePath(void);

		static void NewSubPath();
		static void Arc(Point center, double radius, double startAngle, double endAngle);
		static void ArcNegative(Point center, double radius, double startAngle, double endAngle);

		static void Stroke(void);
		static void Fill(void);

		static void StrokePreserve(void);
		static void FillPreserve(void);

		static void Clear(void);
		static void Clear(Color c);
		static void Blit(void);

		static void SavePNG(String^ pathname);

		static void SaveState();
		static void RestoreState();

		static void Translate(Point p);
		static void Scale(Point p);
		static void Scale(double scale);
		static void Rotate(double angle);

		static void ShowText(String^ text);

	internal:

		static void Initialize(HDC wnd, RECT scr);
		static void Cleanup(void);

	private:

		static const Point size = Point(Width, Height);

		static bool initialized;

		static HDC hdcWnd;			// the HDC of the displayed window
		static HDC hdcMem;			// the HDC of the canvas

		static HBITMAP hbmMem;		// in-memory bitmap
		static RECT* rectptr;		// dimensions of the screenrect

		static cairo_t *cr;
		static cairo_surface_t *surface;
	
		static HBRUSH backbrush;
		//Color backcolor;

	};

}