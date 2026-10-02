namespace _2DMazeSolver;

public class Program
{
    public static void Main(string[] Args)
    {
        char[,] file;
        var onePathOutputFilepath  = "path1.txt";
        var allPathsOutputFilepath = "pathAll.txt"; 

        // Read file
        try
        {
            if (Args.Length == 0)
            {
                throw new ArgumentException("No filepath was provided." +
                    "\r\nPlease pass a filepath in the args." +
                    "\r\nExample: dotnet run --<filepath>");
            }

            file = ReadMazeFile(Args[0]);
        }
        catch (Exception caught)
        {
            Console.WriteLine($"Program.Main: An error occurred reading the provided file");
            Console.WriteLine(caught.Message);
            Console.WriteLine("Exiting program.");
            return;
        }

        // Find Path
        var maze = new Maze(file);

        _ = maze.FindPath();

        if (maze.Solutions.Count < 1)
        {
            Console.WriteLine("No paths were found");
            return;
        }

        // Write solution file
        
    }

    /// <summary>
    /// Reads in the maze from a .txt file
    /// </summary>
    /// <param name="filepath">the path to the file containing the maze</param>
    /// <returns>2D array representing the maze</returns>
    private static char[,] ReadMazeFile(string filepath)
    {
        var rows = File.ReadAllLines(filepath);

        var rowCount = rows.Length;

        if (rowCount == 0) return new char[0, 0];

        var colCount = rows.Max(row => row.Length);

        var maze = new char[rowCount, colCount];

        for (var row = 0; row < rowCount; row++)
        {
            var currRow = rows[row];

            for (var col = 0; col < colCount; col++)
            {
                maze[row, col] = (col < currRow.Length) ? currRow[col] : Maze.CellState.Blocked;
            }
        }

        return maze;
    }

    private static void WriteToFile(string filepath, Maze maze, StreamWriter writer)
    {
        // finish
    }
}