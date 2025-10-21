using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float playerSpeed;

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var movement = new Vector3(horizontal, 0, vertical);

        if(movement.magnitude <= float.Epsilon)
        {
            return;
        }

        controller.Move(movement * playerSpeed * Time.deltaTime);
        gameObject.transform.forward = movement;
    }
}
