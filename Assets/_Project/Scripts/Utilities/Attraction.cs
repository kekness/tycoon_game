using UnityEngine;
using System.Collections.Generic;

public class Attraction : Structure
{
    bool isOpen = true;
    public int ticketCost=10;
    public ExitEntry entrance;
    public ExitEntry exit;
    public float timeRequired;
    
}
