using UnityEngine;
using System.Collections.Generic;

public class PieceHoverTransparency : MonoBehaviour
{
    private List<Renderer> renderers = new List<Renderer>();
    private List<Material[]> originalMaterials = new List<Material[]>();
    private List<Material[]> transparentMaterials = new List<Material[]>();
    private bool isTransparent = false;

    [Range(0f, 1f)]
    public float transparentAlpha = 0.5f;

    void Start()
    {
        // Pega todos os renderers no objeto e filhos
        renderers.AddRange(GetComponentsInChildren<Renderer>());
        foreach (var rend in renderers)
        {
            // Guarda materiais originais
            Material[] origMats = rend.materials;
            originalMaterials.Add(origMats);
            // Cria cópias para transparência
            Material[] transpMats = new Material[origMats.Length];
            for (int i = 0; i < origMats.Length; i++)
            {
                transpMats[i] = new Material(origMats[i]);
                SetMaterialToFade(transpMats[i]);
                SetMaterialAlpha(transpMats[i], transparentAlpha);
            }
            transparentMaterials.Add(transpMats);
        }
    }

    void OnMouseEnter()
    {
        if (!isTransparent)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = transparentMaterials[i];
            }
            isTransparent = true;
        }
    }

    void OnMouseExit()
    {
        if (isTransparent)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = originalMaterials[i];
            }
            isTransparent = false;
        }
    }

    private void SetMaterialAlpha(Material mat, float alpha)
    {
        if (mat.HasProperty("_Color"))
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
    }

    private void SetMaterialToFade(Material mat)
    {
        mat.SetFloat("_Mode", 2f); // Fade
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
} 