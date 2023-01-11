public class Program {
  public static void Main() {
    while (true) {
      int Counter = 0;
      const int MAX = 500;
      var threads = new Thread[MAX];
      for (int num = 0; num < MAX; num++) {
        threads[num] = new Thread(new ThreadStart(delegate {
          Thread.Sleep(1);
          Counter++;
        }));
        threads[num].Start();
      }
      for (int num = 0; num < MAX; num++) {
        threads[num].Join();
      }
      Console.WriteLine(Counter);
    }
  }
}