using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Lab4_3
{
    class Program
    {
        static List<String> storage = new List<String>();
        static Mutex mutexObj = new Mutex();
        static Semaphore sem = new Semaphore(3, 3);
        static void Main(string[] args)
        {
            Console.Write("Количество писателей: ");
            int writerCount = int.Parse(Console.ReadLine());
            List<Thread> writers = new List<Thread>();

            Console.Write("Количество читателей: ");
            int readerCount = int.Parse(Console.ReadLine());
            List<Thread> readers = new List<Thread>();

            for (int i = 0; i < 20; i++)
            {
                storage.Add($@"Запись {i}");
            }

            for (int i = 0; i < writerCount; i++)
            {
                writers.Add(new Thread(changeStorage));
            }
            for (int i = 0; i < readerCount; i++)
            {
                readers.Add(new Thread(readStorage));
            }

            for (int i = 0; i < writerCount; i++)
            {
                writers[i].Start(i);
            }
            for (int i = 0; i < readerCount; i++)
            {
                readers[i].Start(i);
            }

        }

        private static void changeStorage(object idWriter)
        {
            Random random = new Random();
            mutexObj.WaitOne();
            int idNote = random.Next(storage.Count - 1);
            Console.WriteLine($@"Писатель {(int)(idWriter)} изменяет запись {idNote}");
            storage[idNote] = $@"Писатель {(int)(idWriter)} изменил эту запись";
            Thread.Sleep(500);
            mutexObj.ReleaseMutex();
            Thread.Sleep(500);
        }

        private static void readStorage(object idReader)
        {
            Random random = new Random();
            sem.WaitOne();
            Console.WriteLine("Читатель " + (int)(idReader) + " зашёл");
            //mutexObj.WaitOne();
            String note = storage[random.Next(storage.Count-1)];
            Console.WriteLine($@"Читатель {(int)(idReader)} прочитал следующее: {note}");
            //mutexObj.ReleaseMutex();
            Thread.Sleep(600);
            Console.WriteLine("Читатель вышел");
            sem.Release();
        }
    }
}
