using UnityEngine;

namespace Entities.Heeloy.Moudles
{
    [RequireComponent(typeof(Heeloy))]
    public class ForlornWereWolfInteraction : MonoBehaviour
    {
        private const string STATE_MACHINE_ANIMATOR = "React WereWolf.";
        private const string DASH_STRIKE_SWORD_ANIMATOR_STATE = STATE_MACHINE_ANIMATOR + "DashStrikeSword";
        private const string SWORD_01_ANIMATOR_STATE = STATE_MACHINE_ANIMATOR + "Sword01";
        private const string SWORD_02_PART1_ANIMATOR_STATE = STATE_MACHINE_ANIMATOR + "Sword02P1";
        private const string SWORD_02_PART2_ANIMATOR_STATE = STATE_MACHINE_ANIMATOR + "Sword02P2";
        private const string FUCK_OFF_ANIMATOR_STATE = STATE_MACHINE_ANIMATOR + "Fuckoff";
        private float pushBackDirection;

        private Heeloy heeloy;
        [SerializeField] private float fuckOffPushBackSpeed = .3f;
        [SerializeField] private float dashStrikePushBackSpeed = .6f;
        [SerializeField] private float strike2Part2PushBackSpeed = .3f;
        [SerializeField] private float strike2Part1HorizontalInputOffDuration = 2.5f;

        private void Awake() => heeloy = GetComponent<Heeloy>();

        public void ApplyHitFuckOff(bool faceDirection,int damage)
        {
            heeloy.DoDamage(damage);
            if (heeloy.IsDead) return;
            heeloy.DamageState(true);
            heeloy.SetFaceDirection(!faceDirection);
            pushBackDirection = (faceDirection ? 1 : -1);
            heeloy.PushBackSettings(fuckOffPushBackSpeed * pushBackDirection);
            heeloy.DoPlayAnimation(FUCK_OFF_ANIMATOR_STATE);
        }

        public void ApplyHitDashStrikeSword(bool faceDirection, int damage)
        {
            heeloy.DoDamage(damage);
            if (heeloy.IsDead) return;
            heeloy.DamageState(true);
            pushBackDirection = (faceDirection ? 1 : -1);
            heeloy.PushBackSettings(dashStrikePushBackSpeed * pushBackDirection);
            heeloy.DoPlayAnimation(DASH_STRIKE_SWORD_ANIMATOR_STATE);
        }

        public void ApplyHitStrikeSword01(int damage)
        {
            heeloy.DoDamage(damage);
            if (heeloy.IsDead) return;
            heeloy.DamageState(true);
            heeloy.DoPlayAnimation(SWORD_01_ANIMATOR_STATE);
        }
        public void ApplyHitStrikeSword02Part1(int damage)
        {
            heeloy.DoDamage(damage);
            if (heeloy.IsDead) return;
            heeloy.DamageState(true,false);
            heeloy.DoPlayAnimation(SWORD_02_PART1_ANIMATOR_STATE);
            heeloy.SetAllowHorizontalInput(false);
            heeloy.SetAllowHorizontalInputAfterDelay(true, strike2Part1HorizontalInputOffDuration);
        }

        public void ApplyHitStrikeSword02Part2(bool faceDirection, int damage)
        {
            heeloy.DoDamage(damage);
            if (heeloy.IsDead) return;
            heeloy.DamageState(true);
            pushBackDirection = (faceDirection ? 1 : -1);
            heeloy.PushBackSettings(strike2Part2PushBackSpeed * pushBackDirection);
            heeloy.DoPlayAnimation(SWORD_02_PART2_ANIMATOR_STATE);
        }
    }
}