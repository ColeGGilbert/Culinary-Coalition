using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPattern", menuName = "ScriptableObjects/Pattern", order = 1)]
public class Pattern : ScriptableObject
{
    public Bullet bulletType;
    public BulletSpawning.Patterns patternType;
    public int amountOfBullets;

    public Pattern(Bullet bullet, int amount, BulletSpawning.Patterns pattern, int goodIngsAmount)
    {
        bulletType = bullet;
        amountOfBullets = amount;
        patternType = pattern;
    }
}
