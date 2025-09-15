using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControlManager // 플레이어 관리
{
    public GameObject NowPlayer
    {
        get
        {
            return nowPlayer;
        }
        set
        {
            nowPlayer = value;
        }
    }
    private GameObject nowPlayer; // 현재 비행체 오브젝트
    public int VehicleIdx
    {
        get
        {
            return vehicleIdx;
        }
        set
        {
            vehicleIdx = value;
        }
    }
    private int vehicleIdx; // 현재 비행체의 에셋 내 인덱스
    public bool[] CharactersCheck
    {
        get
        {
            return charactersCheck;
        }
        set
        {
            charactersCheck = value;
        }
    }
    private bool[] charactersCheck; // 동료 오브젝트 중복 확인
    public List<GameObject> Characters => characters;
    private List<GameObject> characters = new(); // 현재 동료 오브젝트들
    public List<int> CharactersIdx
    {
        get
        {
            return charactersIdx;
        }
        set
        {
            charactersIdx = value;
        }
    }
    private List<int> charactersIdx = new(); // 현재 동료 오브젝트들의 에셋 내 인덱스

    public void StartGame() // 게임 시작
    {
        charactersCheck = new bool[Managers.Asset.Characters.Length];
        nowPlayer.GetComponent<PlayerControl>().StartGame();
    }

    public void LoadGame()
    {
        vehicleIdx = Managers.Data.LocalPlayerData.gameData.vehicleIdx;
        //charactersCheck = Managers.Data.LocalPlayerData.gameData.charactersCheck.ToArray();

        nowPlayer.GetComponent<PlayerControl>().StartGame();
    }

    public void SetPlayer() // 고용할 때마다 호출
    {
        nowPlayer.GetComponent<PlayerControl>().SetPlayer();
    }

    public void ResetGame() // 근데 오브젝트 파괴는 여기서 안되니까 알아서 해야함~
    {
        nowPlayer = null;
        // vehicleIdx는 건들 필요 없어서 놔둠
        characters = new();
        charactersIdx = new();
    }

    public bool IsOnRightSide() // 현재 플레이어가 화면 오른쪽에 있는지 확인
    {
        if (nowPlayer == null) return false;
        Debug.Log("IsOnRightSide 실행");
        //return nowPlayer.transform.position.x > Camera.main.transform.position.x;
        return nowPlayer.transform.position.x > 0;
    }
}
