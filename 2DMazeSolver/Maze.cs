using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace _2DMazeSolver;

public class Maze(char[,] unsolvedMaze, int startCol = -1, int startRow = -1)
{
    public char[,] UnsolvedMaze { get; init; } = unsolvedMaze; // [theRowYouAreOn:y:0top, theColYouAreOn:x:0left]
    public List<Solution> Solutions { get; private set; } = [];

    // Start coordinates
    private int StartCol = startCol;
    private int StartRow = startRow;

    // Max indices
    private readonly int RowCount = unsolvedMaze.GetLength(0); // 1 for column length
    private readonly int ColCount = unsolvedMaze.GetLength(1); // 0 for row length

    // Possible cell states
    private static class CellState
    {
        public const char Start = 'S';
        public const char Goal = 'G';
        public const char Open = '.';
        public const char Blocked = '#';
        public const char Visited = 'x';
    }

    /// <summary>
    /// Finds a path, if one exists, from the start of the maze to the goal
    /// </summary>
    /// <returns>true if a path was found, false if not</returns>
    public bool FindPath()
    {
        if (StartCol < 0 || StartRow < 0 || StartCol > (ColCount - 1) || StartRow > (RowCount - 1))
        {
            DetermineStartCoordinates();
        }

        Solutions.Clear();

        return FindPath(StartRow, StartCol, CopyMaze(UnsolvedMaze), 0);
    }


    private bool FindPath(int row, int col, char[,] maze, int pathLength)
    {
        // boundary checks
        if (row < 0 || row > (RowCount - 1) || col < 0 || col > (ColCount - 1)) return false;

        var cellState = maze[row, col];
        var newPathLength = pathLength + 1;

        // collision checks
        if (cellState == CellState.Blocked || cellState == CellState.Visited) return false;

        // goal check
        if (cellState == CellState.Goal)
        {
            SaveSolution(maze, newPathLength);
            return true;
        }

        // mark current coordinates as part of solution path
        maze[row, col] = CellState.Visited;

        // find next step in path (unlike ||, | should evaluate both sides of the operator)
        var result = FindPath(row - 1, col, maze, newPathLength)
            | FindPath(row, col + 1, maze, newPathLength)
            | FindPath(row + 1, col, maze, newPathLength)
            | FindPath(row, col - 1, maze, newPathLength);
        
        // backtrack
        maze[row, col] = cellState;

        return result;
    }
    
    /// <summary>
    /// Determines the coordinates of the cell labeled as the start.
    /// To be used if coordinates are not passed in upon init of Maze object.
    /// Time complexity: O(length x width) ~O(N^2)
    /// </summary>
    private void DetermineStartCoordinates()
    {
        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColCount; col++)
            {
                if (UnsolvedMaze[row, col] == CellState.Start)
                {
                    StartCol = col;
                    StartRow = row;

                    return;
                }
            }
        }

        throw new ArgumentException("Start coordinates could not be determined for the maze. " +
            $"Double check maze has a cell labeled {CellState.Start}");
    }

    /// <summary>
    /// Makes a copy of a maze
    /// </summary>
    private char[,] CopyMaze(char[,] maze)
    {
        return (char[,])maze.Clone();
    }

    /// <summary>
    /// Saves a maze solution to a list of solutions
    /// </summary>
    private void SaveSolution(char[,] maze, int pathLength)
    {
        var copiedMaze = CopyMaze(maze);

        copiedMaze[StartRow, StartCol] = CellState.Start;

        this.Solutions.Add(new(copiedMaze, pathLength));
    }

    /// <summary>
    /// A solution to the maze
    /// </summary>
    /// <param name="Maze">The path from start to goal</param>
    /// <param name="PathLength">The length of the path</param>
    public record Solution(char[,] Maze, int PathLength);
}
