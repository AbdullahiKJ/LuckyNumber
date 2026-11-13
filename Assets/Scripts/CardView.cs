using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private GameObject chargeBadge;
    [SerializeField] private TextMeshProUGUI chargeText;

    public CardDefinition Definition => definition;

    private CardDefinition definition;
    private Action<CardDefinition> onSelected;

    public void Bind(
        CardDefinition card,
        Action<CardDefinition> selectionCallback)
    {
        definition = card;
        onSelected = selectionCallback;

        artwork.sprite = card.image;
        summaryText.text = card.summary;

        SetChargeLabel(string.Empty);
    }

    public void SetChargeLabel(string label)
    {
        chargeBadge.SetActive(!string.IsNullOrEmpty(label));
        chargeText.text = label;
    }

    public void OnClick()
    {
        onSelected?.Invoke(definition);
    }
}