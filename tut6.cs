using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
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

    // Binary Tree //
    class TreeSet<T> where T : IComparable<T>
    {
        private class Node
        {
        public T Value;
        public Node? Left;
        public Node? Right;

        public Node(T value) => Value = value;
        }

        private Node? root;

        public bool Contains(T x)
        {
        Node? n = root;
        while (n != null)
        {
            int c = x.CompareTo(n.Value);
            if (c == 0)
            {
            return true;
            }
            else if (c < 0)
            {
            n = n.Left;
            }
            else
            {
            n = n.Right;
            }
        }
        return false;
        }

        // a) Write an insert() method, giving it an appropriate type.
        public void Insert(T x)
        {
        if (root == null)
        {
            root = new(x);
            return;
        }

        Node n = root;
        while (true)
        {
            int c = x.CompareTo(n.Value);
            if (c == 0)
            {
            return;
            }
            else if (c < 0)
            {
            if (n.Left == null)
            {
                n.Left = new(x);
                return;
            }
            else
            {
                n = n.Left;
            }
            }
            else
            {
            if (n.Right == null)
            {
                n.Right = new(x);
                return;
            }
            else
            {
                n = n.Right;
            }
            }
        }
        }

        // b) Add a constructor TreeSet(T[] a) that builds a TreeSet from an array a.
        //    The resulting tree should be balanced. Do not modify the array.
        public TreeSet(T[] a)
        {
        List<T> values = new(a);
        values.Sort();

        Node? go(int from, int to)
        {
            if (from > to)
            {
            return null;
            }
            int mid = from + (to - from) / 2;
            Node? n = new(values[mid]);
            n.Left = go(from, mid - 1);
            n.Right = go(mid + 1, to);
            return n;
        }

        root = go(0, values.Count - 1);
        }

        // c) Add a method T[] range(T a, T b) that returns a sorted array of all
        //    values x such that a ≤ x ≤ b.
        public T[] Range(T a, T b)
        {
        List<T> result = new();

        void go(Node? n)
        {
            if (n == null)
            {
            return;
            }

            int l = a.CompareTo(n.Value);
            int r = n.Value.CompareTo(b);

            if (l < 0) // a < n.Value
            {
            go(n.Left);
            }
            if (l <= 0 && r <= 0) // a <= n.Value <= b
            {
            result.Add(n.Value);
            }
            if (r < 0) // n.Value < b
            {
            go(n.Right);
            }
        }

        go(root);
        return result.ToArray();
        }

        // d) Add a method void validate() that verifies that the structure satisfies
        //    the ordering requirements of a binary tree. If the structure is invalid,
        //    throw an exception with the message "invalid".
        public void Validate()
        {
        void go(Node? n, Node? low, Node? high)
        {
            if (n == null)
            {
            return;
            }

            bool ok = (low == null || low.Value.CompareTo(n.Value) < 0) && (high == null || n.Value.CompareTo(high.Value) < 0);
            if (!ok)
            {
            throw new Exception("invalid");
            }

            go(n.Left, low, n);
            go(n.Right, n, high);
        }

        go(root, null, null);
        }
    }

    // Filter //

    // Write a generic method
    //   T[] filter<T>(T[] a, Predicate<T> p)
    // that selects only the elements of a for which the given predicate is true.
    public static T[] Filter<T>(T[] a, Predicate<T> p)
    {
      List<T> result = new();
      foreach (T t in a)
      {
        if (p(t))
        {
          result.Add(t);
        }
      }
      return result.ToArray();
    }

    // Max By //

    // Write a generic method max_by() that takes an array plus a function f.
    // The method should return the array element x for which f(x) is the greatest,
    // or null if the array is empty. For example:
    //
    //   string[] a = { "red", "yellow", "blue", "green" };
    //   WriteLine(max_by(a, s => s.Length));   // writes "yellow"
    //
    // The method should work for any array type and for any function that maps the
    // array type to a comparable type.
    public static T? MaxBy<T, Key>(T[] a, Func<T, Key> f) where Key : IComparable<Key>
    {
      if (a.Length == 0)
      {
        return default;
      }

      T maxElem = a[0];
      Key maxKey = f(maxElem);
      for (int i = 1; i < a.Length; i++)
      {
        T elem = a[i];
        Key key = f(elem);
        if (key.CompareTo(maxKey) > 0)
        {
          maxElem = elem;
          maxKey = key;
        }
      }

      return maxElem;
    }

    // Sort With Comparer //

    // Write a generic function
    //
    //   void sort<T>(T[] a, Comparison<T> f)
    //
    // that can sort an array using an arbitrary function to compare elements.
    // The delegate type Comparison<T> is defined in the standard library as follows:
    //
    //   delegate int Comparison<T>(T x, T y);
    //
    // Given two objects x and y of type T, a Comparison<T> returns
    //
    //   * a negative value if x is less than y
    //   * 0 if x equals y
    //   * a positive value if x is greater than y
    //
    // You may use any sorting algorithm that you like.
    public static void Sort<T>(T[] a, Comparison<T> f)
    {
      for (int i = 1; i < a.Length; i++)
      {
        T x = a[i];
        int j;
        for (j = i; j > 0; j--)
        {
          // x < a[j - 1]
          if (f(x, a[j - 1]) < 0)
          {
            a[j] = a[j - 1];
          }
          else
          {
            break;
          }
        }
        a[j] = x;
      }
    }

    static void Main(string[] args)
    {
      // Euler 1 //

      // Solve Project Euler Problem 1 in a single line of code using Linq methods or Linq syntax:
      //
      //   Find the sum of all the multiples of 3 or 5 below 1000.
      Console.WriteLine(Enumerable.Range(0, 1000).Where(x => x % 3 == 0 || x % 5 == 0).Sum());
    }
  }
}
