using UnityEngine;
namespace HoverfishHats.Config
{
    [System.Serializable]
    public class HatConfig
    {
        public string PrefabName;
        public string BundleName;
        public string DisplayName;
        public Vector3 FreePosition;
        public Vector3 FreeScale;
        public Vector3 FreeRotation;
        public Vector3 HeldPosition;
        public Vector3 HeldScale;
        public Vector3 HeldRotation;
        public float DefaultWildVerticalOffset;
        public float DefaultWildForwardOffset;
        public float DefaultWildLateralOffset;
        public float DefaultWildScale;
        public float DefaultWildRotX;
        public float DefaultWildRotY;
        public float DefaultWildRotZ;
        public HatConfig(string prefabName, string bundleName, string displayName,
            Vector3 freePos, Vector3 freeScale, Vector3 freeRot,
            Vector3 heldPos, Vector3 heldScale, Vector3 heldRot,
            float wildVert, float wildForward, float wildLateral, float wildScale,
            float wildRotX, float wildRotY, float wildRotZ)
        {
            PrefabName = prefabName;
            BundleName = bundleName;
            DisplayName = displayName;
            FreePosition = freePos;
            FreeScale = freeScale;
            FreeRotation = freeRot;
            HeldPosition = heldPos;
            HeldScale = heldScale;
            HeldRotation = heldRot;
            DefaultWildVerticalOffset = wildVert;
            DefaultWildForwardOffset = wildForward;
            DefaultWildLateralOffset = wildLateral;
            DefaultWildScale = wildScale;
            DefaultWildRotX = wildRotX;
            DefaultWildRotY = wildRotY;
            DefaultWildRotZ = wildRotZ;
        }
    }
}