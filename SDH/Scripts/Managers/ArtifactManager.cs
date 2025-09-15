using System;
using System.Collections.Generic;

public partial class ArtifactManager // �ΰ��� ���� ����
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
    private int artifactCounts; // ������ ��Ƽ��Ʈ ��
    public List<(Type type, ArtifactBaseTemplate artifactBaseTemplate)> ArtifactLists => artifactLists;
    private List<(Type type, ArtifactBaseTemplate artifactBaseTemplate)> artifactLists; // ���� ����Ʈ�� Ÿ��

    public void StartGame() // ���� ����. ��Ʈ �Լ��� Stage��
    {
        artifactLists = new();
        artifactCounts = 0;

        foreach (Type artifact in typeof(ArtifactsList).GetNestedTypes(System.Reflection.BindingFlags.Public)) // ArtifactsList���� Public�ۿ� ������ NonPublic�� �� �ʿ� X
        {
            artifactLists.Add((artifact.BaseType, (ArtifactBaseTemplate)Activator.CreateInstance(artifact)));
        }
    }
}