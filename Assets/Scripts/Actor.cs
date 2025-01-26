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

    [Header("modifiers")]
    public List<ModifierData> Modifiers;
    public ForcedBubble prefab;

    public List<Noun> Eats;
    bool floats = false;

    public Noun ChaseTarget;
    public Noun AvoidTarget;

    private void Awake()
    {
        Eats = new List<Noun>();

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ForcedBubble bubble = Instantiate(prefab, transform);
            bubble.transform.localPosition = BubbleDelta * (i+1);
            bubble.Assign(Modifiers[i]);
        }
    }

    private void Update()
    {
        Eats.Clear();
        floats = false;
        ChaseTarget = Noun.None;
        AvoidTarget = Noun.None;

        foreach (ModifierData modifier in Modifiers)
        {
            if (modifier.Complete)
            {
                UpdateWithModifier(modifier);
            }
        }

        Rigidbody.gravityScale = floats ? 0 : 1;
    }

    private void LateUpdate()
    {
        if (actors == null)
        {
            actors = new List<Actor>();
        }

        bool moved = false;
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
                        moved = true;
                        break;
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
                        moved = true;
                        break;
                    }
                }
            }
        }

        if(!moved)
        {
            SpriteRenderer.transform.rotation = SmoothDamp(SpriteRenderer.transform.rotation, Quaternion.identity, 0.5f);
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

    Quaternion SmoothDamp(Quaternion current, Quaternion target, float smoothTime)
    {
        // Ensure there is no division by zero
        if (smoothTime < Mathf.Epsilon) smoothTime = Mathf.Epsilon;

        // Calculate the step for this frame
        float t = 1.0f - Mathf.Exp(-Time.deltaTime / smoothTime);

        // Smoothly interpolate the rotation
        return Quaternion.Slerp(current, target, t);
    }

    public void Modify(ModifierBubble modifier)
    {
        Modifiers.Add(modifier.ModifierData);

        modifier.transform.parent = transform;
        modifier.transform.localPosition = BubbleDelta * Modifiers.Count;
        if (BarksManager.Instance != null)
        {
            if (modifier.ModifierData.Verb == Verb.Floats)
            {
                foreach (var item in BarksManager.Instance.FloatsAssigned)
                {
                    if (item.noun == type)
                    {
                        StartCoroutine(DisplayBarkWrapper(item.bark));
                        break;
                    }
                }
            }
        }
    }

    public void ModifyComplete(ModifierBubble modifier)
    {
        TargetedBark[] targetedBarks = null;

        switch (modifier.ModifierData.Verb)
        {
            case Verb.Eats:
                targetedBarks = BarksManager.Instance.OnEatAssigned;
                break;
            case Verb.Chases:
                targetedBarks = BarksManager.Instance.OnChaseAssigned;
                break;
            case Verb.Avoids:
                targetedBarks = BarksManager.Instance.OnAvoidAssigned;
                break;
            case Verb.None:
                break;
            default:
                break;
        }

        if (targetedBarks != null)
        {
            foreach (var item in targetedBarks)
            {
                if (item.Noun == type && item.Target == modifier.ModifierData.Noun)
                {
                    StartCoroutine(DisplayBarkWrapper(item.bark));
                    break;
                }
            }
        }
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
        BarkText.StartFadeIn(0.3f);
        BarkBubble.StartFadeIn(this, 0.3f);

        // wait
        yield return new WaitForSeconds(2.6f);

        // fade out
        BarkText.StartFadeOut(0.3f);
        BarkBubble.StartFadeOut(this, 0.3f);

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
    public static void StartFadeIn(this SpriteRenderer spriteRenderer, MonoBehaviour monoBehaviour, float duration)
    {
        monoBehaviour.StartCoroutine(FadeInCoroutine(spriteRenderer, duration));
    }

    private static IEnumerator FadeInCoroutine(SpriteRenderer spriteRenderer, float duration)
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

    public static void StartFadeOut(this SpriteRenderer spriteRenderer, MonoBehaviour monoBehaviour, float duration)
    {
        monoBehaviour.StartCoroutine(FadeOutCoroutine(spriteRenderer, duration));
    }

    private static IEnumerator FadeOutCoroutine(SpriteRenderer spriteRenderer, float duration)
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

    public static void StartFadeIn(this TextMeshPro textMeshPro, float duration)
    {
        textMeshPro.StartCoroutine(FadeInCoroutine(textMeshPro, duration));
    }

    private static IEnumerator FadeInCoroutine(TextMeshPro textMeshPro, float duration)
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

    public static void StartFadeOut(this TextMeshPro textMeshPro, float duration)
    {
        textMeshPro.StartCoroutine(FadeOutCoroutine(textMeshPro, duration));
    }

    private static IEnumerator FadeOutCoroutine(TextMeshPro textMeshPro, float duration)
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

    public static void StartFadeInOutCycle(this SpriteRenderer spriteRenderer, MonoBehaviour monoBehaviour, float fadeDuration, float delayBetweenFades)
    {
        monoBehaviour.StartCoroutine(FadeInOutCycleCoroutine(spriteRenderer, fadeDuration, delayBetweenFades));
    }

    private static IEnumerator FadeInOutCycleCoroutine(SpriteRenderer spriteRenderer, float fadeDuration, float delayBetweenFades)
    {
        while (true)
        {
            yield return FadeInCoroutine(spriteRenderer, fadeDuration);
            yield return new WaitForSeconds(delayBetweenFades);
            yield return FadeOutCoroutine(spriteRenderer, fadeDuration);
            yield return new WaitForSeconds(delayBetweenFades);
        }
    }

    public static void StartFadeInOutCycle(this TextMeshPro textMeshPro, MonoBehaviour monoBehaviour, float fadeDuration, float delayBetweenFades)
    {
        monoBehaviour.StartCoroutine(FadeInOutCycleCoroutine(textMeshPro, fadeDuration, delayBetweenFades));
    }

    private static IEnumerator FadeInOutCycleCoroutine(TextMeshPro textMeshPro, float fadeDuration, float delayBetweenFades)
    {
        while (true)
        {
            yield return FadeInCoroutine(textMeshPro, fadeDuration);
            yield return new WaitForSeconds(delayBetweenFades);
            yield return FadeOutCoroutine(textMeshPro, fadeDuration);
            yield return new WaitForSeconds(delayBetweenFades);
        }
    }
}


