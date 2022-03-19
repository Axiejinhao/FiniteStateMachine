using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using FSMFrame;

public class UseFSM : MonoBehaviour
{
    //总状态机
    private StateMachine leader;
    [Header("速度参数")]
    [Range(0,10)]
    public float speed;
    private void Start()
    {
        //实例化
        leader = new StateMachine("Leader");
        //创建子状态
        State idleState = new State("Idle");
        State walkState = new State("Walk");
        State runState = new State("Run");
        //创建子状态机
        StateMachine locomotionState = new StateMachine("Locomotion");
        //建立状态关系
        locomotionState.AddState(walkState);
        locomotionState.AddState(runState);
        leader.AddState(idleState);
        leader.AddState(locomotionState);
        //添加状态事件
        leader.OnStateEnter += objects => { Debug.Log("Leader Enter"); };
        leader.OnStateUpdate += objects => { Debug.Log("Leader Update"); };
        leader.OnStateExit += objects => { Debug.Log("Leader Exit"); };
        
        locomotionState.OnStateEnter += objects => { Debug.Log("locomotionState Enter"); };
        locomotionState.OnStateUpdate += objects => { Debug.Log("locomotionState Update"); };
        locomotionState.OnStateExit += objects => { Debug.Log("locomotionState Exit"); };
        
        idleState.OnStateEnter += objects => { Debug.Log("idleState Enter"); };
        idleState.OnStateUpdate += objects => { Debug.Log("idleState Update"); };
        idleState.OnStateExit += objects => { Debug.Log("idleState Exit"); };
        
        walkState.OnStateEnter += objects => { Debug.Log("walkState Enter"); };
        walkState.OnStateUpdate += objects => { Debug.Log("walkState Update"); };
        walkState.OnStateExit += objects => { Debug.Log("walkState Exit"); };
        
        runState.OnStateEnter += objects => { Debug.Log("runState Enter"); };
        runState.OnStateUpdate += objects => { Debug.Log("runState Update"); };
        runState.OnStateExit += objects => { Debug.Log("runState Exit"); };
        
        //添加状态的过渡关系
        idleState.RegisterTransitionState("Locomotion", () => { return speed > 1;});
        locomotionState.RegisterTransitionState("Idle", () => { return speed <= 1;});

        walkState.RegisterTransitionState("Run", () => { return speed > 5; });
        runState.RegisterTransitionState("Walk", () => { return speed <= 5; });
        
        leader.EnterState(null,null);
    }
}
