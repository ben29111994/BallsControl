using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    [Header("Pool Manager")]
    public List<ObjectPool> ObjectPools = new List<ObjectPool>();
    
    public enum NameObject
    {
        pixelExplode,
        bullet
    }

    [System.Serializable]
    public class ObjectPool
    {
        public Transform parent;
        public int amount;
        public GameObject objectPrefab;
        public NameObject nameObject;

        [HideInInspector]
        public List<GameObject> listObject = new List<GameObject>();
    }

    private void Start()
    {
        GenerateObjectPool();
    }

    private void GenerateObjectPool()
    {
        int count = ObjectPools.Count;

        for(int i = 0; i < count; i++)
        {
            int amount = ObjectPools[i].amount;
            GameObject prefab = ObjectPools[i].objectPrefab;
            Transform parent = ObjectPools[i].parent;

            for(int j = 0; j < amount; j++)
            {
                GameObject objectClone = Instantiate(prefab, parent);
                objectClone.SetActive(false);
                ObjectPools[i].listObject.Add(objectClone);
            }
        }
    }

    public GameObject GetObject(NameObject name)
    {
        int count = ObjectPools.Count;
        ObjectPool objectPool = null;

        for (int i = 0; i < count; i++)
        {
            if (ObjectPools[i].nameObject == name)
            {
                objectPool = ObjectPools[i];
            }
        }

        if (objectPool == null) return null;

        int childCount = objectPool.listObject.Count;

        for (int i = 0; i < childCount; i++)
        {
            GameObject childObject = objectPool.listObject[i].gameObject;
            if (childObject.activeInHierarchy == false)
            {
                return childObject;
            }
        }

        return null;
    }

    public void RefreshItem(NameObject name)
    {
        for (int i = 0; i < ObjectPools.Count; i++)
        {
            if (ObjectPools[i].nameObject == name)
            {
                for (int k = 0; k < ObjectPools[i].parent.childCount; k++)
                {
                    ObjectPools[i].parent.GetChild(k).gameObject.SetActive(false);
                }
            }
        }
    }
}
