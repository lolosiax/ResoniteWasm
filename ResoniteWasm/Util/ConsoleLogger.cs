using System;

using Elements.Core;

using ResoniteWasm.Config;

namespace ResoniteWasm.Util;

// The standard output stream is strangely ineffective. Why?
public static class ConsoleLogger {
    private static bool mShowLog = false;

    public static void Initialize() {
        UniLog.OnLog += OnLog;
        UniLog.OnWarning += OnWarning;
        UniLog.OnError += OnError;

        ModConfig.OnThisConfigurationChanged += e => {
            if (e.Key.Name == ConfigDefinition.ShowConsoleLog.Name) {
                mShowLog = ModConfig.GetValue(ConfigDefinition.ShowConsoleLog);
            }
        };
        mShowLog = ModConfig.GetValue(ConfigDefinition.ShowConsoleLog);
    }

    private static void OnLog(string obj) {
        if (!mShowLog) return;
        Console.Out.WriteLine("\x1b[0m" + obj);
    }

    private static void OnWarning(string obj) {
        if (!mShowLog) return;
        Console.Out.WriteLine("\x1b[33;1m" + obj);
    }

    private static void OnError(string obj) {
        if (!mShowLog) return;
        Console.Error.WriteLine("\x1b[31;1m" + obj);
    }
}
