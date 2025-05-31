using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Awake()
    {
        PlayersManager.Awake();
        RegionsManager.Awake();
        TasksManager.Awake();
        new GameManager();
    }

    public void NewGame() //Va alla scena "ChoosePlayers"
    {
        SoundEffectsManager.instance.PlayButtonClip();
        SceneManager.LoadScene("ChoosePlayers");
    }

    public void Quit() //Chiude il gioco
    {
        SoundEffectsManager.instance.PlayButtonClip();
        Application.Quit();
    }
}
