using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Structure
{
    public float foodCost;
    public int foodLvl = 50;

    public void visit(Visitor visitor)
    {
        visitor.Pay(foodCost);
        visitor.eat(foodLvl);
    }
}
