using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeCanvas : MonoBehaviour
{
    [SerializeField] private ShopCharacterCanvas[] shopCharacterCanvas;
    [SerializeField] private TextMeshProUGUI upgradeTxt;
    [SerializeField] private RectTransform upgradeDisplay;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private RectTransform seekBtn;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private RectTransform restCanvas; // 선택한 옵션 확인 효과용 캔버스
    private int nowCharacterOrderIdx;
    private GameObject nowCharacter;
    private List<CharacterUpgrade> nowUpgrades;
    private int nowUpgradeSelectIdx;
    private IEnumerator selectEffect;
    private IEnumerator seekEffect;

    // 일단은 아래 SetUpgrades에서 같이 실행해서 초기 컨트롤 설정 필요없음

    private void GetLeft()
    {
        if (nowUpgradeSelectIdx == 0) return;
        else SetNowSelectIdx(nowUpgradeSelectIdx == -1 ? 0 : nowUpgradeSelectIdx - 1);
    }
    private void GetRight() { SetNowSelectIdx(nowUpgradeSelectIdx == -1 ? upgradePanel.childCount - 1 : nowUpgradeSelectIdx + 1); }
    private void GetDown() { if (nowUpgradeSelectIdx != -1) SetNowSelectIdx(-1); }
    private void GetUp() { if (nowUpgradeSelectIdx == -1) SetNowSelectIdx((nowUpgrades.Count - 1) / 2); }

    private void GetMouseInput()
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(seekBtn, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
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

    public IEnumerator SetUpgrades(int characterOrderIdx, GameObject character, float maxTime, float waitTime) // 입력받은 동료(GameObject)의 업그레이드 세팅
    {
        nowCharacterOrderIdx = characterOrderIdx;
        nowCharacter = character;
        nowUpgrades = character.GetComponent<UpgradeController>().ShowUpgradeChoices();

        for (int i = 0; i < nowUpgrades.Count; i++)
        {
            GameObject iconCast = new GameObject("IconCast", typeof(RectTransform));
            iconCast.transform.SetParent(upgradeDisplay);

            GameObject upgradeOption = Instantiate(Managers.Asset.SelectDisplayTemplate, new Vector3(0f, -1000f, 0f), Quaternion.identity, upgradePanel);
            upgradeOption.GetComponent<SelectDisplayTemplate>().SetUpgrade(nowUpgrades[i]);
        }

        GetComponent<Canvas>().enabled = true;
        Canvas.ForceUpdateCanvases();

        Vector3[] IconPositions = Enumerable.Range(0, upgradeDisplay.childCount).Select(x => upgradeDisplay.GetChild(x).position).ToArray();
        for (int i = upgradeDisplay.childCount - 1; i >= 0; i--) Destroy(upgradeDisplay.GetChild(i).gameObject);

        for (int i = 0; i < upgradePanel.childCount; i++)
        {
            StartCoroutine(IconEffect(upgradePanel.GetChild(i).GetComponent<RectTransform>(), IconPositions[i], maxTime, i * waitTime));
        }

        yield return new WaitForSeconds(maxTime + (upgradePanel.childCount - 1) * waitTime);

        // 기본값은 캐릭터 보기
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
    }

    private IEnumerator IconEffect(RectTransform rect, Vector2 endPos, float maxTime, float waitTime)
    {
        rect.position = endPos + Vector2.down * 30f;
        yield return new WaitForSeconds(waitTime);

        Vector2 endSize = rect.sizeDelta;
        rect.sizeDelta = new(endSize.x * 0.5f, endSize.y * 1.5f);
        float nowTime = 0f;

        while (nowTime <= maxTime)
        {
            rect.position = Vector2.Lerp(rect.position, endPos, 0.2f);
            rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, endSize, 0.2f);
            nowTime += Time.deltaTime;
            yield return null;
        }

        rect.position = endPos;
        rect.sizeDelta = endSize;
    }

    private void SetNowSelectIdx(int newIdx) // -1은 캐릭터 보기 버튼
    {
        if (newIdx < -1 || newIdx > upgradePanel.childCount - 1) return;
        if (nowUpgradeSelectIdx == newIdx) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        if(newIdx == -1)
        {
            upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<CanvasGroup>().alpha = 0.7f; // 원래 선택한 옵션 강조 해제
            nowUpgradeSelectIdx = newIdx;
            selectCursor.position = seekBtn.position;
            selectCursor.sizeDelta = seekBtn.sizeDelta + Constant.cursorAddSize;
        }
        else
        {
            if (nowUpgradeSelectIdx != -1) upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<CanvasGroup>().alpha = 0.7f; // 원래 선택한 옵션 강조 해제
            nowUpgradeSelectIdx = newIdx;
            upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<CanvasGroup>().alpha = 1f; // 새로 선택한 옵션 강조 설정

            RectTransform rect = upgradePanel.GetChild(nowUpgradeSelectIdx).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;
        }

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private void Select()
    {
        if (nowUpgradeSelectIdx == -1)
        {
            SoundManager.Instance.PlaySFX("ui_back");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<CanvasGroup>().alpha = 0f;
            Managers.InputControl.UseCancelAction += SeekIE;
            Managers.InputControl.MouseUseCancelAction += SeekIE;
        }
        else
        {
            SoundManager.Instance.PlaySFX("ui_confirm");
            UpgradeCharacter();
        }
    }

    private void SeekIE()
    {
        Managers.InputControl.UseCancelAction = null;
        Managers.InputControl.MouseUseCancelAction = null;
        StartCoroutine(Seek());
    }

    private IEnumerator Seek() // 캐릭터창 보기
    {
        float nowTime = 0f, maxTime = 0.1f;
        while (nowTime <= maxTime)
        {
            GetComponent<CanvasGroup>().alpha = nowTime / maxTime;
            nowTime += Time.deltaTime;
            yield return null;
        }
        GetComponent<CanvasGroup>().alpha = 1f;

        // Managers.InputControl.UseCancelAction = null;
        // Managers.InputControl.MouseUseCancelAction = null;

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        //GetMouseInput();
    }

    private void UpgradeCharacter()
    {
        // 🎯 Character_Upgraded 이벤트 로깅
        LogCharacterUpgradedEvent();
        
        Managers.InputControl.ResetUIAction();

        Transform selectedOption = upgradePanel.GetChild(nowUpgradeSelectIdx);
        selectedOption.SetParent(restCanvas);
        selectedOption.AddComponent<SelectDirect>();

        for (int i = upgradePanel.childCount - 1; i >= 0; i--)
        {
            Destroy(upgradePanel.GetChild(i).gameObject);
        }

        nowUpgrades[nowUpgradeSelectIdx].ApplyUpgrade(nowCharacter);
        Managers.Data.SaveGameData();
        GetComponent<Canvas>().enabled = false;
        selectCursor.GetComponent<Image>().enabled = false;
        shopCharacterCanvas[nowCharacterOrderIdx].SetUpgrades();
    }

    /// <summary>
    /// Character_Upgraded 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogCharacterUpgradedEvent()
    {
        try
        {
            // 강화된 캐릭터 ID
            string characterID = GetCharacterID();
            
            // 선택된 강화 ID
            string upgradeID = GetSelectedUpgradeID();
            
            // 등장한 강화 목록 (CSV 형태)
            string upgradeCandidates = GenerateUpgradeCandidatesCSV();
            
            // 업그레이드 후 캐릭터 레벨
            int characterLevel = GetCharacterLevel();
            
            // 현재 난이도
            int difficulty = Managers.Stage.Difficulty;
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogCharacterUpgraded(characterID, upgradeID, upgradeCandidates, characterLevel, difficulty.ToString());
                
                Debug.Log($"[Character_Upgraded] CharacterID={characterID}, UpgradeID={upgradeID}, UpgradeCandidates={upgradeCandidates}, CharacterLevel={characterLevel}, Difficulty={difficulty}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Character_Upgraded] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 강화된 캐릭터 ID 가져오기
    /// </summary>
    private string GetCharacterID()
    {
        try
        {
            var character = nowCharacter.GetComponent<Character>();
            return character != null ? character.characterNameKey : "UnknownCharacter";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Character_Upgraded] 캐릭터 ID 가져오기 실패: {e.Message}");
            return "Error";
        }
    }
    
    /// <summary>
    /// 선택된 강화 ID 가져오기
    /// </summary>
    private string GetSelectedUpgradeID()
    {
        try
        {
            if (nowUpgradeSelectIdx >= 0 && nowUpgradeSelectIdx < nowUpgrades.Count)
            {
                var selectedUpgrade = nowUpgrades[nowUpgradeSelectIdx];
                return selectedUpgrade.nameKey ?? "UnknownUpgrade";
            }
            return "InvalidSelection";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Character_Upgraded] 선택된 업그레이드 ID 가져오기 실패: {e.Message}");
            return "Error";
        }
    }
    
    /// <summary>
    /// 등장한 강화 목록을 CSV 형태로 생성
    /// </summary>
    private string GenerateUpgradeCandidatesCSV()
    {
        try
        {
            var upgradeNames = new System.Collections.Generic.List<string>();
            
            foreach (var upgrade in nowUpgrades)
            {
                string upgradeName = upgrade.nameKey ?? "UnknownUpgrade";
                upgradeNames.Add(upgradeName);
            }
            
            return string.Join(",", upgradeNames);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Character_Upgraded] 업그레이드 후보 CSV 생성 실패: {e.Message}");
            return "Error,Error,Error";
        }
    }
    
    /// <summary>
    /// 업그레이드 후 캐릭터 레벨 가져오기
    /// </summary>
    private int GetCharacterLevel()
    {
        try
        {
            var upgradeController = nowCharacter.GetComponent<UpgradeController>();
            return upgradeController != null ? upgradeController.characterLevel : 1;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Character_Upgraded] 캐릭터 레벨 가져오기 실패: {e.Message}");
            return 1;
        }
    }

    private IEnumerator SelectEffect(float maxTime)
    {
        Vector2 initSize = selectCursor.sizeDelta;
        float nowTime = 0f;
        while (nowTime <= maxTime)
        {
            selectCursor.sizeDelta = Vector2.Lerp(initSize * Constant.cursorEffectLowSize, initSize, nowTime / maxTime);
            nowTime += Time.deltaTime;
            yield return null;
        }
        selectCursor.sizeDelta = initSize;
    }
}
