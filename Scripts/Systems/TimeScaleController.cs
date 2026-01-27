using UnityEngine;
using UnityEngine.Events;

public class TimeScaleController : MonoBehaviour {

    [Header("Time Scale Settings")]
    [SerializeField] private float currentTimeScale = 1f;
    public float minTimeScale = 0.1f;
    public float maxTimeScale = 10f;
    public float defaultTimeScale = 1f;

    [Header("Presets")]
    public float[] presetSpeeds = new float[] { 0.5f, 1f, 2f, 5f, 10f };
    private int currentPresetIndex = 1;

    [Header("State")]
    [SerializeField] private bool isPaused = false;
    private float timeScaleBeforePause;

    public UnityEvent<float> OnTimeScaleChanged;
    public UnityEvent OnPaused;
    public UnityEvent OnResumed;

    public float CurrentTimeScale => currentTimeScale;
    public bool IsPaused => isPaused;

    void Awake() {
        currentTimeScale = defaultTimeScale;
        Time.timeScale = currentTimeScale;
    }

    void Update() {
        HandleInput();
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals)) {
            NextPresetSpeed();
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            PreviousPresetSpeed();
        }

        for (int i = 0; i < presetSpeeds.Length && i < 9; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                SetTimeScale(presetSpeeds[i]);
            }
        }
    }

    public void SetTimeScale(float scale) {
        if (isPaused) {
            timeScaleBeforePause = Mathf.Clamp(scale, minTimeScale, maxTimeScale);
            return;
        }

        currentTimeScale = Mathf.Clamp(scale, minTimeScale, maxTimeScale);
        Time.timeScale = currentTimeScale;

        if (SparkWorld.Instance != null) {
            SparkWorld.Instance.timeScale = currentTimeScale;
        }

        UpdatePresetIndex();
        OnTimeScaleChanged?.Invoke(currentTimeScale);
    }

    public void SetTimeScaleNormalized(float normalized) {
        float scale = Mathf.Lerp(minTimeScale, maxTimeScale, normalized);
        SetTimeScale(scale);
    }

    public float GetTimeScaleNormalized() {
        return Mathf.InverseLerp(minTimeScale, maxTimeScale, currentTimeScale);
    }

    public void TogglePause() {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause() {
        if (isPaused) return;
        timeScaleBeforePause = currentTimeScale;
        isPaused = true;
        Time.timeScale = 0f;
        OnPaused?.Invoke();
    }

    public void Resume() {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = timeScaleBeforePause;
        currentTimeScale = timeScaleBeforePause;
        OnResumed?.Invoke();
        OnTimeScaleChanged?.Invoke(currentTimeScale);
    }

    public void NextPresetSpeed() {
        if (currentPresetIndex < presetSpeeds.Length - 1) {
            currentPresetIndex++;
            SetTimeScale(presetSpeeds[currentPresetIndex]);
        }
    }

    public void PreviousPresetSpeed() {
        if (currentPresetIndex > 0) {
            currentPresetIndex--;
            SetTimeScale(presetSpeeds[currentPresetIndex]);
        }
    }

    public void ResetToDefault() {
        SetTimeScale(defaultTimeScale);
    }

    void UpdatePresetIndex() {
        float closestDiff = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < presetSpeeds.Length; i++) {
            float diff = Mathf.Abs(presetSpeeds[i] - currentTimeScale);
            if (diff < closestDiff) {
                closestDiff = diff;
                closestIndex = i;
            }
        }

        currentPresetIndex = closestIndex;
    }

    public string GetSpeedDisplayString() {
        if (isPaused) return "PAUSED";
        return $"{currentTimeScale:F1}x";
    }
}
