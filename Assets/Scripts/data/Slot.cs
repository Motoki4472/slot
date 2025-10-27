using UnityEngine;
using System.Collections.Generic;

namespace Assets.data
{
    public class Slot
    {
        private List<ISymbol> symbols;
        private int rows;
        private int verticalLines;

        public Slot(List<ISymbol> symbols, int rows, int verticalLines)
        {
            this.symbols = symbols;
            this.rows = rows;
            this.verticalLines = verticalLines;
        }

        public List<ISymbol> getSymbols()
        {
            return symbols;
        }
        public int getRows()
        {
            return rows;
        }
        public int getVerticalLines()
        {
            return verticalLines;
        }
    }
}
