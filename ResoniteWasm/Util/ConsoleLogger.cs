using System;

using Elements.Core;

using ResoniteWasm.Config;
using ResoniteWasm.WinForm;

namespace ResoniteWasm.Util;

public static class ConsoleLogger {
    public static bool ShowLog { get; private set; }
    private static LogForm? mLogForm;
    private static readonly object Lock = new();

    public static void Initialize() {
        UniLog.OnLog += OnLog;
        UniLog.OnWarning += OnWarning;
        UniLog.OnError += OnError;

        ModConfig.OnThisConfigurationChanged += e => {
            if (e.Key.Name == ConfigDefinition.ShowConsoleLog.Name) {
                ShowLog = ModConfig.GetValue(ConfigDefinition.ShowConsoleLog);
                OnShowLogChange();
            }
        };
        ShowLog = ModConfig.GetValue(ConfigDefinition.ShowConsoleLog);
        OnShowLogChange();
    }

    public static void ChangeLanguage() {
        RunOnUIThread(() => mLogForm?.ChangeLanguage());
    }

    private static void OnShowLogChange() {
        lock (Lock) {
            if (ShowLog && mLogForm == null) {
                RunOnUIThread(() => {
                    mLogForm = new LogForm();
                    mLogForm.Show();
                }).AsyncWaitHandle.WaitOne();
            } else if (!ShowLog) {
                RunOnUIThread(() => {
                    mLogForm?.Close();
                    mLogForm = null;
                }).AsyncWaitHandle.WaitOne();
            }
        }
    }

    private static void OnLog(string obj) {
        if (!ShowLog) return;
        mLogForm?.OnLog(obj);
    }

    private static void OnWarning(string obj) {
        if (!ShowLog) return;
        mLogForm?.OnWarning(obj);
    }

    private static void OnError(string obj) {
        if (!ShowLog) return;
        mLogForm?.OnError(obj);
    }
}
