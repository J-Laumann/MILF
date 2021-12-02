using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fishventory : MonoBehaviour
{
    static Fishventory fishventory;

    public static List<int> inv, dex;

    public GameObject newFishUI, invUI, invGridParent, fishInvPrefab, newCatchPrefab, catchesParent, journalUI;
    public int currentJournalPage;

    // Start is called before the first frame update
    void Start()
    {
        fishventory = this;
        fishventory.newFishUI.SetActive(true);

        inv = new List<int>();
        dex = new List<int>();

        foreach(GameObject obj in Libraries.fish)
        {
            if(PlayerPrefs.GetInt("Dex" + obj.name, 0) == 1)
            {
                dex.Add(int.Parse(obj.name));
            }
        }

        int invCount = PlayerPrefs.GetInt("InvCount", 0);
        for(int i = 0; i < invCount; i++)
        {
            inv.Add(PlayerPrefs.GetInt("Inv" + i, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            OpenInv();
        else if (Input.GetKeyUp(KeyCode.Tab))
            invUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (journalUI.activeSelf)
            {
                ScubaController.scuba.UnPause();
                if(SubmarineController.sub)
                    SubmarineController.sub.paused = false;
                journalUI.SetActive(false);
            }
            else
            {
                OpenJournal(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (journalUI.activeSelf)
            {
                JournalLeft();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (journalUI.activeSelf)
            {
                JournalRight();
            }
        }
    }

    public static void CatchFish(int f)
    {
        PlayerPrefs.SetInt("Inv" + inv.Count, f);
        inv.Add(f);
        PlayerPrefs.SetInt("InvCount", inv.Count);

        GameObject newCatchUI = Instantiate(fishventory.newCatchPrefab, fishventory.catchesParent.transform);
        Image img = newCatchUI.transform.Find("Icon").GetComponent<Image>();
        img.sprite = Libraries.fish[f].GetComponent<FishData>().fishSprite;
        img.color = Libraries.fish[f].GetComponent<FishData>().spriteColor;
        Destroy(newCatchUI, 1f);
        
        if (!dex.Contains(f))
        {
            fishventory.NewFish(f);
        }
    }

    public void NewFish(int f)
    {
        dex.Add(f);
        newFishUI.GetComponent<Animator>().Play("NewFish");
        FishData fd = Libraries.fish[f].GetComponent<FishData>();
        newFishUI.transform.Find("FishIcon").GetComponent<Image>().sprite = fd.fishSprite;
        newFishUI.transform.Find("FishIcon").GetComponent<Image>().color = fd.spriteColor;
        newFishUI.transform.Find("FishName").GetComponent<Text>().text = fd.fishName;
        PlayerPrefs.SetInt("Dex" + fd.name, 1);
    }

    public void OpenInv()
    {
        invUI.SetActive(true);
        foreach(Transform child in invGridParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach(GameObject obj in Libraries.fish)
        {
            int id = int.Parse(obj.name.Substring(0, 3));
            if (inv.Contains(id))
            {
                GameObject item = Instantiate(fishInvPrefab, invGridParent.transform);
                FishData fd = obj.GetComponent<FishData>();
                Image icon = item.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = fd.fishSprite;
                icon.color = fd.spriteColor;
                item.transform.Find("Name").GetComponent<Text>().text = fd.fishName;
                int count = InvCount(id);
                item.transform.Find("Amount").GetComponent<Text>().text = "" + count;
            }
        }
    }

    public static int InvCount(int id)
    {
        int count = 0;
        foreach (int i in inv)
        {
            if (i == id)
                count++;
        }
        return count;
    }

    public static void RemoveFromInv(int id, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            inv.Remove(id);
        }
    }

    public static void CatchMultipleFish(int id, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            CatchFish(id);
        }
    }

    public void OpenJournal(int id)
    {
        ScubaController.scuba.Pause();
        if (SubmarineController.sub)
            SubmarineController.sub.paused = true;
        journalUI.SetActive(true);
        FishData fd = Libraries.fish[id].GetComponent<FishData>();

        if (dex.Contains(id))
        {
            Image icon = journalUI.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = fd.fishSprite;
            icon.color = fd.spriteColor;
            journalUI.transform.Find("Name").GetComponent<TMP_Text>().text = fd.fishName;
            journalUI.transform.Find("Desc").GetComponent<TMP_Text>().text = fd.fishDesc;
        }
        else
        {
            Image icon = journalUI.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = Libraries.fish[id].GetComponent<FishData>().fishSprite;
            icon.color = Color.black;
            journalUI.transform.Find("Name").GetComponent<TMP_Text>().text = "?????";
            journalUI.transform.Find("Desc").GetComponent<TMP_Text>().text = "??????????";
        }

        journalUI.transform.Find("Loc").GetComponent<TMP_Text>().text = "Habitat: " + fd.fishHabitat;
        string diet = "";
        foreach(int i in fd.baitIDs)
        {
            diet += Libraries.baits[i].GetComponent<Bait>().baitName;
            diet += '\n';
        }
        journalUI.transform.Find("Diet").GetComponent<TMP_Text>().text = diet;
    }

    public void JournalRight()
    {
        currentJournalPage++;
        if(currentJournalPage >= Libraries.fish.Count)
        {
            currentJournalPage = 0;
        }
        OpenJournal(currentJournalPage);
    }

    public void JournalLeft()
    {
        currentJournalPage--;
        if (currentJournalPage < 0)
        {
            currentJournalPage = Libraries.fish.Count - 1;
        }
        OpenJournal(currentJournalPage);
    }
}
