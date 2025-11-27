using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour

{
    Transform MainCam;
    Transform unit;
    Transform WolrldSpaceCanvas;
    public Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCam = Camera.main.transform;
        unit = transform.parent;
        WolrldSpaceCanvas = Object.FindAnyObjectByType<Canvas>().transform;
        transform.SetParent(WolrldSpaceCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - MainCam.transform.position);
        transform.position = unit.position + offset;
    }
}
