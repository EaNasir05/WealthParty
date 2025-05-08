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
    private List<int[]> regionsUpgrades; //Lista che contiene tutti gli investimenti di questo round
    private int[] activitiesIncomes; //Voti e denaro che il "currentPlayer" ha guadagnato tramite attività regionali
    private List<int> usedActivities; //Attività regionali che gli altri giocatori hanno già usato in questo round
    private int usedActivity; //Attività regionale che il "currentPlayer" ha usato nel suo turno

    public GameManager() //Istanzia il GameManager
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

    //Metodi usati per ottenere da altri script valori di attributi privati
    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }
    public bool IsOperative() { return operative; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetOperative(bool value) { operative = value; }
    public void SetCurrentPlayer(int value) { currentPlayer = value; }
    public void SetRound (int value) { round = value; }

    public void OnRoundStart() //Insieme di metodi svolgere all'inizio di un round
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

    public void OnRoundEnd() //Insieme di metodi svolgere al termine di un round
    {
        foreach (int[] upgrade in regionsUpgrades)
        {
            RegionsManager.regions[upgrade[0]].ChangeLevel(upgrade[1]);
        }
    }

    public void OnTurnStart() //Insieme di metodi svolgere all'inizio di un turno
    {
        activitiesIncomes[0] = 0;
        activitiesIncomes[1] = 0;
        int[] temp = { currentPlayer, 0 };
        nextTurnOrder.Add(temp);
        usedActivity = -1;
    }

    public void OnTurnEnd() //Insieme di metodi svolgere alla fine di un turno
    {
        AddMoney(activitiesIncomes[0]);
        AddVotes(activitiesIncomes[1]);
        if (usedActivity != -1)
        {
            usedActivities.Add(usedActivity);
        }
    }

    public void UseRegion(int index)  //Attività regionale: vengono sottratti soldi al "currentPlayer" e aggiunte risorse ad "activitiesIncomes"
    {
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        activitiesIncomes[0] += RegionsManager.regions[index].GetMoneyRate();
        activitiesIncomes[1] += RegionsManager.regions[index].GetVotesRate();
        usedActivity = index;
    }

    public bool IsAnAvailableRegion(int index) //Ritorna true se un'attività regionale non è stata usata da altri players in questo round
    {
        if (usedActivities.Contains(index))
        {
            return false;
        }
        return true;
    }

    public void UpgradeRegion(int index, int value) //Investimento: sottrae soldi al "currentPlayer" e aggiunge un investimento a "regionsUpgrades"
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

    private void ResetRegionsUpgrades() //Svuota "regionsUpgrades"
    {
        regionsUpgrades?.Clear();
    }

    private void ResetUsedActivities() //Svuota "usedActivities"
    {
        usedActivities?.Clear();
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
        ResetRegionsUpgrades();
    }

    public void ChangeTurn() //Fine del turno del "currentPlayer": o inizia un altro turno, o finisce il round, o finisce la partita
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
