using UnityEngine;
using System.Collections.Generic;
using Assets.Slot;
using Assets.Data;


namespace Assets.System
{
    public class MainSystem : MonoBehaviour
    {

        private SlotSystem slotSystem;
        private SlotData slotData;
        private Combo combo;
        private ScoreData scoreData;
        [SerializeField] private List<ISymbol> leftSymbols;
        [SerializeField] private List<ISymbol> middleSymbols;
        [SerializeField] private List<ISymbol> rightSymbols;
        private GameState currentState;

        public void Start()
        {
            slotData = new SlotData(leftSymbols, middleSymbols, rightSymbols, 3, 3);
            slotSystem = new SlotSystem(slotData, this);
            combo = new Combo();
            scoreData = new ScoreData();
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