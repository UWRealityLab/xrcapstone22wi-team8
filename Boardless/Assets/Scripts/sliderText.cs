using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class sliderText : MonoBehaviour
{
    public Text b;
    // Start is called before the first frame update
    public void textUpdate(float a) {
        b.text = (int)a+"";
    }
}
