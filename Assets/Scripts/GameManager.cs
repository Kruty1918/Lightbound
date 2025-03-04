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

namespace SGS29.Demo.Lightbound
{
    /// <summary>
    /// Manages game state and provides a global interface for scene reloading.
    /// </summary>
    public static class GameManager
    {
        private static readonly IFadeManager fadeManager = new FadeManager(); // Singleton instance of the fade manager.
        private static readonly ISceneLoader sceneLoader = new SceneLoader(fadeManager); // Scene loader using the fade manager.

        /// <summary>
        /// Reloads the current scene asynchronously with fade effects.
        /// </summary>
        public static async void ReloadScene()
        {
            await sceneLoader.ReloadScene(); // Initiates the scene reloading process.
        }
    }
}