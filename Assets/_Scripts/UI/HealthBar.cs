using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] GameObject player, emptyHearts, halfLHearts, halfRHearts, emptyHeartPrefab, halfHeartLPrefab, halfHeartRPrefab;
    [Space]
    [SerializeField] List<GameObject> heartFilledSpritesList;
    [SerializeField] List<GameObject> heartEmptySpritesList;

    uint totalNumberOfHearts;
    uint currentHealth;

    #endregion

    #region BuiltInMethods

    private void Start()
    {
        //player = FindObjectOfType<Player>().gameObject;
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        GetCurrentHealth();

        if (heartFilledSpritesList.Count != currentHealth || heartEmptySpritesList.Count != totalNumberOfHearts)
        {
            ClearHealthBarSprites();
            CreateEmptyHearts();
            FillHeartsUI();
        }
    }

    #endregion

    #region PublicMethods

    public void GetCurrentHealth()
    {
        if (player == null)
        {
            currentHealth = 0;
            return;
        }

        var _playerHealth = player.GetComponent<Health>();
        currentHealth = _playerHealth.HealthValue;
        totalNumberOfHearts = _playerHealth.MaxHealthValue / 2;
    }

    #endregion

    #region PrivateMethods

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
    
    #endregion

}
