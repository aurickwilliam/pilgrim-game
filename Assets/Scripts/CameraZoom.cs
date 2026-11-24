using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float startOrthographicSize = 20f;
    public float targetOrthographicSize = 5f;
    public float zoomDuration = 2.5f;
    public float delayBeforeZoom = 2f;

    [Header("Position Settings")]
    public Transform arenaCenterTarget;

    [Header("Easing")]
    public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CinemachineCamera _vcam;
    private CinemachineFollow _followComponent;
    private CinemachineConfiner2D _confiner;

    // Dummy target we'll move ourselves — Cinemachine follows this cleanly
    private Transform _dummyTarget;

    void Awake()
    {
        _vcam = GetComponent<CinemachineCamera>();
        _followComponent = GetComponent<CinemachineFollow>();
        _confiner = GetComponent<CinemachineConfiner2D>();

        _vcam.Lens.OrthographicSize = startOrthographicSize;

        // Create an invisible dummy target at runtime
        _dummyTarget = new GameObject("_ZoomDummyTarget").transform;
    }

    void Start()
    {
        StartCoroutine(ZoomInRoutine());
    }

    private IEnumerator ZoomInRoutine()
    {
        Transform realFollowTarget = _vcam.Follow; // save the real player target

        // Place dummy at arena center
        Vector3 startPos = arenaCenterTarget != null
            ? arenaCenterTarget.position
            : (realFollowTarget != null ? realFollowTarget.position : transform.position);

        _dummyTarget.position = startPos;

        // Point Cinemachine at the dummy — no more direct transform writes
        _vcam.Follow = _dummyTarget;

        if (_confiner) _confiner.enabled = false;

        if (delayBeforeZoom > 0f)
            yield return new WaitForSeconds(delayBeforeZoom);

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomDuration);
            float curvedT = zoomCurve.Evaluate(t);

            _vcam.Lens.OrthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, curvedT);

            // Move the dummy toward the real player — Cinemachine follows it naturally
            if (realFollowTarget != null)
            {
                _dummyTarget.position = Vector3.Lerp(startPos, realFollowTarget.position, curvedT);
            }

            yield return null;
        }

        _vcam.Lens.OrthographicSize = targetOrthographicSize;
        _dummyTarget.position = realFollowTarget.position;

        // Zero damping before swap
        var settings = _followComponent.TrackerSettings;
        Vector3 originalDamping = settings.PositionDamping;
        settings.PositionDamping = Vector3.zero;
        _followComponent.TrackerSettings = settings;

        // Swap to real target
        _vcam.Follow = realFollowTarget;
        if (_confiner) _confiner.enabled = true;

        // Force Cinemachine to hard-reset its internal pipeline state — kills the flick
        Vector3 snapPos = new Vector3(
            realFollowTarget.position.x,
            realFollowTarget.position.y,
            transform.position.z
        );
        _vcam.ForceCameraPosition(snapPos, transform.rotation);

        Destroy(_dummyTarget.gameObject);

        // Restore damping next frame
        yield return null;
        settings.PositionDamping = originalDamping;
        _followComponent.TrackerSettings = settings;
    }
}