using UnityEngine;
public class PlayerGroundedState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

        IsRootState = true;
        InitializeSubState();
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {

        Ctx.CanWallJump = false;
        Ctx.CanDash = true;
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
        Ctx.VelocityY = 0;
        Ctx.CurrentMovementY = -.05f;

        if (Ctx.MoveInputX < 0)
        {
            Ctx.SpriteRenderer.flipX = true;
            Ctx.AttackPoint.position = new Vector3(Ctx.Transform.position[0] - 30, Ctx.Transform.position[1] - 4, 0);



        }
        else if (Ctx.MoveInputX > 0)
        {
            Ctx.SpriteRenderer.flipX = false;
            Ctx.AttackPoint.position = new Vector3(Ctx.Transform.position[0] + 30, Ctx.Transform.position[1] - 4, 0);


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
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        // if player is not grounded switch to airborne state
        if (!Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Airborne());
        }
        // if player is grounded and jump is pressed, switch to jump state
        else if (Ctx.IsJumpPressed && !Ctx.RequireJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
        // if player is grounded and attack is pressed swiwtch to attack state
        else if (Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Attack());
        }
        // if player is grounded and roll is pressed switch to roll
        else if (Ctx.IsRollDashPressed && !Ctx.RequireRollDashPressed)
        {
            SwitchState(Factory.Roll());
        }
    }
}