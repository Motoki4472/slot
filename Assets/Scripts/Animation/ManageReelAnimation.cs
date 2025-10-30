using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

namespace Assets.Animation
{
    public class ManageReelAnimation : MonoBehaviour
    {
        [SerializeField] private float spinSpeed = 10.0f;
        [SerializeField] private List<GameObject> symbols = new List<GameObject>();
        [SerializeField] private float symbolHeight = 1.0f; 
        [SerializeField] private bool isSpinning = false; 
        [SerializeField] private float reelHeight;
        private float bottomThreshold;

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
        }

        public void SetSymbols(List<GameObject> symbolObjects)
        {
            symbols = symbolObjects;
        }

        public void SetSpinSpeed(float speed)
        {
            spinSpeed = speed;
        }

        void Update()
        {
            if (!isSpinning) return;

            foreach (var symbol in symbols)
            {
                symbol.transform.Translate(0, -spinSpeed * Time.deltaTime, 0, Space.Self);

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
        }

        private void AdjustPositionAfterStop()
        {
            foreach (var symbol in symbols)
            {
                float currentY = symbol.transform.localPosition.y;
                float closestY = Mathf.Round(currentY / symbolHeight) * symbolHeight;
                Vector3 newPosition = new Vector3(symbol.transform.localPosition.x, closestY, symbol.transform.localPosition.z);
                symbol.transform.localPosition = newPosition;
            }
        }

        public void SnapToAnchorPositions(Transform[] anchors, Action onComplete = null)
        {
            if (symbols == null || symbols.Count == 0 || anchors == null || anchors.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            Transform centerAnchor = anchors[1];
            GameObject closestSymbol = null;
            float minDistance = float.MaxValue;

            foreach (var symbol in symbols)
            {
                float distance = Mathf.Abs(symbol.transform.position.y - centerAnchor.position.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestSymbol = symbol;
                }
            }

            if (closestSymbol == null)
            {
                onComplete?.Invoke();
                return;
            }

            float deltaY = centerAnchor.position.y - closestSymbol.transform.position.y;

            Sequence sequence = DOTween.Sequence();
            foreach (var symbol in symbols)
            {
                Vector3 targetPos = symbol.transform.localPosition + new Vector3(0, deltaY, 0);
                sequence.Join(symbol.transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.OutQuad));
            }
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        
    }
}