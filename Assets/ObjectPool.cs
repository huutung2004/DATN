using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Queue<T> objects = new Queue<T>();
    private Transform parent;
    public ObjectPool(T prefab, int initSize, Transform parent)
    {
        this.parent = parent;
        this.prefab = prefab;
        for (int i = 0; i < initSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
    }
    public T Get()
    {
        if (objects.Count == 0)
        {
            T obj = GameObject.Instantiate(prefab,parent);
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
        T pooledObject = objects.Dequeue();
        pooledObject.gameObject.SetActive(true);
        return pooledObject;
    }
    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        objects.Enqueue(obj);
    }

}