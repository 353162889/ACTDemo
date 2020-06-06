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
        private CameraLockSystem cameraLockSystem;
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
        private DamageSystem damageSystem;
        private InAirSystem inAirSystem;
        private MoveStateSystem moveStateSystem;
        private PropertySystem propertySystem;
        private PointsMoveSystem pointsMoveSystem;
        private AISystem aiSystem;
        protected override void OnEnter()
        {
            world = new GameObjectWorld("Test");
            World.Active = world;
            inputSystem = world.GetOrCreateSystem<InputSystem>();
            pcInputSystem = world.GetOrCreateSystem<PCInputSystem>();
            transformSystem = world.GetOrCreateSystem<TransformSystem>();
            faceSystem = world.GetOrCreateSystem<FaceSystem>();
            prefabSystem = world.GetOrCreateSystem<PrefabSystem>();
            cameraSystem = world.GetOrCreateSystem<CameraSystem>();
            cameraLockSystem = world.GetOrCreateSystem<CameraLockSystem>();
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
            damageSystem = world.GetOrCreateSystem<DamageSystem>();
            inAirSystem = world.GetOrCreateSystem<InAirSystem>();
            moveStateSystem = world.GetOrCreateSystem<MoveStateSystem>();
            propertySystem = world.GetOrCreateSystem<PropertySystem>();
            pointsMoveSystem = world.GetOrCreateSystem<PointsMoveSystem>();
            aiSystem = world.GetOrCreateSystem<AISystem>();


            var playerEntity = CreatePlayer();
            cameraSystem.SetMainCamera(Camera.main);
            cameraSystem.ResetCameraStrategy(GameStarter.Instance.cameraRoot);
            cameraSystem.SetFollow(playerEntity, CameraStrategy.NormalWalkCamera);
            inputSystem.SetInputEntity(playerEntity);

            //            Cursor.visible = false;
//            Cursor.lockState = CursorLockMode.Locked;

            CreateEnemy();

            ViewSys.Instance.Open("CameraLockView");
        }

        private Entity CreatePlayer()
        {
            var entity = world.CreateEntity();
            //在GameObject上标识entity
            var gameObjectComponent = world.GetComponent<GameObjectComponent>(entity);
            var entityMono = gameObjectComponent.gameObject.AddComponentOnce<EntityMonoBehaviour>();
            entityMono.entity = entity;

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
            var moveStateComponent = world.AddComponentOnce<MoveStateComponent>(entity);
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
            physicComponent.attackColliderParent = goAttackBoxParent.transform;
            var rigidBody = physicComponent.attackColliderParent.gameObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;

            var animationComponent = world.AddComponentOnce<AnimationComponent>(entity);
            animationComponent.animator = prefabComponent.transform.GetComponentInChildren<Animator>();
            animationComponent.parameters = animationComponent.animator.parameters;

            var comboComponent = world.AddComponentOnce<ComboComponent>(entity);
            var cacheSkillComponent = world.AddComponentOnce<CacheSkillComponent>(entity);
            var skillComponent = world.AddComponentOnce<SkillComponent>(entity);
            var buffStateComponent = world.AddComponentOnce<BuffStateComponent>(entity);
            var buffComponent = world.AddComponentOnce<BuffComponent>(entity);
            var buffFloatComponent = world.AddComponentOnce<InAirComponent>(entity);
            var propertyComponent = world.AddComponentOnce<PropertyComponent>(entity);
            propertyComponent.moveSpeed = moveStateSystem.GetMoveDesiredSpeed(moveStateComponent);

            var pointsMoveComponent = world.AddComponentOnce<PointsMoveComponent>(entity);

            var entityCommonInfoComponent = world.AddComponentOnce<EntityCommonInfoComponent>(entity);
            entityCommonInfoComponent.bornPosition = bornPos;
            entityCommonInfoComponent.bornForward = transformComponent.forward;

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
            var moveStateComponent = world.AddComponentOnce<MoveStateComponent>(entity);
            moveStateComponent.walkSpeed = 2;
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
            animationComponent.parameters = animationComponent.animator.parameters;

            var skillComponent = world.AddComponentOnce<SkillComponent>(entity);
            var buffStateComponent = world.AddComponentOnce<BuffStateComponent>(entity);
            var buffComponent = world.AddComponentOnce<BuffComponent>(entity);
            var buffFloatComponent = world.AddComponentOnce<InAirComponent>(entity);
            var propertyComponent = world.AddComponentOnce<PropertyComponent>(entity);
            propertyComponent.moveSpeed = moveStateSystem.GetMoveDesiredSpeed(moveStateComponent);

            var pointsMoveComponent = world.AddComponentOnce<PointsMoveComponent>(entity);

            var aiComponent = world.AddComponentOnce<AIComponent>(entity);
            aiComponent.aiFile = "test_ai";

            var entityCommonInfoComponent = world.AddComponentOnce<EntityCommonInfoComponent>(entity);
            entityCommonInfoComponent.bornPosition = bornPos;
            entityCommonInfoComponent.bornForward = transformComponent.forward;

            return entity;
        }

        protected override void OnUpdate()
        {
            physicSystem.Update();

            pcInputSystem.Update();
            inputSystem.Update();
            
            mapSystem.Update();
            aiSystem.Update();
            comboSystem.Update();
            cacheSkillSystem.Update();
            skillSystem.Update();
            buffSystem.Update();
            buffStateSystem.Update();

            moveStateSystem.Update();
            directionMoveSystem.Update();
            pointsMoveSystem.Update();
            jumpSystem.Update();
            inAirSystem.Update();
            gravitySystem.Update();
            stepMoveSystem.Update();
            faceSystem.Update();
            inAirSystem.UpdateState();
            jumpSystem.UpdateState();
            transformSystem.Update();

            forbidSystem.Update();

            prefabSystem.Update();
            animationSystem.Update();
            cameraSystem.Update();
            cameraLockSystem.Update();
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        protected override void OnDrawGizmos()
        {
            cameraLockSystem.OnDrawGizmos();
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