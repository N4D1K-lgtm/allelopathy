using UnityEngine;
public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;
    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerStateMachine Ctx { get { return _ctx; } }
    protected PlayerStateFactory Factory { get { return _factory; } }

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateStateLogic();

    public abstract void UpdateStatePhysics();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStatesLogic()
    {
        UpdateStateLogic();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStatesLogic();
        }
    }

    public void UpdateStatesPhysics()
    {
        UpdateStatePhysics();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStatesPhysics();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        // call the ExitState method of the current state
        ExitState();

        if (_currentSubState != null)
        {
            _currentSubState.ExitState();
        }

        // call the EnterState method of the new state
        newState.EnterState();

        if (newState._currentSubState != null)
        {
            newState._currentSubState.EnterState();
        }

        // if newState is rootState (top of the hierarchy) switch the current state
        if (_isRootState)
        {
            _ctx.CurrentState = newState;
            // else if newState has a superstate, set newState as the active substate of its superstate
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }

    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

}