using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnStartManager : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private RawImage icon;

    private void Awake()
    {
        title.text = "TURNO DI " + PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetName();
        icon.texture = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetIcon();
    }

    public void StartTurn()
    {
        GameManager.instance.SetOperative(true);
        GameManager.instance.OnTurnStart();
        SceneManager.LoadScene("GameMap");
    }
}
