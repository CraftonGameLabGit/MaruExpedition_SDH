using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TestShopCharacterCanvas : MonoBehaviour
{
    public GameObject nowCharacter; // ���� ĳ����

    [SerializeField] private TestShopUpgradeCanvas shopUpgradeCanvas;
    [SerializeField] private RectTransform section; // ������ (ĳ���Ͱ� ���̴� ��)
    [SerializeField] private TextMeshProUGUI characterNameTxt; // ĳ���� �̸�
    [SerializeField] private TextMeshProUGUI characterLevelTxt;
    [SerializeField] private TextMeshProUGUI characterDescriptionTxt; // ĳ���� ����
    [SerializeField] private TextMeshProUGUI[] characterStatusItems; // ĳ���� ���� �׸�
    [SerializeField] private TextMeshProUGUI[] characterStatusValues; // ĳ���� ���� ��
    [SerializeField] private RectTransform bookBtn; // ��ų ���� ���� ��ư
    [SerializeField] private RectTransform upgradesBookBackground; // ���׷��̵� ���� �̹���
    [SerializeField] private RectTransform upgradesBook; // ���׷��̵� ����
    [SerializeField] private RectTransform[] upgradesBooks; // ���׷��̵� ����
    [SerializeField] private RectTransform[] upgradeDisplays; // ����� ���׷��̵�
    [SerializeField] private Image[] upgradeDisplayIcons; // ����� ���׷��̵� �̹���
    [SerializeField] private RectTransform upgradeBtn; // ���׷��̵� ��ư
    [SerializeField] private TextMeshProUGUI upgradeBtnTxt; // ���׷��̵� ��ư �ؽ�Ʈ
    [SerializeField] private RectTransform returnBtn; // ���ư��� ��ư
    [SerializeField] private TextMeshProUGUI upgradeTitleTxt;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionTxt;
    [SerializeField] private RectTransform selectCursor;
    private int nowCharacterSelectIdxX, nowCharacterSelectIdxY; // �ɼ� �ε���
    private UpgradeController upgradeController; // ���� ĳ���� ���׷��̵� ����
    private List<List<CharacterUpgrade>> allUpgrades; // ���׷��̵� ����Ʈ
    private List<CharacterUpgrade> myUpgrades = new(); // ���� ĳ������ ���׷��̵�
    private bool isBookOpened = false; // ��ų ������ �����ִ���

    public void SetShopCharacterCanvas(GameObject ch)
    {
        nowCharacter = ch;

        upgradeController = nowCharacter.GetComponent<UpgradeController>();
        allUpgrades = upgradeController.allUpgrades;
        /*GameObject character = Instantiate(Managers.Asset.OptionTemplate, section);
        character.GetComponent<OptionTemplate>().SetCharacterIcon(Managers.PlayerControl.CharactersIdx[nowCharacterOrderIdx], true);
        SortingGroup sortingGroup = character.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "FrontGround";
        sortingGroup.sortingOrder = 2;*/
        if (upgradeController.characterLevel == 5)
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle"); // �ؽ�Ʈ�� �����ؼ� �׳� �̰� ��
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        else
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeBtn");
            upgradeBtnLocalized.Arguments = new object[] { Constant.upgradesCost[upgradeController.characterLevel + 1] };
            upgradeBtnLocalized.StringChanged += (localized) => { upgradeBtnTxt.text = localized; };
        }
        var characterNameLocalized = new LocalizedString("New Table", nowCharacter.GetComponentInChildren<Character>().characterNameKey);
        characterNameLocalized.StringChanged += (localized) => { characterNameTxt.text = localized; };
        var characterLevelLocalized = new LocalizedString("New Table", "UI_Shop_CharacterLevel");
        characterLevelLocalized.Arguments = new object[] { upgradeController.characterLevel };
        characterLevelLocalized.StringChanged += (localized) => { characterLevelTxt.text = localized; };
        var characterDescriptionLocalized = new LocalizedString("New Table", nowCharacter.GetComponentInChildren<Character>().characterDescriptionKey);
        characterDescriptionLocalized.StringChanged += (localized) => { characterDescriptionTxt.text = localized; };
        SetStatus();

        for (int i = 0; i < Constant.maxUpgrade; i++)
        {
            Debug.Log("allupgradeCount: " + allUpgrades[i].Count.ToString());
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
                }
                else
                {
                    optionTemplate.OptionInnerTemplate.color = new(1f, 1f, 1f, 0.8f);
                    upgradeDisplayIcons[i].color = new(1f, 1f, 1f, 0f);
                }
            }
        }
    }

    private IEnumerator SetBook() // ���� ����
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
        if (Cursor.visible) GetMouseInput();
    }

    public void StartShopCharacterCanvas()
    {
        GetComponent<Canvas>().enabled = true;
        Managers.InputControl.EnableUI();
        Managers.InputControl.ResetUIAction();
        // ������ (0,-1) �׻� �缳��
        nowCharacterSelectIdxY = 0;
        nowCharacterSelectIdxX = -1;
        selectCursor.GetComponent<Image>().enabled = true;
        selectCursor.position = returnBtn.position;
        selectCursor.sizeDelta = returnBtn.sizeDelta + Constant.cursorAddSize;
        //section.GetChild(0).gameObject.SetActive(true);

        var returnTitleLocalized = new LocalizedString("New Table", "UI_Shop_ReturnBtn");
        returnTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
        var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_ReturnDescription");
        returnDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        if (Cursor.visible) GetMouseInput();
    }

    public void SetStatus() // ���� ����
    {
        (string statusValueKey, float[] status) = nowCharacter.GetComponent<Character>().GetStatus(); // statusKey�� ���� ���Ϸ� ����

        var characterStatusLocalized = new LocalizedString("New Table", "UI_Shop_CharacterStatus");
        characterStatusLocalized.StringChanged += (localized) =>
        {
            string[] lines = localized.Split("\n");

            for (int i = 0; i < 8; i++) // 8���� �ƴ϶�� ���� �߸��� ��
            {
                characterStatusItems[i].text = lines[i];
            }
        };
        var characterStatusValueLocalized = new LocalizedString("New Table", statusValueKey);
        characterStatusValueLocalized.Arguments = status.Cast<object>().ToList();
        characterStatusValueLocalized.StringChanged += (localized) =>
        {
            string[] lines = localized.Split("\n");

            for (int i = 0; i < 8; i++) // 8���� �ƴ϶�� ���� �߸��� ��
            {
                characterStatusValues[i].text = lines[i];
            }
        };
    }

    public void SetUpgrades() // ���׷��̵带 �ߴٸ� �������� ��ĥ�ϰ� ���� �� ��Ÿ �ؽ�Ʈ �缳��
    {
        SetStatus();
        for (int i = 0; i < allUpgrades[upgradeController.characterLevel - 2].Count; i++)
        {
            if (upgradeController._acquiredUpgrades.Contains(allUpgrades[upgradeController.characterLevel - 2][i]))
            {
                myUpgrades.Add(allUpgrades[upgradeController.characterLevel - 2][i]);
                upgradesBooks[upgradeController.characterLevel - 2].GetChild(i).GetComponent<OptionTemplate>().OptionInnerTemplate.color = new(1f, 1f, 1f, 1f);
                upgradeDisplayIcons[upgradeController.characterLevel - 2].sprite = allUpgrades[upgradeController.characterLevel - 2][i].icon;
                upgradeDisplayIcons[upgradeController.characterLevel - 2].color = new(1f, 1f, 1f, 1f);
                break;
            }
        }

        selectCursor.GetComponent<Image>().enabled = true;

        if (upgradeController.characterLevel == 5)
        {
            var upgradeBtnLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle"); // �ؽ�Ʈ�� �����ؼ� �׳� �̰� ��
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
    }

    public void SetNoUpgrades() // (�׽�Ʈ�� ����) ���׷��̵带 �� ���� �� ����
    {
        upgradeController.characterLevel--;
        selectCursor.GetComponent<Image>().enabled = true;

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
    }

    private void GetLeft() { SetNowSelectIdxX(nowCharacterSelectIdxX + 1); } // �ε����� �ݴ��� ���� ����
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
        for (int i = 0; i < Constant.maxUpgrade; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(upgradeDisplays[i], Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdxXY(i + 2, -1);
                return;
            }
        }
        if (!isBookOpened)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(upgradesBookBackground, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
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

    private void SetNowSelectIdxY(int newIdxY) // Y���� ����
    {
        if (newIdxY < 0 || newIdxY > 5) return; // �ε��� ��
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
        else if (nowCharacterSelectIdxX == -1) // ���� ��ų
        {
            nowCharacterSelectIdxY = newIdxY;
        }
        else
        {
            if (nowCharacterSelectIdxY > newIdxY) // ������ ��
            {
                nowCharacterSelectIdxX = (2 * nowCharacterSelectIdxX + upgradesBooks[newIdxY - 2].childCount - upgradesBooks[nowCharacterSelectIdxY - 2].childCount) / 2; // �밢�� ����
            }
            else // �ö� ��
            {
                nowCharacterSelectIdxX = (2 * nowCharacterSelectIdxX + upgradesBooks[newIdxY - 2].childCount - upgradesBooks[nowCharacterSelectIdxY - 2].childCount + 1) / 2;
            }

            nowCharacterSelectIdxY = newIdxY;

            if (nowCharacterSelectIdxX < 0) nowCharacterSelectIdxX = 0;
            if (nowCharacterSelectIdxX > upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1) nowCharacterSelectIdxX = upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1;
        }

        SetNowSelectIdx();
    }

    private void SetNowSelectIdxX(int newIdxX) // X���� ����
    {
        if (nowCharacterSelectIdxX == newIdxX) return;

        if (!isBookOpened && nowCharacterSelectIdxY >= 2)
        {
            if (nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX < newIdxX) nowCharacterSelectIdxX = 0;
            else if (nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX > newIdxX) return;
            else if (nowCharacterSelectIdxX == 0 && nowCharacterSelectIdxX < newIdxX) return;
            else nowCharacterSelectIdxX = -1;
        }
        else if (nowCharacterSelectIdxY <= 1 && nowCharacterSelectIdxX < newIdxX) // ���׷��̵� ��ư�̳� �ݱ� ��ư���� ������ ������ ���� ������ �Ʒ� ���׷��̵��
        {
            nowCharacterSelectIdxY = 2;
            nowCharacterSelectIdxX = 0;
        }
        else if (nowCharacterSelectIdxY > 2 && nowCharacterSelectIdxX == upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1 && nowCharacterSelectIdxX < newIdxX) // ���� ���� ���׷��̵忡�� ������ ������ ���� ��������
        {
            nowCharacterSelectIdxY--;
            nowCharacterSelectIdxX = upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1;
        }
        else if (nowCharacterSelectIdxY == 2 && nowCharacterSelectIdxX == -1 && nowCharacterSelectIdxX > newIdxX) // ���� ������ �Ʒ� ���� ��ų���� �������� ������ ���׷��̵� ��ư����
        {
            nowCharacterSelectIdxY = 1;
            nowCharacterSelectIdxX = -1;
        }
        else if (nowCharacterSelectIdxY <= 1 && nowCharacterSelectIdxX > newIdxX) return; // �ε��� ��
        else if (newIdxX < -1 || newIdxX > upgradesBooks[nowCharacterSelectIdxY - 2].childCount - 1) return; // �ε��� ��
        else
        {
            nowCharacterSelectIdxX = newIdxX;
        }

        SetNowSelectIdx();
    }

    private void SetNowSelectIdxXY(int newIdxY, int newIdxX) // XY �� �� ����
    {
        // �ʱ�ȭ�� ���콺�� ���� �뵵�� �ε��� üũ ���ʿ�
        if (nowCharacterSelectIdxY == newIdxY && nowCharacterSelectIdxX == newIdxX) return;

        nowCharacterSelectIdxY = newIdxY;
        nowCharacterSelectIdxX = newIdxX;

        SetNowSelectIdx();
    }

    private void SetNowSelectIdx() // �ٸ� ĵ������ �Ѿ�� ���⼭�� X�� Y���� �� �� �����ϱ� ������ Ŀ�� ��ġ �̵��� ������
    {
        // �׺���̼� ���� ���
        SoundManager.Instance.PlaySFX("ui_navigate");

        if (!isBookOpened && nowCharacterSelectIdxY >= 2 && nowCharacterSelectIdxX >= 0) // ��ų ���� ���� ��ư
        {
            nowCharacterSelectIdxX = 0;
            selectCursor.position = bookBtn.position;
            selectCursor.sizeDelta = bookBtn.sizeDelta + Constant.cursorAddSize;

            var bookTitleLocalized = new LocalizedString("New Table", "UI_Shop_BookTitle");
            bookTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var bookDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_BookDescription");
            bookDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }
        else if (nowCharacterSelectIdxY == 0) // �ݱ� ��ư
        {
            selectCursor.position = returnBtn.position;
            selectCursor.sizeDelta = returnBtn.sizeDelta + Constant.cursorAddSize;

            var returnTitleLocalized = new LocalizedString("New Table", "UI_Shop_ReturnBtn");
            returnTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var returnDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_ReturnDescription");
            returnDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }
        else if (nowCharacterSelectIdxY == 1) // ���׷��̵� ��ư
        {
            selectCursor.position = upgradeBtn.position;
            selectCursor.sizeDelta = upgradeBtn.sizeDelta + Constant.cursorAddSize;

            var upgradeTitleLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeTitle");
            upgradeTitleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            if (upgradeController.characterLevel >= 5) // ������ ��� �ؽ�Ʈ: �� ĳ���ʹ� �ְ� �����Դϴ�.
            {
                var maxUpgradeDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_MaxUpgradeDescription");
                maxUpgradeDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
            else // ������ �ƴ� ��� �ؽ�Ʈ: ĳ���͸� {0}�ܰ�� ���׷��̵� �մϴ�. {1}���� ������ �� �ϳ��� �����մϴ�. �䱸 ����� {2}��� �Դϴ�.
            {
                var upgradeDescriptionLocalized = new LocalizedString("New Table", "UI_Shop_UpgradeDescription");
                upgradeDescriptionLocalized.Arguments = new object[] { upgradeController.characterLevel + 1, upgradeController.characterLevel == 4 ? 2 : 3, Constant.upgradesCost[upgradeController.characterLevel + 1] };
                upgradeDescriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
        }
        else if (nowCharacterSelectIdxX == -1) // ���� ��ų
        {
            selectCursor.position = upgradeDisplays[nowCharacterSelectIdxY - 2].position;
            selectCursor.sizeDelta = upgradeDisplays[nowCharacterSelectIdxY - 2].sizeDelta + Constant.cursorAddSize;

            if (upgradeController.characterLevel < nowCharacterSelectIdxY) // �������� ���� (��ų �Ȱ�������)
            {
                var titleLocalized = new LocalizedString("New Table", "UI_Shop_NoSkillTitle");
                titleLocalized.Arguments = new object[] { nowCharacterSelectIdxY };
                titleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
                var descriptionLocalized = new LocalizedString("New Table", "UI_Shop_NoSkillDescription");
                descriptionLocalized.Arguments = new object[] { nowCharacterSelectIdxY };
                descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
            }
            else // �������� ���ų� ���� (��ų ��������)
            {
                string skillName = "";
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
        else // ���׷��̵�
        {
            RectTransform rect = upgradesBooks[nowCharacterSelectIdxY - 2].GetChild(nowCharacterSelectIdxX).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

            // �� �ڵ嵵 YSU���� �ۼ��ϼ̽��ϴ�
            var titleLocalized = new LocalizedString("New Table", allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].nameKey);
            titleLocalized.StringChanged += (localized) => { upgradeTitleTxt.text = localized; };
            var descriptionLocalized = new LocalizedString("New Table", allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].descriptionKey);
            // {0} �� value, {1} �� value2 ��
            descriptionLocalized.Arguments = new object[] { allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].value, allUpgrades[nowCharacterSelectIdxY - 2][nowCharacterSelectIdxX].value2 };
            descriptionLocalized.StringChanged += (localized) => { upgradeDescriptionTxt.text = localized; };
        }
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
        if (upgradeController.characterLevel >= 5) return; // �̹� ���� 5��� �� �̻� ���׷��̵� �Ұ�
        //if (Managers.Status.Gold < Constant.upgradesCost[upgradeController.characterLevel + 1]) return;

        //Managers.Status.Gold -= Constant.upgradesCost[upgradeController.characterLevel + 1];

        selectCursor.GetComponent<Image>().enabled = false;
        Managers.InputControl.ResetUIAction();
        shopUpgradeCanvas.GetComponent<Canvas>().enabled = true;
        //GetComponent<Canvas>().enabled = false;
        shopUpgradeCanvas.SetUpgrades(this, nowCharacter);
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
        //section.GetChild(0).gameObject.SetActive(false);
        Managers.InputControl.ResetUIAction();
        GetComponent<Canvas>().enabled = false;
        Managers.InputControl.EnablePlayer();
    }
}
