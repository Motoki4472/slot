using UnityEngine;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private ComboText comboText;
        [SerializeField] private EffectAnimation effectAnimation;
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
        [SerializeField] private GameObject Handle;
        [SerializeField] private GameObject Button;
        private GameState currentState;
        private int row = 3;
        private int column = 3;
        private int stopCount = 0;

        [Header("アイドル時操作表示")]
        [SerializeField] private TextMeshProUGUI idlePromptText; // 操作を促すテキスト
        [SerializeField] private float idleThreshold = 3.0f; // 3秒間操作がなければ表示
        [SerializeField] private Transform[] idolAnchors = new Transform[2];
        private float idleTimeForStart = 3.5f;
        private float idleTimeForStop = 0f;
        private bool isIdlePromptVisible = false; // プロンプトが表示中かどうかのフラグ
        private Sequence idlePromptAnimation; // アニメーション管理用のシーケンス

        [Header("特殊演出")]
        [SerializeField] private SevenMatchEffect sevenMatchEffect; // 7揃い演出クラスへの参照
        [SerializeField] private int sevenSymbolId = 3; // 7のシンボルID (インスペクターで変更可能)

        public enum GameState
        {
            Title, // タイトルアニメーション中の状態を追加
            CanStartSlot,
            StopSlot
        }

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
            comboText.UpdateCombo(combo.GetCombo());
            currentState = GameState.Title;
            MakeReels();
            leftReel.GetComponent<ManageReelAnimation>().Initialize(leftSymbols, slotSystem.GetSymbolHeight());
            middleReel.GetComponent<ManageReelAnimation>().Initialize(middleSymbols, slotSystem.GetSymbolHeight());
            rightReel.GetComponent<ManageReelAnimation>().Initialize(rightSymbols, slotSystem.GetSymbolHeight());
            leftReel.GetComponent<ManageReelAnimation>().SetSymbols(leftSymbolInstances);
            middleReel.GetComponent<ManageReelAnimation>().SetSymbols(middleSymbolInstances);
            rightReel.GetComponent<ManageReelAnimation>().SetSymbols(rightSymbolInstances);
            //InvokeRepeating(nameof(DebugStopSlot), 2f, 2f);

            if (idlePromptText != null)
            {
                idlePromptText.text = ""; // 初期状態では非表示
                idlePromptText.alpha = 0f;
            }
        }

        void Update()
        {
            // スタート可能な状態で放置されている場合
            if (currentState == GameState.CanStartSlot)
            {
                idleTimeForStart += Time.deltaTime;
                if (idleTimeForStart >= idleThreshold && !isIdlePromptVisible)
                {

                    ShowIdlePrompt("<rotate=90>スワイプVVV</rotate>", 0); // 縦書きで表示
                    idlePromptText.transform.rotation = Quaternion.Euler(0, 0, -90); // テキストを90度回転
                }
            }
            // ストップ可能な状態で放置されている場合
            else if (currentState == GameState.StopSlot)
            {
                idleTimeForStop += Time.deltaTime;
                if (idleTimeForStop >= idleThreshold && !isIdlePromptVisible)
                {
                    ShowIdlePrompt("タップ", 1); // 横書きで表示
                    idlePromptText.transform.rotation = Quaternion.Euler(0, 0, 0); // テキストを元に戻す
                }
            }
        }

        private void ShowIdlePrompt(string message, int anchorIndex)
        {
            if (idlePromptText == null || idolAnchors == null || anchorIndex >= idolAnchors.Length) return;

            isIdlePromptVisible = true;
            var textRect = idlePromptText.GetComponent<RectTransform>();

            // 既存のアニメーションを停止
            idlePromptAnimation?.Kill();
            textRect.DOKill();
            idlePromptText.DOKill();

            // ■ 表示位置と向きを設定
            idlePromptText.transform.position = idolAnchors[anchorIndex].position;
            idlePromptText.text = message;

            // ■ アニメーションを開始
            // 1. 出現アニメーション (フェードイン & スケールアップ)
            var appearSequence = DOTween.Sequence();
            appearSequence
                .Append(idlePromptText.DOFade(1f, 1.5f))
                .Join(textRect.DOScale(1f, 0.8f).From(0.8f).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    // 2. 表示中アニメーション (呼吸のようにループ)
                    // idlePromptAnimationにループアニメーションを格納して後で停止できるようにする
                    idlePromptAnimation = DOTween.Sequence()
                        .Append(textRect.DOScale(1.05f, 2.0f).SetEase(Ease.InOutSine))
                        .Join(idlePromptText.DOFade(0.2f, 2.5f).SetEase(Ease.InOutSine))
                        .SetLoops(-1, LoopType.Yoyo);
                });
        }

        private void HideIdlePrompt()
        {
            if (idlePromptText == null || !isIdlePromptVisible) return;

            isIdlePromptVisible = false;

            // 既存のアニメーションを停止
            idlePromptAnimation?.Kill();
            idlePromptText.GetComponent<RectTransform>().DOKill(); // ループアニメーションも停止

            // ■ 消滅アニメーション (フェードアウト)
            idlePromptAnimation = DOTween.Sequence();
            idlePromptAnimation
                .Append(idlePromptText.DOFade(0f, 0.4f))
                .OnComplete(() =>
                {
                    idlePromptText.text = "";
                    idlePromptText.transform.localScale = Vector3.one; // スケールをリセット
                });
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

        private void PlayMatchAnimation(List<GameObject> matchedObjects)
        {
            foreach (var obj in matchedObjects)
            {
                // スケールを少し大きくして元に戻すパンチアニメーション
                // パラメーター: (振幅, 持続時間, 振動回数, 緩和率)
                obj.transform.DOPunchScale(new Vector3(0.06f, 0.06f, 0), 1.0f, 2, 0.5f);
            }
        }
        public void ProcessMatch(List<(int SymbolId, int LineId)> matches)
        {
            if (matches.Count == 0)
            {
                SESystem.Instance.PlayMismatchSound();
                combo.ResetCombo(); // マッチしなかったのでコンボをリセット
                comboText.UpdateCombo(combo.GetCombo());
                return;
            }

            // 7が揃ったかどうかをチェック
            bool isSevenMatched = false;
            foreach (var match in matches)
            {
                if (match.SymbolId == sevenSymbolId)
                {
                    isSevenMatched = true;
                    break;
                }
            }

            // 7が揃っていたら特別演出を再生
            if (isSevenMatched && sevenMatchEffect != null)
            {
                sevenMatchEffect.Play();
            }
            else
            {
                // 通常のマッチSEとエフェクト
                SESystem.Instance.PlayMatchSound();
                if (effectAnimation != null)
                {
                    effectAnimation.PlayRandomEffect();
                }
            }

            int totalScore = 0;
            List<GameObject> matchedObjects = new List<GameObject>();

            foreach (var match in matches)
            {
                // ... 既存のシンボル取得とスコア計算ロジック ...
                // LineIdから行インデックスを取得
                // 横ライン (LineId: 0, 1, 2)
                if (match.LineId < row)
                {
                    for (int j = 0; j < column; j++)
                    {
                        matchedObjects.Add(slotSystem.GetSymbol(match.LineId, j).GetGameObject());
                    }
                }
                // 左上から右下への斜めライン (LineId: 3)
                else if (match.LineId == row)
                {
                    for (int i = 0; i < row; i++)
                    {
                        matchedObjects.Add(slotSystem.GetSymbol(i, i).GetGameObject());
                    }
                }
                // 右上から左下への斜めライン (LineId: 4)
                else if (match.LineId == row + 1)
                {
                    for (int i = 0; i < row; i++)
                    {
                        matchedObjects.Add(slotSystem.GetSymbol(i, column - 1 - i).GetGameObject());
                    }
                }

                // スコア計算
                ISymbol matchedSymbol = slotSystem.GetSymbol(match.LineId < row ? match.LineId : 0, 0);
                int score = matchedSymbol.GetValue() * Mathf.Max(1, combo.GetCombo());
                totalScore += score;
            }

            scoreData.AddScore(totalScore);
            scoreText.UpdateScore(scoreData.GetScore());
            combo.IncrementCombo(); // マッチしたのでコンボを増やす

            // マッチしたシンボルをアニメーションさせる
            PlayMatchAnimation(matchedObjects);

            comboText.UpdateCombo(combo.GetCombo());
        }



        public void StartSlot()
        {
            if (currentState == GameState.CanStartSlot)
            {
                slotSystem.StartSlot(combo.GetCombo());
                Handle.GetComponent<PushAnimation>().PlayAnimation();

                idleTimeForStart = 0f;
                HideIdlePrompt();
                SESystem.Instance.PlayStartSound();
                SESystem.Instance.StartSpinLoop();

                // 0.1秒後に状態をStopSlotに変更
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    currentState = GameState.StopSlot;
                });
            }
        }

        public void StopSlot()
        {
            if (currentState == GameState.StopSlot)
            {
                slotSystem.StopSlot();
                Button.GetComponent<PushAnimation>().PlayAnimation();

                idleTimeForStop = 0f;
                HideIdlePrompt();
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