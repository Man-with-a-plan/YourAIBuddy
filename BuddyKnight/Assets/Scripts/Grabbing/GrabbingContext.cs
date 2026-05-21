using System.Collections.Generic;
using NUnit.Framework;
using StarterAssets;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabbingContext 
{
 



  private TwoBoneIKConstraint _leftIkConstraint;
  private TwoBoneIKConstraint _rightIkConstraint;
  private MultiRotationConstraint _leftMultiRotConstraint;
  private MultiRotationConstraint _rightMultiRotConstraint;
  private CharacterController _characterController;
    private ThirdPersonController _thirdPersonController;   
    private Transform _rootTransform;
    private ChainIKConstraint _leftIkConstraintChain;
    private ChainIKConstraint _rightIkConstraintChain;

    public GrabbingContext(ChainIKConstraint leftIkConstraint, ChainIKConstraint rightIkConstraint, TwoBoneIKConstraint leftLegIkContstaint,
        TwoBoneIKConstraint rightLegIkConstraint,
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotConstraint,
        CharacterController characterController, ThirdPersonController thirdPersonController, Transform rootTransform)
    {
        _leftIkConstraintChain = leftIkConstraint;
        _rightIkConstraintChain = rightIkConstraint;
        _leftIkConstraint = leftLegIkContstaint;
        _rightIkConstraint = rightLegIkConstraint;
        _leftMultiRotConstraint = leftMultiRotationConstraint;
        _rightMultiRotConstraint = rightMultiRotConstraint;
        _characterController = characterController;
        _rootTransform = rootTransform;
        _thirdPersonController = thirdPersonController;
    }

    public  ChainIKConstraint LeftIkConstraint => _leftIkConstraintChain;
    public ChainIKConstraint RightIkConstraint => _rightIkConstraintChain;
    public TwoBoneIKConstraint LeftLegIkConstraint => _leftIkConstraint;
    public TwoBoneIKConstraint RightLegIkConstraint => _rightIkConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotConstraint;

    public CharacterController CharacterController => _characterController;
    public ThirdPersonController ThirdPersonController => _thirdPersonController;
    public Transform RootTransform => _rootTransform;

    public ChainIKConstraint CurrentIkConstraint;
    public MultiRotationConstraint CurrentMultiRotationConstraint;

   
   
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
 
    
    public List<GameObject> GrabPointsLeftLeg { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsRightLeg { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsRightArm { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsLeftArm { get; private set; } = new List<GameObject>();
    //List of game objects that character is currently grabbing. Later will be used to defy his rotation in ThirdPersonController
    public Dictionary<string, GameObject> CurrentlyGrabbing = new Dictionary<string, GameObject>();


    //to debug which grab point is being grabbed
    public Vector3 FurthestGrabPointFromLeftShoulder { get; private set; }
    public Vector3 FurthestGrabPointFromRightShoulder { get; private set; } 
    public Vector3 FurthestGrabPointFromLeftHip { get; private set; } 
    public Vector3 FurthestGrabPointFromRightHip { get; private set; } 

    //To optimise can change to Vector3
    protected GameObject leftHandGrab, rightHandGrab;
    public Vector3 Normal;

    public MonoBehaviour Owner;
    public GrabbingStateMachine StateMachine;
  
    public void SetClosestPoint(List<GameObject> ListOfClosestGrabPoints, RigCollisionHandler.BodySide limb)
    {
        
        if (ListOfClosestGrabPoints.Count > 0)
        {
           
            Vector3 Center = RightIkConstraint.data.root.transform.position;
        
            float longestLeftDistance = Vector3.Distance(Center, ListOfClosestGrabPoints[0].transform.position);
            leftHandGrab = ListOfClosestGrabPoints[0];
        

            float distanceToCheck = float.MaxValue;

        

            for (int i = 0; i < ListOfClosestGrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(Center, ListOfClosestGrabPoints[i].transform.position);
                if (distanceToCheck > longestLeftDistance )
                {
                    leftHandGrab = ListOfClosestGrabPoints[i];
                }
            }
            //to indicate which is the closest to the left hand
            switch(limb)
            {
                case RigCollisionHandler.BodySide.LeftArm:
                    FurthestGrabPointFromLeftShoulder = leftHandGrab.transform.position;
                    CurrentlyGrabbing["LeftHand"] = leftHandGrab;
                    Debug.Log("Left hand grab point: " + FurthestGrabPointFromLeftShoulder);
                    break;
                case RigCollisionHandler.BodySide.RightArm:
                    FurthestGrabPointFromRightShoulder = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightHand"] = leftHandGrab;
                    Debug.Log("Right hand grab point: " + FurthestGrabPointFromRightShoulder);
                    break;
                case RigCollisionHandler.BodySide.RightLeg:
                    FurthestGrabPointFromRightHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightLeg"] = leftHandGrab;
                    Debug.Log("Right leg grab point: " + FurthestGrabPointFromRightHip);
                    break;
                case RigCollisionHandler.BodySide.LeftLeg:
                    FurthestGrabPointFromLeftHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["LeftLeg"] = leftHandGrab;
                    Debug.Log("Left leg grab point: " + FurthestGrabPointFromLeftHip);
                    break;
            }

            

        }

        else
        {
            Debug.Log("No grab points in range for " + limb);

            switch (limb)
            {
                case RigCollisionHandler.BodySide.LeftArm:
                    FurthestGrabPointFromLeftShoulder = RootTransform.position + Vector3.up;
                    CurrentlyGrabbing.Remove("LeftHand");
                    break;
                case RigCollisionHandler.BodySide.RightArm:
                    FurthestGrabPointFromRightShoulder = RootTransform.position + Vector3.up;
                    CurrentlyGrabbing.Remove("RightHand");
                    break;
                case RigCollisionHandler.BodySide.RightLeg:
                    FurthestGrabPointFromRightHip = RootTransform.position;
                    CurrentlyGrabbing.Remove("RightLeg");
                    break;
                case RigCollisionHandler.BodySide.LeftLeg:
                    FurthestGrabPointFromLeftHip = RootTransform.position;
                    CurrentlyGrabbing.Remove("LeftLeg");
                    break;
            }
        }

        ListCleanup(ListOfClosestGrabPoints);
    }
    
    void ListCleanup(List<GameObject> ListToClean)
    {

        float maxDistance = _characterController.height / 1.5f;

        // Get the character controller's global position
        Vector3 controllerPosition = _characterController.transform.position;

        // Calculate the center in world space (accounting for local offset)
        Vector3 center = controllerPosition + _characterController.center;
    
       
        Debug.Log("Center: " + center);
        Debug.DrawLine(center, center + Vector3.up * 2f, Color.green, 5f); // Visualize the center point
        for (int i = 0; i < ListToClean.Count; i++)
        {
            float distanceToGrabPoint = Vector3.Distance(center, ListToClean[i].transform.position);
         
            if (distanceToGrabPoint > maxDistance)
            {
                Debug.Log("distance" + distanceToGrabPoint);
                Debug.Log("distance Grab point: " + ListToClean[i].transform.position);
                ListToClean.RemoveAt(i);
                i--;
            }
        }
    }


    public Vector3 GetPlaneNormal()
    {
        try
        {
            Vector3 leftHandPos = CurrentlyGrabbing["LeftHand"].transform.position;
            Vector3 rightHandPos = CurrentlyGrabbing["RightHand"].transform.position;
            Vector3 leftLegPos = CurrentlyGrabbing["LeftLeg"].transform.position;
            Vector3 rightLegPos = CurrentlyGrabbing["RightLeg"].transform.position;

            

            // Create two edge vectors from the limb positions
            Vector3 edge1 = rightLegPos - leftHandPos;  // Vector from left hand to right leg
            Vector3 edge2 = leftLegPos - leftHandPos;    // Vector from left hand to left leg

            // Calculate the normal vector using cross product
            Vector3 normalVector = Vector3.Cross(edge2, edge1).normalized;

            return normalVector;
        }
        catch (System.Exception e)
        {
           // Debug.LogError("Not all limbs are grabbing. Cannot calculate plane normal: " + e.Message);
            return Vector3.zero; // Return a default value or handle as needed
        }

    }

}
