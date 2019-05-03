using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScroll : MonoBehaviour
{
    public Vector2 Scroll;

    MeshRenderer _myRenderer;
    Vector2 _scroll;

	void Start ()
    {
        _myRenderer = GetComponent<MeshRenderer>();	
	}
	
	void Update ()
    {
        _scroll += Scroll;
        _myRenderer.material.SetTextureOffset("_MainTex", _scroll);
	}
}
