using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayersSelectionManager : MonoBehaviour
{
    [SerializeField] private RawImage[] playersIcons;
    [SerializeField] private TMP_InputField[] playersNames;

    private void Awake()
    {
        for (int i = 0; i < playersNames.Length; i++)
        {
            playersNames[i].characterLimit = 10;
        }
    }

    public void StartGame()
    {
        for (int x = 0; x < 4; x++)
        {
            PlayersManager.players[x].SetIcon(playersIcons[x].texture);
            PlayersManager.players[x].SetName(playersNames[x].text);
        }
        SceneManager.LoadScene("RoundStart");
    }
}
