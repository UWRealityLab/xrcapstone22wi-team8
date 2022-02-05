using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    // The object to generate.
    public GameObject Obj;

    // The position of the generated object.
    public Vector3 Pos; 

    // Generates the preset game object at the given position.
    public void Generate()
    {
        GameObject newObj = Instantiate(Obj);
        newObj.transform.position = Pos;
    }
}
