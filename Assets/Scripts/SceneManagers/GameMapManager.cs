using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMapManager : MonoBehaviour
{
    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject playersTab;
    [SerializeField] private GameObject regionTab;
    [SerializeField] private TMP_Text regionName;
    [SerializeField] private Button[] regions;
    [SerializeField] private Button StartMinigameButton;
    private int selectedRegion;

    private void Awake()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            Transform playerStats;
            if (i == GameManager.instance.GetCurrentPlayer())
            {
                playerStats = playerInfo.transform.GetChild(0).transform;
            }
            else
            {
                playerStats = playersTab.transform.GetChild(count).transform;
                count++;
            }
            playerStats.GetChild(0).GetComponent<RawImage>().texture = PlayersManager.players[i].GetIcon();
            playerStats.GetChild(1).GetComponent<TMP_Text>().text = PlayersManager.players[i].GetName();
        }
        if (!GameManager.instance.IsOperative())
        {
            foreach (Button region in regions)
            {
                region.interactable = false;
            }
        }
    }

    public void ShowAllPlayers()
    {
        playersTab.SetActive(true);
    }

    public void HideAllPlayers()
    {
        playersTab.SetActive(false);
    }

    public void SelectRegion(int index)
    {
        switch (index)
        {
            case 0:
                regionName.text = "ABRUZZO";
                break;
            case 1:
                regionName.text = "CAMPANIA";
                break;
            case 2:
                regionName.text = "PUGLIA";
                break;
            case 3:
                regionName.text = "BASILICATA";
                break;
            case 4:
                regionName.text = "CALABRIA";
                break;
            case 5:
                regionName.text = "SICILIA";
                break;
            default:
                regionName.text = "???";
                break;
        }
        selectedRegion = index;
        regionTab.SetActive(true);
    }

    public void HideRegionTab()
    {
        regionTab.SetActive(false);
    }

    public void StartMinigame()
    {
        switch (selectedRegion)
        {
            case 0:
                Debug.Log("ABRUZZO");
                break;
            case 1:
                Debug.Log("CAMPANIA");
                break;
            case 2:
                Debug.Log("PUGLIA");
                break;
            case 3:
                Debug.Log("BASILICATA");
                break;
            case 4:
                Debug.Log("CALABRIA");
                break;
            case 5:
                Debug.Log("SICILIA");
                break;
            default:
                Debug.Log("REGIONE INESISTENTE");
                break;
        }
        foreach (Button region in regions)
        {
            region.interactable = false;
        }
        GameManager.instance.SetOperative(false);
        regionTab.SetActive(false);
    }

    public void Sabotage()
    {
        foreach (Button region in regions)
        {
            region.interactable = false;
        }
        GameManager.instance.SetOperative(false);
        regionTab.SetActive(false);
        Debug.Log("SABOTAGE: " + GameManager.instance.GetCurrentPlayer());
    }

    public void NextTurn()
    {
        GameManager.instance.ChangeTurn();
    }
}
