
namespace TicTacToe
{
    public class Player
    {
        public string Name { get; private set; }
        public char Symbol { get; private set; }
        private bool[,] board = { {false, false, false }, { false, false, false },
                { false, false, false } };

        private bool IsWinner(int row, int column) => 
                (board[row, 0] && board[row, 1] && board[row, 2]) ||
                (board[0, column] && board[1, column] && board[2, column]) ||
                (row == column && board[0, 0] && board[1, 1] && board[2, 2]) ||
                (row + column == 2 && board[0, 2] && board[1, 1] && board[2, 0]);

        public Player(string name, char symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public bool MakeMove(int row, int column)
        {
            board[row, column] = true;
            return IsWinner(row, column);
        }
    }
}