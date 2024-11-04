// MIT License

// Copyright (c) 2024 Oleksiy Gavrylyuk

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// Contact: vkmr582@gmail.com
// Project GitHub: https://github.com/Kruty1918/Lightbound

using System.Collections.Generic;
using System.Linq;
using SGS29.Utilities;
using UnityEngine;

namespace SGS29.Demo.Lightbound
{
    /// <summary>
    /// A class responsible for generating a maze using a depth-first search algorithm.
    /// Inherits from a singleton base class to ensure only one instance exists.
    /// </summary>
    public class MazeGenerator : MonoSingleton<MazeGenerator>
    {
        public const int WALL = 0, FLOOR = 1, START = 3, EXIT = 4;

        [SerializeField] private bool _randomSize = true; // Flag to determine if the maze size should be random
        [SerializeField] private int _minSize = 5, _maxSize = 25; // Minimum and maximum size for random maze generation
        [SerializeField] private int _width = 11, _height = 11, _maximumRecursion = 100; // Width, height, and max recursion depth for maze generation
        [SerializeField] private float _cellSize = 1f, chanceWallToFlor = 0.1f; // Cell size in world units and chance to convert walls to floors
        [SerializeField] private GameObject startCellPrefab, floorCellPrefab, wallCellPrefab, exitCellPrefab; // Prefabs for maze cells

        private int[,] maze; // 2D array representing the maze structure
        private Vector2Int startCell = new Vector2Int(1, 1), exitCell; // Start and exit cell positions
        private List<Vector2Int> floorCells = new List<Vector2Int>(); // List of floor cells in the maze

        /// <summary>
        /// Gets the generated maze as a 2D array.
        /// </summary>
        public int[,] Maze => maze;

        /// <summary>
        /// Gets the starting position of the maze in world coordinates.
        /// </summary>
        public Vector3 StartPosition => new Vector3(startCell.x * _cellSize, startCell.y * _cellSize, 0);

        /// <summary>
        /// Gets the width of the maze.
        /// </summary>
        public int MapWidth { get => _width; }

        /// <summary>
        /// Gets the height of the maze.
        /// </summary>
        public int MapHeight { get => _height; }

        private void Start()
        {
            // Determine random size if enabled
            if (_randomSize)
            {
                _width = GetRandomOdd(_minSize, _maxSize);
                _height = GetRandomOdd(_minSize, _maxSize);
            }

            maze = new int[_width, _height]; // Initialize the maze array
            InitializeMaze(); // Set initial maze structure
            CarvePathDFS(startCell.x, startCell.y, 0); // Generate the maze path
            maze[startCell.x, startCell.y] = START; // Mark the start cell
            SetExitPoint(); // Set the exit cell in the maze
            InstantiateMaze(); // Create the maze GameObjects in the scene
        }

        /// <summary>
        /// Gets a random odd number between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">Minimum value for the range.</param>
        /// <param name="max">Maximum value for the range.</param>
        /// <returns>A random odd number within the specified range.</returns>
        private int GetRandomOdd(int min, int max)
            => Enumerable.Range(min, max).Where(n => n % 2 != 0).OrderBy(_ => Random.value).First();

        /// <summary>
        /// Initializes the maze with walls on the borders and empty spaces inside.
        /// </summary>
        private void InitializeMaze()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    maze[x, y] = (x == 0 || x == _width - 1 || y == 0 || y == _height - 1) ? WALL : 0; // Set walls on the borders
        }

        /// <summary>
        /// Carves a path through the maze using depth-first search.
        /// </summary>
        /// <param name="x">Current x position in the maze.</param>
        /// <param name="y">Current y position in the maze.</param>
        /// <param name="depth">Current recursion depth.</param>
        private void CarvePathDFS(int x, int y, int depth)
        {
            if (depth >= _maximumRecursion) return; // Exit if maximum recursion depth is reached

            maze[x, y] = FLOOR; // Mark current position as a floor
            floorCells.Add(new Vector2Int(x, y)); // Add to list of floor cells

            // Shuffle directions for random path carving
            foreach (var dir in Shuffle(new[] { 0, 1, 2, 3 }))
            {
                int nx = x + (dir == 0 ? 2 : dir == 2 ? -2 : 0),
                    ny = y + (dir == 1 ? 2 : dir == 3 ? -2 : 0);

                // Check if new position is within bounds and is a wall
                if (IsWithinBounds(nx, ny) && maze[nx, ny] == WALL)
                {
                    maze[nx, ny] = maze[x + (nx - x) / 2, y + (ny - y) / 2] = FLOOR; // Carve a path
                    CarvePathDFS(nx, ny, depth + 1); // Recursively carve the next path
                }
            }
            TryConvertWalls(new Vector2Int(x, y)); // Attempt to convert adjacent walls to floors
        }

        /// <summary>
        /// Converts adjacent wall cells to floor cells based on a random chance.
        /// </summary>
        /// <param name="pos">The position of the current floor cell.</param>
        private void TryConvertWalls(Vector2Int pos)
        {
            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                var wallPos = pos + dir; // Calculate adjacent wall position
                if (IsWithinBounds(wallPos.x, wallPos.y) && maze[wallPos.x, wallPos.y] == WALL && Random.value < chanceWallToFlor)
                {
                    maze[wallPos.x, wallPos.y] = FLOOR; // Convert wall to floor
                    floorCells.Add(wallPos); // Add to list of floor cells
                    TryConvertWalls(wallPos); // Recursively try to convert walls
                }
            }
        }

        /// <summary>
        /// Sets the exit point of the maze at a suitable floor cell.
        /// </summary>
        private void SetExitPoint()
        {
            exitCell = floorCells.Where(loc => CountAdjacent(loc, FLOOR) == 1) // Find a floor cell with only one adjacent floor cell
                .OrderByDescending(loc => loc.x + loc.y) // Order by position to ensure better placement
                .FirstOrDefault();

            if (exitCell != default) maze[exitCell.x, exitCell.y] = EXIT; // Mark exit cell
        }

        /// <summary>
        /// Counts the number of adjacent cells of a specified type.
        /// </summary>
        /// <param name="loc">The position of the cell to check.</param>
        /// <param name="type">The type of cell to count.</param>
        /// <returns>The number of adjacent cells matching the specified type.</returns>
        private int CountAdjacent(Vector2Int loc, int type) =>
            new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }
            .Count(d => IsWithinBounds(loc.x + d.x, loc.y + d.y) && maze[loc.x + d.x, loc.y + d.y] == type);

        /// <summary>
        /// Instantiates GameObjects for each cell in the maze.
        /// </summary>
        private void InstantiateMaze()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    Instantiate(GetPrefab(maze[x, y]), new Vector3(x * _cellSize, y * _cellSize, 0), Quaternion.identity);
        }

        /// <summary>
        /// Returns the prefab corresponding to a given cell type.
        /// </summary>
        /// <param name="type">The type of cell.</param>
        /// <returns>The prefab associated with the specified cell type.</returns>
        private GameObject GetPrefab(int type) => type switch
        {
            START => startCellPrefab,
            EXIT => exitCellPrefab,
            FLOOR => floorCellPrefab,
            _ => wallCellPrefab // Default to wall prefab
        };

        /// <summary>
        /// Checks if the specified coordinates are within the bounds of the maze.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>True if within bounds, otherwise false.</returns>
        private bool IsWithinBounds(int x, int y) => x > 0 && x < _width - 1 && y > 0 && y < _height - 1;

        /// <summary>
        /// Shuffles an array of integers and returns a new array.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        /// <returns>A shuffled array.</returns>
        private static IEnumerable<T> Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                yield return array[k];
                array[k] = array[n];
            }
            yield return array[0];
        }
    }
}
