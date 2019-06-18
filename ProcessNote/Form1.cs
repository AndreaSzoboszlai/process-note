using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace ProcessNote
{
    public partial class Form1 : Form
    {
        public Process[] Processes { get; set; }
        List<String> myAL = new List<String>();
        public Dictionary<Process, List<String>> ProcessComments { get; set; }
        int counter = 0;

        public Form1()
        {
            InitializeComponent();
            Processes = Process.GetProcesses();
            ProcessComments = new Dictionary<Process, List<String>>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Process process in Processes)
            {
                string[] row = new string[] { process.Id.ToString(), process.ProcessName };
                listView1.Items.Add(new ListViewItem(row));
            }
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 0)
            {
                listView2.Items.RemoveAt(0);
            }

            Process process = GetChoosenProcess();

            if (process != null)
            {
                PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
                PerformanceCounter ramCounter = new PerformanceCounter("Process", "Private Bytes", process.ProcessName, true);
                double cpu = Math.Round(cpuCounter.NextValue() / Environment.ProcessorCount, 2);
                double ram = Math.Round(ramCounter.NextValue() / 1024 / 1024, 2);

                DateTime startTime = process.StartTime;
                TimeSpan runningTime = DateTime.Now - startTime;

                string[] row = new string[] { cpu + "%", ram + " MB", runningTime.ToString(), startTime.ToString() };
                listView2.Items.Add(new ListViewItem(row));
            }

            foreach (ProcessThread thread in process.Threads)
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(thread.Id);
            }

            fillComments(process);


        }

        private Process GetChoosenProcess()
        {
            if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0] != null)
            {
                foreach (Process process in Processes)
                {
                    if (process.Id.ToString() == listView1.SelectedItems[0].Text)
                    {
                        return process;
                    }
                }
            }
            return null;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Process process = GetChoosenProcess();
            if (textBox1.Text != null && process != null)
            {
                if (!ProcessComments.ContainsKey(process))
                {
                    myAL.Add(textBox1.Text);
                    ProcessComments.Add(process, myAL);
                }
                else
                {
                    counter++;
                    myAL.Add(textBox1.Text);
                }
            }
            fillComments(process);

        }

        private void fillComments(Process process)
        {
            textBox2.Clear();
            List<string> values = new List<string>();
            if (ProcessComments.ContainsKey(process))
            {
                values = ProcessComments[process];
            }

            foreach (string comment in values)
            {
                textBox2.Text += System.Environment.NewLine + comment;
            }
        }
    }
}
