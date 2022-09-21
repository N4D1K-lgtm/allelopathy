using UnityEngine;
public class PlayerWallSlideState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    private int _wallDirX;
    // private bool _isFlipped;

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {

        Ctx.DebugCurrentState = "WallSlide";
        // _isFlipped = Ctx.SpriteRenderer.flipX;
        Ctx.SpriteRenderer.flipX = false;
        _wallDirX = Ctx.Controller2D.collisions.left ? -1 : 1;



        // start playing animation
        if (_wallDirX == 1)
        {
            Ctx.ChangeAnimationState("Wall Slide Right");

        }
        else if (_wallDirX == -1)
        {
            Ctx.ChangeAnimationState("Wall Slide Left");

        }

        Ctx.CanWallJump = true;
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

        Ctx.CurrentMovementY = Ctx.WallSlideSpeed;

        if (Ctx.TimeToWallUnstick > 0)
        {

            Ctx.CurrentMovementX = 0;

            if (Ctx.MoveInputX != _wallDirX && Ctx.MoveInputX != 0)
            {
                Ctx.TimeToWallUnstick -= Ctx.DeltaTime;

            }
            else
            {
                Ctx.TimeToWallUnstick = Ctx.WallStickTime;
            }
        }
        else
        {
            Ctx.TimeToWallUnstick = Ctx.WallStickTime;
        }

        Ctx.SpriteRenderer.flipX = false;

    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        // Why is this here
        Ctx.VelocityY = 0;



        if (_wallDirX == 1)
        {
            Ctx.SpriteRenderer.flipX = true;

        }
        else if (_wallDirX == -1)
        {
            Ctx.SpriteRenderer.flipX = false;

        }
    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (!Ctx.Controller2D.collisions.left && !Ctx.Controller2D.collisions.right)
        {
            SwitchState(Factory.Falling());
        }
    }
}