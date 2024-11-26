using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankSystem : MonoBehaviour
{
    public static RankSystem Instance;

    [SerializeField] private List<Sprite> rankSprites = new();
    [SerializeField] private List<string> rankList = new();

    [SerializeField] private Image rankImageUI;

    public int currentLevel = 0;
    private int maxLevel => rankList.Count;

    [SerializeField] private float maxExp = 100;
    public int currentExp = 0;
    public string currentRank;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateRank()
    {
        if (rankList.Count != 0)
        {
            currentRank = rankList[currentLevel];
            rankImageUI.sprite = rankSprites[currentLevel];
        }
    }

    public void LevelUp(int exp)
    {
        currentExp += exp;
        if (currentExp >= maxExp && currentLevel < maxLevel)
        {
            currentExp = 0;
            maxExp *= 2f;
            currentLevel++;
            UpdateRank();
        }
    }
}
