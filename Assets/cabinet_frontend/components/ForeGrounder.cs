using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class ForeGrounder : MonoBehaviour {

    private const uint LOCK = 1;
    private const uint UNLOCK = 2;

    private IntPtr window;

    float duration = 1.0f;
    float previous_time;

    void Start() {
        previous_time = Time.time;
    }

    void Update() {
        if (Time.time - previous_time > duration)
        {
            previous_time = Time.time;
            LockSetForegroundWindow(LOCK);
            window = GetActiveWindow();
            StartCoroutine(PerformWindowCheck());
        }
    }

    /*IEnumerator Checker() {
        while (true) {

            yield return new WaitForSeconds(1);
            IntPtr newWindow = GetActiveWindow();

            if (window != newWindow) {
                SwitchToThisWindow(window, true);
            }
        }
    }*/

    IEnumerator PerformWindowCheck()
    {
        yield return new WaitForSeconds(1);
        IntPtr newWindow = GetActiveWindow();

        if (window != newWindow)
        {
            SwitchToThisWindow(window, true);
        }
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    static extern bool LockSetForegroundWindow(uint uLockCode);
    [DllImport("user32.dll")]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
}