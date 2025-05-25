using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VotesToObtain : MonoBehaviour
{
    [SerializeField] private TMP_InputField votesToObtain;

    private void Awake() //Mette un limite di caratteri per "votesToObtain"
    {
        votesToObtain.characterLimit = 5;
    }

    public void StartGame() //Salva "votesToObtain" nel game manager e inizia la partita
    {
        GameManager.instance.SetVotesToObtain(int.Parse(votesToObtain.text));
        SceneManager.LoadScene("RoundStart");
    }
}
