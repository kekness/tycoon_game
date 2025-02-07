using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Attraction : Structure
{
    public bool isBroken = false;
    public bool isOpen = false;
    public bool isRunning = false; // Czy atrakcja jest w trakcie biegu?
    public int ticketCost = 10;
    public int todaysVisitations = 0;

    public ExitEntry entrance;
    public ExitEntry exit;
    public float timeRequired; // Czas trwania biegu w minutach gry
    public Sprite attractionSprite;
    public int VisitorCapacity = 5;
    public Tilemap tilemap;
    public int failureChance = 0; // Szansa na awari� (max 25%)
    private int maxFailureChance = 50;

    public List<Visitor> Visitors = new List<Visitor>(); // Lista Visitor�w, kt�rzy korzystaj� z atrakcji
    public List<QueuePath> queuePaths = new List<QueuePath>(); // Kolejki do atrakcji

    private void Awake()
    {
        isBroken = false;
        isOpen = false;
        tilemap = AttractionPlacer.instance?.tilemap;
        Debug.Log($"[Attraction] Start uruchomione dla {gameObject.name}");
        StartCoroutine(AttractionLoop());
    }

    private IEnumerator AttractionLoop()
    {
        while (true)
        {
            if (isOpen && !isRunning)
            {
                if (queuePaths.Count > 0 && queuePaths[0].visitorsOnQueuePath.Count > 0)
                {
                    isRunning = true;
                    LoadVisitors();
                    yield return new WaitForSeconds(timeRequired * 2); // Czas trwania biegu w sekundach
                    UnloadVisitors();
                    isRunning = false;
                    Debug.Log("czas biegu atrakcji sko�czy� si�");
                }
            }
            updateQueue(); // Regularna aktualizacja kolejki
            yield return new WaitForSeconds(1); // Sprawdzaj co sekund�
        }
    }


    private void LoadVisitors()
    {
        int visitorsToLoad = Mathf.Min(VisitorCapacity, queuePaths[0].visitorsOnQueuePath.Count);
        for (int i = 0; i < visitorsToLoad; i++)
        {
            Visitor visitor = queuePaths[0].DequeueVisitor();
            if (visitor != null)
            {
                Visitors.Add(visitor);
                visit();
                visitor.Pay(ticketCost);
                visitor.MoveNPC(visitor.GetCurrentGridPosition(), entrance.coordinates[0]);
                visitor.Deactivate(); // Deaktywuj Visitora na czas trwania biegu
            }
        }
   
    }

    private void UnloadVisitors()
    {
        foreach (Visitor visitor in Visitors)
        {
            visitor.transform.position = tilemap.GetCellCenterWorld(new Vector3Int(
                       exit.coordinates[0].x, exit.coordinates[0].y, 0));
            visitor.Activate(); // Aktywuj Visitora po zako�czeniu biegu
        }
        Visitors.Clear();
        StartCoroutine(DelayedUpdateQueue());
    }

    public void BreakDown()
    {
        isBroken = true;
        isOpen = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color=Color.blue;
            Debug.Log($"{gameObject.name} zmieni� kolor na szary (awaria)!");
        }
        else
        {
            Debug.LogError($"Brak SpriteRenderer w {gameObject.name}, kolor nie mo�e zosta� zmieniony!");
        }
    }

    public void visit()
    {
        todaysVisitations++;
        failureChance = System.Math.Min(failureChance + 1, maxFailureChance); // Rosn�ca szansa na awari� (max 25%)
        if (Random.Range(0, 200) <= failureChance)
        {
            BreakDown();
            Debug.LogError("BALLLLLLLLLLLLLLLLLLLLLLLLLSSSSS");
        }
           
        Debug.Log("NIGGABALLS");
    }

    public void AddQueuePath(QueuePath newQueuePath)
    {
        if (!queuePaths.Contains(newQueuePath))
        {
            queuePaths.Add(newQueuePath);

            if (queuePaths.Count > 1)
            {
                queuePaths[queuePaths.Count - 1].nextQueuePath = queuePaths[queuePaths.Count - 2]; // Zamiana kierunku
            }

            Debug.Log($"[AddQueuePath] Dodano kolejk� {newQueuePath.name}. Teraz queuePaths.Count = {queuePaths.Count}");

            if (!isRunning)
            {
                Debug.Log($"[AddQueuePath] Restartowanie AttractionLoop dla {gameObject.name}");
            }
        }
    }

    private IEnumerator DelayedUpdateQueue()
    {
        yield return new WaitForSeconds(0.1f); // Kr�tkie op�nienie przed aktualizacj� kolejki
        updateQueue();
    }
    public void newDay()
    {
        this.Visitors.Clear();
        foreach (QueuePath quebonafide in queuePaths)
            quebonafide.visitorsOnQueuePath.Clear();

        todaysVisitations = 0;


    }
    public void updateQueue()
    {
        bool movedVisitor;
        do
        {
            movedVisitor = false;
            for (int i = queuePaths.Count - 1; i > 0; i--) // Iteracja od ko�ca do pocz�tku
            {
                List<Visitor> visitorsToMove = new List<Visitor>(queuePaths[i].visitorsOnQueuePath); // Kopia listy, by unikn�� modyfikacji w p�tli
                foreach (Visitor visitor in visitorsToMove)
                {
                    if (!queuePaths[i - 1].IsFull() && queuePaths[i].visitorsOnQueuePath.Contains(visitor)) // Sprawdzenie, czy wcze�niejsza kolejka nie jest pe�na i czy visitor nadal tu jest
                    {
                        queuePaths[i].visitorsOnQueuePath.Remove(visitor); // Usuni�cie z aktualnej kolejki
                        queuePaths[i - 1].visitorsOnQueuePath.Add(visitor); // Przeniesienie do poprzedniej kolejki
                        visitor.MoveNPC(visitor.GetCurrentGridPosition(), queuePaths[i - 1].coordinates[0]); // Przemieszczenie Visitora
                        movedVisitor = true; // Flaga informuj�ca, �e Visitor si� przesun��
                    }
                }
            }
        }
        while (movedVisitor); // Kontynuuj dop�ki kto� si� przesuwa
    }
    public void PerformMaintenance()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        failureChance = 0;
            isOpen = true;
        isBroken = false;
            Debug.Log($"{gameObject.name} przesz�o konserwacj� i jest ponownie otwarte!");
        
    
    }
}
