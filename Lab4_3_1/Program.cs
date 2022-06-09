using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Lab4_3_1
{
    class Program
    {
        static List<String> storage = new List<String>();
        static Mutex mutexObj = new Mutex();
        static Semaphore ReadSem = new Semaphore(1, 1);
        static Semaphore Access = new Semaphore(1, 1);
        static int ReadCount = 0;
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
            Thread.Sleep(random.Next(1000, 5000));
            Access.WaitOne();
            int idNote = random.Next(storage.Count - 1);
            Console.WriteLine($@"Писатель {(int)(idWriter)} изменяет запись {idNote}");
            storage[idNote] = $@"Писатель {(int)(idWriter)} изменил запись {idNote}";
            Access.Release();
        }

        private static void readStorage(object idReader)
        {
            Random random = new Random();

            Thread.Sleep(random.Next(1000, 5000));
            ReadSem.WaitOne();
            ReadCount++;
            if (ReadCount == 1)
            {
                Access.WaitOne();
            }
            ReadSem.Release();

            Console.WriteLine("Читатель " + (int)(idReader) + " зашёл");
            String note = storage[random.Next(storage.Count - 1)];
            Console.WriteLine($@"Читатель {(int)(idReader)} прочитал следующее: {note}");
           
            ReadSem.WaitOne();
            ReadCount--;
            if(ReadCount == 0)
            {
                Access.Release();
            }
            Console.WriteLine($@"Читатель {(int)(idReader)} вышел");
            ReadSem.Release();
        }
    }
}
