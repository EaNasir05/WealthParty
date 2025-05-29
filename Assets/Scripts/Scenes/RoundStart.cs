using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundStartManager : MonoBehaviour
{
    [SerializeField] private GameObject turnsOrder; //Tabella che mostra l'ordine dei giocatori nel prossimo round
    [SerializeField] private GameObject title; //Dai che lo sai...
    [SerializeField] private GameObject roundTitle; //Scritta che indica quale round sta per iniziare
    [SerializeField] private GameObject continueButton; //Bottone da premere per vedere il "turnsOrder"
    [SerializeField] private GameObject infoTab; // Finestra che mostra le info
    [SerializeField] private GameObject infoButton; // Bottone da premere per vedere l' "infoTab"
    private void Awake()
    {
        GameManager.instance.OnRoundStart();
        if (!GameManager.instance.IsLastRound())
        {
            roundTitle.GetComponent<TMP_Text>().text = "INIZIO MESE " + GameManager.instance.GetRound();
        }
        else
        {
            roundTitle.GetComponent<TMP_Text>().text = "ULTIMO MESE PRIMA DEL REFERENDUM";
        }
        for (int i = 0; i < 4; i++)
        {
            turnsOrder.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            turnsOrder.transform.GetChild(i).transform.GetChild(2).GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
        }
    }

    public void ShowNewTurnsOrder() //Mostra "turnsOrder"
    {
        roundTitle.SetActive(false);
        continueButton.SetActive(false);
        title.SetActive(true);
        turnsOrder.SetActive(true);
        infoButton.SetActive(true);
    }

    public void ChangeScene() //Inizia il primo turno
    {
        SceneManager.LoadScene("TurnStart");
    }

    public void ShowInfoTab()
    {
        infoTab.SetActive(true);
    }

    public void HideInfoTab()
    {
        infoTab.SetActive(false);
    }
}
