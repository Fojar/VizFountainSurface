#pragma once

namespace Nile {

	public ref class ProgramBase
	{
	public:

		ProgramBase(void);

		int		Run(void);

		virtual void	Initialize(void);
		virtual void	Update(void);
		virtual void	Draw(void);

		void* GetWindowHandle();

		void Exit(void);

	private:

		LPCWSTR		winClassName;

		HINSTANCE	hinstance;
		HWND		hwnd;
		HDC			hdc;

		bool	exit_requested;

		bool	MakeWindow(void);
		int		DoMainLoop(void);
		void	BreakWindow(void);

		bool	HandlePendingMessages(void);

		//static LRESULT CALLBACK WinProc(HWND, UINT, WPARAM, LPARAM);

	};

	LRESULT CALLBACK WinProc(HWND, UINT, WPARAM, LPARAM);
}
