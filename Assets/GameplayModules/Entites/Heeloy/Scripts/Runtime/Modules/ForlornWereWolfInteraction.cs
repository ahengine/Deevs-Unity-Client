using UnityEngine;

namespace Entities.Heeloy.Moudles
{
    [RequireComponent(typeof(Heeloy))]
    public class ForlornWereWolfInteraction : MonoBehaviour
    {
        private const string FORLORN_HIT_DASH_STRIKE_SWORD_ANIMATOR_STATE = "ForlornWereWolf_DashStrikeSword";
        private const string FORLORN_HIT_SWORD_01_ANIMATOR_STATE = "ForlornWereWolf_HitSword01";
        private const string FORLORN_HIT_SWORD_02_ANIMATOR_STATE = "ForlornWereWolf_HitSword02";

        private Heeloy heeloy;
        private bool firstHitSword;

        private void Awake() => heeloy = GetComponent<Heeloy>();

        public void ApplyHitDashStrikeSword()
        {
            heeloy.DoDamage(2);
            if (heeloy.IsDead) return;
            heeloy.DoPlayAnimation(FORLORN_HIT_DASH_STRIKE_SWORD_ANIMATOR_STATE);
        }

        public void ApplyHitSword()
        {
            if (!firstHitSword)
            {
                ApplyHitSword01();
                firstHitSword = true;
            }
            else
                ApplyHitSword02();
        }

        private void ApplyHitSword01()
        {
            heeloy.DoDamage(2);
            if (heeloy.IsDead) return;
            heeloy.DoPlayAnimation(FORLORN_HIT_SWORD_01_ANIMATOR_STATE);
        }
        private void ApplyHitSword02()
        {
            heeloy.DoDamage(2);
            if (heeloy.IsDead) return;
            heeloy.DoPlayAnimation(FORLORN_HIT_SWORD_02_ANIMATOR_STATE);
        }
    }
}