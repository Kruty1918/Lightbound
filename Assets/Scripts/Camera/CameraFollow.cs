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
    /// This class provides functionality to make the camera follow the player's position smoothly
    /// and adjust its orthographic size to match the maze's dimensions.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        /// <summary>
        /// Reference to the player's transform, which determines the position the camera will follow.
        /// This needs to be assigned in the Inspector to the player GameObject.
        /// </summary>
        [SerializeField] private Transform player;

        /// <summary>
        /// Determines the smoothness of the camera movement.
        /// Lower values create a slower, more gradual following effect.
        /// </summary>
        [SerializeField] private float smoothSpeed = 0.125f;

        /// <summary>
        /// Positional offset applied to the camera's target position to provide a better view of the player and surroundings.
        /// This offset should be adjusted in the Inspector to position the camera at a comfortable distance.
        /// </summary>
        [SerializeField] private Vector3 offset;

        /// <summary>
        /// Reference to the Camera component attached to this GameObject.
        /// Used to adjust the camera's orthographic size based on the aspect ratio and maze dimensions.
        /// </summary>
        private Camera cam;

        /// <summary>
        /// Called once during the initialization phase.
        /// This method caches the Camera component for later use.
        /// </summary>
        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        /// <summary>
        /// Called once per frame, after all Update functions have been processed.
        /// Smoothly follows the player’s position and adjusts the camera's orthographic size.
        /// </summary>
        private void LateUpdate()
        {
            // Calculate the target position by adding the offset to the player's position
            Vector3 desiredPosition = player.position + offset;

            // Smoothly interpolate the camera’s position towards the target position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera's position, keeping the z-axis fixed
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

            // Calculate the screen's aspect ratio
            float aspectRatio = (float)Screen.width / Screen.height;

            // Adjust the orthographic size of the camera based on the maze's dimensions,
            // ensuring the entire maze area is visible within the camera's view.
            cam.orthographicSize = Mathf.Max(
                SM.Instance<MazeGenerator>().MapWidth / 2f / aspectRatio,
                SM.Instance<MazeGenerator>().MapHeight / 2f
            );
        }
    }
}