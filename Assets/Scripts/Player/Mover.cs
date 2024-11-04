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


using SGS29.Utilities;
using UnityEngine;

namespace SGS29.Demo.Lightbound
{
    /// <summary>
    /// The <c>Mover</c> class handles the movement of an object within a maze environment. 
    /// It uses keyboard input to determine the direction of movement and interacts with 
    /// the maze's cells to determine possible actions, such as moving to a new cell or 
    /// reloading the scene when reaching a specific cell type.
    /// </summary>
    public class Mover : MonoBehaviour
    {
        /// <summary>
        /// The target position the object is moving toward. This is updated each time 
        /// a movement input is detected and validated.
        /// </summary>
        private Vector3 targetPosition;

        /// <summary>
        /// Initializes the object's starting position to the starting position defined in 
        /// the <c>MazeGenerator</c> class.
        /// </summary>
        private void Start()
        {
            // Sets the object's position and target position to the maze's starting position.
            transform.position = targetPosition = SM.Instance<MazeGenerator>().StartPosition;
        }

        /// <summary>
        /// Checks for specific keyboard inputs (W, A, S, D) on each frame. 
        /// Depending on the input, it calls the <c>Move</c> method with a 
        /// directional vector representing up, down, left, or right movement.
        /// </summary>
        private void Update()
        {
            // Detects player input and initiates movement in the specified direction.
            if (Input.GetKeyDown(KeyCode.W)) Move(Vector2Int.up);       // Move up
            else if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left); // Move left
            else if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down); // Move down
            else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right); // Move right
        }

        /// <summary>
        /// Attempts to move the object to a new position within the maze.
        /// The new position is determined by adding the provided direction 
        /// vector to the current position. Before moving, it verifies if 
        /// the target cell is passable.
        /// </summary>
        /// <param name="dir">The direction in which the object should move, 
        /// represented as a <c>Vector2Int</c> (e.g., (0, 1) for up).</param>
        private void Move(Vector2Int dir)
        {
            // Calculates the new position by adding the movement direction to the current target position.
            Vector2Int newPos = new Vector2Int(Mathf.RoundToInt(targetPosition.x) + dir.x, Mathf.RoundToInt(targetPosition.y) + dir.y);

            // Retrieves the cell value at the new position from the maze, indicating its type.
            int cell = SM.Instance<MazeGenerator>().Maze[newPos.x, newPos.y];

            // Checks if the cell is passable (type 1) or triggers a special action (type 4).
            if (cell == 1 || cell == 4)
            {
                // Updates the target position and moves the object to this new position.
                targetPosition = new Vector3(newPos.x, newPos.y, 0);
                transform.position = targetPosition;

                // If the cell type is 4, triggers a scene reload via the GameManager.
                if (cell == 4) GameManager.ReloadScene();
            }
        }
    }
}