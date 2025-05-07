using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] firstPlayerInfo;
    [SerializeField] private GameObject[] secondPlayerInfo;
    [SerializeField] private GameObject[] thirdPlayerInfo;
    [SerializeField] private GameObject[] fourthPlayerInfo;
    [SerializeField] private GameObject playersTab;
    [SerializeField] private GameObject regionTab;
    [SerializeField] private TMP_Text regionName;
    [SerializeField] private Button startActivityButton;
    [SerializeField] private Button upgradeActivityButton;
    private List<int> unusableActivities;
    private int selectedRegion;
    private bool readyToStart;
    private bool readyToUpgrade;

    private void Awake()
    {
        unusableActivities = new List<int>();
        readyToStart = true;
        readyToUpgrade = true;

        UpdatePlayersStats();
    }

    private void UpdatePlayersStats()
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

    public void ShowAllPlayers()
    {
        playersTab.SetActive(true);
    }

    public void HideAllPlayers()
    {
        playersTab.SetActive(false);
    }

    public void SelectRegion(int index)
    {
        startActivityButton.interactable = (!unusableActivities.Contains(index) && PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= RegionsManager.regions[index].GetCost());
        upgradeActivityButton.interactable = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetMoney() >= 1000;
        switch (index)
        {
            case 0:
                regionName.text = "ABRUZZO";
                break;
            case 1:
                regionName.text = "CAMPANIA";
                break;
            case 2:
                regionName.text = "PUGLIA";
                break;
            case 3:
                regionName.text = "BASILICATA";
                break;
            case 4:
                regionName.text = "CALABRIA";
                break;
            case 5:
                regionName.text = "SICILIA";
                break;
            default:
                regionName.text = "MOLISE";
                break;
        }
        selectedRegion = index;
        regionTab.SetActive(true);
    }

    public void HideRegionTab()
    {
        regionTab.SetActive(false);
    }

    public void StartActivity()
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
                startActivityButton.interactable = false;
            }
            readyToStart = true;
        }
    }

    public void UpgradeActivity(int value)
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
                startActivityButton.interactable = false;
            }
            readyToUpgrade = true;
        }
    }

    public void NextTurn()
    {
        GameManager.instance.ChangeTurn();
    }
}
