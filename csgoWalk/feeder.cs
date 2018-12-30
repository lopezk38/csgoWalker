using System;
using System.Collections.Generic;
using System.Text;
using vJoyInterfaceWrap;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RawInput_dll; //From https://www.codeproject.com/Articles/17123/%2FArticles%2F17123%2FUsing-Raw-Input-from-C-to-handle-multiple-keyboard

namespace csgoWalk
{
    public class Feeder
    {

        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        private const int centerVal = 16383;
        private const int offsetVal = 5926;
        private static readonly int[] lookUpX = new int[] 
        {centerVal,
         centerVal,
         centerVal,
         centerVal,
         centerVal + offsetVal,
         centerVal + (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal + (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal + offsetVal,
         centerVal - offsetVal,
         centerVal - (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal - (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal - offsetVal,
         centerVal,
         centerVal,
         centerVal,
         centerVal};

        private static readonly int[] lookUpY = new int[] 
        {centerVal,
         centerVal + offsetVal,
         centerVal - offsetVal,
         centerVal,
         centerVal,
         centerVal + (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal - (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal,
         centerVal,
         centerVal + (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal - (int)(offsetVal * (Math.Sqrt(2)/2)),
         centerVal,
         centerVal,
         centerVal + offsetVal,
         centerVal - offsetVal,
         centerVal};

        private static byte keysDown = 0b0000; // left, right, up, down

        private const uint KEY_FORWARD = 0;
        private const uint KEY_RIGHT = 1;
        private const uint KEY_BACKWARD = 2;
        private const uint KEY_LEFT = 3;

        private const uint FORWARD_OR = 0b0010;
        private const uint RIGHT_OR = 0b0100;
        private const uint BACKWARD_OR = 0b0001;
        private const uint LEFT_OR = 0b1000;

        private const uint FORWARD_AND = 0b1101;
        private const uint RIGHT_AND = 0b1011;
        private const uint BACKWARD_AND = 0b1110;
        private const uint LEFT_AND = 0b0111;

        private static Dictionary<uint, uint> keyBinds;

        private static void KbHook_KeyPress(object sender, InputEventArg e)
        {
            uint direction;
            if (keyBinds.TryGetValue((uint)e.KeyPressEvent.VKey, out direction))
            {
                switch (direction)
                {
                    case KEY_FORWARD:
                        if (e.KeyPressEvent.Message == Win32.WM_KEYDOWN) keysDown = (byte)(keysDown | FORWARD_OR);
                        else keysDown = (byte)(keysDown & FORWARD_AND);
                        break;
                    case KEY_RIGHT:
                        if (e.KeyPressEvent.Message == Win32.WM_KEYDOWN) keysDown = (byte)(keysDown | RIGHT_OR);
                        else keysDown = (byte)(keysDown & RIGHT_AND);
                        break;
                    case KEY_BACKWARD:
                        if (e.KeyPressEvent.Message == Win32.WM_KEYDOWN) keysDown = (byte)(keysDown | BACKWARD_OR);
                        else keysDown = (byte)(keysDown & BACKWARD_AND);
                        break;
                    case KEY_LEFT:
                        if (e.KeyPressEvent.Message == Win32.WM_KEYDOWN) keysDown = (byte)(keysDown | LEFT_OR);
                        else keysDown = (byte)(keysDown & LEFT_AND);
                        break;
                }
                UpdateAnalogValue();
            }
        }

        private static void UpdateAnalogValue()
        {
            iReport.bDevice = (byte)id;
            iReport.AxisX = lookUpX[keysDown];
            iReport.AxisY = lookUpY[keysDown];

            /*
            if (((byte)(leftRightKeyDown & forwardBackKeyDown)) == (byte)0x11) iReport.Buttons = (uint)(0x1000);
            else iReport.Buttons = (uint)(0x0000);
            */

            if (!joystick.UpdateVJD(id, ref iReport))
            {
                Program.walkerWindowObj.ConsoleAddLine("Feeding vJoy device number " + id.ToString() + " failed - Is the device enabled?");
                joystick.AcquireVJD(id);
            }
        }

        public void SetKeyBind(string direction, uint key)
        {
            switch (direction)
            {
                case "left":
                    keysDown = (byte)(keysDown & 0x0111);
                    DeleteKeyVal(ref keyBinds, KEY_LEFT);
                    keyBinds.Add(key, KEY_LEFT);
                    break;
                case "right":
                    keysDown = (byte)(keysDown & 0x1011);
                    DeleteKeyVal(ref keyBinds, KEY_RIGHT);
                    keyBinds.Add(key, KEY_RIGHT);
                    break;
                case "forward":
                    keysDown = (byte)(keysDown & 0x1101);
                    DeleteKeyVal(ref keyBinds, KEY_FORWARD);
                    keyBinds.Add(key, KEY_FORWARD);
                    break;
                case "backward":
                    keysDown = (byte)(keysDown & 0x1110);
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
            keyBinds = new Dictionary<uint, uint>(4)
            {
                {0x54, KEY_FORWARD}, //https://docs.microsoft.com/en-us/windows/desktop/inputdev/virtual-key-codes
                {0x48, KEY_RIGHT},
                {0x47, KEY_BACKWARD},
                {0x46, KEY_LEFT}
            };
        }

        public Feeder(uint inputId, uint keyLeft, uint keyRight, uint keyForward, uint keyBackward) : this (inputId)
        {
            keyBinds = new Dictionary<uint, uint>(4)
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

            RawInput kbHook = new RawInput(Program.walkerWindowObj.HWND);
            kbHook.CaptureOnlyIfTopMostWindow = false;
            kbHook.KeyPressed += KbHook_KeyPress;

            iReport.Buttons = (uint)(0x0000);
            UpdateAnalogValue();
            Program.walkerWindowObj.ConsoleAddLine("Feeder initialized successfully");
        }

        private void DeleteKeyVal(ref Dictionary<uint, uint> dic, uint val)
        {
            foreach (KeyValuePair<uint, uint> pair in dic)
            {
                if (pair.Value == val)
                {
                    dic.Remove(pair.Key);
                    break;
                }
            }
        }
    }
}
