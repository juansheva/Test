using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class ProceduralManager : MonoBehaviour
{
    public bool isBusy = false;
    [SerializeField] private List<ProceduralObject> _object;
    public ProceduralObject GetProceduralObjectWithName(string objectname)
    {
        return _object.First(x => x.objectName == objectname);
    }
    [SerializeField] private List<ProceduralStep> _steps;
    
    public Queue<ProceduralStep> proceduralQueue;





    public Transform _canvas;
    public GameObject buttonPrefab;
    
    
    public void Start()
    {
        _object = FindObjectsOfType<ProceduralObject>().ToList();
        proceduralQueue = new Queue<ProceduralStep>(_steps);

        ProceduralObject.objectSelectEvent += SelectObject;

    }

    public void Procedure()
    {
    }

    public GameObject selectedObject;
    public void SelectObject(string objectname)
    {
        if(proceduralQueue.Count<=0)
            return;

        if(isBusy)
            return;
        if(selectedObject==GetProceduralObjectWithName(objectname).gameObject)
            return;
        if (String.Equals(proceduralQueue.Peek().objectName, objectname))
        {
            selectedObject = GetProceduralObjectWithName(objectname).gameObject;
            SetButton();
        }
        else
        {
            Debug.Log("Salah object");

        }
    }

    public void SetButton()
    {
        for (int i = 0; i < proceduralQueue.Peek().option.Count; i++)
        {
            GameObject temp = Instantiate(buttonPrefab, _canvas);
            temp.GetComponentInChildren<Text>().text = proceduralQueue.Peek().option[i];
            if (i == proceduralQueue.Peek().rightOption)
            {
                temp.GetComponent<Button>().onClick.AddListener(RightAnswer);
            }
        }
    }
    public void RightAnswer()
    {
        StartCoroutine(RightAnswerCoroutine());
    }
    
    IEnumerator RightAnswerCoroutine()
    {
        isBusy = true;
        bool isWait = false;
        foreach (Transform VARIABLE in _canvas)
        {
            Destroy(VARIABLE.gameObject);
        }
        ProceduralStep tempstep = proceduralQueue.Dequeue();
        
        foreach (var VARIABLE in tempstep._doSomethingList)
        {

            while (isWait)
            {
                yield return null;
            }

            Action completeevent=null;
            if (VARIABLE.isCutGroup)
            {
                isWait = true;
                completeevent+=delegate { isWait=false; };
            }
            switch (VARIABLE.whatThisDo)
            {
                case DoSomethingEnum.ChangeObject:
                    ProceduralObject tempobject = GetProceduralObjectWithName(tempstep.objectName);
                    tempobject.ChangingObject(1);
                    break;
                
                case DoSomethingEnum.ChangePosition:
                    ChangingPosition(GetProceduralObjectWithName(tempstep.objectName).gameObject,VARIABLE.target,completeevent);
                    break;
                
                case DoSomethingEnum.ChangeRotation:
                    ChangingRotation(GetProceduralObjectWithName(tempstep.objectName).gameObject,VARIABLE.target,completeevent);
                    break;
                case DoSomethingEnum.RotatingAroundObject:
                    float time = 1;
                    while (time > 0)
                    {
                        GetProceduralObjectWithName(tempstep.objectName).gameObject.transform
                            .RotateAround(VARIABLE.target, new Vector3(0, 1, 0), 5);
                        time -= Time.deltaTime;
                        yield return null;
                    }
                    completeevent?.Invoke();
                    break;
            
            }
        }

        isBusy = false;

    }
    
    public void ChangingPosition(GameObject gameobject,Vector3 targetpos,Action completeevent=null)
    {
        gameobject.transform.DOLocalMove(targetpos,1f).OnComplete(delegate { completeevent?.Invoke(); });
    }
    public void ChangingRotation(GameObject gameobject,Vector3 targetrot,Action completeevent=null)
    {
        gameobject.transform.DOLocalRotate(targetrot,1f).OnComplete(delegate { completeevent?.Invoke(); });
    }
}
