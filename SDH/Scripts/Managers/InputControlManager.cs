using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class InputControlManager // Input System (UI항목)
{
    #region InputActions
    public Action ShiftAction;
    public Action DashAction;
    public Action UseAction;
    public Action UseCancelAction;
    public Action LeftAction, RightAction, DownAction, UpAction;
    public Action QuitAction;
    public Action MouseMoveAction;
    public Action MouseUseAction;
    public Action MouseUseCancelAction;
    public Action SettingsToggleAction; // 설정창 토글 이벤트
    public Action SettingsSubmitAction; // 설정창 Submit 이벤트 (Space)
    public Action SettingsCancelAction; // 설정창 Cancel 이벤트 (Escape)
    public bool IsSettingsMode { get; set; } = false; // 설정창 모드 플래그
    public Vector2 InputShift => inputShift;
    private Vector2 inputShift; // 비행체 상하좌우 이동
    public bool InputDash => inputDash;
    private bool inputDash; // 비행체 대시
    public Vector2 InputMouseMove => inputMouseMove;
    private Vector2 inputMouseMove; // 마우스 이동
    public bool PossInputMouseUse
    {
        get
        {
            return possInputMouseUse;
        }
        set
        {
            possInputMouseUse = value;
        }
    }
    private bool possInputMouseUse; // 마우스 클릭 가능 여부
    private long useFrameCount; // 한 프레임 내 중복 입력 방지

    public MonoBehaviour CoroutineRunner { get { return coroutineRunner; } set { coroutineRunner = value; } }
    private MonoBehaviour coroutineRunner; // 입력반복 코루틴 실행용
    private IEnumerator repeatLeft, repeatRight, repeatDown, repeatUp;

    public Action heldAction;
    public InputControl HeldControl => heldControl;
    private InputControl heldControl; // 눌린 버튼
    private IDisposable disposable; // (엔딩씬전용) AnyKey 확인용

    private InputSystem_Actions inputActions;
    private InputAction shiftInputAction;
    private InputAction dashInputAction;
    private InputAction leftInputAction, rightInputAction, downInputAction, upInputAction;
    private InputAction useInputAction;
    private InputAction quitInputAction;
    private InputAction mouseMoveInputAction;
    private InputAction mouseUseInputAction;
    
    // SettingsUI 액션맵용 추가
    private InputAction settingsNavigateInputAction;
    private InputAction settingsSubmitInputAction;
    private InputAction settingsCancelInputAction;
    
    // 이전 액션맵 기억용
    private bool wasPlayerActionMapActive = false;
    #endregion

    public void Init()
    {
        useFrameCount = -1;
        ResetUIAction();

        inputActions = new();

        shiftInputAction = inputActions.Player.Shift;
        dashInputAction = inputActions.Player.Dash;
        leftInputAction = inputActions.UI.Left; rightInputAction = inputActions.UI.Right; downInputAction = inputActions.UI.Down; upInputAction = inputActions.UI.Up;
        useInputAction = inputActions.UI.Use;
        quitInputAction = inputActions.UI.Quit;
        mouseMoveInputAction = inputActions.UI.MouseMove;
        mouseUseInputAction = inputActions.UI.MouseUse;

        // SettingsUI 액션맵 초기화
        settingsNavigateInputAction = inputActions.SettingsUI.Navigate;
        settingsSubmitInputAction = inputActions.SettingsUI.Submit;
        settingsCancelInputAction = inputActions.SettingsUI.Cancel;

        Subscribe(shiftInputAction, OnShift);
        Subscribe(dashInputAction, OnDash); 
        Subscribe(leftInputAction, OnLeft); Subscribe(rightInputAction, OnRight); Subscribe(downInputAction, OnDown); Subscribe(upInputAction, OnUp);
        Subscribe(useInputAction, OnUse);
        Subscribe(quitInputAction, OnQuit);
        Subscribe(mouseMoveInputAction, OnMouseMove);
        Subscribe(mouseUseInputAction, OnMouseUse);
        
        // SettingsUI 액션들 구독
        Subscribe(settingsSubmitInputAction, OnSettingsSubmit);
        Subscribe(settingsCancelInputAction, OnSettingsCancel);
        
        // Player.Quit도 구독해서 설정창 토글 처리
        Subscribe(inputActions.Player.Quit, OnPlayerQuit);

        EnableUI();
    }

    private void Subscribe(InputAction inputAction, Action<InputAction.CallbackContext> action)
    {
        inputAction.started += action;
        inputAction.performed += action;
        inputAction.canceled += action;
    }

    public void SubScribeUIMove(Action left, Action right, Action down, Action up)
    {
        LeftAction += left; RightAction += right; DownAction += down; UpAction += up;
    }

    public void ResetUIAction() // UI만 리셋
    {
        if (repeatRight != null) coroutineRunner?.StopCoroutine(repeatRight);
        if (repeatLeft != null) coroutineRunner?.StopCoroutine(repeatLeft);
        if (repeatDown != null) coroutineRunner?.StopCoroutine(repeatDown);
        if (repeatUp != null) coroutineRunner?.StopCoroutine(repeatUp);
        coroutineRunner = null;
        UseAction = null;
        LeftAction = null; RightAction = null; DownAction = null; UpAction = null;
        QuitAction = null;
        MouseMoveAction = null;
        MouseUseAction = null;
        // SettingsToggleAction은 제외 - 설정창 기능은 항상 유지
    }

    public void EnableNothing()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    public void EnablePlayer()
    {
        inputActions.Player.Enable();
        inputActions.UI.Disable();
    }

    public void EnableUI()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
    }

    public void ControlSetting(bool isOn) // Esc 세팅을 켜고 끄는 용도 (왜 UI냐면 필드에선 그럴 일이 없으니까...)
    {
        if (isOn) inputActions.SettingsUI.Enable();
        else inputActions.SettingsUI.Disable();
    }

    public void EnableSettingsUI()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
        inputActions.SettingsUI.Enable();
    }

    public void SwitchToSettingsUI()
    {
        // 현재 활성 액션맵 기억
        wasPlayerActionMapActive = inputActions.Player.enabled;
        
        // SettingsUI로 전환
        EnableSettingsUI();
    }

    public void RestorePreviousActionMap()
    {
        // 이전 액션맵으로 복귀
        if (wasPlayerActionMapActive)
        {
            EnablePlayer();
        }
        else
        {
            EnableUI();
        }
    }

    public void OnShift(InputAction.CallbackContext context)
    {
        inputShift = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Started) ShiftAction?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (IsOverlap()) return; // 인게임에서만 쓰는 거라 없어도 되지 않나 싶지만 일단은 넣어둠

        inputDash = context.ReadValue<float>() > Constant.deadZone;
        if (context.phase == InputActionPhase.Started) DashAction?.Invoke();
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(repeatLeft == null && context.ReadValue<float>() > Constant.deadZone)
            {
                repeatLeft = HoldIE(LeftAction);
                coroutineRunner?.StartCoroutine(repeatLeft);
            }
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            if(coroutineRunner == null || repeatLeft != null)
            {
                coroutineRunner?.StopCoroutine(repeatLeft);
                repeatLeft = null;
            }
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (repeatRight == null && context.ReadValue<float>() > Constant.deadZone)
            {
                repeatRight = HoldIE(RightAction);
                coroutineRunner?.StartCoroutine(repeatRight);
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (coroutineRunner == null || repeatRight != null)
            {
                coroutineRunner?.StopCoroutine(repeatRight);
                repeatRight = null;
            }
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (repeatDown == null && context.ReadValue<float>() > Constant.deadZone)
            {
                repeatDown = HoldIE(DownAction);
                coroutineRunner?.StartCoroutine(repeatDown);
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (coroutineRunner == null || repeatDown != null)
            {
                coroutineRunner?.StopCoroutine(repeatDown);
                repeatDown = null;
            }
        }
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (repeatUp == null && context.ReadValue<float>() > Constant.deadZone)
            {
                repeatUp = HoldIE(UpAction);
                coroutineRunner?.StartCoroutine(repeatUp);
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (coroutineRunner == null || repeatUp != null)
            {
                coroutineRunner?.StopCoroutine(repeatUp);
                repeatUp = null;
            }
        }
    }

    public IEnumerator HoldIE(Action action) // 상하좌우 이동 반복 누름
    {
        action?.Invoke();
        yield return new WaitForSeconds(Constant.holdInit);
        while (true)
        {
            yield return null;
            action?.Invoke();
            yield return new WaitForSeconds(Constant.holdRepeat);
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (IsOverlap()) return;

        if (context.phase == InputActionPhase.Started) UseAction?.Invoke();
        else if (context.phase == InputActionPhase.Canceled) UseCancelAction?.Invoke();
    }

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (IsOverlap()) return;

        if (context.phase == InputActionPhase.Started) 
        {
            QuitAction?.Invoke();
            SettingsToggleAction?.Invoke(); // 설정창 토글 이벤트 호출
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        inputMouseMove = context.ReadValue<Vector2>();

        MouseMoveAction?.Invoke();
    }

    public void OnMouseUse(InputAction.CallbackContext context)
    {
        if (IsOverlap()) return;

        if (possInputMouseUse && context.phase == InputActionPhase.Started) MouseUseAction?.Invoke();
        else if (possInputMouseUse && context.phase == InputActionPhase.Canceled) MouseUseCancelAction?.Invoke();
    }

    public IEnumerator CheckAnyButtonPress(float maxTime, Action action, Image fillImage) // maxTime만큼 아무 버튼을 꾹 누르면 action이 실행되는 함수
    {
        inputActions.Disable(); // 다른 구독은 다 해제

        heldAction += action;
        disposable = InputSystem.onAnyButtonPress.Call(inputControl =>
        {
            if (inputControl.device is Mouse) return; // 마우스 입력은 무시

            if (heldControl == null)
            {
                heldControl = inputControl;
            }
        }); // AnyKey의 눌림을 확인하는 구독

        float holdTime;

        while (true)
        {
            yield return null;

            if (Managers.InputControl.HeldControl == null) continue;

            holdTime = 0f;

            while (heldControl.IsPressed())
            {
                holdTime += Time.deltaTime;
                if (holdTime >= maxTime)
                {
                    heldAction?.Invoke();

                    disposable?.Dispose();
                    heldControl = null;
                    heldAction = null; // 볼장 다 봤으면 초기화
                    fillImage.fillAmount = 1;
                   yield break;
                }
                fillImage.fillAmount += Time.deltaTime;

                yield return null;
            }
            fillImage.fillAmount = 0;

            heldControl = null;
        }
    }

    public string GetUseKey() // 현재 Use에 할당된 키를 string으로 반환
    {
        return useInputAction.controls[0].displayName;
    }

    public bool IsGamePad() // 게임패드가 연결되어 있으면 true, 아니면(키보드) false
    {
        return Gamepad.current != null;
    }

    private bool IsOverlap() // 같은 프레임에 두 번 눌렸으면 true;
    {
        if (useFrameCount == Time.frameCount) return true;

        useFrameCount = Time.frameCount;
        return false;
    }

    public void Clear() // 종료 시 비활성화
    {
        inputActions.Disable();
    }

    public void OnPlayerQuit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SettingsToggleAction?.Invoke();
        }
    }

    public void OnSettingsSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SettingsSubmitAction?.Invoke();
        }
    }

    public void OnSettingsCancel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SettingsCancelAction?.Invoke();
        }
    }
}
