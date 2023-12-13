using System.Diagnostics;

public class Program {
  public static void Main() {
    const int MAX = 10;
    const int SEM_COUNT = 3;

    var sem = new Semaphore(SEM_COUNT, SEM_COUNT);
    var start = Stopwatch.GetTimestamp();
    int count = MAX;
    
    using var signal = new ManualResetEvent(false);
    for (int num = 0; num < MAX; num++) {
      int i = num;
      new Thread(() => RunSemaphore(i)).Start();
    }
    void RunSemaphore(int i) {
      print("Waiting for the semaphore.");
      sem.WaitOne();

      print("Got the semaphore. Waiting for 2 seconds...");
      Thread.Sleep(2000);
      print("Release the semaphore.");
      sem.Release();

      //signal if all tasks are done
      if (Interlocked.Decrement(ref count) == 0) signal.Set();

      signal.WaitOne();
      print("Done.");
      
      void print(String s) => Console.WriteLine($"Task {i:d3} [{Stopwatch.GetElapsedTime(start) :ss\\.ff}] : {s}" );
    }
    signal.WaitOne();
    Console.WriteLine("All tasks are done.");
  }
}

