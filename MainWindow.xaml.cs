using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Xml;
using System.ComponentModel;
using System.Xml.Linq;

public enum FigureType
{
    Empty,
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn
}

public enum FigureColor
{
    White,
    Black,
    None
}

public class Figure
{
    public FigureType Type { get; set; }
    public FigureColor Color { get; set; }

    public Figure(FigureType type = FigureType.Empty, FigureColor color = FigureColor.None)
    {
        Type = type;
        Color = color;
    }

    public Figure(Figure other)
    {
        Type = other.Type;
        Color = other.Color;
    }

    public bool isEmpty()
    {
        return Type == FigureType.Empty;
    }

    public int value() 
    {
        if (Type == FigureType.King)
            return 1000;
        if (Type == FigureType.Queen)
            return 90;
        if (Type == FigureType.Bishop)
            return 33;
        if (Type == FigureType.Knight)
            return 30;
        if (Type == FigureType.Rook)
            return 50;
        if (Type == FigureType.Pawn)
            return 10;
        return 0;
    }

    public override string ToString()
    {
        return $"Type: {Type}, Color: {Color}";
    }
}

public class ChessGame
{
    private Figure[,] board;
    private int currentMove;
    private List<(int, int, int, int)> moves;

    public ChessGame()
    {
        board = new Figure[8, 8];
        moves = new List<(int, int, int, int)>();
        currentMove = 0;
        InitializeBoard();
    }
    public ChessGame(ChessGame other)
    {
        board = new Figure[8, 8];
        moves = new List<(int, int, int, int)>();
        currentMove = other.currentMove;

        for(int i = 0; i < currentMove; i++)
            moves.Add((other.moves[i].Item1, other.moves[i].Item2, other.moves[i].Item3, other.moves[i].Item4));

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                board[i, j] = new Figure(other.board[i, j]);
    }




    public Figure getFigure(int y, int x)
    {
        return board[y, x];
    }

    private void InitializeBoard()
    {
        // Initialize the board with pieces in the starting position
        // White pieces
        board[0, 0] = new Figure(FigureType.Rook, FigureColor.White);
        board[0, 1] = new Figure(FigureType.Knight, FigureColor.White);
        board[0, 2] = new Figure(FigureType.Bishop, FigureColor.White);
        board[0, 3] = new Figure(FigureType.Queen, FigureColor.White);
        board[0, 4] = new Figure(FigureType.King, FigureColor.White);
        board[0, 5] = new Figure(FigureType.Bishop, FigureColor.White);
        board[0, 6] = new Figure(FigureType.Knight, FigureColor.White);
        board[0, 7] = new Figure(FigureType.Rook, FigureColor.White);

        for (int i = 0; i < 8; i++)
        {
            board[1, i] = new Figure(FigureType.Pawn, FigureColor.White);
        }

        // Black pieces
        board[7, 0] = new Figure(FigureType.Rook, FigureColor.Black);
        board[7, 1] = new Figure(FigureType.Knight, FigureColor.Black);
        board[7, 2] = new Figure(FigureType.Bishop, FigureColor.Black);
        board[7, 3] = new Figure(FigureType.Queen, FigureColor.Black);
        board[7, 4] = new Figure(FigureType.King, FigureColor.Black);
        board[7, 5] = new Figure(FigureType.Bishop, FigureColor.Black);
        board[7, 6] = new Figure(FigureType.Knight, FigureColor.Black);
        board[7, 7] = new Figure(FigureType.Rook, FigureColor.Black);

        for (int i = 0; i < 8; i++)
        {
            board[6, i] = new Figure(FigureType.Pawn, FigureColor.Black);
        }

        // Empty spaces
        for (int i = 2; i < 6; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = new Figure();
            }
        }
    }

    public bool IsValidMove(int y1, int x1, int y2, int x2)
    {
        // Check if the destination is within the board boundaries
        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            return false;
        }

        // Check if there is a figure at the starting position
        Figure movingFigure = board[y1, x1];
        if (movingFigure.isEmpty())
        {
            return false;
        }

        // Check if there is no figure of the same color at the destination
        Figure destinationFigure = board[y2, x2];
        if (!destinationFigure.isEmpty() && destinationFigure.Color == movingFigure.Color)
        {
            return false;
        }

        // Check intervening pieces (except for Knight)
        if (movingFigure.Type != FigureType.Knight)
        {
            // Check if there are any intervening pieces
            if (!NoInterveningPieces(y1, x1, y2, x2))
            {
                return false;
            }
        }

        // Implement different logic for each type of figure
        switch (movingFigure.Type)
        {
            case FigureType.King:
                // Implement logic for King's movement
                // For now, let's assume King can move one square in any direction
                return Math.Abs(y2 - y1) <= 1 && Math.Abs(x2 - x1) <= 1;

            case FigureType.Queen:
                // Implement logic for Queen's movement
                // For now, let's assume Queen can move like a Rook or a Bishop
                return IsValidRookMove(y1, x1, y2, x2) || IsValidBishopMove(y1, x1, y2, x2);

            case FigureType.Rook:
                // Implement logic for Rook's movement
                return IsValidRookMove(y1, x1, y2, x2);

            case FigureType.Bishop:
                // Implement logic for Bishop's movement
                return IsValidBishopMove(y1, x1, y2, x2);

            case FigureType.Knight:
                // Implement logic for Knight's movement
                // For now, let's assume Knight can move in an L-shape
                int deltaY = Math.Abs(y2 - y1);
                int deltaX = Math.Abs(x2 - x1);
                return (deltaY == 2 && deltaX == 1) || (deltaY == 1 && deltaX == 2);

            case FigureType.Pawn:
                // Implement logic for Pawn's movement
                // For now, let's assume Pawn can only move forward one square
                // and can capture diagonally one square forward
                if (movingFigure.Color == FigureColor.White)
                {
                    // White pawn moves upwards
                    if ((y2 == y1 + 1 || y1 == 1 && y2 == 3) && x2 == x1 && destinationFigure.isEmpty())
                    {
                        return true;
                    }
                    // White pawn captures diagonally
                    if (y2 == y1 + 1 && Math.Abs(x2 - x1) == 1 && !destinationFigure.isEmpty() && destinationFigure.Color != FigureColor.White)
                    {
                        return true;
                    }
                }
                else
                {
                    // Black pawn moves downwards
                    if ((y2 == y1 - 1 || y1 == 6 && y2 == 4) && x2 == x1 && destinationFigure.isEmpty())
                    {
                        return true;
                    }
                    // Black pawn captures diagonally
                    if (y2 == y1 - 1 && Math.Abs(x2 - x1) == 1 && !destinationFigure.isEmpty() && destinationFigure.Color != FigureColor.Black)
                    {
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }

    private bool IsValidRookMove(int y1, int x1, int y2, int x2)
    {
        // Rook can move horizontally or vertically
        return y1 == y2 || x1 == x2;
    }

    private bool IsValidBishopMove(int y1, int x1, int y2, int x2)
    {
        // Bishop can move diagonally
        return Math.Abs(y2 - y1) == Math.Abs(x2 - x1);
    }

    private bool NoInterveningPieces(int y1, int x1, int y2, int x2)
    {
        // Check if there are any intervening pieces between (y1, x1) and (y2, x2)
        int deltaY = Math.Sign(y2 - y1);
        int deltaX = Math.Sign(x2 - x1);

        if (deltaY != 0 && deltaX != 0 && Math.Abs(y2 - y1) != Math.Abs(x2 - x1))
            return false;

        int currentY = y1 + deltaY;
        int currentX = x1 + deltaX;

        while (currentY != y2 || currentX != x2)
        {
            if (!board[currentY, currentX].isEmpty())
            {
                return false; // There is an intervening piece
            }
            currentY += deltaY;
            currentX += deltaX;
        }

        return true; // No intervening pieces
    }

    public List<(int, int)> GetPossibleMoves(int y, int x)
    {
        // List to store possible moves
        List<(int, int)> possibleMoves = new List<(int, int)>();

        // Get the figure at the specified position
        Figure figure = board[y, x];
        if (figure.isEmpty())
        {
            return possibleMoves;
        }

        // Iterate over all positions of the grid
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                // Check if the move is valid
                if (IsValidMove(y, x, i, j))
                {
                    possibleMoves.Add((i, j));
                }
            }
        }

        return possibleMoves;
    }

    public int getFullValue()
    {
        int sum = 0;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (board[i, j].Color == FigureColor.White)
                    sum += board[i, j].value();
                else
                    sum -= board[i, j].value();
        return sum;
    }



    public void makeMove(int y1, int x1, int y2, int x2)
    {
        if (!IsValidMove(y1, x1, y2, x2))
        {
            Console.WriteLine("Invalid move!");
            return;
        }

        // Perform the move
        board[y2, x2] = board[y1, x1];
        board[y1, x1] = new Figure();

        // Increment the move counter
        currentMove++;
        moves.Add((y1, x1, y2, x2));

        Console.WriteLine("Move performed successfully.");
    }

    public FigureColor getCurrentColor()
    {
        if(currentMove % 2 == 0)
            return FigureColor.White;
        return FigureColor.Black;
    }

    public bool isChecked(FigureColor color = FigureColor.None)
    {

        for (int y1 = 0; y1 < 8; y1++)
            for (int x1 = 0; x1 < 8; x1++)
            {
                if (board[y1, x1].isEmpty())
                    continue;

                if(board[y1, x1].Color == color)
                    continue;
                
                for (int y2 = 0; y2 < 8; y2++)
                    for (int x2 = 0; x2 < 8; x2++)
                    {
                        if (board[y2, x2].Type != FigureType.King)
                            continue;

                        if (board[y2, x2].Color == board[y1, x1].Color)
                            continue;

                        if (IsValidMove(y1, x1, y2, x2))
                            return true;
                    }
            }
        return false;
    }

    public bool isBadMove(int y1, int x1 , int y2, int x2)
    {
        if(!IsValidMove(y1, x1, y2, x2))
            return false;

        ChessGame new_chess = new ChessGame(this);
        new_chess.makeMove(y1, x1, y2, x2);

        if(new_chess.isChecked(getCurrentColor()))
            return true;
        return false;
    }

    public (int, int) nextMove()
    {
        int ky = 0, kx = 0;

        List<(int, int, int, int)> moves = new List<(int, int, int, int)>();
        for (int y1 = 0; y1 < 8; y1++)
            for (int x1 = 0; x1 < 8; x1++)
            {
                if (board[y1, x1].isEmpty())
                    continue;

                if (board[y1, x1].Type == FigureType.King)
                {
                    if(board[y1, x1].Color == getCurrentColor())
                    {
                        ky = y1;
                        kx = x1;
                    }    
                    //continue;
                }

                if (board[y1, x1].Color != getCurrentColor())
                    continue;

                for (int y2 = 0; y2 < 8; y2++)
                    for (int x2 = 0; x2 < 8; x2++)
                    {
                        if (IsValidMove(y1, x1, y2, x2))
                        {
                            moves.Add((y1, x1, y2, x2));
                        }
                    }
            }

        if (moves.Count == 0)
            return (0, 0);

        moves.Sort(((int, int, int, int) a, (int, int, int, int) b) =>
        {
            int v1 = board[a.Item3, a.Item4].value();
            int d1 = Math.Abs(ky - a.Item3) + Math.Abs(kx - a.Item4);
            bool bm1 = isBadMove(a.Item1, a.Item2, a.Item3, a.Item4);

            if (Math.Abs(ky - a.Item1) + Math.Abs(kx - a.Item2) == 0)
                d1 = 100;

            int v2 = board[b.Item3, b.Item4].value();
            int d2 = Math.Abs(ky - b.Item3) + Math.Abs(kx - b.Item4);
            bool bm2 = isBadMove(b.Item1, b.Item2, b.Item3, b.Item4);


            if (Math.Abs(ky - b.Item1) + Math.Abs(kx - b.Item2) == 0)
                d2 = 100;

            if (bm1 != bm2)
                return bm1.CompareTo(bm2);

            if (v1 != v2)
                return v2.CompareTo(v1);

            return d1.CompareTo(d2);

        });

        int distanceFromKing = Math.Abs(ky - moves[0].Item3) + Math.Abs(kx - moves[0].Item4);

        makeMove(moves[0].Item1, moves[0].Item2, moves[0].Item3, moves[0].Item4);

        return (getFullValue(), distanceFromKing);
    }

    public int getMoveNumber()
    {
        return currentMove;
    }

    public (int, int, int, int) getLastMove() 
    {
        if (moves.Count == 0)
            return (-1, -1, -1, -1);
        return (moves[currentMove - 1].Item1, moves[currentMove - 1].Item2, moves[currentMove - 1].Item3, moves[currentMove - 1].Item4);
    }
}


namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessGame chess;
        string text;

        int squareSize = 60;
        int lastY = -1, lastX = -1;

        private void DrawChessPieces(string text = "")
        {
            // Create a Canvas to hold the chessboard and chess pieces
            Canvas canvas = new Canvas();
            int chessboardWidth = 8 * squareSize;
            int chessboardHeight = 8 * squareSize;

            // Load the chessboard image
            Image chessboardImage = new Image();
            chessboardImage.Width = chessboardWidth;
            chessboardImage.Height = chessboardHeight;
            chessboardImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/empty-board.png"));
            canvas.Children.Add(chessboardImage);



            // Draw a half-transparent red square at the specified position if lastY and lastX are not -1
            int p_y, p_x;

            (int, int, int, int) lastMove = chess.getLastMove();
            p_y = lastMove.Item3;
            p_x = lastMove.Item4;

            if (p_y != -1 && p_x != -1)
            {
                Rectangle highlightRect = new Rectangle();
                highlightRect.Width = squareSize + 2;
                highlightRect.Height = squareSize + 2;
                highlightRect.Fill = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)); // Half-transparent red color
                Canvas.SetLeft(highlightRect, p_x * squareSize);
                Canvas.SetTop(highlightRect, (7 - p_y) * squareSize); // Invert Y-axis to match chessboard orientation
                canvas.Children.Add(highlightRect);
            }


            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Get the figure type and color at the current position
                    Figure currentFigure = chess.getFigure(row, col);

                    // Calculate the position of the current chess square
                    double left = col * squareSize;
                    double top = (7 - row) * squareSize;

                    // Create a control representing the chess piece
                    Image chessPiece = new Image();

                    string color = "d";
                    if (currentFigure.Color == FigureColor.White)
                        color = "l";

                    string prefix = currentFigure.Type.ToString().ToLower().Substring(0, 1);
                    if (currentFigure.Type == FigureType.Knight)
                        prefix = "n";

                    // Set the source of the chess piece image based on the figure type and color
                    if (!currentFigure.isEmpty())
                    {
                        string fileName = "Chess_" + prefix + color + "t60.png";
                        string imagePath = "pack://application:,,,/Resources/" + fileName;
                        chessPiece.Source = new BitmapImage(new Uri(imagePath));
                    }

                    // Set the size and position of the chess piece control
                    chessPiece.Width = squareSize;
                    chessPiece.Height = squareSize;
                    Canvas.SetLeft(chessPiece, left);
                    Canvas.SetTop(chessPiece, top);

                    // Add the chess piece control to the Canvas
                    canvas.Children.Add(chessPiece);
                }
            }

            // Create and position the TextBlock

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text; // Initial text
            textBlock.FontSize = 16; // Adjust font size as needed
            textBlock.Foreground = Brushes.Black; // Text color
            Canvas.SetLeft(textBlock, chessboardWidth + 20); // Adjust horizontal position as needed
            Canvas.SetTop(textBlock, 20); // Adjust vertical position as needed

            // Add the TextBlock to the Canvas
            canvas.Children.Add(textBlock);


            // Attach an event handler to track mouse clicks on the Canvas
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;

            this.Content = canvas;
        }

        private async void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the mouse position relative to the Canvas
            Point mousePosition = e.GetPosition((IInputElement)sender);

            int y = (int)mousePosition.Y / squareSize;
            y = 7 - y;
            int x = (int)mousePosition.X / squareSize;

            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                if(lastX != -1 && lastY != -1)
                {
                    if(chess.IsValidMove(lastY, lastX, y, x))
                    {
                        chess.makeMove(lastY, lastX, y, x);
                        DrawChessPieces();
                        await Task.Delay(TimeSpan.FromSeconds(0.3));
                        lastX = -1;
                        lastY = -1;

                        bool isChecked = chess.isChecked();

                        text = "Is checked = " + isChecked + " | Move Number = " + chess.getMoveNumber() + " | Value = " + chess.getFullValue() + "\n" + text;

                        (int, int) heuristic = chess.nextMove();
                        isChecked = chess.isChecked();
                        text = "Is checked = " + isChecked + " | Move Number = " + chess.getMoveNumber() +  " | Value = " + heuristic.Item1 + " | Distance From King = " + heuristic.Item2 + "\n" + text;


                        DrawChessPieces(text);

                        return;
                    }
                }

                lastX = x;
                lastY = y;
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            chess = new ChessGame();
            DrawChessPieces();   
        }
    }
}