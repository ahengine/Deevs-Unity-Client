using UnityEngine;

public class BloodyRain : MonoBehaviour
{
    [SerializeField] private GameObject spawner;
    [SerializeField] private SplashEffect splashEffect;
    [SerializeField] private float duration = .5f;

    Coroutine bloodyRainCoroutine;
    public void Do()
    {
        if (spawner.activeSelf)
            StopCoroutine(bloodyRainCoroutine);

        spawner.SetActive(true);
        splashEffect.DoEffect();
        bloodyRainCoroutine = StartCoroutine(CoroutineHelper.CallActionWithDelay(() => spawner.SetActive(false), duration));
    }
}
