using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialTab; //Schermata in cui chiede se vuoi giocare il tutorial

    private void Awake()
    {
        PlayersManager.Awake();
        RegionsManager.Awake();
        new GameManager();
    }

    public void NewGame() //Apre la "tutorialTab"
    {
        tutorialTab.SetActive(true);
    }

    public void StartTutorial() //Apre la scena Tutorial
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void StartGame() //Inizia una partita senza tutorial
    {
        SceneManager.LoadScene("ChoosePlayers");
    }

    public void Quit() //Chiude il gioco
    {
        Application.Quit();
    }
}
