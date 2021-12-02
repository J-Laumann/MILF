using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    public string npcName;
    public string[] npcSpeech;
    public Sprite npcSprite;

    [HideInInspector]
    public bool hasTalked;

    public bool quester;
    [HideInInspector]
    public bool completeQuest;
    public int fetchID, fetchAmount, rewardID, rewardAmount;
    public string finishedSpeech;
}
