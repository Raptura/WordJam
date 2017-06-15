using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeComp : MonoBehaviour
{

    public MapNode nodeData;
    private SpriteRenderer spriteRenderer;
    private Color spColor = Color.white;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        GameController.StartListening("enter node " + "(" + nodeData.posX + "," + nodeData.posY + ")", unHide);
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (nodeData.room != null)
        {
            spColor = Color.cyan;
        }
        else
        {
            spColor = Color.white;
        }
        spriteRenderer.sprite = nodeData.nodeSprite;
        transform.localPosition = new Vector2(nodeData.posX, nodeData.posY);
        spriteRenderer.color = new Color(spColor.r, spColor.g, spColor.b, nodeData.hidden ? 0 : 1);
    }

    void unHide()
    {
        nodeData.hidden = false;
    }
}
