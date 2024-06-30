using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;

public class TaskManager : MonoBehaviour
{
    public Transform playerStartPosition1;
    public Transform playerStartPosition2;
    public Transform goalPosition;
    public float distanceThreshold;
    public string csvFilePath = "TaskResults.csv";
    private bool isTaskRunning = false;
    private Stopwatch stopwatch;
    private int trialNumber = 0;
    private StringBuilder csvData;
    private int currentStartPosition;

    void Start()
    {
        InitializeCsvFile();
        stopwatch = new Stopwatch();
    }

    void Update()
    {
        if (!isTaskRunning && Input.GetKeyDown(KeyCode.Return))
        {
            StartTask();
        }
        if (isTaskRunning)
        {
            CheckTaskCompletion();
            // Rボタンでリセット
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetTask();
            }
        }
    }

    void InitializeCsvFile()
    {
        csvData = new StringBuilder();
        csvData.AppendLine("Trial Number,Task Duration (seconds),Result,Start Position");
        File.WriteAllText(csvFilePath, csvData.ToString());
    }

    void StartTask()
    {
        isTaskRunning = true;
        stopwatch.Reset();
        stopwatch.Start();
        trialNumber++;
        UnityEngine.Debug.Log($"試行 {trialNumber} を開始しました．開始位置: {currentStartPosition}");
    }

    void CheckTaskCompletion()
    {
        float distanceToGoal1 = Vector3.Distance(playerStartPosition1.position, goalPosition.position);
        float distanceToGoal2 = Vector3.Distance(playerStartPosition2.position, goalPosition.position);
        if (distanceToGoal1 <= distanceThreshold)
        {
            CompleteTask();
        }
        else if (distanceToGoal2 <= distanceThreshold)
        {
            CompleteTask();
        }
    }

    void CompleteTask()
    {
        stopwatch.Stop();
        float taskDuration = stopwatch.ElapsedMilliseconds / 1000f;
        isTaskRunning = false;
        UnityEngine.Debug.Log($"タスクが完了しました．所要時間: {taskDuration:F2}秒");
        SaveResultToCsv(taskDuration, "完了");
    }

    void ResetTask()
    {
        stopwatch.Stop();
        float taskDuration = stopwatch.ElapsedMilliseconds / 1000f;
        isTaskRunning = false;
        UnityEngine.Debug.Log($"タスクがリセットされました．所要時間: {taskDuration:F2}秒");
        SaveResultToCsv(taskDuration, "エラー");
    }

    void SaveResultToCsv(float taskDuration, string result)
    {
        string newLine = $"{trialNumber},{taskDuration:F2},{result},{currentStartPosition}";
        csvData.AppendLine(newLine);
        File.AppendAllText(csvFilePath, newLine + System.Environment.NewLine);
        UnityEngine.Debug.Log($"結果をCSVファイルに保存しました: {csvFilePath}");
    }
}