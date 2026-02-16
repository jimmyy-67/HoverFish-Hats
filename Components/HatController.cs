using System.Collections.Generic;
using UnityEngine;
using HoverfishHats.Config;
using HoverfishHats.Assets;
namespace HoverfishHats.Components
{
    public class HatController : MonoBehaviour
    {
        public List<HatInstance> activeHats = new List<HatInstance>();
        private Pickupable pickupable;
        private Creature creature;
        private HatState currentState = HatState.Hidden;
        private List<HatType> lastHatTypes = new List<HatType>();
        private Dictionary<HatType, CachedHatValues> cachedValues = new Dictionary<HatType, CachedHatValues>();
        private bool lastWildEnabled;
        private bool lastHatVisible;
        public class HatInstance
        {
            public GameObject gameObject;
            public HatType type;
            public HatConfig config;
            public int stackIndex;
        }
        private class CachedHatValues
        {
            public float scale;
            public float vertical;
            public float forward;
            public float lateral;
            public float rotX;
            public float rotY;
            public float rotZ;
        }
        void Start()
        {
            pickupable = GetComponent<Pickupable>();
            creature = GetComponent<Creature>();
            CacheAllValues();
            RefreshHats();
        }
        void Update()
        {
            List<HatType> currentTypes = ModConfig.GetActiveHatTypes();
            bool configChanged = !ListsEqual(currentTypes, lastHatTypes);
            bool visibilityChanged = lastHatVisible != ModConfig.HatVisible.Value;
            bool wildChanged = lastWildEnabled != ModConfig.WildHatsEnabled.Value;
            if (configChanged || visibilityChanged || wildChanged)
            {
                lastHatVisible = ModConfig.HatVisible.Value;
                lastWildEnabled = ModConfig.WildHatsEnabled.Value;
                if (configChanged) RefreshHats();
            }
            if (activeHats.Count == 0) return;
            bool isPickedUp = IsPickedUp();
            bool showHats = ModConfig.HatVisible.Value;
            bool showWild = ModConfig.WildHatsEnabled.Value;
            HatState targetState;
            if (!showHats)
                targetState = HatState.Hidden;
            else if (isPickedUp)
                targetState = HatState.Held;
            else if (showWild)
                targetState = HatState.Free;
            else
                targetState = HatState.Hidden;
            if (targetState == HatState.Hidden)
            {
                SetAllHatsActive(false);
                currentState = HatState.Hidden;
                return;
            }
            SetAllHatsActive(true);
            if (targetState != currentState)
            {
                currentState = targetState;
                ApplyAllSettings();
            }
            if (currentState == HatState.Free && WildValuesChanged())
            {
                CacheAllValues();
                ApplyAllSettings();
            }
        }
        public void RefreshHats()
        {
            foreach (var hat in activeHats)
            {
                if (hat.gameObject != null)
                    Destroy(hat.gameObject);
            }
            activeHats.Clear();
            List<HatType> hatTypes = ModConfig.GetActiveHatTypes();
            lastHatTypes = new List<HatType>(hatTypes);
            if (hatTypes.Count == 0) return;
            for (int i = 0; i < hatTypes.Count; i++)
            {
                HatType type = hatTypes[i];
                if (!AssetBundleLoader.HatPrefabs.ContainsKey(type))
                    continue;
                if (!HatConfigRegistry.HatConfigs.ContainsKey(type))
                    continue;
                GameObject prefab = AssetBundleLoader.HatPrefabs[type];
                HatConfig config = HatConfigRegistry.HatConfigs[type];
                if (prefab == null || config == null)
                    continue;
                GameObject hatObj = Instantiate(prefab);
                hatObj.name = $"Hat_{type}_{i}";
                hatObj.transform.SetParent(transform, false);
                activeHats.Add(new HatInstance
                {
                    gameObject = hatObj,
                    type = type,
                    config = config,
                    stackIndex = i
                });
                ShaderHelper.ApplyShaderFix(hatObj, type,
                    GetComponentInChildren<Renderer>());
                hatObj.SetActive(false);
            }
            currentState = HatState.Hidden;
            CacheAllValues();
        }
        private void ApplyAllSettings()
        {
            float cumulativeOffset = 0f;
            for (int i = 0; i < activeHats.Count; i++)
            {
                var hat = activeHats[i];
                if (hat.gameObject == null || hat.config == null) continue;
                Transform t = hat.gameObject.transform;
                HatConfig cfg = hat.config;
                Vector3 position;
                Vector3 scale;
                Quaternion rotation;
                if (currentState == HatState.Free)
                {
                    PerHatWildConfig perHat = ModConfig.GetPerHatConfig(hat.type);
                    if (perHat != null)
                    {
                        position = cfg.FreePosition;
                        position.x += perHat.LateralOffset.Value;
                        position.y += perHat.VerticalOffset.Value;
                        position.z += perHat.ForwardOffset.Value;
                        scale = cfg.FreeScale * perHat.ScaleMultiplier.Value;
                        Vector3 baseRot = cfg.FreeRotation;
                        rotation = Quaternion.Euler(
                            baseRot.x + perHat.RotationX.Value,
                            baseRot.y + perHat.RotationY.Value,
                            baseRot.z + perHat.RotationZ.Value
                        );
                    }
                    else
                    {
                        position = cfg.FreePosition;
                        scale = cfg.FreeScale;
                        rotation = Quaternion.Euler(cfg.FreeRotation);
                    }
                }
                else
                {
                    position = cfg.HeldPosition;
                    scale = cfg.HeldScale;
                    rotation = Quaternion.Euler(cfg.HeldRotation);
                }
                Vector3 finalPosition = position + new Vector3(0f, cumulativeOffset, 0f);
                cumulativeOffset += GetStackOffset(hat.type);
                t.localPosition = finalPosition;
                t.localScale = scale;
                t.localRotation = rotation;
            }
        }
        private float GetStackOffset(HatType type)
        {
            return type == HatType.TopHat ? 0.025f : 0.022f;
        }
        private void SetAllHatsActive(bool active)
        {
            foreach (var hat in activeHats)
            {
                if (hat.gameObject != null && hat.gameObject.activeSelf != active)
                    hat.gameObject.SetActive(active);
            }
        }
        private void CacheAllValues()
        {
            cachedValues.Clear();
            lastWildEnabled = ModConfig.WildHatsEnabled.Value;
            lastHatVisible = ModConfig.HatVisible.Value;
            foreach (var hat in activeHats)
            {
                PerHatWildConfig perHat = ModConfig.GetPerHatConfig(hat.type);
                if (perHat == null) continue;
                cachedValues[hat.type] = new CachedHatValues
                {
                    scale = perHat.ScaleMultiplier.Value,
                    vertical = perHat.VerticalOffset.Value,
                    forward = perHat.ForwardOffset.Value,
                    lateral = perHat.LateralOffset.Value,
                    rotX = perHat.RotationX.Value,
                    rotY = perHat.RotationY.Value,
                    rotZ = perHat.RotationZ.Value
                };
            }
        }
        private bool WildValuesChanged()
        {
            foreach (var hat in activeHats)
            {
                PerHatWildConfig perHat = ModConfig.GetPerHatConfig(hat.type);
                if (perHat == null) continue;
                if (!cachedValues.ContainsKey(hat.type)) return true;
                var cached = cachedValues[hat.type];
                if (!Mathf.Approximately(cached.scale, perHat.ScaleMultiplier.Value)
                    || !Mathf.Approximately(cached.vertical, perHat.VerticalOffset.Value)
                    || !Mathf.Approximately(cached.forward, perHat.ForwardOffset.Value)
                    || !Mathf.Approximately(cached.lateral, perHat.LateralOffset.Value)
                    || !Mathf.Approximately(cached.rotX, perHat.RotationX.Value)
                    || !Mathf.Approximately(cached.rotY, perHat.RotationY.Value)
                    || !Mathf.Approximately(cached.rotZ, perHat.RotationZ.Value))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsPickedUp()
        {
            if (Inventory.main != null && pickupable != null)
            {
                ItemsContainer container = Inventory.main.container;
                if (container != null)
                {
                    foreach (InventoryItem item in container)
                    {
                        if (item.item == pickupable)
                            return true;
                    }
                }
                if (Inventory.main.quickSlots != null)
                {
                    InventoryItem heldItem = Inventory.main.quickSlots.heldItem;
                    if (heldItem != null && heldItem.item == pickupable)
                        return true;
                }
            }
            Transform parent = transform.parent;
            if (parent != null)
            {
                string parentName = parent.name.ToLower();
                if (parentName.Contains("player") ||
                    parentName.Contains("inventory") ||
                    parentName.Contains("hand") ||
                    parentName.Contains("socket") ||
                    parentName.Contains("storage") ||
                    parentName.Contains("aquarium"))
                    return true;
                if (parent.GetComponent<Player>() != null ||
                    parent.GetComponent<Inventory>() != null)
                    return true;
            }
            if (creature != null && !creature.enabled)
                return true;
            return false;
        }
        private bool ListsEqual(List<HatType> a, List<HatType> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}