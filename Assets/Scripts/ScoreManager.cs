using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateScoreUI();
    }

    public void AddPoints(int points)
    {
        Debug.Log("Adding points: " + points);

        Score += points;
        UpdateScoreUI();

        Debug.Log("Current Score: " + Score);

        if (BadgeManager.Instance != null)
        {
            BadgeManager.Instance.CheckBadges(Score);
        }
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Points: " + Score;
        }
    }
}