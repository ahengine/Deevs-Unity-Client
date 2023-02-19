using UnityEngine;

namespace Entities.Heeloy
{
    public class HeeloyAnimationEventHandler : MonoBehaviour
    {
        private AudioSource audioSource;
        private Heeloy heeloy;

        private void Awake()
        {
            heeloy = transform.parent.parent.GetComponent<Heeloy>();
            audioSource = GetComponent<AudioSource>();
        }

        public void OnStand() => heeloy.OnStand();
        public void StandUp() => heeloy.StandUp();
        public void OnAttackEnd() => heeloy.AttackEnd();
        public void OnFromSkyLanded() => heeloy.AttackEnd();
        public void OnDamageEnd() => heeloy.DamageState(false);

        public void PushBack() =>
            heeloy.PushBack();

        public void PushBackStop() =>
            heeloy.PushBackStop();

        public void PlayAudioLoop(AudioClip clip)
        {
            if (audioSource.clip == clip && audioSource.isPlaying) return;

            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
        public void PlayAudio(AudioClip clip)
        {
            audioSource.loop = false;
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void DoAttackByDistanceAbility01(AttackCard handler) =>
            heeloy.DoSwordAbilityAttackByDistance(handler.distance,handler.Damage, 0);

        public void DoAttackByDistanceAbility02(AttackCard handler) =>
            heeloy.DoSwordAbilityAttackByDistance(handler.distance, handler.Damage, 1);

        public void DoAttackByDistanceAbility03(AttackCard handler) =>
            heeloy.DoSwordAbilityAttackByDistance(handler.distance, handler.Damage, 2);

        public void DoAttack(AttackCard handler) =>
            heeloy.DoAttackByDistance(handler.distance, handler.Damage);

        public void DoJumpAttack(AttackCard handler) =>
            heeloy.DoJumpAttackByDistance(handler.distance, handler.Damage);

    }
}