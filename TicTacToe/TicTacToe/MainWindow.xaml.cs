using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game = new Game();

        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void RestartGame()
        {
            game = new Game();

            foreach (var button in ButtonGrid.Children.OfType<Button>())
            {
                button.Content = null;
                button.IsEnabled = true;
            }
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = game.CurrentPlayer.Symbol;
            game.MakeMove(Grid.GetRow(button), Grid.GetColumn(button));
            button.IsEnabled = false;

            if (game.Winner != null)
            {
                var result = MessageBox.Show($"And the winner is... {game.Winner.Name}!  Start a new game?", "Congratulations :)", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    RestartGame();
                }
                else
                {
                    IsEnabled = false;
                }
            }

            if (game.Draw)
            {
                var result = MessageBox.Show("It is draw! Start a new game?", ":\\", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    RestartGame();
                }
            }
        }
    }
}