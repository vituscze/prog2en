using System;

namespace ConsoleApp1
{
  // Binary Tree //
  //
  // Write a class BinaryTree that holds a binary search tree of integers.Your class should have these members:
  //
  //   BinaryTree() - create a BinaryTree that is initially empty
  //
  //   void insert(int i) - insert a value into the tree if it is not already present
  //
  //   bool contains(int i) - return true if the tree contains the value i, otherwise false
  //
  //   int[] toArray() - return an array holding all the values in the binary tree, in ascending order
  //
  // For an extra challenge, you may write a method that removes a value from the tree.
  class BinaryTree
  {
    private class Node
    {
      public int Data;
      public Node? Left;
      public Node? Right;

      public Node(int data, Node? left = null, Node? right = null)
      {
        Data = data;
        Left = left;
        Right = right;
      }
    }

    // We'll keep track of the size explicitly. It will
    // help us when implementing the ToArray method.
    private int size;
    private Node? root;

    public BinaryTree()
    {
      size = 0;
      root = null;
    }

    public int Size() => size;

    public bool Contains(int i)
    {
      Node? n = root;
      while (n != null)
      {
        if (i == n.Data)
        {
          return true;
        }
        else if (i < n.Data)
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

    public void Insert(int i)
    {
      if (root == null)
      {
        root = new(i);
        size = 1;
        return;
      }

      // Notice we can use Node (instead of Node?) here.
      Node n = root;
      while (true)
      {
        if (i == n.Data)
        {
          // Value is already in the tree, we're done.
          return;
        }
        else if (i < n.Data)
        {
          if (n.Left == null)
          {
            n.Left = new(i);
            size++;
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
            n.Right = new(i);
            size++;
            return;
          }
          else
          {
            n = n.Right;
          }
        }
      }
    }

    public int[] ToArray()
    {
      int[] array = new int[size];
      int ix = 0;

      // Just like in Python, we can create local functions.
      // But unlike Python, we don't have to explicitly say
      // which variables are nonlocal, C# can figure it out on
      // its own.
      void Go(Node? n)
      {
        if (n == null)
        {
          return;
        }
        else
        {
          Go(n.Left);
          array[ix++] = n.Data;
          Go(n.Right);
        }
      }

      Go(root);
      return array;
    }

    // Array to Tree //
    //
    // Write a method that takes an array containing a sorted list of integers
    // and returns a binary search tree containing all the values in the array.
    // The tree should be as balanced as possible, i.e. it should have the minimum
    // possible height.
    static public BinaryTree FromArray(int[] array)
    {
      Node? Go(int from, int to)
      {
        if (from > to)
        {
          return null;
        }
        else
        {
          int mid = from + (to - from) / 2;
          return new(array[mid], Go(from, mid - 1), Go(mid + 1, to));
        }
      }

      BinaryTree tree = new();
      tree.root = Go(0, array.Length - 1);
      tree.size = array.Length;
      return tree;
    }

    // Tree Check
    //
    // Write a method that takes a binary tree and returns true if the tree satisfies
    // the ordering requirements of a binary search tree.
    public bool CheckOne()
    {
      bool Go(int from, int to, Node? n)
      {
        if (n == null)
        {
          return true;
        }
        else
        {
          return from <= n.Data && n.Data <= to && Go(from, n.Data - 1, n.Left) && Go(n.Data + 1, to, n.Right);
        }
      }

      return Go(int.MinValue, int.MaxValue, root);
      // Problem: Gives incorrect result for
      //   BinaryTree.FromArray(new int[] { 1, int.MinValue, int.MaxValue }).CheckOne()
      // because of the overflow.
    }

    public bool CheckTwo()
    {
      bool Go(int? from, int? to, Node? n)
      {
        if (n == null)
        {
          return true;
        }
        else
        {
          bool dataOk = (from == null || from < n.Data) && (to == null || n.Data < to);
          return dataOk && Go(from, n.Data, n.Left) && Go(n.Data, to, n.Right);
        }
      }

      // The nulls take the role of positive/negative infinity here.
      return Go(null, null, root);
    }

    public bool CheckThree()
    {
      int[] array = ToArray();
      for (int i = 0; i < array.Length - 1; i++)
      {
        if (array[i] >= array[i + 1])
        {
          return false;
        }
      }
      return true;
      // Works but needs extra O(n) memory.
    }
  }

  // Polynomials //
  //
  // Design and implement a C# class Polynomial representing a polynomial of a single variable. Your class should include the following:
  //
  //    a constructor that takes a variable number of arguments of type double, yielding a polynomial with those coefficients. For example,
  //
  //        new Polynomial(1.0, 4.0, 5.0)
  //
  //    should yield the polynomial x^2 + 4x + 5.
  //
  //    a property degree that returns the degree of a polynomial. For example, the polynomial above has degree 2.
  //
  //    an overloaded operator + that adds two polynomials
  //
  //    an overloaded operator - that subtracts two polynomials
  //
  //    an overloaded operator * that multiplies two polynomials, or a polynomial and a scalar
  //
  //    a method that evaluates a polynomial for a given value of x
  //
  //    a method that returns the first derivative of a polynomial
  //
  //    a method asString() that yields a string such as "x^2 + 4x + 5"
  //
  //    a static method Parse() that parses a string such as "x^2 + 4x + 5", yielding a polynomial.
  class Polynomial
  {
    private double[] coefs;

    public Polynomial(params double[] coefs)
    {
      // Skip leading zeros
      int start;
      for (start = 0; start < coefs.Length; start++)
      {
        if (coefs[start] != 0)
        {
          break;
        }
      }

      // Create a new array for the rest and copy it
      this.coefs = new double[coefs.Length - start];
      for (int i = 0; i < this.coefs.Length; i++)
      {
        this.coefs[i] = coefs[i + start];
      }
    }

    public int Degree => coefs.Length - 1;

    public string AsString()
    {
      if (Degree == -1)
      {
        return "0";
      }

      List<string> tokens = new();
      for (int i = 0; i < coefs.Length; i++)
      {
        double coef = coefs[i];
        if (coef == 0)
        {
          continue;
        }

        int exp = Degree - i;
        if (exp == 0)
        {
          tokens.Add($"{coef}");
        }
        else
        {
          string coefStr = coef == 1 ? "" : $"{coef}";
          string expStr = exp == 1 ? "" : $"^{exp}";
          tokens.Add(coefStr + "x" + expStr);
        }
      }

      // TODO: You can improve this to better handle negative coefficients
      // (i.e. "- 3x" instead of "+ -3x").
      return string.Join(" + ", tokens);
    }

    public double Eval(double at)
    {
      double r = 0;
      for (int i = 0; i < coefs.Length; i++)
      {
        r *= at;
        r += coefs[i];
      }
      return r;
    }

    public Polynomial Derivative()
    {
      // Constant polynomials transform into zero.
      if (Degree < 1)
        return new();

      double[] newCoefs = new double[Degree];
      for (int i = 0; i < Degree; i++)
      {
        // Degree - i is the exponent.
        newCoefs[i] = coefs[i] * (Degree - i);
      }
      return new(newCoefs);
    }

    static public Polynomial operator +(Polynomial left, Polynomial right)
    {
      if (left.Degree < right.Degree)
      {
        // Swap them so that left is the bigger one
        (left, right) = (right, left);
      }

      // Copy the larger polynomial
      double[] newCoefs = [..left.coefs];

      // Add the smaller one
      for (int i = 1; i <= right.Degree + 1; i++)
      {
        newCoefs[^i] += right.coefs[^i];
      }

      // We don't need to skip the leading zeros since the
      // constructor does that for us.
      return new(newCoefs);
    }

    static public Polynomial operator -(Polynomial left, Polynomial right) => left + (-1) * right;

    static public Polynomial operator *(Polynomial left, Polynomial right)
    {
      // One of the polynomials is zero
      if (left.Degree == -1 || right.Degree == -1)
        return new();

      int newDegree = left.Degree + right.Degree;
      double[] newCoefs = new double[newDegree + 1];
      for (int i = 1; i <= left.Degree + 1; i++)
      {
        for (int j = 1; j <= right.Degree + 1; j++)
        {
          newCoefs[^(i + j - 1)] += left.coefs[^i] * right.coefs[^j];
        }
      }

      return new(newCoefs);
    }

    static public Polynomial operator *(Polynomial left, double right) => left * new Polynomial(right);

    static public Polynomial operator *(double left, Polynomial right) => new Polynomial(left) * right;

    static public Polynomial Parse(string s)
    {
      // TODO: This can (again) be improved to better handle negative coefficients.
      // Currently, Polynomial.Parse("x - 1") won't work

      // We can safely ignore spaces here.
      string[] tokens = s.Replace(" ", "").Split('+');

      List<(double Coef, int Exp)> factors = new();
      foreach (string token in tokens)
      {
        string[] split = token.Split('x');
        if (split.Length == 1) // No 'x' present: constant term.
        {
          factors.Add((double.Parse(split[0]), 0));
        }
        else
        {
          double coef = split[0] == "" ? 1 : double.Parse(split[0]);
          // If the exponent is not empty, skip the '^' character at the start.
          // Technically, we should check that it is actually there and fail the
          // parsing otherwise.
          int exp = split[1] == "" ? 1 : int.Parse(split[1][1..]);
          factors.Add((coef, exp));
        }
      }

      // Find the highest exponent.
      int degree = factors.Select(p => p.Exp).Max();

      double[] newCoefs = new double[degree + 1];
      foreach (var (c, e) in factors)
      {
        newCoefs[^(e + 1)] = c;
      }
      return new(newCoefs);
    }
  }

  // Double-Ended Queue //
  //
  // Write a class Deque implementing a double-ended queue of integers:
  //
  // Your class should have these members:
  //
  //    Deque()
  //
  //    bool isEmpty()
  //
  //    void enqueueFirst(int i)
  //
  //    int dequeueFirst()
  //
  //    void enqueueLast(int i)
  //
  //    int dequeueLast()
  //
  // All operations should run in O(1). In dequeueFirst() and dequeueLast(), you may assume that the queue is not empty.
  class Deque
  {
    private class Node
    {
      public int Data;
      public Node? Prev;
      public Node? Next;

      public Node(int data, Node? prev = null, Node? next = null)
      {
        Data = data;
        Prev = prev;
        Next = next;
      }
    }

    private Node? head;
    private Node? tail;

    public Deque()
    {
      head = null;
      tail = null;
    }

    public bool IsEmpty() => head == null && tail == null;

    public void EnqueueFirst(int value)
    {
      if (head == null)
      {
        head = tail = new(value);
        return;
      }

      Node n = new(value, null, head);
      head.Prev = n;
      head = n;
    }

    public void EnqueueLast(int value)
    {
      if (tail == null)
      {
        head = tail = new(value);
        return;
      }

      Node n = new(value, tail, null);
      tail.Next = n;
      tail = n;
    }

    public int DequeueFirst()
    {
      if (head == null)
      {
        throw new Exception();
      }

      int value = head.Data;
      if (head.Next == null)
      {
        head = tail = null;
      }
      else
      {
        head = head.Next;
        head.Prev = null;
      }
      return value;
    }

    public int DequeueLast()
    {
      if (tail == null)
      {
        throw new Exception();
      }

      int value = tail.Data;
      if (tail.Prev == null)
      {
        head = tail = null;
      }
      else
      {
        tail = tail.Prev;
        tail.Next = null;
      }
      return value;
    }
  }

  class Program
  {
    static void Main(string[] args)
    {

    }
  }
}
