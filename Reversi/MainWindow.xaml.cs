using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Reversi
{
    public partial class MainWindow : Window
    {
        private Button[,] board;
        private ReversiEngineAI engine = new ReversiEngineAI(1);
        private SolidColorBrush[] colors = { Brushes.Beige, Brushes.Green, Brushes.Brown };
        private string[] playerNames = {"", "green", "brown" };
        private bool playingAgainstComputer = true;
        private DispatcherTimer timer;

        private struct FieldCoordinates
        {
            public int X, Y;
        }

        public MainWindow()
        {
            InitializeComponent();
            
            boardGrid.ColumnDefinitions.Clear();
            boardGrid.RowDefinitions.Clear();
            boardGrid.Children.Clear();
        }

        private FieldCoordinates? DetermineBestMove()
        {
            if (!boardGrid.IsEnabled) return null;

            if(engine.EmptyFieldCount == 0)
            {
                MessageBox.Show("No empty fields", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            try
            {
                int x, y;
                engine.SuggestBestMove(out x, out y);
                return new FieldCoordinates() { X = x, Y = y };
            }
            catch
            {
                MessageBox.Show("Player cannot make a move", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void HighlightBestMove()
        {
            FieldCoordinates? coords = DetermineBestMove();
            if(coords.HasValue)
            {
                SolidColorBrush hintColor = 
                    colors[engine.CurrentPlayerNumber]
                    .Lerp(colors[0]);
                board[coords.Value.X, coords.Value.Y].Background
                    = hintColor;
            }
        }

        public void ExecuteBestMove()
        {
            FieldCoordinates? coords = DetermineBestMove();
            if (coords.HasValue)
            {
                Button button = board[coords.Value.X, coords.Value.Y];
                BoardFieldClicked(button, null);
            }
        }

        private static string GetFieldSymbol(int x, int y)
        {
            if (x > 25 || y > 25) 
                return "(" + x.ToString() + "," + y.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[x] + (y + 1).ToString();
        }

        private void BoardFieldClicked(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            FieldCoordinates coordinates = (FieldCoordinates)clickedButton.Tag;
            int clickedX = coordinates.X;
            int clickedY = coordinates.Y;
            int playerNumber = engine.CurrentPlayerNumber;
            if (engine.PlaceStone(clickedX, clickedY) > 0)
            {
                SynchronizeBoardContent();
                switch (playerNumber)
                {
                    case 1:
                        GreenMovesList.Items.Add(GetFieldSymbol(clickedX, clickedY));
                        break;
                    case 2:
                        BrownMovesList.Items.Add(GetFieldSymbol(clickedX, clickedY));
                        break;
                }
            };

            ReversiEngine.BoardSituation boardSituation = 
                engine.CheckBoardSituation();

            bool gameOver = false;
            switch (boardSituation)
            {
                case ReversiEngine.BoardSituation.AllFieldsOccupied:
                    gameOver = true;
                    break;
                case ReversiEngine.BoardSituation.CurrentPlayerCannotMove:
                    MessageBox.Show("Player " + playerNames[engine.CurrentPlayerNumber] 
                        + " cannot make a move");
                    engine.Pass();
                    SynchronizeBoardContent();
                    break;
                case ReversiEngine.BoardSituation.BothPlayersCannotMove:
                    MessageBox.Show("Neither player can make a move");
                    gameOver = true;
                    break;
            }

            if (gameOver)
            {
                int winnerNumber = engine.LeadingPlayerNumber;
                if (winnerNumber != 0)
                    MessageBox.Show("Player " + playerNames[winnerNumber] + " wins!",
                        Title, MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Draw!", Title,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                 if (MessageBox.Show("Start a new game?", "Reversi", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                 {
                    PrepareNewGame(1, engine.BoardWidth, engine.BoardHeight);
                 }
                 else
                 {
                    boardGrid.IsEnabled = false;
                    playerColorButton.IsEnabled = false;
                 }
            }
            else
            {
                if (playingAgainstComputer && engine.CurrentPlayerNumber == 2)
                {
                    if(timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                        timer.Tick += (_sender, _e) =>
                        { 
                            timer.IsEnabled = false; ExecuteBestMove(); 
                        };
                    }
                    timer.Start();
                }
            }
        }

        private void PrepareNewGame(
            int startingPlayerNumber,
            int boardWidth = 8,
            int boardHeight = 8)
        {
            boardGrid.Children.Clear();
            boardGrid.ColumnDefinitions.Clear();
            boardGrid.RowDefinitions.Clear();

            for (int i = 0; i < boardWidth; i++)
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < boardHeight; i++)
                boardGrid.RowDefinitions.Add(new RowDefinition());

            board = new Button[boardWidth, boardHeight];
            for (int i = 0; i < boardWidth; i++)
                for (int j = 0; j < boardHeight; j++)
                {
                    Button button = new Button();
                    boardGrid.Children.Add(button);
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    button.Tag = new FieldCoordinates { X = i, Y = j };
                    button.Click += new RoutedEventHandler(BoardFieldClicked);
                    board[i, j] = button;
                }

            engine = new ReversiEngineAI(startingPlayerNumber,
                boardWidth, boardHeight);

            GreenMovesList.Items.Clear();
            BrownMovesList.Items.Clear();
            boardGrid.IsEnabled = true;
            playerColorButton.IsEnabled = true;
            SynchronizeBoardContent();
        }

        private void SynchronizeBoardContent()
        {
            for(int i = 0; i < engine.BoardWidth; i++)
            {
                for(int j = 0; j < engine.BoardHeight; j++)
                {
                    board[i, j].Content = engine.GetFieldState(i, j).ToString();
                    board[i, j].Background = colors[engine.GetFieldState(i, j)];
                }
            }
            GreenFieldCount.Text = engine.Player1FieldCount.ToString();
            BrownFieldCount.Text = engine.Player2FieldCount.ToString();
            playerColorButton.Background = colors[engine.CurrentPlayerNumber];
        }
        #region
        private void MenuItem_NewGameSinglePlayerComputerStarts_Click(object sender, RoutedEventArgs e)
        {
            NewGameDialog dialog = new NewGameDialog();
            if (dialog.ShowDialog() == true)
            {
                playingAgainstComputer = true;
                Title = "Reversi - 1 player";
                PrepareNewGame(2, dialog.BoardWidth, dialog.BoardHeight);
                if (engine.EmptyFieldCount > 0)
                {
                    ExecuteBestMove();
                }
            }
        }

        private void MenuItem_NewGameSinglePlayerYouStart_Click(object sender, RoutedEventArgs e)
        {
            NewGameDialog dialog = new NewGameDialog();
            if (dialog.ShowDialog() == true)
            {
                playingAgainstComputer = true;
                Title = "Reversi - 1 player";
                PrepareNewGame(1, dialog.BoardWidth, dialog.BoardHeight);
            }
        }

        private void MenuItem_NewGameTwoPlayers_Click(object sender, RoutedEventArgs e)
        {
            NewGameDialog dialog = new NewGameDialog();
            if (dialog.ShowDialog() == true)
            {
                Title = "Reversi - 2 players";
                playingAgainstComputer = false;
                PrepareNewGame(1, dialog.BoardWidth, dialog.BoardHeight);
            }
        }

        private void MenuItem_ComputerMove_Click(object sender, RoutedEventArgs e)
        {
            ExecuteBestMove();
        }

        private void MenuItem_MoveHint_Click(object sender, RoutedEventArgs e)
        {
            HighlightBestMove();
        }

        private void MenuItem_GameRules_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "In Reversi, players take turns occupying fields on the board, capturing all opponent's pieces " +
                    "that lie between the newly placed piece and other pieces of the player making the move." +
                " The goal is to capture more fields than your opponent.\n" +
                "\n" +
                "A player can only place a piece on a field that allows capturing at least one opponent's piece." +
                " If no such field exists, the player must pass their turn.\n" + 
                "\n" +
                "The game ends when all fields are occupied or when neither player can make a move.\n",
                        "Reversi - Game Rules");
        }

        private void MenuItem_ComputerStrategy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "The computer follows these priorities (from highest):\n" +
                    "1. Place a piece in a corner.\n" +
                    "2. Avoid placing a piece next to a corner.\n" +
                    "3. Place a piece on the edge of the board.\n" +
                    "4. Avoid placing a piece in a row or column one field away from the edge.\n" +
                    "5. Choose the field that captures the most opponent's pieces.\n",
                        "Reversi - Computer Strategy"
                    );
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This program was created at WSB Merito University in Poznan," +
                " completed by student ID 140518 - Mikolaj Kocik", 
                Title, MessageBoxButton.OK);
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        private void PlayerColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) ExecuteBestMove();
            else HighlightBestMove();
        }
    }
}
