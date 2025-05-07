using UnityEngine;
using UnityEngine.UI;

public class Player
{
    private Texture icon;
    private string name;
    private int money;
    private int votes;

    public Player()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        money = 3000;
        votes = 0;
    }
    
    public Texture GetIcon() { return icon; }
    public string GetName() { return name; }
    public int GetMoney() {  return money; }
    public int GetVotes() { return votes; }

    public void SetIcon(Texture icon) { this.icon = icon; }
    public void SetName(string name) {  this.name = name; }

    public void AddMoney(int value)
    {
        money += value;
        if (money < 0)
        {
            money = 0;
        }
    }

    public void AddVotes(int value)
    {
        votes += value;
        if (votes < 0)
        {
            votes = 0;
        }
    }
}
