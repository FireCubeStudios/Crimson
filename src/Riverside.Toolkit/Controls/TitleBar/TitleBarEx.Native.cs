﻿using Riverside.ComponentModel;
using System;
using System.Runtime.InteropServices;
using static Riverside.Toolkit.Helpers.NativeHelper;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        // Window messages
        private const int WM_NCLBUTTONDOWN = 0x00A1;      // Non client left button down
        private const int WM_NCHITTEST = 0x0084;          // Non client hit test
        private const int WM_NCLBUTTONUP = 0x00A2;        // Non client left button up
        private const int WM_NCRBUTTONUP = 0x00A5;        // Non client right button up
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;    // Non client left button double click
        private const int WM_ACTIVATE = 0x0006;           // Activate

        // NCHITTEST response
        private const int HTMINBUTTON = 8;                // Hit test minimize button
        private const int HTMAXBUTTON = 9;                // Hit test maximize button
        private const int HTCLOSE = 20;                   // Hit test close button
        private const int HTBORDER = 18;                  // Hit test border (for no interaction)

        // Window frame
        private const int WND_FRAME_LEFT = 7;             // Window frame width
        private const int WND_FRAME_TOP_MAXIMIZED = 7;    // Top window frame (it goes up by 6px when maximized because of how Windows works)
        private const int WND_FRAME_TOP_NORMAL = 1;       // Top window frame (not maximized)

        // Mouse events
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002; // Left button down
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;   // Left button up

        // Others
        private const int WA_INACTIVE = 0;                // Activate (inactive)
        private const int VK_LBUTTON = 0x01;              // Virtual key code for the left mouse button
        private const uint INPUT_MOUSE = 0;               // Mouse input

        private static bool IsLeftMouseButtonDown()
        {
            // The high-order bit indicates if the key is down
            return (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        private static void SimulateMouseButtonAction(uint mouseEventFlag)
        {
            INPUT input = new()
            {
                type = (int)INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dwFlags = mouseEventFlag
                }
            };

            _ = SendInput(1, [input], Marshal.SizeOf(typeof(INPUT)));
        }

        public static void SimulateLeftButtonDown() => SimulateMouseButtonAction(MOUSEEVENTF_LEFTDOWN);

        public static void SimulateLeftButtonUp() => SimulateMouseButtonAction(MOUSEEVENTF_LEFTUP);

        public void MoveCursorUp()
        {
            if (!GetCursorPos(out POINT currentPos)) return;

            // Simulate mouse button action
            SimulateLeftButtonUp();

            // Move the cursor up by 6 pixels
            SetCursorPos(currentPos.X, currentPos.Y - (int)(6 * Display.Scale(CurrentWindow)));

            SimulateLeftButtonDown();
        }

        // Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // Native methods

        [DllImport(Libraries.User32)]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport(Libraries.User32)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport(Libraries.User32)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport(Libraries.User32)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}