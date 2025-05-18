using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] firstPlayerInfo; //Informazioni stampate a schermo del "currentPlayer"
    [SerializeField] private GameObject[] secondPlayerInfo; //Informazioni stampate a schermo di un giocatore
    [SerializeField] private GameObject[] thirdPlayerInfo; //Informazioni stampate a schermo di un giocatore
    [SerializeField] private GameObject[] fourthPlayerInfo; //Informazioni stampate a schermo di un giocatore
    [SerializeField] private GameObject playersTab; //Scheda che contiene "secondPlayerInfo", "thirdPlayerInfo", "fourthPlayerInfo"
    [SerializeField] private GameObject tasksTab; //Scheda che contiene le task estratte in questo round;
    [SerializeField] private TMP_Text[] tasksNames;
    [SerializeField] private GameObject taskInfoTab;
    [SerializeField] private TMP_Text[] taskInfo;
    [SerializeField] private Button goToRegionButton;
    [SerializeField] private GameObject regionTab; //Scheda che contiene le statistiche della "selectedRegion", e i pulsanti per svolgere la sua attività regionale e per investirci
    [SerializeField] private TMP_Text regionName; //Nome della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionActivityCost; //Costo dell'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionUpgradeCost; //Costo dell'investimento nella "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionVotesProduction; //Produzione di voti dell'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private Button startActivityButton; //Bottone da premere per avviare l'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private Button buffActivityButton; //Bottone da premere per investire positivamente nell'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private Button nerfActivityButton; //Bottone da premere per investire negativamente nell'attività regionale della "selectedRegion" nella "regionTab"
    private List<int> unusableActivities;  //Lista che contiene le attività regionali che il "currentPlayer" non può svolgere
    private int selectedRegion; //Regione selezionata dalla mappa
    private bool readyToStart; //Booleana che previene errori si spam
    private bool readyToUpgrade; //Booleana che previene errori si spam

    private void Awake()
    {
        unusableActivities = new List<int>();
        readyToStart = true;
        readyToUpgrade = true;
        UpdatePlayersStats();
    }

    private void UpdatePlayersStats() //Aggiorna le statistiche di tutti i giocatori su schermo
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i == GameManager.instance.GetCurrentPlayer())
            {
                firstPlayerInfo[0].GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
                firstPlayerInfo[1].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
                firstPlayerInfo[2].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetVotes().ToString();
                firstPlayerInfo[3].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetMoney().ToString();
            }
            else
            {
                switch (count)
                {
                    case 0:
                        secondPlayerInfo[0].GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
                        secondPlayerInfo[1].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
                        secondPlayerInfo[2].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetVotes().ToString();
                        secondPlayerInfo[3].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetMoney().ToString();
                        break;
                    case 1:
                        thirdPlayerInfo[0].GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
                        thirdPlayerInfo[1].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
                        thirdPlayerInfo[2].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetVotes().ToString();
                        thirdPlayerInfo[3].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetMoney().ToString();
                        break;
                    case 2:
                        fourthPlayerInfo[0].GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
                        fourthPlayerInfo[1].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
                        fourthPlayerInfo[2].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetVotes().ToString();
                        fourthPlayerInfo[3].GetComponent<TMP_Text>().text = PlayersManager.players[i].GetMoney().ToString();
                        break;
                }
                count++;
            }
        }
    }

    public void ShowAllPlayers() //Mostra la "playersTab"
    {
        playersTab.SetActive(true);
    }

    public void HideAllPlayers() //Nasconde la "playersTab"
    {
        playersTab.SetActive(false);
    }

    public void SelectRegion(int index) //Apre la regionTab e ne cambia il contenuto in base alla regione selezionata
    {
        taskInfoTab.SetActive(false);
        startActivityButton.interactable = !unusableActivities.Contains(index) && PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= RegionsManager.regions[index].GetCost() && GameManager.instance.IsAnAvailableRegion(index);
        buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(index, 1);
        nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(index, -1);
        regionName.text = RegionsManager.regions[index].GetName();
        int[] production = RegionsManager.regions[index].GetCurrentVotesRate();
        regionVotesProduction.text = production[0] + " - " + production[1];
        regionActivityCost.text = RegionsManager.regions[index].GetCost().ToString() + "€";
        regionUpgradeCost.text = 500.ToString() + "€";
        selectedRegion = index;
        int level = RegionsManager.regions[index].GetLevel();
        if (level > 0)
        {
            for (int i = 0; i < level; i++)
            {
                Debug.Log("Colora pallino verde " + i);
            }
        }else if (level < 0)
        {
            for (int i = 0; i < level * -1; i++)
            {
                Debug.Log("Colora pallino rosso " + i);
            }
        }
        regionTab.SetActive(true);
    }

    public void HideRegionTab() //Nasconde la regionTab
    {
        regionTab.SetActive(false);
    }

    public void ShowTasksList()
    {
        List<DrawnTask> drawnTasks = GameManager.instance.GetDrawnTasks();
        for (int i = 0; i < drawnTasks.Count; i++)
        {
            if (!drawnTasks[i].completed)
            {
                tasksNames[i].text = TasksManager.tasks[drawnTasks[i].task].GetName();
            }
        }
        tasksTab.SetActive(true);
    }

    public void HideTasksList()
    {
        tasksTab.SetActive(false);
    }

    public void ShowTaskInfo(int index)
    {
        tasksTab.SetActive(false);
        List<DrawnTask> drawnTasks = GameManager.instance.GetDrawnTasks();
        taskInfo[0].text = TasksManager.tasks[drawnTasks[index].task].GetName();
        taskInfo[1].text = TasksManager.tasks[drawnTasks[index].task].GetMoney() + "€";
        taskInfo[2].text = TasksManager.tasks[drawnTasks[index].task].GetDescription();
        goToRegionButton.onClick.RemoveAllListeners();
        goToRegionButton.onClick.AddListener(() => SelectRegion(TasksManager.tasks[drawnTasks[index].task].GetRegion()));
        taskInfoTab.SetActive(true);
    }

    public void HideTaskInfo()
    {
        taskInfoTab.SetActive(false);
        tasksTab.SetActive(true);
    }

    public void StartActivity() //Svolge l'attività regionale della "selectedRegion" e aggiorna i dati dei giocatori e della "regionTab"
    {
        if (readyToStart)
        {
            readyToStart = false;
            GameManager.instance.UseRegion(selectedRegion, 1);
            UpdatePlayersStats();
            for (int i = 0; i < 6; i++)
            {
                if (selectedRegion != i)
                {
                    unusableActivities.Add(i);
                }
            }
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < RegionsManager.regions[selectedRegion].GetCost())
            {
                startActivityButton.interactable = false;
            }
            buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, 1);
            nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, -1);
            readyToStart = true;
        }
    }

    public void UpgradeActivity(int value) //Investe nella "selectedRegion" e aggiorna i dati dei giocatori e della "regionTab"
    {
        if (readyToUpgrade)
        {
            readyToUpgrade = false;
            GameManager.instance.UpgradeRegion(selectedRegion, value);
            UpdatePlayersStats();
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < RegionsManager.regions[selectedRegion].GetCost())
            {
                startActivityButton.interactable = false;
            }
            buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, 1);
            nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, -1);
            readyToUpgrade = true;
        }
    }

    public void NextTurn() //Termina il turno
    {
        GameManager.instance.ChangeTurn();
    }
}
