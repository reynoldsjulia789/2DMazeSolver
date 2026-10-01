using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace _2DMazeSolver;

public class Maze(char[][] unsolvedMaze, int startCol = -1, int startRow = -1)
{
    public char[][] UnsolvedMaze { get; init; } = unsolvedMaze; // [theRowYouAreOn:y:0top][theColYouAreOn:x:0left]
    public Dictionary<char[][], int> Solutions { get; private set; } = []; // solution array, path length
    public char[][] SolutionMaze { get; private set; } = unsolvedMaze;

    private readonly Dictionary<string, char> CellState = new()
    {
        ["start"]   = 'S',
        ["goal"]    = 'G',
        ["open"]    = '.',
        ["blocked"] = '#',
        ["visited"] = 'x',
    };

    private int StartCol = startCol;
    private int StartRow = startRow;

    /// <summary>
    /// Finds a path, if one exists from the start of the maze to the goal
    /// </summary>
    /// <returns>true if a path was found, false if not</returns>
    public bool FindPath()
    {
        if (StartCol < 0 || StartRow < 0)
        {
            DetermineStartCoordinates();
        }

        return FindPath(StartCol, StartRow);
    }


    private bool FindPath(int row, int col)
    {
        // check base cases
        if (col < 0
            || row < 0
            || col > (SolutionMaze.GetLength(0) - 1) // 0 for row length
            || row > (SolutionMaze.GetLength(1) - 1) // 0 for column length
            ) return false; // outside maze
        
        if (SolutionMaze[row][col] == CellState["goal"]) return true;

        if (SolutionMaze[row][col] == CellState["blocked"]
            || SolutionMaze[row][col] == CellState["visited"] // loop
            ) return false;

        // mark current coordinates as part of solution path
        SolutionMaze[row][col] = CellState["visited"];

        // find next step in path
        if (FindPath(row - 1, col) is true
            || FindPath(row, col + 1) is true
            || FindPath(row + 1, col) is true
            || FindPath(row, col - 1) is true
            ) return true;

        // no route forward from this cell found
        // unmark current coordinates
        SolutionMaze[row][col] = CellState["open"];

        return false;
    }
    
    /// <summary>
    /// Determines the coordinates of the cell labeled as the start.
    /// To be used if coordinates are not passed in upon init of Maze object.
    /// Time complexity: O(length x width) ~O(N^2)
    /// </summary>
    private void DetermineStartCoordinates()
    {
        for (int row = 0; row < SolutionMaze.GetLength(0); row++)
        {
            for (int col = 0; col < SolutionMaze.GetLength(1); col++)
            {
                if (SolutionMaze[row][col] == CellState["start"])
                {
                    StartCol = col;
                    StartRow = row;

                    return;
                }
            }
        }

        throw new ArgumentException("Start coordinates could not be determined for the maze. " +
            $"Double check maze has a cell labeled {CellState["start"]}");
    }
}
