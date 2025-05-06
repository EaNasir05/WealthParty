using UnityEngine;
using UnityEngine.UI;

public class Player
{
    private Texture icon;
    private string name;
    private int money;
    private int votes;
    private int abruzzoPoints;
    private int campaniaPoints;
    private int pugliaPoints;
    private int basilicataPoints;
    private int calabriaPoints;
    private int siciliaPoints;

    public Player()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        money = 0;
        votes = 0;
        abruzzoPoints = 0;
        campaniaPoints = 0;
        pugliaPoints = 0;
        basilicataPoints = 0;
        calabriaPoints = 0;
        siciliaPoints = 0;
    }
    
    public Texture GetIcon() { return icon; }
    public string GetName() { return name; }
    public int GetMoney() {  return money; }
    public int GetVotes() { return votes; }
    public int GetAbruzzoPoints() { return abruzzoPoints; }
    public int GetCampaniaPoints() { return campaniaPoints; }
    public int GetPugliaPoints() { return pugliaPoints; }
    public int GetBasilicataPoints() { return basilicataPoints; }
    public int GetCalabriaPoints() { return calabriaPoints; }
    public int GetSiciliaPoints() {return siciliaPoints; }

    public void SetIcon(Texture icon) { this.icon = icon; }
    public void SetName(string name) {  this.name = name; }

    public void AddMoney(int value) { money += value; }
    public void AddVotes(int value) { votes += value; }
    public void AddAbruzzoPoints(int value) { abruzzoPoints += value; }
    public void AddCampaniaPoints(int value) { campaniaPoints += value; }
    public void AddPugliaPoints(int value) { pugliaPoints += value; }
    public void AddBasilicataPoints(int value) { basilicataPoints += value; }
    public void AddCalabriaPoints(int value) { calabriaPoints += value; }
    public void AddSiciliaPoints(int value) { siciliaPoints += value; }
}
