using System;
using System.Linq;
using System.Reflection;

using Elements.Core;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

using static System.Reflection.BindingFlags;

using DummyKey = ResoniteModLoader.ModConfigurationKey<Elements.Core.dummy>;

namespace ResoniteWasm.Config;

public class ConfigDefinition {
    [AutoRegisterConfigKey] public static DummyKey LanguageTip = Dummy("LanguageNotFound");
    [AutoRegisterConfigKey] public static DummyKey Sp1 = SplitLine();
    [AutoRegisterConfigKey] public static ModConfigurationKey<bool> ShowConsoleLog = Create("ShowConsoleLog", false);

    private static ModConfigurationKey[]? mItems;
    private static int mSplitLineIndex;

    public static ModConfigurationKey[] Items {
        get {
            if (mItems != null) return mItems;

            mItems = AccessTools.GetDeclaredFields(typeof(ConfigDefinition))
                .Where<FieldInfo>(it =>
                    Attribute.GetCustomAttribute(it, typeof(AutoRegisterConfigKeyAttribute)) != null
                    && typeof(ModConfigurationKey).IsAssignableFrom(it.FieldType)
                )
                .Select(it => (ModConfigurationKey)it.GetValue(null))
                .Where(it => it != null)
                .ToArray();
            return mItems;
        }
    }

    private static ModConfigurationKey<dummy> Dummy(string name) {
        return new ModConfigurationKey<dummy>(name);
    }

    private static ModConfigurationKey<dummy> SplitLine() {
        return new ModConfigurationKey<dummy>(
            $"SplitLine{mSplitLineIndex++}",
            "".PadLeft(129, '-')
        );
    }

    private static ModConfigurationKey<T> Create<T>(string name, T defaultValue) {
        return new ModConfigurationKey<T>(name, "", () => defaultValue);
    }

    private static ModConfigurationKey<T> Create<T>(string name, Func<T> defaultValue) {
        return new ModConfigurationKey<T>(name, "", defaultValue);
    }

    public static void ChangeLanguage() {
        MethodInfo method = typeof(ModConfigurationKey)
            .GetProperty("Description", Public | NonPublic | Instance)
            !.SetMethod;

        foreach (ModConfigurationKey it in Items) {
            if (it.Name.StartsWith("SplitLine")) continue;
            var key = $"Config.{it.Name}.Name";

            method.Invoke(it, new object[] { key.Local() });
        }
    }
}
