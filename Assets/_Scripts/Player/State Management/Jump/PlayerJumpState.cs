using UnityEngine;
public class PlayerJumpState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    private float _targetPosX;
    private float _wallDirX;
    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called

    public override void EnterState()
    {
        // start animation
        Ctx.ChangeAnimationState("Jump");

        // set current state string
        Ctx.DebugCurrentState = "Jump";

        // require new jump press to jump again
        Ctx.RequireJumpPressed = true;

        // If the player cannot wall jump jump regularly
        if (!Ctx.CanWallJump)
        {
            // apply jump velocity
            Ctx.VelocityY = Ctx.InitialJumpVelocity;
        }
        
        // otherwise apply velocity depending on if the player is holding against the wall, no input or off of the wall during the wall jump.
        else
        {
            _wallDirX = Ctx.Controller2D.collisions.left ? -1 : 1;

            if (_wallDirX == Ctx.MoveInputX)
            {
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[0].x;
                Ctx.VelocityY = Ctx.WallJumps[0].y;

            }
            else if (Ctx.MoveInputX == 0)
            {
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[1].x;
                Ctx.VelocityY = Ctx.WallJumps[1].y;

            }
            else
            {
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[2].x;
                Ctx.VelocityY = Ctx.WallJumps[2].y;

            }
            Ctx.TimeToWallUnstick = 0;
            Ctx.CanWallJump = false;
        }
    }


    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // check to see if the current state should switch
        CheckSwitchStates();

    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        if (Ctx.MoveInputX > 0)
        {
        Ctx.TargetDirection = 1;
    }
    else if (Ctx.MoveInputX < 0)
        {
        Ctx.TargetDirection = -1;
        }
        else if (Ctx.MoveInputX == 0)
        {
            Ctx.TargetDirection = 0;

        }

        Ctx.CurrentMovementX = Ctx.MaxFootSpeed * Ctx.MoveInputX * Ctx.DeltaTime;

        Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
       
        // Calculate new VelocityY from gravity and timestep

        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);

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

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.CurrentMovementY <= 0 || Ctx.Controller2D.collisions.above)
        {
            SwitchState(Factory.Airborne());
        }
    }
}