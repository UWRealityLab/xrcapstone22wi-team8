using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    private Renderer r;
    private Color[] texColors;
    private Color[] newColors;
    private int w;
    private float ratioX;
    private float ratioY;
    private int w2;

    void Start()
    {
        r = GetComponent<Renderer>();
        textureSize.x = (int)(300*r.transform.localScale.x);
        textureSize.y = (int)(300*r.transform.localScale.y);
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

    void Update() {
        if (texture.width != (int)(300*r.transform.localScale.x) || texture.height != (int)(300*r.transform.localScale.y)) {
            textureSize.x = (int)(300*r.transform.localScale.x);
            textureSize.y = (int)(300*r.transform.localScale.y);
            if ((int)textureSize.x == 0 || (int)textureSize.y == 0) {
                return;
            }
            Scale(texture, (int)textureSize.x, (int)textureSize.y);
            // texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
            // r.material.mainTexture = texture;
            // Color fillColor = Color.white;
            // var fillColorArray =  texture.GetPixels();
            
            // for(var i = 0; i < fillColorArray.Length; ++i)
            // {
            //     fillColorArray[i] = fillColor;
            // }
            // texture.SetPixels( fillColorArray );
            // texture.Apply();
        }
    }
    
	public void Scale(Texture2D tex, int newWidth, int newHeight) {
        Debug.Log("look at me");
		texColors = tex.GetPixels();
		newColors = new Color[newWidth * newHeight];
		ratioX = 1.0f / ((float)newWidth / (tex.width-1));
		ratioY = 1.0f / ((float)newHeight / (tex.height-1));
		w = tex.width;
		w2 = newWidth;
 
		BilinearScale(0, newHeight);
 
		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();
	}
 
	private void BilinearScale(int start, int end) {
		for (var y = start; y < end; y++) {
			int yFloor = (int)Mathf.Floor(y * ratioY);
			var y1 = yFloor * w;
			var y2 = (yFloor+1) * w;
			var yw = y * w2;
 
			for (var x = 0; x < w2; x++) {
				int xFloor = (int)Mathf.Floor(x * ratioX);
				var xLerp = x * ratioX-xFloor;
				newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
													   ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
													   y*ratioY-yFloor);
			}
		}
	}
 
	private Color ColorLerpUnclamped (Color c1, Color c2, float value) {
        return new Color (c1.r + (c2.r - c1.r) * value, 
						  c1.g + (c2.g - c1.g) * value, 
						  c1.b + (c2.b - c1.b) * value, 
						  c1.a + (c2.a - c1.a) * value);
    }
}