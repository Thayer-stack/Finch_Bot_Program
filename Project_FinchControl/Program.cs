using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using FinchAPI;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Project_FinchControl
{

    // **************************************************\\\
    //
    // Title: Finch Control - Menu Starter
    // Description: Starter solution with the helper methods,
    //              opening and closing screens, and the menu
    // Application Type: Console
    // Author: Velis, John
    // Author: Thayer, Shaun
    // Dated Created: 1/22/2020
    // Last Modified: 11/29/2020
    //
    // **************************************************\\\
    public enum Command
    {
        NONE = 1,
        MOVEBACKWARD,
        MOVEFORWARD,
        STOPMOTOR,
        WAIT,
        TURNRIGHT,
        TURNLEFT,
        LEDON,
        LEDOFF,
        GETTEMPERATURE,
        DONE,
    }
    class Program
    {
        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string userName = null;

            Console.WindowHeight = 35;
            Console.WindowWidth = 90;
            SetTheTheme(userName);
            DisplayWelcomeScreen();
            userName = DisplayUserLoginScreen();
            DisplayMenuScreen(userName);
        }

        #region Login Screen

        /// <summary>
        /// *********************************\\\
        ///  Displaies the user login screen  \\\
        /// ***********************************\\\
        /// </summary>
        /// <returns>The user login screen.</returns>
        private static string DisplayUserLoginScreen() 
        {


            bool quitApplication = false;
            string menuChoice;
            string userName = null;           
            bool validLogin = false;
          
            do
            {
                DisplayScreenHeader("User Login Menu");
                Console.WriteLine("\t\t_________________________");
                Console.WriteLine($"\n\t  {DateTime.Now.ToLongDateString()}" +
                	" {DateTime.Now.ToLongTimeString()}\n");
                Console.WriteLine("\t\t(a): User Login");
                Console.WriteLine("\t\t(b): Register User");
                Console.WriteLine("\t\t(d): For Main Menu");
                Console.WriteLine("\t\t(g): Change the Console Colors");
                Console.WriteLine("\t\t(q): Quit");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        userName = UserLoginScreen(out validLogin);
                        break;

                    case "b":
                        UserCreationScreen();
                        break;

                    case "d":
                        if (userName == null)
                        {
                            Console.WriteLine("\n\tPlease Login or Register.");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                            DisplayMenuScreen(userName);
                        }
                        break;

                    case "q":
                        ;
                        quitApplication = true;
                        break;
                    case "g":
                        DisplayChangeTheTheme(userName);
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);

            return userName;
        }

        /// <summary>
        /// Users the login screen.
        /// </summary>
        /// <returns>The login screen.</returns>
        /// <param name="validLogin">If set to <c>true</c> valid login.</param>
        private static string UserLoginScreen(out bool validLogin)
        {
            string[] usersCredentials = new string[2];

            do
            {
                DisplayScreenHeader("User Login");
                Console.Write("\n\tPlease enter your Username: ");
                usersCredentials[0] = Console.ReadLine();
                Console.Write("\n\n\tPlease enter your Password: ");
                usersCredentials[1] = Console.ReadLine();

                validLogin = FindUsersCredentials(usersCredentials);

                if (validLogin == true){

                    Console.WriteLine("\n\tYou have been logged in!");
                    DisplayMenuPrompt("To User Login");
                }
                else
                {
                    validLogin = false;
                    Console.WriteLine("\n\tCould not find username and/or password.");
                    Console.WriteLine("\n\tTo return to Login Menu Screen Please hit the \"Esc\" Key");
                    Console.WriteLine("\n\tOr hit \"Enter\" to try to login again");
                    var info = Console.ReadKey().Key;

                    switch (info)
                    {
                        case ConsoleKey.Escape:
                            DisplayUserLoginScreen();
                            break;
                        default:
                            break;
                    }
                }

            } while (!validLogin);

            string userName = usersCredentials[0];

            SetTheTheme(userName);

            return userName;
        }

        /// <summary>
        /// Users the creation screen.
        /// </summary>
        private static void UserCreationScreen()
        {
            string datapath = @"Users_Info/Users.txt";
            string[] usersNewCredentials = new string[2];
            bool validResponse;

            do
            {
                DisplayScreenHeader("User Registration");

                Console.Write("\n\tPlease enter a Username: ");
                usersNewCredentials[0] = Console.ReadLine();
                Console.Write("\n\n\tPlease enter a Password: ");
                usersNewCredentials[1] = Console.ReadLine();

                if (usersNewCredentials[0].Length >= 3 && usersNewCredentials[1].Length >= 3)
                {
                    validResponse = true;
                }
                else
                {
                    validResponse = false;
                    Console.WriteLine("\n\tPlease enter a UserName and a Password" +
                    	"\n\tthat is at least 3 Characters Long.");
                }

            } while (!validResponse);

            CreationOfUsersFolders(usersNewCredentials);

            File.AppendAllText(datapath, "\n" + usersNewCredentials[0] + "," + usersNewCredentials[1]);

            Console.WriteLine("\n\tUser Created");

            DisplayContinuePrompt();
        }

        /// <summary>
        /// Creations the of users folders.
        /// </summary>
        /// <param name="usersNewCredentials">Users new credentials.</param>
        private static void CreationOfUsersFolders(string[] usersNewCredentials)
        {
            string datapath = $@"{usersNewCredentials[0]}";
            string datapathCommands = $@"{usersNewCredentials[0]}/Commands";
            string datapathData = $@"{usersNewCredentials[0]}/Data/";
            string datapathTheme = $@"{usersNewCredentials[0]}/Theme/";

            Directory.CreateDirectory(datapath);
            Directory.CreateDirectory(datapathCommands);
            Directory.CreateDirectory(datapathData);
            Directory.CreateDirectory(datapathTheme);

            RightThemeToFileDefault(usersNewCredentials);

        }

        /// <summary>
        /// Rights the theme to file default.
        /// </summary>
        /// <param name="usersNewCredentials">Users new credentials.</param>
        static void RightThemeToFileDefault(string[] usersNewCredentials)
        {
            string dataPath = $@"{usersNewCredentials[0]}/Theme/Theme.txt";
            string theme = "White" + "," + "Black";

            if (theme != null)
            {
                File.WriteAllText(dataPath, theme);
            }
        }

        /// <summary>
        /// Finds the users credentials.
        /// </summary>
        /// <returns><c>true</c>, if users credentials was found, <c>false</c> otherwise.</returns>
        /// <param name="usersCredentials">Users credentials.</param>
        private static bool FindUsersCredentials(string[] usersCredentials)
        {
            string datapath = @"Users_Info/Users.txt";
            string[] usersCredentialsArray = new string[datapath.Length];

            bool validLogin = false;

            usersCredentialsArray = File.ReadAllLines(datapath);

            foreach (string userInfo in usersCredentialsArray)
            {
                if (userInfo == usersCredentials[0] + "," + usersCredentials[1])
                {
                    validLogin = true;
                    break;
                }
                else 
                {
                    validLogin = false;
                }
            }
            return validLogin;
        }

        #endregion

        #region MAIN MENU
        /// <summary>
        /// *****************************************************************
        /// *                     Main Menu                                 *
        /// *****************************************************************
        /// </summary>
        /// <param name="userName">User name.</param>
        static void DisplayMenuScreen(string userName)
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool quitApplication = false;
            string menuChoice;


            Finch myFinch = new Finch();

            do
            {
                DisplayScreenHeader("Main Menu");
                Console.WriteLine("\t\t_________________________\n");

                Console.WriteLine("\t\t(a): Connect Finch Robot");
                Console.WriteLine("\t\t(b): Talent Show");
                Console.WriteLine("\t\t(c): Data Recorder");
                Console.WriteLine("\t\t(d): Alarm System");
                Console.WriteLine("\t\t(e): User Programming");
                Console.WriteLine("\t\t(f): Disconnect Finch Robot");
                Console.WriteLine("\t\t(g): Change the Console Colors");

                Console.WriteLine("\n\t\t(u): User Login Menu");

                Console.WriteLine("\t\t(q): Quit");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        DisplayConnectFinchRobot(myFinch);
                        break;

                    case "b":
                        DisplayTalentShowMenuScreen(myFinch);
                        break;

                    case "c":
                        DisplayDataRecorderMenuScreen(myFinch, userName);
                        break;

                    case "d":
                        DisplayAlarmSystemMenuScreen(myFinch);
                        break;

                    case "e":
                        DisplayUserProgramingMenu(myFinch, userName);
                        break;

                    case "f":
                        DisplayDisconnectFinchRobot(myFinch);
                        break;

                    case "q":
                        quitApplication = true;
                        break;

                    case "g":
                        DisplayChangeTheTheme(userName);
                        break;
                    
                    case "u":
                        userName = DisplayUserLoginScreen();
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;}

            } while (!quitApplication);

            DisplayClosingScreen();
        }

        #endregion

        #region Set The Theme
        /// <summary>
        /// Displaies the change the theme.
        /// </summary>
        static void DisplayChangeTheTheme(string userName)
        {
            DisplayGetTheTheme(userName);
            SetTheTheme(userName);

            DisplayMenuPrompt("To the Main");
        }

        /// <summary>
        /// Displaies the change the theme.
        /// </summary>
        static void SetTheTheme(string userName)
        {
            ReadThemeFile(out ConsoleColor foregroundColor, out ConsoleColor backgroundColor, userName);

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Clear();
        }

        /// <summary>
        /// Reads the theme file.
        /// </summary>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        private static void ReadThemeFile(out ConsoleColor foregroundColor, out ConsoleColor backgroundColor, string userName)
        {
            string dataPath = $@"{userName}/Theme/Theme.txt";
            string dataPathDefault = @"Theme/Theme.txt";

            if (File.Exists(dataPath))
            {
                string[] readTextArray = File.ReadAllText(dataPath, Encoding.ASCII).Split(new[] { ',' });
                Enum.TryParse(readTextArray[0], out foregroundColor);
                Enum.TryParse(readTextArray[1], out backgroundColor);
            }
            else
            {
                string[] readTextArray = File.ReadAllText(dataPathDefault, Encoding.ASCII).Split(new[] { ',' });
                Enum.TryParse(readTextArray[0], out foregroundColor);
                Enum.TryParse(readTextArray[1], out backgroundColor);
            }
        }

        /// <summary>
        /// Rights the theme to file.
        /// </summary>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        static void RightThemeToFile(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string userName)
        {
            string dataPath = $@"{userName}/Theme/Theme.txt";
            string theme = foregroundColor + "," + backgroundColor;
            string dataPathDefault = @"Theme/Theme.txt";

            if (File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, theme);
            }
            else
            {
                File.WriteAllText(dataPathDefault, theme);
            }
        }

        /// <summary>
        /// Displaies the change the theme.
        /// </summary>
        static void DisplayGetTheTheme(string userName)
        {
            bool validResponse;
            ConsoleColor backgroundColor;
            ConsoleColor foregroundColor;

            DisplayScreenHeader("Change the console Colors");
            Console.WriteLine("\n\tColor Chart" +
                                "\n\t_____________________\n");

            foreach (ConsoleColor item in Enum.GetValues(typeof(ConsoleColor)))
            {
                Console.WriteLine("\t" + item.ToString());
            }

            do
            {
                Console.Write("\n\tPlease Choose the Console Text Color, [Case sensitive]: ");
                validResponse = false;

                if (!Enum.TryParse(Console.ReadLine(), out foregroundColor))
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease Choose a proper color from the above Diagram");
                }
                else
                {
                    validResponse = true;
                }

            } while (!validResponse);

            do
            {
                Console.Write("\n\tPlease Choose the Console Background Color, [Case sensitive]: ");

                if (Enum.TryParse(Console.ReadLine(), out backgroundColor))
                {
                    validResponse = true;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease Chosose a proper color from the above Diagram");
                }
            } while (!validResponse);

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.Clear();

            RightThemeToFile(foregroundColor, backgroundColor, userName);

        }
        #endregion

        #region USER PROGRAMING

        /// <summary>
        /// ***********************\\\
        ///  USER PROGRAMMING MENU  \\\
        /// *************************\\\
        /// </summary>
        /// <param name="myFinch"></param>
        private static void DisplayUserProgramingMenu(Finch myFinch, string userName)
        {
            Console.Clear();
            Console.CursorVisible = true;
            int[] ledBrightness = new int[3];
            bool quitApplication = false;
            string menuChoice;

            DisplayScreenHeader("User Programing Menu");

            (int motorSpeed, int[] ledBrightness, double waitSeconds) commandParameters;
            commandParameters = (0, ledBrightness, 0);

            List<Command> commands = new List<Command>();
            do
            {
                DisplayScreenHeader("User Programing Menu");
                Console.WriteLine("\t\t___________________________________\n");

                Console.WriteLine("\t\t(a): Set Command Parameters");
                Console.WriteLine("\t\t(b): Add Commands to a list");
                Console.WriteLine("\t\t(c): View the Command list");
                Console.WriteLine("\t\t(d): Execute Command list");
                Console.WriteLine("\t\t(e): Command the Finch in real time");
                Console.WriteLine("\t\t(f); Access Data Files");
                Console.WriteLine("\t\t(q): Quit Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();
                switch (menuChoice)
                {
                    case "a":
                        commandParameters = DisplaySetCommandParameters();
                        break;

                    case "b":
                        DisplayGetCommands(commands, userName);
                        break;

                    case "c":
                        DisplayViewCommandsAndParameters(commands, commandParameters, userName);
                        break;

                    case "d":
                        DisplayExecuteCommands(myFinch, commands, commandParameters);
                        break;

                    case "e":
                        DisplayCommandFinchLive(myFinch, commands, commandParameters);
                        break;

                    case "q":
                        quitApplication = true;
                        break;

                    default:
                        break;
                }
            } while (!quitApplication);
        }

        /// <summary>
        /// Displaies the command finch live.
        /// </summary>
        /// <param name="myFinch">My finch.</param>
        /// <param name="commands">Commands.</param>
        /// <param name="commandParameters">Command parameters.</param>
        private static void DisplayCommandFinchLive(Finch myFinch, List<Command> commands, (int motorSpeed, int[] ledBrightness, double waitSeconds) commandParameters)
        {
            bool isUserDone = false;
            double temperature = 0;
            
            Console.Clear();
            
            do
            {
                Console.Clear();               
                Console.CursorVisible = false;

                Console.WriteLine("\n\t\t\tCommand the Finch Live");

                Console.WriteLine("\t ______________________________________________________");
                Console.WriteLine("\t|                                                      |");
                Console.WriteLine("\t| Please use the following keys to control the Finch   |");
                Console.WriteLine("\t|______________________________________________________|");
                Console.WriteLine("\t|               |              ||     |                |");
                Console.WriteLine("\t| Movement      | Key to Press || LED | Key to Press   |");
                Console.WriteLine("\t|_______________|______________||_____|________________|");
                Console.WriteLine("\t|               |              ||     |                |");
                Console.WriteLine("\t| Move forwad   | Up Arrow     || ON  | Page up        |");
                Console.WriteLine("\t|_______________|______________||_____|________________|");
                Console.WriteLine("\t|               |              ||     |                |");
                Console.WriteLine("\t| Move backward | Down Arrow   || OFF | Page Down      |");
                Console.WriteLine("\t|_______________|______________||_____|________________|");
                Console.WriteLine("\t|               |              ||                      |");
                Console.WriteLine("\t| Move Right    | Right Arrow  || Increase Motor Speed |");
                Console.WriteLine("\t|_______________|______________||______________________|");
                Console.WriteLine("\t|               |              ||                      |");
                Console.WriteLine("\t| Move Left     | Left Arrow   || Press \"Home\" Key     |");
                Console.WriteLine("\t|_______________|______________||______________________|");
                Console.WriteLine("\t|               |              ||                      |");
                Console.WriteLine("\t| Stop          | Spacebar     || Decrease Motor Speed |");
                Console.WriteLine("\t|_______________|______________||______________________|");                              
                Console.WriteLine("\t|                              ||                      |");
                Console.WriteLine("\t|                              || Press \"End\" Key      |");
                Console.WriteLine("\t|______________________________||______________________|");
                Console.WriteLine("\t|                              ||                      |");
                Console.WriteLine("\t| Take Temperature Reading     ||   Press Letter \"t\"   |");                
                Console.WriteLine("\t|______________________________||______________________|");

                var info = Console.ReadKey().Key;

                Console.SetCursorPosition(10, 30);
                Console.Write($"\n\tCurrent Command: {info}");
                Console.WriteLine($"\n\n\tTemperature Reading: {temperature}");

                switch (info)
                {
                    case ConsoleKey.UpArrow:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setMotors(commandParameters.motorSpeed, commandParameters.motorSpeed);                        
                        break;
                    case ConsoleKey.DownArrow:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setMotors(-commandParameters.motorSpeed, -commandParameters.motorSpeed);                        
                        break;
                    case ConsoleKey.Spacebar:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setMotors(0, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setMotors(commandParameters.motorSpeed, -commandParameters.motorSpeed);                       
                        break;
                    case ConsoleKey.LeftArrow:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setMotors(-commandParameters.motorSpeed, commandParameters.motorSpeed);                        
                        break;
                    case ConsoleKey.PageUp:
                        myFinch.setLED(commandParameters.ledBrightness[0], commandParameters.ledBrightness[1], commandParameters.ledBrightness[2]);
                        break;
                    case ConsoleKey.PageDown:
                        Console.Write($"\n\tCurrent Command: {info}");
                        myFinch.setLED(0, 0, 0);
                        break;
                    case ConsoleKey.T:
                        temperature = myFinch.getTemperature();
                        break;
                    case ConsoleKey.Escape:
                        isUserDone = true;
                        DisplayMenuPrompt("To User Programming");
                        break;
                    case ConsoleKey.Home:
                        commandParameters = (commandParameters.motorSpeed + 10, commandParameters.ledBrightness, commandParameters.waitSeconds);
                        break;
                    case ConsoleKey.End:
                        commandParameters = (commandParameters.motorSpeed - 10, commandParameters.ledBrightness, commandParameters.waitSeconds);
                        break;
                    default:
                        break;
                }
            } while (!isUserDone);
        }

        /// <summary>
        /// Displaies the execute commands.
        /// </summary>
        /// <param name="myFinch">My finch.</param>
        /// <param name="commands">Commands.</param>
        /// <param name="commandParameters">Command parameters.</param>
        private static void DisplayExecuteCommands(Finch myFinch, List<Command> commands,
            (int motorSpeed, int[] ledBrightness, double waitSeconds) commandParameters)
        {          
            Console.Clear();
            DisplayScreenHeader("Execute commands");

            Console.WriteLine("\n\tThe Finch robot is about to Execute the commands");
            DisplayContinuePrompt();

            foreach (Command command in commands)
            {
                switch (command)
                {
                    case Command.NONE:
                        Console.WriteLine("Invalid Command");
                        break;
                    case Command.MOVEBACKWARD:
                        myFinch.setMotors(-commandParameters.motorSpeed, -commandParameters.motorSpeed);
                        break;
                    case Command.MOVEFORWARD:
                        myFinch.setMotors(commandParameters.motorSpeed, commandParameters.motorSpeed);
                        break;
                    case Command.STOPMOTOR:
                        myFinch.setMotors(0, 0);
                        break;
                    case Command.WAIT:
                        myFinch.wait((int)commandParameters.waitSeconds * 1000);
                        break;
                    case Command.TURNRIGHT:
                        myFinch.setMotors(commandParameters.motorSpeed, -commandParameters.motorSpeed);
                        break;
                    case Command.TURNLEFT:
                        myFinch.setMotors(-commandParameters.motorSpeed, commandParameters.motorSpeed);
                        break;
                    case Command.LEDON:
                        myFinch.setLED(commandParameters.ledBrightness[0], commandParameters.ledBrightness[1], commandParameters.ledBrightness[2]);
                        break;
                    case Command.LEDOFF:
                        myFinch.setLED(0, 0, 0);
                        break;
                    case Command.GETTEMPERATURE:
                        double temperature = myFinch.getTemperature();
                        Console.WriteLine(temperature);
                        break;
                    case Command.DONE:
                        myFinch.setMotors(0, 0);
                        DisplayMenuPrompt("To User Programing");
                        break;
                    default:
                        break;
                }
            }            
        }

        /// <summary>
        /// Displaies the view commands.
        /// </summary>
        /// <param name="commands">Commands.</param>
        private static void DisplayViewCommandsAndParameters(List<Command> commands, (int motorSpeed, int[] ledBrightness, double waitSeconds) commandParameters, string userName)
        {
            Console.Clear();
            DisplayScreenHeader("User Programing");

            Console.WriteLine($"\n\tMotor Speed: {commandParameters.motorSpeed}");
            Console.WriteLine($"\n\tRed Color Value: {commandParameters.ledBrightness[0]}");
            Console.WriteLine($"\n\tGreen Color Value: {commandParameters.ledBrightness[1]}");
            Console.WriteLine($"\n\tBlue Color Value: {commandParameters.ledBrightness[2]}");
            Console.WriteLine($"\n\tWait in Seconds: {commandParameters.waitSeconds}");

            ReadCommandsToFile(commands, userName);

            DisplayMenuPrompt("To User Programing");
        }

        /// <summary>
        /// Reads the commands to file.
        /// </summary>
        /// <param name="commands">Commands.</param>
        private static void ReadCommandsToFile(List<Command> commands, string userName)
        {
            string dataPath = $@"{userName}/Commands/Commands.txt";
            string[] filetextArray = new string[commands.LongCount()];

            if (File.Exists(dataPath))
            {
                filetextArray = File.ReadAllLines(dataPath);
                foreach (var command in filetextArray)
                {
                    Console.WriteLine("\t" + command);
                }
            }
            else
            {
                Console.WriteLine("\n\tPlease Right Commands to file");
            }           
        }

        /// <summary>
        /// Displaies the get commands.
        /// </summary>
        /// <returns>The get commands.</returns>
        /// <param name="commands">Commands.</param>
        private static object DisplayGetCommands(List<Command> commands, string userName)
        {         
            Command command;
            bool validResponse;
            bool isDoneAddingCommands = false;
            string userResponse;
                   
            do
            {
                Console.Clear();
                validResponse = true;

                DisplayScreenHeader("Enter Commands");

                foreach (Command commandName in Enum.GetValues(typeof(Command)))
                {
                    if (commandName.ToString() != "NONE")
                    {
                        Console.WriteLine("\t\t" + commandName.ToString().ToLower());
                    }
                }
                Console.WriteLine("\n\tPlease use the above commands to create a list of commands to be" +
                                  "\n\texecuted by the finch bot. ");
                Console.Write("\n\n\tThe [");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("wait");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("] command is used to give the length of time between commands.");
                Console.Write("\n\n\tFor example, [");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("MoveForwad, Wait = (if 1 sec), TurnRight, Wait, StopMotor");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("],\n\tthe Finch bot will moveforward for 1 second then turn right" +
                    "\n\t for 1 sec then stop.");
                Console.Write("\n\n\tPlease enter [");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Done");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("] when finished.");
                Console.CursorVisible = true;
                Console.Write($"\n\tEnter Command: ");
                userResponse = Console.ReadLine().ToUpper();
                Console.SetCursorPosition(10, 15);

                if (userResponse != "DONE")
                {
                    if (!Enum.TryParse(userResponse, out command))
                    {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(10, 26);
                        Console.WriteLine("\n\tPlease enter a proper command");
                        DisplayContinuePrompt();
                        validResponse = false;
                    }
                    else
                    {
                        commands.Add(command);
                    }
                }
                else // Adds DONE to end of list for user, so finch bot stops using its case switch
                {
                    Enum.TryParse(userResponse, out command);
                    commands.Add(command);
                    isDoneAddingCommands = true;
                    Console.CursorVisible = false;                    
                    Console.SetCursorPosition(10, 26);
                    DisplayMenuPrompt("To User Programing");
                }
            } while (!validResponse || !isDoneAddingCommands);

            RightCommandsToFile(commands, userName);
                        
            return commands;
        }
        /// <summary>
        /// Rights the commands to file.
        /// </summary>
        /// <param name="commands">Commands.</param>
        private static void RightCommandsToFile(List<Command> commands, string userName)
        {
            string dataPath = $@"{userName}/Commands/Commands.txt";

            foreach (var command in commands)
            {
                File.AppendAllText(dataPath, command.ToString() + "\n");
            }
        }

        /// <summary>
        /// Displaies the set command parameters.
        /// </summary>
        /// <returns>The set command parameters.</returns>
        private static (int motorSpeed, int[] ledBrightness, double waitSeconds) DisplaySetCommandParameters()
        {
            int[] ledBrightness = new int[3];
            (int motorSpeed, int[] ledBrightness, double waitSeconds) commandParameters;
            commandParameters = (0, ledBrightness, 0);

            Console.Clear();
            DisplayScreenHeader("Command Parameters");

            bool validResponse;
                       
            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tEnter Motor Speed: ");
                
                if (int.TryParse(Console.ReadLine(), out commandParameters.motorSpeed) && (commandParameters.motorSpeed <= 255))
                {
                    Console.CursorVisible = false;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tEnter the Red Brightness of the LED: ");
                
                if (int.TryParse(Console.ReadLine(), out int ledBrightnessRed) && (ledBrightnessRed <= 255))
                {
                    Console.CursorVisible = false;
                    commandParameters.ledBrightness[0] = ledBrightnessRed;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tEnter the Green Brightness of the LED: ");
                
                if (int.TryParse(Console.ReadLine(), out int ledBrightnessGreen) && (ledBrightnessGreen <= 255))
                {
                    Console.CursorVisible = false;
                    commandParameters.ledBrightness[1] = ledBrightnessGreen;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tEnter the Blue Brightness of the LED: ");
                
                if (int.TryParse(Console.ReadLine(), out int ledBrightnessBlue) && (ledBrightnessBlue <= 255))
                {
                    Console.CursorVisible = false;
                    commandParameters.ledBrightness[2] = ledBrightnessBlue; 
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tEnter Wait in Seconds:");
                
                if (double.TryParse(Console.ReadLine(), out commandParameters.waitSeconds))
                {
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("To User Programing");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            return commandParameters;
        }

        #endregion

        #region ALARM SYSTEM
        /// <summary>
        /// ***************************************\\\
        ///  Displaies the alarm system menu screen \\\
        /// *****************************************\\\
        /// </summary>
        /// <param name="myfinch">Myfinch.</param>
        static void DisplayAlarmSystemMenuScreen(Finch myfinch)
        {            
            bool quitAlarmSystemMenu = false;
            string menuChoice;

            string[] sensorsToMonitor = null;
           
            int[] LightSenorsMinMaxThresholdValues = new int[4];            
            int[] TemperatureMinMaxThresholdValues = new int[2];
            
            int timeToMonitor = 0;            

            do
            {
                DisplayScreenHeader         ("Alarm System Menu");
                Console.WriteLine("\t\t_______________________________\n");
                Console.WriteLine("\t\t(a): Sensors to Monitor");
                Console.WriteLine("\t\t(b): Light Sensor Threshold");
                Console.WriteLine("\t\t(c); Temperature Sensor Threshold");
                Console.WriteLine("\t\t(d): Time to Monitor");
                Console.WriteLine("\t\t(e): Set Alarm System");
                Console.WriteLine("\t\t(q): Quit Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                
                Console.CursorVisible = true;

                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        sensorsToMonitor = AlarmSystemSensorsToMonitor();
                        break;

                    case "b":
                        LightSenorsMinMaxThresholdValues = AlarmSystemLightSensorsThreshold(myfinch);
                        break;

                    case "c":
                         TemperatureMinMaxThresholdValues = AlarmSystemTemperatureThreshold(myfinch);
                        break;
                        
                    case "d":
                        timeToMonitor = AlarmSystemTimeToMonitor();
                        break;
                    case "e":
                        if (sensorsToMonitor == null || (TemperatureMinMaxThresholdValues == null && sensorsToMonitor[1] == "yes")
                            || (LightSenorsMinMaxThresholdValues == null && sensorsToMonitor[0] != "no") || timeToMonitor == 0)
                        {

                            Console.WriteLine("\n\tPlease enter all required values.");

                            DisplayContinuePrompt();
                        }
                        else
                        {
                            AlarmSystemSetAlarms(myfinch, TemperatureMinMaxThresholdValues, sensorsToMonitor, LightSenorsMinMaxThresholdValues, timeToMonitor);
                        }
                        break;

                    case "q":
                        quitAlarmSystemMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitAlarmSystemMenu);

        }

        /// <summary>
        /// Alarm System Set Alarms \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <param name="TemperatureMinMaxThresholdValues"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="LightSenorsMinMaxThresholdValues"></param>
        /// <param name="timeToMonitor"></param>
        static void AlarmSystemSetAlarms(Finch myfinch, int[] TemperatureMinMaxThresholdValues, string[] sensorsToMonitor, int[] LightSenorsMinMaxThresholdValues, int timeToMonitor)
        {                   
            int seconds = 1;            

            DisplayScreenHeader("Set the Alarm System");

            Console.CursorVisible = false;

            Console.WriteLine($"\n\n\t\t\t\tSensors to Monitor: " +
                 "\n\t________________________________________________________________\n" +
                $"\n\n\tLight Sensor: {sensorsToMonitor[0]}" +
                $"\n\n\tTemperature Sensor: {sensorsToMonitor[1]}" +
                $"\n\n\tObstacle Sensor: {sensorsToMonitor[2]}");

            Console.WriteLine("\n\t\t\t\tCurrent Thresholds");
            Console.WriteLine("\t________________________________________________________________\n");
            Console.WriteLine($"\n\tMin/Max Threshhold Values for Light Sensors:" +
                $"\n\n\tLeft Minimum: {LightSenorsMinMaxThresholdValues[0]} Left Maximum: {LightSenorsMinMaxThresholdValues[1]}" +
                $"\n\n\tRight Minimum: {LightSenorsMinMaxThresholdValues[2]} Right Maximum: {LightSenorsMinMaxThresholdValues[3]}");
            Console.WriteLine($"\n\tMin/Max Threshhold Values for Temp Sensors: Minimum: {TemperatureMinMaxThresholdValues[0]} Maximum: {TemperatureMinMaxThresholdValues[1]}");
            Console.WriteLine($"\n\tTime for Alarm system to Monitor in seconds: {timeToMonitor / 1000}");
            Console.WriteLine("$\n\tPress any key to set the alarm");
            Console.CursorVisible = false;

            DisplayContinuePrompt();

            Console.CursorVisible = true;

            bool thresholdExceeded;

            Console.Clear();

            do
            {
                thresholdExceeded = false;

                // Get Current Senors Reading \\

                thresholdExceeded = AlarmSystemLightSensorThresholdExceeded(myfinch, sensorsToMonitor, LightSenorsMinMaxThresholdValues) ||
                    AlarmSystemTemperaturesThreshHoldExceeded(TemperatureMinMaxThresholdValues, sensorsToMonitor, myfinch) ||
                    AlarmSystemObjectDetected(sensorsToMonitor, myfinch);

                if (thresholdExceeded == false)
                {
                    seconds++;
                    Console.SetCursorPosition(10, 10);
                    Console.WriteLine($"\t\tTime: {seconds}");
                    myfinch.setLED(0, 255, 0);
                    myfinch.wait(1000);
                    myfinch.setLED(0, 0, 0);
                    GetCurrentSenorsReading(myfinch);
                    seconds++;

                }
                else
                {
                    thresholdExceeded = true;
                }
            } while (thresholdExceeded == false && seconds < timeToMonitor);

            if (thresholdExceeded == true)
            {
                Console.Clear();

                Console.WriteLine("\n\tThreshold Exceeded!\n");
                myfinch.noteOn(1200);
                myfinch.setLED(255, 0, 0);

                Console.WriteLine($"\n\tMin/Max Threshhold Values for Light Sensors:" +
                $"\n\n\tLeft Minimum: {LightSenorsMinMaxThresholdValues[0]} Left Maximum: {LightSenorsMinMaxThresholdValues[1]}" +
                $"\n\n\tRight Minimum: {LightSenorsMinMaxThresholdValues[2]} Right Maximum: {LightSenorsMinMaxThresholdValues[3]}");
                Console.WriteLine($"\n\tMin/Max Threshhold Values for Temp Sensors: Minimum: {TemperatureMinMaxThresholdValues[0]} Maximum: {TemperatureMinMaxThresholdValues[1]}");
                Console.WriteLine($"\n\tTime for Alarm system to Monitor in seconds: {timeToMonitor}");

                GetCurrentSenorsReading(myfinch);

                DisplayContinuePrompt();
                myfinch.noteOff();
                myfinch.setLED(0,0,0);

            }
            else
            {
                Console.Clear();
                Console.WriteLine("\n\t\tAlarm timed out before Threshold was Exceeded");
                Console.WriteLine($"\n\tMin/Max Threshhold Values for Light Sensors:" +
               $"\n\n\tLeft Minimum: {LightSenorsMinMaxThresholdValues[0]} Left Maximum: {LightSenorsMinMaxThresholdValues[1]}" +
               $"\n\n\tRight Minimum: {LightSenorsMinMaxThresholdValues[2]} Right Maximum: {LightSenorsMinMaxThresholdValues[3]}");
                Console.WriteLine($"\n\tMin/Max Threshhold Values for Temp Sensors: Minimum: {TemperatureMinMaxThresholdValues[0]} Maximum: {TemperatureMinMaxThresholdValues[1]}");
                Console.WriteLine($"\n\tTime for Alarm system to Monitor in seconds: {timeToMonitor}");

                GetCurrentSenorsReading(myfinch);
            }


            DisplayMenuPrompt("Alarm system");
        }

        /// <summary>
        /// GetCurrentSenorsReading \\\
        /// </summary>
        /// <param name="myfinch"></param>
        static void GetCurrentSenorsReading(Finch myfinch)
        {
            Console.CursorVisible = false;
            bool currentobstacleReadingleft = myfinch.isObstacleLeftSide();
            bool currentobstacleReadingright = myfinch.isObstacleRightSide();


            Console.WriteLine($"\n\n\tCurrent Light Sensor Reading: Left: {myfinch.getLeftLightSensor()} Right: {myfinch.getRightLightSensor()}");
            Console.WriteLine($"\n\n\tCurrent Temperature Sensor Reading: {myfinch.getTemperature()} Celcius");
            Console.WriteLine($"\n\n\tCurrent object Sensor Reading: Left {currentobstacleReadingleft} Right {currentobstacleReadingright}");
        }

        /// <summary>
        /// Alarm System Object Detected \\\
        /// </summary>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static bool AlarmSystemObjectDetected(string[] sensorsToMonitor, Finch myfinch)
        {            
            bool[] getObstacleReadingArray = new bool[2];
            bool thresholdExceeded = false;                        
            
            getObstacleReadingArray[0] = myfinch.isObstacleLeftSide();
            getObstacleReadingArray[1] = myfinch.isObstacleRightSide();
            
            switch (sensorsToMonitor[2])
            {
                case "yes":
                    if (getObstacleReadingArray[0] == true || getObstacleReadingArray[1] == true)
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;

        }

        /// <summary>
        /// Alarm System Temperatures Thresh Hold Exceeded \\\
        /// </summary>
        /// <param name="TemperatureMinMaxThresholdValues"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static bool AlarmSystemTemperaturesThreshHoldExceeded(int[] TemperatureMinMaxThresholdValues, string[] sensorsToMonitor, Finch myfinch)
        {
            double currentTemperatureReading = myfinch.getTemperature();

            bool thresholdExceeded = false;

            switch (sensorsToMonitor[1])
            {
                case "yes":
                    if (currentTemperatureReading < TemperatureMinMaxThresholdValues[0]  || currentTemperatureReading > TemperatureMinMaxThresholdValues[1])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;

        }

        /// <summary>
        /// Alarm System Light sensor Threshold Exceeded \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="LightSenorsMinMaxThresholdValues"></param>
        /// <returns></returns>
        static bool AlarmSystemLightSensorThresholdExceeded(Finch myfinch, string[] sensorsToMonitor, int[] LightSenorsMinMaxThresholdValues)
        {
            int currentLeftLightSensorValue = myfinch.getLeftLightSensor();
            int currentRightLightSensorValue = myfinch.getRightLightSensor();

            bool thresholdExceeded;

            // int[] LightSenorsMinMaxThresholdValues
            // ******** Array Index ********
            // [0] = minThresholdValuesLeft 
            // [1] = maxThresholdValuesLeft 
            // [2] = minThresholdValuesRight
            // [3] = maxThresholdValuesRight

            switch (sensorsToMonitor[0])
            {
                case "left":
                    if (currentLeftLightSensorValue < LightSenorsMinMaxThresholdValues[0] || currentLeftLightSensorValue > LightSenorsMinMaxThresholdValues[1])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;
                case "right":
                    if (currentRightLightSensorValue < LightSenorsMinMaxThresholdValues[2] || currentRightLightSensorValue > LightSenorsMinMaxThresholdValues[3])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "both":
                    if (currentLeftLightSensorValue < LightSenorsMinMaxThresholdValues[0] || currentRightLightSensorValue < LightSenorsMinMaxThresholdValues[2] || currentLeftLightSensorValue > LightSenorsMinMaxThresholdValues[1] || currentRightLightSensorValue > LightSenorsMinMaxThresholdValues[3])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;
                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;
        }

        /// <summary>
        /// Alarm System Time To Monitor \\\
        /// </summary>
        /// <returns></returns>
        static int AlarmSystemTimeToMonitor()
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool validResponse;

            DisplayScreenHeader("Time to Monitor");

            int timeToMonitor;
            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tLength of time for Alarm To Monitor in Seconds: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out timeToMonitor))
                {
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm system");

                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            

            return timeToMonitor;

        }

        /// <summary>
        /// Alarm System Light Sensors Threshold \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static int[] AlarmSystemLightSensorsThreshold(Finch myfinch)
        {
            Console.Clear();
            Console.CursorVisible = true;

            int[] LightSenorsMinMaxThresholdValues = new int[4];

            // Array Index
            // [0] = minThresholdValuesLeft 
            // [1] = maxThresholdValuesLeft 
            // [2] = minThresholdValuesRight
            // [3] = maxThresholdValuesRight

            Console.WriteLine($"\n\tCurrent values for both left and right light sensor:" +
                $"\n\tLeft: {myfinch.getLeftLightSensor()} Right: {myfinch.getRightLightSensor()}");
            bool validResponse;

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tEnter Minimum Threshold for [Left] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int minThresholdValuesLeft))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[0] = minThresholdValuesLeft;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tEnter Maximum Threshold for [Left] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int maxThresholdValuesLeft))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[1] = maxThresholdValuesLeft;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tEnter Minimum Threshold for [Right] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int minThresholdValuesRight))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[2] = minThresholdValuesRight;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tEnter Minimum Threshold for [Right] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int maxThresholdValuesRight))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[3] = maxThresholdValuesRight;
                    DisplayMenuPrompt("Alarm system");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            return LightSenorsMinMaxThresholdValues;
        }

        /// <summary>
        /// Alarms the system temperature threshold.
        /// </summary>
        /// <returns>The system temperature threshold.</returns>
        /// <param name="myfinch">Myfinch.</param>
        static int[] AlarmSystemTemperatureThreshold(Finch myfinch)
        {
            Console.Clear();
            Console.CursorVisible = true;

            int[] TemperatureMinMaxThresholdValues = new int[2];
            bool validResponse;

            Console.WriteLine($"\n\tCurrent Temperature Value:" +
                $"{myfinch.getTemperature()} Celcius.");
            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tPlease Enter Mininum Temperature Threshold: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out TemperatureMinMaxThresholdValues[0]))
                {
                    Console.CursorVisible = false;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write($"\n\tPlease Enter Maximum Temperature Threshold: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out TemperatureMinMaxThresholdValues[1]))
                {
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm system");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            return TemperatureMinMaxThresholdValues;
        }
                
        /// <summary>
        /// Alarm system sensors to monitor \\\
        /// </summary>
        /// <returns>The system sensors to monitor.</returns>
        static string[] AlarmSystemSensorsToMonitor()
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool validResponse;
            string[] sensorsToMonitor = new string[3];

            Console.WriteLine("\n\tSensors to Monitor\n");

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tDo you want to Monitor the Light Sensors? [left, right, both, no]: ");
                string lightSensorsToMonitor = Console.ReadLine().ToLower();

                if (lightSensorsToMonitor == "left" || lightSensorsToMonitor == "right" || lightSensorsToMonitor == "both" || lightSensorsToMonitor == "no")
                {
                    sensorsToMonitor[0] = lightSensorsToMonitor;
                }                
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tDo you want to Monitor the Temperature? [yes, no]: ");
                string temperaturesToMonitor = Console.ReadLine().ToLower();

                if (temperaturesToMonitor == "yes" || temperaturesToMonitor == "no")
                {
                    sensorsToMonitor[1] = temperaturesToMonitor;
                }               
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");                    
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.CursorVisible = true;
                Console.Write("\n\tDo you want to Monitor the Obstacle sensor? [yes, no]: ");
                string obstacleToMonitor = Console.ReadLine().ToLower();

                if (obstacleToMonitor == "yes" || obstacleToMonitor == "no")
                {
                    sensorsToMonitor[2] = obstacleToMonitor;
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm System");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");                    
                }
            } while (!validResponse);
            
            return sensorsToMonitor;
        }

        #endregion

        #region DATA RECORDING

        /// <summary>
        /// *********************\\\
        ///  Data Recording Menu  \\\
        /// ***********************\\\
        /// </summary>
        /// <param name="myFinch">My finch.</param>
        /// <param name="userName">User name.</param>
        static void DisplayDataRecorderMenuScreen(Finch myFinch, string userName)
        {
            Console.CursorVisible = true;

            bool quitDataRecorderMenu = false;
            string fileName = null;

            int numberOfDataPoints = 0;
            int frequencyOfData = 0;
            int numberOfRows = 0;

            string menuChoice;

            double[][] temperaturesPerRow = null;
            
            do
            {
                DisplayScreenHeader("Data Sensor Grid Menu");
                Console.WriteLine("\t\t_________________________\n");
                //
                // get user menu choice
                //
                Console.WriteLine("\t\t(a): Get the number of Data Points Per Row");
                Console.WriteLine("\t\t(b): Get the timing between Data Points");
                Console.WriteLine("\t\t(c): Get the number of Rows for your grid");
                Console.WriteLine("\t\t(d): Fixed Sized Sensor Grid Array");
                Console.WriteLine("\t\t(e): Non Fixed Sized Sensor Grid Array");
                Console.WriteLine("\t\t(f): Access Data Files");
                Console.WriteLine("\t\t(q): Main Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();
                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        numberOfDataPoints = GetTheNumberOfDataPoints();
                        break;

                    case "b":
                        frequencyOfData = FrequencyOfDataPoints();
                        break;

                    case "c":
                        numberOfRows = NumberOfRows();
                        break;

                    case "d":
                        if (numberOfDataPoints == 0 || frequencyOfData == 0)
                        {
                            Console.WriteLine("Please add values to the Menu options (a) and (b)");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                        StaticGridArrayDataSet(numberOfDataPoints, frequencyOfData, myFinch, userName);
                        }
                        break;
                    
                    case "e":
                        if (numberOfDataPoints == 0 || frequencyOfData == 0 || numberOfRows == 0)
                        {
                            Console.WriteLine("\n\tPlease add values to the Menu options (a), (b), and (c)");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                            NonStaticSensorGridArray(numberOfRows, numberOfDataPoints, frequencyOfData, temperaturesPerRow, myFinch, userName);
                        }
                        break;
                    case "f":
                        AccessNamedDataFile(fileName, userName);
                        break;
                    case "q":
                        quitDataRecorderMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }                          

            } while (!quitDataRecorderMenu);
        }

        /// <summary>
        /// Accesses the named data file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="userName">User name.</param>
        static void AccessNamedDataFile(string fileName, string userName)
        {
            Console.Write("\n\tPlease typ file name: ");
            fileName = Console.ReadLine();

            if (File.Exists($@"{userName}/Data/{fileName}.txt"))
            {
                Console.WriteLine(File.ReadAllText($@"{userName}/Data/{fileName}.txt"));
            }
            else
            {
                Console.WriteLine("File not Found");
            }

            DisplayContinuePrompt();
        }

        /// <summary>
        /// Get The Static Data Set \\\
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static void StaticGridArrayDataSet(int numberOfDataPoints, int frequencyOfData, Finch myFinch, string userName)
        {
            Console.Clear();

            List<double[]> RowListOfArrays = new List<double[]>();

            Console.WriteLine("\n\tTime to build your Data Grid" +
                              "\n\t********* Example ********" +
                              "\n\t|-------|-------|-------|");
            Console.WriteLine  ("\t| Row 1 | Row 2 | Row 3 |" +
                              "\n\t|-------|-------|-------|" +
                              "\n\t| Temp1 | Temp1 | Temp1 |" +
                              "\n\t| Temp2 | Temp2 | Temp2 |" +
                              "\n\t| Temp3 | Temp3 | Temp3 |" +
                              "\n\t|-------|-------|-------|");

            Console.WriteLine($"\n\tThe Finch will make a Sensor Reading then move forward for {frequencyOfData}sec");
            Console.WriteLine("\n\tEach time the Finch makes a reading it will light up and make a 1 sec beep");
            Console.WriteLine($"\n\tNumber of Rows: {3}");
            Console.WriteLine($"\n\tNuber of Data Points: {numberOfDataPoints} per row.");
            Console.WriteLine($"\n\tTime between Data Points: {frequencyOfData}");                      

            StaticGridArrayGetDataSet(numberOfDataPoints, frequencyOfData, myFinch, out string fileName, userName);

            DisplayMenuPrompt("Data Recording");                                               
        }

        /// <summary>
        /// Stati Grid Array Get Data Set
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static double[][] StaticGridArrayGetDataSet(int numberOfDataPoints, int frequencyOfData, Finch myFinch, out string fileName, string userName)
        {
            double temperatureReading;
            bool validResponse = false;

            double[][] temperaturesPerRow = new double[3][];
            double[] temperatureReadingArray1 = new double[numberOfDataPoints];
            double[] temperatureReadingArray2 = new double[numberOfDataPoints];
            double[] temperatureReadingArray3 = new double[numberOfDataPoints];

            do
            {
                Console.CursorVisible = true;
                Console.Write("\n\tPlease name Your Data File: ");
                fileName = Console.ReadLine();

                if (fileName.Length > 2)
                {
                    validResponse = true;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.Write("\n\tPlease enter a File Name that is three letters long or more.\n");
                }
            } while (!validResponse);

            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {1}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray1[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[0] = temperatureReadingArray1;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);
            }

            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {2}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray2[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[1] = temperatureReadingArray2;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);
            }

            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {3}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray3[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[2] = temperatureReadingArray3;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);
            }

            Console.WriteLine("\n\tDone Collecting Data");

            RightDataToFile(temperaturesPerRow, fileName, userName, temperatureReadingArray1,
                 temperatureReadingArray2, temperatureReadingArray3);


            return temperaturesPerRow;
        }


        /// <summary>
        /// Rights the data to file.
        /// </summary>
        /// <param name="temperaturesPerRow">Temperatures per row.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="temperatureReadingArray1">Temperature reading array1.</param>
        /// <param name="temperatureReadingArray2">Temperature reading array2.</param>
        /// <param name="temperatureReadingArray3">Temperature reading array3.</param>
        static void RightDataToFile(double[][] temperaturesPerRow, string fileName, string userName, double[] temperatureReadingArray1,
             double[] temperatureReadingArray2, double[] temperatureReadingArray3)
        {
            string dataPath = $@"{userName}/Data/{fileName}.txt";
            string Time = $"\n\t{DateTime.UtcNow.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
            File.AppendAllText(dataPath, Time);

            string row = $"\n\tRow {1}";
            File.AppendAllText(dataPath, row);
            foreach (double temp in temperatureReadingArray1)
            {
                string Temperature = $"\n\tTemperature - {temp} ";
                File.AppendAllText(dataPath, Temperature);
            }
            row = $"\n\tRow {2}";
            File.AppendAllText(dataPath, row);
            foreach (double temp in temperatureReadingArray2)
            {
                string Temperature = $"\n\tTemperature - {temp} ";
                File.AppendAllText(dataPath, Temperature);
            }
            row = $"\n\tRow {3}";
            File.AppendAllText(dataPath, row);
            foreach (double temp in temperatureReadingArray3)
            {
                string Temperature = $"\n\tTemperature - {temp} ";
                File.AppendAllText(dataPath, Temperature);
            }

            Console.WriteLine(File.ReadAllText($@"{userName}/Data/{fileName}.txt"));
        }

        /// <summary>
        /// FrequencyOfDataPoints
        /// </summary>
        /// <returns></returns>
        static int FrequencyOfDataPoints()
        {
            Console.Clear();

            int frequencyOfData;
            bool validResponse;

            do
            {
                Console.WriteLine("\n\t\tFrequency of Data points");
                Console.Write("\n\tTime between Data Points in Seconds: ");
                string userResponse = Console.ReadLine();

                validResponse = true;

                if (int.TryParse(userResponse, out frequencyOfData))
                {

                    DisplayMenuPrompt("Data Recording Menu");
                }
                else
                {
                    validResponse = false;
                    Console.Write("\n\tPlease enter a valid number: ");
                }
            } while (!validResponse);
            
            return frequencyOfData;
        }

        /// <summary>
        /// GetTheNumberOfDataPoints
        /// </summary>
        static int GetTheNumberOfDataPoints()
        {
            Console.Clear();

            int numberOfDataPoints;
            bool validResponse;

            do
            {
                Console.WriteLine("\n\t\tGet the number of Data Points");

                Console.Write("\n\tNumber of Data Points needed?: ");
                string userResponse = Console.ReadLine();

                validResponse = true;

                if (int.TryParse(userResponse, out numberOfDataPoints))
                {

                    DisplayMenuPrompt("Data Recording");

                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number: ");

                }
            } while (!validResponse);          

            return numberOfDataPoints;
        }

        /// <summary>
        /// Sensors the grid array.
        /// </summary>
        /// <returns>The grid array.</returns>
        /// <param name="numberOfDataPoints">Number of data points.</param>
        /// <param name="frequencyOfData">Frequence of data points.</param>
        /// <param name="numberOfRows">Frequence of data points.</param>
        /// <param name="myFinch">My finch.</param>
        static List<double[]> NonStaticSensorGridArray(int numberOfRows, int numberOfDataPoints, int frequencyOfData, double[][] temperaturesPerRow, Finch myFinch, string userName)
        {
            List<double[]> RowListOfArrays = new List<double[]>();

            Console.Clear();

            Console.WriteLine("\n\tTime to build your Data Grid" +
                            "\n\n\t********* Example ********" +
                            "\n\n\t|-------|-------|-------|");
            Console.WriteLine  ("\t| Row 1 | Row 2 | Row 3 |" +
                              "\n\t|-------|-------|-------|" +
                              "\n\t| Temp1 | Temp1 | Temp1 |" +
                              "\n\t| Temp2 | Temp2 | Temp2 |" +
                              "\n\t| Temp3 | Temp3 | Temp3 |" +
                              "\n\t|-------|-------|-------|");

            Console.WriteLine($"\n\tThe Finch will make a Sensor Reading then move forward for {frequencyOfData}sec");
            Console.WriteLine("\n\tEach time the Finch makes a reading it will light up and make a 1 sec beep");
            Console.WriteLine($"\n\tNumber of Rows: {numberOfRows}, if you were using a none static grid array");
            Console.WriteLine($"\n\tNuber of Data Points: {numberOfDataPoints} per row.");
            Console.WriteLine($"\n\tTime between Data Points: {frequencyOfData}");

            DisplayContinuePrompt();

            NonStaticGridArrayGetDataSet(numberOfRows, numberOfDataPoints, frequencyOfData, myFinch);

            DisplayMenuPrompt("Data Set");

            return RowListOfArrays;
        }

        /// <summary>
        /// Static Grid Array Get Data Set \\\
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static double[][] NonStaticGridArrayGetDataSet(int numberOfRows, int numberOfDataPoints, int frequencyOfData, Finch myFinch)
        {
            double temperatureReading;
            int tempRow;

            double[][] temperaturesPerRow = new double[numberOfRows][];
            double[] temperatureReadingArray = new double[numberOfDataPoints];

            for (int i = 0; i < numberOfRows; i++)
            {
                Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {i + 1}.");

                DisplayContinuePrompt();

                for (int a = 0; a < numberOfDataPoints; a++)
                {
                    int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                    temperatureReading = myFinch.getTemperature();
                    temperatureReadingArray[a] = temperatureReading;                    
                    Console.WriteLine($"Temperature Reading { a + 1}: {temperatureReading}");
                    myFinch.setLED(255, 255, 255);
                    myFinch.noteOn(1000);
                    myFinch.wait(1000);
                    myFinch.setLED(0, 0, 0);
                    myFinch.noteOff();
                    myFinch.setMotors(200, 200);
                    myFinch.wait(frequencyOfdataPointsinMilSeconds);
                    myFinch.setMotors(0, 0);
                                        
                    Console.WriteLine("\n\tTo began next reading");

                    DisplayContinuePrompt();
                }

                temperaturesPerRow[i] = temperatureReadingArray;                
            }
            
            for (tempRow = 0; tempRow < temperaturesPerRow.Length; tempRow++)
            {
                Console.Write($"\n\tRow - {tempRow + 1}", temperaturesPerRow[tempRow]);

                foreach (double temp in temperatureReadingArray)
                {
                    Console.WriteLine($"\n\tTemperature - {temp} ");
                }
            }

            DisplayMenuPrompt("Data Recording");

            return temperaturesPerRow;
        }

        /// <summary>
        /// Number Of Rows in Grid Array \\\
        /// </summary>
        /// <returns></returns>
        static int NumberOfRows()
        {
            int numberOfRows;
            bool validResponse;

            do
            {
                validResponse = true;

                Console.Write("\n\tPlease enter how many rows you need for the Data Grid: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out numberOfRows))
                {
                    DisplayMenuPrompt("Data Recording");

                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number: ");
                }
            } while (!validResponse);
            
            return numberOfRows;
        }
        #endregion

        #region TALENT SHOW

        /// <summary>
        /// ******************\\\
        ///  Talent Show Menu  \\\
        /// ********************\\\
        /// </summary>
        static void DisplayTalentShowMenuScreen(Finch myFinch)
        {
            Console.CursorVisible = true;

            bool quitTalentShowMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Talent Show Menu");
                Console.WriteLine("\t\t_________________________\n");

                Console.WriteLine("\t\ta) Light and Sound");
                Console.WriteLine("\t\tb) Lets Dance");
                Console.WriteLine("\t\tc) ");
                Console.WriteLine("\t\td) ");
                Console.WriteLine("\t\tq) Main Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        DisplayLightAndSound(myFinch);
                        break;

                    case "b":
                        DisplayingItsDanceMoves(myFinch);
                        break;

                    case "c":

                        break;

                    case "d":

                        break;

                    case "q":
                        quitTalentShowMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitTalentShowMenu);
        }

        /// <summary>
        /// Displayings its dance moves.
        /// </summary>
        /// <param name="myFinch">My finch.</param>
        static void DisplayingItsDanceMoves(Finch myFinch)
        {
            Console.WriteLine("\n\tLets play Music then dance to it!");
            Console.Write("\n\tHow many dance moves do you want to see?:");
            string usersResponse = Console.ReadLine();
            Console.WriteLine($"\n\tYou choose {usersResponse} dance moves.");
            bool validResponse;

            do
            {
                validResponse = true;

                if (!int.TryParse(usersResponse, out int DanceMoves))
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number:");
                    usersResponse = Console.ReadLine();
                }

                else if (DanceMoves > 0)
                {
                    for (int i = 0; i < DanceMoves; i++)
                    {
                        myFinch.setLED(255, 0, 0);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 0, 255);
                        myFinch.noteOn(1567);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 255, 0);
                        myFinch.noteOn(1175);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 0, 0);
                    }

                    myFinch.noteOn(2049);
                    myFinch.wait(1000);
                    myFinch.noteOff();

                    Console.WriteLine("\n\tKnow lets Dance!");

                    for (int i = 0; i < DanceMoves; i++)
                    {
                        myFinch.setLED(255, 0, 0);
                        myFinch.setMotors(125, -125);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.noteOn(1567);
                        myFinch.wait(500);
                        myFinch.noteOn(1175);
                        myFinch.wait(500);
                        myFinch.setLED(0, 255, 0);
                        myFinch.setMotors(-125, 125);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.noteOn(1567);
                        myFinch.wait(500);
                        myFinch.noteOn(1175);
                        myFinch.wait(500);
                        myFinch.noteOff();
                    }
                    myFinch.setMotors(50, 50);
                    myFinch.noteOn(2049);
                    myFinch.wait(1000);
                    myFinch.noteOn(3135);
                    myFinch.wait(500);
                    myFinch.noteOn(2349);
                    myFinch.wait(500);
                    myFinch.setMotors(0, 0);
                    myFinch.noteOff();
                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number:");
                    usersResponse = Console.ReadLine();
                }

            } while (!validResponse);

            Console.CursorVisible = false;
            DisplayMenuPrompt("Talent Show");

        }

        /// <summary>
        /// Displaies the light and sound.
        /// </summary>
        /// <param name="myFinch">My finch.</param>
        static void DisplayLightAndSound(Finch myFinch)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Light and Sound");

            Console.WriteLine("\tThe Finch robot will now show off\n\tits glowing talent!");
            DisplayContinuePrompt();

            for (int lightSoundLevel = 0; lightSoundLevel < 255; lightSoundLevel++)
            {
                myFinch.setLED(lightSoundLevel, lightSoundLevel, lightSoundLevel);
                myFinch.noteOn(lightSoundLevel * 100);
            }

            DisplayMenuPrompt("Talent Show");
        }

        #endregion

        #region FINCH ROBOT MANAGEMENT

        /// <summary>
        /// *****************************************************************
        /// *               Disconnect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        static void DisplayDisconnectFinchRobot(Finch myFinch)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("\n\tAbout to disconnect from the Finch robot.");
            DisplayContinuePrompt();

            myFinch.disConnect();

            Console.WriteLine("\n\tThe Finch robot is now disconnect.");

            DisplayMenuPrompt("Main Menu");
        }

        /// <summary>
        /// *****************************************************************
        /// *                  Connect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        /// <returns>notify if the robot is connected</returns>
        static bool DisplayConnectFinchRobot(Finch myFinch)
        {
            Console.CursorVisible = false;

            bool robotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine("\tAbout to connect to Finch robot.\n\n\tPlease be sure the USB cable is connected\n\n\tto the robot and computer now.");
            DisplayContinuePrompt();

            robotConnected = myFinch.connect();

            myFinch.noteOn(1265);
            myFinch.wait(250);
            myFinch.noteOff();
            myFinch.setLED(255, 0, 0);
            myFinch.wait(250);
            myFinch.setLED(0, 0, 255);
            myFinch.wait(250);
            myFinch.setLED(0, 255, 0);
            myFinch.wait(250);
            myFinch.setLED(0, 0, 0);

            // TODO test connection and provide user feedback - text, lights, sounds

            Console.CursorVisible = false;
            DisplayMenuPrompt("Main Menu");

            //
            // reset finch robot
            //

            myFinch.setLED(0, 0, 0);
            myFinch.noteOff();

            return robotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// *****************************************************************
        /// *                     Welcome Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Closing Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display menu prompt
        /// </summary>
        static void DisplayMenuPrompt(string menuName)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPress any key to return to the {menuName} Menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>

        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}