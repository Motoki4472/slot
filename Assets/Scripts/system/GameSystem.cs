using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Assets.Slot;
using Assets.Data;
using Assets.Animation;


namespace Assets.System
{
    public class GameSystem : MonoBehaviour
    {

        private SlotSystem slotSystem;
        private SlotData slotData;
        private Combo combo;
        private ScoreData scoreData;
        [SerializeField] private List<GameObject> leftSymbols;
        [SerializeField] private List<GameObject> middleSymbols;
        [SerializeField] private List<GameObject> rightSymbols;
        [SerializeField] private GameObject leftReel;
        [SerializeField] private GameObject middleReel;
        [SerializeField] private GameObject rightReel;
        [SerializeField] private Vector3 reelPosition;
        [SerializeField] private float reelSpacing;
        private GameState currentState;
        private int row = 3;
        private int column = 3;

        void Awake()
        {
            DOTween.Init();
        }

        public void Start()
        {
            slotData = new SlotData(leftSymbols, middleSymbols, rightSymbols, row, column);
            slotSystem = new SlotSystem(slotData, this);
            combo = new Combo();
            scoreData = new ScoreData();
            currentState = GameState.StopSlot;
            MakeReels();
            leftReel.GetComponent<ManageReelAnimation>().Initialize(leftSymbols, slotSystem.GetSymbolHeight());
            middleReel.GetComponent<ManageReelAnimation>().Initialize(middleSymbols, slotSystem.GetSymbolHeight());
            rightReel.GetComponent<ManageReelAnimation>().Initialize(rightSymbols, slotSystem.GetSymbolHeight());
            currentState = GameState.CanStartSlot;
            StartSlot();
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
            }
        }

        public void ChangeStateToStartSlot()
        {
            currentState = GameState.CanStartSlot;
        }

        public void MakeReels()
        {
            leftReel.transform.position = new Vector3(reelPosition.x - reelSpacing, reelPosition.y, reelPosition.z);
            middleReel.transform.position = new Vector3(reelPosition.x, reelPosition.y, reelPosition.z);
            rightReel.transform.position = new Vector3(reelPosition.x + reelSpacing, reelPosition.y, reelPosition.z);
            slotSystem.SetReels(leftReel, middleReel, rightReel);
        }

        public GameObject GetLeftReel()
        {
            return leftReel;
        }
        public GameObject GetMiddleReel()
        {
            return middleReel;
        }
        public GameObject GetRightReel()
        {
            return rightReel;
        }



    }
}