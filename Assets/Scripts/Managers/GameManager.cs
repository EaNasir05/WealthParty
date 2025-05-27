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
    private int votesToObtain; //Voti da ottenere per far partire l'ultimo round
    private int winner; //Indice che indica il giocatore nella lista del "PlayersManager" che ha vinto la partita
    private List<int> worstRegions; //Lista delle 3 regioni meno produttive
    private List<int> bestRegions; //Lista delle 3 regioni più produttive
    private List<DrawnTask> drawnTasks; //Lista delle task estratte nel round
    private List<int[]> nextTurnOrder; //Lista che contiene i giocatori che hanno giocato questo round e i rispettivi voti che hanno guadagnato in questo round
    private int[] regionsUpgrades; //Lista che contiene tutti gli investimenti di questo round
    private int activitiesIncomes; //Voti che il "currentPlayer" ha guadagnato tramite attività regionali
    private int tasksIncomes; //Denaro che il "currentPlayer" ha guadagnato tramite le tasks
    private List<List<ActivitiesState>> activitiesState; //Attività regionali che gli altri giocatori hanno già usato in questo round
    private int usedActivity; //Attività regionale che il "currentPlayer" ha usato nel suo turno
    private int[] upgradesOfThisTurn; //Quante volte il giocatore ha investito in una certa regione
    private bool lastRound; //Se è l'ultimo round è true
    private Dictionary<Player, List<int>> playersIncomes;

    public GameManager() //Istanzia il GameManager
    {
        if (instance == null)
        {
            instance = this;
        }
        nextTurnOrder = new List<int[]>();
        drawnTasks = new List<DrawnTask>();
        worstRegions = new List<int>();
        bestRegions = new List<int>();
        regionsUpgrades = new int[6];
        activitiesState = new List<List<ActivitiesState>>();
        playersIncomes = new Dictionary<Player, List<int>>();
        foreach (Player player in PlayersManager.players)
        {
            playersIncomes[player] = new List<int>();
            playersIncomes[player].Add(0);
            playersIncomes[player].Add(0);
        }
        upgradesOfThisTurn = new int[6];
        votesToObtain = 15000;
        round = 0;
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
    public int GetVotesToObtain() { return votesToObtain; }
    public int GetUsedActivity() { return usedActivity; }
    public int GetWinner() { return winner; }
    public List<DrawnTask> GetDrawnTasks() { return drawnTasks; }
    public bool IsLastRound() { return lastRound; }
    public bool IsOperative() { return operative; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetVotesToObtain(int value) { votesToObtain = value; }
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
        round++;
        ChangeTurnsOrder();
        drawnTasks?.Clear();
        if (round > 1)
        {
            UpdateWorstRegions();
            UpdateBestRegions();
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
                    int votes = Random.Range(production[0], production[1] + 1);
                    AddVotes(votes);
                    activitiesIncomes += votes;
                    usedActivity = i;
                }
            }
        }
        playersIncomes[PlayersManager.players[currentPlayer]][0] = activitiesIncomes;
    }

    public void OnTurnEnd() //Insieme di metodi svolgere alla fine di un turno
    {
        AddMoney(tasksIncomes);
        if (nextTurnOrder[currentPlayer] == null)
        {
            int[] temp = { currentPlayer, activitiesIncomes };
            nextTurnOrder.Add(temp);
        }
        else
        {
            nextTurnOrder[currentPlayer][1] += activitiesIncomes;
        }
    }

    public List<int> GetCurrentPlayerIncomes()
    {
        return playersIncomes[PlayersManager.players[currentPlayer]];
    }

    public void ResetCurrentPlayerIncomes()
    {
        playersIncomes[PlayersManager.players[currentPlayer]][0] = 0;
        playersIncomes[PlayersManager.players[currentPlayer]][1] = 0;
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

    private void UpdateBestRegions()
    {
        bestRegions?.Clear();
        for (int i = 0; i < 6; i++)
        {
            if (!worstRegions.Contains(i))
            {
                bestRegions.Add(i);
            }
        }
    }

    private void DrawNewTask() //Estrae una nuova task
    {
        int randAction = Random.Range(1, 101);
        int action = -1;
        int region = -1;
        if (randAction > 0 && randAction <= 20)
        {
            action = 0;
            region = worstRegions[GetRandomBadRegion(action)];
        }
        else if (randAction > 20 && randAction <= 60)
        {
            action = 1;
            region = worstRegions[GetRandomBadRegion(action)];
        }
        else if (randAction > 60 && randAction <= 100)
        {
            action = 2;
            region = bestRegions[GetRandomGoodRegion(action)];
        }
        int randTask = GetTask(region, action);
        drawnTasks.Add(new DrawnTask(randTask));
    }

    private int GetRandomBadRegion(int action)
    {
        bool available = false;
        int region = -1;
        while (!available)
        {
            region = Random.Range(0, 3);
            bool found = false;
            for (int i = 0; i < drawnTasks.Count; i++)
            {
                if (TasksManager.tasks[drawnTasks[i].task].GetRegion() == worstRegions[region] && TasksManager.tasks[drawnTasks[i].task].GetAction() == action)
                {
                    found = true;
                }
            }
            if (!found)
            {
                available = true;
            }
        }
        return region;
    }

    private int GetRandomGoodRegion(int action)
    {
        bool available = false;
        int region = -1;
        while (!available)
        {
            region = Random.Range(0, 3);
            bool found = false;
            for (int i = 0; i < drawnTasks.Count; i++)
            {
                if (TasksManager.tasks[drawnTasks[i].task].GetRegion() == bestRegions[region] && TasksManager.tasks[drawnTasks[i].task].GetAction() == action)
                {
                    found = true;
                }
            }
            if (!found)
            {
                available = true;
            }
        }
        return region;
    }

    private int GetTask(int region, int action)
    {
        for (int i = 0; i < 18; i++)
        {
            if (TasksManager.tasks[i].GetRegion() == region && TasksManager.tasks[i].GetAction() == action)
            {
                return i;
            }
        }
        return -1;
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

    public bool UseRegion(int index, int duration) //Attività regionale: vengono sottratti soldi al "currentPlayer", aggiunti voti ad "activitiesIncomes", ed eventualmente aggiunti soldi a "tasksIncomes"
    {
        int[] production = RegionsManager.regions[index].GetCurrentVotesRate();
        PlayersManager.players[currentPlayer].AddMoney(-RegionsManager.regions[index].GetCost());
        int votes = Random.Range(production[0], production[1] + 1);
        AddVotes(votes);
        activitiesIncomes += votes;
        usedActivity = index;
        activitiesState[usedActivity].Add(new ActivitiesState(PlayersManager.players[currentPlayer], duration));
        int completedTask = FindCompletedTask(index, 0);
        if (completedTask != -1)
        {
            drawnTasks[completedTask].completed = true;
            int money = TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
            tasksIncomes += money;
            //Avvia animazione task completata
            Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
            playersIncomes[PlayersManager.players[currentPlayer]][1] += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
            return true;
        }
        return false;
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

    public bool UpgradeRegion(int index, int value) //Investimento: sottrae soldi al "currentPlayer", aggiunge un investimento o un sabotaggio a "regionsUpgrades", ed eventualmente aggiunti soldi a "tasksIncomes"
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
                int money = TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                tasksIncomes += money;
                //Avvia animazione task completata
                Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
                playersIncomes[PlayersManager.players[currentPlayer]][1] += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                return true;
            }
        }
        else if (value == -1)
        {
            int completedTask = FindCompletedTask(index, 2);
            if (completedTask != -1)
            {
                drawnTasks[completedTask].completed = true;
                int money = TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                tasksIncomes += money;
                //Avvia animazione task completata
                Debug.Log("Task '" + TasksManager.tasks[drawnTasks[completedTask].task].GetName() + "' completata");
                playersIncomes[PlayersManager.players[currentPlayer]][1] += TasksManager.tasks[drawnTasks[completedTask].task].GetMoney();
                return true;
            }
        }
        return false;
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
            if (player.GetVotes() >= votesToObtain)
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
