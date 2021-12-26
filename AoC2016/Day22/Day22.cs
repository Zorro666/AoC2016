using System;
using System.Collections.Generic;

/*

--- Day 22: Grid Computing ---

You gain access to a massive storage cluster arranged in a grid; each storage node is only connected to the four nodes directly adjacent to it (three if the node is on an edge, two if it's in a corner).

You can directly access data only on node /dev/grid/node-x0-y0, but you can perform some limited actions on the other nodes:

You can get the disk usage of all nodes (via df). 
The result of doing this is in your puzzle input.
You can instruct a node to move (not copy) all of its data to an adjacent node (if the destination node has enough space to receive the data). 
The sending node is left empty after this operation.
Nodes are named by their position: the node named node-x10-y10 is adjacent to nodes node-x9-y10, node-x11-y10, node-x10-y9, and node-x10-y11.

Before you begin, you need to understand the arrangement of data on these nodes. 
Even though you can only move data between directly connected nodes, you're going to need to rearrange a lot of the data to get access to the data you need. 
Therefore, you need to work out how you might be able to shift data around.

To do this, you'd like to count the number of viable pairs of nodes. 
A viable pair is any two nodes (A,B), regardless of whether they are directly connected, such that:

Node A is not empty (its Used is not zero).
Nodes A and B are not the same node.
The data on node A (its Used) would fit on node B (its Avail).
How many viable pairs of nodes are there?

root@ebhq-gridcenter# df -h
Filesystem              Size  Used  Avail  Use%
/dev/grid/node-x0-y0     92T   68T    24T   73%
/dev/grid/node-x0-y1     87T   73T    14T   83%
/dev/grid/node-x0-y2     89T   64T    25T   71%
/dev/grid/node-x0-y3     91T   64T    27T   70%
/dev/grid/node-x0-y4     86T   73T    13T   84%
/dev/grid/node-x0-y5     90T   71T    19T   78%
/dev/grid/node-x0-y6     88T   66T    22T   75%

--- Part Two ---

Now that you have a better understanding of the grid, it's time to get to work.

Your goal is to gain access to the data which begins in the node with y=0 and the highest x (that is, the node in the top-right corner).

For example, suppose you have the following grid:

Filesystem            Size  Used  Avail  Use%
/dev/grid/node-x0-y0   10T    8T     2T   80%
/dev/grid/node-x0-y1   11T    6T     5T   54%
/dev/grid/node-x0-y2   32T   28T     4T   87%
/dev/grid/node-x1-y0    9T    7T     2T   77%
/dev/grid/node-x1-y1    8T    0T     8T    0%
/dev/grid/node-x1-y2   11T    7T     4T   63%
/dev/grid/node-x2-y0   10T    6T     4T   60%
/dev/grid/node-x2-y1    9T    8T     1T   88%
/dev/grid/node-x2-y2    9T    6T     3T   66%

In this example, you have a storage grid 3 nodes wide and 3 nodes tall. 
The node you can access directly, node-x0-y0, is almost full. 
The node containing the data you want to access, node-x2-y0 (because it has y=0 and the highest x value), contains 6 terabytes of data - enough to fit on your node, if only you could make enough space to move it there.

Fortunately, node-x1-y1 looks like it has enough free space to enable you to move some of this data around. 
In fact, it seems like all of the nodes have enough space to hold any node's data (except node-x0-y2, which is much larger, very full, and not moving any time soon). 
So, initially, the grid's capacities and connections look like this:

( 8T/10T) --  7T/ 9T -- [ 6T/10T]
    |           |           |
  6T/11T  --  0T/ 8T --   8T/ 9T
    |           |           |
 28T/32T  --  7T/11T --   6T/ 9T
The node you can access directly is in parentheses; the data you want starts in the node marked by square brackets.

In this example, most of the nodes are interchangable: they're full enough that no other node's data would fit, but small enough that their data could be moved around. 
Let's draw these nodes as .. 
The exceptions are the empty node, which we'll draw as _, and the very large, very full node, which we'll draw as #. 
Let's also draw the goal data as G. 
Then, it looks like this:

(.) .  G
 .  _  .
 #  .  .
The goal is to move the data in the top right, G, to the node in parentheses. 
To do this, we can issue some commands to the grid and rearrange the data:

Move data from node-y0-x1 to node-y1-x1, leaving node node-y0-x1 empty:

(.) _  G
 .  .  .
 #  .  .
Move the goal data from node-y0-x2 to node-y0-x1:

(.) G  _
 .  .  .
 #  .  .
At this point, we're quite close. 
However, we have no deletion command, so we have to move some more data around. 
So, next, we move the data from node-y1-x2 to node-y0-x2:

(.) G  .
 .  .  _
 #  .  .
Move the data from node-y1-x1 to node-y1-x2:

(.) G  .
 .  _  .
 #  .  .
Move the data from node-y1-x0 to node-y1-x1:

(.) G  .
 _  .  .
 #  .  .
Next, we can free up space on our node by moving the data from node-y0-x0 to node-y1-x0:

(_) G  .
 .  .  .
 #  .  .
Finally, we can access the goal data by moving the it from node-y0-x1 to node-y0-x0:

(G) _  .
 .  .  .
 #  .  .
So, after 7 steps, we've accessed the data we want. 
Unfortunately, each of these moves takes time, and we need to be efficient:

What is the fewest number of steps required to move your goal data to node-x0-y0?

*/

namespace Day22
{
    class Program
    {
        struct Move
        {
            public int NextChildMove;
            public List<int> ChildMoveIndexes;
            public List<int> ParentMoveIndexes;
            public int MoveIndex;
            public int NodeFromIndex;
            public int NodeToIndex;
            public int DataMoved;
            public int GoalNodeIndex;
            public int HoleNodeIndex;
        }

        struct Node
        {
            public int X;
            public int Y;
            public int Index;
            public int Size;
            public int Used;
            public int Avail;
            public bool Goal;
            public int[] Neighbours;
        }

        readonly static int MAX_HEIGHT = 64;
        readonly static int MAX_WIDTH = 64;
        readonly static int MAX_NUM_NODES = MAX_HEIGHT * MAX_WIDTH;
        readonly static Node[] sStartNodes = new Node[MAX_NUM_NODES];
        readonly static Node[] sCurrentNodes = new Node[MAX_NUM_NODES];
        static int sWidth;
        static int sHeight;
        static int sNodeCount;
        static int sGoalNodeIndex;
        static int sHoleNodeIndex;
        readonly static int sRootMoveIndex = 0;

        readonly static int MAX_STEPS_DEPTH = 500;
        readonly static int MAX_NUM_MOVES = 1000;
        static Move[] sMoves;
        static List<int> sPendingMoves = new List<int>(MAX_NUM_MOVES);
        static int sMoveCount;

        static List<int[]> sPreviousBoardStates;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = CountViablePairs();
                Console.WriteLine($"Day22 : Result1 {result1}");
                var expected = 1038;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = ShortestSteps();
                Console.WriteLine($"Day22 : Result2 {result2}");
                var expected = 252;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 2)
            {
                throw new InvalidProgramException($"Not enough input data need at least 3 lines {lines.Length}");
            }
            string expected;

            expected = @"root@ebhq-gridcenter# df -h";
            if (lines[0].Trim() != expected)
            {
                throw new InvalidProgramException($"Invalid first line '{lines[0]}' Expected:'{expected}'");
            }

            expected = @"Filesystem              Size  Used  Avail  Use%";
            if (lines[1].Trim() != expected)
            {
                throw new InvalidProgramException($"Invalid second line '{lines[1]}' Expected:'{expected}'");
            }

            if (lines.Length - 2 > MAX_NUM_NODES)
            {
                throw new InvalidProgramException($"Too many input nodes {lines.Length - 2} Max:{MAX_NUM_NODES}");
            }

            sWidth = int.MinValue;
            sHeight = int.MinValue;
            sNodeCount = 0;
            for (var i = 0; i < MAX_NUM_NODES; ++i)
            {
                ref Node node = ref sStartNodes[i];
                node.X = -1;
                node.Y = -1;
                node.Index = -1;
                node.Size = 0;
                node.Used = 0;
                node.Avail = 0;
                node.Goal = false;
                node.Neighbours = null;
            }

            for (var i = 2; i < lines.Length; ++i)
            {
                //"/dev/grid/node-x0-y0     92T   68T    24T   73%"
                var tokens = lines[i].Trim().Split(' ');
                var nodeName = tokens[0];

                var nextToken = 1;
                for (var j = 1; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var size = int.Parse(tokens[nextToken].TrimEnd('T'));
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var used = int.Parse(tokens[nextToken].TrimEnd('T'));
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var avail = int.Parse(tokens[nextToken].TrimEnd('T'));
                /*
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        break;
                    }
                }
                var availPercentage = int.Parse(tokens[nextToken].TrimEnd('%'));
                */

                var nameTokens = nodeName.Split('-');
                var x = int.Parse(nameTokens[1].TrimStart('x'));
                var y = int.Parse(nameTokens[2].TrimStart('y'));

                var index = GetIndexFromXY(x, y);
                sWidth = Math.Max(sWidth, x);
                sHeight = Math.Max(sHeight, y);
                sNodeCount = Math.Max(sNodeCount, index + 1);
                if (sWidth >= MAX_WIDTH)
                {
                    throw new InvalidProgramException($"Grid is too wide {sWidth} Max::{MAX_WIDTH}");
                }
                if (sHeight >= MAX_HEIGHT)
                {
                    throw new InvalidProgramException($"Grid is too tall {sHeight} Max::{MAX_HEIGHT}");
                }
                if (sNodeCount >= MAX_NUM_NODES)
                {
                    throw new InvalidProgramException($"Grid is too big {sNodeCount} Max::{MAX_NUM_NODES}");
                }

                ref Node node = ref sStartNodes[index];
                node.X = x;
                node.Y = y;
                node.Index = index;
                node.Size = size;
                node.Used = used;
                node.Avail = avail;
                node.Neighbours = new int[4];
                for (var n = 0; n < 4; ++n)
                {
                    node.Neighbours[n] = -1;
                }
                //Console.WriteLine($"X:{x} Y:{y} {size}T {used}T {avail}T {availPercentage}%");
            }

            bool foundGoal = false;
            for (var i = 0; i < sNodeCount; ++i)
            {
                ref Node node = ref sStartNodes[i];
                var x = node.X;
                var y = node.Y;
                if ((y == 0) && (x == sWidth))
                {
                    if (foundGoal)
                    {
                        throw new InvalidProgramException($"Found more than one goal X:{x} Y:{y}");
                    }
                    node.Goal = true;
                    foundGoal = true;
                    sGoalNodeIndex = i;
                }
                if (node.Used == 0)
                {
                    sHoleNodeIndex = i;
                }
                var index = node.Index;
                if (index == -1)
                {
                    continue;
                }
                if (index != GetIndexFromXY(x, y))
                {
                    throw new InvalidProgramException($"Incorrect Node Index {index} != {GetIndexFromXY(x, y)} X:{x} Y:{y}");
                }
                var neighbourCount = 0;
                if (x > 0)
                {
                    var neighbourIndex = GetIndexFromXY(x - 1, y + 0);
                    if (sStartNodes[neighbourIndex].Index != -1)
                    {
                        node.Neighbours[neighbourCount] = neighbourIndex;
                        ++neighbourCount;
                    }
                }
                if (y > 0)
                {
                    var neighbourIndex = GetIndexFromXY(x + 0, y - 1);
                    if (sStartNodes[neighbourIndex].Index != -1)
                    {
                        node.Neighbours[neighbourCount] = neighbourIndex;
                        ++neighbourCount;
                    }
                }
                if (x + 1 < MAX_WIDTH)
                {
                    var neighbourIndex = GetIndexFromXY(x + 1, y + 0);
                    if (sStartNodes[neighbourIndex].Index != -1)
                    {
                        node.Neighbours[neighbourCount] = neighbourIndex;
                        ++neighbourCount;
                    }
                }
                if (y + 1 < MAX_HEIGHT)
                {
                    var neighbourIndex = GetIndexFromXY(x + 0, y + 1);
                    if (sStartNodes[neighbourIndex].Index != -1)
                    {
                        node.Neighbours[neighbourCount] = neighbourIndex;
                        ++neighbourCount;
                    }
                }
                for (var n = neighbourCount; n < 4; ++n)
                {
                    node.Neighbours[n] = -1;
                }
            }
            if (!foundGoal)
            {
                throw new InvalidProgramException($"Failed to find goal node");
            }
            ResetBoard();
        }

        static void ResetBoard()
        {
            for (var i = 0; i < sNodeCount; ++i)
            {
                ref Node node = ref sStartNodes[i];
                sCurrentNodes[i] = node;
                if (node.Goal)
                {
                    sGoalNodeIndex = i;
                }
                if (node.Used == 0)
                {
                    sHoleNodeIndex = i;
                }
            }
        }

        public static int CountViablePairs()
        {
            ResetBoard();
            var viableCount = 0;
            for (var i = 0; i < sNodeCount; ++i)
            {
                ref Node nodeA = ref sCurrentNodes[i];
                if (nodeA.Index == -1)
                {
                    continue;
                }
                var usedA = nodeA.Used;
                if (usedA == 0)
                {
                    continue;
                }
                for (var j = 0; j < sNodeCount; ++j)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    ref Node nodeB = ref sCurrentNodes[j];
                    if (nodeB.Index == -1)
                    {
                        continue;
                    }
                    var availB = nodeB.Avail;
                    if (usedA <= availB)
                    {
                        ++viableCount;
                    }
                }
            }
            return viableCount;
        }

        static List<(int from, int to)> FindMoves()
        {
            var moves = new List<(int from, int to)>(8192);
            for (var i = 0; i < sNodeCount; ++i)
            {
                ref Node nodeA = ref sCurrentNodes[i];
                if (nodeA.Index == -1)
                {
                    continue;
                }
                var usedA = nodeA.Used;
                if (usedA == 0)
                {
                    continue;
                }
                var fromIndex = nodeA.Index;
                for (var n = 0; n < 4; ++n)
                {
                    var neighbourIndex = nodeA.Neighbours[n];
                    if (neighbourIndex == -1)
                    {
                        continue;
                    }
                    ref Node nodeB = ref sCurrentNodes[neighbourIndex];
                    if (nodeB.Goal)
                    {
                        continue;
                    }
                    if (nodeA.Goal)
                    {
                        if (nodeB.Used != 0)
                        {
                            continue;
                        }
                    }
                    var availB = nodeB.Avail;
                    if (usedA <= availB)
                    {
                        var toIndex = nodeB.Index;
                        moves.Add((fromIndex, toIndex));
                    }
                }
            }
            /*
            // Sort the moves to pick the ones closest to the goal first
            (int goalX, int goalY) = GetXYFromIndex(sGoalNodeIndex);
            for (var i = 0; i < moves.Count - 1; ++i)
            {
                for (var j = 0; j < moves.Count; ++j)
                {
                    var fromI = moves[i].from;
                    (int toIX, int toIY) = GetXYFromIndex(fromI);
                    var distanceI = Math.Abs(goalX - toIX) + Math.Abs(goalY - toIY);

                    var fromJ = moves[j].from;
                    (int toJX, int toJY) = GetXYFromIndex(fromJ);
                    var distanceJ = Math.Abs(goalX - toJX) + Math.Abs(goalY - toJY);

                    if (distanceJ < distanceI)
                    {
                        var temp = moves[i];
                        moves[i] = moves[j];
                        moves[j] = temp;
                    }
                }
            }
            */
            return moves;
        }

        static int GetIndexFromXY(int x, int y)
        {
            return y * MAX_WIDTH + x;
        }

        static (int x, int y) GetXYFromIndex(int index)
        {
            var y = index / MAX_WIDTH;
            var x = index - (y * MAX_WIDTH);
            return (x, y);
        }

        static void PerformMove(int moveIndex)
        {
            if (moveIndex == sRootMoveIndex)
            {
                return;
            }
            ref Move move = ref sMoves[moveIndex];
            if (move.NodeFromIndex == -1)
            {
                throw new InvalidProgramException($"PerformMove: Invalid NodeFromIndex {move.NodeFromIndex} Move {moveIndex}");
            }
            if (move.NodeToIndex == -1)
            {
                throw new InvalidProgramException($"PerformMove: Invalid NodeToIndex {move.NodeToIndex} Move {moveIndex}");
            }
            MoveData(move.DataMoved, move.NodeFromIndex, move.NodeToIndex);
            ref Node fromNode = ref sCurrentNodes[move.NodeFromIndex];
            if (fromNode.Used != 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Used after move {fromNode.Used} != 0 Node {move.NodeFromIndex}");
            }
            if (fromNode.Avail != fromNode.Size)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Avail after move {fromNode.Avail} != {fromNode.Size} Node {move.NodeFromIndex}");
            }
            //Console.WriteLine($"Move {move.NodeFromIndex}:{move.NodeToIndex} {fromNode.X},{fromNode.Y} -> {sCurrentNodes[move.NodeToIndex].X},{sCurrentNodes[move.NodeToIndex].Y} {move.DataMoved}");
        }

        static void UndoMove(int moveIndex)
        {
            ref Move move = ref sMoves[moveIndex];
            if (move.NodeFromIndex == -1)
            {
                throw new InvalidProgramException($"PerformMove: Invalid NodeFromIndex {move.NodeFromIndex} Move {moveIndex}");
            }
            if (move.NodeToIndex == -1)
            {
                throw new InvalidProgramException($"PerformMove: Invalid NodeToIndex {move.NodeToIndex} Move {moveIndex}");
            }
            MoveData(move.DataMoved, move.NodeToIndex, move.NodeFromIndex);
        }

        static void MoveData(int dataMoved, int fromNodeIndex, int toNodeIndex)
        {
            if (dataMoved <= 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid dataMoved {dataMoved} Node {fromNodeIndex}");
            }
            ref Node fromNode = ref sCurrentNodes[fromNodeIndex];
            fromNode.Used -= dataMoved;
            fromNode.Avail += dataMoved;
            if (fromNode.Used < 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Used after move {fromNode.Used} < 0 Node {fromNodeIndex}");
            }
            if (fromNode.Used > fromNode.Size)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Used after move {fromNode.Used} > {fromNode.Size} Node {fromNodeIndex}");
            }
            if (fromNode.Avail < 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Avail after move {fromNode.Avail} < 0 Node {fromNodeIndex}");
            }
            if (fromNode.Avail > fromNode.Size)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Avail after move {fromNode.Avail} > {fromNode.Size} Node {fromNodeIndex}");
            }

            ref Node toNode = ref sCurrentNodes[toNodeIndex];
            toNode.Used += dataMoved;
            toNode.Avail -= dataMoved;
            if (toNode.Used < 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Used after move {toNode.Used} < 0 Node {toNodeIndex}");
            }
            if (toNode.Used > toNode.Size)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Used after move {toNode.Used} > {toNode.Size} Node {toNodeIndex}");
            }
            if (toNode.Avail < 0)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Avail after move {toNode.Avail} < 0 Node {toNodeIndex}");
            }
            if (toNode.Avail > toNode.Size)
            {
                throw new InvalidProgramException($"PerformMove: Invalid Avail after move {toNode.Avail} > {toNode.Size} Node {toNodeIndex}");
            }

            if (fromNode.Goal || (fromNodeIndex == sGoalNodeIndex))
            {
                fromNode.Goal = false;
                toNode.Goal = true;
                sGoalNodeIndex = toNodeIndex;
                //Console.WriteLine($"Goal Node {sGoalNodeIndex} {GetXYFromIndex(sGoalNodeIndex).x},{GetXYFromIndex(sGoalNodeIndex).y}");
            }
            if (fromNode.Used == 0)
            {
                sHoleNodeIndex = fromNodeIndex;
            }
            //OutputCurrentState();
            //OutputBoard();
            //ValidateCurrentState();
        }

        public static int ShortestSteps()
        {
            //OutputBoard();
            var goalMinSteps = int.MaxValue;
            goalMinSteps = ShortestStepsImpl(goalMinSteps);
            return goalMinSteps;
        }

        static int[] GenerateCurrentBoardState()
        {
            int[] state = new int[MAX_NUM_NODES];
            for (var i = 0; i < sNodeCount; ++i)
            {
                var used = sCurrentNodes[i].Used;
                state[i] = used;
            }
            return state;
        }

        static bool AddCurrentBoardStateAlreadyExist()
        {
            var currentBoardState = GenerateCurrentBoardState();
            foreach (var state in sPreviousBoardStates)
            {
                bool alreadyExists = true;
                for (var i = 0; i < sNodeCount; ++i)
                {
                    if (state[i] != currentBoardState[i])
                    {
                        alreadyExists = false;
                        break;
                    }
                }
                if (alreadyExists)
                {
                    return false;
                }
            }
            sPreviousBoardStates.Add(currentBoardState);
            return true;
        }

        static bool AddNewMoves(int moveIndex)
        {
            ref Move parentMove = ref sMoves[moveIndex];
            if (parentMove.ChildMoveIndexes == null)
            {
                var moves = FindMoves();
                if (moves.Count == 0)
                {
                    return true;
                }

                if (moves.Count > 0)
                {
                    parentMove.ChildMoveIndexes = new List<int>(moves.Count);
                    parentMove.NextChildMove = 0;
                }

                var parentMoveFrom = parentMove.NodeFromIndex;
                var parentMoveTo = parentMove.NodeToIndex;
                foreach (var (fromNodeIndex, toNodeIndex) in moves)
                {
                    // Ignore moves that undo the parent move
                    if ((toNodeIndex == parentMoveFrom) && (fromNodeIndex == parentMoveTo))
                    {
                        continue;
                    }
                    if (sMoveCount == MAX_NUM_MOVES)
                    {
                        return false;
                    }
                    ref Move childMove = ref sMoves[sMoveCount];
                    childMove.ChildMoveIndexes = null;
                    childMove.MoveIndex = sMoveCount;
                    childMove.ParentMoveIndexes = new List<int>(MAX_STEPS_DEPTH);
                    if (parentMove.ParentMoveIndexes != null)
                    {
                        foreach (var m in parentMove.ParentMoveIndexes)
                        {
                            childMove.ParentMoveIndexes.Add(m);
                        }
                    }
                    if (parentMove.MoveIndex != sRootMoveIndex)
                    {
                        childMove.ParentMoveIndexes.Add(parentMove.MoveIndex);
                    }
                    childMove.NodeFromIndex = fromNodeIndex;
                    childMove.NodeToIndex = toNodeIndex;
                    childMove.DataMoved = sCurrentNodes[fromNodeIndex].Used;
                    childMove.GoalNodeIndex = sGoalNodeIndex;
                    childMove.HoleNodeIndex = sHoleNodeIndex;
                    parentMove.ChildMoveIndexes.Add(sMoveCount);
                    sPendingMoves.Add(sMoveCount);
                    ++sMoveCount;
                    if ((sMoveCount % 30000) == 0)
                    {
                        Console.WriteLine($"sMoveCount {sMoveCount}");
                    }
                }
            }
            return true;
        }

        static void SetupStateForMove(int moveIndex)
        {
            ResetBoard();
            //ValidateCurrentState();
            if (moveIndex == sRootMoveIndex)
            {
                return;
            }
            ref Move move = ref sMoves[moveIndex];
            foreach (var parentMoveIndex in move.ParentMoveIndexes)
            {
                PerformMove(parentMoveIndex);
                //ValidateCurrentState();
            }
        }

        static int FindBestMove()
        {
            var bestScore = int.MaxValue;
            var bestMove = 0;
            for (var i = 0; i < sPendingMoves.Count; ++i)
            {
                var moveIndex = sPendingMoves[i];
                ref var move = ref sMoves[moveIndex];
                (int goalX, int goalY) = GetXYFromIndex(move.GoalNodeIndex);
                (int holeX, int holeY) = GetXYFromIndex(move.HoleNodeIndex);
                int targetX = goalX;
                int targetY = goalY;
                if ((goalX == sWidth) && (goalY == 0))
                {
                    if (holeY > 3)
                    {
                        targetX = 0;
                        targetY = 3;
                    }
                }
                else
                {
                    targetX = goalX;
                    targetY = goalY;
                }
                var from = move.NodeFromIndex;
                (int fromX, int fromY) = GetXYFromIndex(from);
                var goalDistance = Math.Abs(targetX - fromX) + Math.Abs(targetY - fromY);
                if (holeY > 3)
                {
                    goalDistance += 100000;
                }
                goalDistance += 1000 * (goalX + goalY);

                if (goalDistance < bestScore)
                {
                    bestScore = goalDistance;
                    bestMove = i;
                }
            }
            return bestMove;
        }

        public static int ShortestStepsImpl(int startGoalMinSteps)
        {
            ResetBoard();
            //OutputCurrentState();
            //ValidateCurrentState();
            sPreviousBoardStates = new List<int[]>(MAX_NUM_MOVES);

            sMoveCount = 0;
            sMoves = new Move[MAX_NUM_MOVES];
            ref Move rootMove = ref sMoves[sRootMoveIndex];
            rootMove.ChildMoveIndexes = null;
            rootMove.NextChildMove = -1;
            rootMove.MoveIndex = sRootMoveIndex;
            rootMove.ParentMoveIndexes = new List<int>();
            rootMove.NodeFromIndex = -1;
            rootMove.NodeToIndex = -1;
            rootMove.DataMoved = -1;
            ++sMoveCount;
            sPendingMoves.Add(sRootMoveIndex);

            var goalMinSteps = startGoalMinSteps;

            do
            {
                int maxSteps = Math.Min(MAX_STEPS_DEPTH, goalMinSteps);
                var bestMove = FindBestMove();
                var moveIndex = sPendingMoves[bestMove];
                //Console.WriteLine($"Move {moveIndex} MoveCount:{sPendingMoves.Count}");
                sPendingMoves.RemoveAt(bestMove);

                ref Move currentMove = ref sMoves[moveIndex];

                SetupStateForMove(moveIndex);
                PerformMove(moveIndex);

                var currentSteps = currentMove.ParentMoveIndexes.Count + 1;
                if (sGoalNodeIndex == 0)
                {
                    if (currentSteps < goalMinSteps)
                    {
                        Console.WriteLine($"BEST Solution  {goalMinSteps} {currentSteps}");
                        /*
                        foreach (var m in currentMove.ParentMoveIndexes)
                        {
                            Console.Write($"{m} -> ");
                        }
                        Console.WriteLine($"{moveIndex}");
                        */
                    }
                    goalMinSteps = Math.Min(goalMinSteps, currentSteps);
                }
                //OutputCurrentState();
                //OutputBoard();
                //Console.WriteLine($"{bestMove} {sPendingMoves.Count} {GetXYFromIndex(sGoalNodeIndex).x},{GetXYFromIndex(sGoalNodeIndex).y}");
                if (AddCurrentBoardStateAlreadyExist())
                {
                    if (currentSteps + 1 < maxSteps)
                    {
                        // Given current board state find all possible moves
                        AddNewMoves(moveIndex);
                    }
                }
            }
            while (sPendingMoves.Count > 0);

            return goalMinSteps;
        }

        static void ValidateCurrentState()
        {
            for (var i = 0; i < sNodeCount; ++i)
            {
                var node = sCurrentNodes[i];
                (int x, int y) = GetXYFromIndex(i);
                if (node.Used < 0)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Used {node.Used} < 0");
                }
                if (node.Used > node.Size)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Used {node.Used} > Size {node.Size}");
                }
                if (node.Avail < 0)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Avail {node.Avail} < 0");
                }
                if (node.Avail > node.Size)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Avail {node.Avail} > Size {node.Size}");
                }
                if (node.Size < 0)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Size {node.Size} < 0");
                }
                if (node.Used + node.Avail != node.Size)
                {
                    throw new InvalidProgramException($"Invalid node[{i}] X:{x} Y:{y} Used {node.Used} + Avail {node.Avail} != Size {node.Size}");
                }
            }
        }

        static void OutputBoard()
        {
            Console.WriteLine($"-------------------");
            for (var y = 0; y <= sHeight; ++y)
            {
                var line = "";
                for (var x = 0; x <= sWidth; ++x)
                {
                    var nodeIndex = GetIndexFromXY(x, y);
                    if (nodeIndex == sGoalNodeIndex)
                    {
                        line += 'G';
                    }
                    else if (nodeIndex == 0)
                    {
                        line += 'E';
                    }
                    else if (nodeIndex == sHoleNodeIndex)
                    {
                        line += '_';
                    }
                    else
                    {
                        var node = sCurrentNodes[nodeIndex];
                        {
                            var countValidMoves = 0;
                            var countPotentialMoves = 0;
                            var fromUsed = node.Used;
                            var fromAvail = node.Avail;
                            var fromSize = node.Size;
                            for (var n = 0; n < 4; ++n)
                            {
                                var neighbourIndex = node.Neighbours[n];
                                if (neighbourIndex > 0)
                                {
                                    ref Node toNode = ref sCurrentNodes[neighbourIndex];
                                    var toAvail = toNode.Avail;
                                    var toUsed = toNode.Used;
                                    var toSize = toNode.Size;
                                    if (fromUsed <= toAvail)
                                    {
                                        ++countValidMoves;
                                    }
                                    if (fromUsed <= toSize)
                                    {
                                        ++countPotentialMoves;
                                    }
                                }
                            }
                            if ((countValidMoves == 0) && (countPotentialMoves <= 2))
                            {
                                line += '#';
                            }
                            else
                            {
                                line += '.';
                            }
                        }
                    }
                }
                Console.WriteLine($"{line}");
            }
        }

        static void OutputCurrentState()
        {
            Console.WriteLine($"-------------------");
            for (var y = 0; y <= sHeight; ++y)
            {
                var line = "";
                for (var x = 0; x <= sWidth; ++x)
                {
                    var nodeIndex = GetIndexFromXY(x, y);
                    var node = sCurrentNodes[nodeIndex];
                    line += $"{node.Used,3:D}/{node.Avail,3:D} ";
                }
                Console.WriteLine($"{line}");
            }
            Console.WriteLine($"-------------------");
        }

        public static void Run()
        {

            _ = new Program("Day22/input.txt", true);
            _ = new Program("Day22/input.txt", false);
            Console.WriteLine("Day22 : End");
        }
    }
}
