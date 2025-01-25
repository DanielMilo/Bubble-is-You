using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Verb
{
    Eats,
    Chases,
    Floats
}

public enum Noun
{
    Cat,
    Fish,
    Cabbage,
    Sheep,
    Wolf
}

[CreateAssetMenu()]
public class ModifierData : ScriptableObject
{
    public Verb Verb;
    public Noun Noun;
}
