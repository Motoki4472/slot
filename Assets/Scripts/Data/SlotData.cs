using UnityEngine;
using System.Collections.Generic;

namespace Assets.Data
{
    public class SlotData
    {
        private List<ISymbol> leftSymbols;
        private List<ISymbol> middleSymbols;
        private List<ISymbol> rightSymbols;
        private int row;
        private int column;

        public SlotData(List<ISymbol> leftSymbols, List<ISymbol> middleSymbols, List<ISymbol> rightSymbols, int row, int column)
        {
            this.leftSymbols = leftSymbols;
            this.middleSymbols = middleSymbols;
            this.rightSymbols = rightSymbols;
            this.row = row;
            this.column = column;
        }

        public List<ISymbol> getLeftSymbols()
        {
            return leftSymbols;
        }
        public List<ISymbol> getMiddleSymbols()
        {
            return middleSymbols;
        }
        public List<ISymbol> getRightSymbols()
        {
            return rightSymbols;
        }
        public int getRow()
        {
            return row;
        }
        public int getColumn()
        {
            return column;
        }

    }
}
