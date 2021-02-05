
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class InteractionHighlight : MonoBehaviour
{
    [SerializeField] Texture2D cursor;

    [SerializeField] Renderer[] highlightedObjects;

    [SerializeField] AudioClip clickedSound = null;
    [SerializeField] AudioSource clickedAudioSource = null;

    [SerializeField] float whiteMultiplier = 2;
    [SerializeField] float clickedMultiplier = .5f;

    Dictionary<Renderer, Material[]> clickedMaterials = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> highlightedMaterials = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> rendererMaterials = new Dictionary<Renderer, Material[]>();


    bool mouseWithin = false;
    private void Start()
    {

        foreach (var renderer in highlightedObjects)
        {
            rendererMaterials.Add(renderer, renderer.materials);
            highlightedMaterials.Add(renderer, MultiplyMaterialTint(rendererMaterials[renderer], whiteMultiplier));
            clickedMaterials.Add(renderer, MultiplyMaterialTint(rendererMaterials[renderer], clickedMultiplier));
        }

    }

    Material[] MultiplyMaterialTint(Material[] materials, float multiplier)
    {
        List<Material> multipliedMaterial = new List<Material>();

        foreach (Material material in materials)
        {
            Material whitenedMat = new Material(material);
            if (whitenedMat.HasProperty("_Tint"))
            {
                Color matColor = whitenedMat.GetColor("_Tint");
                whitenedMat.SetColor("_Tint", (matColor * multiplier));
            }
            multipliedMaterial.Add(whitenedMat);
        }
        return multipliedMaterial.ToArray();
    }


    static void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }
    void ResetMaterials()
    {
        foreach (var renderer in rendererMaterials.Keys)
        {
            renderer.materials = rendererMaterials[renderer];
        }
    }

    private void OnMouseEnter()
    {
        if( Time.timeScale == 0.0f ) return;

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

        foreach (var renderer in rendererMaterials.Keys)
        {

            renderer.materials = highlightedMaterials[renderer];
        }

        mouseWithin = true;
    }

    private void OnMouseDown()
    {


        if (mouseWithin)
        {
            //Material[] mats = new Material[] { clickedMaterial };
            foreach (var renderer in rendererMaterials.Keys)
            {
                renderer.materials = clickedMaterials[renderer];
            }
        }
    }
    private void OnMouseUpAsButton()
    {
        if (mouseWithin)
        {
            //  Material[] mats = new Material[] { highlightMaterial };
            foreach (var renderer in rendererMaterials.Keys)
            {
                renderer.materials = highlightedMaterials[renderer];
            }
        }
        else
            ResetMaterials();
    }

    //IEnumerator clickHighlight()
    //{

    //    if (clickedAudioSource != null && clickedSound != null)
    //    {
    //        clickedAudioSource.clip = clickedSound;
    //        clickedAudioSource.Play();
    //    }
    //    Material[] mats = new Material[] { clickedMaterial };
    //    foreach (var item in rendererMaterials.Keys)
    //    {
    //        item.materials = mats;
    //    }

    //    yield return new WaitForSeconds(clickHighlightTime);
    //    if (mouseWithin)
    //    {
    //        mats = new Material[] { highlightMaterial };
    //        foreach (var renderer in rendererMaterials.Keys)
    //        {
    //            renderer.materials = mats;
    //        }
    //    }
    //    else
    //    {
    //        foreach (var renderer in rendererMaterials.Keys)
    //        {
    //            renderer.materials = rendererMaterials[renderer];
    //        }
    //    }


    //}


    [ContextMenu("Get child renderes")]
    public void GetAllChildRenderers()
    {
        highlightedObjects = transform.GetComponentsInChildren<Renderer>();
    }

    private void OnDestroy()
    {
        ResetCursor();
    }
    private void OnDisable()
    {
        ResetCursor();
      //  ResetMaterials();
    }
    private void OnMouseExit()
    {
        ResetCursor();
        mouseWithin = false;

        foreach (var renderer in rendererMaterials.Keys)
        {
            renderer.materials = rendererMaterials[renderer];
        }
    }
}
