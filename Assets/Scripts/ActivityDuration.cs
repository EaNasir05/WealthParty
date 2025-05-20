using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityDuration : MonoBehaviour
{
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    public void IncreaseDuration()
    {
        int duration = int.Parse(gameObject.GetComponent<TMP_Text>().text);
        duration++;
        decreaseButton.interactable = true;
        increaseButton.interactable = false;
        gameObject.GetComponent<TMP_Text>().text = duration.ToString();
    }

    public void DecreaseDuration()
    {
        int duration = int.Parse(gameObject.GetComponent<TMP_Text>().text);
        duration--;
        increaseButton.interactable = true;
        decreaseButton.interactable = false;
        gameObject.GetComponent<TMP_Text>().text = duration.ToString();
    }
}
