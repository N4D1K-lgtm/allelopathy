using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Require Controller2D script on gameobject
[RequireComponent(typeof(Controller2D))]
public class OldPlayerScript : MonoBehaviour
{

    Controller2D controller;

    private PlayerActionControls playerActionControls;

    // Final velocity(really this is the calculated movement not velocity) passed to Move() function in the character controller
    Vector3 velocity;
    // The velocity calculated w/ timestep and gravity
    float _velocityY;
    // The previous frame's velocity
    float oldVelocityY;

    // Handled by SmoothDampFunction
    float velocityXSmoothing;

    // Calculated from jump apex, height and horizontal movespeed
    private float gravity;

    // Horizontal movement variables
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float maxHorizontalSpeed = 3f;
    public float horizontalSpeed = .04f;

    // Jump height and force variables
    // To Do: Change jumpForce to depend on horizontal movement and jump height;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 3f;
    public float maxTimeToJumpApex = .225f;
    public float minTimeToJumpApex = .1112f;
    public float maxVerticalVelocity = 20f;

    float maxJumpForce => 2 * maxJumpHeight / maxTimeToJumpApex;
    float minJumpForce => 2 * minJumpHeight / minTimeToJumpApex;

    public float wallSlideSpeedMax = 7f;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    public Vector2 wallJumpSmall = new Vector2(0.03f, 20);
    public Vector2 wallJumpMed = new Vector2(0.04f, 20);
    public Vector2 wallJumpBig = new Vector2(0.04f, 25);

    void Awake()
    {
        // Access 2DController class as controller
        controller = GetComponent<Controller2D>();

        // Initialize Input Action Map
        playerActionControls = new PlayerActionControls();
        playerActionControls.Gameplay.Jump.performed += context => OnJump(context);
        playerActionControls.Gameplay.Jump.canceled += context => OnJump(context);
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        playerActionControls.Enable();
    }

    private void OnDisable()
    {
        playerActionControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {

        // Initialize input vector for player movement
        Vector2 movementInput = playerActionControls.Gameplay.Move.ReadValue<Vector2>();
        int wallDirX = controller.collisions.left ? -1 : 1;
        float targetVelocityX = movementInput.x * horizontalSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne, maxHorizontalSpeed, Time.deltaTime);

        HandleWallSlide(wallDirX, movementInput);

        // Save last calculated _velocity to oldVelocity
        oldVelocityY = _velocityY;
        // Calculate new _velocityY from gravity and timestep
        _velocityY += gravity * Time.deltaTime;

        // Apply average oldVelocityY and new _velocityY * timestep
        velocity.y = (oldVelocityY + _velocityY) * .5f * Time.deltaTime;

        // Clamp vertical velocity in between +/- of maxVerticalVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalVelocity, maxVerticalVelocity);

        // Call move function in 2DController
        Debug.Log(velocity.y);
        Debug.Log(controller.collisions.below);
        controller.Move(velocity);

        //If 2DController detects a collision above or below player
        if (controller.collisions.below)
        {
            // Stop the player from moving
            velocity.y = 0;
            // Prevent gravity from being applied to _velocityY
            gravity = 0;
        }
        else
        {

            // If in the air apply gravity
            gravity = -2 * maxJumpHeight / (maxTimeToJumpApex * maxTimeToJumpApex);
        }

        if (controller.collisions.above)
        {
            // If player collides with ceiling, set velocity.y to 0 to stop player movement
            velocity.y = 0;
            // Prevent the previous frames velocity being applied to the player (this is to prevent hang time on the ceiling if a jump is interrupted)
            _velocityY = 0;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            Vector2 movementInput = playerActionControls.Gameplay.Move.ReadValue<Vector2>();
            int wallDirX = (controller.collisions.left) ? -1 : 1;
            Debug.Log("Jump Performed");
            if (IsSliding())
            {
                if (wallDirX == movementInput.x)
                {
                    velocity.x = -wallDirX * wallJumpSmall.x;
                    _velocityY = wallJumpSmall.y;
                    Debug.Log("wallClimb");
                }
                else if (movementInput.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpMed.x;
                    _velocityY = wallJumpMed.y;
                    Debug.Log("wallJumpMed");
                }
                else
                {
                    velocity.x = -wallDirX * wallJumpBig.x;
                    _velocityY = wallJumpBig.y;
                    Debug.Log("wallJumpBig");
                }
                timeToWallUnstick = 0;
            }
            if (controller.collisions.below)
            {
                _velocityY = maxJumpForce;
            }
        }

        if (context.canceled)
        {
            Debug.Log(velocity.y);
            Debug.Log(_velocityY);
            if (_velocityY > minJumpForce)
            {
                _velocityY = minJumpForce;
            }
        }
    }


    private void HandleWallSlide(int wallDirX, Vector2 movementInput)
    {

        if (IsSliding())
        {
            _velocityY = Mathf.Max(_velocityY, -wallSlideSpeedMax);

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (movementInput.x != wallDirX && movementInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;

                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private bool IsSliding()
    {
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}