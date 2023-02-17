using Entities;
using Entities.WereWolf;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    [SerializeField] private Entity2D player;
    [SerializeField] private WereWolf bossFight;
    [SerializeField] private float gameOverDelay = 2;

    private void Awake()
    {
        player.Health.OnDeath += GameOver;
        bossFight.Health.OnDeath += GameOver;
    }

    private void GameOver() => StartCoroutine(GameOverAnimation());
    private IEnumerator GameOverAnimation()
    {
        if (!player.IsDead)
            while (bossFight.IsDead && !bossFight.DeathIsCompleted)
                yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(gameOverDelay);

        SceneManager.LoadScene(0);
    }
}
