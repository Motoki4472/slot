using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Assets.Slot;
using Assets.Data;
using Assets.Animation;
using Assets.UI;


namespace Assets.System
{
    public class GameSystem : MonoBehaviour
    {

        private SlotSystem slotSystem;
        private SlotData slotData;
        private Combo combo;
        private ScoreData scoreData;
        [SerializeField] private ScoreText scoreText;
        [SerializeField] private List<GameObject> leftSymbols;
        private List<GameObject> leftSymbolInstances;
        [SerializeField] private List<GameObject> middleSymbols;
        private List<GameObject> middleSymbolInstances;
        [SerializeField] private List<GameObject> rightSymbols;
        private List<GameObject> rightSymbolInstances;
        [SerializeField] private GameObject leftReel;
        [SerializeField] private GameObject middleReel;
        [SerializeField] private GameObject rightReel;
        [SerializeField] private Vector3 reelPosition;
        [SerializeField] private float reelSpacing;
        [SerializeField] private Transform[] stopAnchors = new Transform[9];
        private GameState currentState;
        private int row = 3;
        private int column = 3;
        private int stopCount = 0;

        void Awake()
        {
            DOTween.Init();
        }

        public void Start()
        {
            slotData = new SlotData(leftSymbols, middleSymbols, rightSymbols, row, column);
            slotSystem = new SlotSystem(slotData, this, stopAnchors);
            combo = new Combo();
            scoreData = new ScoreData();
            scoreText.UpdateScore(scoreData.GetScore());
            currentState = GameState.StopSlot;
            MakeReels();
            leftReel.GetComponent<ManageReelAnimation>().Initialize(leftSymbols, slotSystem.GetSymbolHeight());
            middleReel.GetComponent<ManageReelAnimation>().Initialize(middleSymbols, slotSystem.GetSymbolHeight());
            rightReel.GetComponent<ManageReelAnimation>().Initialize(rightSymbols, slotSystem.GetSymbolHeight());
            currentState = GameState.CanStartSlot;
            leftReel.GetComponent<ManageReelAnimation>().SetSymbols(leftSymbolInstances);
            middleReel.GetComponent<ManageReelAnimation>().SetSymbols(middleSymbolInstances);
            rightReel.GetComponent<ManageReelAnimation>().SetSymbols(rightSymbolInstances);
            //InvokeRepeating(nameof(DebugStopSlot), 2f, 2f);
        }

        private void DebugStopSlot()
        {
            if (stopCount < 3)
            {
                // 3回に達するまでは StopSlot を呼び出す
                Debug.Log($"StopSlotを呼び出します。 ({stopCount + 1}/3回目)");
                StopSlot();
                StartSlot();
                stopCount++;
            }
            else
            {
                // 3回ストップしたら、StartSlot を実行し、カウンターをリセットする
                Debug.Log("3回ストップしたため、再度StartSlotを実行します。");
                StartSlot();
                stopCount = 0; // カウンターをリセットして、次のサイクルに備える
            }
        }
        public void ProcessMatch(List<(int SymbolId, int LineId)> matches)
        {
            int totalScore = 0;
            foreach (var match in matches)
            {
                // LineIdから行インデックスを取得（斜めラインは別途考慮が必要）
                int rowIndex = match.LineId < row ? match.LineId : 0; // 横ラインの場合
                ISymbol matchedSymbol = slotSystem.GetSymbol(rowIndex, 0); // マッチしたラインの先頭シンボルを取得

                if (matchedSymbol != null)
                {
                    // スコア = シンボルの基本価値 * コンボ数（最低1倍）
                    int score = matchedSymbol.GetValue() * Mathf.Max(1, combo.GetCombo());
                    totalScore += score;
                }
            }

            if (totalScore > 0)
            {
                scoreData.AddScore(totalScore);
                scoreText.UpdateScore(scoreData.GetScore());
                combo.IncrementCombo(); // マッチしたのでコンボを増やす
            }
            else
            {
                combo.ResetCombo(); // マッチしなかったのでコンボをリセット
            }
        }

        public enum GameState
        {
            CanStartSlot,
            StopSlot
        }

        public void StartSlot()
        {
            if (currentState == GameState.CanStartSlot)
            {
                currentState = GameState.StopSlot;
                slotSystem.StartSlot(combo.GetCombo());
            }
        }

        public void StopSlot()
        {
            if (currentState == GameState.StopSlot)
            {
                slotSystem.StopSlot();
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

        public void SetInstances(List<GameObject> leftInstances, List<GameObject> middleInstances, List<GameObject> rightInstances)
        {
            leftSymbolInstances = leftInstances;
            middleSymbolInstances = middleInstances;
            rightSymbolInstances = rightInstances;
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