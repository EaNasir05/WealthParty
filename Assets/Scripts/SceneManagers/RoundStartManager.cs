using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundStartManager : MonoBehaviour
{
    [SerializeField] private GameObject turnsOrder;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject round;
    [SerializeField] private GameObject continueButton;

    private void Awake()
    {
        round.GetComponent<TMP_Text>().text = "INIZIO ROUND " + GameManager.instance.GetRound();
        GameManager.instance.OnRoundStart();
        for (int i = 0; i < 4; i++)
        {
            turnsOrder.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            turnsOrder.transform.GetChild(i).transform.GetChild(2).GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
        }
    }

    public void ShowNewTurnsOrder()
    {
        round.SetActive(false);
        continueButton.SetActive(false);
        title.SetActive(true);
        turnsOrder.SetActive(true);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("TurnStart");
    }
}
