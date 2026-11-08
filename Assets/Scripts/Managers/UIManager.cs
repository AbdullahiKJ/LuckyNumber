using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // References
    private CombatManager combatManager;
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

    private void Awake()
    {
        combatManager = GetComponent<CombatManager>();
        cam = Camera.main;

        foreach (Enemy enemy in combatManager.Enemies)
        {
            if (enemy == null)
                continue;

            EnemyHealthBar bar = Instantiate(enemyHealthBarPrefab, enemyHealthBarLayer);
            bar.Bind(enemy, cam, enemyHealthBarLayer);
        }
    }

    private void OnEnable()
    {
        combatManager.Player.HealthChanged += UpdatePlayerHealthUI;
    }

    private void OnDisable()
    {
        combatManager.Player.HealthChanged -= UpdatePlayerHealthUI;
    }

    public void UpdateDiceUI()
    { }

    public void UpdateBoardUI()
    { }

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
}
