using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabbingContext 
{
  public enum EBodySide {  Top, Bottom };



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

    public Collider ArmCollider;
    public Collider LegCollider;
   
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrenBodySide {  get; set; }
    
    public List<GameObject> GrabPointsTop { get; private set; } = new List<GameObject>();
    public List<GameObject> GrabPointsBottom { get; private set; } = new List<GameObject>();
    //List of game objects that character is currently grabbing. Later will be used to defy his rotation in ThirdPersonController
    public List<GameObject> CurrentlyGrabbing = new List<GameObject>();


    //to debug which grab point is being grabbed
    public Vector3 FurthestGrabPointFromLeftShoulder { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromRightShoulder { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromLeftHip { get; private set; } = Vector3.positiveInfinity;
    public Vector3 FurthestGrabPointFromRightHip { get; private set; } = Vector3.positiveInfinity;

    //To optimise can change to Vector3
    protected GameObject leftHandGrab, rightHandGrab;

    

    public void SetPointsToGrabForBothHands()
    {
        
        if (GrabPointsTop.Count > 0)
        {
            Vector3 leftShoulder = LeftIkConstraint.data.root.transform.position;
            Vector3 rightShoulder = RightIkConstraint.data.root.transform.position;
            float longestLeftDistance = Vector3.Distance(leftShoulder, GrabPointsTop[0].transform.position);
            float longestRightDistance = Vector3.Distance(rightShoulder, GrabPointsTop[0].transform.position);
            leftHandGrab = GrabPointsTop[0];
            rightHandGrab = GrabPointsTop[0];
            float distanceToCheck = float.MaxValue;

            CurrentlyGrabbing.Clear();

            for (int i = 0; i < GrabPointsTop.Count; i++)
            {
                distanceToCheck = Vector3.Distance(leftShoulder, GrabPointsTop[i].transform.position);
                if (distanceToCheck > longestLeftDistance & GrabPointsTop[i] != rightHandGrab)
                {
                    leftHandGrab = GrabPointsTop[i];
                }
            }
            //to indicate which is the closest to the left hand
            FurthestGrabPointFromLeftShoulder = leftHandGrab.transform.position;
            CurrentlyGrabbing.Add(leftHandGrab);
            
            //set the closest to the right hand
            for (int i = 0; i < GrabPointsTop.Count; i++)
            {
                distanceToCheck = Vector3.Distance(rightShoulder, GrabPointsTop[i].transform.position);
                if (distanceToCheck > longestRightDistance  )
                {
                    rightHandGrab = GrabPointsTop[i];
                }
            }
            //to indicate which is the closest to the right hand
            FurthestGrabPointFromRightShoulder = rightHandGrab.transform.position;
                CurrentlyGrabbing.Add(rightHandGrab);
           
           ListCleanup(GrabPointsTop);
   
        }
      
    }
    public void SetTwoClosestPoints(List<GameObject> ListOfClosestGrabPoints)
    {

        if (ListOfClosestGrabPoints.Count > 0)
        {
            Vector3 Center = RightIkConstraint.data.root.transform.position;
           


           
        
            float longestLeftDistance = Vector3.Distance(Center, ListOfClosestGrabPoints[0].transform.position);
            float longestRightDistance = Vector3.Distance(Center, ListOfClosestGrabPoints[0].transform.position);

            leftHandGrab = ListOfClosestGrabPoints[0];
            rightHandGrab = ListOfClosestGrabPoints[0];

            float distanceToCheck = float.MaxValue;

            CurrentlyGrabbing.Clear();

            for (int i = 0; i < ListOfClosestGrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(Center, ListOfClosestGrabPoints[i].transform.position);
                if (distanceToCheck > longestLeftDistance & ListOfClosestGrabPoints[i] != rightHandGrab)
                {
                    leftHandGrab = ListOfClosestGrabPoints[i];
                }
            }
            //to indicate which is the closest to the left hand
            switch(CurrenBodySide)
            {
                case EBodySide.Top:
                    FurthestGrabPointFromLeftShoulder = leftHandGrab.transform.position;
                    break;
                case EBodySide.Bottom:
                    FurthestGrabPointFromLeftHip = leftHandGrab.transform.position;
                    break;
            }
           
            CurrentlyGrabbing.Add(leftHandGrab);

            //set the closest to the right hand
            for (int i = 0; i < ListOfClosestGrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(Center, ListOfClosestGrabPoints[i].transform.position);
                if (distanceToCheck > longestRightDistance & ListOfClosestGrabPoints[i] != leftHandGrab)
                {
                    rightHandGrab = ListOfClosestGrabPoints[i];
                }
            }
            //to indicate which is the closest to the right hand
            switch (CurrenBodySide)
            {
                case EBodySide.Top:
                    FurthestGrabPointFromRightShoulder = rightHandGrab.transform.position;
                    break;
                case EBodySide.Bottom:
                    FurthestGrabPointFromRightHip = rightHandGrab.transform.position;
                    break;
            }
            CurrentlyGrabbing.Add(rightHandGrab);

            ListCleanup(ListOfClosestGrabPoints);
         
        }

    }
    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIkConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIkConstraint.data.root.transform.position;
        //test if bug depends on left only less
        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);

        if (isLeftCloser)
        {
           

            CurrenBodySide = EBodySide.Top;
            CurrentIkConstraint = _leftIkConstraintChain;
            CurrentMultiRotationConstraint = _leftMultiRotConstraint;

        }
        else
        {
        
            CurrenBodySide = EBodySide.Bottom;
            CurrentIkConstraint = _rightIkConstraintChain;
            CurrentMultiRotationConstraint = _rightMultiRotConstraint;

        }
        CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
        CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
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
