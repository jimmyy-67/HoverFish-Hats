using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;
using HoverfishHats.Config;
namespace HoverfishHats.Assets
{
    public static class AssetBundleLoader
    {
        public static Dictionary<string, AssetBundle> LoadedBundles
            = new Dictionary<string, AssetBundle>();
        public static Dictionary<HatType, GameObject> HatPrefabs
            = new Dictionary<HatType, GameObject>();
        public static void LoadAll(string pluginFolder, ManualLogSource log)
        {
            int loadedCount = 0;
            int prefabCount = 0;
            log.LogInfo($"Plugin folder: {pluginFolder}");
            foreach (var kvp in HatConfigRegistry.HatConfigs)
            {
                if (kvp.Value == null) continue;
                HatConfig config = kvp.Value;
                string bundlePath = Path.Combine(pluginFolder, config.BundleName);
                AssetBundle bundle;
                if (LoadedBundles.ContainsKey(config.BundleName))
                {
                    bundle = LoadedBundles[config.BundleName];
                }
                else
                {
                    if (!File.Exists(bundlePath))
                    {
                        log.LogWarning($"Bundle not found: {config.BundleName}");
                        continue;
                    }
                    bundle = AssetBundle.LoadFromFile(bundlePath);
                    if (bundle == null)
                    {
                        log.LogError($"Failed to load bundle: {config.BundleName}");
                        continue;
                    }
                    LoadedBundles[config.BundleName] = bundle;
                    loadedCount++;
                }
                GameObject prefab = bundle.LoadAsset<GameObject>(config.PrefabName);
                if (prefab == null)
                {
                    string[] allAssets = bundle.GetAllAssetNames();
                    foreach (string assetName in allAssets)
                    {
                        if (assetName.ToLower().Contains(
                            config.PrefabName.ToLower().Replace("-", "")))
                        {
                            prefab = bundle.LoadAsset<GameObject>(assetName);
                            if (prefab != null)
                            {
                                log.LogInfo($"Found {kvp.Key} with path: {assetName}");
                                break;
                            }
                        }
                    }
                }
                if (prefab != null)
                {
                    HatPrefabs[kvp.Key] = prefab;
                    prefabCount++;
                    log.LogInfo($"Loaded: {kvp.Key} ({config.PrefabName})");
                }
                else
                {
                    log.LogError($"Failed: {kvp.Key} ({config.PrefabName})");
                    foreach (string name in bundle.GetAllAssetNames())
                        log.LogWarning($"  Available: {name}");
                }
            }
            log.LogInfo($"Loaded {loadedCount} bundles, {prefabCount} prefabs");
        }
        public static void UnloadAll()
        {
            foreach (var bundle in LoadedBundles.Values)
            {
                if (bundle != null)
                    bundle.Unload(false);
            }
            LoadedBundles.Clear();
        }
    }
}