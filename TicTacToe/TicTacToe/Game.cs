using System.Linq;

namespace TicTacToe
{
    /// <summary>
    /// Class containing TicTacToe game state.
    /// </summary>
    class Game
    {
        private const int MaxMoveCount = 9;
        private Player[] players = new Player[] { new Player("dogs", "🐕"), new Player("apples", "🍎") };
        private int moveCount;

        /// <summary>
        /// Player which will move if MakeMove called.
        /// </summary>
        public Player CurrentPlayer { get; private set; }

        /// <summary>
        /// Player who has won the game of null if the game has not ended or in case of draw.
        /// </summary>
        public Player Winner { get; private set; } = null;

        /// <summary>
        /// Returns true in the case of draw, false otherwise. 
        /// </summary>
        public bool Draw { get; private set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Game() => CurrentPlayer = players.First();

        /// <summary>
        /// Makes a move for the current player.
        /// </summary>
        /// <param name="row">Rpw of position to capture.</param>
        /// <param name="column">Column of position to capture.</param>
        public void MakeMove(int row, int column)
        {
            if (Winner != null)
            {
                return;
            }

            CurrentPlayer.MakeMove(row, column);

            if (IsWinner(CurrentPlayer, row, column))
            {
                Winner = CurrentPlayer;
                return;
            }

            ++moveCount;

            if (moveCount == MaxMoveCount)
            {
                Draw = true;
                return;
            }

            CurrentPlayer = players[moveCount % players.Length];
        }

        private bool IsWinner(Player player, int row, int column)
        {
            var threeInRow = true;
            var threeInColumn = true;
            var threeInLeftDiagonal = row == column;
            var threeInRightDiagonal = row + column == 2;

            for (var i = 0; i < 3; ++i)
            {
                threeInRow = threeInRow && player.Captured(row, i);
                threeInColumn = threeInColumn && player.Captured(i, column);
                threeInLeftDiagonal = threeInLeftDiagonal && player.Captured(i, i);
                threeInRightDiagonal = threeInRightDiagonal && player.Captured(i, 2 - i);
            }

            return threeInRow || threeInColumn || threeInLeftDiagonal || threeInRightDiagonal;
        }
    }
}