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
        Grab,
        Reset,
    }

    private GrabbingContext _context;
    public Dictionary<string, GameObject> CurrentlyGrabbing => _context.CurrentlyGrabbing;

    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
   [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotConstraint;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private ChainIKConstraint _leftIkConstraintChain;
    [SerializeField] private ChainIKConstraint _rightIkConstraintChain;
    [SerializeField] public float _reachDistance = 2f;
    


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (_context != null && _context.FurthestGrabPointFromLeftShoulder != null)
        {
            Gizmos.DrawSphere(_context.FurthestGrabPointFromLeftShoulder, 0.3f);
        }
        Gizmos.color = Color.red;
        if (_context != null && _context.FurthestGrabPointFromRightShoulder != null)
        {
            Gizmos.DrawSphere(_context.FurthestGrabPointFromRightShoulder, 0.3f);
            
        }
    
    }

    private void Awake()
    {
        ValidateConstraints();
        _context = new GrabbingContext(_leftIkConstraintChain, _rightIkConstraintChain, _leftIkConstraint,_rightIkConstraint, _leftMultiRotConstraint, _rightMultiRotConstraint,
            _characterController, transform.root);
        InitializeStates();
        ConstructEnvironmentDetectionCollider();
        ConstructEnvironmentDetectionColliderForLegs();
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
        States.Add(EGrabbingState.Grab, new GrabState(_context, EGrabbingState.Grab));
        States.Add(EGrabbingState.Reset, new ResetState(_context, EGrabbingState.Reset));
        CurrentState = States[EGrabbingState.Search];
    }
    private void ConstructEnvironmentDetectionCollider()
    {
        float wingspan = _characterController.height;
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
      //  sphereCollider.size = new Vector3(wingspan/_reachDistance, wingspan/_reachDistance, wingspan/_reachDistance);
       // sphereCollider.center = new Vector3(_characterController.center.x, _characterController.center.y +(.2f * wingspan), _characterController.center.z+0.5f);
       sphereCollider.radius = wingspan/_reachDistance;
       sphereCollider.center = new Vector3(_characterController.center.x, _characterController.center.y + (.33f * wingspan), _characterController.center.z+0.2f);
      sphereCollider.isTrigger = true;
        _context.ArmCollider = sphereCollider;
    }
    private void ConstructEnvironmentDetectionColliderForLegs()
    {
        float wingspan = _characterController.height;
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        //  sphereCollider.size = new Vector3(wingspan/_reachDistance, wingspan/_reachDistance, wingspan/_reachDistance);
        // sphereCollider.center = new Vector3(_characterController.center.x, _characterController.center.y +(.2f * wingspan), _characterController.center.z+0.5f);
        sphereCollider.radius = wingspan / _reachDistance;
        sphereCollider.center = new Vector3(_characterController.center.x, _characterController.center.y - (.31f * wingspan), _characterController.center.z+0.2f);
       sphereCollider.isTrigger = true;
        _context.LegCollider = sphereCollider;
    }
  
}
