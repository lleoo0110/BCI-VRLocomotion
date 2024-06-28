using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EmotivUnityPlugin;
using UnityEngine.UI;
using UnityEngine.XR;


public class LocomotionManager : MonoBehaviour
{
    // メンタルコマンド・表情コマンド
    private int mentalAction;
    private string eyeAction;

    // パラメータ
    public float runSpeed;
    public int lookThreshold;
    public float rotationCooldown;
    public float rotationAngle; // 回転角度
    
    private Vector3 initialPosition;  // 初期位置
    private bool isWalking = false;  // 歩いているかどうか
    private float lastRotationTime;
    private int lookRightCount;
    private int lookLeftCount;

    // ヘッドセット情報
    Quaternion headsetRotation;
    Vector3 xAxisDirection;


    void Start()
    {
        initialPosition = this.transform.position;  // 初期位置を保存
    }


    void Update()
    {
        // 現在のキーボード情報
        var current = Keyboard.current;
        // aキーの入力状態取得
        var aKey = current.aKey;    
        // sキーの入力状態取得
        var sKey = current.sKey;
        // dキーの入力状態取得
        var dKey = current.dKey;
        // スペースキーの入力状態取得
        var spaceKey = current.spaceKey;

        headsetRotation = InputTracking.GetLocalRotation(XRNode.Head);
        xAxisDirection = headsetRotation * Vector3.right;

        // パラメータの更新
        mentalAction = UDPReceiver.receivedInt;
        eyeAction = EmotivUnityItf.EyeAction;

        if (eyeAction == "lookR" || dKey.wasPressedThisFrame)
        {
            LookRightDetected();
        }

        else if (eyeAction == "lookL" || aKey.wasPressedThisFrame)
        {
            LookLeftDetected();
        }

        Locomotion();
    }

    void Locomotion()
    {
        // 現在のキーボード情報
        var current = Keyboard.current;
        // キーの入力状態取得
        var wKey = current.wKey;
        var sKey = current.sKey;
        // 入力
        if (mentalAction == 2 || wKey.isPressed)
        {    
            // ヘッドセットの向いている方向に進む
            //transform.position += xAxisDirection * runSpeed * Time.deltaTime;
            //transform.position += forwardDirection * runSpeed * Time.deltaTime;
            transform.position += transform.forward * runSpeed * Time.deltaTime; 
        }

        // 入力
        else if (sKey.isPressed)
        {    
            // ヘッドセットの向いている方向に進む
            //transform.position += xAxisDirection * runSpeed * Time.deltaTime;
            //transform.position += forwardDirection * runSpeed * Time.deltaTime;
            transform.position -= transform.forward * runSpeed * Time.deltaTime; 
        }
    }

    void LookRightDetected()
    {
        lookRightCount++;
        if ((lookRightCount >= lookThreshold) && (Time.time - lastRotationTime >= rotationCooldown))
        {
            RotateR();
            lookRightCount = 0;  // カウントをリセット
            lookLeftCount = 0;
        }
    }

    void RotateR()
    {
        transform.Rotate(0f, rotationAngle, 0f);
        lastRotationTime = Time.time;
    }

    void LookLeftDetected()
    {
        lookLeftCount++;

        if ((lookLeftCount >= lookThreshold) && (Time.time - lastRotationTime >= rotationCooldown))
        {
            RotateL();
            lookRightCount = 0; // カウントをリセット
            lookLeftCount = 0;
        }
    }

    void RotateL()
    {
        transform.Rotate(0f, -rotationAngle, 0f);
        lastRotationTime = Time.time;
    }
}
