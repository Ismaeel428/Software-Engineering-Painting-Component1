using System;
using System.Windows.Forms;

namespace Painting
{
    // This class represents the main form of my painting application.
    public partial class Form1 : Form
    {
        // This field holds an instance of the CommandParser class which is used to parse and execute commands.
        private CommandParser commandParser;

        // The constructor for Form1 where initialization occurs.
        public Form1()
        {
            // This call is necessary for WinForms designer support.
            InitializeComponent();

            // Initialize the CommandParser with a PictureBox control and a delegate to Invalidate that control.
            // The PictureBox is likely where the drawing will occur, and Invalidate will cause it to repaint.
            commandParser = new CommandParser(pbDrawingArea, () => pbDrawingArea.Invalidate());

            // This sets the window state of the form to maximized when the application starts.
            this.WindowState = FormWindowState.Maximized;
        }

        // Event handler for the 'Run' button click event.
        private void btnRun_Click(object sender, EventArgs e)
        {
            // Retrieves the command input text from a TextBox control.
            var command = txtCommandInput.Text;
            try
            {
                // Tries to execute the command using the CommandParser instance.
                commandParser.ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                // If an exception occurs, it is caught and displayed in a message box.
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for the 'Check Syntax' button click event.
        private void btnCheckSyntax_Click(object sender, EventArgs e)
        {
            // Retrieves the command input text from a TextBox control.
            var command = txtCommandInput.Text;
            try
            {
                // Tries to check the syntax of the command using the CommandParser instance.
                commandParser.CheckSyntax(command);

                // If no exception is thrown, a message box indicates the syntax is correct.
                MessageBox.Show("Syntax is correct.", "Syntax Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // If a syntax error occurs, it is caught and displayed in a message box.
                MessageBox.Show(ex.Message, "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for the 'Exit' button click event.
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            // This method call terminates the application.
            Application.Exit();
        }

        // Event handler for a click event on the PictureBox (presumably the drawing area).
        // Currently, this event handler does nothing.
        private void pbDrawingArea_Click(object sender, EventArgs e)
        {
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
