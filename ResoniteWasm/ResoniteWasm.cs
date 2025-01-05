using System;
using System.Linq;
using System.Reflection;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

using ResoniteWasm.Config;
using ResoniteWasm.Locale;

using Wasmtime;

using Engine = FrooxEngine.Engine;
using Module = Wasmtime.Module;
using WasmEngine = Wasmtime.Engine;

namespace ResoniteWasm;

//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
// ReSharper disable once ClassNeverInstantiated.Global
public class ResoniteWasm : ResoniteMod {
    public static ResoniteWasm ModInstance => mInstance!;
    private static ResoniteWasm? mInstance;

    public static ModConfiguration Config => mInstance!.GetConfiguration()!;

    internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
    public override string Name => "ResoniteWasm";
    public override string Author => "Lolosia";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://github.com/lolosiax/ResoniteWasm/";

    public override void OnEngineInit() {
        mInstance = this;

        Harmony harmony = new("top.lolosia.ResoniteWasm");
        harmony.PatchAll();

        Config.Save();

        using var engine = new WasmEngine();
        using var module = Module.FromText(
            engine,
            "hello",
            "(module (func $hello (import \"test\" \"hello\")) (func (export \"run\") (call $hello)))"
        );
        using var linker = new Linker(engine);
        using var store = new Store(engine);
        linker.Define(
            "test",
            "hello",
            Function.FromCallback(store, () => Msg("Hello from C#!"))
        );

        var instance = linker.Instantiate(store, module);
        var run = instance.GetAction("run")!;
        run();
        
        Engine.Current.RunPostInit(PostInit);

    }

    private void PostInit() {
        I18N.Instance.Initialize();
        
        Engine.Current.WorldManager.WorldFocused += world => {
            Msg($"Change to world, {world.Name}");
        };
    }

    public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder) {
        foreach (ModConfigurationKey it in ConfigDefinition.Items) {
            builder.Key(it);
        }
    }
}
