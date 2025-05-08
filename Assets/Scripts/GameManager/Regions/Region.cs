using UnityEngine;

public class Region
{
    private string name;
    private int level;
    private int cost;
    private int moneyRate;
    private int currentMoneyRate;
    private int votesRate;
    private int currentVotesRate;
    private bool active;

    public Region()
    {
        active = true;
    }

    public void ResetStats(string name, int cost, int moneyRate, int votesRate)
    {
        level = 0;
        this.name = name;
        this.cost = cost;
        this.moneyRate = moneyRate;
        this.currentMoneyRate = moneyRate;
        this.votesRate = votesRate;
        this.currentVotesRate = votesRate;
    }

    public string GetName() { return name; }
    public int GetLevel() { return level; }
    public int GetCost() { return cost; }
    public int GetMoneyRate() { return currentMoneyRate; }
    public int GetVotesRate() { return currentVotesRate; }
    public bool isActive() { return active; }

    public void SetActive(bool value) { active = value; }
    public void SetCost(int cost) { this.cost = cost; }

    public void ChangeLevel(int value)
    {
        level += value;
        if (level < -5) { level = -5; }
        if (level > 5) { level = 5; }
        int increase = (int)(moneyRate * 0.10 * level);
        currentMoneyRate = moneyRate + increase;
    }
}
