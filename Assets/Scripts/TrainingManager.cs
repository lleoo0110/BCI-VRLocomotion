using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    public GameObject xrHMD;
    public float runSpeed;
    public float restDuration; // 安静期間
    public float instructionDuration; // 指示期間
    public float crossDuration; // 合図期間
    public float imageryDuration; // イメージ想起期間
    private Vector3 initialPosition;

    public Canvas canvas;
    public Text text;
    public GameObject verticalLine;
    public GameObject horizontalLine;

    private bool isTaskRunning = false;
    private Stopwatch stopwatch;

    private UdpClient udpClient;
    private string ipAddress = "127.0.0.1";
    private int port = 12354;

    private void Start()
    {
        udpClient = new UdpClient();
        initialPosition = xrHMD.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTaskRunning)
        {
            isTaskRunning = true;
            StartCoroutine(RunTraining());
            SendData("Start");
        }
    }

    private IEnumerator RunTraining()
    {
        while (true)
        {
            // ここを繰り返す
            yield return StartCoroutine(Neutral());
            yield return StartCoroutine(Imagery());
        }
    }

    private IEnumerator Neutral()
    {
        UnityEngine.Debug.Log("Neutral started.");
        stopwatch = Stopwatch.StartNew();
        xrHMD.transform.position = initialPosition; // 初期位置に戻す

        // テキストウィンドウ設定
        verticalLine.SetActive(false);
        horizontalLine.SetActive(false);
        canvas.gameObject.SetActive(true);
        text.gameObject.SetActive(false);

        // 安静期間
        canvas.gameObject.SetActive(true);
        text.gameObject.SetActive(false);
        yield return new WaitForSeconds(restDuration);

        // 指示期間
        text.gameObject.SetActive(true);
        text.text = "Neutral";
        UnityEngine.Debug.Log("Stay still.");
        yield return new WaitForSeconds(instructionDuration);

        // 合図期間
        text.gameObject.SetActive(false);
        verticalLine.SetActive(true);
        horizontalLine.SetActive(true);
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // イメージ想起期間
        canvas.gameObject.SetActive(false);
        UnityEngine.Debug.Log("Staying still.");
        StartCoroutine(Stay());
        yield return new WaitForSeconds(imageryDuration);

        stopwatch.Stop();
        UnityEngine.Debug.Log("Neutral completed. Elapsed time: " + stopwatch.Elapsed.TotalSeconds + " seconds");
    }

    private IEnumerator Imagery()
    {
        UnityEngine.Debug.Log("startedWalkImagery.");
        stopwatch = Stopwatch.StartNew();
        xrHMD.transform.position = initialPosition; // 初期位置に戻す

        // テキストウィンドウ設定
        verticalLine.SetActive(false);
        horizontalLine.SetActive(false);
        canvas.gameObject.SetActive(true);
        text.gameObject.SetActive(false);

        // 安静期間
        yield return new WaitForSeconds(restDuration);

        // 指示期間
        text.gameObject.SetActive(true);
        text.text = "Walk Imagery";
        UnityEngine.Debug.Log("Instruction: Walk Imagery");
        yield return new WaitForSeconds(instructionDuration);

        // 合図期間
        text.gameObject.SetActive(false);
        verticalLine.SetActive(true);
        horizontalLine.SetActive(true);
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // イメージ想起期間
        canvas.gameObject.SetActive(false);
        UnityEngine.Debug.Log("Walking Imagery.");
        StartCoroutine(Locomotion());
        yield return new WaitForSeconds(imageryDuration);

        stopwatch.Stop();
        UnityEngine.Debug.Log("PushCube completed. Elapsed time: " + stopwatch.Elapsed.TotalSeconds + " seconds");
    }

    private IEnumerator Locomotion()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < imageryDuration)
        {
            Vector3 movement = transform.forward * runSpeed * Time.deltaTime;
            xrHMD.transform.position += movement;
            yield return null;
        }
        stopwatch.Stop();
    }
    
    private IEnumerator Stay()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < imageryDuration)
        {
            yield return null;
        }
        stopwatch.Stop();
    }

    public void SendData(string data)
    {
        byte[] sendData = Encoding.UTF8.GetBytes(data);
        udpClient.Send(sendData, sendData.Length, ipAddress, port);
    }


}
