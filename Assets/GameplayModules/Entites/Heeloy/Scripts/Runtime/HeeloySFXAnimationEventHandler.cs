using UnityEngine;

namespace Entities.Heeloy
{
    public class HeeloySFXAnimationEventHandler : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField] private AudioClip walk;
        [SerializeField] private AudioClip sit;
        [SerializeField] private AudioClip standup;
        [SerializeField] private AudioClip swordAttackAbility;
        [SerializeField] private AudioClip jumpAttack;
        [SerializeField] private AudioClip[] death;
        [SerializeField] private AudioClip dodge;
        [SerializeField] private AudioClip dodgeAttack;
        [SerializeField] private AudioClip jump;
        [SerializeField] private AudioClip doubleJump;
        [SerializeField] private AudioClip[] swordAttack;
        [SerializeField] private AudioClip[] swordHeavyAttack;
        [SerializeField] private AudioClip fallingFromSkyAliveOnSky;
        [SerializeField] private AudioClip fallingFromSkyAliveOnSkyDead;
        [SerializeField] private AudioClip fallingFromSkyDead;
        [Header("Were Wolf React")]
        [SerializeField] private AudioClip WWReactFuckOff;
        [SerializeField] private AudioClip WWReactDashStrike;
        [SerializeField] private AudioClip[] WWReactAttack;

        private void Awake() =>
            audioSource = GetComponent<AudioSource>();

        private void PlayAudio(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        private void PlayRandomAudio(AudioClip[] clip) =>
            PlayAudio(clip[Random.Range(0,clip.Length)]);

        public void DeathRandom() =>
            PlayRandomAudio(death);

        public void Death(int index) =>
            PlayAudio(death[index]);
    }
}