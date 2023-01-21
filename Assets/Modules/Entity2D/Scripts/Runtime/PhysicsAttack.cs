using UnityEngine;
using System.Collections.Generic;

namespace Entities
{
    public static class PhysicsAttack
    {
        public static T Ray2D<T>(this Transform tr, float distance, LayerMask layerMask = default) where T : class
        {
            var hit = Physics2D.Raycast(tr.position, tr.right, distance, layerMask);

            return hit && hit is T && hit.transform != tr ? hit as T : null;
        }

        public static List<T> Ray2DAll<T>(this Transform tr, float distance, LayerMask layerMask = default) where T : class
        {
            List<T> list = new List<T>();
            var hits = Physics2D.RaycastAll(tr.position, tr.right, distance, layerMask);

            for (int i = 0; i < hits.Length; i++)
                if (hits[i].transform != tr && hits[i] is T)
                    list.Add(list[i] as T);

            return list;
        }

        public static T Circle2D<T>(this Transform tr, float raidus, LayerMask layerMask = default) where T : class
        {
            var hit = Physics2D.CircleCast(tr.position, raidus,tr.right, layerMask);

            return hit && hit is T && hit.transform != tr ? hit as T : null;
        }

        public static List<T> Circle2DAll<T>(this Transform tr, float raidus, LayerMask layerMask = default) where T : class
        {
            List<T> list = new List<T>();
            var hits = Physics2D.CircleCastAll(tr.position, raidus, tr.right, layerMask);

            for (int i = 0; i < hits.Length; i++)
                if (hits[i].transform != tr && hits[i] is T)
                    list.Add(list[i] as T);

            return list;
        }
    }
}