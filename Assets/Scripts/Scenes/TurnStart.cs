using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnStartManager : MonoBehaviour
{
    [SerializeField] private TMP_Text title; //Dai che lo sai...
    [SerializeField] private RawImage icon; //Icona del "currentPlayer"

    private void Awake()
    {
        title.text = "TURNO DI " + PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetName();
        icon.texture = PlayersManager.players[GameManager.instance.GetCurrentPlayer()].GetIcon();
    }

    private void Start()
    {
        SoundEffectsManager.instance.PlayNextTurnClip();
    }

    public void StartTurn() //Inizia effettivamente il turno
    {
        SoundEffectsManager.instance.PlayButtonClip();
        GameManager.instance.SetOperative(true);
        GameManager.instance.OnTurnStart();
        SceneManager.LoadScene("GameMap");
    }
}
