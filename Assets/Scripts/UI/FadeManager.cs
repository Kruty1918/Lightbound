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

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SGS29.Demo.Lightbound
{
    /// <summary>
    /// Manages screen fade operations for transitioning between scenes.
    /// </summary>
    public class FadeManager : IFadeManager
    {
        private readonly Canvas canvas; // Canvas to hold the fade panel.
        private readonly GameObject panel; // The panel used for fading.
        private readonly Image panelImage; // Image component of the fade panel.
        private const float fadeDuration = 1f; // Duration of the fade effect in seconds.

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeManager"/> class.
        /// Creates a canvas and a fade panel for managing fade effects.
        /// </summary>
        public FadeManager()
        {
            // Create a new Canvas for the fade panel.
            canvas = new GameObject("GameCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Set canvas to overlay mode.
            Object.DontDestroyOnLoad(canvas.gameObject); // Prevents canvas from being destroyed on scene load.

            // Create the fade panel and configure its properties.
            panel = new GameObject("FadePanel");
            panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 1); // Start fully opaque black.
            panel.transform.SetParent(canvas.transform, false); // Set the panel as a child of the canvas.

            // Configure the panel's RectTransform to cover the entire screen.
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            rectTransform.anchoredPosition = Vector2.zero;

            panel.SetActive(false); // Initially hide the panel.
        }

        /// <summary>
        /// Fades the screen in from black.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FadeIn()
        {
            panel.SetActive(true); // Show the fade panel.
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                panelImage.color = new Color(0, 0, 0, t / fadeDuration); // Gradually change the alpha value.
                await Task.Yield(); // Yield control to allow for asynchronous execution.
            }
            panelImage.color = new Color(0, 0, 0, 1); // Ensure the panel is fully opaque at the end.
        }

        /// <summary>
        /// Fades the screen out to black.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FadeOut()
        {
            for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
            {
                panelImage.color = new Color(0, 0, 0, t / fadeDuration); // Gradually decrease the alpha value.
                await Task.Yield(); // Yield control to allow for asynchronous execution.
            }
            panelImage.color = new Color(0, 0, 0, 0); // Ensure the panel is fully transparent at the end.
            panel.SetActive(false); // Hide the panel.
        }
    }
}