using UnityEngine;
using UnityEngine.UI; // Imageを使用するために追加
using TMPro;          // TextMeshProUGUIを使用するために追加
using DG.Tweening;
using Assets.System;

namespace Assets.Animation
{
    public class TitleAnimation : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI titleLogoText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameSystem gameSystem;

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 1.0f;
        [SerializeField] private float descriptionFadeInDelay = 0.3f;
        [SerializeField] private float displayDuration = 2.0f;
        [SerializeField] private float fadeOutDuration = 0.8f;

        void Start()
        {
            // 初期状態を設定 (アルファ値のみ変更)
            if (backgroundImage != null) backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 1f);
            if (titleLogoText != null) titleLogoText.alpha = 0f;
            if (descriptionText != null) descriptionText.alpha = 0f;

            PlayAnimation();
        }

        public void PlayAnimation()
        {
            // 各オブジェクトが設定されているか確認
            if (backgroundImage == null || titleLogoText == null || descriptionText == null || gameSystem == null)
            {
                Debug.LogError("必要なコンポーネントや参照が設定されていません。");
                return;
            }

            // アニメーションシーケンスを作成
            Sequence sequence = DOTween.Sequence();

            // 1. タイトルロゴと説明文のフェードイン
            sequence.Append(titleLogoText.DOFade(1f, fadeInDuration).SetDelay(0.5f))
                    .Join(descriptionText.DOFade(1f, fadeInDuration).SetDelay(descriptionFadeInDelay));

            // 2. 一定時間表示
            sequence.AppendInterval(displayDuration);

            // 3. タイトルロゴと説明文のフェードアウト
            sequence.Append(titleLogoText.DOFade(0f, fadeOutDuration))
                    .Join(descriptionText.DOFade(0f, fadeOutDuration));

            // 4. 背景のフェードアウト
            sequence.Append(backgroundImage.DOFade(0f, fadeOutDuration));

            // 5. アニメーション完了時の処理
            sequence.OnComplete(() =>
            {
                if (titleLogoText != null) titleLogoText.gameObject.SetActive(false);
                if (descriptionText != null) descriptionText.gameObject.SetActive(false);
                if (backgroundImage != null) backgroundImage.gameObject.SetActive(false);
                gameSystem.ChangeStateToStartSlot();
                Debug.Log("タイトルアニメーション完了。ゲーム開始状態へ。");
            });
        }
    }
}