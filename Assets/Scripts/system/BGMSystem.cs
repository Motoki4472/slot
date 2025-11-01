using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro; // TextMeshProUGUIを使用するために追加



namespace Assets.System
{

    [RequireComponent(typeof(AudioSource))]
    public class BGMSystem : MonoBehaviour
    {
        [Header("BGM設定")]
        [SerializeField]
        private List<AudioClip> bgmClips; // 再生するBGMクリップのリスト

        [SerializeField]
        [Range(0f, 1f)]
        private float maxVolume = 0.8f; // BGMの最大音量

        [SerializeField]
        private float fadeDuration = 2.0f; // フェードイン・フェードアウトにかかる時間

        [Header("UI設定")]
        [SerializeField]
        private TextMeshProUGUI trackNameText; // 曲名を表示するテキスト

        [SerializeField]
        private float trackNameDisplayDuration = 3.0f; // 曲名が表示されている時間

        [SerializeField]
        private float trackNameAnimDuration = 0.5f; // 曲名表示のアニメーション時間

        private AudioSource audioSource;
        private List<AudioClip> shuffledPlaylist;
        private int currentTrackIndex = -1;
        private Sequence trackNameAnimation; // 曲名アニメーション用のシーケンス

        void Start()
        {
            // AudioSourceコンポーネントを取得
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.volume = 0f;

            // 曲名テキストの初期化
            if (trackNameText != null)
            {
                trackNameText.alpha = 0f;
            }

            // 再生リストが空でなければ、3秒後に再生を開始
            if (bgmClips != null && bgmClips.Count > 0)
            {
                DOVirtual.DelayedCall(3.0f, () =>
                {
                    ShufflePlaylist();
                    PlayNextTrack();
                });
            }
            else
            {
                Debug.LogWarning("BGMクリップが設定されていません。");
            }
        }

        private void ShufflePlaylist()
        {
            if (shuffledPlaylist == null)
            {
                shuffledPlaylist = new List<AudioClip>();
            }

            shuffledPlaylist.Clear();
            shuffledPlaylist.AddRange(bgmClips);

            for (int i = shuffledPlaylist.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                AudioClip temp = shuffledPlaylist[i];
                shuffledPlaylist[i] = shuffledPlaylist[j];
                shuffledPlaylist[j] = temp;
            }

            currentTrackIndex = -1;
            Debug.Log("BGMプレイリストをシャッフルしました。");
        }

        private void PlayNextTrack()
        {
            currentTrackIndex++;

            if (currentTrackIndex >= shuffledPlaylist.Count)
            {
                ShufflePlaylist();
                currentTrackIndex = 0;
            }

            AudioClip nextClip = shuffledPlaylist[currentTrackIndex];
            audioSource.clip = nextClip;
            audioSource.Play();

            // 曲名を表示
            ShowTrackName(nextClip.name);

            audioSource.DOFade(maxVolume, fadeDuration).SetEase(Ease.InQuad);

            float crossfadeStartTime = nextClip.length - fadeDuration;
            if (crossfadeStartTime > 0)
            {
                DOVirtual.DelayedCall(crossfadeStartTime, () =>
                {
                    audioSource.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad)
                        .OnComplete(PlayNextTrack);
                });
            }
            else
            {
                DOVirtual.DelayedCall(nextClip.length, PlayNextTrack);
            }
        }

        /// <summary>
        /// 曲名を表示し、アニメーションさせます。
        /// </summary>
        /// <param name="name">表示する曲名</param>
        private void ShowTrackName(string name)
        {
            if (trackNameText == null) return;

            // 既存のアニメーションがあれば停止
            trackNameAnimation?.Kill();

            // テキストと位置を設定
            trackNameText.text = name;
            RectTransform textRect = trackNameText.rectTransform;

            // 画面の右外からスライドインし、一定時間表示後、さらに右へスライドアウトする
            // anchoredPositionを使うため、アンカーは右下に設定されている必要があります
            trackNameAnimation = DOTween.Sequence();
            trackNameAnimation
                .Append(textRect.DOAnchorPosX(textRect.anchoredPosition.x, 0)) // 即座に位置をリセット
                .Join(trackNameText.DOFade(1f, 0)) // アルファをリセット
                .Append(textRect.DOAnchorPosX(textRect.anchoredPosition.x + 400, trackNameAnimDuration).From(true).SetEase(Ease.OutCubic)) // 右外からスライドイン
                .AppendInterval(trackNameDisplayDuration) // 表示時間
                .Append(textRect.DOAnchorPosX(textRect.anchoredPosition.x, trackNameAnimDuration).SetEase(Ease.InCubic)) // 右へスライドアウト
                .Join(trackNameText.DOFade(0f, trackNameAnimDuration * 0.8f)); // 同時にフェードアウト
        }

        void OnDestroy()
        {
            DOTween.Kill(audioSource);
            DOTween.Kill(this);
            trackNameAnimation?.Kill();
        }
    }
}