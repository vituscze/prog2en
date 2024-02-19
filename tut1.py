### Programming 2 Tutorial ###

# How to pass the tutorial
#
# * Attendance strongly recommended, but not mandatory
# * Homework
#   - Assigned every week, starting next week
#   - 70% (i.e. 70 points) required to pass the tutorial
#   - Points above 90% go towards the exam (but no more than 10%)
#   - Submitted via ReCodEx (https://recodex.mff.cuni.cz/)
#     * Check README for useful links (new user guide, student guide)
#   - Do not copy code from other students or the internet
# * Semestral project
#   - Solve a nontrivial problem in C#
#   - Can be implementation of an algorithm, a computer game, etc
#   - Before you start working, submit a proposal via email and wait for confirmation
#     * Description of the problem and specification of the solution
#     * Doesn't have to be long, 1-2 paragraphs is usually enough
#   - To submit a project, you need to provide:
#     * Source code
#     * Test data (sample inputs and outputs, if applicable)
#     * User documentation (explain how to use the program to your users)
#     * Developer documentation (explain how your programs works to other developers)
#   - Deadlines are on the course page (https://ksvi.mff.cuni.cz/~dingle/2023-4/prog_2/programming_2.html)

# Study materials
#
# * GitHub repository
# * Discord server (optional)
# * Recordings
#   - Posted every week to the Discord server
#   - Also available on request

## Recursion recap ##

# Recursion is the idea of defining some entity in terms of itself. In programming,
# we typically use recursion with functions.
#
# A recursive function consists of two parts: base case and recursive case. Base
# case handles inputs where the output is immediately known and no recursion is
# necessary. Recursive case handles more complex inputs. Recursive function works
# only if we eventually reach a base case, i.e. the recursive step should be using
# the function with smaller and smaller inputs.
#
# Recursion is very similar to mathematical induction and it makes sense to think
# about recursive definitions in that terms. When trying to write a recursive function,
# you should think about the inputs where the output is immediately known (base cases).
# Then, just like in a proof by induction, assume that the function already works for
# smaller inputs and use these results to build the correct output.

# No base case: never terminates
def infinite(n):
    return infinite(n - 1)

# One base case which is reached for all non-negative numbers. Never terminates
# for negative numbers.
def maybe_finite(n):
    if n == 0:
        return 0
    return maybe_finite(n - 1)

# One base case which is reached for all numbers. Always terminates.
def finite(n):
    if n <= 0:
        return 0
    return finite(n - 1)

# We can extract the output of a recursive function in multiple ways. Typically,
# the simplest one is to just use the return values of these functions. Let's take
# a look at an example:

class Node:
    def __init__(self, v, l=None, r=None):
        self.value = v
        self.left = l
        self.right = r
        
def leaves(tree):
    if tree is None:
        return []
    # No children: leaf node
    if tree.left is None and tree.right is None:
        return [tree.value]
    l = []
    l += leaves(tree.left)
    l += leaves(tree.right)
    return l

# >>> t = Node(5, Node(6), Node(7))
# >>> leaves(t)

def contains(tree, val):
    # Empty tree contains no values
    if tree is None:
        return False
    # Value is in the tree
    if tree.value == val:
        return True
    # Otherwise look into the left and right subtrees
    in_left = contains(tree.left, val)
    in_right = contains(tree.right, val)
    # We're skipping a potential optimization here: if the value is
    # found in the left tree, there's no need to check the right tree
    return in_left or in_right

# >>> t = Node(5, Node(6), Node(7))
# >>> contains(t, 4)

# Sometimes, we can skip the return entirely and mutate some
# value as a side effect instead. In the case of lists, this has the
# (fairly large) advantage of not having to create so many small lists
# that are immediately thrown away. Applying it to the leaves function:

def leaves(tree, output):
    if tree is None:
        return # No return value
    # No children: leaf node
    if tree.left is None and tree.right is None:
        # Mutate the output list
        output.append(tree.value)
    leaves(tree.left, output)
    leaves(tree.right, output)
    # No return value, the output list has been modified instead

# >>> t = Node(5, Node(6), Node(7))
# >>> out = []
# >>> leaves(t, out)
# >>> out

# One slightly annoying aspect of this is that we need to keep passing
# the output variable around. We can use a local function with a non-local
# variable to get around this:

def leaves(tree):
    def go(tree):
        if tree is None:
            return # No return value
        # No children: leaf node
        if tree.left is None and tree.right is None:
            # Mutate the output list
            output.append(tree.value)
        go(tree.left)
        go(tree.right) 
    
    output = [] 
    go(tree)
    return output

# You should definitely use non-local variables here instead of global variables.
# One advantage of this approach is that it can be applied even to immutable values
# (like booleans):

def contains(tree, val):
    def go(tree):
        nonlocal result
        # Empty tree contains no values
        if tree is None:
            return
        # Value is in the tree
        if tree.value == val:
            result = True # Set the outer variable result
            return
        # Otherwise look into the left and right subtrees
        go(tree.left)
        go(tree.right)
    
    result = False
    go(tree)
    return result

# In Python, there is a default recursion depth limit of 1000. For example,
# we couldn't use the above functions to find something in a list of height
# greater than 1000. C# allows you to recurse as much as you want as long
# as you don't run out of stack space (which generally allows much deeper
# recursion than 1000).

## Combinatorial recursion ##

# In this semester, we'll be using recursion to solve combinatorial problems.
# We'll see more details later, but here's a small preview:

# A powerset is a set of all subsets of a given set. For example, the
# powerset of a set {1,2,3} is {{},{1},{2},{3},{1,2},{1,3},{2,3},{1,2,3}}.
# Notice that we also include the empty set {} and the entire set {1,2,3}.
# As a result, for a set of size N, the powerset always has exactly 2^N
# subsets.

# Let's take a closer look at the subset in the previous paragraph. We can
# separate the subsets into two categories, those that contain 1 and those
# that don't:
#
# { {},   {2},   {3},   {2,3}}
# {{1}, {1,2}, {1,3}, {1,2,3}}

# Notice that the first set is a powerset of the set {2,3}. The second set
# is simply the first set but each subset has an additional 1. We can see
# that this problem might be solved recursively.

# To make matters simpler, we'll be using normal lists instead of sets.
# Let's think about the base case: the powerset of an empty set is a set
# containing only a single subset: the empty subset:

def powerset(s):
    if len(s) == 0:
        return [[]] # List with a single element: the empty list
    
# For the recursive case, we assume that the function correctly produces
# powersets of smaller sets. We split to obtain one of its elements and the
# rest:

    x, *rest = s
    
# We recursively compute the powerset of the smaller set rest:

    p = powerset(rest)
    
# As we saw before, p is only half of the output. To get the other half, we
# need to create a new list where we add x to every element.

    p_x = [[x] + l for l in p]
    
# The result is then obtained by adding up these two lists:

    return p + p_x

# Question: what would happen if we returned empty list in the base case?

# We know that Cartesian product can be written simply in Python as:

def cartesian(xs, ys):
    return [[x, y] for x in xs for y in ys]

# In the special case of xs == ys, we can write:

def square(xs):
    return [[x, y] for x in xs for y in xs]

# We can generalize this to any power:
#
#   A^1 = A
#   A^2 = A x A
#   A^3 = A x A x A
#   ...

# Think about what the base case A^0 should be and then try to implement the function power
# that computes the n-th power of the list A in this way.

def power(xs, n):
    pass

# >>> power([0, 1], 2)
# [[0, 0], [0, 1], [1, 0], [1, 1]]
# >>> power([0, 1], 3)
# [[0, 0, 0], [0, 0, 1], [0, 1, 0], [0, 1, 1], [1, 0, 0], [1, 0, 1], [1, 1, 0], [1, 1, 1]]
