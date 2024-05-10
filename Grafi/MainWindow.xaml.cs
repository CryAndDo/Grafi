using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isPlayerX = true;
        private char[,] board;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsTextBox.Text, out int rows) && int.TryParse(ColumnsTextBox.Text, out int columns))
            {
                board = new char[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        board[i, j] = ' ';
                    }
                }
                GameGrid.Children.Clear();
                GameGrid.ColumnDefinitions.Clear();
                GameGrid.RowDefinitions.Clear();

                for (int i = 0; i < rows; i++)
                {
                    GameGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int i = 0; i < columns; i++)
                {
                    GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Button cellButton = new Button
                        {
                            Content = "",
                            Tag = new Tuple<int, int>(i, j),
                            Margin = new Thickness(1),
                            Padding = new Thickness(5),
                            Background = Brushes.LightGray
                        };

                        cellButton.Click += CellButton_Click;
                        Grid.SetRow(cellButton, i);
                        Grid.SetColumn(cellButton, j);
                        GameGrid.Children.Add(cellButton);
                    }
                }

                ResultLabel.Content = CheckPreWinCondition(board,rows, columns);
            }
        }

        

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            Tuple<int, int> position = (Tuple<int, int>)clickedButton.Tag;
            int row = position.Item1;
            int column = position.Item2;

            if (isPlayerX)
            {
                clickedButton.Content = "X";
                board[row, column] = 'X'; // Обновление массива board
            }
            else
            {
                clickedButton.Content = "O";
                board[row, column] = 'O'; // Обновление массива board
            }

            isPlayerX = !isPlayerX;
            clickedButton.IsEnabled = false; // Запретить повторный клик по клетке

            string result = CheckPreWinCondition(board, board.GetLength(0), board.GetLength(1));
            if (result == "Pre-win condition detected")
            {
                ResultLabel.Content = result;
            } // Проверка на предвыигрышное состояние
            if (CheckWinCondition(board, row, column))
            {
                ResultLabel.Content = $"{board[row, column]} wins!";
                DisableAllButtons(); // Отключите все кнопки, чтобы завершить игру
            }
        }
        private bool CheckWinCondition(char[,] board, int row, int column)
        {
            char symbol = board[row, column];
            if (symbol == ' ') return false; // Если ячейка пустая, победы нет

            // Проверка горизонтальных, вертикальных и диагональных линий
            return CheckLine(board, row, column, 0, 1, symbol) || // Горизонталь
                   CheckLine(board, row, column, 1, 0, symbol) || // Вертикаль
                   CheckLine(board, row, column, 1, 1, symbol) || // Главная диагональ
                   CheckLine(board, row, column, 1, -1, symbol);  // Побочная диагональ
        }
        private bool CheckLine(char[,] board, int row, int column, int dRow, int dColumn, char symbol)
        {
            int count = 0;
            int i = row;
            int j = column;

            // Проверяем в одном направлении
            while (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1) && board[i, j] == symbol)
            {
                count++;
                i += dRow;
                j += dColumn;
            }

            i = row - dRow;
            j = column - dColumn;

            // Проверяем в противоположном направлении
            while (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1) && board[i, j] == symbol)
            {
                count++;
                i -= dRow;
                j -= dColumn;
            }

            return count >= 5; // Возвращает true, если найдено 5 или более символов подряд
        }

        private void DisableAllButtons()
        {
            foreach (var child in GameGrid.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
        }
        private string CheckPreWinCondition(char[,] board, int rows, int columns)
        {
            
            // Проверка горизонтальных линий на предвыигрышное состояние
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j <= columns - 4; j++)
                {
                    if (IsPreWinLine(board[i, j], board[i, j + 1], board[i, j + 2], board[i, j + 3]))
                        return "Pre-win condition detected";
                }
            }

            // Проверка вертикальных линий на предвыигрышное состояние
            for (int i = 0; i <= rows - 4; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (IsPreWinLine(board[i, j], board[i + 1, j], board[i + 2, j], board[i + 3, j]))
                        return "Pre-win condition detected";
                }
            }

            // Проверка диагоналей на предвыигрышное состояние
            for (int i = 0; i <= rows - 4; i++)
            {
                for (int j = 0; j <= columns - 4; j++)
                {
                    if (IsPreWinLine(board[i, j], board[i + 1, j + 1], board[i + 2, j + 2], board[i + 3, j + 3]))
                        return "Pre-win condition detected";

                    if (j >= 3 && IsPreWinLine(board[i, j], board[i + 1, j - 1], board[i + 2, j - 2], board[i + 3, j - 3]))
                        return "Pre-win condition detected";
                }
            }

            return "No pre-win condition.";
        }

        // Вспомогательный метод для проверки четырёх подряд идущих символов с возможностью добавления пятого
        private bool IsPreWinLine(char a, char b, char c, char d)
        {
            // Проверяем, что все четыре символа одинаковы
            return (a == b && b == c && c == d) && (a == 'X' || a == 'O');
        }
    }
}