using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialPortDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private SerialPort sp = null; //串口实例
        private byte[] data = new byte[3] { 0x00, 0xff, 0x3a }; //需要发送的数据
        private Thread sendThread = null; //数据发送线程

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //实例化串口
            sp = new SerialPort
            {
                PortName = "COM2",//串口号
                BaudRate = 9600, //波特率
                DataBits = 8, //数据位
                StopBits = StopBits.One, //停止位
                Parity = Parity.None, //奇偶校验
                ReadTimeout = Timeout.Infinite, //读取超时
                WriteTimeout = Timeout.Infinite, //写入超时
                RtsEnable = true, //根据实际情况启用
            };
            sp.DataReceived += Sp_DataReceived; //注册数据接收事件

            try
            {
                sp.Open(); //打开串口
                sendThread = new Thread(delegate ()
                {
                    while (true)
                    {
                        sp.Write(data, 0, data.Length); //发送数据 
                        Thread.Sleep(500); //线程等待时间
                    }
                });
                sendThread.IsBackground = true; //设置为后台线程
                sendThread.Start(); //发送线程开始
            }
            catch (Exception)
            {
                Console.WriteLine("打开串口失败");
            }
        }

        /// <summary>
        /// SerialPort 接收到数据时执行
        /// </summary>
        /// <param name="sender">事件发起者</param>
        /// <param name="e">事件参数</param>
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sp.IsOpen)
            {
                int n = sp.BytesToRead; //获取缓冲区数据的字节数
                byte[] buff = new byte[n]; //新建byte[]准备接收数据
                sp.Read(buff, 0, n); //接收数据

                //输出接收到的数据
                string temp = "";
                foreach (var item in buff)
                {
                    temp += item + " ";
                }
                if (buff.Length > 0)
                {
                    Console.WriteLine(temp);

                }
            }
            else
            {
                Console.WriteLine("请先打开串口");
            }
        }
    }
}
