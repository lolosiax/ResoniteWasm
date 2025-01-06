using System;
using System.Threading;
using System.Windows.Forms;

namespace ResoniteWasm.WinForm;

public class WinFormThread {
    private static readonly WinFormThread Instance = new();

    private readonly Thread mThread;
    private Form? mForm;

    private WinFormThread() {
        mThread = new Thread(ThreadStart);
        mThread.SetApartmentState(ApartmentState.STA);
        mThread.Start();
    }

    private void ThreadStart() {
        var form = new Form();
        form.Load += (_, _) => {
            form.BeginInvoke(() => form.Hide());
            mForm = form;
        };

        form.Opacity = 0;
        form.ShowInTaskbar = false;
        Application.Run(form);
    }

    public static class Global {
        public static bool IsUiThread { get => Thread.CurrentThread == Instance.mThread; }

        public static IAsyncResult RunOnUIThread(Action action) {
            while (Instance.mForm == null) {
                Thread.Sleep(100);
            }

            return Instance.mForm!.BeginInvoke(action);
        }
    }
}
