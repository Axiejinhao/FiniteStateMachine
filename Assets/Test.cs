using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using FSMFrame;
using Object = System.Object;

public class Test : MonoBehaviour 
{
    public State idle = new State("Idle");
    
    private void Start()
    {
        idle.OnStateEnter += objects => { Debug.Log("Idle Enter"); };
        idle.OnStateUpdate += objects => { Debug.Log("Idle Update"); };
        idle.OnStateExit += objects => { Debug.Log("Idle Exit"); };

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            idle.EnterState(null,null);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            idle.ExitState(null);
        }
    }
}
