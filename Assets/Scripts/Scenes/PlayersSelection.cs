using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayersSelectionManager : MonoBehaviour
{
    [SerializeField] private RawImage[] playersIcons; //Icone dei giocatori
    [SerializeField] private TMP_InputField[] playersNames; //Input in cui scrivere i nomi dei giocatori

    private void Awake() //Mette un limite di carattere per i "playersNames"
    {
        for (int i = 0; i < playersNames.Length; i++)
        {
            playersNames[i].characterLimit = 10;
        }
    }

    private void Start()
    {
        SoundEffectsManager.instance.PlayButtonClip();
    }

    public void StartGame() //Assegna le icone e i nomi ai players in "PlayersManager" e inizia la partita
    {
        for (int x = 0; x < 4; x++)
        {
            PlayersManager.players[x].SetIcon(playersIcons[x].texture);
            PlayersManager.players[x].SetName(playersNames[x].text);
        }
        SceneManager.LoadScene("ChooseVotes");
    }
}
