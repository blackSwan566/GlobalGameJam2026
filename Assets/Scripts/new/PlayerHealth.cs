using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private int startLives = 5;

    [Header("UI Hearts")]
    [Tooltip("Assign the Heart Image components in ascending order: Heart_01, Heart_02, ... Heart_05.\nIf left empty the script will try to auto-find GameObjects named Heart_01..Heart_05.")]
    [SerializeField] private Image[] hearts;

    [Header("Heart Sprites")]
    [Tooltip("Sprite for a full/active heart")]
    [SerializeField] private Sprite fullHeartSprite;
    [Tooltip("Sprite for an empty/lost heart")]
    [SerializeField] private Sprite emptyHeartSprite;

    // internal
    private int currentLives;



    void Awake()
    {
        // clamp startLives
        startLives = Mathf.Clamp(startLives, 0, maxLives);
        currentLives = startLives;

        // if hearts array not assigned or size mismatch, attempt to auto-find named hearts
        if (hearts == null || hearts.Length != maxLives)
        {
            Image[] found = new Image[maxLives];
            bool anyFound = false;
            for (int i = 0; i < maxLives; i++)
            {
                // Expecting names like "Heart_01", "Heart_02", ... with two digits
                string name = $"Heart_{(i + 1).ToString("D2")}";
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    Image img = go.GetComponent<Image>();
                    if (img != null)
                    {
                        found[i] = img;
                        anyFound = true;
                    }
                }
            }

            if (anyFound)
                hearts = found;
            else
                hearts = new Image[maxLives]; // leaves entries null if not assigned
        }

        RefreshHearts();
    }

    // Call this to remove lives (damage)
    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0) return;
        if (currentLives <= 0) return;

        currentLives = Mathf.Clamp(currentLives - amount, 0, maxLives);
        RefreshHearts();

        if (currentLives <= 0)
        {
            OnDeath();
        }
    }

    // Optional: call to heal
    public void Heal(int amount = 1)
    {
        if (amount <= 0) return;
        currentLives = Mathf.Clamp(currentLives + amount, 0, maxLives);
        RefreshHearts();
    }

    // Update heart UI: hearts with index < currentLives are full, others empty.
    // This ordering means when currentLives drops from 5->4, hearts[4] (Heart_05) becomes empty first.
    private void RefreshHearts()
    {
        for (int i = 0; i < maxLives; i++)
        {
            if (hearts == null || i >= hearts.Length) continue;
            Image img = hearts[i];
            if (img == null) continue;

            if (i < currentLives)
            {
                if (fullHeartSprite != null) img.sprite = fullHeartSprite;
            }
            else
            {
                if (emptyHeartSprite != null) img.sprite = emptyHeartSprite;
            }
        }
    }

    private void OnDeath()
    {
        Debug.Log("Player died. Implement respawn/game over here.");
        // TODO: add your death handling (disable input, play animation, reload scene, etc.)
    }

    // Expose current lives/read-only
    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;
}