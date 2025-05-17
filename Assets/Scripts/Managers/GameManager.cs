using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static GameManager instance; //Unica istanza statica del GameManager, utilizzabile in tutti gli scripts
    private int round; //Identificativo dell'attuale round di gioco
    private int currentPlayer; //Indice del giocatore nella lista del "PlayersManager" che sta attualmente giocando il suo turno
    private bool operative; //Non ti interessa...
    private int winner; //Indice che indica il giocatore nella lista del "PlayersManager" che ha vinto la partita
    private List<int[]> nextTurnOrder; //Lista che contiene i giocatori che hanno giocato questo round e i rispettivi voti che hanno guadagnato in questo round
    private int[] regionsUpgrades; //Lista che contiene tutti gli investimenti di questo round
    private int activitiesIncomes; //Voti e denaro che il "currentPlayer" ha guadagnato tramite attività regionali
    private int[,] activitiesState; //Attività regionali che gli altri giocatori hanno già usato in questo round
    private int activityDuration; //Quanti turni il giocatore decide di occupare una regione
    private int usedActivity; //Attività regionale che il "currentPlayer" ha usato nel suo turno
    private int[] upgradesOfThisTurn; //Quante volte il giocatore ha investito in una certa regione
    private bool lastRound; //Se è l'ultimo round è true

    public GameManager() //Istanzia il GameManager
    {
        if (instance == null)
        {
            instance = this;
            round = 1;
            lastRound = false;
            currentPlayer = 0;
            nextTurnOrder = new List<int[]>();
            regionsUpgrades = new int[6];
            activitiesState = new int[6, 2];
            upgradesOfThisTurn = new int[6];
        }
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }
    public int GetWinner() { return winner; }
    public bool IsLastRound() { return lastRound; }
    public bool IsOperative() { return operative; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetOperative(bool value) { operative = value; }
    public void SetCurrentPlayer(int value) { currentPlayer = value; }
    public void SetRound (int value) { round = value; }

    public void OnGameStart()
    {
        for (int i = 0; i < 6; i++)
        {
            activitiesState[i, 0] = -1;
            activitiesState[i, 1] = 0;
        }
    }

    public void OnRoundStart() //Insieme di metodi svolgere all'inizio di un round
    {
        ChangeTurnsOrder();
        SetCurrentPlayer(0);
        foreach (Player player in PlayersManager.players)
        {
            player.AddMoney(3000);
        }
        for (int i = 0; i < 6; i++)
        {
            regionsUpgrades[i] = 0;
            if (activitiesState[i, 1] > 1)
            {
                activitiesState[i, 1]--;
            }
            else
            {
                activitiesState[i, 0] = -1;
                activitiesState[i, 1] = 0;
            }
        }
    }

    public List<Dictionary<string, int>> OnRoundEnd() //Insieme di metodi da svolgere al termine di un round
    {
        List<Dictionary<string, int>> list = new();
        for (int i = 0; i < regionsUpgrades.Length; i++)
        {
            int[] rate = RegionsManager.regions[i].GetCurrentVotesRate();
            Dictionary<string, int> dic = new()
            {
                { "region", i },
                { "oldVotesRate1", rate[0] },
                { "oldVotesRate2", rate[1] }
            };
            RegionsManager.regions[i].ChangeLevel(regionsUpgrades[i]);
            list.Add(dic);
        }
        return list;
    }

    public void OnTurnStart() //Insieme di metodi da svolgere all'inizio di un turno
    {
        activitiesIncomes = 0;
        int[] temp = { currentPlayer, 0 };
        nextTurnOrder.Add(temp);
        usedActivity = -1;
        for (int i = 0; i < 6; i++)
        {
            upgradesOfThisTurn[i] = 0;
            if (activitiesState[i, 0] == currentPlayer)
            {
                int[] production = RegionsManager.regions[i].GetCurrentVotesRate();
                activitiesIncomes += Random.Range(production[0], production[1] + 1);
                usedActivity = i;
            }
        }
    }

    public void OnTurnEnd() //Insieme di metodi svolgere alla fine di un turno
    {
        AddVotes(activitiesIncomes);
        if (usedActivity != -1)
        {
            activitiesState[usedActivity, 0] = currentPlayer;
            activitiesState[usedActivity, 1] = activityDuration;
        }
    }

    public void UseRegion(int index, int duration)  //Attività regionale: vengono sottratti soldi al "currentPlayer" e aggiunte risorse ad "activitiesIncomes"
    {
        int[] production = RegionsManager.regions[index].GetCurrentVotesRate();
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        activitiesIncomes += Random.Range(production[0], production[1] + 1);
        usedActivity = index;
        activityDuration = duration;
    }

    public bool IsAnAvailableRegion(int index) //Ritorna true se un'attività regionale non è stata usata da altri players in questo round
    {
        if (activitiesState[index, 0] != -1)
        {
            return false;
        }
        return true;
    }

    public int GetPlayerOnRegion(int index)
    {
        return activitiesState[index, 0];
    }

    public void UpgradeRegion(int index, int value) //Investimento: sottrae soldi al "currentPlayer" e aggiunge un investimento a "regionsUpgrades"
    {
        PlayersManager.players[currentPlayer].AddMoney(-500);
        upgradesOfThisTurn[index] += value;
        regionsUpgrades[index] += value;
    }

    public bool IsUpgradable(int region, int value)
    {
        if (RegionsManager.regions[region].GetLevel() + upgradesOfThisTurn[region] + value > 5 || RegionsManager.regions[region].GetLevel() + upgradesOfThisTurn[region] + value < -5)
        {
            return false;
        }
        return true;
    }

    public bool GotAnyUpdates()
    {
        int count = 0;
        for (int i = 0; i < regionsUpgrades.Length; i++)
        {
            if (regionsUpgrades[i] == 0)
            {
                count++;
            }
        }
        if (regionsUpgrades.Length - count == 0)
        {
            return false;
        }
        return true;
    }

    public void AddVotes(int value) //Modifica la quantità di voti del "currentPlayer", e tiene il conto dei voti guadagnati in questo turno per calcolare il prossimo ordine dei turni
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

    public void AddMoney(int value) //Modifica la quantità di monete del "currentPlayer"
    {
        PlayersManager.players[currentPlayer].AddMoney(value);
    }

    private void SortNextTurnOrder() //Modifica l'ordine dei turni del prossimo round in base al guadagno di voti dei giocatori (utilizzato in ChangeTurnsOrder)
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

    private void ResetNextTurnOrder() //Svuota "nextTurnOrder"
    {
        nextTurnOrder?.Clear();
    }

    private void ChangeTurnsOrder() //Cambia l'ordine dei turni del prossimo round
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

    public void ChangeTurn() //Fine del turno del "currentPlayer": o inizia un altro turno, o finisce il round, o finisce la partita
    {
        OnTurnEnd();
        SortNextTurnOrder();
        if (currentPlayer + 1 == PlayersManager.players.Count)
        {
            if (lastRound)
            {
                FindTheWinner();
                SceneManager.LoadScene("Victory");
            }
            else
            {
                round++;
                if (SomeoneIsWinning())
                {
                    lastRound = true;
                }
                operative = true;
                SceneManager.LoadScene("RoundEnd");
            }
        }
        else
        {
            currentPlayer++;
            operative = true;
            SceneManager.LoadScene("TurnStart");
        }
    }

    private bool SomeoneIsWinning() //Controlla se qualcuno ha raggiunto 30000 voti
    {
        foreach (Player player in PlayersManager.players)
        {
            if (player.GetVotes() >= 30000)
            {
                return true;
            }
        }
        return false;
    }

    private void FindTheWinner() //Trova il vincitore
    {
        int player = 0;
        for (int i = 1; i < 4; i++)
        {
            if (PlayersManager.players[player].GetVotes() < PlayersManager.players[i].GetVotes())
            {
                player = i;
            }
            else if (PlayersManager.players[player].GetVotes() == PlayersManager.players[i].GetVotes())
            {
                if (PlayersManager.players[player].GetMoney() < PlayersManager.players[i].GetMoney())
                {
                    player = i;
                }
            }
        }
        winner = player;
    }
}
