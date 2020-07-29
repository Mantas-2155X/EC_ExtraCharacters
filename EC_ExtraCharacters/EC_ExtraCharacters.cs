using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

using HarmonyLib;

namespace EC_ExtraCharacters
{
    [BepInProcess("EmotionCreators")]
    [BepInPlugin(nameof(EC_ExtraCharacters), nameof(EC_ExtraCharacters), VERSION)]
    public class EC_ExtraCharacters : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";

        public new static ManualLogSource Logger;
        
        public static int charaCount = 8;
        
        private static ConfigEntry<int> CharaCount { get; set; }

        private void Awake()
        {
            Logger = base.Logger;
            
            CharaCount = Config.Bind("Requires restart! Modifies save!", "Females Count", 8, new ConfigDescription("Requires a restart to apply.", new AcceptableValueRange<int>(8, 99)));
            charaCount = CharaCount.Value;

            var harmony = new Harmony(nameof(EC_ExtraCharacters));
            harmony.PatchAll(typeof(Hooks));
            
            Hooks.PatchSpecial(harmony);
        }
    }
}