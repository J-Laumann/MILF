using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineController : MonoBehaviour
{

    public float speed, rotSpeed;
    public Rigidbody rb;

    public bool paused;

    public GameObject NPCUI, NPCButtons;

    // 0 = None, 1 = Succ, 2 = Net, 3 = Boost
    public int attachedTool = 0;
    public Transform toolsParent;
    float swapTimer;

    public static SubmarineController sub;
    public ScubaController scuba;

    public GameObject tpCamera, fpCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sub = this;

        fpCamera.SetActive(true);
        tpCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y > 0)
            rb.useGravity = true;
        else
            rb.useGravity = false;

        if (paused)
            return;

        float curSpeed = speed;
        if (attachedTool == 3)
            curSpeed *= 2;

        //Movement
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");
        float yMove = 0;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(3))
            yMove = -1;
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(4))
            yMove = 1;

        xMove *= rotSpeed;
        yMove *= curSpeed / 2;
        zMove *= curSpeed;

        Vector3 moveVector = transform.up * yMove + transform.forward * zMove;
        if (transform.position.y > 0)
            moveVector.y = 0;

        rb.AddTorque(transform.up * xMove * Time.deltaTime);
        rb.velocity = moveVector;

        Quaternion newRot = transform.rotation;
        newRot.x = 0;
        newRot.z = 0;
        transform.rotation = newRot;

        //Using tools
        if(attachedTool == 1)
        {
            Succ.succ.effect.SetActive(Input.GetMouseButton(0));
            Succ.succ.isActive = Input.GetMouseButton(0);
        }

        //LEAVING SUB
        if (Input.GetKeyDown(KeyCode.Space))
        {
            scuba.transform.position = transform.position;
            
            scuba.gameObject.SetActive(true);
            rb.isKinematic = true;
            paused = true;
        }

        //Swapping Camera
        if (Input.GetKeyDown(KeyCode.R))
        {
            fpCamera.SetActive(!fpCamera.activeSelf);
            tpCamera.SetActive(!tpCamera.activeSelf);
        }

        //Swapping tools
        if (Time.time > swapTimer)
        {
            float scroll = Input.mouseScrollDelta.y;

            if (Input.GetKeyDown(KeyCode.Q))
                scroll--;
            else if (Input.GetKeyDown(KeyCode.E))
                scroll++;

            if (scroll != 0)
            {
                if (scroll > 0)
                {
                    EquipTool(attachedTool + 1);
                }
                else
                {
                    EquipTool(attachedTool - 1);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "NPC")
        {
            NPC npc = other.gameObject.GetComponent<NPC>();
            if (npc)
            {
                paused = true;
                NPCUI.SetActive(true);
                StartCoroutine(TalkToNPC(npc));
            }
        }
    }

    IEnumerator TalkToNPC(NPC npc)
    {
        NPCButtons.SetActive(false);
        Text speechText = NPCUI.transform.Find("Speech").GetComponent<Text>();
        NPCUI.transform.Find("Image").GetComponent<Image>().sprite = npc.npcSprite;
        NPCUI.transform.Find("Name").GetComponent<Text>().text = npc.npcName;
        if (!npc.hasTalked)
        {
            foreach (string line in npc.npcSpeech)
            {
                speechText.text = line;
                yield return new WaitUntil(delegate { return Input.GetKeyDown(KeyCode.Space); });
                yield return new WaitForEndOfFrame();
            }
            npc.hasTalked = true;
        }
        else if(!npc.completeQuest)
        {
            speechText.text = npc.npcSpeech[npc.npcSpeech.Length - 1];
            if(!npc.quester)
                yield return new WaitUntil(delegate { return Input.GetKeyDown(KeyCode.Space); });
        }
        else
        {
            speechText.text = npc.finishedSpeech;
            yield return new WaitUntil(delegate { return Input.GetKeyDown(KeyCode.Space); });
        }

        if (npc.quester && !npc.completeQuest)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            NPCButtons.SetActive(true);
            Button giveButton = NPCButtons.transform.GetChild(0).GetComponent<Button>();
            Button leaveButton = NPCButtons.transform.GetChild(1).GetComponent<Button>();
            giveButton.onClick.RemoveAllListeners();
            if (Fishventory.InvCount(npc.fetchID) >= npc.fetchAmount)
            {
                giveButton.interactable = true;
                giveButton.onClick.AddListener(delegate
                {
                    NPCButtons.transform.gameObject.SetActive(false);
                    Fishventory.RemoveFromInv(npc.fetchID, npc.fetchAmount);
                    npc.completeQuest = true;
                    Fishventory.CatchMultipleFish(npc.rewardID, npc.rewardAmount);
                    StartCoroutine(TalkToNPC(npc));
                });
            }
            else
            {
                giveButton.interactable = false;
            }
        }
        else
        {
            NPCUI.SetActive(false);
            paused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LeaveNPC()
    {
        NPCButtons.SetActive(false);
        NPCUI.SetActive(false);
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void EquipTool(int tool)
    {
        toolsParent.GetChild(attachedTool).gameObject.SetActive(false);
        attachedTool = tool;
        if (attachedTool < 0)
            attachedTool = toolsParent.childCount - 1;
        else if (attachedTool >= toolsParent.childCount)
            attachedTool = 0;
        toolsParent.GetChild(attachedTool).gameObject.SetActive(true);
        swapTimer = Time.time + 0.2f;
    }
    
}
