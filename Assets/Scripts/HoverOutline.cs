using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        outline.enabled = visible;
    }
}