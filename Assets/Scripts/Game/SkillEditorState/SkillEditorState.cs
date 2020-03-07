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
        private SkillSystem skillSystem;
        private FaceSystem faceSystem;
        private DirectionMoveSystem directionMoveSystem;
        private GravitySystem gravitySystem;
        private JumpSystem jumpSystem;
        private StepMoveSystem stepMoveSystem;
        private ForbidSystem forbidSystem;
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
            skillSystem = world.GetOrCreateSystem<SkillSystem>();
            directionMoveSystem = world.GetOrCreateSystem<DirectionMoveSystem>();
            gravitySystem = world.GetOrCreateSystem<GravitySystem>();
            jumpSystem = world.GetOrCreateSystem<JumpSystem>();
            stepMoveSystem = world.GetOrCreateSystem<StepMoveSystem>();
            forbidSystem = world.GetOrCreateSystem<ForbidSystem>();

            var entity = world.CreateEntity();

            var inputComponent = world.GetSingletonComponent<InputComponent>();
            inputComponent.entity = entity;

            var forbidComponent = world.AddComponentOnce<ForbidComponent>(entity);
            forbidComponent.forbiddance = forbidSystem.AddForbiddance(forbidComponent, "ForbidSystem");

            var prefabComponent = prefabSystem.AddPrefabComponent(entity);
            var mainPlayerGO = GameStarter.Instance.mainPlayer;
            var bornPos = mainPlayerGO.transform.position;
            prefabComponent.gameObject.AddChildToParent(mainPlayerGO);
            prefabComponent.gameObject.SetLayerRecursive(LayerDefine.PlayerInt);
           
            var transformComponent = transformSystem.AddTransformComponent(entity);
            bornPos.y = mapSystem.GetGroundInfo(bornPos).point.y;
            transformComponent.position = bornPos;

            var stepMoveComponent = world.AddComponentOnce<StepMoveComponent>(entity);
            var directionMoveComponent = world.AddComponentOnce<DirectionMoveComponent>(entity);
            directionMoveComponent.desiredSpeed = 5;
            var gravityComponent = world.AddComponentOnce<GravityComponent>(entity);
            var jumpComponent = world.AddComponentOnce<JumpComponent>(entity);
            jumpComponent.desiredJumpSpeed = 8;
            jumpComponent.startJumpHeight = 1;
            var groundComponent = world.AddComponentOnce<GroundComponent>(entity);

            var faceComponent = world.AddComponentOnce<FaceComponent>(entity);
            faceComponent.desiredDegreeSpeed = 720;

            cameraSystem.SetFollow(entity, GameStarter.Instance.virtualCamera);

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
        }

        protected override void OnUpdate()
        {
            physicSystem.Update();

            pcInputSystem.Update();
            inputSystem.Update();
            
            mapSystem.Update();
            skillSystem.Update();

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
        }
    }
}