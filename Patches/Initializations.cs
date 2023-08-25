using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;

namespace DialogTest.Patches
{
    internal class Initializations
    {
        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCache_Patch
        {
            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix()
            {
                Main.Logger.Log("Initializing...");
                new CustomUnits.SuccubusThatTalks().Create();
            }
        }
    }
}
