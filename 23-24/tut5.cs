using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
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
      double[] newCoefs = left.coefs.ToArray();

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
