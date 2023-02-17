using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Entities;
using Entities.WereWolf;
using Cam;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private ShakeModule cameraShake;
    [SerializeField] private Entity2D player;
    [SerializeField] private WereWolf bossFight;
    [SerializeField] private float gameOverDelay = 2;
    [SerializeField] private Image playerHealth;
    [SerializeField] private Image bossFightHealth;

    private void Awake()
    {
        player.Health.OnAdd += (value,current) => SetPlayerHealth();
        player.Health.OnDamage += damage => SetPlayerHealth();
        bossFight.Health.OnDamage += damage => SetBossFightHealth();
        player.Health.OnDeath += GameOver;
        bossFight.Health.OnDeath += GameOver;
    }

    private void GameOver() => StartCoroutine(GameOverAnimation());
    private IEnumerator GameOverAnimation()
    {
        if (!player.IsDead)
            while (!bossFight.DeathIsCompleted)
                yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(gameOverDelay);

        SceneManager.LoadScene(0);
    }

    private void SetPlayerHealth()
    {
        cameraShake.Apply();
        playerHealth.fillAmount = player.Health.CurrentFillAmount;
    }
    private void SetBossFightHealth()
    {
        cameraShake.Apply();
        bossFightHealth.fillAmount = bossFight.Health.CurrentFillAmount;
    }
}
