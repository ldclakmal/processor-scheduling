using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorScheduling
{
    public class ReadyQueue
    {
        private Queue<Process> readyQueue;

        public ReadyQueue() {
            this.readyQueue = new Queue<Process>();
        }

        public Queue<Process> Ready_Queue
        {
            get { return readyQueue; }
            set { readyQueue = value; }
        }

        //return next process according to FCFS algorithm
        public Process firstComeFirstServe() {
            return readyQueue.Dequeue();
        }

        //return next process according to SPN algorithm
        public Process SPN() {

            int count = readyQueue.Count;

            //copy Queue into Vector
            List<Process> listProcesses = new List<Process>();

            for (int i = 0; i < count; i++)
            {
                Process p = (Process)readyQueue.Dequeue();
                listProcesses.Add(p);
            }

            //get the index of the next process
            int minIndex = 0;
            Process spn;
            for (int k = 1; k < count; k++)
            {
                if (listProcesses.ElementAt(k).ServiceTime < listProcesses.ElementAt(minIndex).ServiceTime)
                    minIndex = k;
            }

            spn = listProcesses.ElementAt(minIndex);

            readyQueue.Enqueue(spn);

            //enque list again into queue
            for (int i = 0; i < count; i++)
            {
                if (i != minIndex)
                {
                    readyQueue.Enqueue(listProcesses.ElementAt(i));
                }

            }

            return readyQueue.Dequeue();
        }

        //return next process according to HRRN algorithm
        public Process HRRN(int currentTime) {


            int count = readyQueue.Count;

            //copy Queue into Vector
            List<Process> listProcesses = new List<Process>();

            for (int l = 0; l < count; l++)
            {
                Process process = (Process)readyQueue.Dequeue();
                listProcesses.Add(process);
            }

            //get the index of the next process
            int minIndex = 0;
            Process srn;
            int HRRN = ((currentTime - listProcesses.ElementAt(0).ArrivedTime) / listProcesses.ElementAt(0).ServiceTime) + 1;
            for (int k = 1; k < count; k++)
            {
                Process p = listProcesses.ElementAt(k);
                double HRRN_P = ((currentTime - p.ArrivedTime) / p.ServiceTime) + 1;
                if (HRRN < HRRN_P)
                    minIndex = k;

            }

            srn = listProcesses.ElementAt(minIndex);
            readyQueue.Enqueue(srn);

            //enque list again into queue
            for (int i = 0; i < count; i++)
            {
                if (i != minIndex)
                {
                    readyQueue.Enqueue(listProcesses.ElementAt(i));
                }

            }

            return readyQueue.Dequeue();
        }

        //return next process according to RR algorithm
        public Process roundRobin(Process p, bool isFinished) {
            if (!isFinished) {
                readyQueue.Enqueue(p);
            }
            return readyQueue.Dequeue();
        }

        //return next process according to SRN algorithm
        public Process SRN(Process p) {

            if (p != null) {
                readyQueue.Enqueue(p);
            }    

            int count = readyQueue.Count;

            //copy Queue into Vector
            List<Process> listProcesses = new List<Process>();

            for (int l = 0; l < count; l++)
            {
                Process process = (Process)readyQueue.Dequeue();
                listProcesses.Add(process);
            }

            //get the index of the next process
            int minIndex=0;
            Process srn;
            int remainingTime = listProcesses.ElementAt(0).ServiceTime - listProcesses.ElementAt(0).RunTime;
            for (int k = 1; k < count; k++)
            {
                    int remainingTimeK = listProcesses.ElementAt(k).ServiceTime - listProcesses.ElementAt(k).RunTime;
                    if (remainingTime > remainingTimeK)
                        minIndex = k;
                
            }

            srn=listProcesses.ElementAt(minIndex);
            readyQueue.Enqueue(srn);

            //enque list again into queue
            for(int i=0;i<count;i++)
            {
                if(i!=minIndex){
                    readyQueue.Enqueue(listProcesses.ElementAt(i));
                }
                
            }

            return readyQueue.Dequeue();
        }

    }
}
