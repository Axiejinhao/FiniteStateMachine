using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace FSMFrame
{
    public class State
    {
        //状态名称
        public string StateName { get; set; }
        //标记当前状态是否已经正在执行
        public bool IsRun { get; set; }
        //当前状态可以切换的其他状态
        private Dictionary<string,Func<bool>> TransitionStates { get; set; }
        public State(string name)
        {
            StateName = name;
            TransitionStates = new Dictionary<string, Func<bool>>();
            //绑定基础回调
            StateBaseEventBind();
        }
        
        
        #region State BaseEventBinding

        private void StateBaseEventBind()
        {
            //进入状态标记
            OnStateEnter += objects => { IsRun = true; };
            OnStateExit += objects => { IsRun = false; };
        }

        #endregion

        #region Transition States Control
        
        /// <summary>
        /// 状态切换的注册
        /// </summary>
        /// <param name="stateName">名称</param>
        /// <param name="condition">条件</param>
        public void RegisterTransitionState(string stateName,Func<bool> condition)
        {
            if (!TransitionStates.ContainsKey(stateName))
            {
                //添加
                TransitionStates.Add(stateName,condition);
            }
            else
            {
                //更新
                TransitionStates[stateName] = condition;
            }
        }
        
        /// <summary>
        /// 取消注册的状态切换
        /// </summary>
        /// <param name="stateName"></param>
        public void UnRegisterTransitionState(string stateName)
        {
            if (TransitionStates.ContainsKey(stateName))
            {
                //移除
                TransitionStates.Remove(stateName);
            }
        }
        
        #endregion

        #region State Machine Event
        
        // 状态进入事件
        public event Action<object[]> OnStateEnter;
        // 状态更新事件[在状态过程中持续调用]
        public event Action<object[]> OnStateUpdate;
        // 离开状态事件
        public event Action<object[]> OnStateExit;

        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void EnterState(object[] enterEventParameters,object[] updateEventParameters)
        {
            
            if (OnStateEnter != null)
            {
                OnStateEnter(enterEventParameters);
            }
            //绑定当前状态的更新事件
            MonoHelper.Instance.AddUpdateEvent(StateName,OnStateUpdate,updateEventParameters);
        }

        /// <summary>
        /// 离开状态
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void ExitState(object[] parameters)
        {
            //解除绑定
            MonoHelper.Instance.RemoveUpdateEvent(StateName);
            if (OnStateExit != null)
            {
                OnStateExit(parameters);
            }
        }
        
        #endregion

        #region Check Transition

        /// <summary>
        /// 检测状态过渡
        /// </summary>
        /// <returns></returns>
        public string CheckTransition()
        {
            foreach (var item in TransitionStates)
            {
                //执行判断条件
                if (item.Value())
                {
                    //条件满足
                    return item.Key;
                }
            }

            return null;
        }

        #endregion

    }
}
