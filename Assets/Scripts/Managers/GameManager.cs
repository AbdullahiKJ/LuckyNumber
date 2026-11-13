using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References
    [SerializeField] private CardManager cards;
    [SerializeField] private CombatManager combat;
    [SerializeField] private DiceManager dice;
    [SerializeField] private TurnManager turns;
    [SerializeField] private UIManager ui;
    public CardManager Cards => cards;
    public CombatManager Combat => combat;
    public DiceManager Dice => dice;
    public TurnManager Turns => turns;
    public UIManager UI => ui;

    private int currentLevel;
    [SerializeField] int maxLevel;
    private GameState gameState;
    enum GameState
    {
        Active,
        Paused,
        GameOver
    }

    private void Awake()
    {
        if (cards == null)
            cards = GetComponent<CardManager>();

        if (combat == null)
            combat = GetComponent<CombatManager>();

        if (dice == null)
            dice = GetComponent<DiceManager>();

        if (turns == null)
            turns = GetComponent<TurnManager>();

        if (ui == null)
            ui = GetComponent<UIManager>();

        gameState = GameState.Active;
    }
    private void OnEnable()
    {
        combat.Player.Died += GameOver;
    }

    private void OnDisable()
    {
        combat.Player.Died -= GameOver;
    }

    private void StartGame()
    {
    }

    private void NextLevel()
    {
    }

    public bool IsGameActive()
    {
        return gameState == GameState.Active;
    }

    private void GameOver()
    {
    }

    public void WinGame()
    {
    }
}