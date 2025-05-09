using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundEndManager : MonoBehaviour
{
    [SerializeField] private GameObject roundTitle; //Dai che lo sai...
    [SerializeField] private GameObject continueButton; //Bottone da premere per continuare
    [SerializeField] private GameObject[] regionsInfo; //Tabella che mostra gli effetti degli investimenti sulle regioni
    private int click; //Quante volte è stato cliccato il "continueButton"

    private void Awake()
    {
        click = 0;
        roundTitle.GetComponent<TMP_Text>().text = "FINE ROUND " + GameManager.instance.GetRound();
    }

    public void ChangeScene() //Mostra gli effetti degli investimenti o inizia un nuovo round
    {
        click++;
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
                SceneManager.LoadScene("RoundStart");
                break;
            default:
                Debug.Log("TROPPI CLICK");
                break;
        }
    }

    private void UpdateRegionsInfo() //Aggiorna i valori testuali all'interno di "regionsInfo"
    {
        List<Dictionary<string, int>> list = GameManager.instance.OnRoundEnd();
        for (int i = 0; i < list.Count; i++)
        {
            regionsInfo[i].transform.GetChild(0).GetComponent<TMP_Text>().text = RegionsManager.regions[list[i]["region"]].GetName();
            regionsInfo[i].transform.GetChild(1).GetComponent<TMP_Text>().text = list[i]["oldMoneyRate"].ToString() + "€";
            regionsInfo[i].transform.GetChild(2).GetComponent<TMP_Text>().text = RegionsManager.regions[list[i]["region"]].GetMoneyRate().ToString() + "€";
            regionsInfo[i].transform.GetChild(3).GetComponent<TMP_Text>().text = list[i]["oldVotesRate"].ToString() + "V";
            regionsInfo[i].transform.GetChild(4).GetComponent<TMP_Text>().text = RegionsManager.regions[list[i]["region"]].GetVotesRate().ToString() + "V";
            regionsInfo[i].SetActive(true);
        }
    }
}
