using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Verb
{
    Eats,
    Chases,
    Avoids,
    Floats = 20,
    Heavy,
    Speedy,
    None
}

public enum Noun
{
    None,
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

    public bool Complete => (int)Verb < 20 ? Noun != Noun.None : true;
    public override string ToString()
    {
        return $"{(Verb == Verb.None ? string.Empty : Verb)}{((Noun != Noun.None && Verb != Verb.None) ? " " : string.Empty)}{(Noun == Noun.None ? string.Empty : Noun)}";
    }
}
