using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProceduralObject : MonoBehaviour
{
    public string objectName;
    public string ongoingScene;
    public List<GameObject> gameObjectsList;

    public delegate void ObjectSelectEvent(string objectname);

    public static ObjectSelectEvent objectSelectEvent;
    private void OnMouseDown()
    {
        objectSelectEvent?.Invoke(objectName);
    }

    public void ChangingObject(int order)
    {
        foreach (var VARIABLE in gameObjectsList)
        {
            VARIABLE.SetActive(false);
        }
        gameObjectsList[order].SetActive(true);
    }
}
