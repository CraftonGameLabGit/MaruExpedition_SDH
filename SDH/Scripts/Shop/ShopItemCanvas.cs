using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ShopItemCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemTxt;
    [SerializeField] TextMeshProUGUI dealTxt; // 전 스테이지 딜량

    public int NowSelectedIdx
    {
        get
        {
            return nowSelectedIdx;
        }
        set
        {
            nowSelectedIdx = value;
            SetItemText();
        }
    }
    private int nowSelectedIdx;

    private void SetItemText() // 현재 항목의 텍스트
    {
        if (nowSelectedIdx == 0)
        {
            var nextLocalized = new LocalizedString("New Table", "UI_Shop_Next");
            nextLocalized.StringChanged += (localized) => { itemTxt.text = localized; };
        }
        else
        {
            var characterLocalized = new LocalizedString("New Table", Managers.PlayerControl.Characters[nowSelectedIdx - 1].GetComponent<Character>().characterNameKey);
            characterLocalized.StringChanged += (localized) => { itemTxt.text = localized; };

            var characterLevelLocalized = new LocalizedString("New Table", "UI_Shop_CharacterLevel");
            characterLevelLocalized.Arguments = new object[] { Managers.PlayerControl.Characters[nowSelectedIdx - 1].GetComponent<UpgradeController>().characterLevel };
            characterLevelLocalized.StringChanged += (localized) => { itemTxt.text = itemTxt.text + " <size=75%>" + localized + "</size>"; };

            dealTxt.text = Managers.Record.GetDamageRecord(true, (ECharacterType)Managers.PlayerControl.CharactersIdx[nowSelectedIdx - 1]).ToString();
        }
    }
}
