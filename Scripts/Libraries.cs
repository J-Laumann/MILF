using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaData
{
    public string areaName;
    public Vector3 center, size;

    public int[] fish, fishRarity;

    public AreaData(string _areaName, Vector3 _c, Vector3 _s, int[] _f, int[] _fr)
    {
        areaName = _areaName;
        center = _c;
        size = _s;
        fish = _f;
        fishRarity = _fr;
    }
}

[Serializable]
public class BaitRecipe
{
    public string baitName;
    public GameObject baitPrefab;

    public int[] fish, fishAmount;

    public BaitRecipe(string _name, GameObject _p, int[] _f, int[] _fa)
    {
        baitName = _name;
        baitPrefab = _p;
        fish = _f;
        fishAmount = _fa;
    }
}

public class Libraries : MonoBehaviour
{

    public static List<GameObject> fish;
    public static List<GameObject> baits;

    public AreaData[] areas;
    public BaitRecipe[] baitRecipes;
    public static Libraries library;

    // Start is called before the first frame update
    void Awake()
    {
        fish = new List<GameObject>();
        UnityEngine.Object[] objs = Resources.LoadAll("Fish");
        foreach(UnityEngine.Object obj in objs)
        {
            fish.Add((GameObject)obj);
        }

        baits = new List<GameObject>();
        objs = Resources.LoadAll("Bait");
        foreach (UnityEngine.Object obj in objs)
        {
            baits.Add((GameObject)obj);
        }

        library = this;
    }

    private void Start()
    {
        if (Tutorial.t)
        {
            areas = new AreaData[] { new AreaData("Pound Town", Vector3.zero, new Vector3(1000, 1000, 1000), new int[] { 7 }, new int[] { 1 }) };
            baitRecipes = new BaitRecipe[] { new BaitRecipe("Open Can of Beans", Tutorial.t.tutorial_bait, new int[] { 7 }, new int[] { 1 }) };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AreaData CheckArea(Vector3 pos)
    {
        foreach (AreaData area in areas)
        {
            if(pos.x <= area.center.x + area.size.x / 2 && pos.x >= area.center.x - area.size.x / 2)
            {
                if (pos.y <= area.center.y + area.size.y / 2 && pos.y >= area.center.y - area.size.y / 2)
                {
                    if (pos.z <= area.center.z + area.size.z / 2 && pos.z >= area.center.z - area.size.z / 2)
                    {
                        return area;
                    }
                }
            }
        }
        return null;
    }


}
