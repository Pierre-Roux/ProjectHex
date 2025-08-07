using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptExecutionOrderEdit : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Reset");
        ActionSystem.Instance.ResetActions();
    }

}
