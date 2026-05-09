using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card_Sho : MonoBehaviour
{
    public enum ItemEffectType { SpeedUp, SpeedDown, ScoreUp, ScoreSteal }
    public enum ItemTargetType { Self, Opponent, All, AllExceptSelf }

    public ItemEffectType effectType;
    public ItemTargetType targetType;
    public float effectValue;
    public float speedEffectDuration = 5f;

    private Vector2 startPos;
    public float rotateSpeed = 100f;
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2f;

    void Start()
    {
        startPos = transform.position;
        effectType = GetRandomEffect();
        targetType = GetRandomTarget();
        SetEffectValue();
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
            Destroy(gameObject);
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
            case ItemEffectType.SpeedDown:
                target.ApplySpeedEffect(value, speedEffectDuration);
                break;
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
        // ここは任意で重み付け処理に変更可能
        return (ItemEffectType)Random.Range(0, 4);
    }

    ItemTargetType GetRandomTarget()
    {
        return (ItemTargetType)Random.Range(0, 4);
    }

    void SetEffectValue()
    {
        switch (effectType)
        {
            case ItemEffectType.SpeedUp: effectValue = 2f; break;
            case ItemEffectType.SpeedDown: effectValue = -2f; break;
            case ItemEffectType.ScoreUp: effectValue = 10f; break;
            case ItemEffectType.ScoreSteal: effectValue = 5f; break;
        }
    }
}
