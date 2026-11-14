using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] List<CardDefinition> shopCards = new();

    private readonly List<CardDefinition> ownedCards = new();
    private readonly List<ActiveCardEffect> activeEffects = new();

    public IReadOnlyList<CardDefinition> OwnedCards => ownedCards;
    public IReadOnlyList<CardDefinition> ShopCards => shopCards;
    public event Action CardsChanged;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();
    }

    public bool CanAcquireCard(CardDefinition card)
    {
        if (card == null || ownedCards.Contains(card) ||
            !gameManager.IsGameActive() ||
            !gameManager.Turns.IsPlayerTurn ||
            gameManager.Dice.IsRolling)
            return false;

        var availableValues = new List<int>();

        foreach (Dice die in gameManager.Dice.RolledDice)
        {
            if (die != null && die.isActiveAndEnabled)
                availableValues.Add(die.Result);
        }

        foreach (int requiredValue in card.cost)
        {
            // Each requirement uses one matching die.
            if (!availableValues.Remove(requiredValue))
                return false;
        }

        return true;
    }

    public bool AcquireCard(CardDefinition card)
    {
        if (card == null || ownedCards.Contains(card))
            return false;

        var context = new EffectContext(gameManager.Combat.Player, gameManager.Combat.Enemies);
        bool hasOngoingEffects = false;

        foreach (EffectDefinition effect in card.effects)
        {
            if (effect == null)
                continue;

            // Resolve instant use effects
            if (effect.Trigger == EffectTrigger.OnAcquire)
            {
                effect.Resolve(context);

                // Do not remove the card from the shop list
                // shopCards.Remove(card);

                // Update and close the shop ui
                // gameManager.UI.UpdateShopCards();
                gameManager.UI.CloseCards();
            }
            // Otherwise, add them to the list of effects
            else
            {
                activeEffects.Add(new ActiveCardEffect(card, effect));
                hasOngoingEffects = true;
            }
        }

        // Add persistent cards to the owned list and update the ui
        if (hasOngoingEffects)
        {
            ownedCards.Add(card);
            gameManager.UI.AddOwnedCard(card);
        }

        CardsChanged?.Invoke();
        AudioManager.Instance?.PlayAcquire();
        return true;
    }

    private void ConsumeEffectCharge(ActiveCardEffect effect)
    {
        effect.ConsumeCharge();

        if (!effect.IsExpired)
            return;

        activeEffects.Remove(effect);

        bool cardStillHasEffects = activeEffects.Exists(
            other => other.SourceCard == effect.SourceCard);

        if (!cardStillHasEffects)
            ownedCards.Remove(effect.SourceCard);
    }

    public void ResolveEffects(
       EffectTrigger trigger,
       EffectContext context)
    {
        bool anyApplied = false;

        foreach (ActiveCardEffect effect in activeEffects.ToArray())
        {
            if (!activeEffects.Contains(effect) || effect.IsExpired ||
                effect.Definition.Trigger != trigger)
                continue;

            if (!effect.Definition.Resolve(context))
                continue;

            ConsumeEffectCharge(effect);
            anyApplied = true;
        }

        if (anyApplied)
            CardsChanged?.Invoke();
    }

    public bool ResolveDamageEffects(
        EffectTrigger trigger,
        DamageContext context,
        bool stopAfterFirst = false)
    {
        bool anyApplied = false;

        foreach (ActiveCardEffect effect in activeEffects.ToArray())
        {
            // Ignore if the trigger does not match, effect is expired or not in the list
            if (!activeEffects.Contains(effect) || effect.IsExpired ||
                effect.Definition.Trigger != trigger)
                continue;

            // Ignore if the resolve damage fails
            if (!effect.Definition.TryResolveDamage(context))
                continue;

            ConsumeEffectCharge(effect);
            anyApplied = true;

            if (stopAfterFirst)
                break;
        }

        if (anyApplied)
            CardsChanged?.Invoke();

        return anyApplied;
    }

    public string GetChargeLabel(CardDefinition card)
    {
        var labels = new List<string>();

        foreach (ActiveCardEffect effect in activeEffects)
        {
            if (effect.SourceCard != card)
                continue;

            labels.Add(effect.Definition.UnlimitedUses
                ? "∞"
                : effect.RemainingCharges.ToString());
        }

        return string.Join(" / ", labels);
    }
}