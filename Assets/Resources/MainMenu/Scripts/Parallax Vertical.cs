using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax_Vertical : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    // Start is called before the first frame update

    public float animSpeed = 0.5f;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update(){
        meshRenderer.material.mainTextureOffset += new Vector2(0, animSpeed*Time.deltaTime);
    }
}
