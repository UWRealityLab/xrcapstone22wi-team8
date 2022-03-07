using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardSimple : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    private Renderer r;
    void Start()
    {
        r = GetComponent<Renderer>();
        textureSize.x = (int)(350*r.transform.localScale.x);
        textureSize.y = (int)(350*r.transform.localScale.y);
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;
        Color fillColor = Color.white;
        var fillColorArray =  texture.GetPixels();
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        texture.SetPixels( fillColorArray );
        texture.Apply();
    }

}