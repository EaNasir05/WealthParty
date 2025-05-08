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
    [SerializeField] private GameObject regionTab; //Scheda che contiene le statistiche della "selectedRegion", e i pulsanti per svolgere la sua attività regionale e per investirci
    [SerializeField] private TMP_Text regionName; //Nome della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionActivityCost; //Costo dell'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionUpgradeCost; //Costo dell'investimento nella "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionVotesProduction; //Produzione di voti dell'attività regionale della "selectedRegion" nella "regionTab"
    [SerializeField] private TMP_Text regionMoneyProduction; //Produzione di denaro dell'attività regionale della "selectedRegion" nella "regionTab"
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
        startActivityButton.interactable = !unusableActivities.Contains(index) && PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= RegionsManager.regions[index].GetCost() && GameManager.instance.IsAnAvailableRegion(index);
        buffActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 1000;
        nerfActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 1000;
        regionName.text = RegionsManager.regions[index].GetName();
        regionVotesProduction.text = RegionsManager.regions[index].GetVotesRate().ToString();
        regionMoneyProduction.text = RegionsManager.regions[index].GetMoneyRate().ToString();
        regionActivityCost.text = RegionsManager.regions[index].GetCost().ToString();
        regionUpgradeCost.text = (1000).ToString();
        selectedRegion = index;
        regionTab.SetActive(true);
    }

    public void HideRegionTab() //Nasconde la regionTab
    {
        regionTab.SetActive(false);
    }

    public void StartActivity() //Svolge l'attività regionale della "selectedRegion" e aggiorna i dati dei giocatori e della "regionTab"
    {
        if (readyToStart)
        {
            readyToStart = false;
            GameManager.instance.UseRegion(selectedRegion);
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
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < 1000)
            {
                buffActivityButton.interactable = false;
                nerfActivityButton.interactable = false;
            }
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
            if (PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() < 1000)
            {
                buffActivityButton.interactable = false;
                nerfActivityButton.interactable= false;
            }
            readyToUpgrade = true;
        }
    }

    public void NextTurn() //Termina il turno
    {
        GameManager.instance.ChangeTurn();
    }
}
