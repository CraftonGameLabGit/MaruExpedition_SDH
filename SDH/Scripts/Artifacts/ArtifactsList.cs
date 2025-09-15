// Ŭ���� ���� ���� ������ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

public partial class ArtifactsList // ��Ƽ��Ʈ�� ȿ����
{
    public class HealthUp : ArtifactTemplate // 0
    {
        public HealthUp()
        {
            icon = null;
            title = null;
            explain = "���������� ������ ������ ����ü ü�� 5 ����";
        }

        public override void Effect()
        {
            base.Effect();
            Managers.Status.MaxHp += 5;
        }
    };

    public class SpeedUp : ArtifactTemplate<PlayerMove> // 1
    {
        public SpeedUp()
        {
            icon = null;
            title = null;
            explain = "����ü �ӵ� 1 ����";
        }

        public override void Effect(PlayerMove playerMove)
        {
            base.Effect(playerMove);
            playerMove.moveSpeed += 1f;
        }
    };

    public class NoRideSpeedUp : ArtifactTemplate<PlayerMove> // 2
    {
        public NoRideSpeedUp()
        {
            icon = null;
            title = null;
            explain = "����ü�� Ÿ�� �ִ� ĳ���Ͱ� ������ �ӵ� 2��";
        }

        public override void Effect(PlayerMove playerMove)
        {
            base.Effect(playerMove);
            if (Managers.Status.RiderCount == 0) playerMove.moveSpeed *= 2f;
        }
    };

    public class BaseManaUp : ArtifactTemplate<Character> // 3
    {
        public BaseManaUp()
        {
            icon = null;
            title = null;
            explain = "���� �� ���� 20% ȸ��";
        }

        public override void Effect(Character character)
        {
            base.Effect(character);
            character.TmpCurrentMpPlus(character.maxMP * 0.2f);
        }
    };

    public class DashCooldown : ArtifactTemplate<PlayerMove> // 4
    {
        public DashCooldown()
        {
            icon = null;
            title = null;
            explain = "��� ��Ÿ�� 10% ����";
        }

        public override void Effect(PlayerMove playerMove)
        {
            base.Effect(playerMove);
            playerMove.ReduceDashCooldown();
        }
    };

    public class Revival : ArtifactTemplate<PlayerHP> // 5
    {
        public Revival()
        {
            icon = null;
            title = null;
            explain = "1ȸ ��Ȱ ����";
        }

        public override void Effect(PlayerHP playerHP)
        {
            base.Effect(playerHP);

            //playerHP.Revive++;
        }
    };
}