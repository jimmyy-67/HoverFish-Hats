using System.Collections.Generic;
using UnityEngine;
namespace HoverfishHats.Config
{
    public static class HatConfigRegistry
    {
        public static Dictionary<HatType, HatConfig> HatConfigs { get; private set; }
            = new Dictionary<HatType, HatConfig>();
        public static void Initialize()
        {
            HatConfigs[HatType.None] = null;
            HatConfigs[HatType.TopHat] = new HatConfig(
                "SombreroHoverfish", "hoversombrero", "Top Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.1f, 0.1f, 0.1f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.07f, 0.08f),
                new Vector3(0.15f, 0.15f, 0.15f),
                new Vector3(40f, 40f, 0f),
                0.00798f, 0.09014f, 0f, 5.0f, -5.5915f, 180.0f, 0f
            );
            HatConfigs[HatType.Mexican] = new HatConfig(
                "Sombrero", "hovermex", "Mexican Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.08f, 0.08f, 0.08f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.07f, 0.08f),
                new Vector3(0.12f, 0.12f, 0.12f),
                new Vector3(35f, 35f, 0f),
                0.00798f, 0.09014f, 0f, 5.0f, 4.54929f, 0f, 0f
            );
            HatConfigs[HatType.Cowboy] = new HatConfig(
                "CowboyHat", "hovercowboy", "Cowboy Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.08f, 0.08f, 0.08f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.05f, 0.08f),
                new Vector3(0.12f, 0.12f, 0.12f),
                new Vector3(35f, 7.5f, 0f),
                -0.0154f, 0.09014f, 0f, 3.884507f, 13.0f, 0f, 0f
            );
            HatConfigs[HatType.Pajama] = new HatConfig(
                "PajamaHat", "hoverpajama", "Pajama Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.08f, 0.08f, 0.08f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.072f, 0.057f),
                new Vector3(0.12f, 0.12f, 0.12f),
                new Vector3(0f, -90f, -50f),
                -6.7335f, 0.03708f, 0f, 5.0f, 3.38028f, -90.281f, -42.253f
            );
            HatConfigs[HatType.Miner] = new HatConfig(
                "MinerHat", "hovermine", "Miner Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.04f, 0.04f, 0.04f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.07f, 0.08f),
                new Vector3(0.06f, 0.06f, 0.06f),
                new Vector3(35f, 35f, 0f),
                0.00938f, 0.12629f, 0f, 5.0f, 11.8309f, 0f, 0f
            );
            HatConfigs[HatType.Santa] = new HatConfig(
                "santa-hat", "hoversanta", "Santa Hat",
                new Vector3(0f, 0.14f, 0.09f),
                new Vector3(0.007f, 0.007f, 0.007f),
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 0.07f, 0.08f),
                new Vector3(0.01f, 0.01f, 0.01f),
                new Vector3(35f, 35f, 0f),
                0.03051f, 0.12394f, 0f, 5.0f, 10.1408f, 23.6619f, 10.1408f
            );
        }
        public static HatConfig GetConfigForType(HatType type)
        {
            if (type != HatType.None && HatConfigs.ContainsKey(type) && HatConfigs[type] != null)
                return HatConfigs[type];
            return null;
        }
    }
}