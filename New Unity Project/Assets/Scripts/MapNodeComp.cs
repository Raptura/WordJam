using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeComp : MonoBehaviour
{

    public MapNode nodeData;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = nodeData.nodeSprite;
        transform.localPosition = new Vector2(nodeData.posX, nodeData.posY);

    }
}
