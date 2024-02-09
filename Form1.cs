/*
 * Team 6
 * Members name - Sujal Thakur (2529583) , Khushaal Choithramani (2533883) , Sanskar Basnet (2540525)
 * Module Code - AC22005
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
namespace MineSweeper
{
    public partial class Form1 : Form
    {
        // Declare variables
        WindowsMediaPlayer backgroundMusicPlayer = new WindowsMediaPlayer(); // Used for playing background music
        private SoundPlayer OnClickSound; // Sound player for button click sound
        private Panel pnlMenu = new Panel(); // Panel for the main menu
        private Panel pnlDifficultyLevel = new Panel(); // Panel for selecting difficulty level
        private Panel pnlGameStart = new Panel(); // Panel for starting the game
        private Panel pnlGameOver = new Panel(); // Panel for displaying game over message
        private Panel pnlWin = new Panel(); // Panel for displaying win message
        private Panel pnlHighestScore = new Panel(); // Panel for displaying highest score
        private Panel pnlHelp = new Panel(); // Panel for displaying help/instructions
        private Button[,] btnEsyLvlTiles = new Button[10, 8]; // Array of buttons for easy level tiles
        private Button[,] btnMediumLvlTiles = new Button[18, 12]; // Array of buttons for medium level tiles
        private Button[,] btnHardLvlTiles = new Button[24, 15]; // Array of buttons for hard level tiles
        private bool esyLvl = false; // Flag for easy level
        private bool emediumLvl = false; // Flag for medium level
        private bool ehardLvl = false; // Flag for hard level
        private bool firstButtonPressed = true; // Flag to check if the first button is pressed
        private int unrevealedTiles = 0; // Counter for unrevealed tiles
        private int numOfBoomTiles = 0; // Counter for number of bomb tiles
        private Label time = new Label(); // Label for displaying elapsed time
        private Label NoOfFlag = new Label(); // Label for displaying number of flags
        private int elapsedTimeInSeconds; // Counter for time in seconds
        Timer gameTimer; // Timer for tracking game time
        String currentLine; // String variable to store current line of text

        public Form1()
        {
            InitializeComponent();

            //BackGround Music
            backgroundMusicPlayer.URL = "BackgroundMusic.wav"; // Provide the correct path to your music file

            //Button Click Sound
            OnClickSound = new SoundPlayer();
            OnClickSound.SoundLocation = "ButtonClick.wav";

            // Initialize panels
            Panel_Help(ref pnlHelp); // Panel for displaying help/instructions
            Panel_Menu(ref pnlMenu); // Panel for the main menu
            Panel_difficultyLevel(ref pnlDifficultyLevel); // Panel for selecting difficulty level
            Panel_HighestScore(ref pnlHighestScore); // Panel for displaying highest score

            // Add panels to the form
            Controls.Add(pnlHelp);
            Controls.Add(pnlHighestScore);
            Controls.Add(pnlDifficultyLevel);
            Controls.Add(pnlMenu);
            Controls.Add(pnlGameStart);
        }

        private void Panel_Menu(ref Panel pnl)
        {
            //Set label
            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome to Minesweeper";
            setLabel(lblWelcome);

            //Set Start Button
            Button btnStartGame = new Button();
            btnStartGame.SetBounds(300, 100, 200, 90);
            btnStartGame.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnStartGame.Click += new EventHandler(this.btnStartGameEvent_Click);
            btnStartGame.Text = "Start";
            setButton(btnStartGame);

            //Set Highest Score
            Button btnHighestScore = new Button();
            btnHighestScore.SetBounds(250, 200, 300, 90);
            btnHighestScore.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(300, 90));
            btnHighestScore.Click += new EventHandler(this.btnHighestScore_Click);
            btnHighestScore.Text = "Highest Score";
            setButton(btnHighestScore);

            //Set Help Button
            Button btnHelp = new Button();
            btnHelp.SetBounds(300, 300, 200, 90);
            btnHelp.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnHelp.Click += new EventHandler(this.btnHelpEvent_Click);
            btnHelp.Text = "Help";
            setButton(btnHelp);

            //Set Exit Button
            Button btnExitGame = new Button();
            btnExitGame.SetBounds(300, 400, 200, 90);
            btnExitGame.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnExitGame.Click += new EventHandler(this.btnExitGameEvent_Click);
            btnExitGame.Text = "Exit";
            setButton(btnExitGame);

            //adding to main pnl
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Controls.Add(lblWelcome);
            pnl.Controls.Add(btnStartGame);
            pnl.Controls.Add(btnHighestScore);
            pnl.Controls.Add(btnHelp);
            pnl.Controls.Add(btnExitGame);

            pnl.Visible = true;
        }
        private void Panel_HighestScore(ref Panel pnl)
        {
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            
            //Set Highest Score Label
            Label lblHighestScore = new Label();
            lblHighestScore.Text = "Highest Score";
            setLabel(lblHighestScore);

            //Set back Button
            Button btnBackButton2 = new Button();
            btnBackButton2.SetBounds(20, 30, 80, 70);
            btnBackButton2.Image = new Bitmap(Properties.Resources.BackButton, new Size(90, 70));
            btnBackButton2.Click += new EventHandler(this.btnBackButton2Event_Click);
            setButton(btnBackButton2);
            pnl.Controls.Add(btnBackButton2);
            pnl.Controls.Add(lblHighestScore);

            //Creating new panel in which we can store the score
            TableLayoutPanel tablePanel = new TableLayoutPanel();
            tablePanel.Size = new Size(600, 400);
            tablePanel.Location = new Point((pnl.Width - tablePanel.Width) / 2,
                (pnl.Height + 40 - tablePanel.Height) / 2);
            tablePanel.BackColor = Color.AliceBlue;

            // Add column styles
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,10));   // Rank
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,10));   // Time
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,6));   // Difficulty
            //reading from file
            try
            {
                string[] lines = File.ReadAllLines("HighestScore.txt");
                //checking if there is no highest score then display something else
                if(lines.Length == 0)
                {
                    Label emptyLbl = new Label();
                    emptyLbl.Text = "Empty leaderboard! Victory awaits – will you be the first to conquer? Keep playing to claim the top spot!";
                    setLabel(emptyLbl,110,150,36,300);
                    emptyLbl.BackColor = Color.AliceBlue;
                    emptyLbl.ForeColor = Color.Black;
                    pnl.Controls.Add(emptyLbl);
                    pnl.Visible = false;
                    return;
                }
                //sorting fileData from highest score to lowest for score(using bubble sort)
                bool doneSorting;
                do
                {
                    doneSorting = true;
                    TimeSpan previousTime = TimeSpan.MaxValue;
                    string previousLine = "";
                    for (int i = 0; i < lines.Length; i++)  
                    {
                        string line = lines[i];
                        string[] parts = line.Split(',');
                        TimeSpan time = TimeSpan.Parse(parts[0]);
                        if (i == 0)
                        {
                            previousTime = time;
                            previousLine = line;
                            continue;
                        }
                        if (time < previousTime)
                        {
                            lines[i] = previousLine;
                            lines[i - 1] = line;
                            doneSorting = false;
                        }
                        previousTime = time;
                        previousLine = line;
                    }
                } while (!doneSorting);

                // Set up TableLayoutPanel properties
                tablePanel.RowCount = lines.Length;
                tablePanel.ColumnCount = 3;

                // Add headers
                Label rankHeader = new Label();
                rankHeader.Text = "Rank";
                rankHeader.Font = new Font("Times New Roman", 22, FontStyle.Bold);
                rankHeader.ForeColor = Color.Black; // Set the text color
                rankHeader.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                rankHeader.Size = new Size(120, 80);

                Label timeHeader = new Label();
                timeHeader.Text = "Time";
                timeHeader.Font = new Font("Times New Roman", 22, FontStyle.Bold);
                timeHeader.ForeColor = Color.Black; // Set the text color
                timeHeader.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                timeHeader.Size = new Size(120, 80);

                Label difficultyHeader = new Label();
                difficultyHeader.Text = "Difficulty";
                difficultyHeader.Font = new Font("Times New Roman", 22, FontStyle.Bold);
                difficultyHeader.ForeColor = Color.Black; // Set the text color
                difficultyHeader.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                difficultyHeader.Size = new Size(140, 80);

                tablePanel.Controls.Add(rankHeader, 0, 0);
                tablePanel.Controls.Add(timeHeader, 1, 0);
                tablePanel.Controls.Add(difficultyHeader, 2, 0);

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] parts = line.Split(',');
                    // Create labels for each column
                    Label rankLabel = new Label();
                    rankLabel.Text = i + 1 + "";
                    rankLabel.BackColor = Color.Transparent;
                    rankLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    rankLabel.ForeColor = Color.Black; // Set the text color
                    rankLabel.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                    rankLabel.Padding = new Padding(25, 0, 0, 0);

                    Label timeLabel = new Label();
                    timeLabel.Text = parts[0];
                    timeLabel.BackColor = Color.Transparent;
                    timeLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    timeLabel.ForeColor = Color.Black; // Set the text color
                    timeLabel.TextAlign = ContentAlignment.MiddleCenter;
                    timeLabel.Size = new Size(125, 30);

                    Label difficultyLabel = new Label();
                    difficultyLabel.Text = parts[1];
                    difficultyLabel.BackColor = Color.Transparent;
                    difficultyLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    difficultyLabel.ForeColor = Color.Black; // Set the text color
                    difficultyLabel.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                    difficultyLabel.Size = new Size(125, 30);


                    tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                    tablePanel.Controls.Add(rankLabel, 0, i + 1);
                    tablePanel.Controls.Add(timeLabel, 1, i + 1);
                    tablePanel.Controls.Add(difficultyLabel, 2, i + 1);
                }
                tablePanel.AutoScroll = true;
                tablePanel.Visible = true;
                pnl.Controls.Add(tablePanel);
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                Label error = new Label();
                setLabel(error,110,200,34,200);
                error.Text = ex.Message;
                error.BackColor = Color.Black;
                pnl.Controls.Add(error);
            }
            
            pnl.Visible = false;
        }
        private void Panel_difficultyLevel(ref Panel pnl)
        {
            //set difficulty level
            Label lblDifficulty = new Label();
            lblDifficulty.Text = "Difficulty Level";
            setLabel(lblDifficulty);

            //Set Easy level
            Button btnEasyLevel = new Button();
            btnEasyLevel.SetBounds(300, 100, 200, 90);
            btnEasyLevel.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnEasyLevel.Click += new EventHandler(this.btnEasyLevelEvent_Click);
            btnEasyLevel.Text = "Easy";
            setButton(btnEasyLevel);

            //Set Medium level Button
            Button btnMediumLevel = new Button();
            btnMediumLevel.SetBounds(300, 230, 200, 90);
            btnMediumLevel.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnMediumLevel.Click += new EventHandler(this.btnMediumLevelEvent_Click);
            btnMediumLevel.Text = "Medium";
            setButton(btnMediumLevel);

            //Set Hard level Button
            Button btnHardLevel = new Button();
            btnHardLevel.SetBounds(300, 370, 200, 90);
            btnHardLevel.Image = new Bitmap(Properties.Resources.ButtonBackGround, new Size(215, 90));
            btnHardLevel.Click += new EventHandler(this.btnHardLevelEvent_Click);
            btnHardLevel.Text = "Hard";
            setButton(btnHardLevel);

            //Set back Button
            Button btnBackButton = new Button();
            btnBackButton.SetBounds(20, 30, 80, 70);
            btnBackButton.Image = new Bitmap(Properties.Resources.BackButton, new Size(90, 70));
            btnBackButton.Click += new EventHandler(this.btnBackButtonEvent_Click);
            setButton(btnBackButton);

            //adding everything in difficulty level panel 
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Controls.Add(lblDifficulty);
            pnl.Controls.Add(btnEasyLevel);
            pnl.Controls.Add(btnHardLevel);
            pnl.Controls.Add(btnMediumLevel);
            pnl.Controls.Add(btnBackButton);
            pnl.Visible = false;
        }
        private void Panel_Help(ref Panel pnl)
        {
            pnl.Size = this.ClientSize;

            //Set Highest Score Label
            Label lblHighestScore = new Label();
            lblHighestScore.Text = "Help";
            setLabel(lblHighestScore);

            //Set back Button
            Button btnBackButton3 = new Button();
            btnBackButton3.SetBounds(20, 30, 80, 70);
            btnBackButton3.Image = new Bitmap(Properties.Resources.BackButton, new Size(90, 70));
            btnBackButton3.Click += new EventHandler(this.btnBackButton3Event_Click);
            setButton(btnBackButton3);

            Panel insidePanel = new Panel();
            insidePanel.Size = new Size(600, 400);
            insidePanel.Location = new Point((pnl.Width - insidePanel.Width) / 2,
                (pnl.Height + 40 - insidePanel.Height) / 2);
            insidePanel.BackColor = Color.AliceBlue;
            insidePanel.AutoScroll = true;

            // How to play label
            Label lblHowToPlay = new Label();
            lblHowToPlay.Text = "How To Play";
            setLabel(lblHowToPlay, 1, 5, 26);
            lblHowToPlay.ForeColor = Color.Black;

            // Objective Label
            Label lblObjective = new Label();
            lblObjective.Text = "Objective:";
            Label lblObjectiveText = new Label();
            lblObjectiveText.Text = "Navigate the minefield without detonating any mines. Use the numbers provided to strategically reveal squares and flag potential mines.";
            setLabel2(lblObjective, lblObjectiveText);

            // Game Mechanics Label
            Label lblGameMechanics = new Label();
            lblGameMechanics.Text = "Game Mechanics:";
            Label lblGameMechanicsText = new Label();
            lblGameMechanicsText.Text = "- Left-click a square to reveal it. The number displayed indicates how many adjacent squares contain mines.\n- Right-click a square to flag it as a potential mine.\n- Uncovering a mine ends the game.\n- To win, correctly flag all mines and reveal all safe squares.";
            setLabel2(lblGameMechanics, lblGameMechanicsText,150,200,120);

            // Controls Label
            Label lblControls = new Label();
            lblControls.Text = "Controls:";
            Label lblControlsText = new Label();
            lblControlsText.Text = "- Left-click: Reveal square\n- Right-click: Flag square";
            setLabel2(lblControls, lblControlsText, 310, 360, 50);

            // Difficulty Levels Label
            Label lblDifficultyLevels = new Label();
            lblDifficultyLevels.Text = "Difficulty Levels:";
            Label lblDifficultyLevelsText = new Label();
            lblDifficultyLevelsText.Text = "- Easy: 10 mines on a 10x8 grid\n- Medium: 40 mines on a 18x12 grid\n- Hard: 99 mines on a 24x15 grid";
            setLabel2(lblDifficultyLevels, lblDifficultyLevelsText, 410, 460, 80);

            // Tips and Strategies Label
            Label lblTipsStrategies = new Label();
            lblTipsStrategies.Text = "Tips and Strategies:";
            Label lblTipsStrategiesText = new Label();
            lblTipsStrategiesText.Text = "- Start with Safety: Begin by uncovering squares in the corners or along\n edges to minimize the risk of hitting a mine.\n- Logic is Key: Use the numbers revealed to deduce the positions of mines.\n If a square has a '1' nearby and only one unrevealed adjacent square, it's\n likely a mine.\n- Flag with Caution: Misflagging squares can lead to errors. Ensure your \nflag placements are based on solid reasoning.";
            setLabel2(lblTipsStrategies, lblTipsStrategiesText,  530, 580, 150);

            // Add labels to inside panel
            insidePanel.Controls.Add(lblHowToPlay);
            insidePanel.Controls.Add(lblObjective);
            insidePanel.Controls.Add(lblObjectiveText);
            insidePanel.Controls.Add(lblGameMechanics);
            insidePanel.Controls.Add(lblGameMechanicsText);
            insidePanel.Controls.Add(lblControls);
            insidePanel.Controls.Add(lblControlsText);
            insidePanel.Controls.Add(lblDifficultyLevels);
            insidePanel.Controls.Add(lblDifficultyLevelsText);
            insidePanel.Controls.Add(lblTipsStrategies);
            insidePanel.Controls.Add(lblTipsStrategiesText);

            //adding everything to main panel
            pnl.BackColor = Color.Transparent;
            pnl.Controls.Add(lblHighestScore);
            pnl.Controls.Add(btnBackButton3);
            pnl.Controls.Add(insidePanel);
            pnl.Visible = false;
        }
        private void Panel_GameStart(ref Panel pnl, Button[,] btnTiles,
            int btnSize = 50, int BoardWidth = 500, int BoardHeight = 400, int Position = 50
            )
        {
            //setting panel
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Visible = false;
            TableLayoutPanel tableLayoutPanel = pnl.Controls.OfType<TableLayoutPanel>().FirstOrDefault();

            //adding falg image at the top
            PictureBox flagImage = new PictureBox();
            flagImage.Image = Properties.Resources.Flag;
            flagImage.Location = new Point(150, 10);
            flagImage.Size = new Size(70, 70);
            flagImage.SizeMode = PictureBoxSizeMode.StretchImage;

            NoOfFlag.Text = numOfBoomTiles + "";
            NoOfFlag.Size = new Size(120, 70);
            NoOfFlag.Location = new Point(220, 10);
            NoOfFlag.BackColor = Color.Transparent;
            NoOfFlag.TextAlign = ContentAlignment.MiddleLeft;
            NoOfFlag.Font = new Font("Times New Roman", 40, FontStyle.Bold);

            // adding mute and unmute button
            Button muteBtn = new Button();
            muteBtn.Image = new Bitmap(Properties.Resources.Unmute, new Size(60, 70));
            muteBtn.SetBounds(670,10, 80, 70);
            muteBtn.FlatStyle = FlatStyle.Flat; // Add this line to make it flat
            muteBtn.FlatAppearance.BorderSize = 0;
            muteBtn.FlatAppearance.MouseOverBackColor = Color.LightSeaGreen;
            muteBtn.Tag = "UnMute";
            muteBtn.Click += new EventHandler(this.muteBtnEvent_Click);

            //adding clock
            PictureBox ClockImage = new PictureBox();
            ClockImage.Image = Properties.Resources.Clock;
            ClockImage.Location = new Point(350, 10);
            ClockImage.Size = new Size(70, 70);
            ClockImage.SizeMode = PictureBoxSizeMode.StretchImage;
            
            time.SetBounds(420, 10, 230, 70);
            time.TextAlign = ContentAlignment.MiddleLeft;
            time.Font = new Font("Times New Roman", 40, FontStyle.Bold);
            time.Text = "00:00:00";
            gameTimer = new Timer();
            gameTimer.Interval = 1000; // 1 second interval
            gameTimer.Tick += GameTimer_Tick; // Add event handler

            if (tableLayoutPanel != null)
            {
                // Remove the tableLayoutPanel from the Controls collection of the myPanel
                pnl.Controls.Remove(tableLayoutPanel);
            }

            Button btnBackButtonInGame = new Button();
            btnBackButtonInGame.SetBounds(20, 10, 80, 70);
            btnBackButtonInGame.Image = new Bitmap(Properties.Resources.BackButton, new Size(90, 70));
            btnBackButtonInGame.Click += new EventHandler(this.btnBackButtonInGameEvent_Click);
            setButton(btnBackButtonInGame);


            //setting GameBoard
            TableLayoutPanel pnlGameBoard = new TableLayoutPanel();
            pnlGameBoard.Size = new Size(BoardWidth, BoardHeight);
            pnlGameBoard.Location = new Point((pnl.Width - pnlGameBoard.Width) / 2,
                (pnl.Height + Position - pnlGameBoard.Height) / 2);
            pnlGameBoard.BackColor = Color.FromArgb(150, Color.AliceBlue);

            int Row = btnTiles.GetLength(1);
            int Column = btnTiles.GetLength(0);

            pnlGameBoard.RowCount = Row;
            pnlGameBoard.ColumnCount = Column;
            //setting the tiles
            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    btnTiles[i, j] = new Button();
                    btnTiles[i, j].Name = ", " + i + " " + j;
                    btnTiles[i, j].Size = new Size(btnSize, btnSize);
                    btnTiles[i, j].Margin = new Padding(0);
                    btnTiles[i, j].FlatStyle = FlatStyle.Flat;
                    Image img = Properties.Resources.Tile;
                    Bitmap resizedImage = new Bitmap(img, new Size(btnTiles[i, j].Width + 15, btnTiles[i, j].Height + 15));
                    btnTiles[i, j].Image = resizedImage;
                    btnTiles[i, j].Click += (sender, e) => btnTileEvent_Click(sender, e, btnTiles);
                    btnTiles[i, j].MouseUp += new MouseEventHandler(btnTileEvent_MouseUp);
                    pnlGameBoard.Controls.Add(btnTiles[i, j], i, j);
                }
            }

            pnl.Controls.Add(btnBackButtonInGame);
            pnl.Controls.Add(NoOfFlag);
            pnl.Controls.Add(flagImage);
            pnl.Controls.Add(ClockImage);
            pnl.Controls.Add(time);
            pnl.Controls.Add(muteBtn);
            pnl.Controls.Add(pnlGameBoard);
            pnl.Visible = true;
        }
        private void Panel_GameOver(ref Panel pnl,string text,String currentLine)
        {
            pnl.Visible = false;
            pnl.SetBounds(100,70,600,400);
            pnl.BackColor = Color.FromArgb(150, Color.AliceBlue);

            //adding clock
            PictureBox ClockImage = new PictureBox();
            ClockImage.Image = Properties.Resources.Clock;
            ClockImage.Location = new Point(120, 120);
            ClockImage.Size = new Size(100, 100);
            ClockImage.SizeMode = PictureBoxSizeMode.StretchImage;
            ClockImage.BackColor = Color.Transparent;

            //adding timer
            Label timelbl = new Label();
            timelbl.SetBounds(270, 130, 290, 70);
            timelbl.TextAlign = ContentAlignment.MiddleCenter;
            timelbl.Font = new Font("Times New Roman", 50, FontStyle.Bold);
            timelbl.Text = currentLine;
            currentLine = "";
            Label lblGameOver = new Label();
            setLabel(lblGameOver,10);
            lblGameOver.Text = text;
            lblGameOver.ForeColor = Color.Black;

            //adding playagain button and go home button
            Button btnPlayAgain = new Button();
            Button btnGoHome = new Button();

            btnPlayAgain.SetBounds(350, 280, 100, 100);
            btnPlayAgain.Padding = new Padding(10, 0, 0, 0);
            btnPlayAgain.Image = new Bitmap(Properties.Resources.PlayAgain, new Size(150, 120));
            btnPlayAgain.Click += new EventHandler(this.btnPlayAgain_Click);
            setButton(btnPlayAgain);

            btnGoHome.SetBounds(150, 280, 100, 100);
            btnGoHome.Image = new Bitmap(Properties.Resources.GoHome, new Size(100, 100));
            btnGoHome.Click += new EventHandler(this.btnGoHome_Click);
            setButton(btnGoHome);

            //adding everything to main panel
            pnl.Controls.Add(ClockImage);
            pnl.Controls.Add(timelbl);
            pnl.Controls.Add(btnPlayAgain);
            pnl.Controls.Add(btnGoHome);
            pnl.Controls.Add(lblGameOver);
        }
        private void ShowGameOverPanel()
        {

            // Show the game over panel
            pnlGameOver.Dispose();
            Controls.Remove(pnlGameOver);
            pnlGameOver = new Panel();
            Panel_GameOver(ref pnlGameOver, "Game Over","---------");
            Controls.Add(pnlGameOver);
            Controls.SetChildIndex(pnlGameOver, 0);
            pnlGameOver.Visible = true;

        }

        //This event handler is changing the time of the timer
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTimeInSeconds++;

            // Calculate hours, minutes, and seconds
            int hours = elapsedTimeInSeconds / 3600;
            int minutes = (elapsedTimeInSeconds % 3600) / 60;
            int seconds = elapsedTimeInSeconds % 60;

            // Update label text
            time.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            currentLine = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        //event handler for mute and unmute button
        private void muteBtnEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            Button button = (Button)sender;
            if(button.Tag == "UnMute")
            {
                button.Image = new Bitmap(Properties.Resources.Mute, new Size(60, 70));
                backgroundMusicPlayer.controls.pause();
                button.Tag = "Mute";
            }
            else
            {
                button.Image = new Bitmap(Properties.Resources.Unmute, new Size(60, 70));
                backgroundMusicPlayer.controls.play();
                button.Tag = "UnMute";
            }
        }

        //event handler to go back
        private void btnBackButtonInGameEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            gameTimer.Stop();
            pnlGameStart.Visible = false;
            pnlDifficultyLevel.Visible = true;
        }

        //Event handler to show the flag when right click pressed
        private void btnTileEvent_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btnTile = (Button)sender;
                if(btnTile.Tag != "Used")
                {
                    Image img = Properties.Resources.TileWithFlag;
                    Bitmap resizedImage = new Bitmap(img, new Size(btnTile.Width + 15, btnTile.Height + 15));
                    if (btnTile.Image.Tag == "Flag")
                    {
                        Image img1 = Properties.Resources.Tile;
                        Bitmap resizedImage1 = new Bitmap(img1, new Size(btnTile.Width + 15, btnTile.Height + 15));
                        btnTile.Image = resizedImage1;
                        btnTile.Image.Tag = "No Flag";
                        int num = Convert.ToInt32(NoOfFlag.Tag) + 1;
                        NoOfFlag.Tag = num;
                        NoOfFlag.Text = NoOfFlag.Tag + "";
                    }
                    else
                    {
                        int num = Convert.ToInt32(NoOfFlag.Tag) - 1;
                        NoOfFlag.Tag = num;
                        NoOfFlag.Text = NoOfFlag.Tag + "";
                        btnTile.Image = resizedImage;
                        btnTile.Image.Tag = "Flag";
                    }
                }
            }
        }

        //Set hard level
        private void btnHardLevelEvent_Click(object sender, EventArgs e)
        {
            esyLvl = false;
            emediumLvl = false;
            ehardLvl = true;
            firstButtonPressed = true;
            PlayButtonClickSound();
            elapsedTimeInSeconds = 0;
            pnlDifficultyLevel.Visible = false;
            unrevealedTiles = 360;
            numOfBoomTiles = 99;
            NoOfFlag.Text = numOfBoomTiles + "";
            NoOfFlag.Tag = numOfBoomTiles;
            Panel_GameStart(ref pnlGameStart, btnHardLvlTiles,30,720,450,80);
            pnlGameStart.Visible = true;
        }

        //Set Medium level
        private void btnMediumLevelEvent_Click(object sender, EventArgs e)
        {
            esyLvl = false;
            emediumLvl = true;
            firstButtonPressed = true;
            ehardLvl = false;
            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            unrevealedTiles = 216;
            elapsedTimeInSeconds = 0;
            numOfBoomTiles = 40;
            NoOfFlag.Text = numOfBoomTiles + "";
            NoOfFlag.Tag = numOfBoomTiles;
            Panel_GameStart(ref pnlGameStart, btnMediumLvlTiles,36,648,432,80);
            pnlGameStart.Visible = true;
        }

        //Set Easy level
        private void btnEasyLevelEvent_Click(object sender, EventArgs e)
        {
            esyLvl = true;
            emediumLvl = false;
            ehardLvl = false;
            firstButtonPressed = true;
            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            unrevealedTiles = 80;
            numOfBoomTiles = 10;
            NoOfFlag.Text = numOfBoomTiles + "";
            elapsedTimeInSeconds = 0;
            NoOfFlag.Tag = numOfBoomTiles;
            Panel_GameStart(ref pnlGameStart,btnEsyLvlTiles);
            pnlGameStart.Visible = true;
        }

        //This function is use to find the best place for booms
        bool IsPositionValid(int row, int col, int targetX, int targetY)
        {
            int minDistance = 3;

            int distance = Math.Abs(row - targetX) + Math.Abs(col - targetY);

            return distance >= minDistance;
        }

        //Main event handler for tiles
        private async void btnTileEvent_Click(object sender, EventArgs e, Button[,] btnTiles)
        {
            Button btnTile = (Button)sender;
            if (btnTile.Image.Tag != "Flag")
            {
                int Row = btnTiles.GetLength(1);
                int Column = btnTiles.GetLength(0);
                if (firstButtonPressed)
                {
                    gameTimer.Start();
                    string naame = btnTile.Name;
                    string[] paarts = naame.Split(' ');
                    int xPosition = int.Parse(paarts[2]);
                    int yPosition = int.Parse(paarts[1]);
                    //unique position for boom
                    HashSet<Tuple<int, int>> uniquePositions = new HashSet<Tuple<int, int>>();
                    Random random = new Random();
                    for (int i = 0; i < numOfBoomTiles; i++)
                    {
                        int randomRow, randomCol;

                        // Generate unique random position
                        do
                        {
                            randomRow = random.Next(Row - 1);
                            randomCol = random.Next(Column - 1);
                        } while (!IsPositionValid(randomRow, randomCol, xPosition, yPosition) || !uniquePositions.Add(new Tuple<int, int>(randomRow, randomCol)));
                        btnTiles[randomCol, randomRow].Name = "0";
                    }

                    //checking how many boom are attached to tile
                    for (int i = 0; i < Column; i++)
                    {
                        for (int j = 0; j < Row; j++)
                        {
                            if (!btnTiles[i, j].Name.StartsWith("0"))
                            {
                                int x, y, boomNearBy = 0;
                                for (int k = -1; k < 2; k++)
                                {
                                    for (int l = -1; l < 2; l++)
                                    {
                                        if (i + k < 0 || i + k > Column - 1 || j + l < 0 || j + l > Row - 1)
                                        {
                                            continue;
                                        }
                                        if (btnTiles[i + k, j + l].Name.StartsWith("0"))
                                        {
                                            boomNearBy++;
                                        }
                                    }
                                }

                                if (boomNearBy != 0)
                                {
                                    btnTiles[i, j].Name = boomNearBy + " " + i + " " + j;
                                }
                                else
                                {
                                    btnTiles[i, j].Name = "Safe " + i + " " + j;
                                }
                            }
                            else
                            {
                                btnTiles[i, j].Name += " " + i + " " + j;
                            }
                        }
                    }
                    firstButtonPressed = false;
                }

                String name = btnTile.Name;
                String[] parts = name.Split(' ');
                //if player revels the boom
                if (parts[0] == "0")
                {
                    gameTimer.Stop();
                    elapsedTimeInSeconds = 0;
                    ShowGameOverPanel();
                }
                int col = int.Parse(parts[1]);
                int row = int.Parse(parts[2]);


                // Recursive method to reveal safe tiles
                void RevealSafeTiles(int c, int r)
                {
                    // Base case: check bounds
                    if (c < 0 || c >= btnTiles.GetLength(0) || r < 0 || r >= btnTiles.GetLength(1))
                    {
                        return;
                    }

                    if (btnTiles[c, r].Tag == "Used")
                    {
                        return;
                    }
                    if (btnTiles[c, r].Image.Tag == "Flag")
                    {
                        int num = Convert.ToInt32(NoOfFlag.Tag) + 1;
                        NoOfFlag.Tag = num;
                        NoOfFlag.Text = NoOfFlag.Tag + "";
                    }

                    string naame = btnTiles[c, r].Name;
                    string[] naaame = naame.Split(' ');
                    //display the appropriate image when the tile is reveled 
                    Image Image;
                    if (naaame[0] == "Safe")
                        Image = Properties.Resources.RemovedTile;
                    else if (naaame[0] == "1")
                        Image = Properties.Resources.RemovedTile1;
                    else if (naaame[0] == "2")
                        Image = Properties.Resources.RemovedTile2;
                    else if (naaame[0] == "3")
                        Image = Properties.Resources.RemovedTile3;
                    else if (naaame[0] == "4")
                        Image = Properties.Resources.RemovedTile4;
                    else if (naaame[0] == "5")
                        Image = Properties.Resources.RemovedTile5;
                    else if (naaame[0] == "6")
                        Image = Properties.Resources.RemovedTile6;
                    else if (naaame[0] == "7")
                        Image = Properties.Resources.RemovedTile7;
                    else if (naaame[0] == "8")
                        Image = Properties.Resources.RemovedTile8;
                    else
                        Image = Properties.Resources.RemovedTileBoom;
                    
                    Bitmap resizedImagee = new Bitmap(Image, new Size(btnTiles[c, r].Width + 50, btnTiles[c, r].Height + 10));
                    btnTiles[c, r].Image = resizedImagee;

                    btnTiles[c, r].Tag = "Used";
                    unrevealedTiles--;

                    if (parts[0] == "Safe")
                    {
                        // Recursive call for neighboring tiles
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                string n = btnTiles[c, r].Name;
                                string[] p = n.Split(' ');
                                if (p[0].Contains("Safe"))
                                {
                                    RevealSafeTiles(c + i, r + j);
                                }
                            }
                        }
                    }


                }

                RevealSafeTiles(col, row);
                //checking if user win the game or not
                if (unrevealedTiles == numOfBoomTiles)
                {
                    gameTimer.Stop();
                    elapsedTimeInSeconds = 0;
                    string filePath = "HighestScore.txt";
                    string textToWrite;
                    if (esyLvl)
                        textToWrite = currentLine + ",Easy";
                    else if (ehardLvl)
                        textToWrite = currentLine + ",Hard";
                    else
                        textToWrite = currentLine + ",Medium";
                    Console.WriteLine(textToWrite);
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(textToWrite);
                    }

                    pnlWin.Dispose();
                    Controls.Remove(pnlWin);
                    pnlWin = new Panel();
                    Panel_GameOver(ref pnlWin, "You Win", currentLine);
                    Controls.Add(pnlWin);
                    Controls.SetChildIndex(pnlWin, 0);
                    pnlWin.Visible = true;

                    pnlHighestScore.Dispose();
                    Controls.Remove(pnlHighestScore);
                    pnlHighestScore = new Panel();
                    Panel_HighestScore(ref pnlHighestScore);
                    Controls.Add(pnlHighestScore);
                }
            }
            else
            {
                Image img1 = Properties.Resources.Tile;
                Bitmap resizedImage1 = new Bitmap(img1, new Size(btnTile.Width + 15, btnTile.Height + 15));
                btnTile.Image = resizedImage1;
                btnTile.Image.Tag = "No Flag";
                int num = Convert.ToInt32(NoOfFlag.Tag) + 1;
                NoOfFlag.Tag = num;
                NoOfFlag.Text = NoOfFlag.Tag + "";
            }
        }

        //Event for play again button
        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlWin.Visible = false;
            pnlGameOver.Visible = false;
            if (esyLvl)
            {
                btnEasyLevelEvent_Click(sender,e);
            }
            else if (emediumLvl)
            {
                btnMediumLevelEvent_Click(sender,e);
            }
            else if (ehardLvl)
            {
                btnHardLevelEvent_Click(sender, e);
            }
        }

        //Event for go home
        private void btnGoHome_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlWin.Visible = false;
            pnlGameOver.Visible = false;
            pnlGameStart.Visible = false;
            pnlMenu.Visible = true;

        }

        //Go back button event
        private void btnBackButtonEvent_Click(object sender, EventArgs e)
        {

            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            pnlMenu.Visible = true;
        }
        private void btnBackButton3Event_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlHelp.Visible = false;
            pnlMenu.Visible = true;
        }
        private void btnBackButton2Event_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlHighestScore.Visible = false;
            pnlMenu.Visible = true;
        }

        private void btnHighestScore_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlMenu.Visible = false;
            pnlHighestScore.Visible = true;
        }

        private void btnHelpEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlMenu.Visible = false;
            pnlHelp.Visible = true;
        }

        private void btnExitGameEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            Close();
        }

        private void btnStartGameEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlMenu.Visible = false;
            pnlDifficultyLevel.Visible = true;

        }

        //setting Label design
        private void setLabel(Label label, int x = 110,int y = 20,int size = 34,int height = 60)
        {
            label.SetBounds(x, y, 600, height);
            label.BackColor = Color.Transparent;
            label.Font = new Font("Times New Roman", size, FontStyle.Bold);
            label.ForeColor = Color.AntiqueWhite; // Set the text color
            label.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
        }
        private void setLabel2(Label label1, Label label2, int y1 = 50, int y2 = 100,int size = 50)
        {
            label1.SetBounds(10, y1, 600, 50);
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Times New Roman", 20, FontStyle.Underline);
            label1.ForeColor = Color.Black; // Set the text color
            label1.TextAlign = ContentAlignment.MiddleLeft; // Set the text alignment

            label2.SetBounds(10, y2, 600, size);
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Times New Roman", 14);
            label2.ForeColor = Color.Black; // Set the text color
            label2.TextAlign = ContentAlignment.MiddleLeft; // Set the text alignment
        }
        //Setting button design
        private void setButton(Button button)
        {
            button.BackColor = Color.Transparent;
            button.Font = new Font("Times New Roman", 34, FontStyle.Bold, GraphicsUnit.Pixel);
            button.ForeColor = Color.AntiqueWhite;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.FlatStyle = FlatStyle.Flat; // Add this line to make it flat
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.LightSeaGreen;
        }
        //On click sound function
        private void PlayButtonClickSound()
        {
            OnClickSound.Play();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundMusicPlayer.settings.setMode("loop", true);
            backgroundMusicPlayer.controls.play();
        }
    }
}
