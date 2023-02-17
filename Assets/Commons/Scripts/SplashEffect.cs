using UnityEngine;
using UnityEngine.UI;
using IEnumerator = System.Collections.IEnumerator;

public class SplashEffect : MonoBehaviour
{
    private Image img;
    [SerializeField] private float startDelay = .1f;
    [SerializeField] private float durationShowHide = .1f;
    [SerializeField] private float showTime = .1f;
    public bool IsAnimating { private set; get; }

    private Coroutine animatingCoroutine;

    private void Awake() => img = GetComponent<Image>();

    public void DoEffect()
    {
        if(IsAnimating)
            StopCoroutine(animatingCoroutine);

        animatingCoroutine = StartCoroutine(AnimationEffect());
    }

    private IEnumerator AnimationEffect()
    {
        IsAnimating = true;

        yield return new WaitForSeconds(startDelay);

        float timer = 0;
        Color color = img.color;
        while (timer < durationShowHide)
        {
            color.a = Mathf.Lerp(0, 1, timer / durationShowHide);
            img.color = color;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        color.a = 1;
        img.color = color;
        yield return new WaitForSeconds(showTime);

        timer = 0;
        while (timer < durationShowHide)
        {
            color.a = Mathf.Lerp(1, 0, timer / durationShowHide);
            img.color = color;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        color.a = 0;
        img.color = color;
        IsAnimating = false;
    }
}
