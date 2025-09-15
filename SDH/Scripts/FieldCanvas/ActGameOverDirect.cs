using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActGameOverDirect : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI gameClearTxt;
    [SerializeField] TextMeshProUGUI explainTxt;
    [SerializeField] GameObject time;
    [SerializeField] TextMeshProUGUI playTime;
    [SerializeField] GameObject gold;
    [SerializeField] TextMeshProUGUI totalGold;
    [SerializeField] GameObject dealLog;
    [SerializeField] List<Transform> charactersDamageInfo = new List<Transform>();
    [SerializeField] Image fillingImage;
    [SerializeField] CanvasGroup CurtainUI;
    [SerializeField] GameObject difficultyUI;

    private bool skipWait = false;

    public void StartFadeOut(float maxTime)
    {
        // 🎯 Stage_Result 이벤트 로깅 (실패)
        LogStageResultEvent("Fail");
        
        StartCoroutine(Directing(maxTime));
    }

    private IEnumerator Directing(float maxTime)
    {
        Managers.Data.LocalPlayerData.gameData = null; // 게임 데이터 초기화
        Managers.InputControl.EnableUI();
        Managers.PlayerControl.NowPlayer.GetComponent<PlayerControl>().StopPlayerTimeCounter();
        Managers.Unlock.UpdateMissionProgress(EUnlockType.DeathCount);

        float nowTime = 0f;

        SoundManager.Instance.PlaySFX("GameOverSE");

        while (nowTime <= maxTime)
        {
            image.color = new(image.color.r, image.color.g, image.color.b, 0.2f * nowTime / maxTime);
            explainTxt.color = new(explainTxt.color.r, explainTxt.color.g, explainTxt.color.b, nowTime / maxTime);

            nowTime += Time.deltaTime;
            yield return null;
        }

        if (difficultyUI != null)
        {
            difficultyUI.SetActive(true);
            if (difficultyUI != null)
            {
                difficultyUI.GetComponent<TMP_Text>().text = Managers.Stage.Difficulty.ToString();
            }
        }

        yield return WaitUntilInputOrTime(0.8f);

        time.SetActive(true);

        yield return WaitUntilInputOrTime(0.8f);

        {
            int minutes = Mathf.FloorToInt(Managers.Stage.PlayTime / 60f);
            int seconds = Mathf.FloorToInt(Managers.Stage.PlayTime % 60f);

            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            playTime.text = timeString;

            SoundManager.Instance.PlaySFX("GameCanvasResultSFX");

            yield return WaitUntilInputOrTime(0.8f);
        }

        gold.SetActive(true);
        yield return WaitUntilInputOrTime(0.8f);

        int totalGoldValue = Managers.Status.TotalGold;
        StartCoroutine(CountUpNumber(totalGoldValue, totalGold, 0.5f)); // 0.5초 동안 증가

        yield return WaitUntilInputOrTime(0.8f);

        Managers.Record.AddTotalDamageRecord(); // 총 데미지 기록 추가
        dealLog.SetActive(true);
        yield return WaitUntilInputOrTime(0.8f);

        Dictionary<ECharacterType, int> totalDamageRecord = Managers.Record.ReturnAllDamageRecord();
        var orderedRecord = totalDamageRecord.OrderByDescending(kvp => kvp.Value);

        int i = 0;
        foreach (var kvp in orderedRecord)
        {
            charactersDamageInfo[i].gameObject.SetActive(true);

            GameObject prefabToInstantiate = Resources.Load<GameObject>($"CharacterIcons/{kvp.Key}");

            if (prefabToInstantiate != null)
            {
                GameObject character = Instantiate(prefabToInstantiate, charactersDamageInfo[i].GetChild(0).transform);
                character.transform.localPosition = new(0f, -17f, 0f);
                character.transform.localScale = new(100, 100, 1f);
                Destroy(character.GetComponentInChildren<Animator>());
            }

            TMP_Text damageText = charactersDamageInfo[i].GetChild(1).GetComponent<TMP_Text>();
            yield return StartCoroutine(CountUpNumber(kvp.Value, damageText, 0.5f)); // 0.5초 동안 증가

            i++;

            yield return WaitUntilInputOrTime(0.8f);
        }

        yield return WaitUntilInputOrTime(1f);

        explainTxt.gameObject.SetActive(true);
        // explainTxt.text = "Press and Hold " + Managers.InputControl.GetUseKey() + " For 1 Sec To ReStart";

        // UseAction 키 입력 확인
        StartCoroutine(Managers.InputControl.CheckAnyButtonPress(1f, ReturnHome, fillingImage));
    }

    private IEnumerator WaitUntilInputOrTime(float waitTime)
    {
        skipWait = false;
        Managers.InputControl.UseAction += SkipWait;

        float timer = 0f;
        while (timer < waitTime)
        {
            if (skipWait)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        Managers.InputControl.UseAction -= SkipWait;
    }

    private void SkipWait()
    {
        skipWait = true;
    }

    public void ReturnHome()
    {
        foreach(GameObject character in Managers.PlayerControl.Characters)
        {
            Destroy(character);
        }
        Debug.Log("플레이어 제거");
        Managers.InputControl.ResetUIAction();
        Destroy(Managers.PlayerControl.NowPlayer);
        Time.timeScale = 1;

        Managers.Stage.ResetGame();

        StartCoroutine(ReturnHome_Co());
    }

    public IEnumerator ReturnHome_Co()
    {
        CurtainUI.gameObject.SetActive(true);
        while (true)
        {
            CurtainUI.alpha += Time.deltaTime;
            if (CurtainUI.alpha >= 1)
            {
                break;
            }
            yield return null;
        }

        Managers.SceneFlow.GotoScene("Select");
        //StopAllCoroutines(); // 모든 코루틴 중지
    }

    private IEnumerator CountUpNumber(int target, TMP_Text text, float duration)
    {
        int current = 0;
        float timer = 0f;

        skipWait = false; // 입력 대기 초기화
        Managers.InputControl.UseAction += SkipWait;

        while (timer < duration)
        {
            if (skipWait)
            {
                break;
            }

            float t = timer / duration;
            current = Mathf.RoundToInt(Mathf.Lerp(0, target, t));
            text.text = current.ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        text.text = target.ToString();
        SoundManager.Instance.PlaySFX("GameCanvasResultSFX");

        Managers.InputControl.UseAction -= SkipWait; // 입력 종료
    }

    /// <summary>
    /// Stage_Result 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogStageResultEvent(string result)
    {
        try
        {
            // 현재 스테이지 정보 가져오기
            int worldID = Managers.Stage.World;
            int stageNumber = Managers.Stage.Stage;
            int difficulty = Managers.Stage.Difficulty;
            float stageTime = Managers.Stage.PlayTime;
            
            // 데미지 요약 생성 (CSV 형태)
            string damageSummary = GenerateDamageSummary();
            
            // 🎯 실제 골드 수집률 계산
            float goldCollectRate = Managers.Status.GetGoldCollectRate();
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogStageResult(
                    worldID, stageNumber, difficulty.ToString(), result,
                    stageTime, damageSummary, goldCollectRate
                );
                
                Debug.Log($"[Stage_Result] WorldID={worldID}, StageNumber={stageNumber}, Difficulty={difficulty}, Result={result}, StageTime={stageTime:F1}s, DamageSummary={damageSummary}, GoldCollectRate={goldCollectRate:F2}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Stage_Result] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 데미지 요약 CSV 문자열 생성
    /// </summary>
    private string GenerateDamageSummary()
    {
        try
        {
            var stageDamageRecord = new System.Collections.Generic.Dictionary<ECharacterType, int>();
            
            // 현재 스테이지 데미지 기록 가져오기
            foreach (ECharacterType charType in System.Enum.GetValues(typeof(ECharacterType)))
            {
                int damage = Managers.Record.GetDamageRecord(true, charType); // true = 스테이지 데미지
                if (damage > 0)
                {
                    stageDamageRecord[charType] = damage;
                }
            }
            
            // CSV 형태로 변환: "Archer:1000,Magician:500"
            var damageEntries = new System.Collections.Generic.List<string>();
            foreach (var kvp in stageDamageRecord)
            {
                damageEntries.Add($"{kvp.Key}:{kvp.Value}");
            }
            
            return damageEntries.Count > 0 ? string.Join(",", damageEntries) : "NoDamage:0";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Stage_Result] 데미지 요약 생성 실패: {e.Message}");
            return "Error:0";
        }
    }
}