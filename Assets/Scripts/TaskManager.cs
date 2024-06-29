using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class TaskManager : MonoBehaviour
{
    public Transform player;
    public Transform goalPosition;
    public float distanceThreshold;
    public string csvFilePath = "TaskResults.csv";

    private bool isTaskRunning = false;
    private float taskStartTime;
    private int trialNumber = 0;
    private StringBuilder csvData;

    void Start()
    {
        InitializeCsvFile();
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
        }
    }

    void InitializeCsvFile()
    {
        csvData = new StringBuilder();
        csvData.AppendLine("Trial Number,Task Duration (seconds)");
        File.WriteAllText(csvFilePath, csvData.ToString());
    }

    void StartTask()
    {
        isTaskRunning = true;
        taskStartTime = Time.time;
        trialNumber++;
        Debug.Log($"試行 {trialNumber} を開始しました．");
    }

    void CheckTaskCompletion()
    {
        float distanceToGoal = Vector3.Distance(player.position, goalPosition.position);
        
        if (distanceToGoal <= distanceThreshold)
        {
            float taskDuration = Time.time - taskStartTime;
            isTaskRunning = false;
            Debug.Log($"タスクが完了しました．所要時間: {taskDuration:F2}秒");
            SaveResultToCsv(taskDuration);
        }
    }

    void SaveResultToCsv(float taskDuration)
    {
        string newLine = $"{trialNumber},{taskDuration:F2}";
        csvData.AppendLine(newLine);
        File.AppendAllText(csvFilePath, newLine + System.Environment.NewLine);
        Debug.Log($"結果をCSVファイルに保存しました: {csvFilePath}");
    }
}
