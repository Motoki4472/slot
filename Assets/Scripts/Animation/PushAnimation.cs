using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.animation
{
    public class PushAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite firstImage;
        [SerializeField] private Sprite secondImage;
        [SerializeField] private float switchDuration = 0.2f;
        private Image targetImage;
        private void Awake()
        {
            targetImage = GetComponent<Image>();
            if (targetImage == null)
            {
                Debug.LogError("Imageコンポーネントがアタッチされていません。");
            }
        }

        /// <summary>
        /// 画像を2つ目に切り替え、0.2秒後に1つ目に戻す
        /// </summary>
        public void PlayAnimation()
        {
            if (targetImage == null || firstImage == null || secondImage == null)
            {
                Debug.LogError("必要な設定が不足しています。");
                return;
            }

            targetImage.sprite = secondImage;

            DOVirtual.DelayedCall(switchDuration, () =>
            {
                targetImage.sprite = firstImage;
            });
        }
    }
}