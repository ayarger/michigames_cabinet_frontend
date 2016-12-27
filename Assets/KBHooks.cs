using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class KBHooks : MonoBehaviour
{
    [DllImport("user32")]
    protected static extern IntPtr SetWindowsHookEx(
        HookType code, HookProc func, IntPtr hInstance, int threadID);

    [DllImport("user32")]
    protected static extern int UnhookWindowsHookEx(
        IntPtr hhook);

    [DllImport("user32")]
    protected static extern int CallNextHookEx(
        IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

    // Hook types. To hook the keyboard we only need WH_KEYBOARD
    protected enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    protected IntPtr m_hhook = IntPtr.Zero;
    protected HookType m_hookType = HookType.WH_KEYBOARD;

    protected delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

    //We install the hook and hold on to the hook handle.
    //The handle will be need to unhook. 
    protected bool Install(HookProc cbFunc)
    {
        if (m_hhook == IntPtr.Zero)
            m_hhook = SetWindowsHookEx(
                m_hookType, 
                cbFunc, 
                IntPtr.Zero, 
                (int)AppDomain.GetCurrentThreadId());

        if (m_hhook == IntPtr.Zero)
            return false;

        return true;
    }

    protected void Uninstall()
    {
        if (m_hhook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(m_hhook);
            m_hhook = IntPtr.Zero;
        }
    }

    protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code < 0)
            return CallNextHookEx(m_hhook, code, wParam, lParam);

        Debug.Log(
            "hook code =" + code.ToString() + 
            " lparam=" + lParam.ToString() + 
            " wparam=" + wParam.ToString());

        // Yield to the next hook in the chain
        return CallNextHookEx(m_hhook, code, wParam, lParam);
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("install hook");
        Install(CoreHookProc);
    }

    void OnDisable()
    {
        Debug.Log("Uninstall hook");
        Uninstall();
    }

}