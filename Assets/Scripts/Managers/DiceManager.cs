using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allDice;
    private GameObject[] activeDice;

    public void RollDice()
    {
        foreach (GameObject die in activeDice)
        {
            die.GetComponent<Dice>().Roll();
        }
    }

    private void ToggleDieSelection()
    {

    }
    private void GetSelectedTotal()
    {

    }

    private void GetFullRollTotal()
    {

    }
}