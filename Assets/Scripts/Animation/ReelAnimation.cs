using UnityEngine;
using DG.Tweening;

namespace Assets.Animation
{
    public class ReelAnimation : MonoBehaviour
    {
       /* private bool isFalling = false; // アニメーションが実行中かどうかのフラグ

        /// <summary>
        /// ループする落下アニメーションを開始します。
        /// </summary>
        /// <param name="speed">落下速度</param>
        /// <param name="reelHeight">リール全体の高さ（ループ距離）</param>
        public void StartFalling(float speed, float reelHeight)
        {
            // すでに落下中の場合は、新しいアニメーションを開始しない
            if (isFalling) return;
            isFalling = true;

            if (speed <= 0 || reelHeight <= 0)
            {
                isFalling = false; // 不正な値の場合はフラグを戻す
                return;
            }

            // リール1周分の移動時間
            float duration = reelHeight / speed;

            // 現在のローカルY座標からリール1周分だけ下に移動し、
            // 元の位置に戻るアニメーションを無限に繰り返す
            
            transform.DOLocalMoveY(transform.localPosition.y - reelHeight, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart) // -1で無限ループ、Restartで瞬時に開始位置へワープ
                .OnKill(() => isFalling = false); // アニメーションが停止されたらフラグをfalseに戻す
        }

        /// <summary>
        /// 落下アニメーションを停止します。
        /// </summary>
        public void StopFalling()
        {
            // このオブジェクトに紐づくすべてのアニメーションを安全に停止
            // OnKillコールバックが呼ばれ、isFallingフラグが自動的にfalseになります
            transform.DOKill();
        }

        /// <summary>
        /// 指定したローカル座標まで移動します。
        /// </summary>
        /// <param name="targetLocalPosition">移動先のローカル座標</param>
        /// <param name="duration">移動にかかる時間</param>
        public void MoveToPosition(Vector3 targetLocalPosition, float duration)
        {
            // 既存のアニメーションを停止
            StopFalling();

            // 指定座標まで移動するアニメーションを作成
            transform.DOLocalMove(targetLocalPosition, duration)
                .SetEase(Ease.OutQuad);
        }*/
    }
}