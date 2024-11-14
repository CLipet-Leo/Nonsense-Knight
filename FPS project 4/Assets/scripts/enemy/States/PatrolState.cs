using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseSate
{
    public int waypointIndex;
    public float waitTimer;
    public bool isFirstWaypoint = true;

    public override void Enter()
    {
        isFirstWaypoint = true;
    }

    public override void Perform()
    {
        if (enemy.path != null && enemy.path.waypoints.Count != 0)
            PatrolCycle();
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    public override void Exit()
    {

    }

    public void PatrolCycle()
    {
        if (isFirstWaypoint)
        {
            waypointIndex = GetClosestWaypointIndex();
            enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
            isFirstWaypoint = false;
        }

        if (enemy.Agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 3)
            {
                if (waypointIndex < enemy.path.waypoints.Count - 1)
                    waypointIndex++;
                else
                    waypointIndex = 0;

                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
            }
        }
    }

    private int GetClosestWaypointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < enemy.path.waypoints.Count; i++)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.path.waypoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
