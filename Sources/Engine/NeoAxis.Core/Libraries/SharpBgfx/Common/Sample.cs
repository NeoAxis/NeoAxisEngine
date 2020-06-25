using System;
using System.Threading;
using SharpBgfx;

namespace SharpBgfx.Common {
    public class Sample {
        Window window;
        Thread thread;

        public int WindowWidth {
            get;
            private set;
        }

        public int WindowHeight {
            get;
            private set;
        }

        public Sample (string name, int windowWidth, int windowHeight) {
            Thread.CurrentThread.Name = "OS Thread";

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            window = new Window(name, windowWidth, windowHeight);
            window.Show();

            Bgfx.SetWindowHandle(window.Handle);
        }

        public void Run (Action<Sample> renderThread) {
            thread = new Thread(() => {
                renderThread(this);
                window.Close();
            });
            thread.Start();

            // run the native OS message loop on this thread
            // this blocks until the window closes and the loop exits
            window.RunMessageLoop();

            // wait for the render thread to finish
            thread.Join();
        }

        public bool ProcessEvents(ResetFlags resetFlags) {
            WindowEvent? ev;
            var reset = false;

            while ((ev = window.Poll()) != null) {
                var e = ev.Value;
                switch (e.Type) {
                    case WindowEventType.Exit:
                        return false;

                    case WindowEventType.Size:
                        WindowWidth = e.Width;
                        WindowHeight = e.Height;
                        reset = true;
                        break;
                }
            }

            if (reset)
                Bgfx.Reset(WindowWidth, WindowHeight, resetFlags);

            return true;
        }
    }
}
