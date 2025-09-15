using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class SelectDisplayTemplate : MonoBehaviour // 고용, 업그레이드 등 아이콘, 이름, 설명으로 이루어진 선택창 템플릿
{
    [SerializeField] private RectTransform selectIcon;
    [SerializeField] private RectTransform selectInnerIcon;
    [SerializeField] private TextMeshProUGUI selectTitleTxt;
    [SerializeField] private TextMeshProUGUI selectDescriptionTxt;

    public void SetHire(int nowCharacterIdx) // 고용 선택지
    {
        Destroy(selectInnerIcon.gameObject);

        GetComponent<CanvasGroup>().alpha = 0.7f;

        GameObject character = Instantiate(Managers.Asset.OptionTemplate, selectIcon);
        character.GetComponent<OptionTemplate>().SetCharacterIcon(nowCharacterIdx, true);

        IconInfo iconInfo = character.GetComponentInChildren<IconInfo>();

        var titleLocalized = new LocalizedString("New Table", iconInfo.nameKey);
        titleLocalized.StringChanged += (localized) => { selectTitleTxt.text = localized; };

        var descriptionLocalized = new LocalizedString("New Table", iconInfo.descriptionKey);
        descriptionLocalized.StringChanged += (localized) => { selectDescriptionTxt.text = localized; };
    }

    public void SetUpgrade(CharacterUpgrade characterUpgrade) // 업그레이드 선택지
    {
        GetComponent<CanvasGroup>().alpha = 0.7f;

        selectInnerIcon.GetComponent<Image>().sprite = characterUpgrade.icon;

        var nameLocalized = new LocalizedString("New Table", characterUpgrade.nameKey);
        nameLocalized.StringChanged += (localized) => { selectTitleTxt.text = localized; };

        var descriptionLocalized = new LocalizedString("New Table", characterUpgrade.descriptionKey);
        descriptionLocalized.Arguments = new object[] { characterUpgrade.value, characterUpgrade.value2 };
        descriptionLocalized.StringChanged += (localized) => { selectDescriptionTxt.text = localized; };
    }
}
