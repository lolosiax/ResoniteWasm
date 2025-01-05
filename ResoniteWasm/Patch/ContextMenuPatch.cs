using System.Diagnostics.CodeAnalysis;

using FrooxEngine;

using HarmonyLib;

namespace ResoniteWasm.Patch;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ContextMenuPatch {
    
    [HarmonyPatch(typeof(ContextMenu), "OpenMenuIntern")]
    public static class OpenMenuIntern {

        [HarmonyPostfix]
        public static void Postfix(ContextMenu __instance) {
            var that = __instance;
            Msg("Open the Menu!");
        }
    }
}
