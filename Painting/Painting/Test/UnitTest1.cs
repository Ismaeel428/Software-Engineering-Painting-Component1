using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace test
{
    [TestClass]
    public class CommandParserTests
    {
        private PictureBox pictureBox;
        private CommandParser parser;

        [TestInitialize]
        public void Initialize()
        {
            // Create a new PictureBox for each test to ensure a clean state
            pictureBox = new PictureBox();
            //Create a dummy action for validation
            Action invalidateAction = () => { };
            //Initialise the CommandParsere with the picture box
            parser = new CommandParser(pictureBox, invalidateAction);
        }

        [TestMethod]
        public void TestPenCommandChangesPenColor()
        {
            // Act
            parser.ExecuteCommand("pen red");  //HEre we are calling the command to change the pen color to red

            // Assert
            Assert.AreEqual(Color.Red, parser.CurrentPenColor, "The pen color should change to red."); //Here we are checking whether the color changes or not
        }




        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPenCommandThrowsException()
        {
            // Act
            parser.ExecuteCommand("pen rainbow");  //when we enter rainbow as colour it should throw an exception
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUnknownCommandThrowsException()
        {
            // Act
            parser.ExecuteCommand("unknowncommand 123"); //When we enter an unknown an unknow command it should throw an exception
        }


    }
}