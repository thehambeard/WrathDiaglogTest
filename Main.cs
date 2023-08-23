using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
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

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            Logger = modEntry.Logger;

            Harmony = new(modEntry.Info.Id);
            Harmony.PatchAll();

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (GUILayout.Button("test"))
            {
                Game.Instance.EntityCreator.SpawnUnit(ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("7001e2a58c9e86e43b679eda8a59f12f"), Game.Instance.Player.MainCharacter.Value.Position, Quaternion.identity, Game.Instance.CurrentScene.MainState);
            }
        }
    }
}
