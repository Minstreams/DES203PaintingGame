using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// Gameplay System. Provides global settings for gameplay.
    /// </summary>
    public class GameplaySystem : SubSystem<GameplaySystemSetting>
    {
        // Your code here


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
        }
        static void OnGameStart()
        {
            // 在System场景加载后调用
            var @default = 0;
            var living = LayerMask.NameToLayer("Living");
            var attackable = LayerMask.NameToLayer("Attackable");
            var shield = LayerMask.NameToLayer("Shield");
            var projectile = LayerMask.NameToLayer("Projectile");
            var ground = LayerMask.NameToLayer("Ground");
            var invisibleWall = LayerMask.NameToLayer("InvisibleWall");
            Physics.IgnoreLayerCollision(living, attackable);
            Physics.IgnoreLayerCollision(shield, @default);
            Physics.IgnoreLayerCollision(shield, attackable);
            Physics.IgnoreLayerCollision(shield, living);
            Physics.IgnoreLayerCollision(shield, ground);
            Physics.IgnoreLayerCollision(shield, invisibleWall);
            Physics.IgnoreLayerCollision(projectile, invisibleWall);

            // Journal
            journalUnlocked = new bool[Setting.journalPages.Length];
            journalUnlocked[0] = true;
            journalUnlocked[1] = true;
        }



        public static PlayerAvatarController CurrentPlayer { get; set; } = null;
        public static CameraController CurrentCamera { get; set; } = null;
        public static bool IsPaused { get; set; } = false;


        public static float CalculateCameraPointWeight(float distance)
        {
            return 1 / (distance + 0.1f);
        }


        // Combat
        static readonly HashSet<GAttackable> attackedObjects = new HashSet<GAttackable>();
        static readonly int attackMask = 1 << LayerMask.NameToLayer("Attackable");
        public static void GenerateDamageLine(ref List<Vector3> attackPoints, float damage, float power, GAttackable attacker)
        {
            attackedObjects.Clear();
            var ap = attacker.AttackPoint;

            void ProcessHits(RaycastHit[] hits)
            {
                foreach (var h in hits)
                {
                    var attackable = h.collider.GetComponentInParent<GAttackable>();
                    if (attackable != null && attackable != attacker && !attackedObjects.Contains(attackable))
                    {
                        var dir = h.point - ap;
                        dir.y *= Setting.attackPowerRateY;
                        attackable.OnAttacked(damage, power, dir.normalized);

                        attackedObjects.Add(attackable);
                    }
                }
            }

            for (int i = attackPoints.Count - 1; i > 0; --i)
            {
                var delta = attackPoints[i - 1] - attackPoints[i];
                var hits = Physics.RaycastAll(attackPoints[i], delta, delta.magnitude, attackMask, QueryTriggerInteraction.Collide);
                ProcessHits(hits);

                delta = attackPoints[i] - ap;
                hits = Physics.RaycastAll(ap, delta, delta.magnitude, attackMask, QueryTriggerInteraction.Collide);
                ProcessHits(hits);

                Debug.DrawLine(attackPoints[i - 1], attackPoints[i], Color.red, 1);
            }
        }


        // Journal
        public static bool[] journalUnlocked;
        public static void UnlockJournal(int index)
        {
            journalUnlocked[index] = true;
        }
    }
}
