using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapCam2D : MonoBehaviour
{


    public Transform target;
    public int zDist = -5;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.position = new Vector3(target.position.x, target.position.y, zDist);
    }
}
