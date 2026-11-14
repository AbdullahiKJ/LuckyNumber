using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image artwork;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    [SerializeField] private Transform costGroup;
    [SerializeField] private Image costDiePrefab;
    [SerializeField] private Sprite[] diceFaces = new Sprite[6];
    public GameManager Game;

    private CardDefinition selectedCard;

    public void Show(CardDefinition card)
    {
        selectedCard = card;

        artwork.sprite = card.image;
        nameText.text = card.cardName;
        descriptionText.text = card.details;

        RefreshAcquireButton();
        UpdateCostUI(card.cost);

        gameObject.SetActive(true);
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