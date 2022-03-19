using System.Collections.Generic;
using UnityEngine;

namespace FSMFrame
{
    public class StateMachine:State
    {
        public StateMachine(string name) : base(name)
        {
            managerdStates = new Dictionary<string, State>();
            //绑定初始事件
            StateMachineEventBind();
        }
        
        private void StateMachineEventBind()
        {
            OnStateUpdate += CheckCurrentStateTransition;
        }

        #region Manager States

        //被管理的状态
        private Dictionary<string,State> managerdStates;
        //默认状态
        private State defaultState;
        //当前状态
        private State currentState;

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateName"></param>
        public State AddState(string stateName)
        {
            if (IsRun)
            {
                Debug.LogWarning("状态正在运行，不能删除");
                return null;
            }
            if (managerdStates.ContainsKey(stateName))
            {
                return managerdStates[stateName];
            }

            State crtState = new State(stateName);
            managerdStates.Add(stateName,crtState);
            if (managerdStates.Count == 1)
            {
                defaultState = crtState;
            }

            return crtState;
        }
        
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="crtState"></param>
        public void AddState(State crtState)
        {
            if (IsRun)
            {
                Debug.LogWarning("状态正在运行，不能删除");
                return;
            }
            if (managerdStates.ContainsKey(crtState.StateName))
            {
                return;
            }
            
            managerdStates.Add(crtState.StateName,crtState);
            if (managerdStates.Count == 1)
            {
                defaultState = crtState;
            }
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateName"></param>
        public void RemoveState(string stateName)
        {
            if (IsRun)
            {
                Debug.LogWarning("状态正在运行，不能删除");
                return;
            }
            if (managerdStates.ContainsKey(stateName))
            {
                //当前要删除的状态
                State crtState = managerdStates[stateName];
                managerdStates.Remove(stateName);
                //如果当前状态是默认状态
                if (crtState == defaultState)
                {
                    //清除上一次的默认状态
                    defaultState = null;
                    ChooseNewDefaultState();
                }
            }
        }
        
        /// <summary>
        /// 选择新的默认状态
        /// </summary>
        private void ChooseNewDefaultState()
        {
            foreach (var item in managerdStates)
            {
                //遍历到的第一个状态为默认状态
                defaultState = item.Value;
                return;
            }
        }

        #endregion

        #region State Machine Event

        /// <summary>
        /// 状态机的进入
        /// </summary>
        /// <param name="enterEventParameters"></param>
        /// <param name="updateEventParameters"></param>
        public override void EnterState(object[] enterEventParameters, object[] updateEventParameters)
        {
            //先执行当前状态机的进入事件
            base.EnterState(enterEventParameters, updateEventParameters);
            //再执行子状态的进入
            //判断是否有默认状态
            if (defaultState == null)
            {
                return;
            }
            //此时当前状态为默认状态
            currentState = defaultState; 
            //当前状态执行进入事件(进入默认的子状态)
            currentState.EnterState(enterEventParameters, updateEventParameters);
        }

        /// <summary>
        /// 状态机的离开
        /// </summary>
        /// <param name="parameters"></param>
        public override void ExitState(object[] parameters)
        {
            if (currentState != null)
            {
                //当前状态先离开
                currentState.ExitState(parameters);
            }
            //状态机再离开
            base.ExitState(parameters);
        }

        #endregion

        #region State Machine Check State Transition

        /// <summary>
        /// 检测当前状态是否满足过渡条件
        /// </summary>
        private void CheckCurrentStateTransition(object[] objs)
        {
            //检测过渡事件
            string targetState = currentState.CheckTransition();
            //当前状态的过渡事件满足
            if (targetState != null)
            {
                //过渡到新状态
                TransitionToState(targetState);
            }
            
        }
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="targetStateName"></param>
        private void TransitionToState(string targetStateName)
        {
            //要过渡的状态是否被当前状态机所管理
            if (managerdStates.ContainsKey(targetStateName))
            {
                //当前状态要离开
                currentState.ExitState(null);
                //切换当前状态
                currentState = managerdStates[targetStateName];
                //新的状态执行进入
                currentState.EnterState(null,null);
            }
        }

        #endregion
    }
}