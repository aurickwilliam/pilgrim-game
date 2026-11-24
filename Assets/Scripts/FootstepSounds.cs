using UnityEngine;

/// <summary>
/// Reusable footstep sound component.
/// Attach to any character (player or enemy) that needs footstep audio.
///
/// HOW TO USE:
///   Option A (Recommended) — Animation Events:
///     1. Attach this script to your character GameObject.
///     2. Open the Animator window, select your walk/run animation clip.
///     3. At each frame where a foot hits the ground, add an Animation Event
///        and set the function to "PlayFootstep".
///     4. Assign your AudioClips in the Inspector.
///
///   Option B — Code-driven (fallback, less accurate):
///     Call PlayFootstep() from your movement script when the character is moving.
///     Use the interval field to control how often sounds play.
/// </summary>
public class FootstepSounds : MonoBehaviour
{
    [Header("Footstep Audio Clips")]
    [Tooltip("Add multiple clips for variety — one is picked at random each step.")]
    public AudioClip[] footstepClips;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Tooltip("Randomize pitch slightly for a more natural feel.")]
    public bool randomizePitch = true;

    [Range(0.8f, 1.2f)]
    public float pitchMin = 0.9f;

    [Range(0.8f, 1.2f)]
    public float pitchMax = 1.1f;

    [Header("Code-Driven Mode (Option B only)")]
    [Tooltip("Minimum seconds between footstep sounds when using PlayFootstepIfMoving().")]
    public float stepInterval = 0.35f;

    private AudioSource audioSource;
    private float stepTimer = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Auto-create an AudioSource if one isn't already on this GameObject
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound; set to 1 for 3D positional audio
        }
    }

    void Update()
    {
        // Count down the timer each frame (used by PlayFootstepIfMoving)
        if (stepTimer > 0f)
            stepTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Call this directly from an Animation Event on each footfall frame.
    /// This is the most accurate method — sync it with your walk/run clip.
    /// </summary>
    public void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning($"{name}: FootstepSounds has no AudioClips assigned!", this);
            return;
        }

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        if (randomizePitch)
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
        else
            audioSource.pitch = 1f;

        audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Call this from code (e.g. PlayerMovement or Enemy_Movement) every frame
    /// the character is moving. Uses stepInterval to avoid spamming.
    /// Only use this if you are NOT using Animation Events (Option B).
    /// </summary>
    /// <param name="isMoving">Pass true when the character has non-zero velocity.</param>
    public void PlayFootstepIfMoving(bool isMoving)
    {
        if (!isMoving) return;
        if (stepTimer > 0f) return;

        PlayFootstep();
        stepTimer = stepInterval;
    }
}
