using UnityEngine;
using System.Collections.Generic;

public class Attraction : Structure
{
    public bool isOpen = true;
    public int ticketCost = 10;
    public int todaysVisitations = 0;
    public ExitEntry entrance;
    public ExitEntry exit;
    public float timeRequired;
    public Sprite attractionSprite; 
    public void resetVisitations()
    {
        todaysVisitations = 0;
    }

    public void visit()
    {
        todaysVisitations++;
    }
}