using System.IO;

namespace _2DMazeSolver;

public class Program
{
    public static void Main(string[] Args)
    {
        PrintRunInstructions();

        // parse args
        string? inputFilepath = null;
        bool allPaths = false;

        for (var arg = 0; arg < Args.Length; arg++)
        {
            if (Args[arg] == "--filepath" && (arg + 1) < Args.Length)
            {
                inputFilepath = Args[arg + 1];
            }

            if (Args[arg] == "--all-paths")
            {
                allPaths = true;
            }
        }

        if (inputFilepath is null)
        {
            Console.WriteLine("No filepath was provided." +
                "\r\nPlease pass a filepath in the args." +
                "\r\nExample: dotnet run --filepath <filepath>");
            Console.WriteLine("Exiting program.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Reading file...");

        char[,] file;

        // Read file
        try
        {
            file = ReadMazeFile(inputFilepath);

            Console.WriteLine($"Read maze from filepath: {inputFilepath}");
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

        Console.WriteLine("Searching for solutions...");

        _ = maze.FindPath();

        var solutionCount = maze.Solutions.Count;

        if (solutionCount < 1)
        {
            Console.WriteLine("No paths were found");
            return;
        }

        if (allPaths is true)
        {
            Console.WriteLine($"Found {solutionCount} solution{((solutionCount == 1) ? "" : "s")}");
        }
        else
        {
            Console.WriteLine("Solution found");
        }


        // Write solution to file
        var solutionsDirectory = $"../Solutions/{Path.GetFileNameWithoutExtension(inputFilepath)}";

        Directory.CreateDirectory(solutionsDirectory);

        Console.WriteLine("Writing to file");

        Console.ForegroundColor = ConsoleColor.Yellow;

        if (allPaths is true)
        {
            var allPathsOutputFilepath = $"{solutionsDirectory}/pathAll.txt";

            using StreamWriter allSolutionsWriter = new StreamWriter(allPathsOutputFilepath);

            WriteMazeToFile(allSolutionsWriter, "Maze", maze.UnsolvedMaze);

            var count = 0;
        
            foreach (var solution in maze.Solutions)
            { 
                WriteMazeToFile(allSolutionsWriter, $"Solution {count}", solution.Path, solution.PathLength);
                count++;
            }
            
            Console.WriteLine($"All solutions written to {allPathsOutputFilepath}");
            Console.WriteLine();
            return;
        }

        var onePathOutputFilepath  = $"{solutionsDirectory}/path1.txt";

        using StreamWriter oneSolutionWriter  = new StreamWriter(onePathOutputFilepath);

        WriteMazeToFile(oneSolutionWriter, "Maze", maze.UnsolvedMaze);
        WriteMazeToFile(oneSolutionWriter, "Solution", maze.Solutions[0].Path, maze.Solutions[0].PathLength);

        Console.WriteLine($"One solution written to {onePathOutputFilepath}");
        Console.WriteLine();

        Console.ResetColor();
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

        writer.WriteLine($"----------- {label} -----------");
        writer.WriteLine();
        
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

    /// <summary>
    /// Prints the instructions on how to run the program
    /// </summary>
    private static void PrintRunInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine();
        Console.WriteLine("To run the 2dMazeSolver:");
        Console.WriteLine("dotnet run --filepath <path to input file> [--all-paths]");
        Console.WriteLine("* filepath must be included");
        Console.WriteLine("* all-paths flag is optional, including it will return all solutions instead of just 1");

        Console.ResetColor();
    }
}