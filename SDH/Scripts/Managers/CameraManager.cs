public class CameraManager
{
    public CameraControl CameraController;

    public void LandCharacter() // 캐릭터가 탑승했을 때
    {
        CameraController?.SetOrthographicSize(0.1f);
        CameraController?.StartShake(0.1f, 0.1f);
    }

    public void DashPlayer() // 플레이어가 대쉬할 때
    {
        CameraController?.SetOrthographicSize(0.05f);
        CameraController?.StartShake(0.05f, 0.05f);
    }
}
