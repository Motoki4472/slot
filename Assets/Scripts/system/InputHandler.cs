using UnityEngine;
using UnityEngine.InputSystem; // Input System を使用
using UnityEngine.Events; // UnityEvent を使用

namespace Assets.System
{
    public class InputHandler : MonoBehaviour
    {
        [Header("スワイプ判定の最小距離 (Screen Pixel)")]
        [SerializeField] private float minSwipeDistance = 50f;

        [Header("検出イベント")]
        public UnityEvent OnTap; // タップ時に実行
        public UnityEvent<SwipeDirection> OnSwipe; // スワイプ時に実行 (方向付き)

        private InputSystem_Actions inputActions; // 自動生成された C# クラス
        private Vector2 swipeStartPosition;
        private bool isSwiping = false;

        public enum SwipeDirection
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        private void Awake()
        {
            inputActions = new InputSystem_Actions();

            // --- アクションのイベント登録 ---

            // タップ (Tap)
            // performed は「タップが完了した」瞬間に呼ばれます
            inputActions.Player.Tap.performed += _ => HandleTap();

            // スワイプ (PrimaryContact)
            // started は「押した」瞬間に呼ばれます
            inputActions.Player.PrimaryContact.started += ctx => StartSwipe(ctx);
            // canceled は「離した」瞬間に呼ばれます
            inputActions.Player.PrimaryContact.canceled += ctx => EndSwipe(ctx);
        }

        private void OnEnable()
        {
            inputActions.Player.Enable(); // スクリプト有効時に Input を有効化
        }

        private void OnDisable()
        {
            inputActions.Player.Disable(); // 無効時に Input も無効化
        }

        // --- イベント処理 ---

        private void HandleTap()
        {
            Debug.Log("Tap Detected!");
            // 登録された関数（インスペクタで設定）を実行
            OnTap?.Invoke(); 
        }

        private void StartSwipe(InputAction.CallbackContext context)
        {
            // 押した時点の座標を読み取る
            swipeStartPosition = inputActions.Player.PrimaryPosition.ReadValue<Vector2>();
            isSwiping = true;
        }

        private void EndSwipe(InputAction.CallbackContext context)
        {
            if (!isSwiping) return;
            isSwiping = false;

            // 離した時点の座標を読み取る
            Vector2 endPosition = inputActions.Player.PrimaryPosition.ReadValue<Vector2>();

            // 差分（スワイプベクトル）を計算
            Vector2 delta = endPosition - swipeStartPosition;

            // スワイプ距離が閾値未満なら、スワイプとみなさない
            if (delta.magnitude < minSwipeDistance)
            {
                return;
            }

            // スワイプ方向を判定
            SwipeDirection direction = DetectSwipeDirection(delta);
            Debug.Log("Swipe Detected: " + direction);

            // 登録された関数（インスペクタで設定）を実行
            OnSwipe?.Invoke(direction);
        }

        private SwipeDirection DetectSwipeDirection(Vector2 delta)
        {
            // X方向の移動量が大きいか、Y方向の移動量が大きいか
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // 左右スワイプ
                return (delta.x > 0) ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else
            {
                // 上下スワイプ
                return (delta.y > 0) ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }
    }
}