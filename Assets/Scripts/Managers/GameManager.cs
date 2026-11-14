using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
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


    [Header("Waves")]
    [SerializeField] private WaveDefinition[] waves;
    [SerializeField, Min(0f)] private float waveEndDelay = 2f;

    [Header("Scenes")]
    [SerializeField] private string menuScene = "Menu";

    enum GameState
    {
        Transitioning,
        Combat,
        GameOver,
        Victory
    }

    private GameState state = GameState.Transitioning;
    private int currentWave;
    private bool loadingScene;

    private void OnEnable()
    {
        combat.Player.Died += GameOver;
        combat.EnemiesDefeated += OnEnemiesDefeated;
    }

    private void OnDisable()
    {
        combat.Player.Died -= GameOver;
        combat.EnemiesDefeated -= OnEnemiesDefeated;
    }

    private void Start()
    {
        ui.HideResults();

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("Assign at least one wave.", this);
            return;
        }

        currentWave = 0;
        StartCurrentWave();
    }

    public bool IsGameActive()
    {
        return state == GameState.Combat;
    }

    private void StartCurrentWave()
    {
        combat.SpawnWave(waves[currentWave]);

        ui.RebuildEnemyUI(combat.Enemies);
        ui.UpdateWaveUI(currentWave + 1, waves.Length);

        turns.BuildRoundQueue();

        state = GameState.Combat;
        turns.CheckNextTurn();
    }

    private void OnEnemiesDefeated()
    {
        if (!IsGameActive())
            return;

        state = GameState.Transitioning;
        StopCombatFlow();

        StartCoroutine(AdvanceWave());
    }

    private IEnumerator AdvanceWave()
    {
        // Leave time for the final enemy's death presentation.
        yield return new WaitForSeconds(waveEndDelay);

        if (currentWave == waves.Length - 1)
        {
            state = GameState.Victory;
            ui.ShowVictory();
            yield break;
        }

        currentWave++;
        StartCurrentWave();
    }

    private void GameOver()
    {
        if (!IsGameActive())
            return;

        state = GameState.GameOver;

        StopAllCoroutines();
        StopCombatFlow();

        ui.ShowGameOver();
    }

    private void StopCombatFlow()
    {
        turns.StopCombat();
        combat.StopActions();
        ui.SetCombatInputEnabled(false);
        ui.CloseCards();
    }

    public void Retry()
    {
        if (loadingScene)
            return;

        loadingScene = true;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        if (loadingScene)
            return;

        loadingScene = true;
        SceneManager.LoadSceneAsync(menuScene);
    }
}