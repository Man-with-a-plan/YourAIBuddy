using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class characterContrKnight : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded;

    [SerializeField] private CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] 
    private float playerSpeed, rotationSpeed, jumpSpeed, gravity;
    private Vector3 movementDirection = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Vector3 inputMovement = transform.forward * playerSpeed * Input.GetAxisRaw("Vertical");

        Vector3 fullMovement = inputMovement;
        fullMovement.y = movementDirection.y;
        controller.Move(fullMovement * Time.deltaTime);

        transform.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * rotationSpeed);

        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            movementDirection.y = jumpSpeed;
            
        }

        movementDirection.y -= gravity * Time.deltaTime;

        
        animator.SetBool("isWalking", Input.GetAxisRaw("Vertical") != 0);
        animator.SetBool("isJumping", !controller.isGrounded);
        
    }
}
