using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    [System.Serializable]
    public struct SaveData
    {
        public int level;
    }

    public SaveData data;

    public PlayerSaveData(int i_level)
    {
        data.level = i_level;
    }
}
