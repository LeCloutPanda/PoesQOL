using HarmonyLib;
using Il2CppSystem;
using Il2CppVampireSurvivors.Framework.NumberTypes;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;

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

        [HarmonyPatch(typeof(GoldFeverUIManager), "OnEnable")]
        public class GoldFeverUIManagerOnEnable_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(GoldFeverUIManager __instance)
            {
                // Toggle checks for global and local
                if (!globalToggle.Value) return true;
                if (!goldFeverFixToggle.Value) return true; __instance.IntroTween();
                return false;
            }
        }


        [HarmonyPatch(typeof(OptionsController), "BuildGameplayPage")]
        public class CustomSettings
        {
            [HarmonyPostfix]
            public static void Postfix(OptionsController __instance)
            {
                __instance.AddLabel("Poes QOL");
                __instance.AddTickBox("Global Enable", modCategory.GetEntry<bool>(globalToggle.Identifier).Value, (Action<bool>)((value) => { globalToggle.Value = value; modCategory.SaveToFile(true); }), false);
                __instance.AddTickBox("Move Speed Cap Enable", modCategory.GetEntry<bool>(moveSpeedCapToggle.Identifier).Value, (Action<bool>) ((value) => { moveSpeedCapToggle.Value = value; modCategory.SaveToFile(true); }), false);
                //__instance.AddSlider("Move Speed Max", modCategory.GetEntry<float>(moveSpeedMax.Identifier).Value, (Action<float>) ((value) => { moveSpeedMax.Value = value; }), false, 0.0f, 10.0f);
                __instance.AddTickBox("Gold Fever Scale Fix Enable", modCategory.GetEntry<bool>(goldFeverFixToggle.Identifier).Value, (Action<bool>) ((value) => { goldFeverFixToggle.Value = value; modCategory.SaveToFile(true); }), false);                
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
