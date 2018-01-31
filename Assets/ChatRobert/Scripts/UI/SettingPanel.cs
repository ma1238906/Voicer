using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour,IPointerClickHandler
{
    public Toggle PuToggle;
    public Toggle YueToggle;
    public Toggle SiToggle;
    public Button QuitButton;

    public static string voicer = "xiaoyan";

    void Start()
    {
        PuToggle.onValueChanged.AddListener(OnPuChange);
        YueToggle.onValueChanged.AddListener(OnYueChange);
        SiToggle.onValueChanged.AddListener(OnSiChange);
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnPuChange(bool isOn)
    {
        if (isOn)
            voicer = "xiaoyan";
    }
    private void OnYueChange(bool isOn)
    {
        if (isOn)
            voicer = "xiaomei";
    }
    private void OnSiChange(bool isOn)
    {
        if (isOn)
            voicer = "xiaorong";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
