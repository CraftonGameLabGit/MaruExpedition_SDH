using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetTipDisplay : MonoBehaviour // �������� �����ִ� ��
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

        // ���� ���� �̽� ������ Managers.Shop.TipIdx�� ������ ������ (ShopCanvas.cs����) �÷��ִ� ������
    }
}
