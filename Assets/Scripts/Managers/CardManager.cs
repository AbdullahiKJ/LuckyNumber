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

                // Prepare the card from the shop list
                shopCards.Remove(card);

                // Update and close the shop ui
                gameManager.UI.UpdateShopCards();
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
        return true;
    }

    public void ResolveEffects(
        EffectTrigger trigger,
        EffectContext context)
    {
        // Resolve a snapshot so effects added during resolution begin participating on the next dispatch.
        foreach (ActiveCardEffect effect in activeEffects.ToArray())
        {
            if (effect.Definition.Trigger == trigger)
                effect.Definition.Resolve(context);
        }
    }
}