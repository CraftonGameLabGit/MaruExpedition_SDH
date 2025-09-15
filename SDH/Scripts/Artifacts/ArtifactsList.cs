// 클래스 순서 섞지 마세요 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

public partial class ArtifactsList // 아티팩트의 효과들
{
    public class HealthUp : ArtifactTemplate // 0
    {
        public HealthUp()
        {
            icon = null;
            title = null;
            explain = "스테이지를 시작할 때마다 비행체 체력 5 증가";
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
            explain = "비행체 속도 1 증가";
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
            explain = "비행체에 타고 있는 캐릭터가 없으면 속도 2배";
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
            explain = "착지 시 마나 20% 회복";
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
            explain = "대시 쿨타임 10% 감소";
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
            explain = "1회 부활 가능";
        }

        public override void Effect(PlayerHP playerHP)
        {
            base.Effect(playerHP);

            //playerHP.Revive++;
        }
    };
}