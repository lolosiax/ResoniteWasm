﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using FrooxEngine;

using HarmonyLib;

using Newtonsoft.Json;

using ResoniteWasm.Config;
using ResoniteWasm.Util;

namespace ResoniteWasm.Locale;

public class I18N {
    public static I18N Instance { get; private set; } = new();

    private readonly Dictionary<string, string> mLocateMap = new();


    public void Initialize() {
        Settings.RegisterValueChanges<LocaleSettings>(AfterLanguageChanged);
        AfterLanguageChanged(null);
    }

    public string GetValue(string id) {
        return mLocateMap.TryGetValue(id, out string? value) ? value : id;
    }

    private void LoadLocaleMap() {
        var ls = Settings.GetActiveSetting<LocaleSettings>();
        LoadLocaleMap(ls.ActiveLocaleCode);
    }

    private void LoadLocaleMap(string code) {
        Msg($"Active Language: {code}");
        code = code.ToLower();

        mLocateMap.Clear();

        // The default fallback language is machine translation,
        // and it is recommended that you contribute a human-translated version.
        // Do not change "en-fallback" here.
        var en = GetText("en-fallback")!;
        foreach (KeyValuePair<string, string> it in en) {
            mLocateMap[it.Key] = it.Value;
        }

        // TODO: Replace traditional Chinese with simplified Chinese until someone else contributes
        // 在有人贡献繁体中文前，以简体中文替代
        if (code == "zh-cn") code = "zh-cn";

        var local = GetText(code);
        // if language not found, skip.
        if (local == null) return;
        foreach (KeyValuePair<string, string> it in local) {
            if (mLocateMap.ContainsKey(it.Key)) mLocateMap.Remove(it.Key);
            mLocateMap[it.Key] = it.Value;
        }
    }

    private Dictionary<string, string>? GetText(string localCode) {
        var assembly = Assembly.GetAssembly(typeof(I18N));
        try {
            using var fs = assembly.GetManifestResourceStream($"ResoniteWasm.Locale.{localCode}.json")!;
            using var sr = new StreamReader(fs);
            var text = sr.ReadToEnd();
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            return dict;
        } catch (Exception e) {
            Warn($"Language {localCode} not found in resources", e);
            return null;
        }
    }

    private void AfterLanguageChanged(LocaleSettings? locale) {
        LoadLocaleMap(locale?.ActiveLocaleCode ?? "en");
        ConfigDefinition.ChangeLanguage();
        ConsoleLogger.ChangeLanguage();
    }
}

public static class I18NGlobal {
    public static string Local(this string id, params object[] args) {
        string str = I18N.Instance.GetValue(id);
        return args.Length == 0 ? str : string.Format(str, args);
    }
}
