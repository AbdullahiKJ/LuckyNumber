using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CombatPoint : MonoBehaviour
{
    [SerializeField] Camera gameplayCamera;

    private TurnManager turns;
    private DiceManager dice;
    private CombatManager combat;
    private GameManager game;
    private HoverOutline hovered;

    private void Awake()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        turns = GetComponent<TurnManager>();
        dice = GetComponent<DiceManager>();
        combat = GetComponent<CombatManager>();
        game = GetComponent<GameManager>();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !game.IsGameActive() || !turns.IsPlayerTurn ||
            !combat.Player.IsAlive || dice.IsRolling ||
            (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            SetHovered(null);
            return;
        }

        Ray ray = gameplayCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            SetHovered(null);
            return;
        }

        Dice die = hit.collider.GetComponentInParent<Dice>();
        if (dice.CanHold(die))
        {
            SetHovered(die.GetComponent<HoverOutline>());
            if (mouse.leftButton.wasPressedThisFrame)
                dice.ToggleDieSelection(die);

            return;
        }

        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy != null && enemy.isActiveAndEnabled && enemy.IsAlive)
        {
            SetHovered(enemy.GetComponent<HoverOutline>());

            int damage = dice.GetFullRollTotal();
            if (mouse.leftButton.wasPressedThisFrame && damage > 0)
                combat.PlayerAttack(damage, enemy);

            return;
        }

        SetHovered(null);
    }

    private void OnDisable()
    {
        SetHovered(null);
    }

    private void SetHovered(HoverOutline next)
    {
        if (hovered == next)
            return;

        if (hovered != null)
            hovered.SetVisible(false);

        hovered = next;

        if (hovered != null)
            hovered.SetVisible(true);
    }
}
