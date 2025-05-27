using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityDuration : MonoBehaviour
{
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private TMP_Text activityCost;
    private int originalCost;

    public void IncreaseDuration()
    {
        int duration = int.Parse(gameObject.GetComponent<TMP_Text>().text);
        duration++;
        decreaseButton.interactable = true;
        increaseButton.interactable = false;
        int cost = RegionsManager.regions[GameMapManager.selectedRegion].GetCost();
        activityCost.text = (cost * 2) + "€";
        gameObject.GetComponent<TMP_Text>().text = duration.ToString();
    }

    public void DecreaseDuration()
    {
        int duration = int.Parse(gameObject.GetComponent<TMP_Text>().text);
        duration--;
        increaseButton.interactable = true;
        decreaseButton.interactable = false;
        int cost = RegionsManager.regions[GameMapManager.selectedRegion].GetCost();
        activityCost.text = cost + "€";
        gameObject.GetComponent<TMP_Text>().text = duration.ToString();
    }
}
