using Kingmaker.Localization;
using System.Collections.Generic;

namespace DialogTest.Utilities
{
    //
    // Needed to create text in the game. Each text entry must have its own unique key. Generally it is good practice to precede your key with the name of your mod to avoid
    // key collisons. You can use the key to fetch the string. Helpful if you want to localize your mods to different languages.
    //

    internal static class GameStrings
    {
        private static Dictionary<string, LocalizedString> Strings = new();
        internal static LocalizedString CreateString(string key, string value)
        {
            key = key.ToLower();
            var localizedString = new LocalizedString() { m_Key = key };
            LocalizationManager.CurrentPack.PutString(key, value);
            Strings.Add(key, localizedString);
            return localizedString;
        }

        internal static LocalizedString GetString(string key)
        {
            return Strings.ContainsKey(key) ? Strings[key] : default;
        }
    }
}
