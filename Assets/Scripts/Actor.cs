using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public static List<Actor> actors;

    [Header("Typing")]
    [SerializeField]
    private Noun type;

    public Noun Type => type;

    [Header("Sprites")]
    public SpriteRenderer SpriteRenderer;
    public Sprite BaseSprite;
    public Sprite DeadSprite;

    [Header("Barks")]
    public SpriteRenderer BarkBubble;
    public TextMeshPro BarkText;

    [Header("Physics")]
    public Rigidbody2D Rigidbody;

    [Header("Params")]
    public float Speed = 2;
    public float RotationSpeed = 45;
    public float AvoidDistance;
    public float EatDistance;
    public Vector3 BubbleDelta;

    public List<ModifierData> Modifiers;

    public List<Noun> Eats;
    bool floats = false;

    public Noun ChaseTarget;
    public Noun AvoidTarget;

    private void Awake()
    {
        Eats = new List<Noun>();
    }

    private void Update()
    {
        Eats.Clear();
        floats = false;
        ChaseTarget = Noun.None;
        AvoidTarget = Noun.None;

        foreach (ModifierData modifier in Modifiers)
        {
            UpdateWithModifier(modifier);
        }

        Rigidbody.gravityScale = floats ? 0 : 1;
    }

    private void LateUpdate()
    {
        if (actors == null)
        {
            actors = new List<Actor>();
        }

        if (ChaseTarget != Noun.None)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                Actor other = actors[i];
                if (other != this && other.Type == ChaseTarget)
                {
                    float distanceX = other.transform.position.x - this.transform.position.x;
                    if (Mathf.Abs(distanceX) > 0.1f)
                    {
                        float direction = distanceX / Mathf.Abs(distanceX);
                        transform.Translate(new Vector3(Speed * Time.deltaTime * direction, 0, 0));
                        SpriteRenderer.transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime * -direction));
                    }
                }
            }
        }

        if (AvoidTarget != Noun.None)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                Actor other = actors[i];
                if (other != this && other.Type == AvoidTarget)
                {
                    float distanceX = other.transform.position.x - this.transform.position.x;
                    if (Mathf.Abs(distanceX) < AvoidDistance)
                    {
                        float direction = distanceX / Mathf.Abs(distanceX);
                        transform.Translate(new Vector3(Speed * Time.deltaTime * -direction, 0, 0));
                        SpriteRenderer.transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime * direction));
                    }
                }
            }
        }

        for (int i = 0;i < actors.Count;i++)
        {
            Actor other = actors[i];
            if (other != this)
            {
                if (Eats.Contains(other.type))
                {
                    float distance = (other.transform.position - this.transform.position).magnitude;
                    if(distance < EatDistance)
                    {
                        OnEat(other.type);
                        Destroy(other.gameObject);
                    }
                }
            }
        }
    }

    public void Modify(ModifierBubble modifier)
    {
        Modifiers.Add(modifier.ModifierData);

        modifier.transform.parent = transform;
        modifier.transform.localPosition = BubbleDelta * Modifiers.Count;
    }

    private void OnEat(Noun eaten)
    {
        // barks
        if(BarksManager.Instance != null)
        {
            foreach (var item in BarksManager.Instance.OnEatHappened)
            {
                if(item.Noun == type && item.Target == eaten)
                {
                    StartCoroutine(DisplayBarkWrapper(item.bark));
                }
            }
        }
    }

    Coroutine currentFade = null;
    private IEnumerator DisplayBarkWrapper(string text)
    {
        while (currentFade != null)
        {
            yield return null;
        }

        currentFade = StartCoroutine(DisplayBark(text));
    }

    private IEnumerator DisplayBark(string text)
    {
        // set data
        BarkText.text = text;

        // fade in
        BarkText.FadeIn(0.2f);
        BarkBubble.FadeIn(0.2f);

        // wait
        yield return new WaitForSeconds(0.5f);

        // fade out
        BarkText.FadeOut(0.2f);
        BarkBubble.FadeOut(0.2f);

        currentFade = null;
    }

    private void OnEnable()
    {
        if(actors ==  null)
        {
            actors = new List<Actor>();
        }
        if (!actors.Contains(this))
        {
            actors.Add(this);
        }
    }

    private void OnDisable()
    {
        actors.Remove(this);
    }

    private void UpdateWithModifier(ModifierData modifier)
    {
        switch (modifier.Verb)
        {
            case Verb.Eats:
                Eats.Add(modifier.Noun);
                break;
            case Verb.Chases:
                ChaseTarget = modifier.Noun;
                break;
            case Verb.Avoids:
                AvoidTarget = modifier.Noun;
                break;
            case Verb.Floats:
                floats = true;
                break;
            case Verb.Heavy:
                break;
            default:
                break;
        }
    }
}

public static class FadeExtensions
{
    public static IEnumerator FadeIn(this SpriteRenderer spriteRenderer, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);

            if (spriteRenderer != null)
            {
                Color spriteColor = spriteRenderer.color;
                spriteColor.a = alpha;
                spriteRenderer.color = spriteColor;
            }

            yield return null;
        }
    }

    public static IEnumerator FadeOut(this SpriteRenderer spriteRenderer, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration));

            if (spriteRenderer != null)
            {
                Color spriteColor = spriteRenderer.color;
                spriteColor.a = alpha;
                spriteRenderer.color = spriteColor;
            }

            yield return null;
        }
    }

    public static IEnumerator FadeIn(this TextMeshPro textMeshPro, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);

            if (textMeshPro != null)
            {
                Color textColor = textMeshPro.color;
                textColor.a = alpha;
                textMeshPro.color = textColor;
            }

            yield return null;
        }
    }

    public static IEnumerator FadeOut(this TextMeshPro textMeshPro, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration));

            if (textMeshPro != null)
            {
                Color textColor = textMeshPro.color;
                textColor.a = alpha;
                textMeshPro.color = textColor;
            }

            yield return null;
        }
    }
}

