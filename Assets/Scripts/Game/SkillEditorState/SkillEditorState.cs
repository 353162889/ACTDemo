using System;
using System.Diagnostics;
using Cinemachine;
using Framework;
using Unity.Entities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{
    public class SkillEditorState : StateBase
    {
        private World world;
        private InputSystem inputSystem;
        private PCInputSystem pcInputSystem;
        private TransformSystem transformSystem;
        private PrefabSystem prefabSystem;
        private CameraSystem cameraSystem;
        private PhysicSystem physicSystem;
        private MapSystem mapSystem;
        private AnimationSystem animationSystem;
        private ComboSystem comboSystem;
        private CacheSkillSystem cacheSkillSystem;
        private SkillSystem skillSystem;
        private FaceSystem faceSystem;
        private DirectionMoveSystem directionMoveSystem;
        private GravitySystem gravitySystem;
        private JumpSystem jumpSystem;
        private StepMoveSystem stepMoveSystem;
        private ForbidSystem forbidSystem;
        private AvatarSystem avatarSystem;
        private BuffSystem buffSystem;
        private BuffStateSystem buffStateSystem;
        protected override void OnEnter()
        {
            world = new GameObjectWorld("Test");
            inputSystem = world.GetOrCreateSystem<InputSystem>();
            pcInputSystem = world.GetOrCreateSystem<PCInputSystem>();
            transformSystem = world.GetOrCreateSystem<TransformSystem>();
            faceSystem = world.GetOrCreateSystem<FaceSystem>();
            prefabSystem = world.GetOrCreateSystem<PrefabSystem>();
            cameraSystem = world.GetOrCreateSystem<CameraSystem>();
            physicSystem = world.GetOrCreateSystem<PhysicSystem>();
            mapSystem = world.GetOrCreateSystem<MapSystem>();
            animationSystem = world.GetOrCreateSystem<AnimationSystem>();
            comboSystem = world.GetOrCreateSystem<ComboSystem>();
            cacheSkillSystem = world.GetOrCreateSystem<CacheSkillSystem>();
            skillSystem = world.GetOrCreateSystem<SkillSystem>();
            directionMoveSystem = world.GetOrCreateSystem<DirectionMoveSystem>();
            gravitySystem = world.GetOrCreateSystem<GravitySystem>();
            jumpSystem = world.GetOrCreateSystem<JumpSystem>();
            stepMoveSystem = world.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = world.GetOrCreateSystem<ForbidSystem>();
            avatarSystem = world.GetOrCreateSystem<AvatarSystem>();
            buffStateSystem = world.GetOrCreateSystem<BuffStateSystem>();
            buffSystem = world.GetOrCreateSystem<BuffSystem>();

            var playerEntity = CreatePlayer();
            cameraSystem.SetFollow(playerEntity, GameStarter.Instance.virtualCamera);

            CreateEnemy();
        }

        private Entity CreatePlayer()
        {
            var entity = world.CreateEntity();
            //在GameObject上标识entity
            var gameObjectComponent = world.GetComponent<GameObjectComponent>(entity);
            var entityMono = gameObjectComponent.gameObject.AddComponentOnce<EntityMonoBehaviour>();
            entityMono.entity = entity;

            //输入
            var inputComponent = world.GetSingletonComponent<InputComponent>();
            inputComponent.entity = entity;

            //禁止组件
            var forbidComponent = world.AddComponentOnce<ForbidComponent>(entity);
            forbidComponent.forbiddance = forbidSystem.AddForbiddance(forbidComponent, "ForbidSystem");

            //外显
            var prefabComponent = prefabSystem.AddPrefabComponent(entity);
            var mainPlayerGO = GameStarter.Instance.mainPlayer;
            var bornPos = mainPlayerGO.transform.position;
            prefabComponent.gameObject.AddChildToParent(mainPlayerGO);

            var avatarComponent = world.AddComponentOnce<AvatarComponent>(entity);
            avatarComponent.mountPoint = prefabComponent.GetComponentInChildren<MountPointCollector>();

            //位置
            var transformComponent = transformSystem.AddTransformComponent(entity);
            float y = mapSystem.GetGroundInfo(bornPos).point.y;
            if (transformComponent.position.y < y)
            {
                bornPos.y = y;
            }
            transformComponent.position = bornPos;

            //移动
            var stepMoveComponent = world.AddComponentOnce<StepMoveComponent>(entity);
            var directionMoveComponent = world.AddComponentOnce<DirectionMoveComponent>(entity);
            directionMoveComponent.desiredSpeed = 5;
            var gravityComponent = world.AddComponentOnce<GravityComponent>(entity);
            var jumpComponent = world.AddComponentOnce<JumpComponent>(entity);
            jumpComponent.forbidance = forbidSystem.AddForbiddance(forbidComponent, "JumpSystem");
            jumpComponent.desiredHeight = 3;
            jumpComponent.endJumpGroundHeight = 0.5f;
            //跳跃动画需要3帧蓄力
            jumpComponent.startJumpWaitTime = 3 / 30f;
            var groundComponent = world.AddComponentOnce<GroundComponent>(entity);
            //面向
            var faceComponent = world.AddComponentOnce<FaceComponent>(entity);
            faceComponent.desiredDegreeSpeed = 720;

            var physicComponent = world.AddComponentOnce<PhysicComponent>(entity);
            physicComponent.rigidbody = prefabComponent.gameObject.AddComponentOnce<Rigidbody>();
            physicComponent.collisionListener = prefabComponent.gameObject.AddComponentOnce<CollisionListener>();
            physicComponent.rigidbody.isKinematic = false;
            physicComponent.rigidbody.useGravity = false;
            physicComponent.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            physicComponent.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            var goAttackBoxParent = new GameObject();
            prefabComponent.gameObject.AddChildToParent(goAttackBoxParent, "AttackBoxParent");
            physicComponent.attackBoxParent = goAttackBoxParent.transform;
            var rigidBody = physicComponent.attackBoxParent.gameObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;

            var animationComponent = world.AddComponentOnce<AnimationComponent>(entity);
            animationComponent.animator = prefabComponent.transform.GetComponentInChildren<Animator>();

            var comboComponent = world.AddComponentOnce<ComboComponent>(entity);
            var cacheSkillComponent = world.AddComponentOnce<CacheSkillComponent>(entity);
            var skillComponent = world.AddComponentOnce<SkillComponent>(entity);
            var buffStateComponent = world.AddComponentOnce<BuffStateComponent>(entity);
            var buffComponent = world.AddComponentOnce<BuffComponent>(entity);

            return entity;
        }

        private Entity CreateEnemy()
        {
            var entity = world.CreateEntity();
            var gameObjectComponent = world.GetComponent<GameObjectComponent>(entity);
            var entityMono = gameObjectComponent.gameObject.AddComponentOnce<EntityMonoBehaviour>();
            entityMono.entity = entity;

            //禁止组件
            var forbidComponent = world.AddComponentOnce<ForbidComponent>(entity);
            forbidComponent.forbiddance = forbidSystem.AddForbiddance(forbidComponent, "ForbidSystem");

            //外显
            var prefabComponent = prefabSystem.AddPrefabComponent(entity);
            var enemyGO = GameStarter.Instance.enemy;
            var bornPos = enemyGO.transform.position;
            prefabComponent.gameObject.AddChildToParent(enemyGO);

            var avatarComponent = world.AddComponentOnce<AvatarComponent>(entity);
            avatarComponent.mountPoint = prefabComponent.GetComponentInChildren<MountPointCollector>();

            //位置
            var transformComponent = transformSystem.AddTransformComponent(entity);
            float y = mapSystem.GetGroundInfo(bornPos).point.y;
            if (transformComponent.position.y < y)
            {
                bornPos.y = y;
            }
            transformComponent.position = bornPos;
            //移动
            var stepMoveComponent = world.AddComponentOnce<StepMoveComponent>(entity);
            var gravityComponent = world.AddComponentOnce<GravityComponent>(entity);
            var groundComponent = world.AddComponentOnce<GroundComponent>(entity);
            //面向
            var faceComponent = world.AddComponentOnce<FaceComponent>(entity);
            faceComponent.desiredDegreeSpeed = 720;

            var physicComponent = world.AddComponentOnce<PhysicComponent>(entity);
            physicComponent.rigidbody = prefabComponent.gameObject.AddComponentOnce<Rigidbody>();
            physicComponent.collisionListener = prefabComponent.gameObject.AddComponentOnce<CollisionListener>();
            physicComponent.rigidbody.isKinematic = false;
            physicComponent.rigidbody.useGravity = false;
            physicComponent.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            physicComponent.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var animationComponent = world.AddComponentOnce<AnimationComponent>(entity);
            animationComponent.animator = prefabComponent.transform.GetComponentInChildren<Animator>();

            var skillComponent = world.AddComponentOnce<SkillComponent>(entity);
            var buffStateComponent = world.AddComponentOnce<BuffStateComponent>(entity);
            var buffComponent = world.AddComponentOnce<BuffComponent>(entity);

            return entity;
        }

        protected override void OnUpdate()
        {
            physicSystem.Update();

            pcInputSystem.Update();
            inputSystem.Update();
            
            mapSystem.Update();
            comboSystem.Update();
            cacheSkillSystem.Update();
            skillSystem.Update();
            buffSystem.Update();
            buffStateSystem.Update();

            directionMoveSystem.Update();
            jumpSystem.Update();
            gravitySystem.Update();
            stepMoveSystem.Update();
            faceSystem.Update();
            jumpSystem.UpdateState();
            transformSystem.Update();

            forbidSystem.Update();

            prefabSystem.Update();
            animationSystem.Update();
            cameraSystem.Update();
        }

        protected override void OnExit()
        {
            if (world != null)
            {
                world.Dispose();
            }
            SceneEffectPool.Instance.Clear();
        }
    }
}