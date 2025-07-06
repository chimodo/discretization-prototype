using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    public int totalHoney = 0;
    public TextMeshProUGUI honeyText;

    void Awake()
    {
        Instance = this;
    }

    public void AddHoney(int amount)
    {
        totalHoney += amount;
        UpdateUI();
    }

    public bool SpendHoney(int cost)
    {
        if (totalHoney >= cost)
        {
            totalHoney -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (honeyText != null)
            honeyText.text = "Honey: " + totalHoney;
    }
}
