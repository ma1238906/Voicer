using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Button MoonButton;
    public GameObject SettingGO;

    void Start()
    {
        MoonButton.onClick.AddListener(()=>{SettingGO.SetActive(true);});
    }
}
