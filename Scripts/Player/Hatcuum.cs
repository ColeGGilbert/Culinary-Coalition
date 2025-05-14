using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hatcuum", menuName = "ScriptableObjects/HatcuumSettings", order = 1)]
public class Hatcuum : ScriptableObject
{
    public float maxDist;
    public float suctionSpeed;
    public int numCharges;
    public float duration;
    public float moveSpeed;
    public float cooldownDur;
    public string[] suctionTags;

    public Hatcuum(float dist, float speed, int charges, float dur, float playerSpeed, float cooldown, string[] tags)
    {
        maxDist = dist;
        suctionSpeed = speed;
        suctionTags = tags;
        numCharges = charges;
        duration = dur;
        moveSpeed = playerSpeed;
        cooldownDur = cooldown;
    }
}
