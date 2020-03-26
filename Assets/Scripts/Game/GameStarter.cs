using UnityEngine;
using System.Collections;
using Framework;
using System;
using System.Collections.Generic;
using Cinemachine;
using GameData;
using Unity.Entities;

namespace Game
{
    public class GameStarter : SingletonMonoBehaviour<GameStarter>
    {
        public GameObject mainPlayer;
        public GameObject enemy;
        public CinemachineVirtualCamera virtualCamera;

        public StateContainerBase GameGlobalState { get; private set; }


        public static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();
#endif

        }

        private void Awake()
        {
            CLog.Log("GameStarter.Awake"); 
            base.Awake();
            Application.runInBackground = true;
            Screen.orientation = ScreenOrientation.Portrait;
            GameObject.DontDestroyOnLoad(gameObject);
           
        }

        // Use this for initialization
        void Start()
        {
            InitSingleton();
            ReloadCfg(() =>
            {
                InitState();
            });
        }

        public void ReloadCfg(Action callback)
        {
            StopCoroutine("CoroutineLoadCfg");
            StartCoroutine("CoroutineLoadCfg", callback);
        }

        private IEnumerator CoroutineLoadCfg(Action callback)
        {
            ResCfgSys.Instance.Dispose();
            ResCfgSys.Instance.LoadResCfgs("Config/Data", null);
            while (!ResCfgSys.Instance.IsFinish()) yield return null;
            SkillManager.Instance.LoadCfgs(null);
            while (!SkillManager.Instance.IsFinish()) yield return null;
            BuffManager.Instance.LoadCfgs(null);
            while (!BuffManager.Instance.IsFinish()) yield return null;
            callback.Invoke();
            yield return null;
        }

        protected void InitState()
        {
            GameGlobalState = (StateContainerBase)CreateState(GameStateCfg.GameState);
            GameGlobalState._OnEnter();
        }

        protected StateBase CreateState(GameStateData stateData)
        {
            StateBase state = Activator.CreateInstance(stateData.mClassType) as StateBase;
            if (stateData.mSubStateData != null)
            {
                StateContainerBase stateContainer = (StateContainerBase)state;
                for (int i = 0; i < stateData.mSubStateData.Length; i++)
                {
                    var data = stateData.mSubStateData[i];
                    StateBase subState = CreateState(data);
                    stateContainer.AddState((int)data.mStateType, subState, data.mDefaultState);
                }
            }
            return state;
        }

        void InitSingleton()
        {
            gameObject.AddComponentOnce<ConsoleLogger>();
            gameObject.AddComponentOnce<ResourceSys>();
            bool directLoadMode = true;
#if !UNITY_EDITOR || BUNDLE_MODE
            directLoadMode = false;
#endif
            ResourceSys.Instance.Init(directLoadMode, "Assets/ResourceEx");
            gameObject.AddComponentOnce<UpdateScheduler>();
            gameObject.AddComponentOnce<TouchDispatcher>(); 
            //初始化对象池
            GameObject goPool = new GameObject();
            goPool.name = "GameObjectPool";
            GameObject.DontDestroyOnLoad(goPool);
            goPool.AddComponentOnce<ResourceObjectPool>();
            goPool.AddComponentOnce<PrefabPool>();

            var goSceneEffectPool = new GameObject("SceneEffectPool");
            goSceneEffectPool.AddComponentOnce<SceneEffectPool>();
            GameObject.DontDestroyOnLoad(goSceneEffectPool);

            ResetObjectPool<List<EntityHitInfo>>.Instance.Init(30, list => list.Clear());
            ResetObjectPool<List<DamageInfo>>.Instance.Init(30, lst => lst.Clear());
            var goEntityHit = new GameObject("EntityHitPool");
            GameObject.DontDestroyOnLoad(goEntityHit);
            BehaviourPool<EntityBoxHit>.Instance.Init(30, goEntityHit.transform);

            ResetObjectPool<List<Entity>>.Instance.Init(20, list => list.Clear());

            ResetObjectPool<List<int>>.Instance.Init(50, lst => lst.Clear());

            gameObject.AddComponentOnce<FPSMono>();
        }

        private void Update()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnUpdate();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                ReloadCfg(() => { CLog.Log("重新加载配置完成");});
            }
        }


        private void LateUpdate()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnLateUpdate();
            }
        }

        private void OnDestroy()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnDispose();
                GameGlobalState = null;
            }
        }

        public void OnApplicationQuit()
        {
            if (null != GameGlobalState)
            {
                GameGlobalState._OnExit();
                GameGlobalState._OnDispose();
                GameGlobalState = null;
            }
        }

    }
}