#include "raylib.h"
#include "Connect4.cpp"

const int CELL_SIZE = 100;
const int SCREEN_WIDTH = 7 * CELL_SIZE;  // 700 px
const int SCREEN_HEIGHT = 6 * CELL_SIZE; // 600 px

void draw_board(const State &state)
{
    ClearBackground(DARKBLUE);

    for (int r = 0; r < 6; r++)
    {
        for (int c = 0; c < 7; c++)
        {
            int center_x = c * CELL_SIZE + CELL_SIZE / 2;
            int center_y = r * CELL_SIZE + CELL_SIZE / 2;
            int radius = CELL_SIZE / 2 - 8;

            Color circle_color = RAYWHITE;
            if (state.board[r][c] == PLAYER)
                circle_color = RED;
            else if (state.board[r][c] == AI)
                circle_color = YELLOW;

            DrawCircle(center_x, center_y, radius, circle_color);
        }
    }
}

int main()
{
    InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Connect Four - Raylib AI");
    SetTargetFPS(60);

    State state = {};
    for (int r = 0; r < 6; r++)
        for (int c = 0; c < 7; c++)
            state.board[r][c] = EMPTY;

    int current_turn = PLAYER;
    bool game_over = false;

    while (!WindowShouldClose())
    {
        if (!game_over && current_turn == PLAYER)
        {
            if (IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
            {
                Vector2 mouse_pos = GetMousePosition();
                int clicked_col = mouse_pos.x / CELL_SIZE;

                int valid_moves[7];
                int num_moves = get_valid_moves(state, valid_moves);

                for (int i = 0; i < num_moves; i++)
                {
                    if (valid_moves[i] == clicked_col)
                    {
                        make_move(state, clicked_col, PLAYER);

                        if (check_win(state) || check_draw(state))
                        {
                            game_over = true;
                        }
                        else
                        {
                            current_turn = AI;
                        }
                        break;
                    }
                }
            }
        }

        BeginDrawing();
        draw_board(state);

        if (game_over)
        {
            DrawRectangle(0, SCREEN_HEIGHT / 2 - 60, SCREEN_WIDTH, 120, Fade(BLACK, 0.7f));

            if (check_win(state))
            {
                const char *win_text = (current_turn == PLAYER) ? "PLAYER WINS!" : "AI WINS!";
                Color text_color = (current_turn == PLAYER) ? RED : WHITE;

                DrawText(win_text, 110, SCREEN_HEIGHT / 2 - 25, 45, text_color);
            }
            else
            {
                DrawText("DRAW!", 260, SCREEN_HEIGHT / 2 - 25, 45, LIGHTGRAY);
            }
        }
        EndDrawing();

        if (!game_over && current_turn == AI)
        {
            int best_col = -1;

            minimax(state, 5, true, best_col);

            make_move(state, best_col, AI);

            if (check_win(state) || check_draw(state))
            {
                game_over = true;
            }
            else
            {
                current_turn = PLAYER;
            }
        }
    }

    CloseWindow();
    return 0;
}