using System;
using System.Drawing; // Required for graphics operations
using System.Windows.Forms; // Required for manipulating form controls

/// <summary>
/// This class is responsible for parsing and executing a set of drawing commands
/// on a PictureBox control's canvas.
/// </summary>
public class CommandParser
{
    private Graphics graphics; // Graphics object to perform drawing
    private Pen currentPen;  // Pen to draw shapes and lines
    public Pen CurrentPen // Public property to expose the currentPen
    {
        get { return currentPen; }
    }

    public Color CurrentPenColor // Public property to expose the color of the currentPen
    {
        get { return currentPen.Color; }
    }

    public Point CurrentPosition { get; set; }

    private bool fillMode; // Indicates whether shapes should be filled
    private Point currentPos; // Current position of the "pen" on the canvas
    private Bitmap canvas; // Bitmap that serves as the drawing surface
    private Action invalidatePictureBox; // Delegate to invalidate the PictureBox
    private PictureBox pictureBox; // The PictureBox control used for drawing

    /// <summary>
    /// Constructor initializing the CommandParser with a PictureBox.
    /// </summary>
    /// <param name="pictureBox">The PictureBox control where drawings will be rendered.</param>
    /// <param name="invalidateAction">The action to invalidate the PictureBox for updates.</param>
    public CommandParser(PictureBox pictureBox, Action invalidateAction)
    {
        this.pictureBox = pictureBox;
        canvas = new Bitmap(pictureBox.Width, pictureBox.Height); // Initialize the canvas with the size of the PictureBox
        graphics = Graphics.FromImage(canvas); // Get a Graphics object to draw on the bitmap
        pictureBox.Image = canvas; // Set the PictureBox's Image property to the bitmap
        currentPen = new Pen(Color.Black); // Initialize the pen with a default color
        fillMode = false; // Default to no fill mode
        currentPos = new Point(0, 0); // Start at the top-left corner
        invalidatePictureBox = invalidateAction; // Set the delegate for invalidating the PictureBox
    }

    // Resets the current pen position to the top-left corner
    private void ResetPenPosition()
    {
        currentPos = new Point(0, 0);
    }

    /// <summary>
    /// Executes the given command by parsing the input and calling the appropriate methods.
    /// </summary>
    /// <param name="command">The command string to parse and execute.</param>
    public void ExecuteCommand(string command)
    {
        // Split the command into parts and identify the action
        var parts = command.Trim().Split(' ');
        var action = parts[0].ToLower();

        // Switch statement to handle different commands
        switch (action)
        {
            case "moveto":
                MoveTo(parts);
                break;
            case "drawto":
                DrawTo(parts);
                break;
            case "rectangle":
                DrawRectangle(parts);
                break;
            case "circle":
                DrawCircle(parts);
                break;

            case "triangle":
                DrawTriangle(parts);
                break;

            case "pen":
                ChangePenColor(parts);
                break;
            case "fill":
                SetFillMode(parts);
                break;
            case "clear":
                ClearDrawingArea();
                break;
            case "reset":
                ResetPenPosition();
                break;
            default:
                throw new InvalidOperationException("Unknown command.");
        }
        pictureBox.Image = canvas;
        InvalidatePictureBox();
    }

    private void ChangePenColor(string[] parts)
    {
        if (parts.Length != 2) throw new ArgumentException("Pen command expects one parameter: color name.");

        // Change the current pen color
        Color color;
        switch (parts[1].ToLower())
        {
            case "red":
                color = Color.Red;
                break;
            case "green":
                color = Color.Green;
                break;
            case "blue":
                color = Color.Blue;
                break;

            default:
                throw new ArgumentException("Invalid color specified.");
        }

        var newPen = new Pen(color);
        currentPen.Dispose(); // Dispose of the old pen
        currentPen = newPen;
    }
    /// <summary>
    /// Sets the fill mode based on the specified parameter.
    /// </summary>
    /// <param name="parts">Array containing the command and parameters.</param>

    private void SetFillMode(string[] parts)
    {
        if (parts.Length != 2) throw new ArgumentException("Fill command expects one parameter: on or off.");

        // Set the fill mode
        switch (parts[1].ToLower())
        {
            case "on":
                fillMode = true;
                MessageBox.Show("Fill mode is now ON", "Fill Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;
            case "off":
                fillMode = false;
                MessageBox.Show("Fill mode is now OFF", "Fill Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;
            default:
                throw new ArgumentException("Invalid fill mode specified. Use 'on' or 'off'.");
        }
    }

    //function to refresh the box
    private void InvalidatePictureBox()
    {
        pictureBox.Invalidate();
    }


    /// <summary>
    /// Moves the pen to a new position on the drawing area.
    /// </summary>
    /// <param name="x">The x-coordinate of the new position.</param>
    /// <param name="y">The y-coordinate of the new position.</param>
    private void MoveTo(string[] parts)
    {
        // Expecting parts to be ["moveto", "x", "y"]
        if (parts.Length != 3) throw new ArgumentException("MoveTo expects two parameters: x and y coordinates.");

        int x = int.Parse(parts[1]);
        int y = int.Parse(parts[2]);
        currentPos = new Point(x, y); // Update current position

        // Draw a small circle or dot to represent the pen
        DrawPenPosition();

    }
    // This function is to show where the current pointer is
    private void DrawPenPosition()
    {
        const int penSize = 2; // Size of the pen position indicator
        graphics.FillEllipse(currentPen.Brush, currentPos.X - penSize / 2, currentPos.Y - penSize / 2, penSize, penSize);
        InvalidatePictureBox();
    }

    /// <summary>
    /// Draws a line from the current position to a new specified position.
    /// </summary>
    /// <param name="parts">An array of strings where parts[1] is the x-coordinate and parts[2] is the y-coordinate.</param>
    private void DrawTo(string[] parts)
    {
        // The command is expected to have exactly three parts: "drawto", "x", "y"
        if (parts.Length != 3) throw new ArgumentException("DrawTo expects two parameters: x and y coordinates.");

        int x = int.Parse(parts[1]);
        int y = int.Parse(parts[2]);
        // Draws a line from the current position to the new position (x, y)
        graphics.DrawLine(currentPen, currentPos, new Point(x, y));
        currentPos = new Point(x, y); // Updates the current position to the new position after drawing the line
    }

    /// <summary>
    /// Draws a rectangle at the current position with the specified width and height.
    /// </summary>
    /// <param name="parts">An array of strings where parts[1] is the width and parts[2] is the height.</param>
    private void DrawRectangle(string[] parts)
    {
        if (parts.Length != 3) throw new ArgumentException("Rectangle expects two parameters: width and height.");

        int width = int.Parse(parts[1]);
        int height = int.Parse(parts[2]);

        if (fillMode)
        {
            // Fills the rectangle with the current pen's color
            using (var brush = new SolidBrush(currentPen.Color))
            {
                graphics.FillRectangle(brush, currentPos.X, currentPos.Y, width, height);
            }
        }
        else
        {
            // Draws only the rectangle's border
            graphics.DrawRectangle(currentPen, currentPos.X, currentPos.Y, width, height);
        }
    }
    private void DrawTriangle(string[] parts)
    {
        if (parts.Length != 4) throw new ArgumentException("Triangle expects three parameters: base and two sides length.");

        int baseLength = int.Parse(parts[1]);
        int sideLength1 = int.Parse(parts[2]);
        int sideLength2 = int.Parse(parts[3]);

        // Calculate the triangle's vertices
        Point p1 = currentPos;
        Point p2 = new Point(currentPos.X + baseLength, currentPos.Y);
        // Assuming an isosceles triangle for simplicity
        Point p3 = new Point(currentPos.X + baseLength / 2, currentPos.Y - sideLength1);

        Point[] points = { p1, p2, p3 };

        if (fillMode)
        {
            using (var brush = new SolidBrush(currentPen.Color))
            {
                graphics.FillPolygon(brush, points);
            }
        }
        else
        {
            graphics.DrawPolygon(currentPen, points);
        }
    }



    /// <summary>
    /// Draws a circle at the current position with the specified radius.
    /// </summary>
    /// <param name="parts">An array of strings where parts[1] is the radius.</param>
    private void DrawCircle(string[] parts)
    {
        if (parts.Length != 2) throw new ArgumentException("Circle expects one parameter: radius.");

        int radius = int.Parse(parts[1]);

        if (fillMode)
        {
            // Fills the ellipse with the current pen's color
            using (var brush = new SolidBrush(currentPen.Color))
            {
                graphics.FillEllipse(brush, currentPos.X - radius, currentPos.Y - radius, radius * 2, radius * 2);
            }
        }
        else
        {
            // Draws only the ellipse's border
            graphics.DrawEllipse(currentPen, currentPos.X - radius, currentPos.Y - radius, radius * 2, radius * 2);
        }
    }
    /// <summary>
    /// Clears the entire drawing area, resetting it to white.
    /// </summary>
    private void ClearDrawingArea()
    {
        // Clears the canvas to white, effectively erasing any previous drawing
        graphics.Clear(Color.White);
    }

    /// <summary>
    /// Checks the syntax of the provided command string.
    /// </summary>
    /// <param name="command">The command to validate.</param>
    public void CheckSyntax(string command)
    {
        var parts = command.Trim().Split(' ');
        var action = parts[0].ToLower();

        switch (action)
        {
            case "moveto":
            case "drawto":
                CheckMoveAndDrawToSyntax(parts);
                break;
            case "rectangle":
                CheckRectangleSyntax(parts);
                break;
            case "circle":
                CheckCircleSyntax(parts);
                break;
            case "triangle":
                CheckTriangleSyntax(parts);
                break;
            case "pen":
                CheckPenSyntax(parts);
                break;
            case "fill":
                CheckFillSyntax(parts);
                break;
            case "clear":
            case "reset":
                // just check for the correct length
                if (parts.Length != 1)
                {
                    throw new FormatException($"'{action}' command does not take parameters.");
                }
                break;
            default:
                throw new FormatException($"Unknown command: '{action}'.");
        }
    }

    /// <summary>
    /// Validates the syntax for 'moveto' and 'drawto' commands.
    /// </summary>
    /// <param name="parts">Array containing the command parts to validate.</param>
    /// <exception cref="FormatException">Thrown when the syntax does not match the expected format.</exception>
    private void CheckMoveAndDrawToSyntax(string[] parts)
    {
        // Validates that the command has three parts and that the x and y coordinates are integers
        if (parts.Length != 3 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
        {
            throw new FormatException($"Invalid syntax for {parts[0]}. Correct syntax: '{parts[0]} x y'.");
        }
    }

    /// <summary>
    /// Validates the syntax for the 'rectangle' command.
    /// </summary>
    /// <param name="parts">Array containing the command parts to validate.</param>
    /// <exception cref="FormatException">Thrown when the syntax does not match the expected format.</exception>
    private void CheckRectangleSyntax(string[] parts)
    {
        // Validates that the command has three parts and that the width and height are integers
        if (parts.Length != 3 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
        {
            throw new FormatException("Invalid syntax for rectangle. Correct syntax: 'rectangle width height'.");
        }
    }

    /// <summary>
    /// Validates the syntax for the 'circle' command.
    /// </summary>
    /// <param name="parts">Array containing the command parts to validate.</param>
    /// <exception cref="FormatException">Thrown when the syntax does not match the expected format.</exception>
    private void CheckCircleSyntax(string[] parts)
    {
        // Validates that the command has two parts and that the radius is an integer
        if (parts.Length != 2 || !int.TryParse(parts[1], out _))
        {
            throw new FormatException("Invalid syntax for circle. Correct syntax: 'circle radius'.");
        }
    }

    /// <summary>
    /// Validates the syntax for the 'pen' command.
    /// </summary>
    /// <param name="parts">Array containing the command parts to validate.</param>
    /// <exception cref="FormatException">Thrown when the syntax does not match the expected format or the color is not known.</exception>
    private void CheckPenSyntax(string[] parts)
    {
        // Validates that the command has two parts and that the specified color is known
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid syntax for pen. Correct syntax: 'pen color'.");
        }

        // Check if the color is a valid named color
        if (!Color.FromName(parts[1]).IsKnownColor)
        {
            throw new FormatException($"'{parts[1]}' is not a known color.");
        }
    }

    /// <summary>
    /// Validates the syntax for the 'fill' command.
    /// </summary>
    /// <param name="parts">Array containing the command parts to validate.</param>
    /// <exception cref="FormatException">Thrown when the syntax does not match the expected format.</exception>
    private void CheckFillSyntax(string[] parts)
    {
        // Validates that the command has two parts and that the parameter is either 'on' or 'off'
        if (parts.Length != 2 || !(parts[1].ToLower() == "on" || parts[1].ToLower() == "off"))
        {
            throw new FormatException("Invalid syntax for fill. Correct syntax: 'fill on' or 'fill off'.");
        }
    }
    private void CheckTriangleSyntax(string[] parts)
    {
        // Validates that the command has four parts and that the base and sides are integers
        if (parts.Length != 4 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _) || !int.TryParse(parts[3], out _))
        {
            throw new FormatException("Invalid syntax for triangle. Correct syntax: 'triangle base side1 side2'.");
        }
    }


}
