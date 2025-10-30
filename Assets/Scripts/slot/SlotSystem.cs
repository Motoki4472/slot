using UnityEngine;
using System.Collections.Generic;
using Assets.Data;
using Assets.System;
using Assets.Animation;

namespace Assets.Slot
{

    public class SlotSystem
    {
        private int row;
        private int column;
        private ISymbol[,] symbols;
        private SlotData slotData;
        private SlotSpeed slotSpeed;
        private GameSystem gameSystem;
        private SlotState slotState;
        private List<GameObject> leftSymbols;
        private List<GameObject> leftSymbolInstances;
        private List<GameObject> middleSymbols;
        private List<GameObject> middleSymbolInstances;
        private List<GameObject> rightSymbols;
        private List<GameObject> rightSymbolInstances;
        private GameObject leftReel;
        private GameObject middleReel;
        private GameObject rightReel;
        private Transform[] anchors;
        private float symbolHeight = 1f;
        public SlotSystem(SlotData data, GameSystem system, Transform[] stopAnchors)
        {
            this.slotData = data;
            this.row = data.getRow();
            this.column = data.getColumn();
            this.anchors = stopAnchors;
            symbols = new ISymbol[row, column];
            slotSpeed = new SlotSpeed();
            this.gameSystem = system;
            leftReel = gameSystem.GetLeftReel();
            middleReel = gameSystem.GetMiddleReel();
            rightReel = gameSystem.GetRightReel();
            slotState = SlotState.stop;
            leftSymbolInstances = new List<GameObject>();
            middleSymbolInstances = new List<GameObject>();
            rightSymbolInstances = new List<GameObject>();
        }
        private enum SlotState
        {
            left,
            middle,
            right,
            stop
        }

        public float GetSymbolHeight()
        {
            return symbolHeight;
        }

        public void SetSymbol(int row, int column, ISymbol symbol)
        {
            symbols[row, column] = symbol;
        }

        public ISymbol GetSymbol(int row, int column)
        {
            return symbols[row, column];
        }

        public void ResetSymbols()
        {
            symbols = new ISymbol[row, column];
        }

        public void StartSlot(int combo)
        {
            slotState = SlotState.left;
            // Logic to start the slot machine
            float speed = slotSpeed.GetAdjustedSpeed(combo);
            leftReel.GetComponent<ManageReelAnimation>().SetSpinSpeed(speed);
            speed = slotSpeed.GetAdjustedSpeed(combo);
            middleReel.GetComponent<ManageReelAnimation>().SetSpinSpeed(speed);
            speed = slotSpeed.GetAdjustedSpeed(combo);
            rightReel.GetComponent<ManageReelAnimation>().SetSpinSpeed(speed);
            leftReel.GetComponent<ManageReelAnimation>().StartSpinning();
            middleReel.GetComponent<ManageReelAnimation>().StartSpinning();
            rightReel.GetComponent<ManageReelAnimation>().StartSpinning();

        }
        public void StopSlot()
        {
            // アニメーションストップ
            PositionsCorrection();

            if (slotState == SlotState.left)
            {
                var reelManager = leftReel.GetComponent<ManageReelAnimation>();
                reelManager.StopSpinning();
                // 左リール用のアンカー（0, 3, 6番目）を渡す
                var targetAnchors = new Transform[] { anchors[0], anchors[3], anchors[6] };
                reelManager.SnapToAnchorPositions(targetAnchors, () => UpdateSymbolsFromReel(0, targetAnchors));
            }
            else if (slotState == SlotState.middle)
            {
                var reelManager = middleReel.GetComponent<ManageReelAnimation>();
                reelManager.StopSpinning();
                // 中央リール用のアンカー（1, 4, 7番目）を渡す
                var targetAnchors = new Transform[] { anchors[1], anchors[4], anchors[7] };
                reelManager.SnapToAnchorPositions(targetAnchors, () => UpdateSymbolsFromReel(1, targetAnchors));
            }
            else if (slotState == SlotState.right)
            {
                var reelManager = rightReel.GetComponent<ManageReelAnimation>();
                reelManager.StopSpinning();
                // 右リール用のアンカー（2, 5, 8番目）を渡す
                var targetAnchors = new Transform[] { anchors[2], anchors[5], anchors[8] };
                reelManager.SnapToAnchorPositions(targetAnchors, () =>
                {
                    UpdateSymbolsFromReel(2, targetAnchors);
                    var matches = CheckMatch();
                    // TODO: matchesを使ったスコア加算などの処理をここに記述
                    gameSystem.ProcessMatch(matches);
                    if (matches.Count > 0)
                    {
                        Debug.Log($"マッチが見つかりました！: {matches.Count}件");
                    }
                    gameSystem.ChangeStateToStartSlot();
                });
            }
            else if (slotState == SlotState.stop)
            {
                Debug.LogWarning("SlotStateがstopのため、StopSlotの処理を中断します。");
                return;
            }
            NextSlotState();


        }

        private void UpdateSymbolsFromReel(int reelIndex, Transform[] targetAnchors)
        {
            List<GameObject> symbolInstances;
            if (reelIndex == 0) symbolInstances = leftSymbolInstances;
            else if (reelIndex == 1) symbolInstances = middleSymbolInstances;
            else symbolInstances = rightSymbolInstances;

            // アンカーの位置に最も近いシンボルを見つけて盤面にセットする
            for (int i = 0; i < targetAnchors.Length; i++)
            {
                Transform anchor = targetAnchors[i];
                GameObject closestSymbol = null;
                float minDistance = float.MaxValue;

                foreach (var symbol in symbolInstances)
                {
                    float distance = Vector3.Distance(symbol.transform.position, anchor.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestSymbol = symbol;
                    }
                }

                if (closestSymbol != null)
                {
                    // ISymbolコンポーネントを取得して盤面配列に格納
                    // 行のインデックスはアンカーの順序に依存します。
                    // 上のアンカーから順に 0, 1, 2 と仮定します。
                    symbols[i, reelIndex] = closestSymbol.GetComponent<ISymbol>();
                }
            }
        }
        private void PositionsCorrection()
        {
            // Logic to correct positions of symbols
        }

        private void NextSlotState()
        {
            if (slotState == SlotState.left)
            {
                slotState = SlotState.middle;
            }
            else if (slotState == SlotState.middle)
            {
                slotState = SlotState.right;
            }
            else if (slotState == SlotState.right)
            {
                slotState = SlotState.stop;
            }
        }


        public void SetReels(GameObject leftReel, GameObject middleReel, GameObject rightReel)
        {
            leftSymbols = slotData.getLeftSymbols();
            middleSymbols = slotData.getMiddleSymbols();
            rightSymbols = slotData.getRightSymbols();

            // Logic to set reels with the provided GameObjects
            SetSingleReel(leftReel, leftSymbols, leftSymbolInstances);
            SetSingleReel(middleReel, middleSymbols, middleSymbolInstances);
            SetSingleReel(rightReel, rightSymbols, rightSymbolInstances);
            gameSystem.SetInstances(leftSymbolInstances, middleSymbolInstances, rightSymbolInstances);

        }

        private void SetSingleReel(GameObject reel, List<GameObject> symbols, List<GameObject> instances)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                GameObject symbol = GameObject.Instantiate(symbols[i], reel.transform);
                symbol.transform.localPosition = new Vector3(0, i * symbolHeight, 0);
                instances.Add(symbol);
            }

        }

        public List<(int SymbolId, int LineId)> CheckMatch()
        {
            List<(int SymbolId, int LineId)> matches = new List<(int SymbolId, int LineId)>();

            // 横方向のチェック
            for (int i = 0; i < row; i++)
            {
                int currentSymbolId = symbols[i, 0].GetId();
                for (int j = 1; j < column; j++)
                {
                    if (symbols[i, j].GetId() != currentSymbolId)
                    {
                        break;
                    }
                    else if (j == column - 1)
                    {
                        matches.Add((currentSymbolId, i));
                    }
                }
            }

            // 左上から右下への斜め方向のチェック
            int diagonalSymbolId1 = symbols[0, 0].GetId();
            for (int i = 1; i < row; i++)
            {
                if (symbols[i, i].GetId() != diagonalSymbolId1)
                {
                    break;
                }
                else if (i == row - 1)
                {
                    matches.Add((diagonalSymbolId1, row));
                }
            }

            // 右上から左下への斜め方向のチェック
            int diagonalSymbolId2 = symbols[0, column - 1].GetId();
            for (int i = 1; i < row; i++)
            {
                if (symbols[i, column - 1 - i].GetId() != diagonalSymbolId2)
                {
                    break;
                }
                else if (i == row - 1)
                {
                    matches.Add((diagonalSymbolId2, row + 1));
                }
            }

            return matches;
        }

    }
}