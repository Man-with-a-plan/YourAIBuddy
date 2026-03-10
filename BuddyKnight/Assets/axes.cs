using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class axes : MonoBehaviour
{
    public UnityEvent foundAthing;

    private void Start()
    {
        foundAthing.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>().foundAxes);
    }
    private Vector3 GetClosestPoint(Collider collider, Vector3 positionToCheck)
    {
        return collider.ClosestPoint(positionToCheck);
    }
    private void OnTriggerStay(Collider collision)
    {
        
        if (collision.CompareTag("Player") & Input.GetKey(KeyCode.R ))
        {
            
            foundAthing.Invoke();
            Destroy(gameObject);
        }
      
    }



}
