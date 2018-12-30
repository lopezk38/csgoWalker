using System;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;
using System.Drawing;
using RawInput_dll; //From https://www.codeproject.com/Articles/17123/%2FArticles%2F17123%2FUsing-Raw-Input-from-C-to-handle-multiple-keyboard
using System.Threading.Tasks;

namespace csgoWalk
{
    public partial class walkerWindow : Form
    {
        public Feeder Feeder;
        public IntPtr HWND;

        private string saveDataPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\keyBinds.xml";

        private ManualResetEvent waitForFeeder = new ManualResetEvent(false);
        private ManualResetEvent waitForFileOp = new ManualResetEvent(true);
        private ManualResetEvent waitForKeyPress = new ManualResetEvent(false);
        private ManualResetEvent keyPressEventLock = new ManualResetEvent(true);
        private ManualResetEvent saveAllPacerLock = new ManualResetEvent(false);

        private const int KEY_FORWARD = 0;
        private const int KEY_RIGHT = 1;
        private const int KEY_BACKWARD = 2;
        private const int KEY_LEFT = 3;

        private const int NODE_COUNT = 4; //Number of keybinds

        RawInput keyBoardRawInput;
        private uint lastKeyPress;

        public walkerWindow()
        {
            InitializeComponent();
            ConsoleAddLine("Initialized");
        }

        public void ConsoleAddLine(string line)
        {
            consoleBox.AppendText(line);
            consoleBox.AppendText(Environment.NewLine);
        }

        public void SetBindButtonColor(int dir, bool keyDown)
        {
            switch (dir)
            {
                case KEY_FORWARD:
                    if (keyDown) upBind.BackColor = SystemColors.Highlight;
                    else upBind.BackColor = SystemColors.ControlLight;
                    break;
                case KEY_RIGHT:
                    if (keyDown) rightBind.BackColor = SystemColors.Highlight;
                    else rightBind.BackColor = SystemColors.ControlLight;
                    break;
                case KEY_BACKWARD:
                    if (keyDown) downBind.BackColor = SystemColors.Highlight;
                    else downBind.BackColor = SystemColors.ControlLight;
                    break;
                case KEY_LEFT:
                    if (keyDown) leftBind.BackColor = SystemColors.Highlight;
                    else leftBind.BackColor = SystemColors.ControlLight;
                    break;
                default:
                    break;
            }
        }

        private void walkerWindow_Shown(object sender, EventArgs e)
        {
            HWND = this.Handle;
            consoleBox.Select();
            Feeder = new Feeder();
            waitForFeeder.Set();
            Task.Factory.StartNew(() => LoadSavedBinds()); //Spawn LoadSaveBinds() on new thread so we don't block the UI thread
        }

        private void LoadSavedBinds()
        {
            waitForFileOp.WaitOne(); //Check if file is being accessed. Wait until it's free if it isn't.
            waitForFileOp.Reset(); //Lock file

            consoleBox.Invoke(new Action(() => ConsoleAddLine("Loading saved keybinds...")));
            if (Feeder == null)
            {
                consoleBox.Invoke(new Action(() => ConsoleAddLine("Waiting for Feeder to finish initialization before loading binds...")));
                waitForFeeder.WaitOne(); //wait for Feeder to be initialized
                waitForFeeder.Reset();
                consoleBox.Invoke(new Action(() => ConsoleAddLine("Feeder loaded, loading binds...")));
            }

            if (File.Exists(saveDataPath))
            {
                //load parameters
                string[] xmlOut = new string[NODE_COUNT];
                uint[] parsedXmlOut = new uint[NODE_COUNT];
                XmlDocument saveDataxml = new XmlDocument();
                saveDataxml.Load(saveDataPath);
                for (int i = 0; i < NODE_COUNT; ++i)
                {
                    xmlOut[i] = saveDataxml.DocumentElement.ChildNodes[i].InnerText;
                }

                for (int i = 0; i < NODE_COUNT; ++i)
                {
                    int curData = Int32.Parse(xmlOut[i]);
                    string curString = System.Windows.Input.KeyInterop.KeyFromVirtualKey(curData).ToString("G"); //"G" for general format. Look up enum format strings
                    if (curData >= 0x08 && curData <= 0xFE && curData != 0x07 && curData != 0x0A && curData != 0x0B && curData != 0x0E && curData != 0x0F && curData != 0x16 &&
                        curData != 0x1A && curData != 0x5E && curData != 0xB8 && curData != 0xB9 && curData != 0xE0 && curData != 0xE8 && curData != 0xFC && !(curData >= 0x3A &&
                        curData <= 0x40) && !(curData >= 0x88 && curData <= 0x8F) && !(curData >= 0x97 && curData <= 0x9F) && !(curData >= 0xC1 && curData <= 0xDA))
                    { //Key exists/isn't blacklisted
                        switch (i)
                        {
                            case KEY_FORWARD:
                                Feeder.SetKeyBind("forward", (uint)curData);
                                consoleBox.Invoke(new Action(() => upBind.Text = curString));
                                break;
                            case KEY_RIGHT:
                                Feeder.SetKeyBind("right", (uint)curData);
                                consoleBox.Invoke(new Action(() => rightBind.Text = curString));
                                break;
                            case KEY_BACKWARD:
                                Feeder.SetKeyBind("backward", (uint)curData);
                                consoleBox.Invoke(new Action(() => downBind.Text = curString));
                                break;
                            case KEY_LEFT:
                                Feeder.SetKeyBind("left", (uint)curData);
                                consoleBox.Invoke(new Action(() => leftBind.Text = curString));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        consoleBox.Invoke(new Action(() => ConsoleAddLine("Invalid keycode for direction" + i.ToString())));
                    }
                }
            }
            else
            {
                //file does not exist
                CreateNewSaveFile();
            }
            waitForFileOp.Set(); //Unlock file
        }

        private bool SaveBinds(uint keyDir, uint key, bool recursive)
        {
            if (!recursive)
            {
                waitForFileOp.WaitOne(); //Check if file is being accessed. Wait until it's free if it isn't.
                waitForFileOp.Reset(); //Lock file
            }

            if (keyDir > 3)
            {
                consoleBox.Invoke(new Action(() => ConsoleAddLine("Invalid key direction given to SaveBinds. Keybind not saved.")));
                waitForFileOp.Set(); //Unlock file
                return false;
            }

            if (File.Exists(saveDataPath))
            {
                consoleBox.Invoke(new Action(() => ConsoleAddLine("Saving keybind...")));
                if (key >= 0x08 && key <= 0xFE && key != 0x07 && key != 0x0A && key != 0x0B && key != 0x0E && key != 0x0F && key != 0x16 &&
                key != 0x1A && key != 0x5E && key != 0xB8 && key != 0xB9 && key != 0xE0 && key != 0xE8 && key != 0xFC && !(key >= 0x3A &&
                key <= 0x40) && !(key >= 0x88 && key <= 0x8F) && !(key >= 0x97 && key <= 0x9F) && !(key >= 0xC1 && key <= 0xDA))
                { //Key exists and is not blacklisted
                    string curString = System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)key).ToString("G"); //"G" for general format. Look up enum format strings
                    switch (keyDir)
                    {
                        case KEY_FORWARD:
                            Feeder.SetKeyBind("forward", key);
                            consoleBox.Invoke(new Action(() => upBind.Text = curString));
                            break;
                        case KEY_RIGHT:
                            Feeder.SetKeyBind("right", key);
                            consoleBox.Invoke(new Action(() => rightBind.Text = curString));
                            break;
                        case KEY_BACKWARD:
                            Feeder.SetKeyBind("backward", key);
                            consoleBox.Invoke(new Action(() => downBind.Text = curString));
                            break;
                        case KEY_LEFT:
                            Feeder.SetKeyBind("left", key);
                            consoleBox.Invoke(new Action(() => leftBind.Text = curString));
                            break;
                        default:
                            break;
                    }

                    XmlDocument saveDataxml = new XmlDocument();
                    saveDataxml.Load(saveDataPath);
                    saveDataxml.DocumentElement.ChildNodes[(int)keyDir].InnerText = key.ToString();
                    saveDataxml.Save(saveDataPath);
                    consoleBox.Invoke(new Action(() => ConsoleAddLine("Save completed")));
                    waitForFileOp.Set(); //Unlock file
                    return true;
                }
                else
                {
                    consoleBox.Invoke(new Action(() => ConsoleAddLine("Invalid key. Keybind not saved.")));
                    waitForFileOp.Set(); //Unlock file
                    return false;
                }
            }
            else
            {
                CreateNewSaveFile();
                return SaveBinds(keyDir, key, true);
            }
        }

        private void CreateNewSaveFile()
        {
            consoleBox.Invoke(new Action(() => ConsoleAddLine("Creating new save file...")));
            XmlDocument saveDataxml = new XmlDocument();
            XmlNode saveRootNode = saveDataxml.CreateElement("WALKERBINDS");
            saveDataxml.AppendChild(saveRootNode);

            XmlNode bindNode;
            string[] element = new string[] { "UP", "RIGHT", "DOWN", "LEFT" };
            for (int i = 0; i < NODE_COUNT; ++i)
            {
                bindNode = saveDataxml.CreateElement("BIND");
                XmlAttribute bindKey = saveDataxml.CreateAttribute("FUNCTION");
                bindKey.Value = element[i];
                bindNode.Attributes.Append(bindKey);
                bindNode.InnerText = Feeder.GetKeyBind((uint)i).ToString();
                saveRootNode.AppendChild(bindNode);
            }
            saveDataxml.Save(saveDataPath);
            consoleBox.Invoke(new Action(() => ConsoleAddLine("Save file created")));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => SaveAll()); //Call SaveAll() on new thread so we don't block the UI thread
        }

        private void SaveAll()
        {
            SaveBinds(KEY_FORWARD, (uint)Feeder.GetKeyBind(KEY_FORWARD), false);
            SaveBinds(KEY_RIGHT, (uint)Feeder.GetKeyBind(KEY_RIGHT), false);
            SaveBinds(KEY_BACKWARD, (uint)Feeder.GetKeyBind(KEY_BACKWARD), false);
            SaveBinds(KEY_LEFT, (uint)Feeder.GetKeyBind(KEY_LEFT), false);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => LoadSavedBinds()); //Call LoadSavedBinds() on new thread so we don't block the UI thread
        }

        private void KbHook_KeyPress(object sender, InputEventArg e)
        {
            lastKeyPress = (uint)e.KeyPressEvent.VKey;
            ((RawKeyboard)sender).KeyPressed -= KbHook_KeyPress;
            sender = null;
            waitForKeyPress.Set(); //Allow GetKeyPress to continue
        }

        private delegate string GetButtonTextDelegate(Button button);
        private delegate void SetButtonTextDelegate(Button button, string text);

        private string GetButtonText(Button button)
        {
            return button.Text;
        }

        private void SetButtonText(Button button, string text)
        {
            button.Text = text;
        }

        private RawInput GetKeyPressHelper()
        {
            keyBoardRawInput = new RawInput(Program.walkerWindowObj.HWND)
            {
                CaptureOnlyIfTopMostWindow = false
            };
            keyBoardRawInput.KeyPressed += KbHook_KeyPress;
            return keyBoardRawInput;
        }

        private void GetKeyPress(uint direction, Button button)
        {
            keyPressEventLock.WaitOne(); //Check if another button is waiting on a keypress, wait for them to get it
            keyPressEventLock.Reset(); //Lock keypress to this call


            string initButText = (string)this.Invoke(new GetButtonTextDelegate(GetButtonText), button);

            Task.Factory.StartNew(() => GetKeyPressHelper()); //Spawn RawInput object on new thread
            consoleBox.Invoke(new Action(() => ConsoleAddLine("Waiting for keypress...")));
            Invoke(new SetButtonTextDelegate(SetButtonText), button, "Press a key");
            waitForKeyPress.WaitOne(); //Wait for kbHook_KeyPress to return a keypress
            waitForKeyPress.Reset(); //Prepare kbHook_KeyPress for next time

            if (!SaveBinds(direction, lastKeyPress, false))
            {
                Invoke(new SetButtonTextDelegate(SetButtonText), button, initButText);
            }

            keyPressEventLock.Set(); //Unlock keypress to this call
        }

        private void upBind_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => GetKeyPress(KEY_FORWARD, this.upBind)); //Start new async task running method GetKeyPress()
        }

        private void leftBind_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => GetKeyPress(KEY_LEFT, this.leftBind)); //Start new async task running method GetKeyPress()
        }

        private void downBind_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => GetKeyPress(KEY_BACKWARD, this.downBind)); //Start new async task running method GetKeyPress()
        }

        private void rightBind_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => GetKeyPress(KEY_RIGHT, this.rightBind)); //Start new async task running method GetKeyPress()
        }
    }
}
