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
        private List<GameObject> middleSymbols;
        private List<GameObject> rightSymbols;
        private GameObject leftReel;
        private GameObject middleReel;
        private GameObject rightReel;
        private float symbolHeight = 1f;
        public SlotSystem(SlotData slotData, GameSystem gameSystem)
        {
            this.slotData = slotData;
            this.row = slotData.getRow();
            this.column = slotData.getColumn();
            symbols = new ISymbol[row, column];
            slotSpeed = new SlotSpeed();
            this.gameSystem = gameSystem;
            leftReel = gameSystem.GetLeftReel();
            middleReel = gameSystem.GetMiddleReel();
            rightReel = gameSystem.GetRightReel();
            slotState = SlotState.left;
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
            middleReel.GetComponent<ManageReelAnimation>().SetSpinSpeed(speed);
            rightReel.GetComponent<ManageReelAnimation>().SetSpinSpeed(speed);
            leftReel.GetComponent<ManageReelAnimation>().StartSpinning();
            middleReel.GetComponent<ManageReelAnimation>().StartSpinning();
            rightReel.GetComponent<ManageReelAnimation>().StartSpinning();

        }
        public void StopSlot()
        {
            // アニメーションストップ
            PositionsCorrection();
            NextSlotState();
            if (slotState == SlotState.left)
            {
                leftReel.GetComponent<ManageReelAnimation>().StopSpinning();
                //PositionsCorrection();
            }
            else if (slotState == SlotState.middle)
            {
                middleReel.GetComponent<ManageReelAnimation>().StopSpinning();
                //PositionsCorrection();
            }
            else if (slotState == SlotState.right)
            {
                rightReel.GetComponent<ManageReelAnimation>().StopSpinning();
                //PositionsCorrection();
            }
            else if (slotState == SlotState.stop)
            {
                //updateSymbols();
                // CheckMatch();
            }


        }
        private void PositionsCorrection()
        {
            // Logic to correct positions of symbols
        }
        private void updateSymbols()
        {
            // Logic to update symbols on the slot machine
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
            SetSingleReel(leftReel, leftSymbols);
            SetSingleReel(middleReel, middleSymbols);
            SetSingleReel(rightReel, rightSymbols);

        }

        private void SetSingleReel(GameObject reel, List<GameObject> symbols)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                GameObject symbol = GameObject.Instantiate(symbols[i], reel.transform);
                symbol.transform.localPosition = new Vector3(0, i * symbolHeight, 0);
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