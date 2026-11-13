using System.Collections.Generic;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Transform iconGroup;
    [SerializeField] private StatusIconView iconPrefab;

    private StatusController controller;
    private readonly List<StatusIconView> icons = new();

    public void Bind(StatusController target)
    {
        if (controller != null)
            controller.StatusesChanged -= Refresh;

        controller = target;

        if (controller != null)
            controller.StatusesChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        foreach (StatusIconView icon in icons)
        {
            icon.gameObject.SetActive(false);
            Destroy(icon.gameObject);
        }

        icons.Clear();

        if (controller == null)
            return;

        foreach (var (definition, turns) in controller.GetStatuses())
        {
            StatusIconView icon = Instantiate(iconPrefab, iconGroup);
            icon.Bind(definition, turns);
            icons.Add(icon);
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.StatusesChanged -= Refresh;
    }
}