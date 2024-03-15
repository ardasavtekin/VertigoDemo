using UnityEngine;
using UnityEngine.UI;

public class LosePanel : Panel
{
    private Button giveUpButton, reviveButton;

    void Start()
    {
        giveUpButton = GameObject.Find("GiveUpButton").GetComponent<Button>();
        reviveButton = GameObject.Find("ReviveButton").GetComponent<Button>();
        giveUpButton.onClick.AddListener(GiveUpButtonClick);
        reviveButton.onClick.AddListener(ReviveButtonClick);
    }
    

    void GiveUpButtonClick()
    {
        gameController.Currency = 100;
        gameController.Stage = 1;
        gameController.RewardCount = 0;
        gameController.UpdateCurrencyAndRewards();
        panel.SetActive(false);
    }

    void ReviveButtonClick()
    {
        panel.SetActive(false);
        gameController.Currency -= 20;
        gameController.UpdateCurrencyAndRewards();
    }
}