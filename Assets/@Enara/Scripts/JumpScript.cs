using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpScript : MonoBehaviour
{
    [SerializeField] private InputActionProperty jumpButton;
    [SerializeField] private InputActionProperty teleButton;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private CharacterController cc;
    //[SerializeField] private AudioClip audio1;
    //[SerializeField] AudioClip audio2;
    //[SerializeField] AudioSource source;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private int levelSize;

    private float gravity = Physics.gravity.y;
    private Vector3 movement;
    private bool canJump;

    private bool isHolding = false;
    private float holdDuration = 3.0f; // Duration the button needs to be held
    private Coroutine holdCoroutine;
    private float teleportTime = 0f;


    private void Awake()
    {
        jumpButton.action.Enable();
        jumpButton.action.performed += Action_performed;

        teleButton.action.Enable();
        teleButton.action.started += OnActionStarted;
        teleButton.action.canceled += OnActionCanceled;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (canJump)
        {
            Jump();
        }
    }

    private void OnActionStarted(InputAction.CallbackContext context)
    {
        isHolding = true;
    }

    private void OnActionCanceled(InputAction.CallbackContext context)
    {
        isHolding = false;
        teleportTime = 0f;
    }

    public void Update()
    {
        bool _isGrounded = IsGrounded();

        //if (_isGrounded && verticalVelocity < 0)
        //{
        //    verticalVelocity = -2f; // Pequeña fuerza hacia abajo para mantener al personaje pegado al suelo
        //}

        if (!_isGrounded)
        {
            if (movement.y >= gravity)
            {
                movement.y += gravity * Time.deltaTime;
            }
            canJump = false;
        }

        if (isHolding && teleportTime < holdDuration)
        {
            teleportTime += Time.deltaTime;
        }

        if (teleportTime >= holdDuration)
        {
            Teleport();
            teleportTime = 0f;
        }


        //verticalVelocity += gravity * Time.deltaTime;
        //verticalVelocity;

        // Mover al personaje
        cc.Move(movement * Time.deltaTime);
        //Debug.Log(canJump);
    }

    private void Jump()
    {
        movement.y = Mathf.Sqrt(jumpHeight * -0.3f * gravity);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsGrounded())
        {
            canJump = true;
            return;
        }
    }

    private void Teleport()
    {
        cc.enabled = false;
        transform.position = new Vector3(levelSize / 2, levelSize + 2, levelSize / 2);
        cc.enabled = true;
    }

    private void OnDestroy()
    {
        teleButton.action.started -= OnActionStarted;
        teleButton.action.canceled -= OnActionCanceled;
    }
}