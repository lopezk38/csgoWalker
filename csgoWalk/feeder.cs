using System;
using System.Collections.Generic;
using System.Text;
using vJoyInterfaceWrap;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace csgoWalk
{
    public class Feeder
    {

        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        private const int centerVal = 16383;
        private const int offsetVal = 5926;
        private static readonly int[] lookUp = new int[] {centerVal, centerVal - offsetVal, centerVal + offsetVal, centerVal}; //c# can't define arrays at compile time why

        private static byte leftRightKeyDown = 0b00; // msb = left, lsb = right
        private static byte forwardBackKeyDown = 0b00; //msb = forward, lsb = backwards

        private const byte leftForwardMask = 0b01;
        private const byte rightBackwardMask = 0b10;
        private const byte leftForwardSetBit = rightBackwardMask;
        private const byte rightBackwardSetBit = leftForwardMask;

        private const uint KEY_FORWARD = 0;
        private const uint KEY_RIGHT = 1;
        private const uint KEY_BACKWARD = 2;
        private const uint KEY_LEFT = 3;

        private static Dictionary<Keys, uint> keyBinds;

        private static void KbHook_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            uint direction;
            if (keyBinds.TryGetValue(key, out direction))
            {
                switch (direction)
                {
                    case KEY_FORWARD:
                        forwardBackKeyDown = (byte)(forwardBackKeyDown | leftForwardSetBit);
                        break;
                    case KEY_RIGHT:
                        leftRightKeyDown = (byte)(leftRightKeyDown | rightBackwardSetBit);
                        break;
                    case KEY_BACKWARD:
                        forwardBackKeyDown = (byte)(forwardBackKeyDown | rightBackwardSetBit);
                        break;
                    case KEY_LEFT:
                        leftRightKeyDown = (byte)(leftRightKeyDown | leftForwardSetBit);
                        break;
                }
                UpdateAnalogValue();
            }
        }

        private static void KbHook_KeyUp(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            uint direction;
            if (keyBinds.TryGetValue(key, out direction))
            {
                switch (direction)
                {
                    case KEY_FORWARD:
                        forwardBackKeyDown = (byte)(forwardBackKeyDown & leftForwardMask);
                        break;
                    case KEY_RIGHT:
                        leftRightKeyDown = (byte)(leftRightKeyDown & rightBackwardMask);
                        break;
                    case KEY_BACKWARD:
                        forwardBackKeyDown = (byte)(forwardBackKeyDown & rightBackwardMask);
                        break;
                    case KEY_LEFT:
                        leftRightKeyDown = (byte)(leftRightKeyDown & leftForwardMask);
                        break;
                }
                UpdateAnalogValue();
            }
        }

        private static void UpdateAnalogValue()
        {
            iReport.bDevice = (byte)id;
            iReport.AxisX = lookUp[leftRightKeyDown];
            iReport.AxisY = lookUp[forwardBackKeyDown];

            if (!joystick.UpdateVJD(id, ref iReport))
            {
                Program.walkerWindowObj.ConsoleAddLine("Feeding vJoy device number " + id.ToString() + " failed - Is the device enabled?");
                joystick.AcquireVJD(id);
            }
        }

        public void SetKeyBind(string direction, Keys key)
        {
            switch (direction)
            {
                case "left":
                    leftRightKeyDown = (byte)(leftRightKeyDown & leftForwardMask);
                    DeleteKeyVal(ref keyBinds, KEY_LEFT);
                    keyBinds.Add(key, KEY_LEFT);
                    break;
                case "right":
                    leftRightKeyDown = (byte)(leftRightKeyDown & rightBackwardMask);
                    DeleteKeyVal(ref keyBinds, KEY_RIGHT);
                    keyBinds.Add(key, KEY_RIGHT);
                    break;
                case "forward":
                    forwardBackKeyDown = (byte)(forwardBackKeyDown & leftForwardMask);
                    DeleteKeyVal(ref keyBinds, KEY_FORWARD);
                    keyBinds.Add(key, KEY_FORWARD);
                    break;
                case "backward":
                    forwardBackKeyDown = (byte)(forwardBackKeyDown & rightBackwardMask);
                    DeleteKeyVal(ref keyBinds, KEY_BACKWARD);
                    keyBinds.Add(key, KEY_BACKWARD);
                    break;
                default:
                    break;
            }
            return;
        }

        public Feeder() : this(1) //calls uint constructor with args of 1
        {
            keyBinds = new Dictionary<Keys, uint>(4)
            {
                {Keys.Up, KEY_FORWARD},
                {Keys.Right, KEY_RIGHT},
                {Keys.Down, KEY_BACKWARD},
                {Keys.Left, KEY_LEFT}
            };
        }

        public Feeder(uint inputId, Keys keyLeft, Keys keyRight, Keys keyForward, Keys keyBackward) : this (inputId)
        {
            keyBinds = new Dictionary<Keys, uint>(4)
            {
                {keyForward, KEY_FORWARD},
                {keyRight, KEY_RIGHT},
                {keyBackward, KEY_BACKWARD},
                {keyLeft, KEY_LEFT}
            };
        }

        public Feeder(uint inputId) //From the vJoy SDK with adjustment from lopezk38
        {
            // Create one joystick object and a position structure.
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();

            // Device ID can only be in the range 1-16
            if (id <= 0 || id > 16)
            {
                Program.walkerWindowObj.ConsoleAddLine("Illegal device ID " + id.ToString() + " Exiting...");
                return;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                Program.walkerWindowObj.ConsoleAddLine("vJoy driver not enabled: Failed Getting vJoy attributes. Exiting...");
                return;
            }
            else
            {
                Program.walkerWindowObj.ConsoleAddLine("Vendor: " + joystick.GetvJoyManufacturerString());
                Program.walkerWindowObj.ConsoleAddLine("Product: " + joystick.GetvJoyProductString());
                Program.walkerWindowObj.ConsoleAddLine("Version Number: " + joystick.GetvJoySerialNumberString());
                Program.walkerWindowObj.ConsoleAddLine(Environment.NewLine);
            }

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " is already owned by this feeder\n");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " is free\n");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " is already owned by another feeder. Exiting...");
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " is not installed or disabled. Exiting...");
                    return;
                default:
                    Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " general error. Exiting...");
                    return;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);

            // Print results
            Program.walkerWindowObj.ConsoleAddLine(Environment.NewLine);
            Program.walkerWindowObj.ConsoleAddLine("vJoy Device " + id.ToString() + " capabilities:");
            if (AxisX) Program.walkerWindowObj.ConsoleAddLine("X axis present: Yes");
            else Program.walkerWindowObj.ConsoleAddLine("X axis present: No" + Environment.NewLine + "This will not work without the X axis");

            if (AxisY) Program.walkerWindowObj.ConsoleAddLine("Y axis present: Yes");
            else Program.walkerWindowObj.ConsoleAddLine("Y axis present: No" + Environment.NewLine + "This will not work without the Y axis");

            Program.walkerWindowObj.ConsoleAddLine(Environment.NewLine);

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Program.walkerWindowObj.ConsoleAddLine("Version of Driver Matches DLL Version " + DllVer.ToString());
            else
                Program.walkerWindowObj.ConsoleAddLine("Version of Driver (" + DrvVer.ToString() + ") does NOT match DLL Version (" + DllVer.ToString() + ")");


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Program.walkerWindowObj.ConsoleAddLine("Failed to acquire vJoy device number " + id.ToString() + ". Exiting...");
                return;
            }
            else
                Program.walkerWindowObj.ConsoleAddLine("Acquired: vJoy device number " + id.ToString());

            long maxVal = 0;

            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxVal);

            var kbHook = new KeyboardHook(true);
            kbHook.KeyDown += KbHook_KeyDown;
            kbHook.KeyUp += KbHook_KeyUp;

            UpdateAnalogValue();
            Program.walkerWindowObj.ConsoleAddLine("Feeder initialized successfully");
        }

        private void DeleteKeyVal(ref Dictionary<Keys, uint> dic, uint val)
        {
            foreach (KeyValuePair<Keys, uint> pair in dic)
            {
                if (pair.Value == val)
                {
                    dic.Remove(pair.Key);
                    break;
                }
            }
        }
    }

    public class KeyboardHook : IDisposable //By Wolf5 at https://stackoverflow.com/questions/34281223/c-sharp-hook-global-keyboard-events-net-4-0
    {
        bool Global = false;

        public delegate void LocalKeyEventHandler(Keys key, bool Shift, bool Ctrl, bool Alt);
        public event LocalKeyEventHandler KeyDown;
        public event LocalKeyEventHandler KeyUp;

        public delegate int CallbackDelegate(int Code, int W, int L);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct KBDLLHookStruct
        {
            public Int32 vkCode;
            public Int32 scanCode;
            public Int32 flags;
            public Int32 time;
            public Int32 dwExtraInfo;
        }

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(HookType idHook, CallbackDelegate lpfn, int hInstance, int threadId);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();

        public enum HookType : int
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

        private int HookID = 0;
        CallbackDelegate TheHookCB = null;

        //Start hook
        public KeyboardHook(bool Global)
        {
            this.Global = Global;
            TheHookCB = new CallbackDelegate(KeybHookProc);
            if (Global)
            {
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, TheHookCB,
                    0, //0 for local hook. eller hwnd til user32 for global
                    0); //0 for global hook. eller thread for hooken
            }
            else
            {
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD, TheHookCB,
                    0, //0 for local hook. or hwnd to user32 for global
                    GetCurrentThreadId()); //0 for global hook. or thread for the hook
            }
        }

        bool IsFinalized = false;
        ~KeyboardHook()
        {
            if (!IsFinalized)
            {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }
        public void Dispose()
        {
            if (!IsFinalized)
            {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }

        //The listener that will trigger events
        private int KeybHookProc(int Code, int W, int L)
        {
            KBDLLHookStruct LS = new KBDLLHookStruct();
            if (Code < 0)
            {
                return CallNextHookEx(HookID, Code, W, L);
            }
            try
            {
                if (!Global)
                {
                    if (Code == 3)
                    {
                        IntPtr ptr = IntPtr.Zero;

                        int keydownup = L >> 30;
                        if (keydownup == 0)
                        {
                            if (KeyDown != null) KeyDown((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                        }
                        if (keydownup == -1)
                        {
                            if (KeyUp != null) KeyUp((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                        }
                        //System.Diagnostics.Debug.WriteLine("Down: " + (Keys)W);
                    }
                }
                else
                {
                    KeyEvents kEvent = (KeyEvents)W;

                    Int32 vkCode = Marshal.ReadInt32((IntPtr)L); //Leser vkCode som er de første 32 bits hvor L peker.

                    if (kEvent != KeyEvents.KeyDown && kEvent != KeyEvents.KeyUp && kEvent != KeyEvents.SKeyDown && kEvent != KeyEvents.SKeyUp)
                    {
                    }
                    if (kEvent == KeyEvents.KeyDown || kEvent == KeyEvents.SKeyDown)
                    {
                        if (KeyDown != null) KeyDown((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    if (kEvent == KeyEvents.KeyUp || kEvent == KeyEvents.SKeyUp)
                    {
                        if (KeyUp != null) KeyUp((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                }
            }
            catch (Exception)
            {
                //Ignore all errors...
            }

            return CallNextHookEx(HookID, Code, W, L);

        }

        public enum KeyEvents
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SKeyDown = 0x0104,
            SKeyUp = 0x0105
        }

        [DllImport("user32.dll")]
        static public extern short GetKeyState(System.Windows.Forms.Keys nVirtKey);

        public static bool GetCapslock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.CapsLock)) & true;
        }
        public static bool GetNumlock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.NumLock)) & true;
        }
        public static bool GetScrollLock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.Scroll)) & true;
        }
        public static bool GetShiftPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.ShiftKey);
            if (state > 1 || state < -1) return true;
            return false;
        }
        public static bool GetCtrlPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.ControlKey);
            if (state > 1 || state < -1) return true;
            return false;
        }
        public static bool GetAltPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.Menu);
            if (state > 1 || state < -1) return true;
            return false;
        }
    }
}
