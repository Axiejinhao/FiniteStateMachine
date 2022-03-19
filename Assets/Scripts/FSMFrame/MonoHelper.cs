using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace FSMFrame
{
    public class MonoHelper : MonoBehaviour
    {
        class StateUpdateModule
        {
            //状态更新事件
            public Action<object[]> stateUpdateEvent;
            //触发状态更新事件的参数
            public object[] stateUpdateEventParamters;

            public StateUpdateModule(Action<object[]> e, object[] param)
            {
                stateUpdateEvent = e;
                stateUpdateEventParamters = param;
            }
        }

        //状态更新模块字典
        private Dictionary<string, StateUpdateModule> stateUpdateModuleDic;
        //状态更新模块数组
        private StateUpdateModule[] stateUpdateModuleArray;
        public static MonoHelper Instance;
        [Header("更新事件执行的事件间隔")]
        public float invokeInterval = -1;

        private void DicToArray()
        {
            //实例化数组
            stateUpdateModuleArray = new StateUpdateModule[stateUpdateModuleDic.Count];
            int counter = 0;
            foreach (var item in stateUpdateModuleDic)
            {
                //模块事件对象
                stateUpdateModuleArray[counter] = item.Value;
                //计数器递增
                counter++;
            }
        }
        
        /// <summary>
        /// 添加更新事件
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="updateEvent">更新事件</param>
        /// <param name="updateEventParamters">更新事件执行时的参数</param>
        public void AddUpdateEvent(string stateName, Action<object[]> updateEvent, object[] updateEventParamters)
        {
            if (stateUpdateModuleDic.ContainsKey(stateName))
            {
                //更新
                stateUpdateModuleDic[stateName] = new StateUpdateModule(updateEvent,updateEventParamters);
            }
            else
            {
                //添加
                stateUpdateModuleDic.Add(stateName,new StateUpdateModule(updateEvent,updateEventParamters));
            }

            DicToArray();
        }
        
        /// <summary>
        /// 移除更新事件
        /// </summary>
        /// <param name="stateName"></param>
        public void RemoveUpdateEvent(string stateName)
        {
            if (stateUpdateModuleDic.ContainsKey(stateName))
            {
                //移除
                stateUpdateModuleDic.Remove(stateName);
                DicToArray();
            }
        }
        private void Awake()
        {
            Instance = this;
            stateUpdateModuleDic = new Dictionary<string, StateUpdateModule>();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                if (invokeInterval <= 0)
                {
                    //等待一帧
                    yield return 0;
                }
                else
                {
                    yield return new WaitForSeconds(invokeInterval);
                }
                
                //执行事件
                for (int i = 0; i < stateUpdateModuleArray.Length; i++)
                {
                    if (stateUpdateModuleArray[i].stateUpdateEvent != null)
                    {
                        //调用每个字典中的参数,并传递参数
                        stateUpdateModuleArray[i].stateUpdateEvent(stateUpdateModuleArray[i].stateUpdateEventParamters);
                    }
                }
                
                // foreach (var updateModule in stateUpdateModuleDic)
                // {
                //     if (updateModule.Value.stateUpdateEvent != null)
                //     {
                //         //调用每个字典中的事件
                //         updateModule.Value.stateUpdateEvent(updateModule.Value.stateUpdateEventParamters);
                //     }
                // }
            }
        }

        private void Update()
        {

        }

    }

}
