using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetClearTxt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clearTxt;

    private void Start()
    {
        var fieldClearLocalized = new LocalizedString("New Table", "UI_Stage_Clear");
        fieldClearLocalized.Arguments = new object[] { Managers.Stage.World, Managers.Stage.Stage };
        fieldClearLocalized.StringChanged += (localized) => { clearTxt.text = localized; };
    }
}
