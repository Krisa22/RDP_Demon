﻿using System;
using System.Text;
using System.Windows.Forms;
using AxRDPCOMAPILib;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Collections.Generic;
using VncSharp;
using System.Drawing;

namespace Simple_RDP_Client
{
    public partial class RDP_Client : Form
    {

        IPHostEntry host1 = Dns.GetHostEntry(Dns.GetHostName());
        string adr;
        string stream = "";
        int zagr = 0;
        string combo;
        string ip;
        string trigger = "";
        bool vncON = false;
        int purport = 0;
       
        List<Socket> clientSockets;
        public RDP_Client()
        {
            InitializeComponent();
            axRDPViewer.Visible = false;
            pictureBox1.Enabled = false;
            pictureBox1.Visible = false;
            button3.Enabled = false;
            Thread myThread = new Thread(new ThreadStart(Slushaem));
            myThread.Start();
            System.Windows.Forms.ToolTip t = new System.Windows.Forms.ToolTip();
            t.SetToolTip(button1, "Среднее время поиска 5 минут");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            comboBox2.Visible = false;
            pictureBox3.Visible = false;
            if(Properties.Settings.Default.vnc == "1")
            {
                axRDPViewer.Visible = false;
                listBox1.Visible = false;
                button3.Visible = false;
                label1.Visible = true;
                pictureBox1.Visible = false;
                button1.Visible = false;
                textBox2.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                pictureBox3.Visible = true;
                panel1.Visible = false;
                pictureBox4.Visible = false;
                textBox1.Location = new Point(239, 132);
                button2.Location = new Point(243, 161);
                label1.Location = new Point(217, 136);
                comboBox2.Visible = true;
                comboBox2.Location = new Point(338, 132);
                vncON = true;
                label4.Text = "RDP MOD";
            }
        }
        public static void Connect(string invitation, AxRDPViewer display, string userName, string password)
        {
            display.SmartSizing = true;
            display.Connect(invitation, userName, password);
          
        }

        public static void disconnect(AxRDPViewer display)
        {
            display.Disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Стоп")
            {
                zagr = 6;
                pictureBox1.Enabled = false;
                pictureBox1.Visible = false;
                button1.Text = "Поиск";
            }
            else
            {
                zagr = 0;
                listBox1.Items.Clear();
                button1.Text = "Стоп";
                combo = textBox2.Text;
                pictureBox1.Enabled = true;
                pictureBox1.Visible = true;
                Thread myThread2 = new Thread(new ThreadStart(Zapros));
                myThread2.Start();
            }


        }
        void Slushaem()
        {
            TcpListener Listen_Data;
            Listen_Data = new TcpListener(IPAddress.Any, 4000);
            Listen_Data.Start();
            Socket ReceiveSocket = Listen_Data.AcceptSocket();
            Byte[] Receive = new Byte[4086];
            using (MemoryStream MessageR = new MemoryStream())
            {
                //Количество считанных байт
                Int32 ReceivedBytes;
                do
                {//Собственно читаем
                    ReceivedBytes = ReceiveSocket.Receive(Receive, Receive.Length, 0);
                    //и записываем в поток
                    MessageR.Write(Receive, 0, ReceivedBytes);
                    stream = Encoding.ASCII.GetString(Receive, 0, ReceivedBytes);
                    //Читаем до тех пор, пока в очереди не останется данных
                } while (ReceiveSocket.Available > 0);
               
                    Invoke((MethodInvoker)delegate
                    {
                        if (trigger == "1")
                        {
                            trigger = "";
                            Thread myThread10 = new Thread(new ThreadStart(zap));
                            myThread10.Start();
                        }
                        string p = ReceiveSocket.RemoteEndPoint.ToString();
                        p = p.Substring(0, p.LastIndexOf(':') + 1);
                        p = p.Replace(":", "");
                        listBox1.Items.Add(p);
                        
                    });
                
                if (stream != "")
                {
                    Listen_Data.Stop();
                    Slushaem();
                }
            }
        }
        void zap()
        {
            Invoke((MethodInvoker)delegate
            {
                try
                {
                    listBox1.Visible = false;
                    axRDPViewer.Visible = true;
                    label2.Visible = false;
                    textBox2.Visible = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    textBox1.Visible = false;
                    label3.Visible = false;
                    pictureBox1.Visible = false;
                    panel1.Visible = false;
                    panel2.Visible = false;
                    button3.Visible = false;
                    label1.Visible = false;
                    pictureBox2.Visible = false;
                    this.AutoSize = true;
                    this.MaximizeBox = true;
                    label4.Visible = false;
                    this.WindowState = FormWindowState.Maximized;
                    Connect(stream, this.axRDPViewer, "", "");
                }
                catch
                {
                    MessageBox.Show("Нет доступа к ПК!");
                }

            });

        }
       async void Zapros()
        {
            for (int i = 131; i <= 254; i++)
            {
               
                Ping ping = new  Ping();
                PingReply pingresult = ping.Send("192.168." + combo + "." + i, 200);
                if (pingresult != null)
                {
                    foreach (IPAddress ip in host1.AddressList)
                        adr = ip.ToString();
                    Byte[] SendBytes = Encoding.Default.GetBytes(adr);
                    IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("192.168." + combo + "." + i), 5001); // берется с Textbox-a text_IP адрес другого компьютера
                    Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    try {

                        if (!Connector.Poll(50, SelectMode.SelectError))
                        {
                            Connector.Connect(EndPoint);
                            Connector.Send(SendBytes);
                            Connector.Close();
                        }
                    }
                   catch
                    {

                    }
                    if (i == 150)
                    {
                        zagr += 1;
                    }
                    if (zagr == 6)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            button1.Text = "Поиск";
                            pictureBox1.Enabled = false;
                            pictureBox1.Visible = false;
                        });
                    }
                }

            }

        }
       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.vnc == "1")
            {
                vncON = false;
            }
            else
            {
                vncON = true;
            }
            Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RDP Demon v3.0.0 ");
        }
        void ZaprosRU()
        {
            Invoke((MethodInvoker)delegate
            {
                Ping ping = new Ping();
            PingReply pingresult = ping.Send(ip, 200);
            if (pingresult != null)
            {

               String host = System.Net.Dns.GetHostName();
               System.Net.IPAddress adr = System.Net.Dns.GetHostByName(host).AddressList[0];
               Byte[] SendBytes = Encoding.Default.GetBytes(adr.ToString());
               IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(ip), 5001); // берется с Textbox textIP адрес другого компьютера
               Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
           
                try
                {
                    Connector.Connect(EndPoint);
                    Connector.Send(SendBytes);
                    Connector.Close();
                    listBox1.Items.Clear();
                        purport = 1;
                        trigger = "1";
                }
                catch
                {}

            }
            });
        }
       private void linux()
        {
            Invoke((MethodInvoker)delegate
            {
                if (comboBox2.Text == "0")
                {
                    RemoteDesktop1.VncPort = 5900;
                }
                if (comboBox2.Text == "1")
                {
                    RemoteDesktop1.VncPort = 5901;
                }
                if (comboBox2.Text == "2")
                {
                    RemoteDesktop1.VncPort = 5902;
                }
                if (comboBox2.Text == "3")
                {
                    RemoteDesktop1.VncPort = 5903;
                }
                if (comboBox2.Text == "4")
                {
                    RemoteDesktop1.VncPort = 5904;
                }
                if (comboBox2.Text == "5")
                {
                    RemoteDesktop1.VncPort = 5905;
                }
                if (comboBox2.Text == "6")
                {
                    RemoteDesktop1.VncPort = 5906;
                }
                axRDPViewer.Visible = false;
                RemoteDesktop1.Visible = true;
                pictureBox3.Visible = false;
                RemoteDesktop1.Connect(textBox1.Text);
                RemoteDesktop1.SetScalingMode(true);
                RemoteDesktop1.AutoScroll = true;
                RemoteDesktop1.Dock = System.Windows.Forms.DockStyle.Fill;
                listBox1.Visible = false;
                axRDPViewer.Visible = true;
                label2.Visible = false;
                textBox2.Visible = false;
                button1.Visible = false;
                button2.Visible = false;
                textBox1.Visible = false;
                label3.Visible = false;
                pictureBox1.Visible = false;
                panel1.Visible = false;
                panel2.Visible = false;
                button3.Visible = false;
                label1.Visible = false;
                comboBox2.Visible = false;
                pictureBox2.Visible = false;
                this.AutoSize = true;
                this.MaximizeBox = true;
                label4.Visible = false;
                this.WindowState = FormWindowState.Maximized;
            });
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ip = listBox1.SelectedItem.ToString();
            Thread myThread8 = new Thread(new ThreadStart(ZaprosRU));
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ip = textBox1.Text;
            if(vncON == true)
            {
                Thread myThread8 = new Thread(new ThreadStart(linux));
                myThread8.Start();

            }
            else
            {
                Thread myThread8 = new Thread(new ThreadStart(ZaprosRU));
                myThread8.Start();
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if(vncON == false)
            {
                axRDPViewer.Visible = false;
                listBox1.Visible = false;
                button3.Visible = false;
                label1.Visible = true;
                pictureBox1.Visible = false;
                button1.Visible = false;
                textBox2.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                pictureBox3.Visible = true;
                pictureBox4.Visible = false;
                panel1.Visible = false;
                textBox1.Location = new Point(239, 132);
                button2.Location = new Point(243, 161);
                label1.Location = new Point(217, 136);
                comboBox2.Visible = true;
                comboBox2.Location = new Point(338, 132);
                vncON = true;
                label4.Text = "RDP MOD";
            }
            else
            {
                Properties.Settings.Default.vnc = "0";
                Properties.Settings.Default.Save();
                axRDPViewer.Visible = true;
                listBox1.Visible = true;
                button3.Visible = true;
                label1.Visible = true;
                button1.Visible = true;
                textBox2.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                panel1.Visible = true;
                pictureBox4.Visible = true;
                pictureBox3.Visible = false;
                textBox1.Location = new Point(283, 132);
                button2.Location = new Point(287, 161);
                label1.Location = new Point(265, 136);
                comboBox2.Visible = false;
                comboBox2.Location = new Point(383, 132);
                vncON = false;
                label4.Text = "VNC MOD";
            }
        }

        private void RemoteDesktop1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(label4.Text == "RDP MOD")
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Properties.Settings.Default.vnc = "1";
                    Properties.Settings.Default.Save();
                    Application.Exit();
                }
                
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (label4.Text == "RDP MOD")
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Properties.Settings.Default.vnc = "1";
                    Properties.Settings.Default.Save();
                    Application.Exit();
                }

            }
        }
    }
}