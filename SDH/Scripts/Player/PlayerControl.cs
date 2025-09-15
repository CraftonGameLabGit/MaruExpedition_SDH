using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerControl : MonoBehaviour // 플레이어의 전투-상점 씬 전환을 컨트롤하는 스크립트
{
    private PlayerMove playerMove;
    private Rigidbody2D rb;
    private IEnumerator PlayTimeCounter;
    public bool isInStage;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartGame() // 게임 시작할 때 불러오는 함수
    {
        DontDestroyOnLoad(gameObject);
        Managers.Status.RiderCount = Managers.PlayerControl.Characters.Count;
        StartCoroutine(PlayTimeCounter = StartPlayTimeCounter());
        GetComponentInChildren<SetToolTip>().StartToolTip();
    }

    private IEnumerator StartPlayTimeCounter()
    {
        while (true)
        {
            Managers.Stage.PlayTime += Time.deltaTime;
            yield return null;
        }
    }

    public void StopPlayerTimeCounter()
    {
        StopCoroutine(PlayTimeCounter);
    }

    public void SetPlayer() // 고용할 때마다 호출하는 함수 (상점 씬에서 호출된다는 점을 명심)
    {
        //SetFieldPosition();

        foreach (GameObject character in Managers.PlayerControl.Characters)
        {
            character.GetComponent<Character>().EndFieldAct();
            character.GetComponent<Character>().enabled = false;
        }

        //Managers.Status.RiderCount = Managers.PlayerControl.Characters.Count;
    }

    public void StageEnd() // 필드나 상점이 끝났을 때 호출되는 함수
    {
        StartCoroutine(SetStageEnd());
        GetComponent<PlayerMove>().StageEndDash();
    }

    private IEnumerator SetStageEnd() // 필드나 상점이 끝났을 때 연출
    {
        rb.bodyType = RigidbodyType2D.Static;
        playerMove.enabled = false;

        for(int i=0;i< Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].GetComponent<Character>().EndFieldAct();
            Managers.PlayerControl.Characters[i].GetComponent<Character>().enabled = false;
        }

        //Managers.Status.RiderCount = Managers.PlayerControl.Characters.Count;

        if (Managers.Stage.OnField) // 필드 돌입
        {
             //yield return StartCoroutine(ShopEnd());
        }
        else // 상점 돌입
        {
            yield return StartCoroutine(FieldEnd());
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        playerMove.enabled = true;
    }

    private IEnumerator FieldEnd() // 필드 스테이지가 끝난 뒤 연출
    {
        isInStage = false;

        Managers.PlayerControl.NowPlayer.GetComponent<PlayerMPCanvas>().HideMPCanvas();
        //Managers.SceneFlow.ClearTxt();
        yield return new WaitForSeconds(0.8f);

        float nowTime, maxTime; // 이동시간용 변수
        Vector3 startPlayerPos; // 현재 위치용 변수

        nowTime = 0f; maxTime = 0.3f; // maxTime 시간동안 모이기
        Vector3[] startCharacterPos = Enumerable.Range(0, Managers.PlayerControl.Characters.Count).Select(i => Managers.PlayerControl.Characters[i].transform.localPosition).ToArray();
        startPlayerPos = rb.position;

        Managers.Stage.EnemySpawner?.DeleteField(); // 코드 실행 뒤 투사체가 생기면 남는 문제 해결을 위해 여러 번 발동

        while (nowTime <= maxTime)
        {
            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = Vector3.Lerp(startCharacterPos[i], new(1.5f - i, 1f, 0f), nowTime / maxTime);
            }
            rb.position = Vector3.Lerp(startPlayerPos, Vector3.zero, nowTime / maxTime);

            nowTime += Time.deltaTime;
            yield return null;
        }

        SoundManager.Instance.PlaySFX("GameStageEnd");      // JSW 추가 날아가는 사운드

        /*for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f);
        }*/

        yield return new WaitForSeconds(0.1f); // 대기시간

        Managers.Stage.EnemySpawner?.DeleteField(); // 코드 실행 뒤 투사체가 생기면 남는 문제 해결을 위해 여러 번 발동

        Managers.SceneFlow.FadeOut(0.8f); // 기본값 = 뒤로이동maxTime+앞으로이동maxTime+대기시간

        nowTime = 0f; maxTime = 0.2f; // maxTime 시간동안 뒤로 이동
        while (nowTime <= maxTime)
        {
            rb.position = Vector3.Lerp(transform.position, 5 * Vector3.left, 0.2f);
            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f); // 조금씩 움직이길래 강제고정
            }

            nowTime += Time.deltaTime;
            yield return null;
        }

        Managers.Stage.EnemySpawner?.DeleteField(); // 코드 실행 뒤 투사체가 생기면 남는 문제 해결을 위해 여러 번 발동

        nowTime = 0f; maxTime = 0.5f; // maxTime 시간동안 앞으로 이동
        startPlayerPos = rb.position;
        while (nowTime <= maxTime)
        {
            rb.position = Vector3.Lerp(startPlayerPos, 40 * Vector3.right, nowTime / maxTime);
            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f); // 조금씩 움직이길래 강제고정
            }

            nowTime += Time.deltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(0.2f); // 대기시간

        Managers.SceneFlow.GotoScene("Shop");

        yield break;
    }

    public void StartFieldDirect() // 필드 시작할 때 연출
    {
        StartCoroutine(FieldDirect());
    }

    private IEnumerator FieldDirect() // 전투가 시작할 때 연출 + 비행체와 캐릭터 위치 보정 + 레이어 순서 변경
    {
        rb.position = Vector3.left * 10f + Vector3.left * Camera.main.orthographicSize * Screen.width / Screen.height;
        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].GetComponent<Character>().EndFieldAct();
            Managers.PlayerControl.Characters[i].GetComponent<Character>().enabled = false;
            Managers.PlayerControl.Characters[i].transform.SetAsLastSibling();
            Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f);
        }

        yield return new WaitForSeconds(1f); // 시작 연출과 겹치는 현상 방지용
        

        float nowTime = 0f, maxTime = 1f; // maxTime 시간동안 앞으로 이동
        Vector3 startPlayerPos = rb.position;
        while (nowTime <= maxTime)
        {
            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f); // 조금씩 움직이길래 강제고정
            }
            rb.position = Vector3.Lerp(startPlayerPos, Vector3.zero, nowTime / maxTime);

            nowTime += Time.deltaTime;
            yield return null;
        }


        if (Managers.Stage.NowStage.world == 4 && Managers.Stage.NowStage.stage == 1) // 최종보스 스테이지일때는 더 길게 못움직이게 따로 관리
        {
            Debug.Log("최종보스 스테이지 시작 연출");
            ChangeMovability(false); // 움직임을 정지시킴
            Managers.Stage.StartStage();

            StartCoroutine(CoLightningStartAction()); // 최종보스 스테이지일 때 조금 (0, -5)로 이동하는 연출
            yield return new WaitForSeconds(6f);

            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f);
                Managers.PlayerControl.Characters[i].GetComponent<Character>().enabled = true;
            }

            yield break;
        }

        yield return new WaitForEndOfFrame();

        isInStage = true;

        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f);
            Managers.PlayerControl.Characters[i].GetComponent<Character>().enabled = true;
        }

        yield return null;
        Managers.Stage.StartStage();
        yield return null;
        Managers.InputControl.EnablePlayer();
        yield return null;

        rb.bodyType = RigidbodyType2D.Dynamic; // 다시 움직일 수 있도록 설정
        playerMove.enabled = true;

        yield return null;

        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f); // 조금씩 움직이길래 강제고정
        }

        yield break;
    }

    public void SetShopPosition() // 상점이 시작할 때 비행체를 숨기기
    {
        Managers.PlayerControl.NowPlayer.transform.position = new(0f, -5000f, 0f);
    }

    public void SetOrderInLayer(Transform character) // 캐릭터들이 점프하거나 착지할 때마다 레이어 순서 변경
    {
        character?.SetAsLastSibling();

        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Managers.PlayerControl.Characters[i].GetComponent<SortingGroup>().sortingOrder = Managers.PlayerControl.Characters[i].transform.GetSiblingIndex() + 3;
        }
    }

    public void StartGameOver(float dropConstant)
    {
        StopAllCoroutines();
        StartCoroutine(GameOver(dropConstant));
    }

    private IEnumerator GameOver(float dropConstant) // 사망 연출
    {
        rb.bodyType = RigidbodyType2D.Static;
        playerMove.enabled = false;

        Managers.PlayerControl.NowPlayer.transform.GetComponent<BoxCollider2D>().enabled = false;

        foreach (GameObject character in Managers.PlayerControl.Characters)
        {
            character.transform.SetParent(null);
            character.transform.GetComponent<Character>().DeadFalling();
        }

        for(int i = 2; i < 4; i++)
        {
            Managers.PlayerControl.NowPlayer.transform.GetChild(i).gameObject.SetActive(false);
        }

        float dropSpeed = (Managers.PlayerControl.NowPlayer.transform.position.y + 15f) / dropConstant;
        while (Managers.PlayerControl.NowPlayer.transform.position.y > -20f)
        {
            Managers.PlayerControl.NowPlayer.transform.position -= new Vector3(0f, dropSpeed, 0f);
            Managers.PlayerControl.NowPlayer.transform.rotation = Quaternion.Euler(Managers.PlayerControl.NowPlayer.transform.rotation.eulerAngles + new Vector3(0, 0, dropSpeed * 5f));

            yield return null;
        }

        Managers.SceneFlow.FieldCanvas.StartGameOverDirect(1f);

        yield break;
    }

    public void ChangeMovability(bool isMovable)
    {
        if (isMovable)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // 움직일 수 있도록 설정
            rb.linearVelocity = Vector2.zero;
            playerMove.enabled = true;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Static; // 움직일 수 없도록 설정
            playerMove.enabled = false;
        }
    }

    IEnumerator CoLightningStartAction()
    {
        float nowTime = 0f, maxTime = 1f; // maxTime 시간동안 앞으로 이동
        Vector3 startPlayerPos = rb.position;
        while (nowTime <= maxTime)
        {
            for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
            {
                Managers.PlayerControl.Characters[i].transform.localPosition = new(1.5f - i, 1f, 0f); // 조금씩 움직이길래 강제고정
            }
            rb.position = Vector3.Lerp(startPlayerPos, new Vector3(0, -8, 0), nowTime / maxTime);

            nowTime += Time.deltaTime;
            yield return null;
        }
    }
}
