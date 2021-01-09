using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour {

    public static LoadingScreenUI singleton;

    public float transition;
    public TextMeshProUGUI statusText;

    [HideInInspector] public CanvasGroup canvasGroup;

    public delegate void OnHideDelegate();
    public event OnHideDelegate OnHide;

    public delegate void OnShowDelegate();
    public event OnShowDelegate OnShow;

    public delegate void OnStartHideDelegate();
    public event OnStartHideDelegate OnStartHide;

    public delegate void OnStartShowDelegate();
    public event OnStartShowDelegate OnStartShow;

    private float timestamp;

    private void Awake() {
        if (singleton == null) {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(string message) {
        OnStartShow?.Invoke();

        canvasGroup.blocksRaycasts = true;
        statusText.text = message + "...";
        timestamp = Time.time;

        StartCoroutine(FadeIn());
    }

    public void Hide() {
        OnStartHide?.Invoke();

        timestamp = Time.time;

        CancelInvoke("Hide");
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn() {
        yield return null;

        float deltaTime = Mathf.Clamp(Time.time - timestamp, 0f, transition);
        canvasGroup.alpha = Mathf.Lerp(0f, 1f, deltaTime / transition);

        if (canvasGroup.alpha < 1f) {
            StartCoroutine(FadeIn());
        } else {
            canvasGroup.alpha = 1f;

            OnShow?.Invoke();
            OnShow = null;
        }
    }

    private IEnumerator FadeOut() {
        yield return null;

        float deltaTime = Mathf.Clamp(Time.time - timestamp, 0f, transition);
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, deltaTime / transition);

        if (canvasGroup.alpha > 0f) {
            StartCoroutine(FadeOut());
        } else {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            OnHide?.Invoke();
            OnHide = null;
        }
    }

}
