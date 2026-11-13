using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI turnsText;

    public void Bind(StatusDefinition definition, int turns)
    {
        icon.sprite = definition.icon;
        turnsText.text = turns.ToString();
    }
}