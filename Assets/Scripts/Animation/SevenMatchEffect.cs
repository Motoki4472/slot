using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Assets.Animation
{
    public class SevenMatchEffect : MonoBehaviour
    {
        [Header("演出オブジェクト")]
        [SerializeField] private Image flashImage; // 画面全体を覆うフラッシュ用のImage
        [SerializeField] private Image jackpotBackground; // Jackpotテキストの背景画像
        [SerializeField] private TextMeshProUGUI jackpotText; // 「JACKPOT!」などのテキスト
        [SerializeField] private ParticleSystem confettiParticles; // 紙吹雪などのパーティクル

        [Header("効果音")]
        [SerializeField] private AudioSource jackpotAudioSource; // 専用のAudioSource
        [SerializeField] private AudioClip jackpotSound; // 7揃い専用の効果音

        void Start()
        {
            if (flashImage != null) flashImage.color = new Color(1, 1, 1, 0);
            if (jackpotBackground != null)
            {
                jackpotBackground.color = new Color(jackpotBackground.color.r, jackpotBackground.color.g, jackpotBackground.color.b, 0);
            }
            if (jackpotText != null)
            {
                jackpotText.alpha = 0;
            }
        }

        /// <summary>
        /// 7揃い時の特別演出を再生します。
        /// </summary>
        public void Play()
        {
            // 演出オブジェクトが設定されていなければ何もしない
            if (flashImage == null || jackpotText == null || jackpotBackground == null)
            {
                Debug.LogWarning("7揃いの演出オブジェクトが設定されていません。");
                return;
            }

            // 既存のアニメーションを停止
            DOTween.Kill(this);

            // 専用SEを再生
            if (jackpotAudioSource != null && jackpotSound != null)
            {
                jackpotAudioSource.PlayOneShot(jackpotSound);
            }

            // 紙吹雪パーティクルを再生
            if (confettiParticles != null)
            {
                confettiParticles.Play();
            }

            // アニメーションシーケンスを開始
            Sequence effectSequence = DOTween.Sequence().SetId(this);

            // 1. 画面を白くフラッシュさせる
            effectSequence.Append(flashImage.DOFade(0.7f, 0.1f))
                          .Append(flashImage.DOFade(0f, 0.4f));

            // 2. カットインを左から右へ流すアニメーション
            RectTransform textRect = jackpotText.rectTransform;
            RectTransform bgRect = jackpotBackground.rectTransform;
            float screenWidth = ((RectTransform)textRect.root).sizeDelta.x;
            float slideInDuration = 0.15f;
            float slideOutDuration = 0.1f;
            float displayDuration = 1.0f;

            // 初期位置を画面左外に設定
            textRect.anchoredPosition = new Vector2(-screenWidth, textRect.anchoredPosition.y);
            bgRect.anchoredPosition = new Vector2(-screenWidth, bgRect.anchoredPosition.y);

            // フェードイン
            effectSequence.Insert(0.1f, jackpotText.DOFade(1f, slideInDuration * 0.5f));
            effectSequence.Insert(0.1f, jackpotBackground.DOFade(0.8f, slideInDuration * 0.5f));

            // スライドイン -> 表示 -> スライドアウト
            effectSequence.Insert(0.1f, textRect.DOAnchorPosX(0, slideInDuration).SetEase(Ease.OutCubic));
            effectSequence.Insert(0.1f, bgRect.DOAnchorPosX(0, slideInDuration).SetEase(Ease.OutCubic))
                          .AppendInterval(displayDuration)
                          .Append(textRect.DOAnchorPosX(screenWidth, slideOutDuration).SetEase(Ease.InCubic))
                          .Join(bgRect.DOAnchorPosX(screenWidth, slideOutDuration).SetEase(Ease.InCubic))
                          .OnComplete(() => {
                              // アニメーション完了後に透明にする
                              jackpotText.alpha = 0;
                              jackpotBackground.color = new Color(jackpotBackground.color.r, jackpotBackground.color.g, jackpotBackground.color.b, 0);
                          });
        }
    }
}