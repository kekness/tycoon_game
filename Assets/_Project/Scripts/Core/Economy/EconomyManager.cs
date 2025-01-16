using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : BaseManager<EconomyManager>
{
    public float TotalIncome { get; private set; }
    public float TotalExpenses { get; private set; }
    public float CurrentBalance => TotalIncome - TotalExpenses;

    public override void InitializeManager()
    {
        TotalIncome = 0f;
        TotalExpenses = 0f;
    }

    public void AddIncome(float amount)
    {

    }

    public void AddExpense(float amount)
    {

    }

    public override void ResetManager()
    {
        TotalIncome = 0f;
        TotalExpenses = 0f;
    }
}
