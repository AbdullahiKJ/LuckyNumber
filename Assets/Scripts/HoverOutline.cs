using UnityEngine;

public class HoverOutline : MonoBehaviour
{
    [SerializeField] private GameObject outlineVisual;

    private void Awake() => SetVisible(false);

    public void SetVisible(bool visible)
    {
        outlineVisual.SetActive(visible);
    }
}