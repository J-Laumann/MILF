using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FishData : MonoBehaviour
{

    public string fishName, fishDesc, fishHabitat;
    public Sprite fishSprite;
    public Color spriteColor;

    public int maxHealth;
    [HideInInspector]
    public int health;
    public GameObject healthBar;
    float healthbarTimeout;
    public int size;
    float invulnTimer;
    public bool invuln;
    public List<int> baitIDs;
    public float rodDifficulty;

    FloorDweller fd;
    NavMeshAgent nav;
    public Boid boid;
    [HideInInspector]
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        fd = GetComponent<FloorDweller>();
        nav = GetComponent<NavMeshAgent>();
        boid = GetComponent<Boid>();
        id = int.Parse(gameObject.name.Substring(0, 3));
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > healthbarTimeout)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
            healthbarTimeout = Mathf.Infinity;
        }
    }

    public void TakeDamage(int dmg, int weaponSize)
    {
        if (!invuln && weaponSize >= size)
        {
            healthbarTimeout = Time.time + 5f;
            healthBar.transform.parent.gameObject.SetActive(true);
            
            health -= dmg;

            Vector3 newScale = healthBar.transform.localScale;
            newScale.x = (float)health / (float)maxHealth;
            healthBar.transform.localScale = newScale;
            if (health <= 0)
            {
                if (fd)
                    fd.enabled = false;
                if (nav)
                    nav.enabled = false;
                if (boid)
                    boid.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Net")
        {
            if (other.gameObject.tag == "Net" && Time.time > invulnTimer)
            {
                TakeDamage(20, 1);
                invulnTimer = Time.time + 0.5f;
            }

            if (health <= 0)
            {
                Fishventory.CatchFish(int.Parse(gameObject.name.Substring(0, 3)));
                Destroy(gameObject);
            }
        }
    }
}
