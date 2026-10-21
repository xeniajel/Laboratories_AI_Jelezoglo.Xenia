#include <iostream>
using namespace std;

const int EMPTY = 0;
const int PLAYER = 1;
const int AI = 2;

struct State
{
    int board[6][7];
};

int get_valid_moves(const State &state, int valid_moves[])
{
    int count = 0;

    for (int i = 0; i < 7; i++)
    {
        if (state.board[0][i] == EMPTY)
        {
            valid_moves[count] = i;
            count++;
        }
    }
    return count;
}

void make_move(State &state, int col, int piece)
{
    for (int row = 5; row >= 0; row--)
    {
        if (state.board[row][col] == EMPTY)
        {
            state.board[row][col] = piece;
            break;
        }
    }
}

bool check_win(const State &state)
{
    int d_row[] = {0, 1, 1, -1};
    int d_col[] = {1, 0, 1, 1};

    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            int player = state.board[row][col];
            if (player == EMPTY)
                continue;

            for (int i = 0; i < 4; i++)
            {
                int r = row, c = col;
                int count = 0;

                for (int step = 0; step < 4; step++)
                {
                    if (r >= 0 && r < 6 && c >= 0 && c < 7 && state.board[r][c] == player)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }

                    r += d_row[i];
                    c += d_col[i];
                }
                if (count == 4)
                    return true;
            }
        }
    }
    return false;
}

bool check_draw(const State &state)
{
    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            if (state.board[row][col] == EMPTY)
            {
                return false;
            }
        }
    }
    return true;
}

int score_window(int ai_count, int player_count, int empty_count)
{
    if (ai_count > 0 && player_count > 0)
    {
        return 0;
    }

    static const int weights[5][5] = {
        // 0 игроков  1 игрок   2 игрока   3 игрока   4 игрока
        {0, 0, 0, -80, -100000}, // 0 ИИ
        {10, 0, 0, 0, 0},        // 1 ИИ
        {100, 0, 0, 0, 0},       // 2 ИИ
        {1000, 0, 0, 0, 0},      // 3 ИИ
        {100000, 0, 0, 0, 0}     // 4 ИИ
    };

    return weights[ai_count][player_count];
}

int evaluate_board(const State &state)
{
    int total_score = 0;

    const int d_row[] = {0, 1, 1, -1};
    const int d_col[] = {1, 0, 1, 1};

    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            for (int i = 0; i < 4; i++)
            {
                int end_r = row + 3 * d_row[i];
                int end_c = col + 3 * d_col[i];

                if (end_r >= 0 && end_r < 6 && end_c >= 0 && end_c < 7)
                {
                    int ai_count = 0;
                    int player_count = 0;
                    int empty_count = 0;

                    for (int step = 0; step < 4; step++)
                    {
                        int piece = state.board[row + step * d_row[i]][col + step * d_col[i]];
                        if (piece == AI)
                            ai_count++;
                        else if (piece == PLAYER)
                            player_count++;
                        else if (piece == EMPTY)
                            empty_count++;
                    }

                    total_score += score_window(ai_count, player_count, empty_count);
                }
            }
        }
    }

    return total_score;
}

int minimax(State& state, int depth, bool is_maximizing, int& best_col) {
    
    if (check_win(state)) {
        return is_maximizing ? -100000 : 100000; 
    }
    if (check_draw(state) || depth == 0) {
        return evaluate_board(state);
    }

    int valid_moves[7];
    int num_moves = get_valid_moves(state, valid_moves);

    if (is_maximizing) {
        int max_eval = -1000000;
        for (int i = 0; i < num_moves; i++) {
            int col = valid_moves[i];
            
            State temp_state = state;
            make_move(temp_state, col, AI);

            int dummy_col;
            int eval = minimax(temp_state, depth - 1, false, dummy_col);

            if (eval > max_eval) {
                max_eval = eval;
                best_col = col;
            }
        }
        return max_eval;
    } 
    else {
        int min_eval = 1000000;
        for (int i = 0; i < num_moves; i++) {
            int col = valid_moves[i];
            
            State temp_state = state;
            make_move(temp_state, col, PLAYER);

            int dummy_col;
            int eval = minimax(temp_state, depth - 1, true, dummy_col);

            if (eval < min_eval) {
                min_eval = eval;
                best_col = col;
            }
        }
        return min_eval;
    }
}