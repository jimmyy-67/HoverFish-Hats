using UnityEngine;
using HoverfishHats.Config;
namespace HoverfishHats.Components
{
    public static class ShaderHelper
    {
        public static void ApplyShaderFix(GameObject hat, HatType type, Renderer fishRenderer)
        {
            Renderer[] hatRenderers = hat.GetComponentsInChildren<Renderer>(true);
            if (hatRenderers.Length == 0) return;
            Shader referenceShader = fishRenderer?.material?.shader;
            foreach (Renderer r in hatRenderers)
            {
                if (r == null) continue;
                Material[] materials = r.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];
                    if (mat == null) continue;
                    Color originalColor = mat.HasProperty("_Color")
                        ? mat.GetColor("_Color") : Color.white;
                    Texture originalMainTex = mat.HasProperty("_MainTex")
                        ? mat.GetTexture("_MainTex") : null;
                    Texture originalBumpMap = mat.HasProperty("_BumpMap")
                        ? mat.GetTexture("_BumpMap") : null;
                    if (type == HatType.TopHat)
                        ApplyOriginalSombreroShader(mat, referenceShader,
                            originalColor, originalMainTex);
                    else
                        ApplyGenericShaderFix(mat, referenceShader,
                            originalColor, originalMainTex, originalBumpMap);
                }
            }
        }
        private static void ApplyOriginalSombreroShader(Material mat,
            Shader referenceShader, Color originalColor, Texture originalMainTex)
        {
            if (referenceShader != null)
            {
                mat.shader = referenceShader;
                ConfigureMarmosetShader(mat, originalColor, originalMainTex);
            }
            else
            {
                Shader marmoset = Shader.Find("MarmosetUBER")
                    ?? Shader.Find("Marmoset/Uber");
                if (marmoset != null)
                {
                    mat.shader = marmoset;
                    ConfigureMarmosetShader(mat, originalColor, originalMainTex);
                }
                else
                {
                    mat.shader = Shader.Find("Standard");
                    ConfigureStandardShader(mat, originalColor, originalMainTex);
                }
            }
            if (originalMainTex != null && mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", originalMainTex);
        }
        private static void ApplyGenericShaderFix(Material mat, Shader referenceShader,
            Color originalColor, Texture originalMainTex, Texture originalBumpMap)
        {
            Shader targetShader = Shader.Find("MarmosetUBER")
                ?? Shader.Find("Marmoset/Uber");
            if (targetShader != null)
            {
                mat.shader = targetShader;
                if (mat.HasProperty("_Color")) mat.SetColor("_Color", originalColor);
                if (originalMainTex != null && mat.HasProperty("_MainTex"))
                    mat.SetTexture("_MainTex", originalMainTex);
                if (originalBumpMap != null && mat.HasProperty("_BumpMap"))
                    mat.SetTexture("_BumpMap", originalBumpMap);
                if (mat.HasProperty("_Fresnel")) mat.SetFloat("_Fresnel", 0.4f);
                if (mat.HasProperty("_Shininess")) mat.SetFloat("_Shininess", 6f);
                if (mat.HasProperty("_SpecInt")) mat.SetFloat("_SpecInt", 0.8f);
                if (mat.HasProperty("_SpecColor"))
                    mat.SetColor("_SpecColor", new Color(0.8f, 0.8f, 0.8f, 1f));
                if (mat.HasProperty("_EnableGlow")) mat.SetFloat("_EnableGlow", 0f);
                if (mat.HasProperty("_GlowColor"))
                    mat.SetColor("_GlowColor", Color.black);
                if (mat.HasProperty("_IBLreflectionStr"))
                    mat.SetFloat("_IBLreflectionStr", 0.6f);
                if (mat.HasProperty("_IBLdiffuseStr"))
                    mat.SetFloat("_IBLdiffuseStr", 1.2f);
                if (mat.HasProperty("_SelfIllumination"))
                    mat.SetFloat("_SelfIllumination", 0.1f);
                mat.renderQueue = 2000;
                mat.EnableKeyword("MARMO_SIMPLE_GLASS");
                mat.EnableKeyword("MARMO_SPECMAP");
                mat.EnableKeyword("_ZWRITE_ON");
            }
            else if (referenceShader != null)
            {
                mat.shader = referenceShader;
                if (mat.HasProperty("_Color")) mat.SetColor("_Color", originalColor);
                if (originalMainTex != null && mat.HasProperty("_MainTex"))
                    mat.SetTexture("_MainTex", originalMainTex);
                mat.renderQueue = 2000;
            }
            else
            {
                mat.shader = Shader.Find("Standard");
                ConfigureStandardShader(mat, originalColor, originalMainTex);
            }
        }
        private static void ConfigureMarmosetShader(Material mat,
            Color color, Texture mainTex)
        {
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mainTex != null && mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", mainTex);
            if (mat.HasProperty("_Fresnel")) mat.SetFloat("_Fresnel", 0.5f);
            if (mat.HasProperty("_Shininess")) mat.SetFloat("_Shininess", 5f);
            if (mat.HasProperty("_SpecInt")) mat.SetFloat("_SpecInt", 1f);
            if (mat.HasProperty("_SpecColor"))
                mat.SetColor("_SpecColor", Color.white);
            if (mat.HasProperty("_EnableGlow")) mat.SetFloat("_EnableGlow", 0f);
            if (mat.HasProperty("_IBLreflectionStr"))
                mat.SetFloat("_IBLreflectionStr", 0.5f);
            if (mat.HasProperty("_IBLdiffuseStr"))
                mat.SetFloat("_IBLdiffuseStr", 1f);
            mat.renderQueue = 2000;
            mat.EnableKeyword("MARMO_SIMPLE_GLASS");
            mat.EnableKeyword("MARMO_SPECMAP");
        }
        private static void ConfigureStandardShader(Material mat,
            Color color, Texture mainTex)
        {
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mainTex != null && mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", mainTex);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", 0f);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.3f);
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", color * 0.2f);
            mat.renderQueue = 2000;
        }
    }
}