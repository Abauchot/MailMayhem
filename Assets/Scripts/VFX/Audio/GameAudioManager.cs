using Scoring;
using UnityEngine;

namespace VFX.Audio
{
    /// <summary>
    /// Manages gameplay audio feedback based on scoring events.
    /// Plays sound effects for correct hits, errors, and combo milestones.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class GameAudioManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip correctHitSound;
        [SerializeField] private AudioClip errorHitSound;
        [SerializeField] private AudioClip comboMilestoneSound;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float correctHitVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float errorHitVolume = 0.8f;
        [SerializeField] [Range(0f, 1f)] private float comboMilestoneVolume = 1f;

        [Header("Combo Settings")]
        [SerializeField] private int[] comboMilestones = { 3, 5, 10, 15, 20, 25, 30 };

        private AudioSource _audioSource;
        private int _lastCelebratedCombo = 0;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            ValidateReferences();
            scoreSystem.OnScoringEvent += HandleScoringEvent;
            Debug.Log("[GameAudioManager] Initialized and subscribed to scoring events.");
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoringEvent -= HandleScoringEvent;
            }
            Debug.Log("[GameAudioManager] Unsubscribed from scoring events.");
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            if (evt.IsCorrect)
            {
                PlaySound(correctHitSound, correctHitVolume);
                
                if (!IsMilestone(evt.NewCombo) || evt.NewCombo <= _lastCelebratedCombo) return;

                PlaySoundDelayed(comboMilestoneSound, comboMilestoneVolume, 0.1f);
                _lastCelebratedCombo = evt.NewCombo;
            }
            else
            {
                PlaySound(errorHitSound, errorHitVolume);
                _lastCelebratedCombo = 0; 
            }
        }

        private bool IsMilestone(int combo)
        {
            foreach (int milestone in comboMilestones)
            {
                if (combo == milestone)
                    return true;
            }
            return false;
        }

        private void PlaySound(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                Debug.LogWarning("[GameAudioManager] Attempted to play null AudioClip.");
                return;
            }

            _audioSource.PlayOneShot(clip, volume);
        }

        private void PlaySoundDelayed(AudioClip clip, float volume, float delay)
        {
            if (clip == null)
            {
                Debug.LogWarning("[GameAudioManager] Attempted to play null AudioClip.");
                return;
            }

            Invoke(nameof(PlayDelayedSound), delay);
            _delayedClip = clip;
            _delayedVolume = volume;
        }
        
        private AudioClip _delayedClip;
        private float _delayedVolume;

        private void PlayDelayedSound()
        {
            if (_delayedClip != null)
            {
                _audioSource.PlayOneShot(_delayedClip, _delayedVolume);
            }
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[GameAudioManager] ScoreSystem reference missing!");

            if (correctHitSound == null)
                Debug.LogWarning("[GameAudioManager] CorrectHitSound not assigned!");

            if (errorHitSound == null)
                Debug.LogWarning("[GameAudioManager] ErrorHitSound not assigned!");

            if (comboMilestoneSound == null)
                Debug.LogWarning("[GameAudioManager] ComboMilestoneSound not assigned!");
        }
    }
}