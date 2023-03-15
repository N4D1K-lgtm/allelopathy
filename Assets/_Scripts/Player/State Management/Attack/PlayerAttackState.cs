using UnityEngine;
using System.Collections;
public class PlayerAttackState : PlayerBaseState
{

    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

// this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
public override void EnterState()
    {
     

        if (Ctx.AttackCooldown <= 0)
        {
            Ctx.NumConsecutiveAttacks = 0;
        }

        switch (Ctx.NumConsecutiveAttacks)
            {
                case 0:
                    Ctx.DebugCurrentState = "Attack 1";
                    Ctx.ChangeAnimationState("Attack 1");
                    break;
                case 1:
                    Ctx.DebugCurrentState = "Attack 2";
                    Ctx.ChangeAnimationState("Attack 2");
                    break;
                case 2:
                    Ctx.DebugCurrentState = "Attack 3";
                    Ctx.ChangeAnimationState("Attack 3");
                    break;
                case 3:
                    Ctx.DebugCurrentState = "Attack 4";
                    Ctx.ChangeAnimationState("Attack 4");
                    break;
                default:

                    break;
                    
            }
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
        Ctx.CurrentMovementX = 0;
        Ctx.CurrentMovementY = -0.05f;


        if (Ctx.CheckAttackCollisions == true)
        {
            Attack();
            Ctx.CheckAttackCollisions = false;
        }

        if (Ctx.AttackIsFinished)
        { 
            if (Ctx.NumConsecutiveAttacks >= 3)
            {
                Ctx.NumConsecutiveAttacks = 0;
                Ctx.DebugCurrentState = "Attack 1";
                Ctx.ChangeAnimationState("Attack 1");
            }
            else
            {
                Ctx.NumConsecutiveAttacks += 1;
                switch(Ctx.NumConsecutiveAttacks)
                { 
                    case 1:
                        Ctx.DebugCurrentState = "Attack 2";
                        Ctx.ChangeAnimationState("Attack 2");
                        break;
                    case 2:
                        Ctx.DebugCurrentState = "Attack 3";
                        Ctx.ChangeAnimationState("Attack 3");
                        break;
                    case 3:
                        Ctx.DebugCurrentState = "Attack 4";
                        Ctx.ChangeAnimationState("Attack 4");
                        break;
                        default:

                        break;

                }
            }
                Ctx.AttackIsFinished = false;
        }

        
    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        if(Ctx.NumConsecutiveAttacks < 3)
        {
            Ctx.AttackCooldown = Ctx.AttackCooldownLength;
            Ctx.NumConsecutiveAttacks += 1;
        }  else
        {
            Ctx.NumConsecutiveAttacks = 0;
        }

        Ctx.AttackIsFinished = false;
    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if(Ctx.AttackIsFinished && Ctx.Controller2D.collisions.below && !Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Grounded());
        } else if(Ctx.AttackIsFinished && !Ctx.Controller2D.collisions.below && !Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Airborne());
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(Ctx.AttackPoint.position, Ctx.AttackRange, Ctx.EnemyLayer);
        
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We Hit " + enemy.name);
        }
    }
}