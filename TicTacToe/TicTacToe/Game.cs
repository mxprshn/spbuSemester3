using System.Linq;

namespace TicTacToe
{
    class Game
    {
        private const int MaxMoveCount = 9; 
        private Player[] players = new Player[] { new Player("crosses", 'X'), new Player("noughts",'O') };
        private int moveCount;
        public Player CurrentPlayer { get; private set; }
        public Player Winner { get; private set; } = null;
        public bool Draw { get; private set; } = false;

        public Game() => CurrentPlayer = players.First();

        public void MakeMove(int row, int column)
        {
            if (Winner != null)
            {
                return;
            }

            if (CurrentPlayer.MakeMove(row, column))
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
    }
}