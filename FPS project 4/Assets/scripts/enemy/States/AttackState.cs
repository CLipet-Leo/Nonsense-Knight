using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseSate
{
    private float moveTimer;
    private float losePlayerTimer;

    public override void Enter()
    {
        enemy.GetComponent<enemyGunBehaviour>().currentGun.GetComponent<GenericWeaponControl>().shooting = true;
    }

    public override void Exit()
    {
        enemy.GetComponent<enemyGunBehaviour>().currentGun.GetComponent<GenericWeaponControl>().shooting = false;
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            if (moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                
                moveTimer = 0;
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 8)
            {
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }
}
