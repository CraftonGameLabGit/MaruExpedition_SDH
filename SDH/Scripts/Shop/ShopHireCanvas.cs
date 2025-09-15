using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ShopHireCanvas : MonoBehaviour
{
    [SerializeField] private Canvas curtain;
    [SerializeField] private ShopCanavs shopCanavas;
    [SerializeField] private TextMeshProUGUI hireTxt;
    [SerializeField] private RectTransform hireDisplay;
    [SerializeField] private RectTransform hirePanel;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private RectTransform restCanvas; // 선택한 옵션 확인 효과용 캔버스
    private int[] characterOptionsIdx;
    private int nowHireSelectIdx = 0;
    private IEnumerator selectEffect;

    private void Start() // 고용은 상점 시작마다 하니까 조작키 활성화도 여기서 바로 하기
    {
        if (Managers.Shop.IsHired || Managers.PlayerControl.Characters.Count >= Constant.maxCharacter) // (이어하기) 이번 스테이지에서 이미 고용했거나 동료가 다 차면 고용하지 않음
        {
            Managers.Shop.IsHired = true;
            StartCoroutine(NoHireDirect());
        }
        else
        {
            StartCoroutine(FadeInSetIcon(0.4f, 0.1f));
            // 초기화는 SetIcon에서 함
        }
    }

    private IEnumerator NoHireDirect() // 고용했어도 페이드인 할건해야제
    {
        shopCanavas.SetShopCharacter();

        // 페이드인
        curtain.enabled = true;
        float nowTime = 0f, fadeTime = 0.4f;
        while (nowTime <= fadeTime)
        {
            curtain.GetComponent<CanvasGroup>().alpha = 1 - nowTime / fadeTime;
            nowTime += Time.deltaTime;
            yield return null;
        }
        curtain.GetComponent<CanvasGroup>().alpha = 0f;
        curtain.enabled = false;

        shopCanavas.StartShopCanvas();
        Destroy(gameObject);
    }

    private void GetLeft() { SetNowSelectIdx(nowHireSelectIdx - 1); }
    private void GetRight() { SetNowSelectIdx(nowHireSelectIdx + 1); }

    private void GetMouseInput()
    {
        for(int i = 0; i < hirePanel.childCount; i++)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(hirePanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdx(i);
                return;
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    private void SetNowSelectIdx(int newIdx) // 다른 옵션으로 넘어가고 색을 변경 
    {
        if (newIdx < 0 || newIdx > hirePanel.childCount - 1) return; // 인덱스 밖 
        if (nowHireSelectIdx == newIdx) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        hirePanel.GetChild(nowHireSelectIdx).GetComponent<CanvasGroup>().alpha = 0.7f; // 원래 선택한 옵션 강조 해제
        nowHireSelectIdx = newIdx;
        hirePanel.GetChild(nowHireSelectIdx).GetComponent<CanvasGroup>().alpha = 1f; // 새로 선택한 옵션 강조 설정

        RectTransform rect = hirePanel.GetChild(nowHireSelectIdx).GetComponent<RectTransform>();
        selectCursor.position = rect.position;
        selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private IEnumerator FadeInSetIcon(float maxTime, float waitTime) // 페이드 인 연출 + 아이콘 설정
    {
        GetComponent<Canvas>().enabled = true;

        var hireLocalized = new LocalizedString("New Table", "UI_Shop_Hire");
        hireLocalized.Arguments = new object[] { Managers.PlayerControl.Characters.Count + 1 };
        hireLocalized.StringChanged += (localized) => { hireTxt.text = localized; };

        // 페이드 인
        curtain.enabled = true;
        float nowTime = 0f, fadeTime = 0.4f;
        while (nowTime <= fadeTime)
        {
            curtain.GetComponent<CanvasGroup>().alpha = 1 - nowTime / fadeTime;
            nowTime += Time.deltaTime;
            yield return null;
        }
        curtain.GetComponent<CanvasGroup>().alpha = 0f;
        curtain.enabled = false;

        // 중복 없이 랜덤으로 3명 뽑기
        List<int> hireIndices = new();
        for (int i = 0; i < Managers.Asset.Characters.Length; i++)
        {
            if (Managers.PlayerControl.CharactersCheck[i]) continue; // 기존에 고용된 영웅과 중복 체크
            hireIndices.Add(i);
        }
        characterOptionsIdx = hireIndices.OrderBy(x => Random.value).Take(3).ToArray();

        // 아이콘 설정
        for (int i = 0; i < characterOptionsIdx.Length; i++)
        {
            GameObject iconCast = new GameObject("IconCast", typeof(RectTransform));
            iconCast.transform.SetParent(hireDisplay);

            GameObject hireOption = Instantiate(Managers.Asset.SelectDisplayTemplate, new Vector3(0f, -1000f, 0f), Quaternion.identity, hirePanel);
            hireOption.GetComponent<SelectDisplayTemplate>().SetHire(characterOptionsIdx[i]);
        }

        Canvas.ForceUpdateCanvases();

        Vector3[] IconPositions = Enumerable.Range(0, hireDisplay.childCount).Select(x => hireDisplay.GetChild(x).position).ToArray();
        for (int i = hireDisplay.childCount - 1; i >= 0; i--) Destroy(hireDisplay.GetChild(i).gameObject);

        for (int i = 0; i < hirePanel.childCount; i++)
        {
            StartCoroutine(IconEffect(hirePanel.GetChild(i).GetComponent<RectTransform>(), IconPositions[i], maxTime, i * waitTime));
        }

        yield return new WaitForSeconds(maxTime + (hirePanel.childCount - 1) * waitTime);

        // 기본값은 캐릭터 보기
        selectCursor.GetComponent<Image>().enabled = true;
        nowHireSelectIdx = 1;
        hirePanel.GetChild(nowHireSelectIdx).GetComponent<CanvasGroup>().alpha = 1f;

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += HireCharacter;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, null, null);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += HireCharacter;
        //GetMouseInput();
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

    private void HireCharacter() // 동료 고용
    {
        SoundManager.Instance.PlaySFX("ui_confirm");
        
        Managers.InputControl.ResetUIAction();

        Transform selectedOption = hirePanel.GetChild(nowHireSelectIdx);
        selectedOption.SetParent(restCanvas);
        selectedOption.AddComponent<SelectDirect>();

        // 🎯 Shop_Select 이벤트 로깅
        LogShopSelectEvent();

        Managers.PlayerControl.Characters.Add(Instantiate(Managers.Asset.Characters[characterOptionsIdx[nowHireSelectIdx]], Managers.PlayerControl.NowPlayer.transform));
        Managers.PlayerControl.CharactersIdx.Add(characterOptionsIdx[nowHireSelectIdx]);
        Managers.PlayerControl.CharactersCheck[characterOptionsIdx[nowHireSelectIdx]] = true;
        Managers.PlayerControl.SetPlayer();

        Managers.Shop.IsHired = true;
        Managers.Data.SaveGameData();

        shopCanavas.SetShopCharacter();
        shopCanavas.StartShopCanvas();
        Destroy(gameObject);
    }

    /// <summary>
    /// Shop_Select 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogShopSelectEvent()
    {
        try
        {
            // 등장한 캐릭터 3종 (CSV 형태)
            string offeredCharacters = GenerateOfferedCharactersCSV();
            
            // 선택된 캐릭터
            string selectedCharacter = GetSelectedCharacterName();
            
            // 현재 난이도
            int difficulty = Managers.Stage.Difficulty;
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogShopSelect(offeredCharacters, selectedCharacter, difficulty.ToString());
                
                Debug.Log($"[Shop_Select] OfferedCharacters={offeredCharacters}, SelectedCharacter={selectedCharacter}, Difficulty={difficulty}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Shop_Select] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 등장한 캐릭터 3종을 CSV 형태로 생성
    /// </summary>
    private string GenerateOfferedCharactersCSV()
    {
        try
        {
            var offeredNames = new System.Collections.Generic.List<string>();
            
            for (int i = 0; i < characterOptionsIdx.Length; i++)
            {
                int charIdx = characterOptionsIdx[i];
                var character = Managers.Asset.Characters[charIdx].GetComponent<Character>();
                string characterName = character != null ? character.characterNameKey : $"Character_{charIdx}";
                offeredNames.Add(characterName);
            }
            
            return string.Join(",", offeredNames);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Shop_Select] 등장 캐릭터 CSV 생성 실패: {e.Message}");
            return "Error,Error,Error";
        }
    }
    
    /// <summary>
    /// 선택된 캐릭터 이름 가져오기
    /// </summary>
    private string GetSelectedCharacterName()
    {
        try
        {
            int selectedCharIdx = characterOptionsIdx[nowHireSelectIdx];
            var character = Managers.Asset.Characters[selectedCharIdx].GetComponent<Character>();
            return character != null ? character.characterNameKey : $"Character_{selectedCharIdx}";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Shop_Select] 선택된 캐릭터 이름 가져오기 실패: {e.Message}");
            return "Error";
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
