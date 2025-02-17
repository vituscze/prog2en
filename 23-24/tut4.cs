using System;

namespace ConsoleApp1
{
  // Dynamic Array //
  //
  // Write a class DynArray that represents a variable-sized array of integers, with indices starting at 0. It should have the following members:
  //
  //   DynArray() - create a new empty DynArray
  //
  //   int length() - return the current array length
  //
  //   void append(int i) - append an integer to the array
  //
  //   int get(int index) - read the integer at the given index
  //
  //   void set(int index, int val) - set the integer at the given index, which must be within the current array bounds
  //
  // Do not use the C# class List<T> in your implementation! The point of this exercise is to implement a dynamic array yourself, using arrays.
  // What will be the average-case big-O running time of append()?
  class DynArray
  {
    private const int initialCapacity = 16;
    private int[] data;
    private int size;

    public DynArray()
    {
      // Create an array with the initial capacity
      data = new int[initialCapacity];
      size = 0;
    }

    private void Resize()
    {
      int[] newData = new int[data.Length * 2];
      for (int i = 0; i < data.Length; i++)
      {
        newData[i] = data[i];
      }
      data = newData;
    }

    public int Length() => size;

    public void Append(int i)
    {
      if (size == data.Length)
      {
        Resize();
      }

      data[size++] = i;
    }

    public int Get(int ix) => data[ix];

    public void Set(int ix, int val)
    {
      if (ix >= 0 && ix < size)
      {
        data[ix] = val;
      }
    }
  }

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

  class Program
  {
    static void Main(string[] args)
    {

    }
  }
}
