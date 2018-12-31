using System;
using System.Collections.Generic;
using vJoyInterfaceWrap;
using RawInput_dll; //From https://www.codeproject.com/Articles/17123/%2FArticles%2F17123%2FUsing-Raw-Input-from-C-to-handle-multiple-keyboard
using System.Threading;
using System.Threading.Tasks;

namespace csgoWalk
{
    public class Feeder
    {

        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        private const int centerVal = 16383;
        private const int offsetVal = 5750;

        //centerVal + (int)(offsetVal * (Math.Sqrt(2)/2))
        private static readonly int[] lookUpX = new int[] 
        {centerVal,
         centerVal,
         centerVal,
         centerVal,
         centerVal + offsetVal,
         centerVal + (int)(offsetVal/1.3),
         centerVal + (int)(offsetVal/1.3),
         centerVal + offsetVal,
         centerVal - offsetVal,
         centerVal - (int)(offsetVal/1.3),
         centerVal - (int)(offsetVal/1.3),
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
         centerVal + (int)(offsetVal/1.3),
         centerVal - (int)(offsetVal/1.3),
         centerVal,
         centerVal,
         centerVal + (int)(offsetVal/1.3),
         centerVal - (int)(offsetVal/1.3),
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

        private const uint GET_LSB = BACKWARD_OR;

        private static Task taskThread = Task.Factory.StartNew(() => {});

        private static Dictionary<uint, uint> keyBinds;

        private static void KbHook_KeyPress(object sender, InputEventArg e)
        {
            uint direction;
            if (keyBinds.TryGetValue((uint)e.KeyPressEvent.VKey, out direction))
            {
                UpdateKeysDown(direction, e.KeyPressEvent.Message);
            }
        }

        private static void UpdateAnalogValue()
        {
            iReport.bDevice = (byte)id;
            iReport.AxisX = centerVal;
            iReport.AxisY = centerVal;
            joystick.UpdateVJD(id, ref iReport);

            System.Threading.Thread.Sleep(60); //prevent single footstep during transition

            iReport.bDevice = (byte)id;
            iReport.AxisX = lookUpX[keysDown];
            iReport.AxisY = lookUpY[keysDown];

            /*
            if (((byte)(leftRightKeyDown & forwardBackKeyDown)) == (byte)0x11) iReport.Buttons = (uint)(0x1000);
            else iReport.Buttons = (uint)(0x0000);
            */

            if (!joystick.UpdateVJD(id, ref iReport))
            {

                SendConsoleText("Feeding vJoy device number " + id.ToString() + " failed - Is the device enabled?");
                joystick.AcquireVJD(id);
            }
        }

        private static void SendConsoleText(string str)
        {
            Program.walkerWindowObj.invokerControl.Invoke(new Action(() => Program.walkerWindowObj.ConsoleAddLine(str)));
        }

        private static void UpdateKeysDown(uint direction, uint keyDownMes)
        {
            bool keyDown;
            if (keyDownMes == Win32.WM_KEYDOWN) keyDown = true;
            else keyDown = false;

            switch (direction)
            {
                case KEY_FORWARD:
                    if (keyDown == Convert.ToBoolean((keysDown >> 1) & GET_LSB)) break;
                    if (keyDown) {keysDown = (byte)(keysDown | FORWARD_OR); Program.walkerWindowObj.SetBindButtonColor((int)KEY_FORWARD, true);}
                    else {keysDown = (byte)(keysDown & FORWARD_AND); Program.walkerWindowObj.SetBindButtonColor((int)KEY_FORWARD, false);}
                    taskThread.ContinueWith(delegate {UpdateAnalogValue();}); //Add UpdateAnalogValue to taskThread schedule to run syncronously
                    break;
                case KEY_RIGHT:
                    if (keyDown == Convert.ToBoolean((keysDown >> 2) & GET_LSB)) break;
                    if (keyDown) {keysDown = (byte)(keysDown | RIGHT_OR); Program.walkerWindowObj.SetBindButtonColor((int)KEY_RIGHT, true);}
                    else {keysDown = (byte)(keysDown & RIGHT_AND); Program.walkerWindowObj.SetBindButtonColor((int)KEY_RIGHT, false);}
                    taskThread.ContinueWith(delegate {UpdateAnalogValue();}); //Add UpdateAnalogValue to taskThread schedule to run syncronously
                    break;
                case KEY_BACKWARD:
                    if (keyDown == Convert.ToBoolean(keysDown & GET_LSB)) break;
                    if (keyDown) {keysDown = (byte)(keysDown | BACKWARD_OR); Program.walkerWindowObj.SetBindButtonColor((int)KEY_BACKWARD, true);}
                    else {keysDown = (byte)(keysDown & BACKWARD_AND); Program.walkerWindowObj.SetBindButtonColor((int)KEY_BACKWARD, false);}
                    taskThread.ContinueWith(delegate {UpdateAnalogValue();}); //Add UpdateAnalogValue to taskThread schedule to run syncronously
                    break;
                case KEY_LEFT:
                    if (keyDown == Convert.ToBoolean((keysDown >> 3) & GET_LSB)) break;
                    if (keyDown) {keysDown = (byte)(keysDown | LEFT_OR); Program.walkerWindowObj.SetBindButtonColor((int)KEY_LEFT, true);}
                    else {keysDown = (byte)(keysDown & LEFT_AND); Program.walkerWindowObj.SetBindButtonColor((int)KEY_LEFT, false);}
                    taskThread.ContinueWith(delegate {UpdateAnalogValue();}); //Add UpdateAnalogValue to taskThread schedule to run syncronously
                    break;
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

        public int GetKeyBind(uint direction)
        {
            foreach (KeyValuePair<uint, uint> pair in keyBinds)
            {
                if (pair.Value == direction)
                {
                    return (int)pair.Key;
                }
            }
            return -1;
        }

        public int GetKeyBind(string strDir)
        {
            uint dir = 0xFFFF;
            switch (strDir)
            {
                case "forward":
                    dir = KEY_FORWARD;
                    break;
                case "right":
                    dir = KEY_RIGHT;
                    break;
                case "backward":
                    dir = KEY_BACKWARD;
                    break;
                case "left":
                    dir = KEY_LEFT;
                    break;
                default:
                    break;
            }
            if (dir == 0xFFFF)
            {
                return -1;
            }
            return GetKeyBind(strDir);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            joystick.RelinquishVJD(id);
            SendConsoleText("Relinquishing joystick...");
        }

        public Feeder() : this(1) //calls uint constructor with args of 1
        {
            keyBinds = new Dictionary<uint, uint>(4)
            {
                {0x57, KEY_FORWARD}, //https://docs.microsoft.com/en-us/windows/desktop/inputdev/virtual-key-codes
                {0x44, KEY_RIGHT},
                {0x53, KEY_BACKWARD},
                {0x41, KEY_LEFT}
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
                SendConsoleText("Illegal device ID " + id.ToString() + " Exiting...");
                return;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                SendConsoleText("vJoy driver not enabled: Failed Getting vJoy attributes. Exiting...");
                return;
            }
            else
            {
                SendConsoleText("Vendor: " + joystick.GetvJoyManufacturerString());
                SendConsoleText("Product: " + joystick.GetvJoyProductString());
                SendConsoleText("Version Number: " + joystick.GetvJoySerialNumberString());
                SendConsoleText(Environment.NewLine);
            }

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    SendConsoleText("vJoy Device " + id.ToString() + " is already owned by this feeder\n");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    SendConsoleText("vJoy Device " + id.ToString() + " is free\n");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    SendConsoleText("vJoy Device " + id.ToString() + " is already owned by another feeder. Exiting...");
                    return;
                case VjdStat.VJD_STAT_MISS:
                    SendConsoleText("vJoy Device " + id.ToString() + " is not installed or disabled. Exiting...");
                    return;
                default:
                    SendConsoleText("vJoy Device " + id.ToString() + " general error. Exiting...");
                    return;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);

            // Print results
            SendConsoleText(Environment.NewLine);
            SendConsoleText("vJoy Device " + id.ToString() + " capabilities:");
            if (AxisX) SendConsoleText("X axis present: Yes");
            else SendConsoleText("X axis present: No" + Environment.NewLine + "This will not work without the X axis");

            if (AxisY) SendConsoleText("Y axis present: Yes");
            else SendConsoleText("Y axis present: No" + Environment.NewLine + "This will not work without the Y axis");

            SendConsoleText(Environment.NewLine);

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                SendConsoleText("Version of Driver Matches DLL Version " + DllVer.ToString());
            else
                SendConsoleText("Version of Driver (" + DrvVer.ToString() + ") does NOT match DLL Version (" + DllVer.ToString() + ")");


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                SendConsoleText("Failed to acquire vJoy device number " + id.ToString() + ". Exiting...");
                return;
            }
            else
                SendConsoleText("Acquired: vJoy device number " + id.ToString());

            long maxVal = 0;

            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxVal);

            RawInput kbHook = new RawInput(Program.walkerWindowObj.HWND)
            {
                CaptureOnlyIfTopMostWindow = false
            };
            kbHook.KeyPressed += KbHook_KeyPress;

            iReport.Buttons = (uint)(0x0000);
            UpdateAnalogValue();
            SendConsoleText("Feeder initialized successfully");
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
