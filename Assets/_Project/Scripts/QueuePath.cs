using System.Collections.Generic;
using UnityEngine;

public class QueuePath : Path
{
    public int capacity = 5;
    public List<Visitor> visitorsOnQueuePath = new List<Visitor>();
    public QueuePath nextQueuePath;
    public Attraction hostAttraction;


    public void RemoveVisitor(Visitor visitor)
    {
        if (visitorsOnQueuePath.Contains(visitor))
        {
            visitorsOnQueuePath.Remove(visitor);
        }
    }

    public QueuePath(string name, int baseCost) : base(name, baseCost)
    {
        visitorsOnQueuePath = new List<Visitor>(); // Inicjalizacja listy
    }

    private void Awake()
    {
        if (visitorsOnQueuePath == null)
        {
            visitorsOnQueuePath = new List<Visitor>();
        }
    }

    public bool IsFull()
    {
        if (visitorsOnQueuePath == null) return false; // Sprawdzenie, czy lista istnieje
        return visitorsOnQueuePath.Count >= capacity;
    }

    public bool IsEmpty()
    {
        if (visitorsOnQueuePath == null) return true;
        return visitorsOnQueuePath.Count == 0;
    }

    public void EnqueueVisitor(Visitor visitor)
    {

        if (hostAttraction == null)
        {
            Debug.LogWarning($"[QueuePath] Kolejka {this.name} NIE MA przypisanej atrakcji!");
            return;
        }

        List<QueuePath> queuePaths = hostAttraction.queuePaths;
        int index = queuePaths.IndexOf(this);

        if (index == -1)
        {
            Debug.LogError($"[QueuePath] {this.name} nie znajduje siê na liœcie kolejek atrakcji {hostAttraction.name}!");
            return;
        }

        if (!IsFull()) // Jeœli kolejka ma miejsce, dodaj visitora tutaj
        {
            visitorsOnQueuePath.Add(visitor);
            visitor.MoveNPC(visitor.GetCurrentGridPosition(), coordinates[0]);
            Debug.Log($"[EnqueueVisitor] Visitor {visitor.name} dodany do kolejki {this.name} (pozycja: {visitorsOnQueuePath.Count}/{capacity})");
        }
        else if (index < queuePaths.Count - 1) // Jeœli kolejka jest pe³na i istnieje wczeœniejsza kolejka
        {
            QueuePath previousQueue = queuePaths[index + 1]; // Kolejka dalej od atrakcji

            Debug.Log($"[EnqueueVisitor] Kolejka {this.name} jest pe³na, próbujê przesun¹æ visitora do {previousQueue.name}");

            // Wywo³ujemy rekurencyjnie EnqueueVisitor dla wczeœniejszej kolejki
            previousQueue.EnqueueVisitor(visitor);

        }
        else
        {
            Debug.LogWarning($"[EnqueueVisitor] Visitor {visitor.name} nie ma miejsca w ¿adnej kolejce i pozostaje w miejscu.");
        }
    }



    public Visitor DequeueVisitor()
    {
        if (!IsEmpty())
        {
            Visitor visitor = visitorsOnQueuePath[0];
            visitorsOnQueuePath.RemoveAt(0);
            return visitor;
        }
        return null;
    }



}