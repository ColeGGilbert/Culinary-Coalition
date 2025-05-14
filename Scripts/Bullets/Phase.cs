using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPhase", menuName = "ScriptableObjects/Phase", order = 1)]
public class Phase : ScriptableObject
{

    [System.Serializable]
    public struct PhasePattern
    {
        public Pattern pattern;
        public int goodIngredientsToSpawn;
        public bool canSpawnBadIngredients;
    }

    [Header("Patterns for the phase, split into each mini-phase (Each third of the way through satisfaction)")]
    public PhasePattern[] Served0;
    public PhasePattern[] Served1;
    public PhasePattern[] Served2;

    [Space(5)]
    [Header("Patterns for the phase, random or linear?")]
    public bool Random0;
    public bool Random1;
    public bool Random2;

    [Space(5)]
    public PhasePattern Frederico;

    [Space(5)]
    public PhasePattern Tornado;

    [Header("Wave Variable Declarations")]
    [Space(15)]
    public float bulletCountModifier; // Affects how many bullets are spawned in a pattern
    [Range(1, 100)] public int badIngredientSpawnChance; // The chance that a bullet spawns as a bad ingredient as %
    [Tooltip("Shotgun Random Spread for the phase")] public bool randomShotgunSpread;
    [Tooltip("Amount for the shotgun spread to be randomised")][Range(1, 20)] public float maxSpreadAmount = 1f;
    public float snowFallPower;
    [Space(10)]
    public int fredericoWaveGap; // How many waves in between each frederico appearance
    public bool canSpawnFrederico;

    [Space(10)]
    public int tornadoWaveGap; // How many waves in between each tornado appearance
    public bool canSpawnTornado;
    [Space(10)]
    public int goodIngredientWaveGap; // How many waves should we wait before a good ingredient can spawn
    [Tooltip("The delay between each shot is calculated as (firing duration/amount of bullets)")] public float minigunFiringDuration;
    [Tooltip("The delay between each shot is calculated as (firing duration/amount of bullets)")] public float normalShotFiringDuration;
    [Tooltip("The delay between each shot is calculated as (firing duration/amount of bullets)")] public float sniperFiringDuration;

    public Phase(PhasePattern[] serv0, PhasePattern[] serv1, PhasePattern[] serv2, bool random0, bool random1, bool random2)
    {
        Served0 = serv0;
        Served1 = serv1;
        Served2 = serv2;
        Random0 = random0;
        Random1 = random1;
        Random2 = random2;
    }
}
