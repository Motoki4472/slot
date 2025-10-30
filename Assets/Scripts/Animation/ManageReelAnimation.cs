using UnityEngine;
using System.Collections.Generic;

namespace Assets.Animation
{
    public class ManageReelAnimation : MonoBehaviour
    {
        [SerializeField] private float spinSpeed = 10.0f; // スピン速度
        [SerializeField] private List<GameObject> symbols = new List<GameObject>(); // シンボルリスト
        [SerializeField] private float symbolHeight = 1.0f; // シンボルの高さ
        [SerializeField] private bool isSpinning = false; // スピン中かどうかのフラグ
        [SerializeField] private float reelHeight; // リール全体の高さ
        private float bottomThreshold; // ループ判定のためのY座標の下限

        public void Initialize(List<GameObject> symbolObjects, float height)
        {
            symbols = symbolObjects;
            symbolHeight = height;

            if (symbols != null && symbols.Count > 0)
            {
                reelHeight = symbolHeight * symbols.Count;
                // シンボルの初期位置が0中心と仮定し、下限を設定
                // 例えば、シンボルが3つで高さが1なら、リールの高さは3。
                // Y座標が -1.5 を下回ったらループさせる。
                bottomThreshold = -reelHeight / 2f;
            }
            else
            {
                reelHeight = 0;
            }
        }

        public void SetSpinSpeed(float speed)
        {
            spinSpeed = speed;
        }

        void Update()
        {
            // スピン中でなければ何もしない
            if (!isSpinning)
            {
                return;
            }

            // 各シンボルを下に移動させる
            foreach (var symbol in symbols)
            {
                // Time.deltaTime を使ってフレームレートに依存しない移動を行う
                symbol.transform.Translate(0, -spinSpeed * Time.deltaTime, 0, Space.Self);

                // シンボルが下限を超えたら、リール全体の高さ分だけ瞬時に上に移動させる
                if (symbol.transform.localPosition.y < bottomThreshold)
                {
                    symbol.transform.localPosition += new Vector3(0, reelHeight, 0);
                }
            }
        }

        public void StartSpinning()
        {
            if (reelHeight <= 0)
            {
                Debug.LogWarning("reelHeightが0のため、StartSpinningを中断します。Initializeが正しく呼ばれていますか？");
                return;
            }
            isSpinning = true;
        }

        public void StopSpinning()
        {
            isSpinning = false;
            // ここに停止時の位置調整処理を追加できます
            // AdjustPositionAfterStop();
        }

        // (オプション) 停止時に最寄りの位置にスナップさせる関数
        private void AdjustPositionAfterStop()
        {
            foreach (var symbol in symbols)
            {
                float currentY = symbol.transform.localPosition.y;
                // 最も近いシンボルのY座標を計算
                float closestY = Mathf.Round(currentY / symbolHeight) * symbolHeight;
                // 新しい位置をVector3で作成
                Vector3 newPosition = new Vector3(symbol.transform.localPosition.x, closestY, symbol.transform.localPosition.z);
                // 位置を更新
                symbol.transform.localPosition = newPosition;
            }
        }
    }
}