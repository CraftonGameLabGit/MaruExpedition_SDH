using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager // 씬 전환 및 sceneLoaded계열 관리
{
    public FindAnyFieldCanvas FieldCanvas;

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬이 로드될 때 실행
    {
        if (scene.name == "Field") FieldSceneLoaded();
        else if (scene.name == "Shop") ShopSceneLoaded();
        else if (scene.name == "Main") MainSceneLoaded();
        else if (scene.name == "Select") SelectSceneLoaded();
    }

    private void FieldSceneLoaded() // 필드 씬이 시작될 때 실행
    {
        Managers.Data.SaveGameData();

        Managers.InputControl.EnableNothing(); // 캐릭터가 움직일 수 있도록은 PlayerControl에서 실행해줌
        StartDirect(1.2f, 0.8f);

        int world = Managers.Stage.World;
        if (Managers.Stage.NowStage != null)
        {
            if (Managers.Stage.NowStage.isBossStage) // 보스 스테이지면 월드 1 증가
            {
                SoundManager.Instance.PlayBGM($"Boss{world}");
            }
            else
            {
                SoundManager.Instance.PlayBGM($"World{world}"); // 월드별 BGM 재생
            }
        }
        else
        {
            Debug.LogError("현재 스테이지 정보가 없습니다. BGM을 재생할 수 없습니다.");
        }

        Managers.PlayerControl.NowPlayer?.GetComponent<PlayerControl>().StartFieldDirect();
    }

    private void ShopSceneLoaded() // 상점 씬이 시작될 때 실행
    {
        Managers.Data.SaveGameData();

        Managers.InputControl.EnableUI();
        Managers.PlayerControl.NowPlayer?.GetComponent<PlayerControl>().SetShopPosition();
        SoundManager.Instance.PlayBGM("Shop"); // 상점 BGM 재생

        Managers.Status.Gold = Managers.Status.Gold; // 함수 발동용
        Managers.Status.Hp = Managers.Status.MaxHp; // 체력 저장 기능 때문에 게임 시작 및 상점 씬 시작 때 초기화
    }

    private void MainSceneLoaded() // 메인 씬이 시작될 때 실행
    {
        Managers.InputControl.EnableUI();
        SoundManager.Instance.PlayBGM("Main"); // 메인 BGM 재생
    }

    private void SelectSceneLoaded() // 선택 씬이 시작될 때 실행
    {
        Managers.InputControl.EnableUI();
        SoundManager.Instance.PlayBGM("Main"); // 선택 BGM 재생
    }

    public void GotoScene(string sceneName) // string으로 씬 전환하기
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartDirect(float waitTime, float maxTime) // 필드 시작 연출
    {
        if (FieldCanvas == null)
        {
            Debug.Log("FieldCanvas 못찾음");
            return;
        }

        FieldCanvas.StartStartDirect(waitTime, maxTime);
    }

    public void FadeOut(float maxTime) // 필드가 끝난 뒤 어두워지는 연출
    {
        if (FieldCanvas == null)
        {
            Debug.Log("FieldCanvas 못찾음");
            return;
        }

        FieldCanvas.StartFadeOut(maxTime);
    }

    public void GameOver() // 게임오버
    {
        if (FieldCanvas == null)
        {
            Debug.Log("FieldCanvas 못찾음");
            return;
        }

        Managers.Stage.EnemySpawner.StopAllCoroutines();
        Managers.PlayerControl.NowPlayer.GetComponent<PlayerControl>().StartGameOver(50);
    }

    public void Clear() // 종료 시 구독 해제 (에디터용)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
