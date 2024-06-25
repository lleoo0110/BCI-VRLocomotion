using UnityEngine;
using System.Diagnostics;

public class ExperimentTask : MonoBehaviour
{
    public GameObject objectPrefab; // オブジェクトのプレハブ
    public GameObject windowPrefab; // ウィンドウのプレハブ
    public Vector3[] objectPositions; // オブジェクトの生成位置の配列
    public Vector3[] windowPositions; // ウィンドウの生成位置の配列
    public float delayTime = 3f; // 静止時間
    public float stayDuration = 0.5f; // 静止判定の時間

    private GameObject currentObject; // 現在のオブジェクト
    private GameObject currentWindow; // 現在のウィンドウ
    private bool isTaskStarted = false; // タスク開始フラグ
    private bool isObjectMovable = false; // オブジェクト移動可能フラグ
    private Vector3 previousPosition; // 前回のオブジェクト位置
    private Stopwatch stayStopwatch = new Stopwatch(); // 静止判定用ストップウォッチ
    private Stopwatch delayStopwatch = new Stopwatch(); // 遅延時間用ストップウォッチ

    private void Start()
    {
        SetupExperiment();
        StartDelayTimer();
    }

    private void SetupExperiment()
    {
        SpawnObjectAndWindow();
        SetupObjectRigidbody();
        SetupWindowCollider();
        SetInitialWindowColor();
    }

    private void SpawnObjectAndWindow()
    {
        int objectIndex = Random.Range(0, objectPositions.Length);
        int windowIndex = Random.Range(0, windowPositions.Length);
        currentObject = Instantiate(objectPrefab, objectPositions[objectIndex], Quaternion.identity);
        currentWindow = Instantiate(windowPrefab, windowPositions[windowIndex], Quaternion.identity);
    }

    private void SetupObjectRigidbody()
    {
        Rigidbody objectRigidbody = currentObject.AddComponent<Rigidbody>();
        objectRigidbody.useGravity = false;
    }

    private void SetupWindowCollider()
    {
        Collider windowCollider = currentWindow.AddComponent<BoxCollider>();
        windowCollider.isTrigger = true;
    }

    private void SetInitialWindowColor()
    {
        currentWindow.GetComponent<Renderer>().material.color = Color.red;
    }

    private void StartDelayTimer()
    {
        delayStopwatch.Start();
    }

    private void Update()
    {
        if (isTaskStarted)
        {
            UpdateTaskState();
            MoveObjectWithMouseInput();
        }
        else
        {
            CheckDelayTimerToStartTask();
        }
    }

    private void UpdateTaskState()
    {
        if (!isObjectMovable && IsDelayTimeElapsed())
        {
            isObjectMovable = true;
            SetWindowColor(Color.red);
        }
    }

    private bool IsDelayTimeElapsed()
    {
        return delayStopwatch.Elapsed.TotalSeconds >= delayTime;
    }

    private void SetWindowColor(Color color)
    {
        currentWindow.GetComponent<Renderer>().material.color = color;
    }

    private void MoveObjectWithMouseInput()
    {
        if (isObjectMovable && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            currentObject.GetComponent<Rigidbody>().MovePosition(mousePosition);
        }
    }

    private void CheckDelayTimerToStartTask()
    {
        if (IsDelayTimeElapsed())
        {
            isTaskStarted = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsObjectEnteringWindow(other))
        {
            SetWindowColor(Color.green);
            previousPosition = currentObject.transform.position;
            stayStopwatch.Restart();
        }
    }

    private bool IsObjectEnteringWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsObjectStayingInWindow(other))
        {
            CheckObjectStationaryState();
        }
    }

    private bool IsObjectStayingInWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }

    private void CheckObjectStationaryState()
    {
        if (IsObjectStationary())
        {
            if (IsStayDurationElapsed())
            {
                UnityEngine.Debug.Log("Task Success!");
                // 次の試行に移行するための処理を追加
            }
        }
        else
        {
            UpdatePreviousPosition();
            stayStopwatch.Restart();
        }
    }

    private bool IsObjectStationary()
    {
        return currentObject.transform.position == previousPosition;
    }

    private bool IsStayDurationElapsed()
    {
        return stayStopwatch.Elapsed.TotalSeconds >= stayDuration;
    }

    private void UpdatePreviousPosition()
    {
        previousPosition = currentObject.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsObjectExitingWindow(other))
        {
            SetWindowColor(Color.red);
            stayStopwatch.Stop();
        }
    }

    private bool IsObjectExitingWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }
}
