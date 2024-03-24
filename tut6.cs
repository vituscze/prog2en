using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Tutorial
{
  class Program
  {
    // Dictionary Flip //
    //
    // Write a function
    //
    //   Dictionary<string, string> flip(Dictionary<string, string> d)
    //
    // that takes a dictionary from strings to strings and returns a flipped
    // version of the dictionary. For example, if d maps "dog" to "pes", then
    // the returned dictionary will map "pes" to "dog". Assume that all values
    // in the dictionary d are unique.
    static Dictionary<string, string> Flip(Dictionary<string, string> d)
    {
      Dictionary<string, string> r = new();
      // foreach (KeyValuePair<string, string> p in d)
      foreach (var (k, v) in d)
      {
        r[v] = k;
      }
      return r;
    }

    // Xyz //
    //
    // What will this program print?
    class Foo
    {
      public virtual int xyz() => 5;
    }

    class Bar : Foo
    {
      public override int xyz() => 10;
    }

    /*
    class Top
    {
      static void Main()
      {
        Bar b = new Bar();
        Foo f = b;
        Console.WriteLine(b.xyz());
        Console.WriteLine(f.xyz());
      }
    }
    */

    // For virtual methods, the program cares about the runtime type of
    // the object and not the type of the variable in which it is stored.
    //
    // So both will end up calling Bar's xyz.

    // Frequent Words //
    //
    // Write a function
    //
    //   string[] topWords(string filename, int n)
    //
    // that reads a file and returns an array of the n most frequent words in
    // the file, listed in descending order of frequency. A word is any sequence
    // of non-whitespace characters; comparisons are case-sensitive. If the file
    // has fewer than n distinct words, return an array of all words in the file.
    // Use the C# collection classes.
    static string[] TopWords(string filename, int n)
    {
      Dictionary<string, int> freq = new();

      // We can read the content of a file in multiple ways.
      // We need System.IO for all of these.
      // Manually, line by line:
      /*
      using (StreamReader f = File.OpenText(filename))
      {
        while (f.ReadLine() is string line)
        {
          // Process line
        }
      }
      */

      // Read all lines at once:
      /*
      string[] lines = File.ReadAllLines(filename);
      foreach (string line in lines)
      {
        // Process line
      }
      */

      // Read the entire file at once:
      /*
      string content = File.ReadAllText(filename);
      foreach (string line in content.Split('\n'))
      {
        // Process line
      }
      */

      // Read all lines, but process them lazily:
      IEnumerable<string> lines = File.ReadLines(filename);
      foreach (string line in lines)
      {
        foreach (string word in line.Split(null as char[], StringSplitOptions.RemoveEmptyEntries))
        {
          // If the key wasn't found, TryGetValue sets val to
          // its default value, which in case of int is 0.
          freq.TryGetValue(word, out var val);
          freq[word] = val + 1;
        }
      }

      // Now we just need to sort it by the frequency and take top n results.
      return freq.OrderByDescending(p => p.Value).Select(p => p.Key).Take(n).ToArray();
    }

    // Hash Table Versus Tree //
    //
    // The C# library includes a class HashSet<T>, based on a hash table,
    // and a class SortedSet<T>, based on a balanced binary tree.
    //
    // a) Suppose that we insert N random integers into each of these data structures.
    // Which one do you think will be faster? Perform an experiment that compares the
    // performance for various values of N. Draw a graph (e.g. using Python and matplotlib)
    // showing the results.
    //
    // b) Now suppose that in each trial we insert N random integers, then remove all
    // N integers. Compare the performance of the two data structures in this case.
    //
    // c) Probably the class HashSet<T> rebuilds the hash table whenever its load factor
    // exceeds a certain threshold. Design and perform an experiment to determine that threshold.
    static void ExperimentA()
    {
      Random rng = new();

      for (int i = 1; i <= 40; i++)
      {
        int count = i * 10000;

        // Manually run the garbage collector so that any GC that runs
        // in the following code block doesn't skew the results.
        GC.Collect();
        Stopwatch sw = Stopwatch.StartNew(); // from System.Diagnostics
        {
          HashSet<int> set1 = new();
          for (int j = 0; j < count; j++)
          {
            set1.Add(rng.Next());
          }
        }
        double set1time = sw.Elapsed.TotalMilliseconds;

        GC.Collect();
        sw.Restart();
        {
          SortedSet<int> set2 = new();
          for (int j = 0; j < count; j++)
          {
            set2.Add(rng.Next());
          }
        }
        double set2time = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"{count}\t{set1time:0.00}\t{set2time:0.00}");
      }
    }

    static void ExperimentB()
    {
      Random rng = new();

      for (int i = 1; i <= 40; i++)
      {
        int count = i * 10000;
        List<int> random = new();
        for (int j = 0; j < count; j++)
        {
          random.Add(rng.Next());
        }

        GC.Collect();
        Stopwatch sw = Stopwatch.StartNew();
        {
          HashSet<int> set1 = new();
          foreach (var val in random)
          {
            set1.Add(val);
          }
          foreach (var val in random)
          {
            set1.Remove(val);
          }
        }
        double set1time = sw.Elapsed.TotalMilliseconds;

        GC.Collect();
        sw.Restart();
        {
          SortedSet<int> set2 = new();
          foreach (var val in random)
          {
            set2.Add(val);
          }
          foreach (var val in random)
          {
            set2.Remove(val);
          }
        }
        double set2time = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"{count}\t{set1time:0.00}\t{set2time:0.00}");
      }
    }

    static void ExperimentC()
    {
      HashSet<int> set = new(1000000);

      const int steps = 300;
      const int insert = 20000;

      for (int i = 0; i < steps; i++)
      {
        Stopwatch sw = Stopwatch.StartNew();
        for (int j = 0; j < insert; j++)
        {
          set.Add(i * insert + j);
        }
        Console.WriteLine($"{set.Count}\t{sw.Elapsed.TotalMilliseconds:0.00}");
      }
    }

    static void Main(string[] args)
    {

    }
  }
}
