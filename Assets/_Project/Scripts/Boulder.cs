using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Obstacle
{
    public Sprite[] sprites;
    public int stage = 3;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeBoulder();
    }

    private void InitializeBoulder()
    {
        // Losujemy etap z zakresu 1-3
        stage = Random.Range(1, 4);

        // Ustawiamy odpowiedni sprite na podstawie wylosowanego etapu
        spriteRenderer.sprite = sprites[stage - 1];
    }

    public void decreaseStage()
    {
        stage--;
        if (stage == 0)
        {
            Destroy(gameObject);
            return;
        }
        spriteRenderer.sprite = sprites[stage - 1];
    }
    public float GetBoulderRemovalCost(int stage)
    {
        switch (stage)
        {
            case 3: return 150;
            case 2: return 100;
            case 1: return 50;
            default: return 0; // Jeœli coœ posz³o nie tak, nic nie kosztuje
        }
    }
}