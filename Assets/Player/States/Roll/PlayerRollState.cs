using UnityEngine;
using System.Collections;
public class PlayerRollState : PlayerBaseState
{
    private IEnumerator rollCoroutine;

    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerRollState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {

        Ctx.RequireRollDashPressed = true;

        // set current state string
        Ctx.DebugCurrentState = "Roll";

        // change animation state
        Ctx.ChangeAnimationState("Roll");


        Ctx.CurrentMovementX = Ctx.RollSpeed;

        // initialize and call roll coroutinerollCoroutine = Ctx.WaitCoroutine(Ctx.RollTime, Ctx.IsRollFinished);

        Ctx.StartCoroutine(Ctx.WaitCoroutine(Ctx.RollTime, (result) =>
        {
            Ctx.IsRollFinished = result;
        }));
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
        if (!Ctx.Controller2D.collisions.below)
        {
            Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
            // Calculate new _velocityY from gravity and timestep
            Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);
            // Clamp vertical velocity in between +/- of MaxFallSpeed;
            Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MaxFallSpeed, Ctx.MaxFallSpeed);
        }
        else
        {
            Ctx.CurrentMovementY = -0.05f;
        }

    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        Ctx.IsRollFinished = false;
    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (Ctx.IsRollFinished && !Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Airborne());
        }
        else if (Ctx.IsRollFinished && Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        }
    }
}