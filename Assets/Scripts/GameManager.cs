using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton: Erlaubt einfachem Zugriff von überall
    public static GameManager Instance { get; private set; }

    public int life = 5;
    public int score = 0;
    public int counter = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        counter++;
        if (counter == 5)
        {
            score += amount;
            UpdateUI();
        }

        if (counter >= 5)
        {
            counter = 0;

        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Gegner besiegt: " + score;
    }
}