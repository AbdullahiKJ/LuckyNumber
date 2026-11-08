using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float heightAboveEnemy = 2f;

    private Enemy enemy;
    private Camera gameplayCamera;
    private RectTransform layer;

    public void Bind(Enemy target, Camera camera, RectTransform parent)
    {
        enemy = target;
        gameplayCamera = camera;
        layer = parent;
        enemy.HealthChanged += UpdateHealth;
    }

    private void Start()
    {
        // The enemy's Awake has initialized its health by this point.
        UpdateHealth(enemy.CurrentHealth, enemy.MaxHealth);
    }

    private void LateUpdate()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 worldPosition = enemy.transform.position + Vector3.up * heightAboveEnemy;
        Vector3 screenPosition = gameplayCamera.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            layer, screenPosition, null, out Vector2 localPosition);

        transform.localPosition = localPosition;
    }

    private void UpdateHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
        label.text = $"{current:0.#} / {max:0.#}";

        if (current <= 0f)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (enemy != null)
            enemy.HealthChanged -= UpdateHealth;
    }
}