 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    public ModifierData ModifierData;

    [Header("Components")]
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    new Collider2D collider;
    [SerializeField]
    TextMeshPro text;

    public Vector3 startPosition;

    bool attached = false;
    private Actor attachedTo;
    public bool Attached => attached;
    public bool Complete => ModifierData.Complete;

    // Start is called before the first frame update
    void Start()
    {
        text.text = ModifierData.ToString();
        ModifierData = Instantiate(ModifierData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteBubble(ModifierBubble other)
    {
        ModifierData.Noun = other.ModifierData.Noun;
        text.text = ModifierData.ToString();
        attachedTo.ModifyComplete(this);
        Destroy(other.gameObject);
    }

    public void Attach(Actor AttachedTo)
    {
        attached = true;
        attachedTo = AttachedTo;
        collider.enabled = true;
    }

    public void Pop()
    {
        attachedTo.Modifiers.Remove(ModifierData);
        Destroy(gameObject);
    }

    public void PickUp()
    {
        startPosition = transform.position;
    }

    public void DropReset()
    {
        transform.position = startPosition;
        collider.enabled = true;
    }
}
