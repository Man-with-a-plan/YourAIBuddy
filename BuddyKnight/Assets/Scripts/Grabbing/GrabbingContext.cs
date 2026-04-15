using System.Collections.Generic;
using NUnit.Framework;
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
    private Transform _rootTransform;
    private ChainIKConstraint _leftIkConstraintChain;
    private ChainIKConstraint _rightIkConstraintChain;

    public GrabbingContext(ChainIKConstraint leftIkConstraint, ChainIKConstraint rightIkConstraint, TwoBoneIKConstraint leftLegIkContstaint,
        TwoBoneIKConstraint rightLegIkConstraint,
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotConstraint,
        CharacterController characterController, Transform rootTransform)
    {
        _leftIkConstraintChain = leftIkConstraint;
        _rightIkConstraintChain = rightIkConstraint;
        _leftIkConstraint = leftLegIkContstaint;
        _rightIkConstraint = rightLegIkConstraint;
        _leftMultiRotConstraint = leftMultiRotationConstraint;
        _rightMultiRotConstraint = rightMultiRotConstraint;
        _characterController = characterController;
        _rootTransform = rootTransform;
    }

    public ChainIKConstraint LeftIkConstraint => _leftIkConstraintChain;
    public ChainIKConstraint RightIkConstraint => _rightIkConstraintChain;
    public TwoBoneIKConstraint LeftLegIkConstraint => _leftIkConstraint;
    public TwoBoneIKConstraint RightLegIkConstraint => _rightIkConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotConstraint;

    public CharacterController CharacterController => _characterController;
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
    public Vector3 FurthestGrabPointFromLeftShoulder { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromRightShoulder { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromLeftHip { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromRightHip { get; private set; } = Vector3.positiveInfinity;

    //To optimise can change to Vector3
    protected GameObject leftHandGrab, rightHandGrab;

    

    
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
                if (distanceToCheck > longestLeftDistance & ListOfClosestGrabPoints[i] != CurrentlyGrabbing["RightLeg"])
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
                    break;
                case RigCollisionHandler.BodySide.RightArm:
                    FurthestGrabPointFromRightShoulder = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightHand"] = leftHandGrab;
                    
                    break;
                case RigCollisionHandler.BodySide.RightLeg:
                    FurthestGrabPointFromRightHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["RightLeg"] = leftHandGrab;
                    break;
                case RigCollisionHandler.BodySide.LeftLeg:
                    FurthestGrabPointFromLeftHip = leftHandGrab.transform.position;
                    CurrentlyGrabbing["LeftLeg"] = leftHandGrab;
                    break;
            }

           

        }
        ListCleanup(ListOfClosestGrabPoints);

    }
    
    void ListCleanup(List<GameObject> ListToClean)
   {

        float maxDistance = _characterController.height / 2f;

        // Get the character controller's global position
        Vector3 controllerPosition = _characterController.transform.position;

        // Calculate the center in world space (accounting for local offset)
        Vector3 center = controllerPosition + _characterController.center;
        center.y += 0.33f * _characterController.height;
        center.z += 0.2f;

        for (int i = 0; i < ListToClean.Count; i++)
        {
            float distanceToGrabPoint = Vector3.Distance(center, ListToClean[i].transform.position);
            if (distanceToGrabPoint > maxDistance)
            {
              
                ListToClean.RemoveAt(i);
                i--;
            }
        }
    }
  


}
