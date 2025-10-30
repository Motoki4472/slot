using UnityEngine;
using System.Collections.Generic;

namespace Assets.Data
{
    public class SlotData
    {
        private List<GameObject> leftSymbols;
        private List<GameObject> middleSymbols;
        private List<GameObject> rightSymbols;
        private int row;
        private int column;

        public SlotData(List<GameObject> leftSymbols, List<GameObject> middleSymbols, List<GameObject> rightSymbols, int row, int column)
        {
            this.leftSymbols = leftSymbols;
            this.middleSymbols = middleSymbols;
            this.rightSymbols = rightSymbols;
            this.row = row;
            this.column = column;
        }

        public List<GameObject> getLeftSymbols()
        {
            return leftSymbols;
        }
        public List<GameObject> getMiddleSymbols()
        {
            return middleSymbols;
        }
        public List<GameObject> getRightSymbols()
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
