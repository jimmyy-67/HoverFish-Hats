using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections;
using Nautilus.Handlers;
using HoverfishHats.Config;
using HoverfishHats.Assets;
using HoverfishHats.Components;
using HoverfishHats.Options;
namespace HoverfishHats
{
    [BepInPlugin("com.qopp.hoverfishhats", "Hoverfish Hats Mod", "1.0.3")]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPlugin : BaseUnityPlugin
    {
        public static MainPlugin Instance;
        public static ManualLogSource Log;
        void Awake()
        {
            Instance = this;
            Log = Logger;
            try
            {
                Log.LogInfo("HoverfishHats Initializing...");
                Log.LogInfo("Hat configs...");
                HatConfigRegistry.Initialize();
                Log.LogInfo("Mod config...");
                ModConfig.Initialize(Config);
                Log.LogInfo("Asset bundles...");
                string pluginFolder = Path.GetDirectoryName(Info.Location);
                AssetBundleLoader.LoadAll(pluginFolder, Log);
                Log.LogInfo("Options panel...");
                OptionsPanelHandler.RegisterModOptions(new HoverfishHatsOptions());
                Log.LogInfo("Harmony patches...");
                var harmony = new Harmony("com.qopp.hoverfishhats");
                harmony.PatchAll();
                Log.LogInfo("Starting Hoverfish scanner...");
                StartCoroutine(HoverfishScanner());
                Log.LogInfo("Mod loaded successfully!");
            }
            catch (System.Exception ex)
            {
                Log.LogError($"HoverfishHats FAILED to load: {ex}");
                Log.LogError($"Stack trace: {ex.StackTrace}");
            }
        }
        private IEnumerator HoverfishScanner()
        {
            yield return new WaitForSeconds(10f);
            Log.LogInfo("Hoverfish scanner active");
            while (true)
            {
                yield return new WaitForSeconds(3f);
                try
                {
                    Hoverfish[] allHoverfish = FindObjectsOfType<Hoverfish>();
                    foreach (Hoverfish hf in allHoverfish)
                    {
                        if (hf != null && hf.gameObject != null)
                        {
                            if (hf.GetComponent<HatController>() == null)
                            {
                                hf.gameObject.AddComponent<HatController>();
                                Log.LogInfo($"Scanner: HatController added to Hoverfish [{hf.gameObject.name}]");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.LogError($"Scanner error: {ex.Message}");
                }
            }
        }
        void OnDestroy()
        {
            AssetBundleLoader.UnloadAll();
        }
        public static void UpdateAllHats()
        {
            HatController[] controllers = FindObjectsOfType<HatController>();
            Log.LogInfo($"UpdateAllHats: Found {controllers.Length} controllers");
            foreach (HatController controller in controllers)
            {
                controller.RefreshHats();
            }
        }
    }
}