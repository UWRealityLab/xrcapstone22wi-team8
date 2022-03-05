using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Material material;
    // Update is called once per frame
    void Update()
    {
        material.color = fcp.color;
    }
}
