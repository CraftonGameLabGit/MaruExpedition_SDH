public partial class ArtifactManager // 아티팩트 구매와 효과 적용 함수만 관리하는 코드
{
    public void BuyArtifact(int artifactIdx) // 아티팩트 구매 (액션 구독)
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

    public void SellArtifact(int artifactIdx) // 아티팩트 판매 (액션 구독 해제)
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

    public void ApplyArtifact(int artifactIdx) // 아티팩트 효과 적용
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
