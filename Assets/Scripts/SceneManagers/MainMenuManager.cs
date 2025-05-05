using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialTab;

    private void Awake()
    {
        PlayersManager.Awake();
        new GameManager();
    }

    public void NewGame()
    {
        tutorialTab.SetActive(true);
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("ChoosePlayers");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
