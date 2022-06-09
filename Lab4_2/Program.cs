using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Lab4_2
{
    class Program
    {
        static Queue<List<int>> queue = new Queue<List<int>>();
        static Queue<int> queueSum = new Queue<int>();
        static bool isEndRead = false;
        static bool isEndWrite = false;
        static Mutex mutexObj = new Mutex();
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(readFromFile);
            Thread thread2 = new Thread(sumMassive);
            thread1.Start();
            thread2.Start();
        }

        private static void readFromFile()
        {
            string path = "../../../TextFile1.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                int size = int.Parse(reader.ReadLine());
                for (int i = 0; i < size; i++)
                {
                    string[] array = reader.ReadLine().ToString().Split(" ");
                    List<int> massive = new List<int>();
                    for (int k = 0; k < array.Length; k++)
                    {
                        massive.Add(int.Parse(array[k]));
                    }
                    mutexObj.WaitOne();
                    queue.Enqueue(massive);
                    mutexObj.ReleaseMutex();
                }
                isEndRead = true;
            }
            path = "../../../TextFile2.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                while (!isEndWrite || queueSum.Count != 0)
                {
                    if (queueSum.Count != 0)
                    {
                        int sum = queueSum.Dequeue();
                        writer.WriteLine(sum);
                        Console.WriteLine($@"Записана сумма: {sum}");
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private static void sumMassive()
        {
            while (!isEndRead || queue.Count != 0)
            {
                if (queue.Count != 0)
                {
                    mutexObj.WaitOne();
                    List<int> massive = queue.Dequeue();
                    mutexObj.ReleaseMutex();
                    string strMassive = "";
                    massive.ForEach(num => strMassive += num.ToString() + " ");
                    Console.WriteLine("Считали массив: " + strMassive);
                    int sum = 0;
                    massive.ForEach(num => sum += num);
                    queueSum.Enqueue(sum);

                    Thread.Sleep(1500);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            isEndWrite = true;
        }
    }
}
