using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConveyorBeltVisual : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, 0);
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}

