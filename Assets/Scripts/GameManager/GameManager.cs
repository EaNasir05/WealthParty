using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static GameManager instance;
    private int round;
    private int currentPlayer;
    private bool operative;
    private int winner;
    private List<int> pointsPerRound;
    private List<int> nextTurnOrder;

    public GameManager()
    {
        if (instance == null)
        {
            instance = this;
            round = 1;
            currentPlayer = 0;
            nextTurnOrder = new List<int>();
        }
    }

    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }
    public bool IsOperative() { return operative; }

    public void SetOperative(bool value) { operative = value; }
    public void SetCurrentPlayer(int value) { currentPlayer = value; }
    public void SetRound (int value) { round = value; }

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

    public void ChangeTurn()
    {
        if (PlayersManager.players[currentPlayer].GetVictoryPoints() == 5)
        {
            winner = currentPlayer;
            SceneManager.LoadScene("Victory");
        }
        else
        {
            if (currentPlayer + 1 == PlayersManager.players.Count)
            {
                round++;
                operative = true;
                SceneManager.LoadScene("RoundStart");
            }
            else
            {
                currentPlayer++;
                operative = true;
                SceneManager.LoadScene("TurnStart");
            }
        }
    }

    public void AddPoints(int index, int value)
    {
        switch (index)
        {
            case 0:
                PlayersManager.players[currentPlayer].AddAbruzzoPoints(value);
                break;
            case 1:
                PlayersManager.players[currentPlayer].AddCampaniaPoints(value);
                break;
            case 2:
                PlayersManager.players[currentPlayer].AddPugliaPoints(value);
                break;
            case 3:
                PlayersManager.players[currentPlayer].AddBasilicataPoints(value);
                break;
            case 4:
                PlayersManager.players[currentPlayer].AddCalabriaPoints(value);
                break;
            case 5:
                PlayersManager.players[currentPlayer].AddSiciliaPoints(value);
                break;
            default:
                Debug.Log("RISORSA INESISTENTE");
                break;
        }
        pointsPerRound[currentPlayer] = value;
        SortNextTurnOrder();
    }

    private void SortNextTurnOrder()
    {
        //ORDINA LA PROSSIMA LISTA DEI TURNI
    }

    public void ResetNextTurnOrder()
    {
        nextTurnOrder.Clear();
        pointsPerRound.Clear();
    }
}
