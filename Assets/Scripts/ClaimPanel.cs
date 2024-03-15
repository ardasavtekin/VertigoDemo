using UnityEngine;
using UnityEngine.UI;

public class ClaimPanel : Panel
{
    private Button claimButton;

    void Start()
    {
        claimButton = GameObject.Find("ClaimButton").GetComponent<Button>();
        claimButton.onClick.AddListener(ClaimButtonClick);
    }

    void ClaimButtonClick()
    {
        panel.SetActive(false);
    }
}