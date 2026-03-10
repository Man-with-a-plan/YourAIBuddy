using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GrabbingContext 
{
  public enum EBodySide {  Left, Right };

  private TwoBoneIKConstraint _leftIkConstraint;
  private TwoBoneIKConstraint _rightIkConstraint;
  private MultiRotationConstraint _leftMultiRotConstraint;
  private MultiRotationConstraint _rightMultiRotConstraint;
  private CharacterController _characterController;
    private Transform _rootTransform;
    private ChainIKConstraint _leftIkConstraintChain;
    private ChainIKConstraint _rightIkConstraintChain;

    public GrabbingContext(ChainIKConstraint leftIkConstraint, ChainIKConstraint rightIkConstraints,
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotConstraint,
        CharacterController characterController, Transform rootTransform)
    {
        _leftIkConstraintChain = leftIkConstraint;
        _rightIkConstraintChain = rightIkConstraints;
        _leftMultiRotConstraint = leftMultiRotationConstraint;
        _rightMultiRotConstraint = rightMultiRotConstraint;
        _characterController = characterController;
        _rootTransform = rootTransform;
    }

    public ChainIKConstraint LeftIkConstraint => _leftIkConstraintChain;
    public ChainIKConstraint RightIkConstraint => _rightIkConstraintChain;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotConstraint;

    public CharacterController CharacterController => _characterController;
    public Transform RootTransform => _rootTransform;

    public ChainIKConstraint CurrentIkConstraint;
    public MultiRotationConstraint CurrentMultiRotationConstraint;
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrenBodySide {  get; private set; }
    public List<GameObject> GrabPoints { get; private set; } = new List<GameObject>();
    //List of game objects that character is currently grabbing. Later will be used to defy his rotation in GrabbingState
    public List<GameObject> CurrentlyGrabbing = new List<GameObject>();

    //to debug which grab point is being grabbed
    public Vector3 ClosestGrabPointFromLeftShoulder { get; private set; } = Vector3.positiveInfinity;
    public Vector3 ClosestGrabPointFromRightShoulder { get; private set; } = Vector3.positiveInfinity;

    //To optimise can change to Vector3
    protected GameObject leftHandGrab, rightHandGrab;


    public void SetPointsToGrabForBothHands()
    {
        if (GrabPoints.Count > 0)
        {
            Vector3 leftShoulder = LeftIkConstraint.data.root.transform.position;
            Vector3 rightShoulder = RightIkConstraint.data.root.transform.position;
            float shortestLeftDistance = Vector3.Distance(leftShoulder, GrabPoints[0].transform.position);
            float shortestRightDistance = Vector3.Distance(rightShoulder, GrabPoints[0].transform.position);
            leftHandGrab = GrabPoints[0];
            rightHandGrab = GrabPoints[0];
            float distanceToCheck = float.MaxValue;

            for (int i = 0; i < GrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(leftShoulder, GrabPoints[i].transform.position);
                if (distanceToCheck < shortestLeftDistance )
                {
                    leftHandGrab = GrabPoints[i];

                }
            }
            //to indicate which is the closest to the left hand
            ClosestGrabPointFromLeftShoulder = leftHandGrab.transform.position;

            //set the closest to the right hand
            for (int i = 0; i < GrabPoints.Count; i++)
            {
                distanceToCheck = Vector3.Distance(rightShoulder, GrabPoints[i].transform.position);
                if (distanceToCheck < shortestRightDistance )
                {
                    rightHandGrab = GrabPoints[i];
                }
            }
            //to indicate which is the closest to the right hand
            ClosestGrabPointFromRightShoulder = rightHandGrab.transform.position;
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
            Debug.Log("Left");
            
            CurrenBodySide = EBodySide.Left;
            CurrentIkConstraint = _leftIkConstraintChain;
            CurrentMultiRotationConstraint = _leftMultiRotConstraint;

        }
        else {
            Debug.Log("Right");
            CurrenBodySide = EBodySide.Right;
            CurrentIkConstraint = _rightIkConstraintChain;
            CurrentMultiRotationConstraint = _rightMultiRotConstraint;

        }
        CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
        CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
    }

}
