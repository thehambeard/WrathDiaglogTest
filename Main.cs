using HarmonyLib;
using static Kingmaker.Cheats.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Area;
using Kingmaker.Designers;
using System.IO;
using UnityEngine;
using UnityModManagerNet;
using Kingmaker.Utility;

namespace DialogTest
{
#if (DEBUG)
    [EnableReloading]
#endif
    static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        private static Harmony Harmony;
        private static AssetBundle bundle;
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
            
        }
    }
}
