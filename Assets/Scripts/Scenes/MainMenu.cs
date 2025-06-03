using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
        SceneManager.LoadScene("ChoosePlayers");
    }

    public void Quit() //Chiude il gioco
    {
        Application.Quit();
    }
}
