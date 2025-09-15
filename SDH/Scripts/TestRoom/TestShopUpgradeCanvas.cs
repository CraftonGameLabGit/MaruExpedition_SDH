using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class TestShopUpgradeCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeTxt;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private RectTransform seekBtn;
    [SerializeField] private RectTransform selectCursor;
    private GameObject nowCharacter;
    private TestShopCharacterCanvas nowCharacterCanavs;
    private List<CharacterUpgrade> nowUpgrades;
    private int nowUpgradeSelectIdx;

    private bool canReroll = false;
    private bool doReroll = false;

    // �ϴ��� �Ʒ� SetUpgrades���� ���� �����ؼ� �ʱ� ��Ʈ�� ���� �ʿ����

    private void GetLeft() { SetNowSelectIdx(nowUpgradeSelectIdx == -1 ? 0 : nowUpgradeSelectIdx - 1); }
    private void GetRight() { SetNowSelectIdx(nowUpgradeSelectIdx == -1 ? upgradePanel.childCount - 1 : nowUpgradeSelectIdx + 1); }
    private void GetDown() { if (nowUpgradeSelectIdx != -1) SetNowSelectIdx(-1); }
    private void GetUp() { if (nowUpgradeSelectIdx == -1) SetNowSelectIdx((nowUpgrades.Count - 1) / 2); }

    private void Update()
    {
        if (doReroll)
        {
            SetUpgrades(nowCharacterCanavs, nowCharacter);
            doReroll = false;
            canReroll = false;
        }

        if (canReroll && Input.GetKeyDown(KeyCode.R))
        {
            Managers.InputControl.ResetUIAction();
            StopAllCoroutines();
            for (int i = upgradePanel.childCount - 1; i >= 0; i--)
            {
                Destroy(upgradePanel.GetChild(i).gameObject);
            }
            doReroll = true;
        }

    }

    private void GetMouseInput()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(seekBtn, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdx(-1);
            return;
        }
        for (int i = 0; i < upgradePanel.childCount; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(upgradePanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdx(i);
                return;
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    public void SetUpgrades(TestShopCharacterCanvas testShopCharacterCanvas, GameObject character) // �Է¹��� ����(GameObject)�� ���׷��̵� ����
    {
        nowCharacterCanavs = testShopCharacterCanvas;
        nowCharacter = character;
        nowUpgrades = character.GetComponent<UpgradeController>().ShowUpgradeChoices();
        Debug.Log("hihihiihihi   " + nowUpgrades.Count.ToString());

        for (int i = 0; i < nowUpgrades.Count; i++)
        {
            GameObject upgradeOption = Instantiate(Managers.Asset.SelectDisplayTemplate, new Vector3(0f, -1000f, 0f), Quaternion.identity, upgradePanel);
            upgradeOption.GetComponent<SelectDisplayTemplate>().SetUpgrade(nowUpgrades[i]);
        }

        GetComponent<Canvas>().enabled = true;
        Canvas.ForceUpdateCanvases();

        // �⺻���� ĳ���� ����
        selectCursor.GetComponent<Image>().enabled = true;
        nowUpgradeSelectIdx = -1;
        selectCursor.position = seekBtn.position;
        selectCursor.sizeDelta = seekBtn.sizeDelta + Constant.cursorAddSize;

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        if (Cursor.visible) GetMouseInput();

        StartCoroutine(CanReroll());
    }

    private IEnumerator CanReroll()
    {
        yield return null;
        yield return null;
        canReroll = true;
    }

    private void SetNowSelectIdx(int newIdx) // -1�� ĳ���� ���� ��ư
    {
        if (newIdx < -1 || newIdx > upgradePanel.childCount - 1) return;
        if (nowUpgradeSelectIdx == newIdx) return;
        if (nowUpgradeSelectIdx == 0 && newIdx == -1) return; // ���� ���� ���������� ������ ���� �Ʒ��� ���� ��� ����

        // �׺���̼� ���� ���
        SoundManager.Instance.PlaySFX("ui_navigate");

        if (newIdx == -1)
        {
            upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<Image>().color = new(1f, 1f, 1f, 0.95f); // ���� ������ �ɼ� ���� ����
            nowUpgradeSelectIdx = newIdx;
            selectCursor.position = seekBtn.position;
            selectCursor.sizeDelta = seekBtn.sizeDelta + Constant.cursorAddSize;
        }
        else
        {
            if (nowUpgradeSelectIdx != -1) upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<Image>().color = new(1f, 1f, 1f, 0.95f); // ���� ������ �ɼ� ���� ����
            nowUpgradeSelectIdx = newIdx;
            upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<Image>().color = new(1f, 1f, 1f, 1f); // ���� ������ �ɼ� ���� ����

            RectTransform rect = upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;
        }
    }

    private void Select()
    {
        if (nowUpgradeSelectIdx == -1)
        {
            SoundManager.Instance.PlaySFX("ui_back");

            Managers.InputControl.ResetUIAction();
            GetComponent<CanvasGroup>().alpha = 0f;
            Managers.InputControl.UseCancelAction += Seek;
            Managers.InputControl.MouseUseCancelAction += Seek;
        }
        else
        {
            SoundManager.Instance.PlaySFX("ui_confirm");
            UpgradeCharacter();
        }
    }

    private void Seek() // ĳ����â ����
    {
        GetComponent<CanvasGroup>().alpha = 1f;

        Managers.InputControl.UseCancelAction = null;
        Managers.InputControl.MouseUseCancelAction = null;

        Managers.InputControl.ResetUIAction();
        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
    }

    private void UpgradeCharacter()
    {
        canReroll = false;
        doReroll = false;
        for (int i = upgradePanel.childCount - 1; i >= 0; i--)
        {
            Destroy(upgradePanel.GetChild(i).gameObject);
        }
        Managers.InputControl.ResetUIAction();
        nowUpgrades[nowUpgradeSelectIdx].ApplyUpgrade(nowCharacter);
        selectCursor.GetComponent<Image>().enabled = false;
        nowCharacterCanavs.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        nowCharacterCanavs.SetUpgrades();
    }
}
