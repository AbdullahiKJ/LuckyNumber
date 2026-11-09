using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image faceRenderer;
    [SerializeField] private Button button;
    [SerializeField] private Sprite[] faces = new Sprite[6];
    [SerializeField] private int fastFrames = 30;
    [SerializeField] private float minSlowTime = 0.06f;
    [SerializeField] private float maxSlowTime = 0.20f;
    [SerializeField] private Color heldTint = Color.goldenRod;


    [Header("Managers")]
    public GameManager GameManager;
    public DiceManager DiceManager;
    public TurnManager TurnManager;

    public int Result { get; private set; }
    public bool IsHeld { get; private set; }
    public event Action<Dice, int> RollCompleted;

    private bool isRolling;

    public void OnClick()
    {
        if (GameManager.IsGameActive() && TurnManager.IsPlayerTurn)
        {
            DiceManager.ToggleDieSelection(this);
        }
    }

    public void SetHeld(bool held)
    {
        IsHeld = held;
        ColorBlock block = button.colors;
        block.normalColor = held ? heldTint : Color.white;
        button.colors = block;
    }

    public void Roll()
    {
        if (isRolling || IsHeld)
            return;

        isRolling = true;
        StartCoroutine(AnimateRoll());
    }

    private IEnumerator AnimateRoll()
    {
        int finalFace = UnityEngine.Random.Range(1, 7);
        int previewFace = Result > 0 ? Result - 1 : 0;
        int fastSwaps = UnityEngine.Random.Range(9, 15);
        const int slowSwaps = 5;

        for (int i = 0; i < fastSwaps + slowSwaps; i++)
        {
            previewFace = (previewFace + UnityEngine.Random.Range(1, 6)) % 6;
            faceRenderer.sprite = faces[previewFace];

            float delay = i < fastSwaps
                ? 1f / fastFrames
                : Mathf.Lerp(minSlowTime, maxSlowTime, (i - fastSwaps) / (slowSwaps - 1f));

            yield return new WaitForSeconds(delay);
        }

        faceRenderer.sprite = faces[finalFace - 1];
        Result = finalFace;
        isRolling = false;
        RollCompleted?.Invoke(this, Result);
    }
}