using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    [SerializeField] Mesh defaultBulletMesh;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i=0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Vector3 dir, Bullet bul, Material mat, IngredientType ingData)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        if (tag != "Tornado")
        {
            if (ingData != null)
            {
                objToSpawn.GetComponent<MeshFilter>().mesh = ingData.modelPrefab;
            }
            else
            {
                objToSpawn.GetComponent<MeshFilter>().mesh = defaultBulletMesh;
            }
        }

        if(mat != null)
        {
            objToSpawn.GetComponent<Renderer>().material = mat;
        }

        if (!objToSpawn.GetComponent<BulletMovement>()) 
        { objToSpawn.AddComponent<BulletMovement>(); }

        BulletMovement objMov = objToSpawn.GetComponent<BulletMovement>();
        objMov.isSnowfall = false;
        objMov.returnbullet = false;
        objMov.dir = dir;
        objMov.speed = bul.speed;
        objMov.dam = bul.damage;
        objMov.lifetime = bul.lifespan;
        objToSpawn.transform.localScale = bul.size;
        objToSpawn.transform.position = position;

        if (tag != "Tornado")
        {
            objToSpawn.transform.rotation = rotation;
        }

        if(tag == "Good_Ingredient")
        {
            objMov.isIng = true;
            if (!objToSpawn.GetComponent<PickUp>())
            { objToSpawn.AddComponent<PickUp>();}
            objToSpawn.GetComponent<PickUp>().ingredientType = ingData;
            objToSpawn.transform.localScale = ingData.size;
        }
        else if(tag == "Bad_Ingredient")
        {
            objMov.isIng = true;
            if (!objToSpawn.GetComponent<PickUp>())
            { objToSpawn.AddComponent<PickUp>(); }
            objToSpawn.GetComponent<PickUp>().ingredientType = null;
        }

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject SpawnNonBulletFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }
}
