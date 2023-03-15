using UnityEngine;
public class PlayerWalkState : PlayerBaseState
{

    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = false;
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        Ctx.ChangeAnimationState("Walk");

        Ctx.DebugCurrentState = "Walk";
        Ctx.TargetDirection = 1;


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

        Ctx.CurrentMovementX = Ctx.MaxFootSpeed * Ctx.MoveInputX * Ctx.DeltaTime;
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


        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
            // else if movement is still pressed and run is pressed, switch to run
        }
    }
}