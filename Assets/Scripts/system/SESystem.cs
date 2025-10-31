using UnityEngine;
using System.Collections.Generic;

namespace Assets.System
{
    public class SESystem : MonoBehaviour
    {
        // シングルトンインスタンス
        public static SESystem Instance { get; private set; }

        [Header("オーディオソース")]
        [SerializeField]
        private AudioSource oneShotAudioSource; // 単発SE用のAudioSource
        [SerializeField]
        private AudioSource loopAudioSource;    // ループSE用のAudioSource

        [Header("SEクリップ")]
        [SerializeField]
        private List<AudioClip> startClips;      // スロット開始音 (2種)
        [SerializeField]
        private AudioClip stopClip;              // スロット停止音 (1種)
        [SerializeField]
        private AudioClip spinLoopClip;          // スロット回転音 (ループ)
        [SerializeField]
        private List<AudioClip> matchClips;      // 絵柄が揃った時の音 (5種)
        [SerializeField]
        private List<AudioClip> mismatchClips;   // 絵柄が揃わなかった時の音 (2種)

        void Awake()
        {
            // シングルトンパターンの実装
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// スロット開始音を再生します。
        /// </summary>
        public void PlayStartSound()
        {
            PlayRandomClip(startClips);
        }

        /// <summary>
        /// スロット停止音を再生します。
        /// </summary>
        public void PlayStopSound()
        {
            if (oneShotAudioSource != null && stopClip != null)
            {
                oneShotAudioSource.PlayOneShot(stopClip);
            }
        }

        /// <summary>
        /// 絵柄が揃った時の音を再生します。
        /// </summary>
        public void PlayMatchSound()
        {
            PlayRandomClip(matchClips);
        }

        /// <summary>
        /// 絵柄が揃わなかった時の音を再生します。
        /// </summary>
        public void PlayMismatchSound()
        {
            PlayRandomClip(mismatchClips);
        }

        /// <summary>
        /// スロット回転中のループ音を開始します。
        /// </summary>
        public void StartSpinLoop()
        {
            if (loopAudioSource != null && spinLoopClip != null && !loopAudioSource.isPlaying)
            {
                loopAudioSource.clip = spinLoopClip;
                loopAudioSource.loop = true;
                loopAudioSource.Play();
            }
        }

        /// <summary>
        /// スロット回転中のループ音を停止します。
        /// </summary>
        public void StopSpinLoop()
        {
            if (loopAudioSource != null && loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop();
            }
        }

        /// <summary>
        /// 指定されたリストからランダムなクリップを再生します。
        /// </summary>
        /// <param name="clips">再生するクリップのリスト</param>
        private void PlayRandomClip(List<AudioClip> clips)
        {
            if (oneShotAudioSource != null && clips != null && clips.Count > 0)
            {
                // リストからランダムに1つクリップを選ぶ
                AudioClip clip = clips[Random.Range(0, clips.Count)];
                // PlayOneShotで再生することで、他のSEと音が重なっても再生できる
                oneShotAudioSource.PlayOneShot(clip);
            }
        }
    }
}