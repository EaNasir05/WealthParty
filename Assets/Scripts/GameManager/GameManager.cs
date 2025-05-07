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
    private List<int[]> regionsUpgrades;
    private int[] activitiesIncomes;

    public GameManager()
    {
        if (instance == null)
        {
            instance = this;
            round = 1;
            currentPlayer = 0;
            nextTurnOrder = new List<int[]>();
            regionsUpgrades = new List<int[]>();
            activitiesIncomes = new int[2];
        }
    }

    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }
    public bool IsOperative() { return operative; }

    public void SetOperative(bool value) { operative = value; }
    public void SetCurrentPlayer(int value) { currentPlayer = value; }
    public void SetRound (int value) { round = value; }

    public void OnRoundStart()
    {
        ChangeTurnsOrder();
        SetCurrentPlayer(0);
    }

    public void OnRoundEnd()
    {
        foreach (int[] upgrade in regionsUpgrades)
        {
            RegionsManager.regions[upgrade[0]].ChangeLevel(upgrade[1]);
        }
    }

    public void OnTurnStart()
    {
        activitiesIncomes[0] = 0;
        activitiesIncomes[1] = 0;
        int[] temp = { currentPlayer, 0 };
        nextTurnOrder.Add(temp);
    }

    public void OnTurnEnd()
    {
        PlayersManager.players[currentPlayer].AddMoney(activitiesIncomes[0]);
        PlayersManager.players[currentPlayer].AddVotes(activitiesIncomes[1]);
    }

    public void UseRegion(int index)
    {
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        activitiesIncomes[0] += RegionsManager.regions[index].GetMoneyRate();
        activitiesIncomes[1] += RegionsManager.regions[index].GetVotesRate();
    }

    public void UpgradeRegion(int index, int value)
    {
        PlayersManager.players[currentPlayer].AddMoney(-1000);
        int region = -1;
        for (int i = 0; i < regionsUpgrades.Count; i++)
        {
            if (regionsUpgrades[i][0] == index)
            {
                region = i;
                break;
            }
        }
        if (region == -1)
        {
            int[] temp = { index, value };
            regionsUpgrades.Add(temp);
        }
        else
        {
            regionsUpgrades[region][1] += value;
        }
    }

    public void AddVotes(int value)
    {
        PlayersManager.players[currentPlayer].AddVotes(value);
        int index = nextTurnOrder.Count - 1;
        nextTurnOrder[index][1] += value;
    }

    public void AddMoney(int value)
    {
        PlayersManager.players[currentPlayer].AddMoney(value);
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
        nextTurnOrder?.Clear();
    }

    public void ResetRegionsUpgrades()
    {
        regionsUpgrades?.Clear();
    }

    private void ChangeTurnsOrder()
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
        ResetRegionsUpgrades();
    }

    public void ChangeTurn()
    {
        OnTurnEnd();
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
}
