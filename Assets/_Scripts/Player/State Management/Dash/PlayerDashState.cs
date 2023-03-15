using UnityEngine;
public class PlayerDashState : PlayerBaseState
{

    private float _targetVelocityX;
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

        IsRootState = true;

    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {

        Ctx.CanDash = false;
        Ctx.RequireRollDashPressed = true;

        // set current state string
        Ctx.DebugCurrentState = "Dash";

        // change animation state
        Ctx.ChangeAnimationState("Dash");



        // initialize and call roll coroutinerollCoroutine = Ctx.WaitCoroutine(Ctx.RollTime, Ctx.IsRollFinished);

        Ctx.StartCoroutine(Ctx.WaitCoroutine(Ctx.DashTime, (result) =>
        {
            Ctx.IsDashFinished = result;
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


    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        Ctx.IsDashFinished = false;
    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (Ctx.IsDashFinished == true && !Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Airborne());
        }
        else if (Ctx.IsDashFinished == true && Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        }
    }
}