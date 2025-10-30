using UnityEngine;
using UnityEngine.UI; // Imageを使用するために追加
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System;

namespace Assets.Animation
{
    public class EffectAnimation : MonoBehaviour
    {
        [Header("テキスト設定")]
        [SerializeField] private TextMeshProUGUI effectText;

        [Header("カットイン背景設定")]
        [SerializeField] private Image cutinBackground; // カットイン用の背景Image
        [SerializeField] private Color cutinColor = Color.black; // 背景の基本色

        [Header("アニメーション設定")]
        [SerializeField] private float animationDuration = 0.5f; // 片道のアニメーション時間
        [SerializeField] private float stayDuration = 0.8f;      // 中央での待機時間
        
        private float screenWidth;

        [Header("エフェクトパターン")]
        [SerializeField] private List<EffectPattern> effectPatterns;

        private RectTransform textRectTransform;
        private RectTransform cutinRectTransform; // 背景のRectTransform

        [Serializable]
        public class EffectPattern
        {
            public string message;
            public Color color = Color.white;
            public AnimationType animationType;
        }

        public enum AnimationType
        {
            LeftToRight,
            LeftToLeft,
            RightToLeft,
            RightToRight
        }

        void Awake()
        {
            if (effectText == null || cutinBackground == null)
            {
                Debug.LogError("EffectTextまたはCutinBackgroundが設定されていません。");
                return;
            }
            textRectTransform = effectText.GetComponent<RectTransform>();
            cutinRectTransform = cutinBackground.GetComponent<RectTransform>();

            // 初期状態では非表示
            effectText.alpha = 0;
            cutinBackground.color = new Color(cutinColor.r, cutinColor.g, cutinColor.b, 0);

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                screenWidth = canvas.GetComponent<RectTransform>().rect.width;
            }
            else
            {
                screenWidth = Screen.width;
                Debug.LogWarning("Canvasが見つからなかったため、Screen.widthを基準にします。");
            }
        }

        public void PlayRandomEffect()
        {
            if (effectPatterns == null || effectPatterns.Count == 0) return;
            int randomIndex = UnityEngine.Random.Range(0, effectPatterns.Count);
            PlayEffect(effectPatterns[randomIndex]);
        }

        public void PlayEffect(EffectPattern pattern)
        {
            if (effectText == null) return;

            effectText.text = pattern.message;
            effectText.color = pattern.color;

            float textWidth = effectText.preferredWidth;
            float offset = (screenWidth / 2) + (textWidth / 2);

            switch (pattern.animationType)
            {
                case AnimationType.LeftToRight:
                    Animate(-offset, 0, offset);
                    break;
                case AnimationType.LeftToLeft:
                    Animate(-offset, 0, -offset);
                    break;
                case AnimationType.RightToLeft:
                    Animate(offset, 0, -offset);
                    break;
                case AnimationType.RightToRight:
                    Animate(offset, 0, offset);
                    break;
            }
        }

        private void Animate(float startX, float centerX, float endX)
        {
            DOTween.Kill(textRectTransform);
            DOTween.Kill(effectText);
            DOTween.Kill(cutinRectTransform);
            DOTween.Kill(cutinBackground);

            // 初期位置設定
            textRectTransform.anchoredPosition = new Vector2(startX, textRectTransform.anchoredPosition.y);
            cutinRectTransform.anchoredPosition = new Vector2(startX, cutinRectTransform.anchoredPosition.y);
            effectText.alpha = 0;
            cutinBackground.color = new Color(cutinColor.r, cutinColor.g, cutinColor.b, 0);

            // アニメーションシーケンスを作成
            Sequence sequence = DOTween.Sequence();
            sequence.Append(textRectTransform.DOAnchorPosX(centerX, animationDuration).SetEase(Ease.OutCubic))
                    .Join(cutinRectTransform.DOAnchorPosX(centerX, animationDuration).SetEase(Ease.OutCubic)) // 背景も一緒に移動
                    .Join(effectText.DOFade(1, animationDuration / 2))
                    .Join(cutinBackground.DOFade(0.9f, animationDuration / 2)) // 背景もフェードイン（少し半透明に）
                    .AppendInterval(stayDuration)
                    .Append(textRectTransform.DOAnchorPosX(endX, animationDuration).SetEase(Ease.InCubic))
                    .Join(cutinRectTransform.DOAnchorPosX(endX, animationDuration).SetEase(Ease.InCubic)) // 背景も一緒に消える
                    .Join(DOVirtual.DelayedCall(animationDuration / 2, () => {
                        effectText.DOFade(0, animationDuration / 3);
                        cutinBackground.DOFade(0, animationDuration / 3); // 背景もフェードアウト
                    }));
        }
    }
}