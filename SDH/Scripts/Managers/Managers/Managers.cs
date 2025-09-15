using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Managers : MonoBehaviour
{
    // Singleton
    public static Managers Instance => instance;
    private static Managers instance;
    #region Managers
    public static DataManager Data => instance.data;
    private DataManager data = new(); // 로컬 데이터 관리
    public static AssetManager Asset => instance.asset;
    private AssetManager asset = new(); // Resources에서 받아오는 각종 오브젝트 관리
    public static InputControlManager InputControl => instance.inputControl;
    private InputControlManager inputControl = new(); // Input System
    public static StatusManager Status => instance.status;
    private StatusManager status = new(); // 인게임 스탯 관리
    public static ArtifactManager Artifact => instance.artifact;
    private ArtifactManager artifact = new(); // 인게임 유물 관리
    public static StageManager Stage => instance.stage;
    private StageManager stage = new(); // 전투 관리 및 씬 흐름 관리 (상점이여도 이곳에서 관리할 수 있다는 의미)
    public static ShopManager Shop => instance.shop;
    private ShopManager shop = new(); // 상점 관리 (stageManager와는 다르게 상점 내 기능에 초점)
    public static PlayerControlManager PlayerControl => instance.playerControl;
    private PlayerControlManager playerControl = new(); // 플레이어 관리
    public static SceneFlowManager SceneFlow => instance.sceneFlow;
    private SceneFlowManager sceneFlow = new(); // 씬 전환 및 sceneLoaded계열 관리
    public static CameraManager Cam => instance.cam;
    private CameraManager cam = new(); // 카메라 관리
    public static RecordManager Record => instance.record;
    private RecordManager record = new(); // 게임 기록 관리
    public static SteamAchievementManager SteamAchievement => instance.steamAchievement;
    private SteamAchievementManager steamAchievement = new(); // 도전과제 관리
    public static  UnlockManager Unlock => instance.unlock;
    private UnlockManager unlock = new(); // 해금 관리

    #endregion
    private bool isOnly = false; // 첫 오브젝트인지 체크 (OnDestroy 에 사용하는 에디터용 코드)

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        isOnly = true;
        instance = this;
        DontDestroyOnLoad(gameObject);

        SetInit();
    }

    private void SetInit() // Awake 초기 설정
    {
        Application.targetFrameRate = 60;
        
        // 저장된 설정들 적용
        GameSettingController.ApplyStartupLanguage();   // 언어 설정 먼저 적용
        GameSettingController.ApplyStartupResolution(); // 해상도 설정 적용
        
        // FontManager 초기화 (씬에 FontManager가 있는 경우에만)
        if (FontManager.Instance == null)
        {
            // FontManager가 없으면 찾아서 생성할 수도 있음
            GameObject fontManagerObject = GameObject.Find("FontManager");
            if (fontManagerObject == null)
            {
                fontManagerObject = new GameObject("FontManager");
                fontManagerObject.AddComponent<FontManager>();
            }
        }
        
        StartCoroutine(FixResolution());

        GetComponent<PlayerInput>().enabled = true; // 시작부터 활성화하면 중복 경고 떠서 이렇게 함

        data.Init();
        unlock.Init(); // 얘는 data.Init()보다 늦게 실행되어야 함
        asset.Init();
        inputControl.Init();
        sceneFlow.Init();
    }

    private IEnumerator FixResolution()
    {
        // 전체화면 모드를 FullScreenWindow로 설정 (사용자 해상도 유지)
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        bool _wasFullScreen = Screen.fullScreen;
        FullScreenMode _wasFullScreenMode = Screen.fullScreenMode;

        while (true)
        {
            // 전체화면 모드가 변경되었을 때만 처리
            if (Screen.fullScreenMode != _wasFullScreenMode)
            {
                _wasFullScreenMode = Screen.fullScreenMode;
                
                // 전체화면 모드가 FullScreenWindow로 변경되었을 때만 사용자 설정 적용
                if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
                {
                    // 사용자가 설정한 해상도 정보 가져오기
                    int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
                    int savedHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
                    bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
                    
                    // 현재 화면 상태와 저장된 설정이 다를 때만 적용
                    if (Screen.width != savedWidth || Screen.height != savedHeight || Screen.fullScreen != isFullScreen)
                    {
                        Debug.Log($"FixResolution: 사용자 설정 적용 - {savedWidth}x{savedHeight}, FullScreen: {isFullScreen}");
                        Screen.SetResolution(savedWidth, savedHeight, isFullScreen);
                    }
                }
            }
            
            // 전체화면 상태 변경 감지 (별도 처리)
            if (Screen.fullScreen != _wasFullScreen)
            {
                _wasFullScreen = Screen.fullScreen;
                Debug.Log($"FixResolution: 전체화면 상태 변경 감지 - {_wasFullScreen}");
            }

            yield return new WaitForSeconds(0.1f); // 0.1초마다 체크 (성능 최적화)
        }
    }

    private void OnDestroy() // 초기화
    {
        if (!isOnly) return;

        data.Clear();
        inputControl.Clear();
        sceneFlow.Clear();
    }
}
