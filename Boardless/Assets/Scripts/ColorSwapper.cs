using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField]
    public Material hoverMaterial;

    private Material initialMaterial;

    private void Start()
    {
        initialMaterial = GetComponent<Renderer>().material;
    }

    public void startHoverColor()
    {
        GetComponent<Renderer>().material = hoverMaterial;
    }

    public void endHoverColor()
    {
        GetComponent<Renderer>().material = initialMaterial;
    }
}
    
