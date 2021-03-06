﻿using BTCore;
using Framework;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public static class SkillBlackBoardKeys
    {
        public static string ListDamageInfo = "ListDamageInfo";
    }

    public struct SkillTargetInfo
    {
        public Entity target;
        public Vector3 targetDirection;
        public Vector3 targetPosition;

        public void Reset()
        {
            target = Entity.Null;
            targetDirection = Vector3.forward;
            targetPosition = Vector3.zero;
        }
    }

    public enum SkillPhaseType
    {
        Normal = 1,
        Backswing = 2,//后摇
    }

    public class SkillData : IPoolable
    {
        public int skillId;
        public float skillTime;
        public float skillTimeScale;
        public bool enableInputSkill;
        public SkillPhaseType phase;
        public SkillTargetInfo targetInfo;
        public BTBlackBoard blackBoard = new BTBlackBoard();
        public BTExecuteCache executeCache = new BTExecuteCache();
        public Forbiddance forbidance;
        public bool isBreak;

        private Entity m_cTarget = Entity.Null;
        public Entity target
        {
            get { return m_cTarget; }
        }

        private Vector3 m_cTargetDirection = Vector3.forward;

        public Vector3 targetDirection
        {
            get { return m_cTargetDirection; }
        }

        private Vector3 m_cTargetPosition = Vector3.zero;
        public Vector3 targetPosition
        {
            get { return m_cTargetPosition; }
        }

        public void SetTargetInfo(Entity target, Vector3 targetDirection, Vector3 targetPosition)
        {
            this.m_cTarget = target;
            this.m_cTargetPosition = targetPosition;
            this.m_cTargetDirection = targetDirection;
        }

        public void Reset()
        {
            blackBoard.Clear();
            executeCache.Clear();
            this.m_cTarget = Entity.Null;
            this.m_cTargetPosition = Vector3.zero;
            this.m_cTargetDirection = Vector3.forward;
            enableInputSkill = false;
            isBreak = false;
            if (this.forbidance != null)
            {
                this.forbidance.Reset();
            }

            this.forbidance = null;
            targetInfo.Reset();
        }
    }
}