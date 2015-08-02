using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessorScheduling
{
    public partial class Form1 : MetroForm
    {
        private ReadyQueue readyQueue;
        private Process p1, p2, p3, p4, p5;
        private Process[] processList;
        private Process currentlyRunning;
        private int clockInterval;
        private int time;
        private System.Windows.Forms.Timer timer;
        private Boolean isTimerPause;
        private Boolean isRunForOneTriger;
        private int timeSlice;
        private Panel[] process1Panel;
        private Panel[] process2Panel;
        private Panel[] process3Panel;
        private Panel[] process4Panel;
        private Panel[] process5Panel;
        public Form1()
        {
            InitializeComponent();

            readyQueueText.Text = "";

            algoComboBox.Items.Add("Select a scheduling method");
            algoComboBox.Items.Add("First Come First Serve (FCFS)");
            algoComboBox.Items.Add("Shortest Process Next (SPN)");
            algoComboBox.Items.Add("Shortest Remaining Time (SRT)");
            algoComboBox.Items.Add("Highest Response Ratio Next (HRRN)");
            algoComboBox.Items.Add("Round Robin");
            algoComboBox.SelectedIndex = 0;

            process1Panel = new Panel[20] { A1, A2, A3, A4, A5, A6, panel14, panel15, panel16, panel17, panel18, panel19, panel20, panel21, panel22, panel23, panel24, panel25, panel26,panel28 };
            process2Panel = new Panel[20] { panel29, panel30, panel31, panel32, panel33, panel34, panel35, panel36, panel37, panel38, panel39, panel40, panel41, panel42, panel43, panel44, panel45, panel27, panel46, panel47 };
            process3Panel = new Panel[20] { panel48, panel49, panel50, panel51, panel52, panel53, panel54, panel55, panel56, panel57, panel58, panel59, panel60, panel61, panel62, panel63, panel64, panel65, panel66, panel67 };
            process4Panel = new Panel[20] { panel68, panel69, panel70, panel71, panel72, panel73, panel74, panel75, panel76, panel77, panel78, panel79, panel80, panel81, panel82, panel83, panel84, panel85, panel86, panel87 };
            process5Panel = new Panel[20] { panel88, panel89, panel90, panel91, panel92, panel93, panel94, panel95, panel96, panel97, panel98, panel99, panel100, panel101, panel102, panel8, panel11, panel12, panel10, panel13};

            readyQueue = new ReadyQueue();

            //declare processes
            p1 = new Process(3, 0, 0, 1);
            p2 = new Process(6, 2, 0, 2);
            p3 = new Process(4, 4, 0, 3);
            p4 = new Process(5, 6, 0, 4);
            p5 = new Process(2, 8, 0, 5);
            processList = new Process[5] { p1, p2, p3, p4, p5 };

            currentlyRunning = null;

            for (int i = 0; i < processList.Length; i++)
            {
                String[] rowDetail = { "Process " + processList[i].Pid.ToString(), processList[i].ArrivedTime.ToString(), processList[i].ServiceTime.ToString() };
                processTable.Rows.Add(rowDetail);
            }

            timer = new System.Windows.Forms.Timer();
            time = 0;
            isTimerPause = false;
            isRunForOneTriger = false;


            job1Progressbar.Value = 0;
            job2Progressbar.Value = 0;
            job3Progressbar.Value = 0;
            job4Progressbar.Value = 0;
            job5Progressbar.Value = 0;


            timeSpinner.Value = 0;
            label30.Text += "\n 0%";
            

            for (int i = 0; i < 20; i++) {
                process1Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process2Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process3Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process4Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process5Panel[i].BackColor = Color.FromArgb(184, 27, 27);
            }

        }

        public void m()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    job1Progressbar.Value = i;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
        }


        private void metroButton1_Click(object sender, EventArgs e)
        {
            string selectedAlgo = algoComboBox.GetItemText(algoComboBox.SelectedItem);
            clockInterval = Int32.Parse(speedText.Text);

            start_timer(selectedAlgo);
        }

        public void start_timer(string selectedAlgo)
        {
            //check if pause button has clicked before
            if (isTimerPause)
            {
                timer.Enabled = true;
            }
            else
            {
                reset();
                //when timer trigging relavant method is called
                if (selectedAlgo == "First Come First Serve (FCFS)")
                {
                    timer.Tick += new EventHandler(timer_Tick_FCFS);
                }
                else if (selectedAlgo == "Round Robin")
                {
                    timeSlice = Int32.Parse(timeSliceText.Text);
                    timer.Tick += new EventHandler(timer_Tick_RoundRobin);
                }
                else if (selectedAlgo == "Shortest Process Next (SPN)")
                {
                    timer.Tick += new EventHandler(timer_Tick_SPN);
                }
                else if (selectedAlgo == "Shortest Remaining Time (SRT)")
                {
                    timeSlice = Int32.Parse(timeSliceText.Text);
                    timer.Tick += new EventHandler(timer_Tick_SRT);
                }
                else if (selectedAlgo == "Highest Response Ratio Next (HRRN)")
                {
                    timer.Tick += new EventHandler(timer_Tick_HRRN);
                }
                //set timer interval, enable timer and start timer
                timer.Interval = clockInterval;
                timer.Enabled = true;
                timer.Start();
            }

        }

        void timer_Tick_FCFS(object sender, EventArgs e)
        {
            readyQueueText.Text = " ";
            timeText.Text = time.ToString();

            //add process to ready queue if arrival time is equal to current time
            for (int k = 0; k < 5; k++)
            {
                if (processList[k].ArrivedTime == time)
                {
                    readyQueue.Ready_Queue.Enqueue(processList[k]);
                }

            }

            //if time=0 start with the first process in the queue
            if (time == 0)
            {
                currentlyRunning = readyQueue.firstComeFirstServe();
            }

            //if the service time of a process equal to run time, then get the next in the queue
            if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
            {
                currentlyRunning = readyQueue.firstComeFirstServe();
            }

            showReadyQueue();

            currentlyRunning.RunTime += 1;

            showProcessTimeline();

            //display the process id of the currently running pricess of the cpu
            processInCPU.Text = currentlyRunning.Pid.ToString();

            time++;

            //display time spinner value
            timeSpinner.Value += 5;
            label30.Text = "Timer\n" + timeSpinner.Value + "%";

            //stop timer after all processes are executed
            if (time == 20)
            {
                timer.Stop();
                btnNext.Enabled = false;
                btnPause.Enabled = false;
                //btnStop.Enabled = false;
            }

            //check if this method executed when step next button clicked
            //then timer should be disabled as this should run step by step
            if (isRunForOneTriger)
            {
                isRunForOneTriger = false;
                timer.Enabled = false;
            }
        }

        public void timer_Tick_RoundRobin(object sender, EventArgs e)
        {
            readyQueueText.Text = " ";
            timeText.Text = time.ToString();

            //add process to ready queue if arrival time is equal to current time
            for (int k = 0; k < 5; k++)
            {
                if (processList[k].ArrivedTime == time)
                {
                    readyQueue.Ready_Queue.Enqueue(processList[k]);
                }
            }

            //if time=0 start with the first process in the queue
            if (time == 0)
            {
                currentlyRunning = readyQueue.roundRobin(null, true);
            }

            //if the service time of a process equal to run time, then get the next in the queue
            if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
            {
                currentlyRunning = readyQueue.roundRobin(null, true);
                timeSlice = Int32.Parse(timeSliceText.Text);
            }
            else if (timeSlice == 0)
            {
                currentlyRunning = readyQueue.roundRobin(currentlyRunning, false);
                timeSlice = Int32.Parse(timeSliceText.Text);
            }

            showReadyQueue();

            currentlyRunning.RunTime += 1;

            showProcessTimeline();

            //display the process id of the currently running pricess of the cpu
            processInCPU.Text = currentlyRunning.Pid.ToString();

            timeSlice--;

            time++;

            //display time spinner value
            timeSpinner.Value += 5;
            label30.Text = "Timer\n" + timeSpinner.Value + "%";

            //stop timer after all processes are executed
            if (time == 20)
            {
                timer.Stop();
                btnNext.Enabled = false;
                btnPause.Enabled = false;
                //btnStop.Enabled = false;
            }

            //check if this method executed when step next button clicked
            //then timer should be disabled as this should run step by step
            if (isRunForOneTriger)
            {
                isRunForOneTriger = false;
                timer.Enabled = false;
            }
        }


        public void timer_Tick_SPN(object sender, EventArgs e)
        {
            readyQueueText.Text = " ";
            timeText.Text = time.ToString();

            //add process to ready queue if arrival time is equal to current time
            for (int k = 0; k < 5; k++)
            {
                if (processList[k].ArrivedTime == time)
                {
                    readyQueue.Ready_Queue.Enqueue(processList[k]);
                }

            }

            //if time=0 start with the first process in the queue
            if (time == 0)
            {
                currentlyRunning = readyQueue.SPN();
            }

            //if the service time of a process equal to run time, then get the next in the queue
            if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
            {
                currentlyRunning = readyQueue.SPN();
            }

            showReadyQueue();

            currentlyRunning.RunTime += 1;

            showProcessTimeline();

            //display the process id of the currently running pricess of the cpu
            processInCPU.Text = currentlyRunning.Pid.ToString();

            time++;

            //display time spinner value
            timeSpinner.Value += 5;
            label30.Text = "Timer\n" + timeSpinner.Value + "%";

            //stop timer after all processes are executed
            if (time == 20)
            {
                timer.Stop();
                btnNext.Enabled = false;
                btnPause.Enabled = false;
                //btnStop.Enabled = false;
            }

            //check if this method executed when step next button clicked
            //then timer should be disabled as this should run step by step
            if (isRunForOneTriger)
            {
                isRunForOneTriger = false;
                timer.Enabled = false;
            }

        }

        public void timer_Tick_SRT(object sender, EventArgs e)
        {
            readyQueueText.Text = " ";
            timeText.Text = time.ToString();

            //add process to ready queue if arrival time is equal to current time
            for (int k = 0; k < 5; k++)
            {
                if (processList[k].ArrivedTime == time)
                {
                    readyQueue.Ready_Queue.Enqueue(processList[k]);
                }

            }

            //if time=0 start with the first process in the queue
            if (time == 0)
            {
                currentlyRunning = readyQueue.SRN(null);
            }
            //if the service time of a process equal to run time, then get the next in the queue
            else if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
            {
                currentlyRunning = readyQueue.SRN(null);
                timeSlice = Int32.Parse(timeSliceText.Text);
            }
            else if (timeSlice == 0)
            {
                if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
                {
                    currentlyRunning = readyQueue.SRN(null);
                }
                else
                {
                    currentlyRunning = readyQueue.SRN(currentlyRunning);
                }
                
                timeSlice = Int32.Parse(timeSliceText.Text);
            }

            showReadyQueue();

            currentlyRunning.RunTime += 1;

            showProcessTimeline();

            //display the process id of the currently running pricess of the cpu
            processInCPU.Text = currentlyRunning.Pid.ToString();

            timeSlice--;

            time++;

            //display time spinner value
            timeSpinner.Value += 5;
            label30.Text = "Timer\n" + timeSpinner.Value + "%";

            //stop timer after all processes are executed
            if (time == 20)
            {
                timer.Stop();
                btnNext.Enabled = false;
                btnPause.Enabled = false;
                //btnStop.Enabled = false;
            }

            //check if this method executed when step next button clicked
            //then timer should be disabled as this should run step by step
            if (isRunForOneTriger)
            {
                isRunForOneTriger = false;
                timer.Enabled = false;
            }
        }

        public void timer_Tick_HRRN(object sender, EventArgs e)
        {
            readyQueueText.Text = " ";
            timeText.Text = time.ToString();

            //add process to ready queue if arrival time is equal to current time
            for (int k = 0; k < 5; k++)
            {
                if (processList[k].ArrivedTime == time)
                {
                    readyQueue.Ready_Queue.Enqueue(processList[k]);
                }

            }

            //if time=0 start with the first process in the queue
            if (time == 0)
            {
                currentlyRunning = readyQueue.HRRN(time);
            }
            //if the service time of a process equal to run time, then get the next in the queue
            else if (currentlyRunning.ServiceTime == currentlyRunning.RunTime)
            {
                currentlyRunning = readyQueue.HRRN(time);
            }

            showReadyQueue();

            currentlyRunning.RunTime += 1;

            showProcessTimeline();

            //display the process id of the currently running pricess of the cpu
            processInCPU.Text = currentlyRunning.Pid.ToString();

            time++;

            //display time spinner value
            timeSpinner.Value += 5;
            label30.Text = "Timer\n" + timeSpinner.Value + "%";

            //stop timer after all processes are executed
            if (time == 20)
            {
                timer.Stop();
                btnNext.Enabled = false;
                btnPause.Enabled = false;
                //btnStop.Enabled = false;
            }

            //check if this method executed when step next button clicked
            //then timer should be disabled as this should run step by step
            if (isRunForOneTriger)
            {
                isRunForOneTriger = false;
                timer.Enabled = false;
            }

        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            isTimerPause = true;
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if (isTimerPause)
            {
                timer.Enabled = true;
                isRunForOneTriger = true;
            }
            //timer.Enabled = false;
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            reset();
        }

        //reset programme to initial conditions
        public void reset()
        {
            timeSpinner.Value = 0;

            for (int i = 0; i < 20; i++) {
                process1Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process2Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process3Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process4Panel[i].BackColor = Color.FromArgb(184, 27, 27);
                process5Panel[i].BackColor = Color.FromArgb(184, 27, 27);
            }
            timer.Stop();
            readyQueue = new ReadyQueue();

            //declare processes
            p1 = new Process(3, 0, 0, 1);
            p2 = new Process(6, 2, 0, 2);
            p3 = new Process(4, 4, 0, 3);
            p4 = new Process(5, 6, 0, 4);
            p5 = new Process(2, 8, 0, 5);
            processList = new Process[5] { p1, p2, p3, p4, p5 };

            currentlyRunning = null;

            timer = new System.Windows.Forms.Timer();
            time = 0;
            isTimerPause = false;
            isRunForOneTriger = false;

            job1Progressbar.Value = 0;
            job2Progressbar.Value = 0;
            job3Progressbar.Value = 0;
            job4Progressbar.Value = 0;
            job5Progressbar.Value = 0;

            label30.Text = "Timer";
          
        }

        //play button click event
        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            if (speedText.Text == "" || System.Text.RegularExpressions.Regex.IsMatch(speedText.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter a valid value for Simulation Timer Interval.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string selectedAlgo = algoComboBox.GetItemText(algoComboBox.SelectedItem);
                clockInterval = Int32.Parse(speedText.Text);

                if ((algoComboBox.SelectedIndex == 3 || algoComboBox.SelectedIndex == 5) && (timeSliceText.Text == "" || System.Text.RegularExpressions.Regex.IsMatch(timeSliceText.Text, "[^0-9]")))
                {
                    MessageBox.Show("Please enter valid value for Quantum Time.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    start_timer(selectedAlgo);

                    btnPause.Enabled = true;
                    btnNext.Enabled = true;
                    btnStop.Enabled = true;
                }
                
            }
            algoComboBox.Enabled = false;
        }

        //pause button button click event
        private void metroButton2_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = false;
            isTimerPause = true;
        }

        //step next button click event
        private void metroButton3_Click_1(object sender, EventArgs e)
        {
            if (isTimerPause)
            {
                timer.Enabled = true;
                isRunForOneTriger = true;
            }
        }

        //stop button click event
        private void metroButton4_Click_1(object sender, EventArgs e)
        {
            reset();

            btnNext.Enabled = false;
            btnPause.Enabled = false;

            algoComboBox.Enabled = true;

            speedText.Text = "";
            timeSliceText.Text = "";
            processInCPU.Text = "";
            timeText.Text = "";
            job1Waiting.Text = "";
            job2Waiting.Text = "";
            job3Waiting.Text = "";
            job4Waiting.Text = "";
            job5Waiting.Text = "";
            job1Remaining.Text = "";
            job2Remaining.Text = "";
            job3Remaining.Text = "";
            job4Remaining.Text = "";
            job5Remaining.Text = "";
        }

        private void algoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (algoComboBox.SelectedIndex == 1)
            {
                lblTopic.Text = "First Come First Serve (FCFS)";
                lblDescription.Text = "Processes are dispatched according to their arrival time on the ready queue.";
                lblType.Text = "Non-Preemptive";
                btnPlay.Enabled = true;
                btnBanner.Visible = false;
            }
            else if (algoComboBox.SelectedIndex == 2)
            {
                lblTopic.Text = "Shortest Process Next (SPN)";
                lblDescription.Text = "The process with the shortest expected processing time is selected next.";
                lblType.Text = "Non-Preemptive";
                btnPlay.Enabled = true;
                btnBanner.Visible = false;
            }
            else if (algoComboBox.SelectedIndex == 3)
            {
                lblTopic.Text = "Shortest Remaining Time (SRT)";
                lblDescription.Text = "Chooses the process that has the shortest expected remaining processing time.";
                lblType.Text = "Preemptive";
                btnPlay.Enabled = true;
                btnBanner.Visible = false;
            }
            else if (algoComboBox.SelectedIndex == 4)
            {
                lblTopic.Text = "Highest Response Ratio Next";
                lblDescription.Text = "Chooses next process with the greatest response ratio. It accounts for the age of the process.";
                lblType.Text = "Non-Preemptive";
                btnPlay.Enabled = true;
                btnBanner.Visible = false;
            }
            else if (algoComboBox.SelectedIndex == 5)
            {
                lblTopic.Text = "Round Robin (RR)";
                lblDescription.Text = "Each process is given a slice of time before being pre-empted.";
                lblType.Text = "Preemptive";
                btnPlay.Enabled = true;
                btnBanner.Visible = false;
            }
            else if (algoComboBox.SelectedIndex == 0)
            {
                btnPlay.Enabled = false;
                btnBanner.Visible = true;
            }
        }

        //show the process in the ready queue at current time
        private void showReadyQueue()
        {
            for (int i = 0; i < readyQueue.Ready_Queue.Count; i++)
            {
                if (readyQueue.Ready_Queue.Count == 1)
                {
                    readyQueueText.Text += "Process " + readyQueue.Ready_Queue.ElementAt(i).Pid;
                }
                else
                {
                    if (i == readyQueue.Ready_Queue.Count - 1)
                    {
                        readyQueueText.Text += "Process " + readyQueue.Ready_Queue.ElementAt(i).Pid;
                    }
                    else
                    {
                        readyQueueText.Text += "Process " + readyQueue.Ready_Queue.ElementAt(i).Pid + " | ";
                    }

                }

            }
        }

        //display value for the Remaining Time, Witing Time, Progress Bar.
        //color relavant boxes in display panel (timeline)
        private void showProcessTimeline()
        {
            if (currentlyRunning.Pid == 1)
            {
                job1Remaining.Text = (currentlyRunning.ServiceTime - currentlyRunning.RunTime).ToString();
                job1Waiting.Text = (time + 1 - currentlyRunning.ArrivedTime).ToString();
                job1Progressbar.Value += 33;
                process1Panel[time].BackColor = Color.FromArgb(51, 153, 255);
            }
            else if (currentlyRunning.Pid == 2)
            {
                job2Remaining.Text = (currentlyRunning.ServiceTime - currentlyRunning.RunTime).ToString();
                job2Waiting.Text = (time + 1 - currentlyRunning.ArrivedTime).ToString();
                job2Progressbar.Value += 16;
                process2Panel[time].BackColor = Color.FromArgb(51, 153, 255);
            }
            else if (currentlyRunning.Pid == 3)
            {
                job3Remaining.Text = (currentlyRunning.ServiceTime - currentlyRunning.RunTime).ToString();
                job3Waiting.Text = (time + 1 - currentlyRunning.ArrivedTime).ToString();
                job3Progressbar.Value += 25;
                process3Panel[time].BackColor = Color.FromArgb(51, 153, 255);
            }
            else if (currentlyRunning.Pid == 4)
            {
                job4Remaining.Text = (currentlyRunning.ServiceTime - currentlyRunning.RunTime).ToString();
                job4Waiting.Text = (time + 1 - currentlyRunning.ArrivedTime).ToString();
                job4Progressbar.Value += 20;
                process4Panel[time].BackColor = Color.FromArgb(51, 153, 255);
            }
            else if (currentlyRunning.Pid == 5)
            {
                job5Remaining.Text = (currentlyRunning.ServiceTime - currentlyRunning.RunTime).ToString();
                job5Waiting.Text = (time + 1 - currentlyRunning.ArrivedTime).ToString();
                job5Progressbar.Value += 50;
                process5Panel[time].BackColor = Color.FromArgb(51, 153, 255);
            }
        }
    }
}
