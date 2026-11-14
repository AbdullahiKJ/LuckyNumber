using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject luckyNumberPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private string gameplayScene = "Gameplay";

    private int selectedNumber;
    private bool loading;

    private void Start()
    {
        AudioManager.Instance?.PlayBGMusic();
        startPanel.SetActive(true);
        luckyNumberPanel.SetActive(false);
        confirmButton.interactable = false;
    }

    public void Play()
    {
        startPanel.SetActive(false);
        luckyNumberPanel.SetActive(true);
    }

    public void SelectLuckyNumber(int number)
    {
        if (number < 1 || number > 6)
            return;

        selectedNumber = number;
        confirmButton.interactable = true;
    }

    public void ConfirmLuckyNumber()
    {
        if (loading || selectedNumber == 0)
            return;

        loading = true;
        confirmButton.interactable = false;

        GameSession.Instance.SetLuckyNumber(selectedNumber);
        SceneManager.LoadSceneAsync(gameplayScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}