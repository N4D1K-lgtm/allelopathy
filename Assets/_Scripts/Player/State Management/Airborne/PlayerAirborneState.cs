using UnityEngine;
public class PlayerAirborneState : PlayerBaseState
{
    private float _targetPosX;
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerAirborneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();

    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();


    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        if (Ctx.MoveInputX > 0)
        {
            
        }
        else if (Ctx.MoveInputX < 0)
        {
            
        }
        else if (Ctx.MoveInputX == 0)
        {
            Ctx.TargetDirection = 0;

        }

        Ctx.CurrentMovementX = Ctx.MaxFootSpeed * Ctx.MoveInputX * Ctx.DeltaTime;


        if (Ctx.Controller2D.collisions.above)
        {
            //If player collides with ceiling, set velocity.y to 0 to stop player movement
            Ctx.VelocityY = 0;

        }


        Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
        // Calculate new _velocityY from gravity and timestep
        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);
        // Clamp vertical velocity in between +/- of MaxFallSpeed;
        Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MaxFallSpeed, Ctx.MaxFallSpeed);

        if (Ctx.MoveInputX < 0)
        {
            Ctx.SpriteRenderer.flipX = true;


        }
        else if (Ctx.MoveInputX > 0)
        {
            Ctx.SpriteRenderer.flipX = false;

        }

        if (Ctx.AttackCooldown >= 0)
        {
            Ctx.AttackCooldown -= 1 * Ctx.DeltaTime;
        }
    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
        if (Ctx.CurrentMovementY <= 0 && (Ctx.Controller2D.collisions.left || Ctx.Controller2D.collisions.left))
        {
            SetSubState(Factory.WallSlide());
        }
        else if (Ctx.CurrentMovementY < 0 && (!Ctx.Controller2D.collisions.left || !Ctx.Controller2D.collisions.right))
        {
            SetSubState(Factory.Falling());
        }
    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        // if player is grounded and jump is pressed, switch to jump state
        if (Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsJumpPressed && Ctx.CanWallJump && !Ctx.RequireJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsRollDashPressed && Ctx.CanDash && !Ctx.RequireRollDashPressed)
        {
            SwitchState(Factory.Dash());
        }
    }
}