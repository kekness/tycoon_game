using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Structure
{
    public int foodCost = 10;
    public int foodLvl = 50;

    private void Awake()
    {
        Player.instance.shopList.Add(this);
        PathManager.instance.registerPath(this.coordinates[0]);
    }
    public void visit(Visitor visitor)
    {
        visitor.currentState = VisitorStates.TARGETING;
        visitor.MoveNPC(visitor.GetCurrentGridPosition(), this.coordinates[0]);
        StartCoroutine(WaitForArrival(visitor));
    }

    private IEnumerator WaitForArrival(Visitor visitor)
    {
        Vector2Int targetPos = this.coordinates[0];
        while (visitor.GetCurrentGridPosition() != targetPos)
            yield return null;

        // Dopiero tutaj wywo³aj Pay() i Eat()
        visitor.Pay(foodCost);
        visitor.Eat(foodLvl);
        visitor.currentState = VisitorStates.BOIDS;
    }
}
