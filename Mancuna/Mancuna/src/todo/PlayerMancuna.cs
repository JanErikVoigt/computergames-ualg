using System;
using System.Collections.Generic;

public class PlayerMancuna : Player
{
    private const int MAX_DEPTH = 32;

    // UPGRADE 1: The dictionary now stores a Tuple of (Value, Depth)
    // If a hash collision occurs, we at least know how "deep" the cached calculation was.
    private Dictionary<int, (double Value, int Depth)> memo = new();

    public PlayerMancuna(string name, int pos) : base(name, pos) { }

    public override int play(IBoard board)
    {
        memo.Clear();
        int overallBestAction = -1;

        (List<IBoard> children, List<int> actions) = board.children();

        if (actions.Count == 1) return actions[0];
        if (actions.Count == 0) return -1; // Should not happen

        // UPGRADE 2: Iterative Deepening Loop
        for (int currentDepth = 1; currentDepth <= MAX_DEPTH; currentDepth++)
        {
            int bestActionThisDepth = -1;
            double alpha = double.NegativeInfinity;
            double beta = double.PositiveInfinity;
            double v = double.NegativeInfinity;

            // Move Ordering: Put the best action from the previous depth iteration FIRST
            OrderMoves(ref children, ref actions, overallBestAction);

            for (int i = 0; i < children.Count; i++)
            {
                // Notice we pass currentDepth - 1 because we already took 1 step by generating children
                double res = MinValue(children[i], alpha, beta, currentDepth - 1);
                if (res > v)
                {
                    v = res;
                    bestActionThisDepth = actions[i];
                }
                alpha = Math.Max(alpha, v);
            }

            overallBestAction = bestActionThisDepth != -1 ? bestActionThisDepth : actions[0];

            // Early Exit: If the AI finds a forced win, stop searching deeper.
            // This prevents it from overthinking and tripping on a hash collision at depth 32.
            if (v >= 9000 || v <= -9000)
            {
                break;
            }
        }

        return overallBestAction;
    }

    // Helper method to swap the known best move to index 0
    private void OrderMoves(ref List<IBoard> children, ref List<int> actions, int bestAction)
    {
        if (bestAction == -1) return;

        int index = actions.IndexOf(bestAction);
        if (index > 0)
        {
            int tempAction = actions[0];
            actions[0] = actions[index];
            actions[index] = tempAction;

            IBoard tempBoard = children[0];
            children[0] = children[index];
            children[index] = tempBoard;
        }
    }

    private double MaxValue(IBoard board, double alpha, double beta, int remainingDepth)
    {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner, remainingDepth);
        if (remainingDepth <= 0) return Evaluate(board, winner, remainingDepth);

        int key = (board.hash() << 1) | 0;

        // UPGRADE 3: Depth-Stamped Cache Check
        if (memo.TryGetValue(key, out var cached))
        {
            // Only trust the cache if it was searched to at least the depth we need right now.
            // This protects you from using shallow hash collisions to override deep thinking.
            if (cached.Depth >= remainingDepth) return cached.Value;
        }

        double v = double.NegativeInfinity;
        (List<IBoard> children, _) = board.children();

        foreach (var child in children)
        {
            v = Math.Max(v, MinValue(child, alpha, beta, remainingDepth - 1));
            if (v >= beta)
            {
                memo[key] = (v, remainingDepth);
                return v;
            }
            alpha = Math.Max(alpha, v);
        }

        memo[key] = (v, remainingDepth);
        return v;
    }

    private double MinValue(IBoard board, double alpha, double beta, int remainingDepth)
    {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner, remainingDepth);
        if (remainingDepth <= 0) return Evaluate(board, winner, remainingDepth);

        int key = (board.hash() << 1) | 1;

        if (memo.TryGetValue(key, out var cached))
        {
            if (cached.Depth >= remainingDepth) return cached.Value;
        }

        double v = double.PositiveInfinity;
        (List<IBoard> children, _) = board.children();

        foreach (var child in children)
        {
            v = Math.Min(v, MaxValue(child, alpha, beta, remainingDepth - 1));
            if (v <= alpha)
            {
                memo[key] = (v, remainingDepth);
                return v;
            }
            beta = Math.Min(beta, v);
        }

        memo[key] = (v, remainingDepth);
        return v;
    }

    private double Evaluate(IBoard board, int winner, int remainingDepth)
    {
        // UPGRADE 4: Horizon Effect Fix
        // By adding 'remainingDepth' to a win, the AI prefers to win in 2 moves (+10030) 
        // rather than winning in 30 moves (+10002).
        if (winner == _position) return 10000.0 + remainingDepth;
        if (winner >= 0) return -10000.0 - remainingDepth;
        if (winner == (int)GameEnd.Tie) return 0;

        // Keep your tweaked heuristic here
        return (board.score(_position) - board.score(1 - _position)) * 10;
    }
}
