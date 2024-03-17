using HarmonyLib;
using Il2CppSystem;
using Il2CppVampireSurvivors.Framework.NumberTypes;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using MelonLoader;

namespace PoesQOL
{
    public static class ModInfo
    {
        public const string Name = "Poes QOL";
        public const string Id = "poesqol";
        public const string Description = "Multi-Purpose quality of life mod.";
        public const string Author = "LeCloutPanda";
        public const string Company = "Pandas Hell Hole";
        public const string Version = "1.1.0";
        public const string DownloadLink = "https://github.com/LeCloutPanda/PoesQOL";
    }

    public class PoesQOL : MelonMod
    {
        private static MelonPreferences_Category modCategory;
        private static MelonPreferences_Entry<bool> globalToggle;
        // Move Speed Cap
        private static MelonPreferences_Entry<bool> moveSpeedCapToggle;
        private static MelonPreferences_Entry<int> moveSpeedMin;
        private static MelonPreferences_Entry<int> moveSpeedMax;
        // Gold Fever Fix
        private static MelonPreferences_Entry<bool> goldFeverFixToggle;
        // Max Weapon Count
        private static MelonPreferences_Entry<bool> maxWeaponCountToggle;
        private static MelonPreferences_Entry<int> maxWeaponCount;

        public override void OnInitializeMelon()
        {
            modCategory = MelonPreferences.CreateCategory(ModInfo.Id);

            globalToggle = modCategory.CreateEntry<bool>("globalToggle", true);

            moveSpeedCapToggle = modCategory.CreateEntry<bool>("moveSpeedCapToggle", true);
            moveSpeedMin = modCategory.CreateEntry<int>("moveSpeedMin", -1);
            moveSpeedMax = modCategory.CreateEntry<int>("moveSpeedMax", 3);

            goldFeverFixToggle = modCategory.CreateEntry<bool>("goldFeverFixToggle", true);

            maxWeaponCountToggle = modCategory.CreateEntry<bool>("maxWeaponCountToggle", true);
            maxWeaponCount = modCategory.CreateEntry<int>("maxWeaponCount", 6);
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

        [HarmonyPatch(typeof(GoldFeverUIManager), "OnEnable")]
        public class GoldFeverUIManagerOnEnable_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(GoldFeverUIManager __instance)
            {
                // Toggle checks for global and local
                if (!globalToggle.Value) return true;
                if (!goldFeverFixToggle.Value) return true; 
                __instance.IntroTween();
                return false;
            }
        }

        [HarmonyPatch(typeof(CharacterSelectionPage))]
        public class MoreWeapons_Patch
        {
            // TODO: Change this so it doesn't use a bool active state and just check if the max is 6 then don't do anything but anything above we
            // automatically set the weapon count to be the max when opening the menu to save people needing to click but keep the current way of cycling

            [HarmonyPatch("IncreaseMaxWeapons")]
            [HarmonyPrefix]
            public static bool Prefix(CharacterSelectionPage __instance)
            {
                // Toggle checks for global and local
                if (!globalToggle.Value) return true;
                if (!maxWeaponCountToggle.Value) return true;

                if (__instance._playerOptions._mainGameConfig.SelectedMaxWeapons > maxWeaponCount.Value - 1)
                {
                    __instance._playerOptions._mainGameConfig.SelectedMaxWeapons = 1;
                    __instance._MaxWeaponsText.text = $"{__instance._playerOptions._mainGameConfig.SelectedMaxWeapons}";
                }
                else
                {
                    __instance._playerOptions._mainGameConfig.SelectedMaxWeapons += 1;
                    __instance._MaxWeaponsText.text = $"{__instance._playerOptions._mainGameConfig.SelectedMaxWeapons}";
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(OptionsController), "BuildGameplayPage")]
        public class CustomSettings
        {
            [HarmonyPostfix]
            public static void Postfix(OptionsController __instance)
            {
                // Global
                __instance.AddLabel("Poes QOL");
                //__instance.AddTickBox("Global Enable", modCategory.GetEntry<bool>(globalToggle.Identifier).Value, (Action<bool>)((value) => { globalToggle.Value = value; modCategory.SaveToFile(false); }), false);
                // Move Speed Cap
                __instance.AddTickBox("Move Speed Cap Enable", modCategory.GetEntry<bool>(moveSpeedCapToggle.Identifier).Value, (Action<bool>) ((value) => { moveSpeedCapToggle.Value = value; modCategory.SaveToFile(false); }), false);
                __instance.AddSliderInteger("Move Speed Cap Min", modCategory.GetEntry<int>(moveSpeedMin.Identifier).Value, (Action<int>)((value) => { moveSpeedMin.Value = value; modCategory.SaveToFile(false); }), false, -10, -1);
                __instance.AddSliderInteger("Move Speed Cap Max", modCategory.GetEntry<int>(moveSpeedMax.Identifier).Value, (Action<int>)((value) => { moveSpeedMax.Value = value; modCategory.SaveToFile(false); }), false,  1, 100);
                // Gold Fever Scale Fix
                __instance.AddTickBox("Gold Fever Scale Fix Enable", modCategory.GetEntry<bool>(goldFeverFixToggle.Identifier).Value, (Action<bool>) ((value) => { goldFeverFixToggle.Value = value; modCategory.SaveToFile(false); }), false);
                // Max Weapon Count
                __instance.AddTickBox("Max Weapon Count Enable", modCategory.GetEntry<bool>(maxWeaponCountToggle.Identifier).Value, (Action<bool>)((value) => { maxWeaponCountToggle.Value = value; modCategory.SaveToFile(false); }), false);
                __instance.AddSliderInteger("Max Weapon Count", modCategory.GetEntry<int>(maxWeaponCount.Identifier).Value, (Action<int>)((value) => { maxWeaponCount.Value = value; modCategory.SaveToFile(false); }), false, 6, 250);
            }
        }

        [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
        public static class Patch_Il2CppDetourMethodPatcher
        {
            public static bool Prefix(Exception ex)
            {
                MelonLogger.Error("During invoking native->managed trampoline", ex);

                return false;
            }
        }
    }
}
