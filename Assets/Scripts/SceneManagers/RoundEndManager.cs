using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundEndManager : MonoBehaviour
{
    [SerializeField] private GameObject roundTitle; //Dai che lo sai...
    [SerializeField] private GameObject continueButton; //Bottone da premere per continuare
    [SerializeField] private GameObject[] regionsInfo; //Tabella che mostra gli effetti degli investimenti sulle regioni
    [SerializeField] private GameObject[] playersInfo; //Tabella che mostra le statistiche dei giocatori
    private int click; //Quante volte è stato cliccato il "continueButton"

    private void Awake()
    {
        click = 0;
        roundTitle.GetComponent<TMP_Text>().text = "FINE ROUND " + GameManager.instance.GetRound();
    }

    public void ChangeScene() //Mostra gli effetti degli investimenti o inizia un nuovo round
    {
        click++;
        if (click == 1 && !GameManager.instance.GotAnyUpdates())
        {
            click++;
        }
        switch (click)
        {
            case 0:
                Debug.Log("NESSUN CLICK");
                break;
            case 1:
                roundTitle.SetActive(false);
                UpdateRegionsInfo();
                break;
            case 2:
                roundTitle.SetActive(false);
                foreach (GameObject regionInfo in regionsInfo)
                {
                    regionInfo.SetActive(false);
                }
                UpdatePlayersInfo();
                break;
            case 3:
                SceneManager.LoadScene("RoundStart");
                break;
            default:
                Debug.Log("TROPPI CLICK");
                break;
        }
    }

    private void UpdateRegionsInfo() //Aggiorna i valori all'interno di "regionsInfo"
    {
        List<Dictionary<string, int>> list = GameManager.instance.OnRoundEnd();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i]["oldVotesRate"] != RegionsManager.regions[list[i]["region"]].GetCurrentVotesRate())
            {
                regionsInfo[i].transform.GetChild(0).GetComponent<TMP_Text>().text = RegionsManager.regions[list[i]["region"]].GetName();
                regionsInfo[i].transform.GetChild(1).GetComponent<TMP_Text>().text = list[i]["oldVotesRate"].ToString() + "V";
                regionsInfo[i].transform.GetChild(2).GetComponent<TMP_Text>().text = RegionsManager.regions[list[i]["region"]].GetCurrentVotesRate().ToString() + "V";
                regionsInfo[i].SetActive(true);
            }
        }
    }

    private void UpdatePlayersInfo() //Aggiorna i valori all'interno di "playersInfo"
    {
        for (int i = 0; i < PlayersManager.players.Count; i++)
        {
            playersInfo[i].transform.GetChild(0).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            playersInfo[i].transform.GetChild(1).GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
            if (PlayersManager.players[i].GetVotes() >= 30000)
            {
                playersInfo[i].transform.GetChild(2).GetComponent<TMP_Text>().color = Color.red;
            }
            playersInfo[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "V: " + PlayersManager.players[i].GetVotes().ToString();
            playersInfo[i].transform.GetChild(3).GetComponent<TMP_Text>().text = "€: " + PlayersManager.players[i].GetMoney().ToString();
            playersInfo[i].SetActive(true);
        }
    }
}
