using UnityEngine;
using DG.Tweening;

namespace Assets.Animation
{
    public class PushAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite firstImage;
        [SerializeField] private Sprite secondImage;
        [SerializeField] private float switchDuration = 0.2f;
        private SpriteRenderer targetRenderer;

        private void Awake()
        {
            targetRenderer = GetComponent<SpriteRenderer>();
            if (targetRenderer == null)
            {
                Debug.LogError("SpriteRenderer コンポーネントがアタッチされていません。");
            }
        }

        /// <summary>
        /// 画像を2つ目に切り替え、指定時間後に1つ目に戻す
        /// </summary>
        public void PlayAnimation()
        {
            if (targetRenderer == null || firstImage == null || secondImage == null)
            {
                Debug.LogError("必要な設定が不足しています。");
                return;
            }

            targetRenderer.sprite = secondImage;

            DOVirtual.DelayedCall(switchDuration, () =>
            {
                targetRenderer.sprite = firstImage;
            });
        }
    }
}