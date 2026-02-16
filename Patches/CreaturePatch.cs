using HarmonyLib;
using UnityEngine;
using System.Collections;
using HoverfishHats.Components;
namespace HoverfishHats.Patches
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
    public static class CreaturePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Creature __instance)
        {
            if (__instance is Hoverfish)
            {
                if (__instance.GetComponent<HatController>() == null)
                {
                    if (MainPlugin.Instance != null)
                    {
                        MainPlugin.Instance.StartCoroutine(
                            AttachHatController(__instance.gameObject));
                    }
                }
            }
        }
        private static IEnumerator AttachHatController(GameObject hoverfish)
        {
            yield return new WaitForSeconds(0.3f);
            if (hoverfish == null) yield break;
            if (hoverfish.GetComponent<HatController>() != null) yield break;
            hoverfish.AddComponent<HatController>();
            MainPlugin.Log.LogInfo($"[Patch] HatController added to [{hoverfish.name}]");
        }
    }
}