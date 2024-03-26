using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProceduralStep
{
    public string objectName;
    public List<string> option;
    public int rightOption;
    public List<DoSomething> _doSomethingList;
}

[Serializable]
public class DoSomething
{
    public bool isCutGroup=false;
    public DoSomethingEnum whatThisDo;

    public Vector3 target;
    public float waitTime;

    public string otherObject;

    public List<string> listSelectObject;

}

public enum DoSomethingEnum
{
    ChangeObject,
    ChangePosition,
    ChangeRotation,
    RotatingAroundObject,
    RandomEvent,
    Wait,
    HideObject,
    ShowObject,
    ChangePositionOther
}
