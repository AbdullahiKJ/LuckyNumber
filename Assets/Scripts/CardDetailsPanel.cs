using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;
    public GameManager Game;

    private CardDefinition selectedCard;

    public void Show(CardDefinition card)
    {
        selectedCard = card;

        artwork.sprite = card.image;
        nameText.text = card.cardName;
        descriptionText.text = card.details;

        RefreshAcquireButton();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void AcquireSelectedCard()
    {
        if (selectedCard == null)
            return;

        if (Game.Cards.AcquireCard(selectedCard))
            Close();
        else
            RefreshAcquireButton();
    }

    private void RefreshAcquireButton()
    {
        button.interactable = Game.Cards.CanAcquireCard(selectedCard);
    }
}