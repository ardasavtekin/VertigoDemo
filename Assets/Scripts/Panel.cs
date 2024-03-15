using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    [SerializeField] private GameObject claimPanel;
    [SerializeField] private GameObject losePanel;
    public GameController gameController;
    [SerializeField] private TextMeshProUGUI rewardCount;
    [SerializeField] private Image cardImage;

    private void OnEnable()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController.GameState == GameState.Win)
        {
            losePanel.SetActive(false);
            claimPanel.SetActive(true);
            cardImage.sprite = gameController.CurrentRewardSprite;
            rewardCount.text = gameController.CurrentRewardCount.ToString();
        }
        else if (gameController.GameState == GameState.Lose)
        {
            claimPanel.SetActive(false);
            losePanel.SetActive(true);
        }
    }
}