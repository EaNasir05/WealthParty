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
    private List<int> usedActivities;
    private int usedActivity;

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
            usedActivities = new List<int>();
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
        ResetUsedActivities();
        foreach (Player player in PlayersManager.players)
        {
            if (player.GetMoney() < 2000)
            {
                player.AddMoney(2000 - player.GetMoney());
            }
        }
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
        usedActivity = -1;
    }

    public void OnTurnEnd()
    {
        AddMoney(activitiesIncomes[0]);
        AddVotes(activitiesIncomes[1]);
        if (usedActivity != -1)
        {
            usedActivities.Add(usedActivity);
        }
    }

    public void UseRegion(int index)
    {
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        activitiesIncomes[0] += RegionsManager.regions[index].GetMoneyRate();
        activitiesIncomes[1] += RegionsManager.regions[index].GetVotesRate();
        usedActivity = index;
    }

    public bool IsAnAvailableRegion(int index)
    {
        if (usedActivities.Contains(index))
        {
            return false;
        }
        return true;
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
        if (nextTurnOrder[currentPlayer] == null)
        {
            int[] temp = { currentPlayer, value };
            nextTurnOrder.Add(temp);
        }
        else
        {
            nextTurnOrder[currentPlayer][1] += value;
        }
    }

    public void AddMoney(int value)
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
                    (nextTurnOrder[y], nextTurnOrder[x]) = (nextTurnOrder[x], nextTurnOrder[y]);
                }
            }
        }
    }

    private void ResetNextTurnOrder()
    {
        nextTurnOrder?.Clear();
    }

    private void ResetRegionsUpgrades()
    {
        regionsUpgrades?.Clear();
    }

    private void ResetUsedActivities()
    {
        usedActivities?.Clear();
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
        if (PlayersManager.players[currentPlayer].GetVotes() >= 30000)
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
                SceneManager.LoadScene("RoundEnd");
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
