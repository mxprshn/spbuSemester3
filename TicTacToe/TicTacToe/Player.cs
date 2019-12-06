
namespace TicTacToe
{
    /// <summary>
    /// Class representing a TicTacToe player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Name of the player.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Symbol which is drawn at the poard when the player captures position.
        /// </summary>
        public string Symbol { get; private set; }

        private bool[,] board = { { false, false, false }, { false, false, false },
                { false, false, false } };

        /// <summary>
        /// Constructor.
        /// </summary>
        public Player(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        /// <summary>
        /// Makes a move for the player by capturing position.
        /// </summary>
        /// <param name="row">Row of position to capture.</param>
        /// <param name="column">Column of position to capture.</param>
        public void MakeMove(int row, int column) => board[row, column] = true;

        /// <summary>
        /// Checks whether a position is captured by the player.
        /// </summary>
        /// <param name="row">Row of position to check.</param>
        /// <param name="column">Column of position to check.</param>
        /// <returns>True if the position is captured, false otherwise.</returns>
        public bool Captured(int row, int column) => board[row, column];
    }
}