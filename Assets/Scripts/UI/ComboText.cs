using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace Assets.UI
{
    public class ComboText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private List<Color> comboColors = new List<Color> { Color.white, Color.yellow, Color.red };
        [SerializeField] private float baseFontSize = 72f;
        [SerializeField] private float sizeIncreasePerCombo = 2f;
        [SerializeField] private float maxFontSize = 120f;

        private int previousCombo = 0;
        private Sequence currentAnimation;

        void Start()
        {
            // 初期状態ではテキストを非表示にする
            comboText.text = "";
            comboText.alpha = 0;
            comboText.fontSize = baseFontSize;
        }

        public void UpdateCombo(int combo)
        {
            // ... (既存のUpdateComboのコードはそのまま) ...
            // 実行中のアニメーションがあれば中断する
            currentAnimation?.Kill();

            bool wasVisible = previousCombo > 1;
            bool isVisible = combo > 1;

            // コンボ数に応じた色とサイズを計算
            Color targetColor = GetColorForCombo(combo);
            float targetFontSize = Mathf.Min(baseFontSize + (combo * sizeIncreasePerCombo), maxFontSize);

            if (!wasVisible && isVisible)
            {
                // ■ 出現アニメーション
                comboText.text = GetComboString(combo);
                comboText.color = targetColor;
                comboText.fontSize = baseFontSize * 0.8f; // 少し小さい状態から開始

                currentAnimation = DOTween.Sequence();
                currentAnimation.Append(comboText.DOFade(1f, 0.3f)) // 0.3秒でフェードイン
                              .Join(DOTween.To(() => comboText.fontSize, x => comboText.fontSize = x, targetFontSize, 0.2f).SetEase(Ease.OutBack)); // 0.3秒でサイズアップ
            }
            else if (wasVisible && isVisible)
            {
                // ■ 更新アニメーション
                comboText.text = GetComboString(combo);

                currentAnimation = DOTween.Sequence();
                currentAnimation.Append(comboText.DOColor(targetColor, 0.2f)) // 色を滑らかに変更
                              .Join(DOTween.To(() => comboText.fontSize, x => comboText.fontSize = x, targetFontSize, 0.2f)) // サイズを滑らかに変更
                              .Append(comboText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1)); // パンチエフェクト
            }
            else if (wasVisible && !isVisible)
            {
                // ■ 消滅アニメーション
                currentAnimation = DOTween.Sequence();
                currentAnimation.Append(comboText.DOFade(0f, 0.3f)) // 0.3秒でフェードアウト
                              .Join(DOTween.To(() => comboText.fontSize, x => comboText.fontSize = x, baseFontSize * 0.8f, 0.2f).SetEase(Ease.InBack))
                              .OnComplete(() =>
                              {
                                  comboText.text = ""; // アニメーション完了後にテキストを空にする
                                  comboText.fontSize = baseFontSize; // フォントサイズをリセット
                              });
            }

            // 今回のコンボ数を記録
            previousCombo = combo;


        }

        // ... (GetComboString と GetColorForCombo メソッドはそのまま) ...
        // テキストを生成するヘルパーメソッド
        private string GetComboString(int combo)
        {
            string text = combo.ToString() + "レンゾク";
            int exclamationCount = combo / 5;
            if (exclamationCount > 0)
            {
                if (exclamationCount > 10) exclamationCount = 10; // 最大10個まで
                text += new string('!', exclamationCount);
            }
            return text;
        }

        // コンボ数に応じた色を取得するヘルパーメソッド
        private Color GetColorForCombo(int combo)
        {
            if (comboColors == null || comboColors.Count == 0)
            {
                return Color.white;
            }
            // 例: 5コンボごとに色を変える
            int colorIndex = combo  / 5;
            colorIndex = Mathf.Clamp(colorIndex, 0, comboColors.Count - 1);
            return comboColors[colorIndex];
        }
    }
}