using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForcedBubble : MonoBehaviour
{
    [SerializeField]
    TextMeshPro text;

    public void Assign(ModifierData data)
    {
        text.text = data.ToString();
    }
}
