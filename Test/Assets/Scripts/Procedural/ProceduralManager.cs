using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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


    public List<string> wrongOptionList;
    
    
    
    private string assetName = "Kitchen";
    private string bundleName = "Test";

    IEnumerator Start()
    {
        var assetBundleLoadRequest =
            AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));
        
        yield return assetBundleLoadRequest;
        
        var localAssetBundle = assetBundleLoadRequest.assetBundle;
        
        if (localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }
        
        var assetLoadRequest = localAssetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetLoadRequest;
        
        GameObject asset = (GameObject)assetLoadRequest.asset;
        
        Instantiate(asset);
        
        localAssetBundle.Unload(false);

        


        _object = FindObjectsOfType<ProceduralObject>(true).ToList();
        proceduralQueue = new Queue<ProceduralStep>(_steps);

        ProceduralObject.objectSelectEvent += SelectObject;

        yield return null;

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
        
        selectedObject = GetProceduralObjectWithName(objectname).gameObject;
        DestroyButton();
        if (String.Equals(proceduralQueue.Peek().objectName, objectname))
        {
            SetButton();
        }
        
        else
        {
            SetButtonSalah();

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
    public void SetButtonSalah()
    {
        for (int i = 0; i < wrongOptionList.Count; i++)
        {
            GameObject temp = Instantiate(buttonPrefab, _canvas);
            temp.GetComponentInChildren<Text>().text = wrongOptionList[i];
        }
    }

    public void DestroyButton()
    {
        foreach (Transform VARIABLE in _canvas)
        {
            Destroy(VARIABLE.gameObject);
        }
    }

    private Coroutine runningScenario;
    public void RightAnswer()
    {
        runningScenario=StartCoroutine(RightAnswerCoroutine());
    }
    
    IEnumerator RightAnswerCoroutine()
    {
        isBusy = true;
        bool isWait = false;

        DestroyButton();
        ProceduralStep tempstep = proceduralQueue.Dequeue();
        
        foreach (var VARIABLE in tempstep._doSomethingList)
        {
            selectedObject = null;

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
                case DoSomethingEnum.RandomEvent:
                    isWait = true;
                    RandomEvent(delegate { completeevent?.Invoke(); });
                    break;
                case DoSomethingEnum.Wait:
                    isWait = true;
                    float timer = VARIABLE.waitTime;
                    while (timer>0)
                    {
                        timer -= Time.deltaTime;
                        Debug.Log(timer);
                        yield return null;
                    }
                    completeevent?.Invoke();
                    break;
            
                case DoSomethingEnum.HideObject:
                    foreach (var selectedobject in VARIABLE.listSelectObject)
                    {
                        GameObject temp = GetProceduralObjectWithName(selectedobject).gameObject;
                        temp.SetActive(false);
                    }
                    break;
                case DoSomethingEnum.ShowObject:
                    foreach (var selectedobject in VARIABLE.listSelectObject)
                    {
                        GameObject temp = GetProceduralObjectWithName(selectedobject).gameObject;
                        temp.SetActive(true);
                    }
                    break;
                case DoSomethingEnum.ChangePositionOther:

                    ChangingPosition(GetProceduralObjectWithName(VARIABLE.otherObject).gameObject,VARIABLE.target,completeevent);
                    
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

    public void RandomEvent(Action isComplete)
    {
        int temp = Random.Range(0, 2);
        
        if (temp == 0)
        {
            
            StopCoroutine(runningScenario);
            isBusy = false;
            return;
        }
        
        proceduralQueue.Dequeue();
        isComplete?.Invoke();
    }
}
