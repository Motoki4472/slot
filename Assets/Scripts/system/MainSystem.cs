using UnityEngine;
using System.Collections.Generic;
using Assets.slot;
using Assets.data;


namespace Assets.system
{
    public class MainSystem : MonoBehaviour
    {

        private SlotSystem slotSystem;
        private Slot slot;
        private Combo combo;
        private Score score;
        [SerializeField] private List<ISymbol> leftSymbols;
        [SerializeField] private List<ISymbol> middleSymbols;
        [SerializeField] private List<ISymbol> rightSymbols;
        private GameState currentState;

        public void Start()
        {
            slot = new Slot(leftSymbols, middleSymbols, rightSymbols, 3, 3);
            slotSystem = new SlotSystem(slot, this);
            combo = new Combo();
            score = new Score();
            currentState = GameState.StopSlot;
        }

        public enum GameState
        {
            CanStartSlot,
            StopSlot
        }

        private void StartSlot()
        {
            if (currentState == GameState.CanStartSlot)
            {
                currentState = GameState.StopSlot;
                slotSystem.StartSlot(combo.GetCombo());
                // アニメーション

            }
        }

        public void ChangeStateToStartSlot()
        {
            currentState = GameState.CanStartSlot;
        }
        


    }
}