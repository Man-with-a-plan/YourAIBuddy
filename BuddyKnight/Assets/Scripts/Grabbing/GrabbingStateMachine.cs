using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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
    public List<GameObject> GrabPointsLeftArm => _context.GrabPointsLeftArm;
    public List<GameObject> GrabPointsRightArm => _context.GrabPointsRightArm;
    public List<GameObject> GrabPointsLeftLeg => _context.GrabPointsLeftLeg;
    public List<GameObject> GrabPointsRightLeg => _context.GrabPointsRightLeg;

    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotConstraint;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private ThirdPersonController _thirdPersonController;
    [SerializeField] private ChainIKConstraint _leftIkConstraintChain;
    [SerializeField] private ChainIKConstraint _rightIkConstraintChain;
    [SerializeField] public float _reachDistance = 2f;

    private void Awake()
    {

        ValidateConstraints();
        _context = new GrabbingContext(_leftIkConstraintChain, _rightIkConstraintChain, _leftIkConstraint, _rightIkConstraint, _leftMultiRotConstraint, _rightMultiRotConstraint,
            _characterController, _thirdPersonController, transform.root);
        InitializeStates();
        _context.Owner = this;
        _context.StateMachine = this;
        _context.SetOriginalLimbPositions();
        RigCollisionHandler.NewPointEntered += _context.OnPointEntered;
        RigCollisionHandler.NewPointExited += _context.OnPointExited;
    }

    

    private void OnDisable()
    {
        // Unsubscribe from collision events when the state machine is disabled
        RigCollisionHandler.NewPointEntered -= _context.OnPointEntered;
        RigCollisionHandler.NewPointExited -= _context.OnPointExited;
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
    private void OnDestroy()
    {
        // Clean up any resources or references if needed
        // Unsubscribe from collision events when the state machine is disabled
        RigCollisionHandler.NewPointEntered -= _context.OnPointEntered;
        RigCollisionHandler.NewPointExited -= _context.OnPointExited;
    }
    public Vector3 GetPlaneNormal()
    {
        return _context.GetPlaneNormal();
    }

    public Vector3 GetCenter()
    {
        return _context.GetCenter();
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