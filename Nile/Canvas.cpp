#include "StdAfx.h"

#include "Canvas.h"

using namespace System::Runtime::InteropServices;

namespace Nile
{


	void Canvas::Initialize(HDC wnd, RECT scr)
	{
		if (initialized) return;

		hdcWnd = wnd;
		rectptr = new RECT();
		*rectptr = scr;

		hdcMem = CreateCompatibleDC(hdcWnd);
		hbmMem = CreateCompatibleBitmap(hdcWnd, Width, Height);
		SelectObject(hdcMem, hbmMem);

		backbrush = CreateSolidBrush(RGB(255, 255, 255));

		surface = cairo_win32_surface_create(hdcMem);
		cr = cairo_create(surface);

		initialized = true;
	}


	void Canvas::Cleanup(void)
	{
		if (!initialized) return;

		DeleteObject(backbrush);
		cairo_surface_destroy(surface);
		cairo_destroy(cr);
		DeleteObject(hbmMem);
		delete rectptr;

		initialized = false;
	}


	void Canvas::DrawLine(double x1, double y1, double x2, double y2)
	{
		cairo_move_to(cr, x1, y1);
		cairo_line_to(cr, x2, y2);
		cairo_stroke(cr);
	}

	void Canvas::DrawLine(Point a, Point b)
	{
		cairo_move_to(cr, a.X, a.Y);
		cairo_line_to(cr, b.X, b.Y);
		cairo_stroke(cr);
	}

	void Canvas::DrawCircle(double x, double y, double radius)
	{
		cairo_arc(cr, x, y, radius, 0, TAU);
		cairo_stroke(cr);
	}

	void Canvas::DrawCircle(Point center, double radius)
	{
		cairo_arc(cr, center.X, center.Y, radius, 0, TAU);
		cairo_stroke(cr);
	}


	void Canvas::FillCircle(double x, double y, double radius)
	{
		cairo_arc(cr, x, y, radius, 0, TAU);
		cairo_fill(cr);
	}

	void Canvas::FillCircle(Point center, double radius)
	{
		cairo_arc(cr, center.X, center.Y, radius, 0, TAU);
		cairo_fill(cr);
	}


	void Canvas::DrawRectangle(double x, double y, double width, double height)
	{
		cairo_rectangle(cr, x, y, width, height);
		cairo_stroke(cr);
	}

	void Canvas::FillRectangle(double x, double y, double width, double height)
	{
		cairo_rectangle(cr, x, y, width, height);
		cairo_fill(cr);

	}


	void Canvas::MoveTo(Point a)
	{
		cairo_move_to(cr, a.X, a.Y);
	}

	void Canvas::MoveTo(double x, double y)
	{
		cairo_move_to(cr, x, y);
	}

	void Canvas::LineTo(Point a)
	{
		cairo_line_to(cr, a.X, a.Y);
	}

	void Canvas::LineTo(double x, double y)
	{
		cairo_line_to(cr, x, y);
	}



	void Canvas::RelativeMoveTo(Point a)
	{
		cairo_rel_move_to(cr, a.X, a.Y);
	}

	void Canvas::RelativeMoveTo(double x, double y)
	{
		cairo_rel_move_to(cr, x, y);
	}

	void Canvas::RelativeLineTo(Point a)
	{
		cairo_rel_line_to(cr, a.X, a.Y);
	}

	void Canvas::RelativeLineTo(double x, double y)
	{
		cairo_rel_line_to(cr, x, y);
	}



	void Canvas::CurveTo(Point b, Point c, Point d)
	{
		cairo_curve_to(cr, b.X, b.Y, c.X, c.Y, d.X, d.Y);
	}

	void Canvas::ClosePath(void)
	{
		cairo_close_path(cr);
	}

	void Canvas::NewSubPath()
	{
		cairo_new_sub_path(cr);
	}

	void Canvas::Arc(Point center, double radius, double startAngle, double endAngle)
	{
		cairo_arc(cr, center.X, center.Y, radius, startAngle, endAngle);
	}

	void Canvas::ArcNegative(Point center, double radius, double startAngle, double endAngle)
	{
		cairo_arc_negative(cr, center.X, center.Y, radius, startAngle, endAngle);
	}


	void Canvas::Stroke(void)
	{
		cairo_stroke(cr);
	}

	void Canvas::Fill(void)
	{
		cairo_fill(cr);
	}

	void Canvas::StrokePreserve(void)
	{
		cairo_stroke_preserve(cr);
	}

	void Canvas::FillPreserve(void)
	{
		cairo_fill_preserve(cr);
	}


	void Canvas::SaveState()
	{
		cairo_save(cr);
	}

	void Canvas::RestoreState()
	{
		cairo_restore(cr);
	}

	void Canvas::Translate(Point p)
	{
		cairo_translate(cr, p.X, p.Y);
	}

	void Canvas::Scale(Point p)
	{}

	void Canvas::Scale(double scale)
	{
		cairo_scale(cr, scale, scale);
	}

	void Canvas::Rotate(double angle)
	{
		cairo_rotate(cr, angle);
	}


	void Canvas::Blit(void)
	{
		BitBlt(hdcWnd, 0, 0, Width, Height, hdcMem, 0, 0, SRCCOPY);
	}


	void Canvas::Clear(void)
	{
		FillRect(hdcMem, rectptr, backbrush);
	}

	void Canvas::Clear(Color c)
	{
		DeleteObject(backbrush);
		backbrush = CreateSolidBrush(RGB(c.Red * 255, c.Green * 255, c.Blue * 255));

		FillRect(hdcMem, rectptr, backbrush);
	}


	void Canvas::SavePNG(String^ pathname)
	{
		char* ansi_pathname = (char*)Marshal::StringToHGlobalAnsi(pathname).ToPointer();
		cairo_status_t result = cairo_surface_write_to_png(surface, ansi_pathname);
		Marshal::FreeHGlobal((IntPtr)ansi_pathname);

		if (result == CAIRO_STATUS_WRITE_ERROR)
		{
			throw gcnew System::Exception("Error while attempting to write PNG.");
		}
	}


	void Canvas::ShowText(String^ text)
	{
		char* ansi_text = (char*)Marshal::StringToHGlobalAnsi(text).ToPointer();
		cairo_show_text(cr, ansi_text);
		Marshal::FreeHGlobal((IntPtr)ansi_text);
	}


}