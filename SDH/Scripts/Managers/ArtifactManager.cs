using System;
using System.Collections.Generic;

public partial class ArtifactManager // 인게임 유물 관리
{
    public int ArtifactCounts
    {
        get
        {
            return artifactCounts;
        }
        set
        {
            artifactCounts = value;
        }
    }
    private int artifactCounts; // 구매한 아티팩트 수
    public List<(Type type, ArtifactBaseTemplate artifactBaseTemplate)> ArtifactLists => artifactLists;
    private List<(Type type, ArtifactBaseTemplate artifactBaseTemplate)> artifactLists; // 유물 리스트와 타입

    public void StartGame() // 게임 시작. 루트 함수는 Stage임
    {
        artifactLists = new();
        artifactCounts = 0;

        foreach (Type artifact in typeof(ArtifactsList).GetNestedTypes(System.Reflection.BindingFlags.Public)) // ArtifactsList에는 Public밖에 없으니 NonPublic는 볼 필요 X
        {
            artifactLists.Add((artifact.BaseType, (ArtifactBaseTemplate)Activator.CreateInstance(artifact)));
        }
    }
}