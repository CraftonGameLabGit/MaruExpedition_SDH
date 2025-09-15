using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopCharacterCanvas : MonoBehaviour
{
    [SerializeField] private ShopCanavs shopCanavs;
    [SerializeField] private ShopUpgradeCanvas shopUpgradeCanvas;
    [SerializeField] private RectTransform section; // 사진란 (캐릭터가 보이는 곳)
    [SerializeField] private TextMeshProUGUI characterNameTxt; // 캐릭터 이름
    [SerializeField] private TextMeshProUGUI characterLevelTxt;
    [SerializeField] private TextMeshProUGUI characterDescriptionTxt; // 캐릭터 설명
    [SerializeField] private TextMeshProUGUI[] characterStatusItems; // 캐릭터 스탯 항목
    [SerializeField] private TextMeshProUGUI[] characterStatusValues; // 캐릭터 스탯 값
    [SerializeField] private RectTransform bookBtn; // 스킬 도감 보기 버튼
    [SerializeField] private RectTransform upgradesBookBackground; // 업그레이드 도감 이미지
    [SerializeField] private RectTransform upgradesBook; // 업그레이드 도감
    [SerializeField] private RectTransform[] upgradesBooks; // 업그레이드 도감
    [SerializeField] private RectTransform[] upgradeDisplays; // 적용된 업그레이드
    [SerializeField] private Image[] upgradeDisplayIcons; // 적용된 업그레이드 이미지
    [SerializeField] private RectTransform upgradeBtn; // 업그레이드 버튼
    [SerializeField] private TextMeshProUGUI upgradeBtnTxt; // 업그레이드 버튼 텍스트
    [SerializeField] private RectTransform returnBtn; // 돌아가기 버튼
    [SerializeField] private TextMeshProUGUI upgradeTitleTxt;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionTxt;
    [SerializeField] private RectTransform selectCursor;
    private int nowCharacterOrderIdx; // 현재 선택한 캐릭터 인덱스
    private int nowCharacterSelectIdxX, nowCharacterSelectIdxY; // 옵션 인덱스
    private UpgradeController upgradeController; // 현재 캐릭터 업그레이드 관리
    private List<List<CharacterUpgrade>> allUpgrades; // 업그레이드 리스트
    private List<CharacterUpgrade> myUpgrades = new(); // 현재 캐릭터의 업그레이드
    private bool isBookOpened = false; // 스킬 도감이 열려있는지
    private IEnumerator selectEffect;

    public void SetShopCharacterCanvas(int characterOrderIdx)
    {
        if (Managers.PlayerControl.Characters.Count <= characterOrderIdx) return; // 걸릴 일은 없을 듯 하지만 혹시 몰라서

        nowCharacterOrderIdx = characterOrderIdx;

        upgradeController = Managers.PlayerControl.Characters[nowCharacterOrderIdx].GetComponent<UpgradeController>();
        allUpgrades = upgradeController.allUpgrades;
        GameObject character = Instantiate(Managers.Asset.OptionTemplate, section);
        character.GetComponent<OptionTemplate>().SetCharacterIcon(Managers.PlayerControl.CharactersIdx[nowCharacterOrderIdx], true);
        SortingGroup sortingGroup = character.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "FrontGround";
        sortingGroup.sortingOrder = 2;
        if(upgradeController.characterLevel == 5)
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle"); // 텍스트가 동일해서 그냥 이거 씀
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        else
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeBtn");
            upgradeBtnLocalized.Arguments = new object[] { Constant.upgradesCost[upgradeController.characterLevel + 1] };
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        var characterNameLocalized = new LocalizedString("New Table", character.GetComponentInChildren<IconInfo>().nameKey);
        characterNameLocalized.StringChanged += (localized) => { characterNameTxt.text = localized; };
        var characterLevelLocalized = new LocalizedString("New Table", "UI_Shop_CharacterLevel");
        characterLevelLocalized.Arguments = new object[] { upgradeController.characterLevel };
        characterLevelLocalized.StringChanged += (localized) => { characterLevelTxt.text = localized; };
        var characterDescriptionLocalized = new LocalizedString("New Table", character.GetComponentInChildren<IconInfo>().descriptionKey);
        characterDescriptionLocalized.StringChanged += (localized) => { characterDescriptionTxt.text = localized; };
        SetStatus();

        for (int i = 0; i < Constant.maxUpgrade; i++) // 도감 생성
        {
            for (int j = 0; j < allUpgrades[i].Count; j++)
            {
                GameObject upgradeIcon = Instantiate(Managers.Asset.OptionTemplate, upgradesBooks[i]);
                OptionTemplate optionTemplate = upgradeIcon.GetComponent<OptionTemplate>();
                optionTemplate.SetInner(allUpgrades[i][j].icon);
                if (upgradeController._acquiredUpgrades.Contains(allUpgrades[i][j]))
                {
                    myUpgrades.Add(allUpgrades[i][j]);
                    upgradeDisplayIcons[i].sprite = allUpgrades[i][j].icon;
                    upgradeDisplayIcons[i].color = new(1f, 1f, 1f, 1f);
                    upgradeDisplayIcons[i].sprite = allUpgrades[i][j].icon;
                    upgradeDisplayIcons[i].color = new(1f, 1f, 1f, 1f);
                }
                else
                {
                    optionTemplate.GetComponent<CanvasGroup>().alpha = 0.5f;
                }
            }
        }
    }

    private IEnumerator SetBook() // 도감 설정
    {
        Managers.InputControl.ResetUIAction();
        isBookOpened = true;
        selectCursor.GetComponent<Image>().enabled = false;
        bookBtn.gameObject.SetActive(false);

        float nowTime = 0f, maxTime = 0.1f;
        while (nowTime <= maxTime)
        {
            upgradesBook.localScale = new(nowTime / maxTime, nowTime / maxTime, 1f);
            nowTime += Time.deltaTime;
            yield return null;
        }
        upgradesBook.localScale = new(1f, 1f, 1f);

        yield return new WaitForEndOfFrame();

        nowCharacterSelectIdxY = 2;
        nowCharacterSelectIdxX = 0;
        selectCursor.GetComponent<Image>().enabled = true;
        RectTransform rect = upgradesBooks[0].GetChild(0).GetComponent<RectTransform>();
        selectCursor.position = rect.position;
        selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

        var titleLocalized = new LocalizedString("New Table", allUpgrades[0][0].nameKey);
        titleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
        var descriptionLocalized = new LocalizedString("New Table", allUpgrades[0][0].descriptionKey);
        descriptionLocalized.Arguments = new object[] { allUpgrades[0][0].value, allUpgrades[0][0].value2 };
        descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        //GetMouseInput();
    }

    public void StartShopCharacterCanvas()
    {
        // 시작은 (0,-1) 항상 재설정
        nowCharacterSelectIdxY = 0;
        nowCharacterSelectIdxX = -1;
        selectCursor.GetComponent<Image>().enabled = true;
        selectCursor.position = returnBtn.position;
        selectCursor.sizeDelta = returnBtn.sizeDelta + Constant.cursorAddSize;
        section.GetChild(0).gameObject.SetActive(true);

        var returnTitleLocalized = new LocalizedString("New Table", "UI_Shop_ReturnBtn");
        returnTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
        var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_ReturnDescription");
        returnDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        //GetMouseInput();
    }

    public void SetStatus() // 스탯 설정
    {
        (string statusValueKey, float[] status) = Managers.PlayerControl.Characters[nowCharacterOrderIdx].GetComponent<Character>().GetStatus(); // statusKey는 전부 통일로 변경

        var characterStatusLocalized = new LocalizedString("New Table", "UI_Shop_CharacterStatus");
        characterStatusLocalized.StringChanged += (localized) => 
        {
            string[] lines = localized.Split("\n"); 

            for(int i = 0; i < 8; i++) // 8개가 아니라면 뭔가 잘못된 것
            {
                characterStatusItems[i].text = lines[i];
            }
        };
        var characterStatusValueLocalized = new LocalizedString("New Table", statusValueKey);
        characterStatusValueLocalized.Arguments = status.Cast<object>().ToList();
        characterStatusValueLocalized.StringChanged += (localized) => 
        {
            string[] lines = localized.Split("\n");

            for (int i = 0; i < 8; i++) // 8개가 아니라면 뭔가 잘못된 것
            {
                characterStatusValues[i].text = lines[i];
            }
        };
    }

    public void SetUpgrades() // 업그레이드를 했다면 도감에서 색칠하고 스탯 및 기타 텍스트 재설정
    {
        SetStatus();
        for (int i = 0; i < allUpgrades[upgradeController.characterLevel - 2].Count; i++)
        {
            if (upgradeController._acquiredUpgrades.Contains(allUpgrades[upgradeController.characterLevel - 2][i]))
            {
                myUpgrades.Add(allUpgrades[upgradeController.characterLevel - 2][i]);
                upgradesBooks[upgradeController.characterLevel - 2].GetChild(i).GetComponent<CanvasGroup>().alpha = 1f;
                upgradeDisplayIcons[upgradeController.characterLevel - 2].sprite = allUpgrades[upgradeController.characterLevel - 2][i].icon;
                upgradeDisplayIcons[upgradeController.characterLevel - 2].color = new(1f, 1f, 1f, 1f);
                break;
            }
        }

        selectCursor.GetComponent<Image>().enabled = true;

        if (upgradeController.characterLevel == 5)
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle"); // 텍스트가 동일해서 그냥 이거 씀
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        else
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeBtn");
            upgradeBtnLocalized.Arguments = new object[] { Constant.upgradesCost[upgradeController.characterLevel + 1] };
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        var characterLevelLocalized = new LocalizedString("New Table", "UI_Shop_CharacterLevel");
        characterLevelLocalized.Arguments = new object[] { upgradeController.characterLevel };
        characterLevelLocalized.StringChanged += (localized) => { characterLevelTxt.text = localized; };

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        //GetMouseInput();
    }

    private void GetLeft() { SetNowSelectIdxX(nowCharacterSelectIdxX + 1); } // 인덱스가 반대라는 점에 유의
    private void GetRight() { SetNowSelectIdxX(nowCharacterSelectIdxX - 1); }
    private void GetDown() { SetNowSelectIdxY(nowCharacterSelectIdxY - 1); }
    private void GetUp() { SetNowSelectIdxY(nowCharacterSelectIdxY + 1); }

    private void GetMouseInput()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(upgradeBtn, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdxXY(1, -1);
            return;
        }
        if (RectTransformUtility.RectangleContainsScreenPoint(returnBtn, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdxXY(0, -1);
            return;
        }
        for(int i = 0; i < Constant.maxUpgrade; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(upgradeDisplays[i], Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdxXY(i + 2, -1);
                return;
            }
        }
        if(!isBookOpened)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(upgradesBookBackground, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdxXY(Mathf.Max(nowCharacterSelectIdxY, 2), 0);
                return;
            }
        }
        else
        {
            for (int i = 0; i < Constant.maxUpgrade; i++)
            {
                for (int j = 0; j < allUpgrades[i].Count; j++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(upgradesBooks[i].GetChild(j).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
                    {
                        Managers.InputControl.PossInputMouseUse = true;
                        SetNowSelectIdxXY(i + 2, j);
                        return;
                    }
                }
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    private void SetNowSelectIdxY(int newIdxY) // Y값만 변경
    {
        if (newIdxY < 0 || newIdxY > 5) return; // 인덱스 밖
        if (nowCharacterSelectIdxY == newIdxY) return;

        if (!isBookOpened && nowCharacterSelectIdxY >= 2 && nowCharacterSelectIdxX == 0)
        {
            if (nowCharacterSelectIdxY < newIdxY) return;
            else
            {
                nowCharacterSelectIdxY = 1;
                nowCharacterSelectIdxX = -1;
            }
        }
        else if (newIdxY <= 1 || nowCharacterSelectIdxY <= 1)
        {
            nowCharacterSelectIdxY = newIdxY;
        }
        else if (nowCharacterSelectIdxX == -1) // 보유 스킬
        {
            nowCharacterSelectIdxY = newIdxY;
        }
        else
        {
            if (nowCharacterSelectIdxY > newIdxY) // 내려올 때
            {
                nowCharacterSelectIdxX = (2 * nowCharacterSelectIdxX + upgradesBooks[newIdxY - 2].childCount - upgradesBooks[nowCharacterSelectIdxY - 2].childCount) / 2; // 대각선 보정
            }
            else // 올라갈 때
            {
                nowCharacterSelectIdxX = (2 * nowCharacterSelectIdxX + upgradesBooks[newIdxY - 2].childCount - upgradesBooks[nowCharacterSelectIdxY - 2].childCount + 1) / 2;
            }

            nowCharacterSelectIdxY = newIdxY;

            if (nowCharacterSelectIdxX < 0) nowCharacterSelectIdxX = 0;
            if (nowCharacterSelectIdxX > upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1) nowCharacterSelectIdxX = upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1;
        }

        SetNowSelectIdx();
    }

    private void SetNowSelectIdxX(int newIdxX) // X값만 변경
    {
        if (nowCharacterSelectIdxX == newIdxX) return;

        if (nowCharacterSelectIdxY == 2 && nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX > newIdxX) // 가장 오른쪽 아래 보유 스킬에서 오른쪽을 누르면 업그레이드 버튼으로
        {
            nowCharacterSelectIdxY = 1;
            nowCharacterSelectIdxX = -1;
        }
        else if (!isBookOpened && nowCharacterSelectIdxY >= 2) // 도감 열기 버튼 상태일 때 처리
        {
            if (nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX < newIdxX) nowCharacterSelectIdxX = 0;
            else if (nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX > newIdxX) return;
            else if (nowCharacterSelectIdxX == 0 && nowCharacterSelectIdxX < newIdxX) return;
            else nowCharacterSelectIdxX = -1;
        }
        else if (nowCharacterSelectIdxY <= 1 && nowCharacterSelectIdxX < newIdxX) // 업그레이드 버튼이나 닫기 버튼에서 왼쪽을 누르면 가장 오른쪽 아래 업그레이드로
        {
            nowCharacterSelectIdxY = 2;
            nowCharacterSelectIdxX = 0;
        }
        else if (nowCharacterSelectIdxY > 2 && nowCharacterSelectIdxX == upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1 && nowCharacterSelectIdxX < newIdxX) // 가장 왼쪽 업그레이드에서 왼쪽을 누르면 한줄 내려가기
        {
            nowCharacterSelectIdxY--;
            nowCharacterSelectIdxX = upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1;
        }
        else if (nowCharacterSelectIdxY <= 1 && nowCharacterSelectIdxX > newIdxX) return; // 인덱스 밖
        else if (newIdxX < -1 || newIdxX > upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1) return; // 인덱스 밖
        else
        {
            nowCharacterSelectIdxX = newIdxX;
        }

        SetNowSelectIdx();
    }

    private void SetNowSelectIdxXY(int newIdxY, int newIdxX) // XY 둘 다 변경
    {
        // 초기화나 마우스에 쓰일 용도라 인덱스 체크 불필요
        if (nowCharacterSelectIdxY == newIdxY && nowCharacterSelectIdxX == newIdxX) return;

        nowCharacterSelectIdxY = newIdxY;
        nowCharacterSelectIdxX = newIdxX;

        SetNowSelectIdx();
    }

    private void SetNowSelectIdx() // 다른 캔버스로 넘어가고 여기서는 X와 Y축을 둘 다 관리하기 때문에 커서 위치 이동을 별도로
    {
        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        if(!isBookOpened && nowCharacterSelectIdxY >= 2 && nowCharacterSelectIdxX >= 0) // 스킬 도감 보기 버튼
        {
            nowCharacterSelectIdxX = 0;
            selectCursor.position = bookBtn.position;
            selectCursor.sizeDelta = bookBtn.sizeDelta + Constant.cursorAddSize;

            var bookTitleLocalized = new LocalizedString("New Table", "UI_Shop_BookTitle");
            bookTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var bookDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_BookDescription");
            bookDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }
        else if (nowCharacterSelectIdxY == 0) // 닫기 버튼
        {
            selectCursor.position = returnBtn.position;
            selectCursor.sizeDelta = returnBtn.sizeDelta + Constant.cursorAddSize;

            var returnTitleLocalized = new LocalizedString("New Table", "UI_Shop_ReturnBtn");
            returnTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_ReturnDescription");
            returnDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }
        else if (nowCharacterSelectIdxY == 1) // 업그레이드 버튼
        {
            selectCursor.position = upgradeBtn.position;
            selectCursor.sizeDelta = upgradeBtn.sizeDelta + Constant.cursorAddSize;

            var upgradeTitleLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle");
            upgradeTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            if (upgradeController.characterLevel >= 5) // 만렙인 경우 텍스트: 이 캐릭터는 최고 레벨입니다.
            {
                var maxUpgradeDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_MaxUpgradeDescription");
                maxUpgradeDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
            else // 만렙이 아닌 경우 텍스트: 캐릭터를 {0}단계로 업그레이드 합니다. {1}가지 선택지 중 하나를 선택합니다. 요구 비용은 {2}골드 입니다.
            {
                var upgradeDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeDescription");
                upgradeDescriptionLocalized.Arguments = new object[] { upgradeController.characterLevel + 1, upgradeController.characterLevel == 4 ? 2 : 3, Constant.upgradesCost[upgradeController.characterLevel + 1] };
                upgradeDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
        }
        else if(nowCharacterSelectIdxX == -1) // 보유 스킬
        {
            selectCursor.position = upgradeDisplays[nowCharacterSelectIdxY - 2].position;
            selectCursor.sizeDelta = upgradeDisplays[nowCharacterSelectIdxY - 2].sizeDelta + Constant.cursorAddSize;

            if (upgradeController.characterLevel < nowCharacterSelectIdxY) // 레벨보다 높음 (스킬 안갖고있음)
            {
                var titleLocalized = new LocalizedString("New Table", "UI_Shop_NoSkillTitle");
                titleLocalized.Arguments = new object[] { nowCharacterSelectIdxY };
                titleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
                var descriptionLocalized = new LocalizedString("New Table", "UI_Shop_NoSkillDescription");
                descriptionLocalized.Arguments = new object[] { nowCharacterSelectIdxY };
                descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
            else // 레벨보다 낮거나 같음 (스킬 갖고있음)
            {
                string skillName= "";
                var titleLocalized = new LocalizedString("New Table", myUpgrades[nowCharacterSelectIdxY - 2].nameKey);
                titleLocalized.StringChanged += (localized) => { skillName = localized; };
                var titleLocalized2 = new LocalizedString("New Table", "UI_Shop_SkillTitle");
                titleLocalized2.Arguments = new object[] { nowCharacterSelectIdxY, skillName };
                titleLocalized2.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
                var descriptionLocalized = new LocalizedString("New Table", myUpgrades[nowCharacterSelectIdxY - 2].descriptionKey);
                descriptionLocalized.Arguments = new object[] { myUpgrades[nowCharacterSelectIdxY - 2].value, myUpgrades[nowCharacterSelectIdxY - 2].value2 };
                descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
        }
        else // 업그레이드
        {
            RectTransform rect = upgradesBooks[nowCharacterSelectIdxY - 2].GetChild(nowCharacterSelectIdxX).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

            // 이 코드도 YSU분이 작성하셨습니다
            var titleLocalized = new LocalizedString("New Table", allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].nameKey);
            titleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var descriptionLocalized = new LocalizedString("New Table", allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].descriptionKey);
            // {0} 은 value, {1} 은 value2 등
            descriptionLocalized.Arguments = new object[] { allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].value, allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].value2 };
            descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private void Select()
    {
        SoundManager.Instance.PlaySFX("ui_confirm");

        if (nowCharacterSelectIdxY == 1) SetUpgrade();
        else if (nowCharacterSelectIdxY == 0) CloseShopCharacterCanvas();
        else if (!isBookOpened && nowCharacterSelectIdxY >= 2 && nowCharacterSelectIdxX == 0) StartCoroutine(SetBook());
    }

    private void SetUpgrade()
    {
        if (upgradeController.characterLevel >= 5) return; // 이미 레벨 5라면 더 이상 업그레이드 불가
        if (Managers.Status.Gold < Constant.upgradesCost[upgradeController.characterLevel + 1]) return;

        Managers.Status.Gold -= Constant.upgradesCost[upgradeController.characterLevel + 1];

        selectCursor.GetComponent<Image>().enabled = false;
        Managers.InputControl.ResetUIAction();
        shopUpgradeCanvas.GetComponent<Canvas>().enabled = true;
        //GetComponent<Canvas>().enabled = false;
        StartCoroutine(shopUpgradeCanvas.SetUpgrades(nowCharacterOrderIdx, Managers.PlayerControl.Characters[nowCharacterOrderIdx], 0.4f, 0.1f));
    }

    private void CloseShopCharacterCanvas()
    {
        /*SoundManager.Instance.PlaySFX("ui_back");
        selectCursor.GetComponent<Image>().enabled = false;
        Managers.InputControl.ResetUIAction();
        Destroy(section.GetChild(0).gameObject);
        for(int i = 0; i < Constant.maxUpgrade; i++)
        {
            for (int j = upgradesBooks[i].childCount - 1; j >= 0; j--) Destroy(upgradesBooks[i].GetChild(j).gameObject);
            upgradeDisplayIcons[i].color = new(1f, 1f, 1f, 0f);
        }*/
        SoundManager.Instance.PlaySFX("ui_back");
        section.GetChild(0).gameObject.SetActive(false);
        Managers.InputControl.ResetUIAction();
        shopCanavs.StartShopCanvas();
        Debug.Log("jhasdasdasdasdsadasdas");
        GetComponent<Canvas>().enabled = false;
    }

    private IEnumerator SelectEffect(float maxTime)
    {
        Vector2 initSize = selectCursor.sizeDelta;
        float nowTime = 0f;
        while (nowTime <= maxTime)
        {
            selectCursor.sizeDelta = Vector2.Lerp(initSize * Constant.cursorEffectSize, initSize, nowTime / maxTime);
            nowTime += Time.deltaTime;
            yield return null;
        }
        selectCursor.sizeDelta = initSize;
    }
}
