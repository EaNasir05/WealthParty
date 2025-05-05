using UnityEngine;
using UnityEngine.UI;

public class GameMapManager : MonoBehaviour
{
    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject playersTab;
    [SerializeField] private GameObject regionTab;

    private void Awake()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i == GameManager.instance.GetCurrentPlayer())
            {
                Transform playerStats = playerInfo.transform.GetChild(0).transform;
                playerStats.GetChild(0).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            }
            else
            {
                Transform playerStats = playersTab.transform.GetChild(count).transform;
                playerStats.GetChild(0).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
                count++;
            }
        }
    }

    private void Update()
    {
        
    }

    public void ShowAllPlayers()
    {
        playersTab.SetActive(true);
    }

    public void HideAllPlayers()
    {
        playersTab.SetActive(false);
    }
}
