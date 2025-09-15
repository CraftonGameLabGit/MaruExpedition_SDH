using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class SelectInfoCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectTitleTxt; // 현재 무슨 선택인지 설명하는 텍스트창
    [SerializeField] private Sprite randomIcon; // 무작위
    [SerializeField] private Sprite returnIcon; // 돌아가기
    [SerializeField] private GameObject vehicleInfo;
    [SerializeField] private Transform vehiclePlaceholder;
    [SerializeField] private TextMeshProUGUI vehicleTitleTxt;
    [SerializeField] private TextMeshProUGUI vehicleExplainTxt;
    [SerializeField] private GameObject characterInfo;
    [SerializeField] private Transform characterPlaceholder;
    [SerializeField] private TextMeshProUGUI characterTitleTxt;
    [SerializeField] private TextMeshProUGUI characterExplainTxt;
    [SerializeField] private GameObject difficultyInfo;
    [SerializeField] private Transform difficultyPlaceholder;
    [SerializeField] private TextMeshProUGUI difficultyTitleTxt;
    [SerializeField] private TextMeshProUGUI difficultyExplainTxt;
    private float onSize = 430f; // 선택 중 크기
    private float endSize = 200f; // 선택 끝난 후 크기
    private RectTransform rectVehicleInfo;
    private RectTransform rectCharacterInfo;
    private RectTransform rectDifficultyInfo;

    private void Awake()
    {
        rectVehicleInfo = vehicleInfo.GetComponent<RectTransform>();
        rectCharacterInfo = characterInfo.GetComponent<RectTransform>();
        rectDifficultyInfo = difficultyInfo.GetComponent<RectTransform>();

        // 혹시 모르니 코드 상에서도 꺼두기
        vehicleInfo.SetActive(false);
        characterInfo.SetActive(false);
        difficultyInfo.SetActive(false);
    }

    public void OffVehicleInfo()
    {
        // 이게 첫 선택지라 필요가 없음
    }

    public void OnVehicleInfo()
    {
        var selectTitleLocalized = new LocalizedString("New Table", "UI_Select_SelectVehicle");
        selectTitleLocalized.StringChanged += (localized) => { selectTitleTxt.text = localized; };

        vehicleInfo.gameObject.SetActive(true);
        rectVehicleInfo.sizeDelta = new(rectVehicleInfo.sizeDelta.x, onSize);
        vehicleExplainTxt.enabled = true;
    }

    public void EndVehicleInfo()
    {
        rectVehicleInfo.sizeDelta = new(rectVehicleInfo.sizeDelta.x, endSize);
        vehicleExplainTxt.enabled = false;
    }

    public void OffCharacterInfo()
    {
        characterInfo.gameObject.SetActive(false);
    }

    public void OnCharacterInfo()
    {
        var selectTitleLocalized = new LocalizedString("New Table", "UI_Select_SelectCharacter");
        selectTitleLocalized.StringChanged += (localized) => { selectTitleTxt.text = localized; };

        characterInfo.gameObject.SetActive(true);
        rectCharacterInfo.sizeDelta = new(rectCharacterInfo.sizeDelta.x, onSize);
        characterExplainTxt.enabled = true;
    }

    public void EndCharacterInfo()
    {
        rectCharacterInfo.sizeDelta = new(rectCharacterInfo.sizeDelta.x, endSize);
        characterExplainTxt.enabled = false;
    }

    public void OffDificultyInfo()
    {
        difficultyInfo.gameObject.SetActive(false);
    }

    public void OnDifficultyInfo()
    {
        var selectTitleLocalized = new LocalizedString("New Table", "UI_Select_SelectDifficulty");
        selectTitleLocalized.StringChanged += (localized) => { selectTitleTxt.text = localized; };

        difficultyInfo.gameObject.SetActive(true);
        rectDifficultyInfo.sizeDelta = new(rectDifficultyInfo.sizeDelta.x, onSize);
        difficultyExplainTxt.enabled = true;
    }

    public void EndDifficultyInfo()
    {
        // 이게 마지막 선택지라 필요가 없음
    }

    public void SetVehicleThumbnail(int idx)
    {
        for (int i = vehiclePlaceholder.childCount - 1; i >= 0; i--) Destroy(vehiclePlaceholder.GetChild(i).gameObject);

        if (idx == -1) // 무작위
        {
            GameObject randomThumbnail = Instantiate(Managers.Asset.OptionTemplate, vehiclePlaceholder);
            randomThumbnail.GetComponent<OptionTemplate>().SetInner(randomIcon, Constant.toolIconSize);

            var randomTitleLocalized = new LocalizedString("New Table", "UI_Select_RandomTitle");
            randomTitleLocalized.StringChanged += (localized) => { vehicleTitleTxt.text = localized; };
            var randomDescriptionLocalized = new LocalizedString("New Table", "UI_Select_RandomDescription");
            randomDescriptionLocalized.StringChanged += (localized) => { vehicleExplainTxt.text = localized; };
            return;
        }
        if (idx == -2) // 돌아가기
        {
            GameObject randomThumbnail = Instantiate(Managers.Asset.OptionTemplate, vehiclePlaceholder);
            randomThumbnail.GetComponent<OptionTemplate>().SetInner(returnIcon, Constant.toolIconSize);

            var returnTitleLocalized = new LocalizedString("New Table", "UI_Select_ReturnTitle");
            returnTitleLocalized.StringChanged += (localized) => { vehicleTitleTxt.text = localized; };
            var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Select_ReturnDescription");
            returnDescriptionLocalized.StringChanged += (localized) => { vehicleExplainTxt.text = localized; };
            return;
        }

        GameObject vehicleThumbnail = Instantiate(Managers.Asset.OptionTemplate, vehiclePlaceholder);
        vehicleThumbnail.GetComponent<OptionTemplate>().SetVehicleIcon(idx, true, 0.75f);

        IconInfo iconInfo = vehicleThumbnail.GetComponentInChildren<IconInfo>();

        if (Managers.Unlock.IsMissionCleared(iconInfo.GetComponent<IconInfo>().unlockType))
        {
            var vehicleTitleLocalized = new LocalizedString("New Table", iconInfo.nameKey);
            vehicleTitleLocalized.StringChanged += (localized) => { vehicleTitleTxt.text = localized; };
            var vehicleDescriptionLocalized = new LocalizedString("New Table", iconInfo.descriptionKey);
            vehicleDescriptionLocalized.StringChanged += (localized) => { vehicleExplainTxt.text = localized; };
        }
        else
        {
            var vehicleTitleLocalized = new LocalizedString("New Table", iconInfo.nameKey);
            vehicleTitleLocalized.StringChanged += (localized) => { vehicleTitleTxt.text = localized; };
            var vehicleDescriptionLocalized = new LocalizedString("New Table", iconInfo.unlockdescriptionKey);
            vehicleDescriptionLocalized.Arguments = new object[] { Managers.Unlock.MissionClearedNowCondition[(int)iconInfo.GetComponent<IconInfo>().unlockType], Managers.Unlock.MissionClearedCondition[(int)iconInfo.GetComponent<IconInfo>().unlockType] };
            vehicleDescriptionLocalized.StringChanged += (localized) => { vehicleExplainTxt.text = localized; };
        }
            
    }

    public void SetCharacterThumbnail(int idx)
    {
        for (int i = characterPlaceholder.childCount - 1; i >= 0; i--) Destroy(characterPlaceholder.GetChild(i).gameObject);

        if (idx == -1) // 무작위
        {
            GameObject randomThumbnail = Instantiate(Managers.Asset.OptionTemplate, characterPlaceholder);
            randomThumbnail.GetComponent<OptionTemplate>().SetInner(randomIcon, Constant.toolIconSize);

            var randomTitleLocalized = new LocalizedString("New Table", "UI_Select_RandomTitle");
            randomTitleLocalized.StringChanged += (localized) => { characterTitleTxt.text = localized; };
            var randomDescriptionLocalized = new LocalizedString("New Table", "UI_Select_RandomDescription");
            randomDescriptionLocalized.StringChanged += (localized) => { characterExplainTxt.text = localized; };
            return;
        }
        if (idx == -2) // 돌아가기
        {
            GameObject randomThumbnail = Instantiate(Managers.Asset.OptionTemplate, characterPlaceholder);
            randomThumbnail.GetComponent<OptionTemplate>().SetInner(returnIcon, Constant.toolIconSize);

            var returnTitleLocalized = new LocalizedString("New Table", "UI_Select_ReturnTitle");
            returnTitleLocalized.StringChanged += (localized) => { characterTitleTxt.text = localized; };
            var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Select_ReturnDescription");
            returnDescriptionLocalized.StringChanged += (localized) => { characterExplainTxt.text = localized; };
            return;
        }

        GameObject characterThumbnail = Instantiate(Managers.Asset.OptionTemplate, characterPlaceholder);
        characterThumbnail.GetComponent<OptionTemplate>().SetCharacterIcon(idx, true);

        IconInfo iconInfo = characterThumbnail.GetComponentInChildren<IconInfo>();

        if (Managers.Unlock.IsMissionCleared(iconInfo.GetComponent<IconInfo>().unlockType))
        {
            var characterTitleLocalized = new LocalizedString("New Table", iconInfo.nameKey);
            characterTitleLocalized.StringChanged += (localized) => { characterTitleTxt.text = localized; };
            var characterDescriptionLocalized = new LocalizedString("New Table", iconInfo.descriptionKey);
            characterDescriptionLocalized.StringChanged += (localized) => { characterExplainTxt.text = localized; };
        }
        else
        {
            var characterTitleLocalized = new LocalizedString("New Table", iconInfo.nameKey);
            characterTitleLocalized.StringChanged += (localized) => { characterTitleTxt.text = localized; };
            var characterDescriptionLocalized = new LocalizedString("New Table", iconInfo.unlockdescriptionKey);
            characterDescriptionLocalized.Arguments = new object[] { Managers.Unlock.MissionClearedNowCondition[(int)iconInfo.GetComponent<IconInfo>().unlockType], Managers.Unlock.MissionClearedCondition[(int)iconInfo.GetComponent<IconInfo>().unlockType] };
            characterDescriptionLocalized.StringChanged += (localized) => { characterExplainTxt.text = localized; };
        }
    }

    public void SetDifficultyThumbnail(int idx, Sprite idxSprite, bool isClear = true)
    {
        for (int i = difficultyPlaceholder.childCount - 1; i >= 0; i--) Destroy(difficultyPlaceholder.GetChild(i).gameObject);

        if (idx == -1) // 돌아가기
        {
            GameObject randomThumbnail = Instantiate(Managers.Asset.OptionTemplate, difficultyPlaceholder);
            randomThumbnail.GetComponent<OptionTemplate>().SetInner(returnIcon, Constant.toolIconSize);

            var returnTitleLocalized = new LocalizedString("New Table", "UI_Select_ReturnTitle");
            returnTitleLocalized.StringChanged += (localized) => { difficultyTitleTxt.text = localized; };
            var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Select_ReturnDescription");
            returnDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            return;
        }

        GameObject difficultyThumbnail = Instantiate(Managers.Asset.OptionTemplate, difficultyPlaceholder);
        difficultyThumbnail.GetComponent<OptionTemplate>().SetDiffficulty(idxSprite, Constant.difficultyIconSize, isClear);

        if (idx == 0)
        {
            var difficultyTitleLocalized = new LocalizedString("New Table", "UI_Select_DifficultyTitle" + idx.ToString());
            difficultyTitleLocalized.StringChanged += (localized) => { difficultyTitleTxt.text = localized; };
            var difficultyDescriptionLocalized = new LocalizedString("New Table", "UI_Select_DifficultyDescription" + idx.ToString());
            difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
        }
        if (idx == 1)
        {
            var difficultyTitleLocalized = new LocalizedString("New Table", "UI_Select_DifficultyTitle" + idx.ToString());
            difficultyTitleLocalized.StringChanged += (localized) => { difficultyTitleTxt.text = localized; };

            if (Managers.Unlock.IsMissionCleared(EUnlockType.ZeroClearCount))
            {
                difficultyThumbnail.GetComponent<Image>().color = new Color32(6, 214, 160, 255);
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "UI_Select_DifficultyDescription" + idx.ToString());
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
            else
            {
                difficultyThumbnail.GetComponent<Image>().color = Color.white;
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "Difficulty_Achieve_Desc1");
                difficultyDescriptionLocalized.Arguments = new object[] { Managers.Unlock.MissionClearedNowCondition[(int)EUnlockType.ZeroClearCount], Managers.Unlock.MissionClearedCondition[(int)EUnlockType.ZeroClearCount] };
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
        }
        if (idx == 2)
        {
            var difficultyTitleLocalized = new LocalizedString("New Table", "UI_Select_DifficultyTitle" + idx.ToString());
            difficultyTitleLocalized.StringChanged += (localized) => { difficultyTitleTxt.text = localized; };

            if (Managers.Unlock.IsMissionCleared(EUnlockType.OneClearCount))
            {
                difficultyThumbnail.GetComponent<Image>().color = new Color32(255, 209, 102, 255);
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "UI_Select_DifficultyDescription" + idx.ToString());
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
            else
            {
                difficultyThumbnail.GetComponent<Image>().color = Color.white;
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "Difficulty_Achieve_Desc2");
                difficultyDescriptionLocalized.Arguments = new object[] { Managers.Unlock.MissionClearedNowCondition[(int)EUnlockType.OneClearCount], Managers.Unlock.MissionClearedCondition[(int)EUnlockType.OneClearCount] };
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
        }
        if (idx == 3)
        {
            var difficultyTitleLocalized = new LocalizedString("New Table", "UI_Select_DifficultyTitle" + idx.ToString());
            difficultyTitleLocalized.StringChanged += (localized) => { difficultyTitleTxt.text = localized; };

            if (Managers.Unlock.IsMissionCleared(EUnlockType.TwoClearCount))
            {
                difficultyThumbnail.GetComponent<Image>().color = new Color32(239, 71, 111, 255);
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "UI_Select_DifficultyDescription" + idx.ToString());
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
            else
            {
                difficultyThumbnail.GetComponent<Image>().color = Color.white;
                var difficultyDescriptionLocalized = new LocalizedString("New Table", "Difficulty_Achieve_Desc3");
                difficultyDescriptionLocalized.Arguments = new object[] { Managers.Unlock.MissionClearedNowCondition[(int)EUnlockType.TwoClearCount], Managers.Unlock.MissionClearedCondition[(int)EUnlockType.TwoClearCount] };
                difficultyDescriptionLocalized.StringChanged += (localized) => { difficultyExplainTxt.text = localized; };
            }
        }
    }
}
