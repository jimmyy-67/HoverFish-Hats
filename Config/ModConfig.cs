using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;
namespace HoverfishHats.Config
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> HatVisible;
        public static ConfigEntry<HatType> PrimaryHat;
        public static ConfigEntry<bool> WildHatsEnabled;
        public static Dictionary<HatType, PerHatWildConfig> PerHatConfigs = new Dictionary<HatType, PerHatWildConfig>();
        public static ConfigEntry<bool> ExperimentalMode;
        public static ConfigEntry<bool> ExperimentalWarningAccepted;
        public static ConfigEntry<bool> EnableSecondaryHat;
        public static ConfigEntry<HatType> SecondaryHat;
        public static ConfigEntry<bool> EnableTertiaryHat;
        public static ConfigEntry<HatType> TertiaryHat;
        private static ConfigFile configFile;
        public static void Initialize(ConfigFile config)
        {
            configFile = config;
            HatVisible = config.Bind(
                "General", "Hats Visible", true,
                "Show or hide hats on Hoverfish");
            PrimaryHat = config.Bind(
                "General", "Primary Hat", HatType.TopHat,
                "Choose the hat the Hoverfish will wear");
            PrimaryHat.SettingChanged += (sender, args) =>
            {
                LoadDefaultsForHat(PrimaryHat.Value);
            };
            WildHatsEnabled = config.Bind(
                "Wild Hoverfish", "Enable Wild Hats", true,
                "Show hats on free-swimming Hoverfish");
            CreateAllPerHatConfigs(config);
            ExperimentalWarningAccepted = config.Bind(
                "Experimental", "1. Accept Warning", false,
                "READ: Experimental mode may cause visual bugs.");
            ExperimentalMode = config.Bind(
                "Experimental", "2. Enable Experimental Mode", false,
                "Allows using multiple hats at once.");
            EnableSecondaryHat = config.Bind(
                "Experimental", "3. Enable Secondary Hat", false,
                "Adds a secondary hat");
            SecondaryHat = config.Bind(
                "Experimental", "4. Secondary Hat", HatType.Cowboy,
                "Type of secondary hat");
            EnableTertiaryHat = config.Bind(
                "Experimental", "5. Enable Tertiary Hat", false,
                "Adds a tertiary hat");
            TertiaryHat = config.Bind(
                "Experimental", "6. Tertiary Hat", HatType.Pajama,
                "Type of tertiary hat");
        }
        private static void CreateAllPerHatConfigs(ConfigFile config)
        {
            HatType[] hatTypes = { HatType.TopHat, HatType.Mexican, HatType.Cowboy, HatType.Pajama, HatType.Miner, HatType.Santa };
            foreach (var hatType in hatTypes)
            {
                HatConfig hatCfg = HatConfigRegistry.GetConfigForType(hatType);
                if (hatCfg == null) continue;
                var defaults = GetHardcodedDefaults(hatType);
                string section = hatCfg.DisplayName;
                var perHat = new PerHatWildConfig();
                perHat.ScaleMultiplier = config.Bind(
                    section, "Hat Scale", defaults.scale,
                    new ConfigDescription(
                        "Size multiplier for hat on wild Hoverfish",
                        new AcceptableValueRange<float>(0.1f, 10.0f)));
                perHat.VerticalOffset = config.Bind(
                    section, "Vertical Offset", defaults.verticalOffset,
                    new ConfigDescription(
                        "Move hat up/down",
                        new AcceptableValueRange<float>(-10f, 10f)));
                perHat.ForwardOffset = config.Bind(
                    section, "Forward Offset", defaults.forwardOffset,
                    new ConfigDescription(
                        "Move hat forward/backward",
                        new AcceptableValueRange<float>(-0.3f, 0.3f)));
                perHat.LateralOffset = config.Bind(
                    section, "Lateral Offset", 0f,
                    new ConfigDescription(
                        "Move hat left/right",
                        new AcceptableValueRange<float>(-0.3f, 0.3f)));
                perHat.RotationX = config.Bind(
                    section, "Rotation X (Pitch)", defaults.rotX,
                    new ConfigDescription(
                        "Tilt hat forward/backward",
                        new AcceptableValueRange<float>(-180f, 180f)));
                perHat.RotationY = config.Bind(
                    section, "Rotation Y (Yaw)", defaults.rotY,
                    new ConfigDescription(
                        "Spin hat left/right",
                        new AcceptableValueRange<float>(-180f, 180f)));
                perHat.RotationZ = config.Bind(
                    section, "Rotation Z (Roll)", defaults.rotZ,
                    new ConfigDescription(
                        "Roll hat sideways",
                        new AcceptableValueRange<float>(-180f, 180f)));
                PerHatConfigs[hatType] = perHat;
            }
        }
        public static HatDefaults GetHardcodedDefaults(HatType type)
        {
            switch (type)
            {
                case HatType.TopHat:
                    return new HatDefaults(5.0f, 0.018f, 0.09014f, -5.5915f, 180.0f, 0f);
                case HatType.Mexican:
                    return new HatDefaults(5.0f, 0.0199f, 0.09014f, 14.0f, 0f, 0f);
                case HatType.Cowboy:
                    return new HatDefaults(3.884507f, 0.0021f, 0.09014f, 13.0f, 0f, 0f);
                case HatType.Pajama:
                    return new HatDefaults(5.0f, 0.01f, 0.03708f, 3.38028f, -90.281f, -42.253f);
                case HatType.Miner:
                    return new HatDefaults(5.0f, 0.00938f, 0.12629f, 11.8309f, 0f, 0f);
                case HatType.Santa:
                    return new HatDefaults(5.0f, 0.03051f, 0.12394f, 10.1408f, 23.6619f, 10.1408f);
                default:
                    return new HatDefaults(5.0f, 0f, 0.09f, 0f, 0f, 0f);
            }
        }
        public static void LoadDefaultsForHat(HatType type)
        {
            if (!PerHatConfigs.ContainsKey(type))
            {
                MainPlugin.Log.LogInfo($"[ModConfig] No per-hat config for {type}");
                return;
            }
            MainPlugin.Log.LogInfo($"[ModConfig] Switched to hat: {type}");
        }
        public static void ResetHatToDefaults(HatType type)
        {
            if (!PerHatConfigs.ContainsKey(type))
            {
                MainPlugin.Log.LogInfo($"[ModConfig] No per-hat config to reset for {type}");
                return;
            }
            var defaults = GetHardcodedDefaults(type);
            var perHat = PerHatConfigs[type];
            perHat.ScaleMultiplier.Value = defaults.scale;
            perHat.VerticalOffset.Value = defaults.verticalOffset;
            perHat.ForwardOffset.Value = defaults.forwardOffset;
            perHat.LateralOffset.Value = 0f;
            perHat.RotationX.Value = defaults.rotX;
            perHat.RotationY.Value = defaults.rotY;
            perHat.RotationZ.Value = defaults.rotZ;
            configFile.Save();
            MainPlugin.Log.LogInfo($"[ModConfig] Reset defaults for {type}: " +
                $"Scale={defaults.scale} Vert={defaults.verticalOffset} " +
                $"Fwd={defaults.forwardOffset} Lat=0 " +
                $"RotX={defaults.rotX} RotY={defaults.rotY} RotZ={defaults.rotZ}");
        }
        public static bool IsExperimentalEnabled()
        {
            return ExperimentalMode.Value && ExperimentalWarningAccepted.Value;
        }
        public static List<HatType> GetActiveHatTypes()
        {
            List<HatType> hats = new List<HatType>();
            if (PrimaryHat.Value != HatType.None)
                hats.Add(PrimaryHat.Value);
            if (IsExperimentalEnabled())
            {
                if (EnableSecondaryHat.Value && SecondaryHat.Value != HatType.None)
                    hats.Add(SecondaryHat.Value);
                if (EnableTertiaryHat.Value && TertiaryHat.Value != HatType.None)
                    hats.Add(TertiaryHat.Value);
            }
            return hats;
        }
        public static PerHatWildConfig GetPerHatConfig(HatType type)
        {
            if (PerHatConfigs.ContainsKey(type))
                return PerHatConfigs[type];
            return null;
        }
    }
    public class PerHatWildConfig
    {
        public ConfigEntry<float> ScaleMultiplier;
        public ConfigEntry<float> VerticalOffset;
        public ConfigEntry<float> ForwardOffset;
        public ConfigEntry<float> LateralOffset;
        public ConfigEntry<float> RotationX;
        public ConfigEntry<float> RotationY;
        public ConfigEntry<float> RotationZ;
    }
    public struct HatDefaults
    {
        public float scale;
        public float verticalOffset;
        public float forwardOffset;
        public float rotX;
        public float rotY;
        public float rotZ;
        public HatDefaults(float scale, float verticalOffset, float forwardOffset, float rotX, float rotY, float rotZ)
        {
            this.scale = scale;
            this.verticalOffset = verticalOffset;
            this.forwardOffset = forwardOffset;
            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
        }
    }
}