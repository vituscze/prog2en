# Programming 2 Tutorial //

# How to pass the tutorial
#
# * Attendance strongly recommended, but not mandatory
# * Homework
#   - Assigned every week, starting next week
#   - 70% (i.e. 70 points) required to pass the tutorial
#   - Submitted via ReCodEx (https://recodex.mff.cuni.cz/)
#     * Check README for useful links (new user guide, student guide)
#   - Do not copy code from other students or the internet
# * Semestral project
#   - Solve a nontrivial problem in C#
#   - Can be implementation of an algorithm, a computer game, etc
#   - Before you start working, submit a proposal via email and wait for confirmation
#     * Description of the problem and specification of the solution
#     * Doesn't have to be long, 1-2 paragraphs is usually enough
#     * Submit via ReCodEx
#   - To submit a project, you need to provide:
#     * Source code
#     * Test data (sample inputs and outputs, if applicable)
#     * User documentation (explain how to use the program to your users)
#     * Developer documentation (explain how your programs works to other developers)
#   - Use Git for the project
#   - Deadlines are on the course page (https://ksvi.mff.cuni.cz/~dingle/2025-6/prog_2/programming_2.html)

# Recursion and discussion about common difficulties during the test

class Node:
    def __init__(self, v, l=None, r=None):
        self.value = v
        self.left = l
        self.right = r

def height(t):
    if t is None:
        return 0

    return 1 + max(height(t.left), height(t.right))

# AVL tree condition: for each node, the heights of the children
# cannot differ by more than 1.
def balanced(t):
    if t is None:
        return True

    # You generally shouldn't compute the same thing multiple times
    # but it's especially important with recursion as it could easily
    # turn a nice linear recursion into an exponential monstrosity

    # if height(t.left) - height(t.right) > 1 or height(t.right) - height(t.left) > 1:
    #     return False

    # Better solution:
    lh = height(t.left)
    rh = height(t.right)
    # if lh - rh > 1 or rh - lh > 1:
    #     return False

    # Even better solution:
    if abs(lh - rh) > 1:
        return False

    # Then we just check that left and right subtrees are also balanced.
    return balanced(t.left) and balanced(t.right)

# Time complexity? height(t) is linear in tree size; for a reasonably balanced
# tree we spend on the order of O(size) on each layer, giving us O(size * log size).
# Not horrible but we could do better!

# Problem: we're recomputing heights for a lot of subtrees. We could either precompute
# all the heights somewhere else and use them in balanced(t), which is annoying. We
# could also compute the heights as we go, avoiding any intermediate structures!

def better_balanced(tree):
    # Recursive helper that computes balancing info AND height
    def go(t):
        if t is None:
            # Empty tree is balanced and its height is 0
            return True, 0

        lb, lh = go(t.left)
        rb, rh = go(t.right)

        # Just going by definition: A tree is balanced if the subtrees are
        # and their heights differ by at most one.
        b = lb and rb and abs(lh - rh) <= 1
        h = 1 + max(lh, rh)
        return b, h

    b, _ = go(tree)
    return b

# Time complexity? Clearly we only do O(1) work for each node in the tree, giving us
# O(size) total.

# Another fairly common mistake was just not using the return values. That's not to say
# you can't write a recursive function that doesn't have a return value, but it should be
# a deliberate decision. Consider:

def leaves(t):
    if t is None:
        return []

    if t.left is None and t.right is None:
        # We found a leaf!
        return [t.value]

    return leaves(t.left) + leaves(t.right)

# Works fine but + on lists is linear and we're gonna be joining a lot of small lists together.
# It would be easier if we only had a single list that we were adding to.

def leaves_better(tree):
    def go(t, ls):
        if t is None:
            return

        if t.left is None and t.right is None:
            ls.append(t.value)
            return

        go(t.left, ls)
        go(t.right, ls)

    res = []
    go(tree, res)
    return res

# Since the ls parameter doesn't ever change, it might be useful to pull it out of the
# local function as a nonlocal variable:

def leaves_better2(tree):
    res = []
    def go(t):
        if t is None:
            return

        if t.left is None and t.right is None:
            res.append(t.value)
            return

        go(t.left)
        go(t.right)

    go(tree)
    return res

# Since we're not changing res directly, we don't need the "nonlocal" keyword but you should still
# think of it as a nonlocal variable!

# What's the difference between these two?

def remove_leaves1(t):
    if t is None:
        return None

    if t.left is None and t.right is None:
        return None

    # Note that most of the time, this assignment won't do anything interesting. For the inner
    # nodes, the function just returns its input and you're essentially doing just
    #
    #   t.left = t.left; t.right = t.right
    #
    # It's only different when you encounter a leaf node and then you set one (or both) of these
    # to None. It's also possible to avoid the return value and check for the leaf in some different
    # way. E.g.:
    #
    # if is_leaf(t.left):
    #     t.left = None
    #
    # (Needs a bit of extra code to handle the case where the entire tree is a single leaf)
    t.left = remove_leaves1(t.left)
    t.right = remove_leaves1(t.right)
    return t

def remove_leaves2(t):
    if t is None:
        return None

    if t.left is None and t.right is None:
        return None

    return Node(t.value, remove_leaves2(t.left), remove_leaves2(t.right))

# Which one to pick? Depends on your use case.

# Sneak peek at what's to come: combinatiorial recursion. We can use recursion to implement
# more than just tree operations and stuff that's better done with loops. Consider the problem
# of finding the powerset (the set of all subsets). For simplicity, I'll represent the sets with
# a normal Python list (since I'll need an easy way to access the first element).

# powerset({1,2,3}) = {{}, {1}, {2}, {1,2}, {3}, {1,3}, {1,2,3}}
#
# Notice an interesting property:
#
# powerset({2,3}) = {{}, {2}, {3}, {2,3}}
#
# We can get the subsets of {1,2,3} by taking the subsets of {2,3} and creating two copies of each:
# one with no extra elements and one with an extra 1.

def powerset(elems):
    if len(elems) == 0:
        # The powerset of the empty set is *not* empty! It's a single
        # empty set.
        return [[]]

    # The powerset of the remaining elements
    ps_rest = powerset(elems[1:])
    result = []
    # For each powerset:
    for ps in ps_rest:
        # Either leave it as it was:
        result.append(ps)
        # Or add the element we skipped:
        result.append([elems[0]] + ps)

    return result
