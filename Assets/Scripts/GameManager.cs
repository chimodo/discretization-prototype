using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    public int totalHoney = 0;
    public TextMeshProUGUI honeyText;

    void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("GameManager Awake() ran!");
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddHoney(int amount)
    {
        totalHoney += amount;
        UpdateUI();
    }

    public bool SpendHoney(int cost)
    {
        // debugging
        Debug.Log("Trying to spend honey...");
        if (honeyText == null)
        {
            Debug.LogWarning("honeyText is null!");
        }

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
