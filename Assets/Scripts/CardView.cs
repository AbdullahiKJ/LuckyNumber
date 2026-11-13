using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI summaryText;

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
    }

    public void OnClick()
    {
        onSelected?.Invoke(definition);
    }
}