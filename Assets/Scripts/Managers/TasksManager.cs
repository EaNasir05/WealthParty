using System.Collections.Generic;
using UnityEngine;

public class Task
{
    private string name; //Nome della task
    private string description; //Descrizione della task
    private int money; //Ricompensa in denaro della task
    private int region; //Regione a cui appartiene la task
    private int action; //Azione da svolgere per completare la task

    public Task(string name, string description, int money, int region, int action)
    {
        this.name = name;
        this.description = description;
        this.money = money;
        this.region = region;
        this.action = action;
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public string GetName() {  return name; }
    public string GetDescription() { return description; }
    public int GetMoney() { return money; }
    public int GetRegion() { return region; }
    public int GetAction() { return action; }
}

public class TasksManager
{
    public static List<Task> tasks; //Lista delle tasks che potrebbero essere estratte
    
    public static void Awake() //Istanzia la lista delle tasks (Non è l'Awake di Unity)
    {
        if (tasks == null)
        {
            tasks = new List<Task>();
            for (int i = 0; i < 12; i++)
            {
                switch (i)
                {
                    case 0:
                        tasks.Add(new Task("Avvia la costruzione di nuove rosticcerie", "Avvia almeno una volta l'attività in Abruzzo", 3000, 0, 0));
                        break;
                    case 1:
                        tasks.Add(new Task("Avvia la costruzione di discariche", "Avvia almeno una volta l'attività in Campania", 3000, 1, 0));
                        break;
                    case 2:
                        tasks.Add(new Task("Allestisci le impalcature nelle piazze", "Avvia almeno una volta l'attività in Puglia", 3000, 2, 0));
                        break;
                    case 3:
                        tasks.Add(new Task("Avvia la demolizione di nuovi negozi", "Avvia almeno una volta l'attività in Basilicata", 3000, 3, 0));
                        break;
                    case 4:
                        tasks.Add(new Task("Ara i campi nella piana di Sibari", "Avvia almeno una volta l'attività in Calabria", 3000, 4, 0));
                        break;
                    case 5:
                        tasks.Add(new Task("Fai un discorso contro la Mafia", "Avvia almeno una volta l'attività in Sicilia", 3000, 5, 0));
                        break;
                    case 6:
                        tasks.Add(new Task("Ingaggia il personale culinario", "Investi almeno una volta nell'Abruzzo", 1500, 0, 1));
                        break;
                    case 7:
                        tasks.Add(new Task("Ingaggia netturbini", "Investi almeno una volta nella Campania", 1500, 1, 1));
                        break;
                    case 8:
                        tasks.Add(new Task("Ingaggia musicisti e ballerini", "Investi almeno una volta nella Puglia", 1500, 2, 1));
                        break;
                    case 9:
                        tasks.Add(new Task("Disapprova le proposte dei giovani", "Investi almeno una volta nella Basilicata", 1500, 3, 1));
                        break;
                    case 10:
                        tasks.Add(new Task("Ingaggia nuovi contadini", "Investi almeno una volta nella Calabria", 1500, 4, 1));
                        break;
                    case 11:
                        tasks.Add(new Task("Ingaggia investigatori privati", "Investi almeno una volta nella Sicilia", 1500, 5, 1));
                        break;
                    case 12:
                        tasks.Add(new Task("Fai propaganda vegetariana", "Sabota almeno una volta l'Abruzzo", 1500, 0, 2));
                        break;
                    case 13:
                        tasks.Add(new Task("Nascondi la spazzatura tra i vicoli", "Sabota almeno una volta la Campania", 1500, 1, 2));
                        break;
                    case 14:
                        tasks.Add(new Task("Rimuovi i manifesti degli eventi in paese", "Sabota almeno una volta la Puglia", 1500, 2, 2));
                        break;
                    case 15:
                        tasks.Add(new Task("Avvia la costruzione di negozi di elettronica", "Sabota almeno una volta la Basilicata", 1500, 3, 2));
                        break;
                    case 16:
                        tasks.Add(new Task("Spruzza il diserbante sulle piante", "Sabota almeno una volta la Calabria", 1500, 4, 2));
                        break;
                    case 17:
                        tasks.Add(new Task("Predisponi nuovi covi nascosti della Mafia", "Sabota almeno una volta la Sicilia", 1500, 5, 2));
                        break;
                }
            }
        }
    }
}
