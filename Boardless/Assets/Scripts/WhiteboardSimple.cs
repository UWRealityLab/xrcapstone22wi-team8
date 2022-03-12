using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardSimple : MonoBehaviour
{
    public Texture2D texture;
    public Texture2D inputTexture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    private Renderer r;
    void Start()
    {
        // r = GetComponent<Renderer>();
        // textureSize.x = (int)(350*r.transform.localScale.x);
        // textureSize.y = (int)(350*r.transform.localScale.y);
        // // var texture2 = Resize(inputTexture, (int)textureSize.x, (int)textureSize.y);
        // // Debug.Log(texture.GetPixels().Length);
        // texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        // r.material.mainTexture = texture;
        // // var fill = texture2.GetPixels();
        // // texture.SetPixels(fill);
        // // texture.Apply();
        // Color fillColor = Color.white;
        // var fillColorArray =  texture.GetPixels();
        // // var toFill = inputTexture.GetPixels();
        // // Debug.Log(toFill.Length);
        // // Debug.Log(fillColorArray.Length);
        // for(var i = 0; i < fillColorArray.Length; ++i)
        // {
        //     fillColorArray[i] = fillColor;
        // }
        // texture.SetPixels( fillColorArray );
        // texture.Apply();


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
    Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }

}