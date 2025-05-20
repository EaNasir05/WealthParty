using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class DrawnTask
{
    public int task;
    public bool completed;

    public DrawnTask(int task)
    {
        this.task = task;
        completed = false;
    }
}

public class ActivitiesState
{
    public Player player;
    public int duration;

    public ActivitiesState(Player player, int duration)
    {
        this.player = player;
        this.duration = duration;
    }
}

public class GameManager
{
    public static GameManager instance; //Unica istanza statica del GameManager, utilizzabile in tutti gli scripts
    private int round; //Identificativo dell'attuale round di gioco
    private int currentPlayer; //Indice del giocatore nella lista del "PlayersManager" che sta attualmente giocando il suo turno
    private bool operative; //Non ti interessa...
    private int winner; //Indice che indica il giocatore nella lista del "PlayersManager" che ha vinto la partita
    private List<int> worstRegions; //Lista delle 3 regioni meno produttive
    private List<DrawnTask> drawnTasks; //Lista delle task estratte nel round
    private List<int[]> nextTurnOrder; //Lista che contiene i giocatori che hanno giocato questo round e i rispettivi voti che hanno guadagnato in questo round
    private int[] regionsUpgrades; //Lista che contiene tutti gli investimenti di questo round
    private int activitiesIncomes; //Voti che il "currentPlayer" ha guadagnato tramite attività regionali
    private int tasksIncomes; //Denaro che il "currentPlayer" ha guadagnato tramite le tasks
    private List<List<ActivitiesState>> activitiesState; //Attività regionali che gli altri giocatori hanno già usato in questo round
    private int usedActivity; //Attività regionale che il "currentPlayer" ha usato nel suo turno
    private int[] upgradesOfThisTurn; //Quante volte il giocatore ha investito in una certa regione
    private bool lastRound; //Se è l'ultimo round è true

    public GameManager() //Istanzia il GameManager
    {
        if (instance == null)
        {
            instance = this;
            nextTurnOrder = new List<int[]>();
            drawnTasks = new List<DrawnTask>();
            worstRegions = new List<int>();
            regionsUpgrades = new int[6];
            activitiesState = new List<List<ActivitiesState>>();
            upgradesOfThisTurn = new int[6];
        }
        round = 1;
        lastRound = false;
        currentPlayer = 0;
        for (int i = 0; i < 6; i++)
        {
            activitiesState.Add(new List<ActivitiesState>());
        }
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public int GetRound() { return round; }
    public int GetCurrentPlayer() { return currentPlayer; }
    public int GetUsedActivity() { return usedActivity; }
    public int GetWinner() { return winner; }
    public List<DrawnTask> GetDrawnTasks() { return drawnTasks; }
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
            activitiesState.Add(new List<ActivitiesState>());
        }
    }

    public void OnRoundStart() //Insieme di metodi svolgere all'inizio di un round
    {
        ChangeTurnsOrder();
        drawnTasks?.Clear();
        if (round > 1)
        {
            UpdateWorstRegions();
        }
        currentPlayer = 0;
        foreach (Player player in PlayersManager.players)
        {
            player.AddMoney(3000);
        }
        for (int i = 0; i < 6; i++)
        {
            regionsUpgrades[i] = 0;
            for (int j = 0; j < activitiesState[i].Count; j++)
            {
                if (activitiesState[i][j].duration > 1)
                {
                    activitiesState[i][j].duration--;
                }
                else
                {
                    activitiesState[i].Remove(activitiesState[i][j]);
                    j--;
                }
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
        if (round > 1)
        {
            DrawNewTask();
        }
        activitiesIncomes = 0;
        tasksIncomes = 0;
        int[] temp = { currentPlayer, 0 };
        nextTurnOrder.Add(temp);
        usedActivity = -1;
        for (int i = 0; i < 6; i++)
        {
            upgradesOfThisTurn[i] = 0;
            for (int j = 0; j < activitiesState[i].Count; j++)
            {
                if (activitiesState[i][j].player == PlayersManager.players[currentPlayer])
                {
                    int[] production = RegionsManager.regions[i].GetCurrentVotesRate();
                    activitiesIncomes += Random.Range(production[0], production[1] + 1);
                    usedActivity = i;
                }
            }
        }
    }

    public void OnTurnEnd() //Insieme di metodi svolgere alla fine di un turno
    {
        AddVotes(activitiesIncomes);
        AddMoney(tasksIncomes);
    }

    private void UpdateWorstRegions() //Aggiorna "worstRegions"
    {
        worstRegions?.Clear();
        for (int i = 0; i < RegionsManager.regions.Count; i++)
        {
            if (worstRegions.Count < 3)
            {
                worstRegions.Add(i);
            }
            else
            {
                int best = GetTheBestWorstRegion();
                if (RegionsManager.regions[i].GetLevel() < RegionsManager.regions[worstRegions[best]].GetLevel())
                {
                    worstRegions[best] = i;
                }
            }
        }
    }

    private int GetTheBestWorstRegion() //Ritorna l'indice della regione più produttiva in "worstRegions"
    {
        int best = -1;
        for (int i = 0; i < worstRegions.Count; i++)
        {
            if (best == -1)
            {
                best = i;
            }
            else if (RegionsManager.regions[worstRegions[i]].GetLevel() > RegionsManager.regions[worstRegions[best]].GetLevel())
            {
                best = i;
            }
        }
        return best;
    }

    private void DrawNewTask() //Estrae una nuova task
    {
        List<int> drawableTasks = GetDrawableTasks();
        int drawn = Random.Range(0, drawableTasks.Count);
        drawnTasks.Add(new DrawnTask(drawableTasks[drawn]));
    }

    private List<int> GetDrawableTasks() //Sceglie quali task possono essere estratte
    {
        List<int> drawableTasks = new List<int>();
        for (int x = 0; x < 12; x++)
        {
            if (worstRegions.Contains(TasksManager.tasks[x].GetRegion()))
            {
                bool wasDrawn = false;
                for (int y = 0; y < drawnTasks.Count; y++)
                {
                    if (drawnTasks[y].task == x)
                    {
                        wasDrawn = true;
                    }
                }
                if (!wasDrawn)
                {
                    drawableTasks.Add(x);
                }
            }
        }
        return drawableTasks;
    }

    private int FindCompletedTask(int region, int action) //Controlla se è stata completata una task eseguendo una certa azione su una regione
    {
        for (int i = 0; i < drawnTasks.Count; i++)
        {
            if (TasksManager.tasks[drawnTasks[i].task].GetRegion() == region && TasksManager.tasks[drawnTasks[i].task].GetAction() == action && !drawnTasks[i].completed)
            {
                return i;
            }
        }
        return -1;
    }

    public void UseRegion(int index, int duration) //Attività regionale: vengono sottratti soldi al "currentPlayer", aggiunti voti ad "activitiesIncomes", ed eventualmente aggiunti soldi a "tasksIncomes"
    {
        int[] production = RegionsManager.regions[index].GetCurrentVotesRate();
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        activitiesIncomes += Random.Range(production[0], production[1] + 1);
        usedActivity = index;
        activitiesState[usedActivity].Add(new ActivitiesState(PlayersManager.players[currentPlayer], duration));
        int completedTask = FindCompletedTask(index, 0);
        if (completedTask != -1)
        {
            drawnTasks[completedTask].completed = true;
            tasksIncomes += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
            //Avvia animazione task completata
            Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
        }
    }

    public bool IsAnAvailableRegion(int index) //Ritorna true se un'attività regionale non è stata usata da altri players in questo round
    {
        if (activitiesState[index].Count > 0 && activitiesState[index][0].player != PlayersManager.players[currentPlayer])
        {
            return false;
        }
        if (usedActivity != -1 && usedActivity != index)
        {
            return false;
        }
        return true;
    }

    public Player GetPlayerOnRegion(int index) //Ritorna l'indice del player che sta usando una regione
    {
        if (activitiesState[index].Count == 0)
        {
            return null;
        }
        return activitiesState[index][0].player;
    }

    public void UpgradeRegion(int index, int value) //Investimento: sottrae soldi al "currentPlayer", aggiunge un investimento o un sabotaggio a "regionsUpgrades", ed eventualmente aggiunti soldi a "tasksIncomes"
    {
        PlayersManager.players[currentPlayer].AddMoney(-500);
        upgradesOfThisTurn[index] += value;
        regionsUpgrades[index] += value;
        if (value == 1)
        {
            int completedTask = FindCompletedTask(index, 1);
            if (completedTask != -1)
            {
                drawnTasks[completedTask].completed = true;
                tasksIncomes += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                //Avvia animazione task completata
                Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
            }
        }
        else if (value == -1)
        {
            int completedTask = FindCompletedTask(index, -1);
            if (completedTask != -1)
            {
                drawnTasks[completedTask].completed = true;
                tasksIncomes += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                //Avvia animazione task completata
                Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
            }
        }
    }

    public bool IsUpgradable(int region, int value) //Ritorna true se è possibile usare altri investimenti/sabotaggi
    {
        if (RegionsManager.regions[region].GetLevel() + upgradesOfThisTurn[region] + value > 4 || RegionsManager.regions[region].GetLevel() + upgradesOfThisTurn[region] + value < -4)
        {
            return false;
        }
        return true;
    }

    public bool GotAnyUpdates() //Ritorna true se il livello di produzione di almeno una regione è cambiato
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
        nextTurnOrder?.Clear();
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
            if (player.GetVotes() >= 15000)
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
