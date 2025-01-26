using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BarksManager : ScriptableObject
{
    public static BarksManager Instance;

    public void Load()
    {
        Instance = this;
    }

    public TargetedBark[] OnEatAssigned;
    public TargetedBark[] OnEatHappened;

    public TargetedBark[] OnChaseAssigned;
    public TargetedBark[] OnChaseApproach;

    public TargetedBark[] OnAvoidAssigned;
    public TargetedBark[] OnAvoidHappened;

    public Bark[] Idle;

    public Bark[] FloatsAssigned;
}

[Serializable]
public struct TargetedBark
{
    public Noun noun;
    public Noun Target;
    public string bark;
}

[Serializable]
public struct Bark
{
    public Noun noun;
    public string bark;
}
