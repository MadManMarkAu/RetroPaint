# RetroPaint
Demonstration of the "span filling" flood fill algorithm using WinForms and .NET 10.

Inspired by a video from Coco Town, [Retro graphics GENIUS revealed! Inside 1980s BASIC’s PAINT](https://www.youtube.com/watch?v=f9YkL9_GfwQ), I attempted to recreate the painting algirthm described in this video. I thin discovered [WikiPedia](https://en.wikipedia.org/wiki/Flood_fill#Span_filling) had an entire section dedicated to this algorithm, which was much easier to understand.

## Usage
Run the project, then drag/drop an image onto the form. The image will load and display in glorious 8-bit indexed color. click a pixel to start a floof fill at that location.

Displayed bitmap will update after each step of the flood fill, showing how the drawing stack returns to previous areas only once future encounted areas are complete.