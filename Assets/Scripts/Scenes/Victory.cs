using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private RawImage icon;

    private void Awake()
    {
        title.text = "VITTORIA DI\n" + PlayersManager.players[GameManager.instance.GetWinner()].GetName();
        icon.texture = PlayersManager.players[GameManager.instance.GetWinner()].GetIcon();
    }

    private void Start()
    {
        SoundEffectsManager.instance.PlayVictoryClip();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
