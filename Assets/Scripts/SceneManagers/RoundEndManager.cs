using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundEndManager : MonoBehaviour
{
    [SerializeField] private GameObject title; //Dai che lo sai...

    private void Awake()
    {
        title.GetComponent<TMP_Text>().text = "FINE ROUND " + GameManager.instance.GetRound();
        GameManager.instance.OnRoundEnd();
    }

    public void ChangeScene() //Inizia un nuovo round
    {
        SceneManager.LoadScene("RoundStart");
    }
}
