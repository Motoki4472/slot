using UnityEngine;

namespace UI
{

    [ExecuteAlways] // エディタ上でも動作させる
    [RequireComponent(typeof(Camera))] // このスクリプトはCameraにしか付かないようにする
    public class AspectRatioFixer : MonoBehaviour
    {
        // 基準となるアスペクト比 (縦画面 1080x1920)
        public float targetAspectWidth = 9.0f;
        public float targetAspectHeight = 16.0f;

        private Camera targetCamera;

        void Start()
        {
            targetCamera = GetComponent<Camera>();
            UpdateViewportRect();
        }

        // エディタでの変更を即時反映するため
        void Update()
        {
            if (Application.isPlaying) return; // 実行中はStart()のみ
            UpdateViewportRect();
        }

        void UpdateViewportRect()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }

            // 基準のアスペクト比
            float targetAspect = targetAspectWidth / targetAspectHeight;

            // 現在の画面のアスペクト比
            float currentAspect = (float)Screen.width / Screen.height;

            // 基準よりも横長の場合 (PCモニターなど)
            if (currentAspect > targetAspect)
            {
                float newWidth = targetAspect / currentAspect;
                float newX = (1.0f - newWidth) / 2.0f;
                targetCamera.rect = new Rect(newX, 0, newWidth, 1.0f);
            }
            // 基準よりも縦長の場合 (特殊なスマホなど)
            else
            {
                float newHeight = currentAspect / targetAspect;
                float newY = (1.0f - newHeight) / 2.0f;
                targetCamera.rect = new Rect(0, newY, 1.0f, newHeight);
            }
        }
    }
}