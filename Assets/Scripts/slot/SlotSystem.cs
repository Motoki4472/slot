using UnityEngine;
using System.Collections.Generic;
using Assets.data;
using Assets.system;

namespace Assets.slot
{

    public class SlotSystem
    {
        private int row;
        private int column;
        private ISymbol[,] symbols;
        private Slot slotData;
        private SlotSpeed slotSpeed;
        private MainSystem mainSystem;
        private SlotState slotState;
        public SlotSystem(Slot slotData, MainSystem mainSystem)
        {
            this.slotData = slotData;
            this.row = slotData.getRow();
            this.column = slotData.getColumn();
            symbols = new ISymbol[row, column];
            slotSpeed = new SlotSpeed();
            this.mainSystem = mainSystem;
            slotState = SlotState.left;
        }
        private enum SlotState
        {
            left,
            middle,
            right,
            stop
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
        }
        public void StopSlot()
        {
            // アニメーションストップ
            PositionsCorrection();
            NextSlotState();
            if(slotState != SlotState.stop)
            {
                // CheckMatch();
            }


        }
        private void PositionsCorrection()
        {
            // Logic to correct positions of symbols
        }
        private void updateSymbles()
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