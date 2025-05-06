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
    private List<int[]> nextTurnOrder;

    public GameManager()
    {
        if (instance == null)
        {
            instance = this;
            round = 1;
            currentPlayer = 0;
            nextTurnOrder = new List<int[]>();
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
            PlayersManager.ChangePlayersOrder(nextTurnOrder[0][0], nextTurnOrder[1][0], nextTurnOrder[2][0], nextTurnOrder[3][0]);
        }
        ResetNextTurnOrder();
    }

    public void ChangeTurn()
    {
        if (PlayersManager.players[currentPlayer].GetVotes() == 400)
        {
            winner = currentPlayer;
            SceneManager.LoadScene("Victory");
        }
        else
        {
            SortNextTurnOrder();
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

    public void OnTurnStart()
    {
        int[] temp = { currentPlayer, 0 };
        nextTurnOrder.Add(temp);
    }

    public void AddVotes(/*int index, */int value)
    {
        PlayersManager.players[currentPlayer].AddVotes(value);
        /*switch (index)
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
        }*/
        int index = nextTurnOrder.Count - 1;
        nextTurnOrder[index][0] = currentPlayer;
        nextTurnOrder[index][1] = value;
    }

    private void ChangeMoneyCount(int value)
    {
        PlayersManager.players[currentPlayer].AddMoney(value);
    }

    private void SortNextTurnOrder()
    {
        for (int x = 0; x < nextTurnOrder.Count; x++)
        {
            for(int y = x; y < nextTurnOrder.Count; y++)
            {
                if (nextTurnOrder[x][1] > nextTurnOrder[y][1])
                {
                    int[] temp = nextTurnOrder[x];
                    nextTurnOrder[x] = nextTurnOrder[y];
                    nextTurnOrder[y] = temp;
                }
            }
        }
    }

    public void ResetNextTurnOrder()
    {
        if (nextTurnOrder != null)
        {
            nextTurnOrder.Clear();
        }
    }
}
