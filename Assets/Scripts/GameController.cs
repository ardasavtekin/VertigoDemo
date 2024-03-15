using UnityEngine;
using UI.FortuneWheel;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private FortuneWheel fortuneWheel;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI chestText;
    private int rewardCount;
    private Sprite currentRewardSprite;
    private int currentRewardCount;
    private int currency;
    public GameState GameState { get; private set; }

    public int Currency
    {
        get => currency;
        set => currency = value;
    }

    public int Stage
    {
        get => stage;
        set => stage = value;
    }

    public int RewardCount
    {
        get => rewardCount;
        set => rewardCount = value;
    }

    public Sprite CurrentRewardSprite
    {
        get => currentRewardSprite;
        set => currentRewardSprite = value;
    }

    public int CurrentRewardCount
    {
        get => currentRewardCount;
        set => currentRewardCount = value;
    }

    private int stage;

    private void SetWheelState()
    {
        if (stage % 5 == 0 && stage % 30 != 0)
        {
            fortuneWheel.wheelType = WheelType.Silver;
        }
        else if (stage % 30 == 0)
        {
            fortuneWheel.wheelType = WheelType.Golden;
        }
        else
        {
            fortuneWheel.wheelType = WheelType.Bronze;
        }
    }

    public void UpdateCurrencyAndRewards()
    {
        currencyText.text = currency + "$";
        chestText.text = rewardCount.ToString();
        stageText.text = stage.ToString();
    }

    private void Start()
    {
        stage = 1;
        currency = 100;
        rewardCount = 0;
        UpdateCurrencyAndRewards();
        stageText.text = stage.ToString();
        uiSpinButton.onClick.AddListener(() =>
        {
            uiSpinButton.interactable = false;
            fortuneWheel.OnSpinEnd(wheelPiece =>
            {
                if (wheelPiece.giftType == GiftType.Bomb)
                {
                    GameState = GameState.Lose;
                }
                else
                {
                    GameState = GameState.Win;
                }

                if (wheelPiece.giftType == GiftType.Currency)
                {
                    currency += wheelPiece.Amount;
                }
                else
                {
                    if (wheelPiece.giftType != GiftType.Bomb)
                    {
                        rewardCount += wheelPiece.Amount;
                    }
                }

                currentRewardSprite = wheelPiece.icon;
                currentRewardCount = wheelPiece.Amount;
                uiSpinButton.interactable = true;
                panel.SetActive(true);
                stage += 1;
                UpdateCurrencyAndRewards();
                SetWheelState();
                fortuneWheel.SetWheelAndIndicatorView();
                stageText.text = stage.ToString();
            });
            fortuneWheel.Spin();
        });
    }
}

public enum GameState
{
    Win,
    Lose
}