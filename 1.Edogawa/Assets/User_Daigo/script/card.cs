using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public enum ItemEffectType { SpeedUp, SpeedDown, ScoreUp, ScoreSteal }
    public enum ItemTargetType { Self, Opponent, All, AllExceptSelf }

    public ItemEffectType effectType;
    public ItemTargetType targetType;
    public float effectValue;

    public Sprite speedUpIcon;
    public Sprite speedDownIcon;
    public Sprite scoreUpIcon;
    public Sprite scoreStealIcon;

    private SpriteRenderer spriteRenderer;

    public AudioClip pickupSE;   // 取得時の効果音をInspectorで設定
    private AudioSource audioSource;

    [Header("効果確率")]
    public int weightSpeedUp = 20;
    public int weightSpeedDown = 10;
    public int weightScoreUp = 40;
    public int weightScoreSteal = 30;

    [Header("ターゲット確率")]
    public int weightSelf = 50;
    public int weightOpponent = 25;
    public int weightAll = 15;
    public int weightAllExceptSelf = 10;

    [Header("効果時間")]
    public float speedEffectDuration = 5f;


    private Vector2 startPos;
    public float rotateSpeed = 100f;
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2f;

    void Start()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // ランダムに効果・ターゲットを決定
        effectType = GetRandomEffect();
        targetType = GetRandomTarget();
        SetEffectValue();

        // ★ 生成された瞬間にアイコンを決定しておく
        ApplyIconByEffect(effectType);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector2(startPos.x, newY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICharacter character = collision.GetComponent<ICharacter>();
        if (character != null)
        {
            ApplyEffect(character);
            SEManager.Instance.PlaySE(pickupSE);
            Destroy(gameObject, pickupSE.length);
        }
    }

    void ApplyEffect(ICharacter character)
    {
        if (effectType == ItemEffectType.ScoreSteal)
        {
            ICharacter target = GetRandomTargetCharacter(character);
            if (target != null)
                ApplyToCharacter(target, effectType, effectValue, character);
            return;
        }

        switch (targetType)
        {
            case ItemTargetType.Self:
                ApplyToCharacter(character, effectType, effectValue);
                break;
            case ItemTargetType.Opponent:
                ICharacter opponent = GetRandomTargetCharacter(character);
                if (opponent != null)
                    ApplyToCharacter(opponent, effectType, effectValue, character);
                break;
            case ItemTargetType.All:
                foreach (ICharacter c in GetAllCharacters())
                    ApplyToCharacter(c, effectType, effectValue, character);
                break;
            case ItemTargetType.AllExceptSelf:
                foreach (ICharacter c in GetAllCharacters())
                    if (c != character)
                        ApplyToCharacter(c, effectType, effectValue, character);
                break;
        }
    }

    void ApplyToCharacter(ICharacter target, ItemEffectType type, float value, ICharacter owner = null)
    {
        switch (type)
        {
            case ItemEffectType.ScoreUp:
                target.ApplyScoreEffect(value);
                break;
            case ItemEffectType.ScoreSteal:
                if (owner != null && target.Score > 0)
                {
                    int stealAmount = Mathf.Min((int)value, target.Score);
                    target.Score -= stealAmount;
                    owner.Score += stealAmount;
                    Debug.Log($"{owner.Name} が {target.Name} から {stealAmount} 点奪った");
                }
                break;
            case ItemEffectType.SpeedUp:
                target.ApplySpeedEffect(value, speedEffectDuration);
                break;
            case ItemEffectType.SpeedDown:
                target.ApplySpeedEffect(value, speedEffectDuration);
                break;
        }
    }

    void ApplyIconByEffect(ItemEffectType type)
    {
        switch (type)
        {
            case ItemEffectType.SpeedUp: spriteRenderer.sprite = speedUpIcon; break;
            case ItemEffectType.SpeedDown: spriteRenderer.sprite = speedDownIcon; break;
            case ItemEffectType.ScoreUp: spriteRenderer.sprite = scoreUpIcon; break;
            case ItemEffectType.ScoreSteal: spriteRenderer.sprite = scoreStealIcon; break;
        }
    }

    List<ICharacter> GetAllCharacters()
    {
        List<ICharacter> all = new List<ICharacter>();
        all.Add(PlayerManagers.Instance.GetPlayer());
        all.AddRange(PlayerManagers.Instance.GetAllNPCs());
        return all;
    }

    ICharacter GetRandomTargetCharacter(ICharacter exclude)
    {
        List<ICharacter> candidates = GetAllCharacters();
        candidates.Remove(exclude);
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    ItemEffectType GetRandomEffect()
    {
        int total = weightSpeedUp + weightSpeedDown + weightScoreUp + weightScoreSteal;
        int rand = Random.Range(0, total);
        int cumulative = 0;

        if ((cumulative += weightSpeedUp) > rand) return ItemEffectType.SpeedUp;
        if ((cumulative += weightSpeedDown) > rand) return ItemEffectType.SpeedDown;
        if ((cumulative += weightScoreUp) > rand) return ItemEffectType.ScoreUp;
        return ItemEffectType.ScoreSteal;
    }

    ItemTargetType GetRandomTarget()
    {
        int total = weightSelf + weightOpponent + weightAll + weightAllExceptSelf;
        int rand = Random.Range(0, total);
        int cumulative = 0;

        if ((cumulative += weightSelf) > rand) return ItemTargetType.Self;
        if ((cumulative += weightOpponent) > rand) return ItemTargetType.Opponent;
        if ((cumulative += weightAll) > rand) return ItemTargetType.All;
        return ItemTargetType.AllExceptSelf;
    }

    void SetEffectValue()
    {
        switch (effectType)
        {
            case ItemEffectType.SpeedUp: effectValue = 2f; break;
            case ItemEffectType.SpeedDown: effectValue = -2f; break;
            case ItemEffectType.ScoreUp: effectValue = 10; break;
            case ItemEffectType.ScoreSteal: effectValue = 5; break;
        }
    }
}
