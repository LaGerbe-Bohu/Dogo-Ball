using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawer : MonoBehaviour
{
    public static DebugDrawer instance;

    private void Awake()
    {
        instance = this;
    }

    public void DrawSphere(Vector3 position, float size,PrimitiveType t)
    {
        GameObject go =   Instantiate(GameObject.CreatePrimitive(t), position, Quaternion.identity);
        go.transform.localScale = new Vector3(size, size, size);
      
    }
    
}
