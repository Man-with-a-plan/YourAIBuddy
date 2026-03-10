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

  //  [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    //[SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotConstraint;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private ChainIKConstraint _leftIkConstraintChain;
    [SerializeField] private ChainIKConstraint _rightIkConstraintChain;



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (_context != null && _context.ClosestGrabPointFromLeftShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestGrabPointFromLeftShoulder, 0.3f);
        }
        Gizmos.color = Color.red;
        if (_context != null && _context.ClosestGrabPointFromRightShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestGrabPointFromRightShoulder, 0.3f);
        }
    }

    private void Awake()
    {
        ValidateConstraints();
        _context = new GrabbingContext(_leftIkConstraintChain, _rightIkConstraintChain, _leftMultiRotConstraint, _rightMultiRotConstraint,
            _characterController, transform.root);
        InitializeStates();
        ConstructEnvironmentDetectionCollider();
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
        SphereCollider boxCollider = gameObject.AddComponent<SphereCollider>();
        boxCollider.radius = wingspan/2;
        boxCollider.center = new Vector3(_characterController.center.x, _characterController.center.y +(.1f * wingspan), _characterController.center.z+(.5f*wingspan));
        boxCollider.isTrigger = true;
    }
}
