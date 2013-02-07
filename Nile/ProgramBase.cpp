// This is the main DLL file.

#include "stdafx.h"

#include "ProgramBase.h"
#include "Canvas.h"

namespace Nile
{

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	ProgramBase::ProgramBase(void)
	{
		winClassName = L"VizFountain";
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	int ProgramBase::Run(void)
	{
		

		int retval = 0;

		if (MakeWindow())
		{
			ShowWindow(hwnd, SW_SHOWNORMAL);
			retval = DoMainLoop();
		}
		BreakWindow();

		return retval;
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	bool ProgramBase::MakeWindow(void)
	{
		hinstance = GetModuleHandle(nullptr);

		RECT rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = CANVAS_WIDTH;
		rect.bottom = CANVAS_HEIGHT;

		// ---- create the window ----

		WNDCLASSEX wc;     // the extended window class

		wc.cbSize           = sizeof(WNDCLASSEX);
		wc.style			= CS_OWNDC;		// get a persistent DC for speed
		wc.lpfnWndProc		= WinProc;
		wc.cbClsExtra		= 0;
		wc.cbWndExtra		= 0;
		wc.hInstance		= hinstance;
		wc.hIcon			= LoadIcon(nullptr, IDI_APPLICATION);
		wc.hCursor			= LoadCursor(nullptr, IDC_ARROW);
		wc.hbrBackground	= nullptr;
		wc.lpszMenuName		= nullptr;
		wc.lpszClassName	= winClassName;
		wc.hIconSm			= LoadIcon(nullptr, IDI_APPLICATION);

		RegisterClassEx(&wc);

		hwnd = CreateWindowEx(
			0,						// extended style
			winClassName,			// class name, previously registered
			winClassName,			// title
			WS_POPUP,				// basic style
			0, 0,					// top left coordinate
			rect.right,
			rect.bottom,
			nullptr,				// parent
			nullptr,				// menu
			hinstance,
			nullptr					// used only for MDI
			);

		if (hwnd == nullptr) return false;

		hdc = GetDC(hwnd);

		Canvas::Initialize(hdc, rect);

		// call the user's initialize method
		Initialize();

		return true;
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	int ProgramBase::DoMainLoop(void)
	{
		HandlePendingMessages();

		int total_updates = 0;
		int total_draws = 0;
	
		INT64 frequency;	// the number of ticks per second
		QueryPerformanceFrequency((LARGE_INTEGER*)&frequency);

		// track how much time is in main loop, and in update and draw methods
		INT64 TotalTicks = 0;
		INT64 UpdateTicks = 0;
		INT64 DrawTicks = 0;

		INT64 timedebt = 0;

		INT64 now = 0, then = 0;	// used to count time between successive calls to update/draw
		INT64 start, finish;

		//int oldmousex = input.mousex, oldmousey = input.mousey;
		//input.mousedz = 0;

		QueryPerformanceCounter((LARGE_INTEGER*)&then);
		start = then;
		
		while (!exit_requested)
		{
			float fraction = 0;
			if (timedebt < 0) fraction = 1 + (float)(timedebt) / frequency;

			Draw();
			total_draws++;			

			if (exit_requested) break;

			QueryPerformanceCounter((LARGE_INTEGER*)&now);

			const int SIM_RATE = 60;
			timedebt += (now - then) * SIM_RATE;		// by drawing (and indeed updating too!) we incur a time debt

			// now we pay back the time debt by doing updates (if necessary)
			if (timedebt > 0)
			{
				// determine how many updates we need to do
				int updatesneeded = int(timedebt / frequency + 1);
				if (updatesneeded > SIM_RATE)
				{
					// a time debt greater than 1s is considered a bankruptcy
					// we reset the debt to allow only 1 update and carry on
					timedebt = frequency;
				}
			}

			while (timedebt > 0 && !exit_requested)
			{
				exit_requested = HandlePendingMessages();
				if (exit_requested) break;

				//input.mousedx = input.mousex - oldmousex;
				//input.mousedy = input.mousey - oldmousey;
				
				Update();
				total_updates++;

				timedebt -= frequency;

				//oldmousex = input.mousex;
				//oldmousey = input.mousey;
				//input.mousedz = 0;
			}

			then = now;
		}
		finish = now;

		return 0;	// this was supposed to be the wParam of the WM_QUIT message, but it always seems to be 0 anyway
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void ProgramBase::BreakWindow(void)
	{
		Canvas::Cleanup();

		if (hdc)
		{
			ReleaseDC(hwnd, hdc);
			hdc = NULL;
		}

		if (hwnd)
		{
			DestroyWindow(hwnd);
			hwnd = NULL;
		}

		if (hinstance)
		{
			UnregisterClass(winClassName, hinstance);
			hinstance = NULL;
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	bool ProgramBase::HandlePendingMessages(void)
	{
		MSG message;

		// while there is some message available...
		while (PeekMessage(&message, NULL, 0, 0, PM_REMOVE))
		{
			//...process it
			if (message.message == WM_QUIT) return true;
			else DispatchMessage(&message);
		}
		return false;	// this means don't quit yet
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	LRESULT CALLBACK WinProc(HWND window, UINT message, WPARAM wParam, LPARAM lParam)
	{
		switch (message)
		{
		case WM_ERASEBKGND:
			return 1;	// claim that we took care of it

		case WM_PAINT:
			// don't do any painting but validate the region
			PAINTSTRUCT ps;
			BeginPaint(window, &ps);
			EndPaint(window, &ps);
			return 0;

		case WM_CLOSE:
			PostQuitMessage(0);
			return 0;

		default:
			return (DefWindowProc(window, message, wParam, lParam));
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


	void ProgramBase::Initialize(void) {}

	void ProgramBase::Update(void) {}

	void ProgramBase::Draw(void)
	{
		Canvas::Clear();
		Canvas::Blit();
	}

	void* ProgramBase::GetWindowHandle()
	{
		return hwnd;
	}


	void ProgramBase::Exit(void)
	{
		exit_requested = true;
	}

}