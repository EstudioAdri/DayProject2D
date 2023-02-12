using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject emptyHearts, halfLHearts, halfRHearts, emptyHeartPrefab, halfHeartLPrefab, halfHeartRPrefab;
    [Space]
    [SerializeField] List<GameObject> heartFilledSpritesList;
    [SerializeField] List<GameObject> heartEmptySpritesList;

    [SerializeField] uint totalNumberOfHearts = 3; // Every heart allows for 2 health points
    [SerializeField] uint currentHealth;

    private void Start()
    {
        currentHealth = totalNumberOfHearts * 2;

        CreateEmptyHearts();
        FillHeartsUI();
    }

    private void Update()
    {
        if (heartFilledSpritesList.Count != currentHealth || heartEmptySpritesList.Count != totalNumberOfHearts)
        {
            ClearHealthBarSprites();
            CreateEmptyHearts();
            FillHeartsUI();
        }
    }

    void ClearHealthBarSprites()
    {
        foreach (var item in heartFilledSpritesList)
        {
            Destroy(item);
        }
        foreach (var item in heartEmptySpritesList)
        {
            Destroy(item);
        }

        heartFilledSpritesList = new();
        heartEmptySpritesList = new();
    }

    void CreateEmptyHearts()
    {
        for (int i = 0; i < totalNumberOfHearts; i++)
        {
            GameObject thisEmptyHeart = Instantiate(emptyHeartPrefab, emptyHearts.transform);
            heartEmptySpritesList.Add(thisEmptyHeart);
        }
    }

    void FillHeartsUI()
    {
        if (currentHealth > totalNumberOfHearts * 2) // Quick check to avoid health bar overflow
            currentHealth = totalNumberOfHearts * 2;

        for (int i = 1; i <= currentHealth; i++)
        {
            GameObject thisHalfHeart;

            if (i % 2 != 0) // Odd number = L heart
            {
                thisHalfHeart = Instantiate(halfHeartLPrefab, halfLHearts.transform);
            }
            else // Even number = R heart
            {
                thisHalfHeart = Instantiate(halfHeartRPrefab, halfRHearts.transform);
            }

            heartFilledSpritesList.Add(thisHalfHeart);
        }
    }
}
