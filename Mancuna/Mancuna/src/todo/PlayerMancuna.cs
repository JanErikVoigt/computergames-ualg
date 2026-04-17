/** @author Gemini CLI
 *  @version 20260414
 *
 *  Minimax AI agent for Mancuna with Alpha-Beta pruning and Transposition Table.
 */
using System;
using System.Collections.Generic;

public class PlayerMancuna : Player {

    private const int MAX_DEPTH = 16; 
    private Dictionary<int, double> memo = new();

    public PlayerMancuna(string name, int pos) : base(name, pos) {  }

    public override int play(IBoard board) { 
        memo.Clear();
        
        int bestAction = -1;
        double alpha = double.NegativeInfinity;
        double beta = double.PositiveInfinity;
        
        (List<IBoard> children, List<int> actions) = board.children();
        
        if (actions.Count == 1) return actions[0];
        if (actions.Count == 0) return -1; // Should not happen

        double v = double.NegativeInfinity;
        for (int i = 0; i < children.Count; i++) {
            double res = MinValue(children[i], alpha, beta, 1);
            if (res > v) {
                v = res;
                bestAction = actions[i];
            }
            alpha = Math.Max(alpha, v);
        }
        
        return bestAction != -1 ? bestAction : actions[0];
    }

    private double MaxValue(IBoard board, double alpha, double beta, int depth) {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner);
        if (depth >= MAX_DEPTH) return Evaluate(board, winner);

        int key = (board.hash() << 1) | 0; // 0 for Max node
        if (memo.TryGetValue(key, out double cached)) return cached;

        double v = double.NegativeInfinity;
        (List<IBoard> children, _) = board.children();
        
        foreach (var child in children) {
            v = Math.Max(v, MinValue(child, alpha, beta, depth + 1));
            if (v >= beta) return v;
            alpha = Math.Max(alpha, v);
        }

        memo[key] = v;
        return v;
    }

    private double MinValue(IBoard board, double alpha, double beta, int depth) {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) return Evaluate(board, winner);
        if (depth >= MAX_DEPTH) return Evaluate(board, winner);

        int key = (board.hash() << 1) | 1; // 1 for Min node
        if (memo.TryGetValue(key, out double cached)) return cached;

        double v = double.PositiveInfinity;
        (List<IBoard> children, _) = board.children();
        
        foreach (var child in children) {
            v = Math.Min(v, MaxValue(child, alpha, beta, depth + 1));
            if (v <= alpha) return v;
            beta = Math.Min(beta, v);
        }

        memo[key] = v;
        return v;
    }

    private double Evaluate(IBoard board, int winner) {
        if (winner == _position) return 1000.0 + board.score(_position);
        if (winner >= 0) return -1000.0 - board.score(1 - _position);
        if (winner == (int)GameEnd.Tie) return 0;

        // Heuristic: pieces on my side minus pieces on opponent's side
        return board.score(_position) - board.score(1 - _position);
    }
}
