public partial class ArtifactManager // ��Ƽ��Ʈ ���ſ� ȿ�� ���� �Լ��� �����ϴ� �ڵ�
{
    public void BuyArtifact(int artifactIdx) // ��Ƽ��Ʈ ���� (�׼� ����)
    {
        if(artifactLists[artifactIdx].type == typeof(ArtifactTemplate))
        {
            ((ArtifactTemplate)artifactLists[artifactIdx].artifactBaseTemplate).Subscribe();
        }
        else if(artifactLists[artifactIdx].type == typeof(ArtifactTemplate<Character>))
        {
            ((ArtifactTemplate<Character>)artifactLists[artifactIdx].artifactBaseTemplate).Subscribe();
        }
        else if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate<PlayerMove>))
        {
            ((ArtifactTemplate<PlayerMove>)artifactLists[artifactIdx].artifactBaseTemplate).Subscribe();
        }
        else if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate<PlayerHP>))
        {
            ((ArtifactTemplate<PlayerHP>)artifactLists[artifactIdx].artifactBaseTemplate).Subscribe();
        }
    }

    public void SellArtifact(int artifactIdx) // ��Ƽ��Ʈ �Ǹ� (�׼� ���� ����)
    {
        if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate))
        {
            ((ArtifactTemplate)artifactLists[artifactIdx].artifactBaseTemplate).Unsubscribe();
        }
        else if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate<Character>))
        {
            ((ArtifactTemplate<Character>)artifactLists[artifactIdx].artifactBaseTemplate).Unsubscribe();
        }
        else if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate<PlayerMove>))
        {
            ((ArtifactTemplate<PlayerMove>)artifactLists[artifactIdx].artifactBaseTemplate).Unsubscribe();
        }
        else if (artifactLists[artifactIdx].type == typeof(ArtifactTemplate<PlayerHP>))
        {
            ((ArtifactTemplate<PlayerHP>)artifactLists[artifactIdx].artifactBaseTemplate).Unsubscribe();
        }
    }

    public void ApplyArtifact(int artifactIdx) // ��Ƽ��Ʈ ȿ�� ����
    {
        ((ArtifactTemplate)artifactLists[artifactIdx].artifactBaseTemplate).effect?.Invoke();
    }

    public void ApplyArtifact(int artifactIdx, Character character)
    {
        ((ArtifactTemplate<Character>)artifactLists[artifactIdx].artifactBaseTemplate).effect?.Invoke(character);
    }

    public void ApplyArtifact(int artifactIdx, PlayerMove playerMove)
    {
        ((ArtifactTemplate<PlayerMove>)artifactLists[artifactIdx].artifactBaseTemplate).effect?.Invoke(playerMove);
    }

    public void ApplyArtifact(int artifactIdx, PlayerHP playerHP)
    {
        ((ArtifactTemplate<PlayerHP>)artifactLists[artifactIdx].artifactBaseTemplate).effect?.Invoke(playerHP);
    }
}
