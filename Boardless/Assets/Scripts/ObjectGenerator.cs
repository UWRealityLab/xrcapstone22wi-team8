using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    // The object to generate.
    public GameObject obj;

    // The position of the generated object.
    public Vector3 pos; 

    // Generates the preset game object at the given position.
    public void generate()
    {
        GameObject newObj = Instantiate(obj);
        newObj.transform.position = pos;
    }
}
