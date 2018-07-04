using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_objectPooler : MonoBehaviour {

	[Tooltip("Prefab to spawn/pool.")]
	public GameObject pooledObject;
	[Tooltip("How many objects to cache.")]
	public int pooledAmount = 20;
	[Tooltip("Will allow for the pool amount to grow if needed.")]
	public bool willGrow = true;

	List<GameObject> pooledObjects;

	void Start () {

		pooledObjects = new List<GameObject>();
		for(int i = 0; i < pooledAmount; i ++)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			obj.SetActive(false);
			pooledObjects.Add(obj);

		}

	}
	
	public GameObject GetPooledObject()
	{

		for(int i = 0; i < pooledObjects.Count; i++)
		{
			if(!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		}
		if(willGrow)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			pooledObjects.Add(obj);
			return obj;

		}
		return null;
	}
}
