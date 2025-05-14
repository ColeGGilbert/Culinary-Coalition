using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 1)]
public class Bullet : ScriptableObject
{
    public Vector3 size;
    public float speed;
    public int damage;
    public float lifespan;

    public Bullet(float siz, float spe, int dam, float life)
    {
        size = new Vector3(siz, siz, siz);
        speed = spe;
        damage = dam;
        lifespan = life;
    }
}
