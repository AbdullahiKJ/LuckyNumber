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
    [SerializeField] private Transform costGroup;
    [SerializeField] private Image costDiePrefab;
    [SerializeField] private Sprite[] diceFaces = new Sprite[6];

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
        UpdateCostUI(card.cost);
    }

    private void UpdateCostUI(int[] cost)
    {
        foreach (Transform child in costGroup)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        foreach (int value in cost)
        {
            Image die = Instantiate(costDiePrefab, costGroup);
            die.sprite = diceFaces[value - 1];
        }
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