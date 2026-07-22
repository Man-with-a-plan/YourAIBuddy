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
    [Header("Gizmo Visualization")]
    [SerializeField] private bool _showGrabPointGizmos = true;
    [SerializeField] private float _gizmoSphereRadius = 0.1f;
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
    private void OnDrawGizmosSelected()
    {
        if (!_showGrabPointGizmos || _context == null)
            return;

        // Left Arm - Light Blue
        DrawGrabPointList(GrabPointsLeftArm, new Color(0.5f, 0.8f, 1f, 0.7f), "LeftHand");

        // Right Arm - Light Green
        DrawGrabPointList(GrabPointsRightArm, new Color(0.5f, 1f, 0.5f, 0.7f), "RightHand");

        // Left Leg - Light Orange
        DrawGrabPointList(GrabPointsLeftLeg, new Color(1f, 0.8f, 0.5f, 0.7f), "LeftLeg");

        // Right Leg - Light Pink/Magenta
        DrawGrabPointList(GrabPointsRightLeg, new Color(1f, 0.5f, 0.8f, 0.7f), "RightLeg");

        // Draw currently grabbing points with larger spheres and lines to labels
        DrawCurrentlyGrabbingPoints();
    }

    private void DrawGrabPointList(List<GameObject> grabPoints, Color color, string limbName)
    {
        if (grabPoints == null || grabPoints.Count == 0)
            return;

        Gizmos.color = color;

        foreach (GameObject point in grabPoints)
        {
            if (point == null) continue;

            // Draw sphere for available grab point
            Gizmos.DrawSphere(point.transform.position, _gizmoSphereRadius);
        }
    }

    private void DrawCurrentlyGrabbingPoints()
    {
        if (CurrentlyGrabbing == null || CurrentlyGrabbing.Count == 0)
            return;

        // Define colors for each limb
        Dictionary<string, Color> limbColors = new Dictionary<string, Color>()
        {
            { "LeftHand", new Color(0.2f, 0.6f, 1f, 1f) },      // Bright Blue
            { "RightHand", new Color(0.2f, 1f, 0.2f, 1f) },     // Bright Green
            { "LeftLeg", new Color(1f, 0.6f, 0.2f, 1f) },       // Bright Orange
            { "RightLeg", new Color(1f, 0.2f, 0.8f, 1f) }       // Bright Magenta
        };

        foreach (var kvp in CurrentlyGrabbing)
        {
            string limbName = kvp.Key;
            GameObject grabPoint = kvp.Value;

            if (grabPoint == null) continue;

            Color limbColor = limbColors.ContainsKey(limbName) ? limbColors[limbName] : Color.white;

            // Draw larger sphere for grabbed point
            Gizmos.color = limbColor;
            Gizmos.DrawSphere(grabPoint.transform.position, _gizmoSphereRadius * 1.5f);

            // Draw a wire cube around it to make it more visible
            Gizmos.color = limbColor;
            Gizmos.DrawWireCube(grabPoint.transform.position, Vector3.one * _gizmoSphereRadius);

            // Draw line from grab point to center
            Vector3 center = _context.GetCenter();
            Gizmos.color = limbColor;
            Gizmos.DrawLine(grabPoint.transform.position, center);

            // Draw label offset (visual indicator)
            Vector3 labelOffset = grabPoint.transform.position + Vector3.up * 0.2f;
            Gizmos.color = limbColor;
            Gizmos.DrawSphere(labelOffset, _gizmoSphereRadius * 0.5f);
        }

        // Draw the center point
        Gizmos.color = Color.yellow;
        Vector3 centerPos = _context.GetCenter();
        Gizmos.DrawSphere(centerPos, _gizmoSphereRadius * 2f);
        Gizmos.DrawWireCube(centerPos, Vector3.one * _gizmoSphereRadius * 2f);
    }
}
