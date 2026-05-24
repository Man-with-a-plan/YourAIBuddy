using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
public class GrabbingStateMachine : StateManager<GrabbingStateMachine.EGrabbingState>
{
    public enum EGrabbingState
    {
        Search,
        Approach,
        StartClimbing,
        LeftGrab,
        RightGrab,
        Reset,
    }

    private GrabbingContext _context;
    
    public Dictionary<string, GameObject> CurrentlyGrabbing => _context.CurrentlyGrabbing;

    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
   [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotConstraint;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private ThirdPersonController _thirdPersonController;
    [SerializeField] private ChainIKConstraint _leftIkConstraintChain;
    [SerializeField] private ChainIKConstraint _rightIkConstraintChain;
    [SerializeField] public float _reachDistance = 2f;

   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (_context.FurthestGrabPointFromLeftShoulder != null)
        {
            for(int i = 0; i < _context.GrabPointsLeftArm.Count; i++)
            {
                Gizmos.DrawSphere(_context.GrabPointsLeftArm[i].transform.position, 0.1f);
            }
            for (int i = 0; i < _context.GrabPointsRightLeg.Count; i++)
            {
                Gizmos.DrawSphere(_context.GrabPointsRightLeg[i].transform.position, 0.1f);
            }

            Gizmos.DrawSphere(_context.FurthestGrabPointFromLeftShoulder, 0.3f);
            Gizmos.DrawSphere(_context.FurthestGrabPointFromLeftHip, 0.3f);
            Debug.Log("LeftShoulder: " + _context.FurthestGrabPointFromLeftShoulder);
        }
        Gizmos.color = Color.red;
       
        if (_context.FurthestGrabPointFromRightShoulder != null)
        {
            for (int i = 0; i < _context.GrabPointsRightArm.Count; i++)
            {
                Gizmos.DrawSphere(_context.GrabPointsRightArm[i].transform.position, 0.1f);
            }
            for (int i = 0; i < _context.GrabPointsLeftLeg.Count; i++)
            {
                Gizmos.DrawSphere(_context.GrabPointsLeftLeg[i].transform.position, 0.1f);
            }
            Gizmos.DrawSphere(_context.FurthestGrabPointFromRightShoulder, 0.3f);
            Gizmos.DrawSphere(_context.FurthestGrabPointFromRightHip, 0.3f);

        }
    
    }

    private void Awake()
    {
        
        ValidateConstraints();
        _context = new GrabbingContext(_leftIkConstraintChain, _rightIkConstraintChain, _leftIkConstraint,_rightIkConstraint, _leftMultiRotConstraint, _rightMultiRotConstraint,
            _characterController, _thirdPersonController, transform.root);
        //_context.CurrentlyGrabbing["RightLeg"] = new GameObject();
      //  _context.Normal = GetPlaneNormal();
        InitializeStates();
       _context.Owner = this;
       _context.StateMachine = this;
        _context.SetOriginalLimbPositions();

    }

    

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIkConstraintChain, "Left IK constraint is not assigned.");
        Assert.IsNotNull(_rightIkConstraintChain, "right Ik constraint is not assigned.");
        Assert.IsNotNull(_leftMultiRotConstraint, "Left multi rotation constraint is not assigned.");
        Assert.IsNotNull(_rightMultiRotConstraint, "right multi rotation constraint is not assigned.");
        Assert.IsNotNull(_characterController, "Character controller is not assigned.");
        
    }

    private void InitializeStates()
    {
        States.Add(EGrabbingState.Search, new SearchState(_context, EGrabbingState.Search));
        States.Add(EGrabbingState.Approach, new ApproachState(_context, EGrabbingState.Approach));
        States.Add(EGrabbingState.StartClimbing, new StartClimbingState(_context, EGrabbingState.StartClimbing));
        States.Add(EGrabbingState.LeftGrab, new LeftHandGrabState(_context, EGrabbingState.LeftGrab));
        States.Add(EGrabbingState.RightGrab, new RightHandGrabState(_context, EGrabbingState.RightGrab));
        States.Add(EGrabbingState.Reset, new ResetState(_context, EGrabbingState.Reset));
        CurrentState = States[EGrabbingState.Reset];
    }
    public Vector3 GetPlaneNormal()
    {
        return _context.GetPlaneNormal();
    }

    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopRunningCoroutine(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

}
