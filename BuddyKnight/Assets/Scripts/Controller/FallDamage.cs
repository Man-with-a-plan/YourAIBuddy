using StarterAssets;
using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [Tooltip("Minimum fall speed required to take damage")]
    public float FallSpeedThreshold = 10f;

    [Tooltip("Multiplier applied to fall speed to calculate damage")]
    public float FallDamageMultiplier = 1.5f;

    [Tooltip("Fall damage value that triggers animator disable")]
    public float CriticalDamageThreshold = 25f;

    private ThirdPersonController _thirdPersonController;
    private Animator _animator;
    private float _lastVelocityBeforeImpact = 0f;
    private bool _canTakeDamage = true;

    private void Start()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Store the vertical velocity before collision resets it
        if (!_thirdPersonController.Grounded)
        {
            // Access the private _verticalVelocity through reflection or use a public getter
            // For now, we'll track it via the public Gravity property
            _lastVelocityBeforeImpact = GetVerticalVelocityFromController();
        }
        else if (_canTakeDamage && _lastVelocityBeforeImpact < -FallSpeedThreshold)
        {
            // Player just landed - calculate damage
            CalculateFallDamage(_lastVelocityBeforeImpact);
            _canTakeDamage = false;
        }
        else if (_thirdPersonController.Grounded)
        {
            _canTakeDamage = true;
        }
    }

    private float GetVerticalVelocityFromController()
    {
        // Access the ThirdPersonController's private _verticalVelocity field
        var field = typeof(ThirdPersonController).GetField("_verticalVelocity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (float)field.GetValue(_thirdPersonController);
        }

        return 0f;
    }

    private void CalculateFallDamage(float verticalVelocity)
    {
        // Only calculate damage on downward impact
        if (verticalVelocity < -FallSpeedThreshold)
        {
            // Calculate fall damage based on velocity
            float fallDamage = (Mathf.Abs(verticalVelocity) - FallSpeedThreshold) * FallDamageMultiplier;

            Debug.Log($"Fall Damage Taken: {fallDamage:F2} (Velocity: {verticalVelocity:F2})");

            // Check if damage exceeds critical threshold
            if (fallDamage >= CriticalDamageThreshold)
            {
                DisableAnimator();
                Debug.LogWarning($"CRITICAL FALL DAMAGE: {fallDamage:F2}. Animator disabled!");
            }
        }
    }

    private void DisableAnimator()
    {
        if (_animator != null)
        {
            _animator.enabled = false;
        }
        else
        {
            Debug.LogError("Animator component not found on this GameObject");
        }
    }

    /// <summary>
    /// Re-enables the animator component if it was disabled
    /// </summary>
    public void ReenableAnimator()
    {
        if (_animator != null)
        {
            _animator.enabled = true;
            Debug.Log("Animator re-enabled");
        }
    }
}