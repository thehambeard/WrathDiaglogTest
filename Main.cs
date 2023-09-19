using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.ResourceManagement;
using Kingmaker.Utility;
using Kingmaker.View;
using UnityEngine;
using UnityModManagerNet;

namespace DialogTest
{
#if (DEBUG)
    [EnableReloading]
#endif
    static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        private static Harmony Harmony;
        private static int index = 0;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            Logger = modEntry.Logger;

            Harmony = new(modEntry.Info.Id);
            Harmony.PatchAll();

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float value)
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                var bp = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("2db556136eac2544fa9744314c2a5713");
                var vars = bp.CustomizationPreset.UnitVariations.FirstOrDefault().Variations;

                if (index < vars.Count)
                {
                    var pc = Game.Instance.Player.MainCharacter.Value;
                    var prefab = BundledResourceHandle<UnitEntityView>.Request(vars[index].Prefab.AssetId); // e6ec3993da548c24ea9fa1448c2f68f7 UnitEntityView link is what you would put in the request.

                    Logger.Log($"Spawning Variation {index}: {vars[index].Prefab.AssetId}");

                    var unit = Game.Instance.EntityCreator.SpawnUnit(bp, prefab.Object, pc.Position, Quaternion.LookRotation(pc.OrientationDirection), Game.Instance.CurrentScene.MainState);
                    unit.SwitchFactions(ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("d64258e86eeb1d8479f35a9b16f6590a"), true);
                    index++;
                }
            }
        }
    }
}
