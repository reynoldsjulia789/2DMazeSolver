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

        // Write solution to file
        using StreamWriter oneSolutionWriter  = new StreamWriter(onePathOutputFilepath);
        using StreamWriter allSolutionsWriter = new StreamWriter(allPathsOutputFilepath);

        WriteMazeToFile(oneSolutionWriter, "Maze", maze.UnsolvedMaze);
        oneSolutionWriter.WriteLine("---");
        WriteMazeToFile(oneSolutionWriter, "Solution", maze.Solutions[0].Path, maze.Solutions[0].PathLength);

        WriteMazeToFile(allSolutionsWriter, "Maze", maze.UnsolvedMaze);
        allSolutionsWriter.WriteLine("---");

        var count = 0;
        
        foreach (var solution in maze.Solutions)
        { 
            WriteMazeToFile(allSolutionsWriter, $"Solution {count}", solution.Path, solution.PathLength);
            count++;
        }

        Console.WriteLine($"One solution written to {onePathOutputFilepath}");
        Console.WriteLine($"All solutions written to {allPathsOutputFilepath}");
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

    /// <summary>
    /// Prints a graph to a file using the provided stream writer
    /// </summary>
    /// <param name="writer">stream writer writing to desired file</param>
    /// <param name="label">label for the graph</param>
    /// <param name="maze">maze to print</param>
    private static void WriteMazeToFile(StreamWriter writer, string label, char[,] maze, int pathLength = -1)
    {
        var rowCount = maze.GetLength(0);
        var colCount = maze.GetLength(1);

        writer.WriteLine($"-----------{label}-----------");
        
        for (var row = 0; row < rowCount; row++)
        {
            for (var col = 0; col < colCount; col++)
            {
                writer.Write(maze[row, col]);
            }

            writer.WriteLine();
        }

        if (pathLength > 0)
        {
            writer.WriteLine();
            writer.WriteLine($"Path length: {pathLength}");
        }

        writer.WriteLine();
    }
}