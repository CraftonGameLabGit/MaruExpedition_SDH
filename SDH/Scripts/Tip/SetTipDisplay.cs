using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetTipDisplay : MonoBehaviour // 상점에서 보여주는 팁
{
    [SerializeField] TextMeshProUGUI tipTxt;

    private void Start()
    {
        SetTip();
    }

    private void SetTip()
    {
        LocalizedString tipLocalized;

        switch (Managers.Shop.TipIdx)
        {
            case 0:
                tipLocalized = new LocalizedString("New Table", "UI_Shop_Tip1");
                tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
                break;
            case 1:
                tipLocalized = new LocalizedString("New Table", "UI_Shop_Tip2");
                tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
                break;
            case 2:
                tipLocalized = new LocalizedString("New Table", "UI_Shop_Tip3");
                tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
                break;
            case 3:
                tipLocalized = new LocalizedString("New Table", "UI_Shop_Tip4");
                tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
                break;
        }

        // 저장 관련 이슈 때문에 Managers.Shop.TipIdx는 상점을 나갈때 (ShopCanvas.cs에서) 올려주는 것으로
    }
}
