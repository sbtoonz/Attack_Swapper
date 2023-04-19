using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;

namespace AttackSwapper
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class AttackSwapperMod : BaseUnityPlugin
    {
        private const string ModName = "AttackSwapperMod";
        private const string ModVersion = "1.0.0";
        private const string ModGUID = "zarboz.AttackSwapperMod";
        private static Harmony harmony = null!;
        
        #region ConfigSync
        ConfigSync configSync = new(ModGUID) 
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion};
        internal static ConfigEntry<bool> ServerConfigLocked = null!;
        ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }
        ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

        #endregion
        
        internal AssetBundle _assetBundle;
        internal static Dictionary<string, AnimationClip> ExternalAnimations = new();
        internal static Dictionary<string, string> ReplacementAnimationList = new();
        internal static bool FirstInit;
        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            ServerConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
            configSync.AddLockingConfigEntry(ServerConfigLocked);
            
            _assetBundle = Utilities.LoadAssetBundle("attackattack");
            
            ExternalAnimations.Add("SkillALL", _assetBundle.LoadAsset<AnimationClip>("Skill_G_ALL"));
            
            ReplacementAnimationList.Add("Greatsword BaseAttack (3)", "SkillALL");
            
        }
    }
}
