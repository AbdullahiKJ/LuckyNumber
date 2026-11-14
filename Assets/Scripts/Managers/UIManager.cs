using System.Collections.Generic;
using FGUIStarter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // References
    [SerializeField] private GameManager gameManager;
    private Camera cam;

    [Header("Health UI")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private EnemyHealthBar enemyHealthBarPrefab;
    [SerializeField] private RectTransform enemyHealthBarLayer;

    [Header("Turn UI")]
    [SerializeField] private GameObject playerTurnPrefab;
    [SerializeField] private GameObject enemyTurnPrefab;
    [SerializeField] private RectTransform turnBackground;
    private readonly List<GameObject> turnRows = new();

    [Header("Roll UI")]
    [SerializeField] private TextMeshProUGUI rollText;
    [SerializeField] private CustomButton rollButton;
    private float rollsRemaining;

    [Header("Cards UI")]
    [SerializeField] private GameObject ownedCards;
    [SerializeField] private GameObject shopCards;
    [SerializeField] private GameObject cardButton;
    [SerializeField] private Button ownedCardsButton;
    [SerializeField] private Button shopCardsButton;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform ownedCardsGroup;
    [SerializeField] private Transform shopCardsGroup;
    [SerializeField] private CardDetailsPanel cardDetailsPanel;

    private readonly List<CardView> ownedCardViews = new();

    [Header("Results")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Wave")]
    [SerializeField] private TextMeshProUGUI waveText;

    private readonly List<EnemyHealthBar> enemyBars = new();

    [Header("Lucky Roll")]
    [SerializeField] private GameObject luckyRollPanel;
    [SerializeField] private TextMeshProUGUI luckyRollText;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();

        cam = Camera.main;

        // Disable cards and the card button on start
        CloseCards();

        // Create the shop card ui
        cardDetailsPanel.Game = gameManager;
        UpdateShopCards();
    }

    private void OnEnable()
    {
        gameManager.Combat.Player.HealthChanged += UpdatePlayerHealthUI;
        gameManager.Dice.RollStarted += DisableRollButton;
        gameManager.Dice.RollFinished += EnableRollButton;
        gameManager.Dice.LuckyRoll += ShowLuckyRoll;
        gameManager.Turns.PlayerTurnStarted += EnableCardButtons;
        gameManager.Turns.PlayerTurnEnded += DisableCardButtons;
        gameManager.Cards.CardsChanged += RefreshOwnedCardCharges;
    }

    private void OnDisable()
    {
        gameManager.Combat.Player.HealthChanged -= UpdatePlayerHealthUI;
        gameManager.Dice.RollStarted -= DisableRollButton;
        gameManager.Dice.RollFinished -= EnableRollButton;
        gameManager.Dice.LuckyRoll -= ShowLuckyRoll;
        gameManager.Turns.PlayerTurnStarted -= EnableCardButtons;
        gameManager.Turns.PlayerTurnEnded -= DisableCardButtons;
        gameManager.Cards.CardsChanged -= RefreshOwnedCardCharges;

        CancelInvoke(nameof(HideLuckyRoll));
        HideLuckyRoll();
    }

    public void UpdateRollsUI(int rolls)
    {
        rollsRemaining = rolls;
        if (rollText != null)
            rollText.text = $"{rolls}";
    }

    private void DisableRollButton()
    {
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }
    }

    public void EnableRollButton()
    {
        SetCombatInputEnabled(
            gameManager.IsGameActive() &&
            gameManager.Turns.IsPlayerTurn);
    }

    private void DisableCardButtons()
    {
        ownedCardsButton.interactable = false;
        shopCardsButton.interactable = false;
    }

    private void EnableCardButtons()
    {
        ownedCardsButton.interactable = true;
        shopCardsButton.interactable = true;
    }

    public void CloseCards()
    {
        ownedCards.SetActive(false);
        shopCards.SetActive(false);
        cardButton.SetActive(false);
    }

    public void UpdateShopCards()
    {
        // Destroy all children of the shop cards group
        foreach (Transform child in shopCardsGroup.transform)
            Destroy(child.gameObject);

        // Re-instantiate the children
        foreach (CardDefinition definition in gameManager.Cards.ShopCards)
        {
            CardView view = Instantiate(cardPrefab, shopCardsGroup).GetComponent<CardView>();
            view.Bind(definition, cardDetailsPanel.Show);
        }
    }

    public void AddOwnedCard(CardDefinition card)
    {
        CardView view = Instantiate(cardPrefab, ownedCardsGroup).GetComponent<CardView>();
        view.Bind(card, cardDetailsPanel.Show);
        view.SetChargeLabel(gameManager.Cards.GetChargeLabel(card));

        ownedCardViews.Add(view);
        CloseCards();
    }

    private void RefreshOwnedCardCharges()
    {
        for (int i = ownedCardViews.Count - 1; i >= 0; i--)
        {
            CardView view = ownedCardViews[i];
            string label = gameManager.Cards.GetChargeLabel(view.Definition);

            if (string.IsNullOrEmpty(label))
            {
                ownedCardViews.RemoveAt(i);
                view.gameObject.SetActive(false);
                Destroy(view.gameObject);
            }
            else
            {
                view.SetChargeLabel(label);
            }
        }
    }

    private void ShowLuckyRoll(float healed)
    {
        CancelInvoke(nameof(HideLuckyRoll));

        luckyRollText.text = healed > 0f
            ? $"Lucky roll! +{healed:0.#} HP"
            : "Lucky roll! Health already full";

        luckyRollPanel.SetActive(true);
        Invoke(nameof(HideLuckyRoll), 2f);
    }

    private void HideLuckyRoll()
    {
        luckyRollPanel.SetActive(false);
    }
    private void UpdatePlayerHealthUI(float current, float max)
    {
        SetHealthUI(playerHealthSlider, playerHealthText, current, max);
    }

    private void SetHealthUI(Slider slider, TextMeshProUGUI label, float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
        label.text = $"{current:0.#} / {max:0.#}";
    }

    public void UpdateTurnUI(IReadOnlyList<(bool isPlayer, string label)> turns)
    {
        foreach (GameObject row in turnRows)
        {
            row.SetActive(false);
            Destroy(row);
        }

        turnRows.Clear();

        for (int i = 0; i < turns.Count; i++)
        {
            var (isPlayer, label) = turns[i];
            GameObject prefab = isPlayer ? playerTurnPrefab : enemyTurnPrefab;
            GameObject row = Instantiate(prefab, turnBackground, false);

            TextMeshProUGUI rowText = row.GetComponentInChildren<TextMeshProUGUI>();
            rowText.text = i == 0 ? $"> {label}" : label;

            turnRows.Add(row);
        }
    }

    public void RebuildEnemyUI(IReadOnlyList<Enemy> enemies)
    {
        foreach (EnemyHealthBar bar in enemyBars)
        {
            if (bar == null)
                continue;

            bar.gameObject.SetActive(false);
            Destroy(bar.gameObject);
        }

        enemyBars.Clear();

        foreach (Enemy enemy in enemies)
        {
            EnemyHealthBar bar = Instantiate(
                enemyHealthBarPrefab, enemyHealthBarLayer);

            bar.Bind(enemy, cam, enemyHealthBarLayer);
            bar.GetComponent<StatusUI>().Bind(
                enemy.GetComponent<StatusController>());

            enemyBars.Add(bar);
        }
    }

    public void UpdateWaveUI(int current, int total)
    {
        waveText.text = $"Wave {current} / {total}";
    }

    public void HideResults()
    {
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        HideResults();
        victoryPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        HideResults();
        gameOverPanel.SetActive(true);
    }

    public void SetCombatInputEnabled(bool enabled)
    {
        rollButton.interactable = enabled &&
            rollsRemaining > 0 && !gameManager.Dice.IsRolling;

        ownedCardsButton.interactable = enabled;
        shopCardsButton.interactable = enabled;
    }
}
