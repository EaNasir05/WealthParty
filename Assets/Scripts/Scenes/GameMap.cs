using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameMapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerInfo; //Informazioni stampate a schermo del "currentPlayer"
    [SerializeField] private TMP_Text playerVotesIncome;
    [SerializeField] private TMP_Text playerMoneyIncome;
    [SerializeField] private GameObject[] incomesTabs; //Schede che mostrano le entrate a inizio turno
    [SerializeField] private GameObject taskCompletedTab; //Div che compare quando il player completa una task
    [SerializeField] private GameObject tasksTab; //Scheda che contiene le task estratte in questo round
    [SerializeField] private TMP_Text[] tasksNames; //Nomi delle attivit� stampati nella "tasksTab"
    [SerializeField] private GameObject taskInfoTab; //Finestra che contiene le informazioni di una attivit�
    [SerializeField] private TMP_Text[] taskInfo; //Informazioni della task contenute in "taskInfoTab"
    [SerializeField] private Button goToRegionButton; //Bottone nella "taskInfoTab" che ti porta alla regione della task
    [SerializeField] private GameObject activityTab; //Scheda che contiene le informazioni di una attivit� regionale
    [SerializeField] private TMP_Text activityDuration; //Testo che contiene la durata della tua prossima attivit�
    [SerializeField] private GameObject regionTab; //Scheda che contiene le statistiche della "selectedRegion", e i pulsanti per svolgere la sua attivit� regionale e per investirci
    [SerializeField] private TMP_Text regionName; //Nome della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionLevel;
    [SerializeField] private TMP_Text regionActivityCost; //Costo dell'attivit� regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionUpgradeCost; //Costo dell'investimento nella "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionVotesProduction; //Produzione di voti dell'attivit� regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private RawImage regionPlayerIcon; //Icona del giocatore che sta occupando la regione selezionata
    [SerializeField] private RawImage[] regionsPlayersIcons; //Icone dei giocatori sopra le regioni nella mappa
    [SerializeField] private GameObject regionProductionLevel; //Immagine che rappresenta il livello di produzione della regione
    [SerializeField] private Button tasksButton; //Bottone da premere per aprire la "tasksTab"
    [SerializeField] private Button activityButton; //Bottone da premere per aprire la "activityTab"
    [SerializeField] private Button startActivityButton; //Bottone da premere per avviare l'attivit� regionale della "selectedRegion" nella "activityTab"
    [SerializeField] private Button buffActivityButton; //Bottone da premere per investire positivamente nell'attivit� regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private Button nerfActivityButton; //Bottone da premere per investire negativamente nell'attivit� regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private GameObject upgradesTab;
    [SerializeField] private TMP_Text activityName;
    [SerializeField] private TMP_Text activityCost;
    [SerializeField] private GameObject[] infoTabs;
    public static int selectedRegion; //Regione selezionata dalla mappa
    private int selectedInfoTab;
    public static int lastActivityIncome;
    private bool readyToStart; //Booleana che previene errori di spam
    private bool readyToUpgrade; //Booleana che previene errori di spam
    private List<int> upgradesOfThisTurn;

    private void Awake()
    {
        readyToStart = true;
        readyToUpgrade = true;
        UpdatePlayerStats();
        if (GameManager.instance.GetRound() < 2)
        {
            tasksButton.interactable = false;
        }
        upgradesOfThisTurn = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            Player player = GameManager.instance.GetPlayerOnRegion(i);
            if (player != null)
            {
                regionsPlayersIcons[i].texture = player.GetIcon();
                regionsPlayersIcons[i].gameObject.SetActive(true);
            }
            upgradesOfThisTurn.Add(0);
        }
        selectedInfoTab = 0;
    }

    private void Start()
    {
        List<int> incomes = GameManager.instance.GetCurrentPlayerIncomes();
        SoundEffectsManager.instance.PlayIncomeClip();
        incomesTabs[0].SetActive(true);
        StartCoroutine(DissolveItem(incomesTabs[0], 2));
        for (int i = 0; i < 2; i++)
        {
            if (incomes[i] != 0)
            {
                string value;
                if (i == 0)
                {
                    value = "V";
                }
                else
                {
                    value = "�";
                }
                incomesTabs[i + 1].transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + incomes[i] + value;
                incomesTabs[i + 1].SetActive(true);
                StartCoroutine(DissolveItem(incomesTabs[i + 1], 2));
            }
        }
        playerMoneyIncome.text = "+" + (incomes[1] + 3000);
        playerMoneyIncome.color = Color.green;
        playerMoneyIncome.gameObject.SetActive(true);
        StartCoroutine(DissolveItem(playerMoneyIncome.gameObject, 1));
        if (incomes[0] > 0)
        {
            playerVotesIncome.text = "+" + incomes[0];
            playerVotesIncome.color = Color.green;
            playerVotesIncome.gameObject.SetActive(true);
            StartCoroutine(DissolveItem(playerVotesIncome.gameObject, 1));
        }
    }

    private IEnumerator DissolveItem(GameObject item, int time)
    {
        yield return new WaitForSeconds(2);
        item.SetActive(false);
    }

    private void UpdatePlayerStats() //Aggiorna le statistiche di tutti i giocatori su schermo
    {
        playerInfo[0].GetComponent<RawImage>().texture = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetIcon();
        playerInfo[1].GetComponent<TMP_Text>().text = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetName();
        playerInfo[2].GetComponent<TMP_Text>().text = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetVotes().ToString();
        playerInfo[3].GetComponent<TMP_Text>().text = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney().ToString();
    }

    private void UpdateProductionLevel(int region)
    {
        int level = RegionsManager.regions[region].GetLevel() + upgradesOfThisTurn[region];
        if (level >= 0)
        {
            int count = 0;
            for (int i = 0; i < level; i++)
            {
                regionProductionLevel.transform.GetChild(i).GetComponent<RawImage>().color = Color.green;
                count++;
            }
            for (int i = 3; i >= count; i--)
            {
                regionProductionLevel.transform.GetChild(i).GetComponent<RawImage>().color = Color.gray;
            }
        }
        else if (level < 0)
        {
            int count = 0;
            for (int i = 0; i < level * -1; i++)
            {
                regionProductionLevel.transform.GetChild(i).GetComponent<RawImage>().color = Color.red;
                count++;
            }
            for (int i = 3; i >= count; i--)
            {
                regionProductionLevel.transform.GetChild(i).GetComponent<RawImage>().color = Color.gray;
            }
        }
    }

    public void SelectRegion(int index) //Apre la regionTab e ne cambia il contenuto in base alla regione selezionata
    {
        SoundEffectsManager.instance.PlayButtonClip();
        taskInfoTab.SetActive(false);
        upgradesTab.SetActive(true);
        SoundEffectsManager.instance.PlayRegionOST(index);
        activityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= RegionsManager.regions[index].GetCost() && GameManager.instance.IsAnAvailableRegion(index);
        buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(index, 1);
        nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(index, -1);
        regionName.text = RegionsManager.regions[index].GetName();
        Player playerOnRegion = GameManager.instance.GetPlayerOnRegion(index);
        if (playerOnRegion != null)
        {
            regionPlayerIcon.gameObject.SetActive(true);
            regionPlayerIcon.texture = playerOnRegion.GetIcon();
        }
        else
        {
            regionPlayerIcon.gameObject.SetActive(false);
        }
        List<int> previousUsedRegion = GameManager.instance.GetPreviousUsedRegion();
        string levelText = "LIVELLO: " + RegionsManager.regions[index].GetLevel();
        if (previousUsedRegion[0] == index)
        {
            levelText += " (" + (previousUsedRegion[1] * -1) + ")";
        }
        regionLevel.text = levelText;
        int[] production = GameManager.instance.GetPlayerVotesRateOnRegion(index);
        regionVotesProduction.text = production[0] + " - " + production[1];
        regionActivityCost.text = RegionsManager.regions[index].GetCost() + "�";
        regionUpgradeCost.text = 500.ToString() + "�";
        selectedRegion = index;
        UpdateProductionLevel(index);
        regionTab.SetActive(true);
    }

    public void HideRegionTab() //Nasconde la regionTab
    {
        SoundEffectsManager.instance.StopCurrentOST();
        SoundEffectsManager.instance.PlayButtonClip();
        regionTab.SetActive(false);
        activityTab.SetActive(false);
    }

    public void ShowActivityTab()
    {
        SoundEffectsManager.instance.PlayButtonClip();
        activityCost.text = RegionsManager.regions[selectedRegion].GetCost() + "�";
        activityDuration.text = "1";
        activityName.text = RegionsManager.regions[selectedRegion].GetActivity();
        activityTab.transform.GetChild(5).GetComponent<Button>().interactable = false;
        activityTab.transform.GetChild(4).GetComponent<Button>().interactable = true;
        upgradesTab.SetActive(false);
        activityTab.SetActive(true);
    }

    public void HideActivityTab()
    {
        SoundEffectsManager.instance.PlayButtonClip();
        activityTab.SetActive(false);
        upgradesTab.SetActive(true);
    }

    public void ShowTasksList() //Apre la "tasksTab"
    {
        SoundEffectsManager.instance.PlayButtonClip();
        List<DrawnTask> drawnTasks = GameManager.instance.GetDrawnTasks();
        for (int i = 0; i < drawnTasks.Count; i++)
        {
            if (!drawnTasks[i].completed)
            {
                tasksNames[i].text = TasksManager.tasks[drawnTasks[i].task].GetName();
                tasksNames[i].gameObject.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                tasksNames[i].gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        tasksTab.SetActive(true);
    }

    public void HideTasksList() //Chiude la "tasksTab"
    {
        SoundEffectsManager.instance.PlayButtonClip();
        tasksTab.SetActive(false);
    }

    public void ShowTaskInfo(int index) //Apre la "taskInfoTab"
    {
        SoundEffectsManager.instance.PlayButtonClip();
        List<DrawnTask> drawnTasks = GameManager.instance.GetDrawnTasks();
        taskInfo[0].text = TasksManager.tasks[drawnTasks[index].task].GetName();
        taskInfo[1].text = TasksManager.tasks[drawnTasks[index].task].GetMoney() + "�";
        taskInfo[2].text = TasksManager.tasks[drawnTasks[index].task].GetDescription();
        goToRegionButton.onClick.RemoveAllListeners();
        goToRegionButton.onClick.AddListener(() => FromTaskToRegion(TasksManager.tasks[drawnTasks[index].task].GetRegion()));
        taskInfoTab.SetActive(true);
    }

    public void FromTaskToRegion(int region)
    {
        tasksTab.SetActive(false);
        taskInfoTab.SetActive(false);
        SelectRegion(region);
    }

    public void HideTaskInfo() //Chiude la "taskInfoTab"
    {
        SoundEffectsManager.instance.PlayButtonClip();
        taskInfoTab.SetActive(false);
    }

    public void ShowInfoTab(int index)
    {
        SoundEffectsManager.instance.PlayButtonClip();
        infoTabs[selectedInfoTab].SetActive(false);
        selectedInfoTab = index;
        infoTabs[selectedInfoTab].transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 1; i < infoTabs[selectedInfoTab].transform.childCount; i++)
        {
            infoTabs[selectedInfoTab].transform.GetChild(i).gameObject.SetActive(false);
        }
        infoTabs[selectedInfoTab].SetActive(true);
    }

    public void ShowNextInfoTab(int index)
    {
        SoundEffectsManager.instance.PlayButtonClip();
        infoTabs[selectedInfoTab].transform.GetChild(index).gameObject.SetActive(false);
        infoTabs[selectedInfoTab].transform.GetChild(index + 1).gameObject.SetActive(true);
    }

    public void ShowPreviousInfoTab(int index)
    {
        SoundEffectsManager.instance.PlayButtonClip();
        infoTabs[selectedInfoTab].transform.GetChild(index).gameObject.SetActive(false);
        infoTabs[selectedInfoTab].transform.GetChild(index - 1).gameObject.SetActive(true);
    }

    public void HideInfoTab()
    {
        SoundEffectsManager.instance.PlayButtonClip();
        infoTabs[selectedInfoTab].SetActive(false);
    }

    public void StartActivity() //Svolge l'attivit� regionale della "selectedRegion" e aggiorna i dati dei giocatori e della "regionTab"
    {
        if (readyToStart)
        {
            SoundEffectsManager.instance.PlayButtonClip();
            readyToStart = false;
            bool completed = GameManager.instance.UseRegion(selectedRegion, int.Parse(activityDuration.text));
            int income = -(RegionsManager.regions[selectedRegion].GetCost() * int.Parse(activityDuration.text));
            SoundEffectsManager.instance.PlayIncomeClip();
            playerMoneyIncome.color = Color.red;
            playerMoneyIncome.text = income.ToString();
            playerMoneyIncome.gameObject.SetActive(true);
            StartCoroutine(DissolveItem(playerMoneyIncome.gameObject, 1));
            playerVotesIncome.color = Color.green;
            playerVotesIncome.text = "+" + lastActivityIncome;
            playerVotesIncome.gameObject.SetActive(true);
            StartCoroutine(DissolveItem(playerVotesIncome.gameObject, 1));
            regionPlayerIcon.texture = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetIcon();
            regionPlayerIcon.gameObject.SetActive(true);
            regionsPlayersIcons[selectedRegion].texture = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetIcon();
            regionsPlayersIcons[selectedRegion].gameObject.SetActive(true);
            UpdatePlayerStats();
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < RegionsManager.regions[selectedRegion].GetCost())
            {
                startActivityButton.interactable = false;
            }
            if (completed)
            {
                taskCompletedTab.SetActive(true);
                StartCoroutine(DissolveItem(taskCompletedTab, 2));
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
            SoundEffectsManager.instance.PlayButtonClip();
            readyToUpgrade = false;
            SoundEffectsManager.instance.PlayIncomeClip();
            playerMoneyIncome.color = Color.red;
            playerMoneyIncome.text = "-500";
            playerMoneyIncome.gameObject.SetActive(true);
            StartCoroutine(DissolveItem(playerMoneyIncome.gameObject, 1));
            upgradesOfThisTurn[selectedRegion] += value;
            UpdateProductionLevel(selectedRegion);
            bool completed = GameManager.instance.UpgradeRegion(selectedRegion, value);
            UpdatePlayerStats();
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < RegionsManager.regions[selectedRegion].GetCost())
            {
                activityButton.interactable = false;
            }
            if (completed)
            {
                taskCompletedTab.SetActive(true);
                StartCoroutine(DissolveItem(taskCompletedTab, 2));
            }
            buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, 1);
            nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 500 && GameManager.instance.IsUpgradable(selectedRegion, -1);
            readyToUpgrade = true;
        }
    }

    public void NextTurn() //Termina il turno
    {
        SoundEffectsManager.instance.PlayButtonClip();
        GameManager.instance.ChangeTurn();
    }
}
