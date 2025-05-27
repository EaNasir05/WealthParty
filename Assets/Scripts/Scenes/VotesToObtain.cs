using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VotesToObtain : MonoBehaviour
{
    [SerializeField] private TMP_InputField votesToObtain;
    [SerializeField] private GameObject infoTab;

    private void Awake() //Mette un limite di caratteri per "votesToObtain"
    {
        votesToObtain.characterLimit = 5;
    }

    public void StartGame() //Salva "votesToObtain" nel game manager e inizia la partita
    {
        GameManager.instance.SetVotesToObtain(int.Parse(votesToObtain.text));
        SceneManager.LoadScene("RoundStart");
    }

    public void ShowInfoTab()
    {
        infoTab.SetActive(true);
    }

    public void HideInfoTab()
    {
        infoTab.SetActive(false);
    }
}
