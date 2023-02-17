using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

        public void SetAttackVDamage(int attackDamage) =>
            heeloy.SetAttackDamage(attackDamage);

        public bool DoAttackByDistance(float distance) =>
            heeloy.DoAttackByDistance(distance);


        public void DoAttackByDistanceAbility01(float distance) =>
            heeloy.DoSwordAbilityAttackByDistance(distance, 0);

        public void DoAttackByDistanceAbility02(float distance) =>
            heeloy.DoSwordAbilityAttackByDistance(distance, 1);

        public void DoAttackByDistanceAbility03(float distance) =>
            heeloy.DoSwordAbilityAttackByDistance(distance, 2);
    }
}