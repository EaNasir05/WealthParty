using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int round;
    private int currentPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            round = 1;
            currentPlayer = 0;
        }
    }

    void Update()
    {
        
    }

    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }

    public void ChangeTurnsOrder()
    {
        if (round == 1)
        {
            PlayersManager.ShufflePlayers();
        }
        else
        {
            //Ordina in base al guadagno
        }
    }
}
