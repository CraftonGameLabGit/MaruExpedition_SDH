public class CameraManager
{
    public CameraControl CameraController;

    public void LandCharacter() // ĳ���Ͱ� ž������ ��
    {
        CameraController?.SetOrthographicSize(0.1f);
        CameraController?.StartShake(0.1f, 0.1f);
    }

    public void DashPlayer() // �÷��̾ �뽬�� ��
    {
        CameraController?.SetOrthographicSize(0.05f);
        CameraController?.StartShake(0.05f, 0.05f);
    }
}
