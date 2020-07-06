using System;
using System.Text;
using System.Configuration;

namespace TicTacToe
{
    public class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                var game = new GameSession();
                if (game.ProcessGameLoop() == false) { return; }
            }
        }
    }

    public enum CellState : byte
    {
        Vacant = 0,
        Player1Holds = 1,
        Player2Holds = 2
    }

    public class Cell
    {
        public Cell(CellState state = CellState.Vacant) => this.state = state;
        private CellState state = CellState.Vacant;
        public CellState State
        {
            get => state;
            set => state = value;
        }
    }

    public class PieceSymbols
    {
        public string vacantSymbol = ".";
        public string player1Symbol = "X";
        public string player2Symbol = "O";
    }

    public class Board
    {
        private Cell[,] cells;
        private PieceSymbols symbols;
        
        // The scale of the grid (ie 3x3 means scale of 3)
        public byte Scale { get; set; }

        public Board(byte scale = 3)
        {
            Scale = (scale >= 3 && scale <= Byte.MaxValue) ? scale : (byte)3;
            cells = new Cell[Scale, Scale];

            string vacantSymSetting = ConfigurationSettings.AppSettings.Get("VacantSymbol");
            string player1SymSetting = ConfigurationSettings.AppSettings.Get("Player1Symbol");
            string player2SymSetting = ConfigurationSettings.AppSettings.Get("Player2Symbol");

            symbols = new PieceSymbols();
            if (vacantSymSetting.Length > 0 && vacantSymSetting.Length <= 5) { symbols.vacantSymbol = vacantSymSetting; }
            if (player1SymSetting.Length > 0 && player1SymSetting.Length <= 5) { symbols.player1Symbol = player1SymSetting; }
            if (player2SymSetting.Length > 0 && player2SymSetting.Length <= 5) { symbols.player2Symbol = player2SymSetting; }

            for (byte x = 0; x < Scale; ++x)
            {
                for (byte y = 0; y < Scale; ++y)
                {
                    cells[x, y] = new Cell(); 
                }
            }
        }

        // Prints the contents of the game board to the screen along with an optional caption
        public void PrintContents(string caption)
        {
            if (caption.Length > 0) { Console.WriteLine(caption + "\n"); }

            for (int y = 0; y < Scale; ++y)
            {
                var currentLine = new StringBuilder();
                for (int x = 0; x < Scale; ++x)
                {
                    switch(cells[x, y].State)
                    {
                        case CellState.Vacant: { currentLine.Append(symbols.vacantSymbol); } break;
                        case CellState.Player1Holds: { currentLine.Append(symbols.player1Symbol); } break;
                        case CellState.Player2Holds: { currentLine.Append(symbols.player2Symbol); } break;
                        default:break;
                    }
                    currentLine.Append(" ");
                }

                Console.WriteLine(currentLine + "\n");
            }
        }

        // Used to retrieve a copy of a cell at the given coordinates
        // Returns: A copy of the cell at the given coordinates if found. Null otherwise.
        public Cell GetCell(byte x, byte y)
        {
            return (x < cells.Length && y < cells.Length) ? cells[x, y] : null;
        }

        // Checks to see if a cell at the given coordinates is vacant
        // Returns: True if cell is vacant. False otherwise.
        public bool IsCellOccupied(byte x, byte y)
        {
            Cell cell = GetCell(x, y);
            return (cell != null) ? cell.State != CellState.Vacant : false;
        }


        // Checks for a consecutive sequence of pieces from the top right to bottom left corner
        // Returns: A GameResult value representing the current state of the game
        public GameResult CheckForResult()
        {
            if (CheckForColumnVictory() ||
                CheckForRowVictory() ||
                CheckForLeftDiagonalVictory() ||
                CheckForRightDiagonalVictory())
            {
                return GameResult.Victory;
            }

            if ( CheckForAvailableCell() ) { return GameResult.NoResult; }

            return GameResult.Draw;
        }


        // Checks for a consecutive sequence of pieces across a column of the board
        // Returns: True if sequence found. False otherwise.
        private bool CheckForColumnVictory()
        {
            for (byte x = 0; x < Scale; ++x)
            {
                byte columnSequenceCount = 0;
                for (byte y = 0; y < (Scale - 1); ++y)
                {
                    if (cells[x, y].State != cells[x, y + 1].State ||
                        cells[x, y].State == CellState.Vacant ||
                        cells[x, y + 1].State == CellState.Vacant)
                    {
                        break;
                    }
                    else
                    {
                        columnSequenceCount++;
                    }
                }

                if (columnSequenceCount == (Scale - 1)) { return true; }
            }

            return false;
        }


        // Checks for a consecutive sequence of pieces across a row of the board
        // Returns: True if sequence found. False otherwise.
        private bool CheckForRowVictory()
        {
            for (byte y = 0; y < Scale; ++y)
            {
                byte rowSequenceCount = 0;
                for (byte x = 0; x < (Scale - 1); ++x)
                {
                    if (cells[x, y].State != cells[x + 1, y].State ||
                        cells[x, y].State == CellState.Vacant ||
                        cells[x + 1, y].State == CellState.Vacant)
                    {
                        break;
                    }
                    else
                    {
                        rowSequenceCount++;
                    }
                }

                if (rowSequenceCount == (Scale - 1)) { return true; }
            }
            
            return false;
        }


        // Checks for a consecutive sequence of pieces from the top left to bottom right corner
        // Returns: True if sequence found. False otherwise.
        private bool CheckForLeftDiagonalVictory()
        {
            byte sequenceCount = 0;
            for (byte xy = 0; xy < (Scale - 1); ++xy)
            {
                if (cells[xy, xy].State != cells[xy + 1, xy + 1].State ||
                    cells[xy, xy].State == CellState.Vacant ||
                    cells[xy + 1, xy + 1].State == CellState.Vacant)
                {
                    break;
                }
                else
                {
                    sequenceCount++;
                }

                if (sequenceCount == (Scale - 1)) { return true; }
            }

            return false;
        }

        // Checks for a consecutive sequence of pieces from the top right to bottom left corner
        // Returns: True if sequence found. False otherwise.
        private bool CheckForRightDiagonalVictory()
        {
            byte sequenceCount = 0;
            byte x = (byte)(Scale - 1);
            byte y = 0;
            while (x > 0 && y < Scale - 1)
            {
                if (cells[x, y].State != cells[x - 1, y + 1].State ||
                    cells[x, y].State == CellState.Vacant ||
                    cells[x - 1, y + 1].State == CellState.Vacant)
                {
                    break;
                }
                else
                {
                    sequenceCount++;
                }
         
                if (sequenceCount == (Scale - 1)) { return true; }

                x--;
                y++;
            }

            return false;
        }

        // Checks whether there are any vacant cells still available on the board
        // Returns: True if vacant cell found. False otherwise.
        private bool CheckForAvailableCell()
        {
            for (byte y = 0; y < Scale; ++y)
            {
                for (byte x = 0; x < Scale; ++x)
                {
                    if (cells[x, y].State == CellState.Vacant) { return true; }    
                }
            }

            return false;
        }
    }

    public enum GameResult : byte
    {
        NoResult = 0,
        Victory = 1,
        Draw = 2
    }

    public class GameSession
    {
        private Board board;
        public Board Board { get => board; }

        public GameSession()
        {
            byte boardScale = 0;
            byte.TryParse(ConfigurationSettings.AppSettings.Get("BoardScale"), out boardScale);

            board = new Board(boardScale);
            Console.Clear();
        }

        // Used to control the running of a game session
        // Returns: True if the player wants to play again. False otherwise.
        public bool ProcessGameLoop()
        {
            Console.WriteLine("Welcome to Tic Tac Toe!\n");
            Board.PrintContents("Here's the current board: ");

            bool isCurrentPlayer1 = true;
            while(true)
            {
                if (ProcessPlayerTurn(isCurrentPlayer1) == false)
                {
                    byte loser = (isCurrentPlayer1) ? (byte)1 : (byte)2;
                    byte victor = (isCurrentPlayer1) ? (byte)2 : (byte)1;
                    Console.WriteLine("Player {0} has given up. Player {1} wins!\n", loser, victor);
                    return ProcessPlayAgainRequest();
                }

                switch (Board.CheckForResult())
                {
                    case GameResult.Victory:
                    {
                        Console.WriteLine("Move accepted, well done you've won the game!\n");
                        return ProcessPlayAgainRequest(); 
                    }
                    case GameResult.Draw:
                    {
                        Console.WriteLine("No vacant cells remain, it's a draw!\n");
                        return ProcessPlayAgainRequest(); 
                    }
                    default:break;
                }

                isCurrentPlayer1 = !isCurrentPlayer1;
            }
        }
        
        // Requests input from a given player until valid input is received
        // Returns: True if a move was completed successfully. False if a player quits.
        private bool ProcessPlayerTurn(bool isPlayer1)
        {
            while(true)
            {
                string playerInput = ProcessPlayerInput(isPlayer1).ToLower();

                if (ProcessQuitRequest(playerInput))
                {
                    return false;
                }
                
                byte indexX = 0;
                byte indexY = 0;
                if ( ProcessMoveRequest(playerInput, out indexX, out indexY) )
                {
                    if (Board.IsCellOccupied(indexX, indexY))
                    {
                        Console.WriteLine("Oh no, a piece is already at this place! Try again...\n");
                    }
                    else
                    {
                        CellState newState = (isPlayer1) ? CellState.Player1Holds : CellState.Player2Holds;
                        Board.GetCell(indexX, indexY).State = newState;
                        Board.PrintContents("Move accepted, here's the current board:");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("I don't recognise that input. Try again...\n");
                }
            }          
        }

        // Retrieves player input and returns it
        // Returns: A string representing the player input received
        private string ProcessPlayerInput(bool isPlayer1)
        {
            char playerNum = (isPlayer1) ? '1' : '2';
            char playerSymbol = (isPlayer1) ? 'X' : 'O';
            Console.Write("Player {0} enter a coord x,y to place your {1} or enter 'q' to give up: ", playerNum, playerSymbol);
            string input = Console.ReadLine();
            Console.WriteLine();
            return input;
        }

        // Parses a given input for a quit character
        // Returns: True if quit request received. False otherwise.
        public bool ProcessQuitRequest(string input)
        {
            return input.Equals("q");
        }

        // Parses a given input for a valid move request and stores the result
        // Returns: True if the input represents a valid move. False otherwise.
        public bool ProcessMoveRequest(string input, out byte x, out byte y)
        {
            string[] digits = input.Split(',');
            if (digits.Length == 2 && byte.TryParse(digits[0], out x) && byte.TryParse(digits[1], out y)) 
            {
                if (x > 0 && x <= Board.Scale && y > 0 && y <= Board.Scale)
                {
                    x -= 1;
                    y -= 1;
                    return true;
                }
            }

            x = 0;
            y = 0;
            return false;
        }

        // Checks to see if the player would like to play another game session
        // Returns: True if the player would like to play again. False if they would not.
        public bool ProcessPlayAgainRequest()
        {
            while(true)
            {
                Console.Write("Would you like to play again? (enter 'y' for yes or 'n' for no): ");
                string input = Console.ReadLine();
                
                if (input.Equals("n")) { return false; }
                else if (input.Equals("y")) { return true; }
                else { Console.WriteLine("I don't recognise that input. Try again..."); }
            }
        }
    }
}