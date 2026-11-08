using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References
    private CombatManager combatManager;

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
        combatManager = GetComponent<CombatManager>();
        gameState = GameState.Active;
    }
    private void OnEnable()
    {
        combatManager.Player.Died += GameOver;
    }

    private void OnDisable()
    {
        combatManager.Player.Died -= GameOver;
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