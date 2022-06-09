using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab4
{
    class Program
    {
        static Queue<String> queue = new Queue<string>();
        static bool isEnd = false;
        static Mutex mutexObj = new Mutex();
        static int sizeQueue = 3;
        static void Main(string[] args)
        {
            Console.Write("Сколько заявок: ");
            int countRequest = int.Parse(Console.ReadLine());
            Console.Write("Размер очереди: ");
            sizeQueue = int.Parse(Console.ReadLine());
            Thread manufacturer = new Thread(pushRequest);
            Thread consumer = new Thread(readRequest);
            manufacturer.Start(countRequest);
            consumer.Start();
        }

        private static void readRequest()
        {
            while (!isEnd || queue.Count != 0)
            {
                if (queue.Count != 0)
                {
                    mutexObj.WaitOne();
                    Console.WriteLine("Принята заявка: " + queue.Dequeue());
                    mutexObj.ReleaseMutex();
                    Thread.Sleep(1500);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static void pushRequest(object countRequest)
        {
            for (int i = 0; i < (int)countRequest; i++)
            {
                Thread.Sleep(500);
                if (queue.Count < sizeQueue)
                {
                    mutexObj.WaitOne();
                    queue.Enqueue(i.ToString());
                    Console.WriteLine("Подана заявка: " + i.ToString());
                    mutexObj.ReleaseMutex();
                }
                else
                {
                    Console.WriteLine("Заявка отклонена: " + i.ToString());
                }
            }
            isEnd = true;
        }
    }
}
