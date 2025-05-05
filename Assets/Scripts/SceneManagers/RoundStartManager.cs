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

    private void Awake()
    {
        Debug.Log(GameManager.instance.GetRound());
    }

    private void Start()
    {
        round.GetComponent<TMP_Text>().text = "Round " + GameManager.instance.GetRound();
        GameManager.instance.ChangeTurnsOrder();
        for (int i = 0; i < 4; i++)
        {
            turnsOrder.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            turnsOrder.transform.GetChild(i).transform.GetChild(2).GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
        }
        GameManager.instance.SetCurrentPlayer(0);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (turnsOrder.activeSelf)
            {
                SceneManager.LoadScene("TurnStart");
            }
            else
            {
                round.SetActive(false);
                title.SetActive(true);
                turnsOrder.SetActive(true);
            }
        }
    }
}
