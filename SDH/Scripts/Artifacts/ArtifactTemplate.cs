using System;
using UnityEngine;

public abstract class ArtifactBaseTemplate
{
    public Sprite icon;
    public string title;
    public string explain;
    public bool isPurchased = false;
    public bool isAvailable = false;
}

public abstract class ArtifactTemplate : ArtifactBaseTemplate
{
    public Action effect;
    public virtual void Subscribe() // ���� ȿ�� ����
    {
        if (isPurchased)
        {
            Debug.Log("������ ��Ƽ��Ʈ �� ���ŵ�");
            return;
        }
        isPurchased = true;
        Managers.Artifact.ArtifactCounts++;
        effect += Effect;
    }

    public virtual void Unsubscribe() // ���� ȿ�� ����
    {
        Managers.Artifact.ArtifactCounts--;
        effect = null;
    }

    public virtual void Effect()
    {
        if (!isAvailable) return;
    }
}

public class ArtifactTemplate<T> : ArtifactBaseTemplate
{
    public Action<T> effect;
    public virtual void Subscribe()
    {
        if (isPurchased)
        {
            Debug.Log("������ ��Ƽ��Ʈ �� ���ŵ�");
            return;
        }
        isPurchased = true;
        Managers.Artifact.ArtifactCounts++;
        effect += Effect;
    }

    public virtual void Unsubscribe()
    {
        Managers.Artifact.ArtifactCounts--;
        effect = null;
    }

    public virtual void Effect(T data)
    {
        if (!isAvailable) return;
    }
}