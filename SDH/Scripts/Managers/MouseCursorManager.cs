using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    // 싱글톤
    public static MouseCursorManager Instance { get; private set; }
    
    [Header("Cursor Settings")]
    [SerializeField] private string cursorResourcePath = "Cursor"; // Resources 폴더 내 경로
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
    [SerializeField] private float hideCursorDelay = 3f;
    
    // 내부 상태 관리
    private Vector3 lastMousePosition;
    private float mouseIdleTimer;
    private bool isCursorVisible = true;
    private Texture2D cachedCursorTexture; // 캐시된 커서 텍스처

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Resources에서 커서 텍스처 로드
        LoadCursorTexture();
    }

    private void LoadCursorTexture()
    {
        cachedCursorTexture = Resources.Load<Texture2D>(cursorResourcePath);
        if (cachedCursorTexture == null)
        {
            Debug.LogWarning($"커서 텍스처를 찾을 수 없습니다: Resources/{cursorResourcePath}");
        }
        else
        {
            Debug.Log($"커서 텍스처 로드 성공: {cachedCursorTexture.name}, 크기: {cachedCursorTexture.width}x{cachedCursorTexture.height}");
        }
    }

    private void Start()
    {
        // 게임 시작 시 기본 커서 설정
        SetCustomCursor();
        
        // 초기 마우스 위치 저장
        lastMousePosition = Input.mousePosition;
        isCursorVisible = true;
        Cursor.visible = true;
    }

    private void SetCustomCursor()
    {
        if (cachedCursorTexture != null)
        {
            // ForceSoftware 모드로 플랫폼 호환성 향상
            Cursor.SetCursor(cachedCursorTexture, cursorHotspot, CursorMode.ForceSoftware);
            Debug.Log("커서 설정 완료");
        }
        else
        {
            Debug.LogError("커서 텍스처가 없어 기본 커서를 사용합니다.");
        }
    }

    private void Update()
    {
        CheckMouseMovement();
    }

    private void CheckMouseMovement()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        
        // 마우스가 움직였는지 확인
        if (currentMousePosition != lastMousePosition)
        {
            // 마우스가 움직임 - 타이머 리셋 및 커서 보이기
            mouseIdleTimer = 0f;
            
            if (!isCursorVisible)
            {
                Cursor.visible = true;
                isCursorVisible = true;
                // 커서가 다시 보일 때 커스텀 커서 재설정
                SetCustomCursor();
            }
            
            lastMousePosition = currentMousePosition;
        }
        else
        {
            // 마우스가 안 움직임 - 타이머 증가
            mouseIdleTimer += Time.deltaTime;
            
            // 지정된 시간이 지나면 커서 숨기기
            if (mouseIdleTimer >= hideCursorDelay && isCursorVisible)
            {
                Cursor.visible = false;
                isCursorVisible = false;
            }
        }
    }
    
    // 외부에서 커서 텍스처를 변경할 수 있는 메서드
    public void ChangeCursor(string resourcePath)
    {
        cursorResourcePath = resourcePath;
        LoadCursorTexture();
        SetCustomCursor();
    }
} 