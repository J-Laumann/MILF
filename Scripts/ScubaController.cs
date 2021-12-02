using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ScubaController : MonoBehaviour
{
    public bool paused;

    //Movement
    public float landSpeed, waterSpeed;
    float moveSpeed;
    float curSpeed;

    public float oxygen;
    public Image airFill;
    public GameObject interactIcon;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    [Range(0.1f, 10f)]
    public float sensMultiplier = 1f;

    //Tools
    public LayerMask rayMask;
    public GameObject model_fishingRod, bobber, model_reel, fishingHelpUI, baitCraftUI, baitCraftPrefab;
    float castStrength, rodTension, hookedFishTimer;
    int currentHookedFish;
    Rigidbody rb_bobber;
    [HideInInspector]
    public bool hasCast, isCasting, hookedFish, plopped;
    Vector3 bobberLocalPos, hookedFishDir;
    LineRenderer rodLine, spearLine;
    IEnumerator fishingQTE;
    public GameObject qteCircle;
    public Image tensionImg;

    public GameObject model_gun, spearTip;
    Rigidbody rb_spearTip;
    Vector3 tipLocalPos;
    public bool hasShot;
    public FishData spearedFish;

    [HideInInspector]
    public Rigidbody rb;
    CapsuleCollider capCol;

    AreaData area;
    public TMP_Text areaText;
    public static ScubaController scuba;
    new public GameObject camera, fishParent;

    AudioSource reelAudio;
    public AudioClip splashSound, snapSound, caughtSound, rodCastSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        Camera.main.transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        Quaternion tempRot = transform.rotation;
        transform.rotation = Quaternion.identity;
        Camera.main.transform.rotation = tempRot;
        scuba = this;
        rb.useGravity = !(transform.position.y > 0);

        rb_bobber = bobber.GetComponent<Rigidbody>();
        bobberLocalPos = bobber.transform.localPosition;
        rodLine = model_fishingRod.GetComponentInChildren<LineRenderer>();
        reelAudio = model_reel.GetComponent<AudioSource>();

        rb_spearTip = spearTip.GetComponent<Rigidbody>();
        tipLocalPos = spearTip.transform.localPosition;
        spearLine = model_gun.GetComponentInChildren<LineRenderer>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        oxygen = 20;
    }

    // Update is called once per frame
    void Update()
    {
        //UI
        if (Input.GetKeyDown(KeyCode.H))
        {
            Pause();
            fishingHelpUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (baitCraftUI.activeSelf)
            {
                CloseBaitCraftingUI();
            }
            else
            {
                OpenBaitCraftingUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(baitCraftUI.activeSelf || fishingHelpUI.activeSelf)
            {
                baitCraftUI.SetActive(false);
                fishingHelpUI.SetActive(false);
                UnPause();
            }
        }

        if (paused)
        {
            return;
        }

        //Oxygen
        if(transform.position.y > -0.5)
        {
            oxygen = Mathf.Clamp(oxygen + Time.deltaTime * 10, 0, 20);
        }
        else
        {
            oxygen = Mathf.Clamp(oxygen - Time.deltaTime, -0.5f, 20);
        }
        airFill.fillAmount = oxygen / 20;

        if (isCasting)
        {
            Tool_Fishing();
            return;
        }


        //If above water
        if (transform.position.y > 0)
        {
            //If swapping from underwater
            if (!rb.useGravity)
            {
                Quaternion tempRot = transform.rotation;
                transform.rotation = Quaternion.identity;
                Camera.main.transform.rotation = tempRot;

                capCol.direction = 1;
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                moveSpeed = landSpeed;
                curSpeed = moveSpeed;                
            }

            //Land Tools
            if (!hasShot && model_gun.activeSelf)
            {
                model_gun.SetActive(false);
                ResetGun();
                model_fishingRod.SetActive(true);
            }
            else if (hasShot)
            {
                Tool_Gun();
            }
            else
            {
                Tool_Fishing();
            }

            LandMove();
            LandLook();
        }
        else
        {
            //If swapping to underwater
            if (rb.useGravity)
            {
                Quaternion tempRot = Camera.main.transform.rotation;
                Camera.main.transform.rotation = Quaternion.identity;
                transform.rotation = tempRot;

                capCol.direction = 2;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.None;
                moveSpeed = waterSpeed;
                curSpeed = moveSpeed;

                //Water Tools
                model_fishingRod.SetActive(false);
                ResetFishingLine();
                model_gun.SetActive(true);

                //Tutorial
                if (Tutorial.t)
                {
                    if (Tutorial.t.tutorial_UI3.activeSelf)
                    {
                        Tutorial.t.tutorial_UI3.SetActive(false);
                        Tutorial.t.tutorial_UI4.SetActive(true);
                    }
                }
            }
            
            WaterMove();
            WaterLook();

            Tool_Gun();
        }

        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, rayMask))
        {
            Collider col = hit.collider;
            if (col.gameObject.tag == "Boat" || col.gameObject.tag == "Radio")
            {
                interactIcon.SetActive(true);
            }
            else
            {
                interactIcon.SetActive(false);
            }
        }
        else
        {
            interactIcon.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, rayMask))
            {
                Collider col = hit.collider;

                if(col.gameObject.tag == "Submarine")
                {
                    SubmarineController sc = col.gameObject.GetComponent<SubmarineController>();
                    sc.scuba = this;
                    sc.enabled = true;
                    sc.rb = sc.GetComponent<Rigidbody>();
                    sc.rb.isKinematic = false;
                    gameObject.SetActive(false);
                }

                //Entering Boat
                if(col.gameObject.tag == "Boat")
                {
                    BoatController bc = col.gameObject.GetComponent<BoatController>();
                    bc.enabled = true;
                    bc.player = gameObject;
                    gameObject.transform.position = bc.seat.transform.position;
                    gameObject.transform.localRotation = Quaternion.identity;
                    bc.camera.SetActive(true);
                    camera.GetComponent<WaterEffects>().SetAboveWater();
                    camera.SetActive(false);
                    rb.isKinematic = true;
                    GetComponent<Collider>().isTrigger = true;
                    transform.parent = bc.seat.transform;
                    interactIcon.SetActive(false);
                    this.enabled = false;
                }

                if(col.gameObject.tag == "Radio")
                {
                    BoatController bc = col.transform.parent.GetComponent<BoatController>();
                    bc.NextRadioSong();
                }
            }
        }

        //Running
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            curSpeed = moveSpeed * 1.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            curSpeed = moveSpeed;
        }
    }

    private float desiredX;
    private void WaterLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier * Time.timeScale;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier * Time.timeScale;

        //Find current look rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    }

    private void LandLook()
    {

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier * Time.timeScale;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier * Time.timeScale;

        //Find current look rotation
        Vector3 rot = Camera.main.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    }

    private void WaterMove()
    {
        float hMove = Input.GetAxis("Horizontal") * curSpeed / 4;
        float vMove = Input.GetAxis("Vertical") * curSpeed / 2;

        rb.velocity = (transform.forward * vMove) + (transform.right * hMove);

        if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector3.up * curSpeed / 3;
        }

        if(oxygen <= 0)
        {
            rb.velocity += Vector3.up * curSpeed;
        }
    }

    private void LandMove()
    {
        float hMove = Input.GetAxis("Horizontal") * curSpeed;
        float vMove = Input.GetAxis("Vertical") * curSpeed;

        Vector3 forwardMove = Camera.main.transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();

        Vector3 newVec = rb.velocity;
        newVec.x = ((forwardMove * vMove) + (Camera.main.transform.right * hMove)).x;
        newVec.z = ((forwardMove * vMove) + (Camera.main.transform.right * hMove)).z;
        rb.velocity = newVec;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            if(Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 0.6f))
            {
                rb.AddForce(Vector3.up * 200);
            }
        }
    }

    void Tool_Fishing()
    {
        float bobDist = Vector3.Distance(bobber.transform.position, transform.position);

        //Cool looking line code
        /**
        rodLine.positionCount = 20;
        rodLine.SetPosition(0, rodLine.transform.position);

        Vector3 fullLine = bobber.transform.position - rodLine.transform.position;
        for(int x = rodLine.positionCount - 1; x > 0; x--)
        {
            //rodLine.SetPosition(x, rodLine.transform.position + (fullLine / (2 * (rodLine.positionCount - x))) + (Vector3.down * (0.3f / x) * bobDist * (1 - rodTension)));
            //rodLine.SetPosition(x, rodLine.transform.position + (fullLine / x) + (Vector3.down * Mathf.Min(x, rodLine.positionCount - x) * 0.01f * bobDist * (1 - rodTension)));
            rodLine.SetPosition(x, rodLine.transform.position + (fullLine / x));
        }

        rodLine.positionCount++;
        */
        


        //Help

        if (!hasCast)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isCasting = true;
            }
            if (isCasting)
            {
                castStrength += Time.deltaTime;
                if (Input.GetMouseButtonUp(0))
                {
                    bobber.layer = 0;
                    rb_bobber.isKinematic = false;
                    bobber.transform.position = camera.transform.position;
                    rb_bobber.AddForce(Mathf.Clamp(castStrength + 4, 4, 7) * Camera.main.transform.forward * 200);
                    rb_bobber.useGravity = true;
                    bobber.transform.parent = null;
                    hasCast = true;
                    AudioSource.PlayClipAtPoint(rodCastSound, transform.position, 0.3f);
                }
            }
        }
        else if(hookedFish)
        {
            Camera.main.transform.LookAt(bobber.transform);
            
            bobber.transform.LookAt(transform);

            rodTension -= Time.deltaTime;
            rodTension = Mathf.Clamp(rodTension, 0f, 1f);
            tensionImg.fillAmount = rodTension;

            if(Time.time > hookedFishTimer)
            {
                if (hookedFishDir.magnitude != 0)
                {
                    hookedFishDir = Vector3.zero;
                }
                else
                {
                    hookedFishDir = bobber.transform.right * Random.Range(-2f, 2f) - ((bobber.transform.forward / 2) * Random.Range(0f, 1f));
                    hookedFishDir *= 0.1f * Libraries.fish[currentHookedFish].GetComponent<FishData>().rodDifficulty;
                }
                hookedFishTimer = Time.time + 1f;
            }

            rb_bobber.velocity += (hookedFishDir);

            //User Control
            if (Input.GetKey(KeyCode.A))
            {
                rodTension += Time.deltaTime / 2;
                rb_bobber.velocity += bobber.transform.right * 4 * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                rodTension += Time.deltaTime / 2;
                rb_bobber.velocity += -bobber.transform.right * 4 * Time.deltaTime;
            }

            if (Input.GetMouseButton(0))
            {
                model_reel.transform.Rotate(Vector3.forward * Time.deltaTime * 720);
                rodTension += Time.deltaTime * 1.5f;
                rb_bobber.velocity += bobber.transform.forward * 4 * Time.deltaTime;
                if(!reelAudio.isPlaying)
                    reelAudio.Play();
            }

            if(bobDist < 1.2f)
            {
                ResetFishingLine();
                Fishventory.CatchFish(currentHookedFish);
                AudioSource.PlayClipAtPoint(caughtSound, transform.position, 0.3f);
                if (Tutorial.t)
                {
                    if (Tutorial.t.tutorial_Walls1.activeSelf)
                    {
                        Tutorial.t.tutorial_Walls1.SetActive(false);
                        Tutorial.t.tutorial_UI2.SetActive(false);
                        Tutorial.t.tutorial_UI3.SetActive(true);
                    }
                }
            }

            if(rodTension > 1.1f)
            {
                AudioSource.PlayClipAtPoint(snapSound, bobber.transform.position);
                ResetFishingLine();
            }
        }

        if (hasCast)
        {
            if (Input.GetMouseButtonDown(1) || transform.position.y < 0)
            {
                ResetFishingLine();
            }
        }

        if(!hookedFish)
            LandLook();

        rodLine.SetPosition(0, rodLine.transform.position);
        rodLine.SetPosition(rodLine.positionCount - 1, bobber.transform.position);
    }

    IEnumerator FishingQTE(float wait)
    {
        yield return new WaitForSeconds(wait);

        qteCircle.SetActive(true);
        qteCircle.transform.localScale = Vector3.one;
        rb_bobber.AddForce(Vector3.down * 100);
        if(hookedFish)
            AudioSource.PlayClipAtPoint(splashSound, bobber.transform.position);

        while (qteCircle.transform.localScale.x > 0 && hookedFish)
        {
            qteCircle.transform.localScale -= Vector3.one * Time.deltaTime;

            if (Input.GetKey(KeyCode.Space))
            {
                qteCircle.transform.localScale = Vector3.zero;
                qteCircle.SetActive(false);
                if (hookedFish)
                    StartCoroutine(FishingQTE(Random.Range(2f, 4f)));
                yield break;
            }

            if (qteCircle.transform.localScale.x <= 0)
            {
                //Failed QTE, Break
                AudioSource.PlayClipAtPoint(snapSound, bobber.transform.position);
                ResetFishingLine();
            }

            yield return new WaitForEndOfFrame();
        }

        qteCircle.SetActive(false);
    }

    public void PlopBobber()
    {
        if (plopped)
            return;

        plopped = true;
        StartCoroutine("HookFish");
    }

    IEnumerator HookFish()
    {
        yield return new WaitForSeconds(Random.Range(3, 6));
        if (hasCast)
        {
            rb_bobber.AddForce(Vector3.down * 100);
            hookedFish = true;
            rodTension = 0;

            CheckArea();
            currentHookedFish = 0;
            if(area != null)
            {
                int totalRoll = 0;
                foreach(int r in area.fishRarity)
                {
                    totalRoll += r;
                }
                int rand = Random.Range(0, totalRoll);
                for (int i = 0; i < area.fishRarity.Length; i++)
                {
                    rand -= area.fishRarity[i];
                    if(rand < 0)
                    {
                        currentHookedFish = area.fish[i];
                        break;
                    }
                }
            }

            if (Tutorial.t)
            {
                if (Tutorial.t.tutorial_UI1.activeSelf)
                {
                    Tutorial.t.tutorial_UI1.SetActive(false);
                    Tutorial.t.tutorial_UI2.SetActive(true);
                }
            }

            StartCoroutine(FishingQTE(0));
        }
    }

    public void ResetFishingLine()
    {
        StopAllCoroutines();
        bobber.transform.parent = model_fishingRod.transform;
        bobber.transform.localPosition = bobberLocalPos;
        rb_bobber.isKinematic = true;
        hasCast = false;
        isCasting = false;
        hookedFish = false;
        rodTension = 0;
        tensionImg.fillAmount = 0;
        qteCircle.SetActive(false);
        plopped = false;
        areaText.transform.parent.gameObject.SetActive(false);
        bobber.layer = 10;
    }

    void Tool_Gun()
    {
        if (!hasShot)
        {
            if (Input.GetMouseButtonUp(0))
            {
                spearTip.layer = 0;
                rb_spearTip.isKinematic = false;
                spearTip.transform.position = camera.transform.position + camera.transform.forward;
                rb_spearTip.AddForce(Camera.main.transform.forward * 1500);
                rb_spearTip.useGravity = true;
                spearTip.transform.parent = null;
                hasShot = true;
            }
        }
        else
        {

            if (Input.GetMouseButton(0))
            {
                rb_spearTip.useGravity = false;
                rb_spearTip.velocity = Vector3.Normalize(transform.position - spearTip.transform.position) * 5;
            }

            if(Vector3.Distance(transform.position, spearTip.transform.position) <= 0.5f)
            {
                if (spearedFish)
                {
                    Fishventory.CatchFish(spearedFish.id);
                    AudioSource.PlayClipAtPoint(caughtSound, transform.position, 0.3f);
                    Destroy(spearedFish.gameObject);
                    spearedFish = null;

                    if (Tutorial.t)
                    {
                        if (Tutorial.t.tutorial_UI6.activeSelf)
                        {
                            Tutorial.t.tutorial_UI6.SetActive(false);
                            Tutorial.t.tutorial_UI7.SetActive(true);
                            Invoke("StartGame", 5);
                        }
                    }
                }
                ResetGun();
            }
            else if(Vector3.Distance(transform.position, spearTip.transform.position) >= 70 || Input.GetMouseButtonUp(1))
            { 
                ResetGun();
            }
        }

        spearLine.SetPosition(0, spearLine.transform.position);
        spearLine.SetPosition(spearLine.positionCount - 1, spearTip.transform.position);
    }

    public void ResetGun()
    {
        if (spearedFish)
        {
            Boid b = spearedFish.gameObject.GetComponent<Boid>();
            if (b)
                b.enabled = true;
            FloorDweller floor = spearedFish.gameObject.GetComponent<FloorDweller>();
            if (floor)
                floor.enabled = true;
            NavMeshAgent nav = spearedFish.gameObject.GetComponent<NavMeshAgent>();
            if (nav)
                nav.enabled = true;

            spearedFish.transform.parent = fishParent.transform;
            spearedFish = null;
        }
        spearTip.transform.parent = model_gun.transform;
        spearTip.transform.localPosition = tipLocalPos;
        spearTip.transform.localRotation = Quaternion.identity;
        rb_spearTip.isKinematic = true;
        hasShot = false;
        spearTip.layer = 10;
    }

    public void UnPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        paused = true;
    }

    void CheckArea()
    {
        AreaData newArea = Libraries.library.CheckArea(transform.position);
        if (newArea != null)
        {
            areaText.transform.parent.gameObject.SetActive(true);
            areaText.text = newArea.areaName;
        }
        area = newArea;
    }

    void OpenBaitCraftingUI()
    {
        Pause();
        baitCraftUI.SetActive(true);
        Transform gridParent = baitCraftUI.GetComponentInChildren<GridLayoutGroup>().transform;
        foreach(Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        foreach(BaitRecipe br in Libraries.library.baitRecipes)
        {
            GameObject newBait = Instantiate(baitCraftPrefab, gridParent);
            newBait.transform.Find("Name").GetComponent<TMP_Text>().text = br.baitName;
            string ingreds = "";
            bool flag = true;
            for (int i = 0; i < br.fish.Length; i++)
            {
                ingreds += Libraries.fish[br.fish[i]].GetComponent<FishData>().fishName;
                ingreds += " x" + br.fishAmount[i];
                ingreds += '\n';
                if (Fishventory.InvCount(br.fish[i]) < br.fishAmount[i])
                    flag = false;
            }
            newBait.transform.Find("Ingreds").GetComponent<TMP_Text>().text = ingreds;

            Button button = newBait.GetComponent<Button>();
            button.interactable = flag;
            button.onClick.AddListener(delegate { CraftBait(br); });
            
        }
    }

    void CloseBaitCraftingUI()
    {
        baitCraftUI.SetActive(false);
        UnPause();
    }

    void CraftBait(BaitRecipe br)
    {
        for(int i = 0; i < br.fish.Length; i++)
        {
            Fishventory.RemoveFromInv(br.fish[i], br.fishAmount[i]);
        }
        GameObject baitObj = Instantiate(br.baitPrefab, camera.transform.position, camera.transform.rotation);
        baitObj.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 500);
        CloseBaitCraftingUI();

        if (Tutorial.t)
        {
            if (Tutorial.t.tutorial_UI4.activeSelf)
            {
                Tutorial.t.tutorial_UI4.SetActive(false);
                Tutorial.t.tutorial_UI5.SetActive(true);
            }
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
