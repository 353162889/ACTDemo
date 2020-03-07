using System.Collections.Generic;
using Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class GameObjectComponent : MonoBehaviour
    {
    }

    public class DataComponent : MonoBehaviour
    {
        public Entity entity { get; private set; }

        public void SetEntity(Entity entity)
        {
            this.entity = entity;
        }
        public virtual void OnDestroy()
        {
        }
    }

    public class GameObjectWorld : World
    {
        public GameObject root { get; }

        public Entity singleEntity { get; }

        public GameObjectWorld(string name) : base(name)
        {
            root = new GameObject(name);
            GameObject.DontDestroyOnLoad(root);
            root.Reset();
            singleEntity = this.CreateEntity();
        }

        public void DestroyWorld()
        {
            GameObject.Destroy(root);
        }

    }

    public static class WorldExtension
    {
        private static void BindGo(World world, Entity entity)
        {
            var go = new GameObject();
            var goWorld = ((GameObjectWorld) world);
            goWorld.root.AddChildToParent(go, "Entity_" + entity);
            var goComponent = go.AddComponent<GameObjectComponent>();
            world.EntityManager.AddComponentObject(entity, goComponent);
        }

        public static Entity CreateEntity(this World world)
        {
            var entity = world.EntityManager.CreateEntity();
            BindGo(world, entity);
            return entity;
        }

        public static Entity CreateEntity(this World world, EntityArchetype archetype)
        {
            var entity = world.EntityManager.CreateEntity(archetype);
            BindGo(world, entity);
            return entity;
        }

        public static Entity CreateEntity(this World world, params ComponentType[] types)
        {
            var entity = world.EntityManager.CreateEntity(types);
            BindGo(world, entity);
            return entity;
        }

        public static void CreateEntity(this World world, EntityArchetype archetype, NativeArray<Entity> entities)
        {
            world.EntityManager.CreateEntity(archetype, entities);
            foreach (var entity in entities)
            {
                BindGo(world, entity);
            }
        }

        public static T GetComponent<T>(this World world, Entity entity) where T : Component
        {
            if (!world.EntityManager.HasComponent<T>(entity))
            {
                return null;
            }

            return world.EntityManager.GetComponentObject<T>(entity);
        }

        public static T AddComponentOnce<T>(this World world, Entity entity) where T : Component
        {
            var goComponent = world.EntityManager.GetComponentObject<GameObjectComponent>(entity);
            var component = world.GetComponent<T>(entity);
            if (null == component)
            {
                component = goComponent.gameObject.AddComponentOnce<T>();
                if (component is DataComponent)
                {
                    var dataComponent = component as DataComponent;
                    dataComponent.SetEntity(entity);
                }
                world.EntityManager.AddComponentObject(entity, component);
            }

            return component;
        }

        public static T AddSingletonComponent<T>(this World world) where T : Component
        {
            var goWorld = (GameObjectWorld) world;
            return world.AddComponentOnce<T>(goWorld.singleEntity);
        }

        public static T GetSingletonComponent<T>(this World world) where T : Component
        {
            var goWorld = (GameObjectWorld) world;
            return world.GetComponent<T>(goWorld.singleEntity);
        }

        public static void DestroyEntity(this World world, Entity entity)
        {
            var component = world.GetComponent<GameObjectComponent>(entity);
            var go = component.gameObject;
            go.SetActive(false);
            world.EntityManager.DestroyEntity(entity);
            var lst = ResetObjectPool<List<DataComponent>>.Instance.GetObject();
            go.GetComponents<DataComponent>(lst);
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].OnDestroy();
            }

            ResetObjectPool<List<DataComponent>>.Instance.SaveObject(lst);
            GameObject.Destroy(go);
        }

    }
}