using System;
using System.Collections.Generic;

public class PlayerMancuna : Player
{
    private enum HashFlag
    {
        Exact,
        LowerBound, // We know the score is AT LEAST this value (Beta Cutoff)
        UpperBound  // We know the score is AT MOST this value (All children failed to raise Alpha)
    }
    private const int MAX_DEPTH = 12;

    // The dictionary now stores a Tuple of (Value, Depth, Flag)
    private Dictionary<int, (double Value, int Depth, HashFlag Flag)> memo = new();

    public PlayerMancuna(string name, int pos) : base(name, pos) { }

    public override int play(IBoard board)
    {
        // memo.Clear();
        int overallBestAction = -1;

        (List<IBoard> children, List<int> actions) = board.children();

        if (actions.Count == 1) return actions[0];
        if (actions.Count == 0) return -1; // Should not happen

        for (int currentDepth = 1; currentDepth <= MAX_DEPTH; currentDepth++)
        {
            int bestActionThisDepth = -1;
            double alpha = double.NegativeInfinity;
            double beta = double.PositiveInfinity;
            double v = double.NegativeInfinity;

            for (int i = 0; i < children.Count; i++)
            {
                double res = MinValue(children[i], alpha, beta, currentDepth - 1, 1);
                if (res > v)
                {
                    v = res;
                    bestActionThisDepth = actions[i];
                }
                alpha = Math.Max(alpha, v);
            }

            overallBestAction = bestActionThisDepth != -1 ? bestActionThisDepth : actions[0];

            // Early Exit: If the AI finds a forced win, stop searching deeper.
            if (v >= 9000)
            {
                break;
            }
        }

        return overallBestAction;
    }

    private double MaxValue(IBoard board, double alpha, double beta, int remainingDepth, int ply = 0)
    {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner, ply);
        if (remainingDepth <= 0) return Evaluate(board, winner, ply);

        int key = (board.hash() << 1) | 0;

        if (memo.TryGetValue(key, out var cached))
        {
            if (cached.Depth >= remainingDepth)
            {
                if (cached.Flag == HashFlag.Exact)
                    return cached.Value;
                if (cached.Flag == HashFlag.LowerBound && cached.Value >= beta)
                    return cached.Value;
                if (cached.Flag == HashFlag.UpperBound && cached.Value <= alpha)
                    return cached.Value;
            }
        }

        double originalAlpha = alpha; // Remember the original alpha to determine the flag later
        double v = double.NegativeInfinity;
        (List<IBoard> children, _) = board.children();

        foreach (var child in children)
        {
            v = Math.Max(v, MinValue(child, alpha, beta, remainingDepth - 1, ply + 1));

            // Beta Cutoff
            if (v >= beta)
            {
                if (v < 9000 && v > -9000)
                {
                    memo[key] = (v, remainingDepth, HashFlag.LowerBound);
                }
                return v;
            }
            alpha = Math.Max(alpha, v);
        }

        // If 'v' never exceeded our original alpha, this node failed to improve our position,
        // meaning 'v' is just an Upper Bound of the true score.
        if (v < 9000 && v > -9000)
        {
            HashFlag flag = (v > originalAlpha) ? HashFlag.Exact : HashFlag.UpperBound;
            memo[key] = (v, remainingDepth, flag);
        }

        return v;
    }

    private double MinValue(IBoard board, double alpha, double beta, int remainingDepth, int ply = 0)
    {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner, ply);
        if (remainingDepth <= 0) return Evaluate(board, winner, ply);

        int key = (board.hash() << 1) | 1;

        if (memo.TryGetValue(key, out var cached))
        {
            if (cached.Depth >= remainingDepth)
            {
                if (cached.Flag == HashFlag.Exact)
                    return cached.Value;
                if (cached.Flag == HashFlag.LowerBound && cached.Value >= beta)
                    return cached.Value;
                if (cached.Flag == HashFlag.UpperBound && cached.Value <= alpha)
                    return cached.Value;
            }
        }

        double originalBeta = beta; // Remember the original beta to determine the flag later
        double v = double.PositiveInfinity;
        (List<IBoard> children, _) = board.children();

        foreach (var child in children)
        {
            v = Math.Min(v, MaxValue(child, alpha, beta, remainingDepth - 1, ply + 1));

            // Alpha Cutoff
            if (v <= alpha)
            {
                if (v < 9000 && v > -9000)
                {
                    memo[key] = (v, remainingDepth, HashFlag.UpperBound);
                }
                return v;
            }
            beta = Math.Min(beta, v);
        }

        // If 'v' never dipped below our original beta, this node failed to improve the opponent's position,
        // meaning 'v' is just a Lower Bound of the true score.
        if (v < 9000 && v > -9000)
        {
            HashFlag flag = (v < originalBeta) ? HashFlag.Exact : HashFlag.LowerBound;
            memo[key] = (v, remainingDepth, flag);
        }

        return v;
    }

    private double Evaluate(IBoard board, int winner, int ply)
    {
        if (winner == (int)GameEnd.Tie) return 0;
        if (winner == _position) return 10000.0 - ply; // Win faster
        if (winner >= 0) return -10000.0 + ply;        // Lose slower

        return (board.score(_position) - board.score(1 - _position)) * 10;
    }
}
