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
    [SerializeField] private Text turnOrderText;


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

    public void UpdateTurnUI(IReadOnlyList<string> turns)
    {
        turnOrderText.text = turns.Count == 0
        ? string.Empty
        : $"> {string.Join("\n", turns)}";
    }

}
