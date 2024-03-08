using HarmonyLib;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.NumberTypes;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using MelonLoader;

using static Il2CppVampireSurvivors.UI.OptionsController;


namespace PoesQOL
{
    public static class ModInfo
    {
        public const string Name = "Poes QOL";
        public const string Id = "poesqol";
        public const string Description = "Multi-Purpose quality of life mod.";
        public const string Author = "LeCloutPanda";
        public const string Company = "Pandas Hell Hole";
        public const string Version = "1.0.0";
        public const string DownloadLink = "https://github.com/LeCloutPanda/PoesQOL";
    }


    public class PoesQOL : MelonMod
    {
        private static MelonPreferences_Category modCategory;
        private static MelonPreferences_Entry<bool> globalToggle;
        // Move Speed Cap
        private static MelonPreferences_Entry<bool> moveSpeedCapToggle;
        private static MelonPreferences_Entry<float> moveSpeedMin;
        private static MelonPreferences_Entry<float> moveSpeedMax;
        // Gold Fever Fix
        private static MelonPreferences_Entry<bool> goldFeverFixToggle;
        // Golden Eggs for all
        private static MelonPreferences_Entry<bool> allowGoldenEggsOnEveryone;

        public override void OnInitializeMelon()
        {
            modCategory = MelonPreferences.CreateCategory(ModInfo.Id);
            globalToggle = modCategory.CreateEntry<bool>("globalToggle", true);
            moveSpeedCapToggle = modCategory.CreateEntry<bool>("moveSpeedCapToggle", true);
            moveSpeedMin = modCategory.CreateEntry<float>("moveSpeedMin", -1.0f);
            moveSpeedMax = modCategory.CreateEntry<float>("moveSpeedMax", 3.0f);
            goldFeverFixToggle = modCategory.CreateEntry<bool>("goldFeverFixToggle", true);
        }

        [HarmonyPatch(typeof(CharacterController), "OnUpdate")]
        class MaxMoveSpeedPatch
        {
            [HarmonyPostfix]
            static void Postfix(CharacterController __instance)
            {
                // Toggle checks for global and local
                if (!globalToggle.Value) return;
                if (!moveSpeedCapToggle.Value) return;

                EggFloat _MoveSpeed = __instance.PlayerStats.MoveSpeed;
                if (_MoveSpeed < moveSpeedMin.Value) _MoveSpeed = new EggFloat(moveSpeedMin.Value);
                if (_MoveSpeed > moveSpeedMax.Value) _MoveSpeed = new EggFloat(moveSpeedMax.Value);
                __instance.PlayerStats.MoveSpeed = _MoveSpeed;
            }
        }
    }
}
