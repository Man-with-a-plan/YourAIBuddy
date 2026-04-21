using JetBrains.Annotations;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
public class RigCollisionHandler : MonoBehaviour
{
    public enum BodySide
    {
        LeftLeg,
        RightLeg,
        RightArm,
        LeftArm
        
    }
    [SerializeField] private BodySide Limb;
    public static event Action<GameObject, BodySide> NewPointEntered;
    public static event Action<GameObject, BodySide> NewPointExited;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            NewPointEntered?.Invoke(other.gameObject, Limb);
           // Debug.Log("New Point Entered");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            NewPointExited?.Invoke(other.gameObject, Limb);
           // Debug.Log("New Point Exited");
        }
    }
}
