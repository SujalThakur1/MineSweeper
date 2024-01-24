using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        private SoundPlayer backgroundMusicPlayer;
        private SoundPlayer OnClickSound;
        private Panel pnlMenu = new Panel();
        private Panel pnlDifficultyLevel = new Panel();
        private Panel pnlEasyLevel = new Panel();
        private Panel pnlHighestScore = new Panel();
        public Form1()
        {
            InitializeComponent();

            //BackGround Music
            backgroundMusicPlayer = new SoundPlayer();
            backgroundMusicPlayer.SoundLocation = "BackgroundMusic.wav"; // Provide the correct path to your music file

            //Button Click Sound
            OnClickSound = new SoundPlayer();
            OnClickSound.SoundLocation = "ButtonClick.wav";

            Panel_Menu(ref pnlMenu);
            Panel_difficultyLevel(ref pnlDifficultyLevel);
            Panel_HighestScore(ref pnlHighestScore);
            Panel_easyGame(ref pnlEasyLevel);

            Controls.Add(pnlHighestScore);
            Controls.Add(pnlDifficultyLevel);
            Controls.Add(pnlMenu);
            Controls.Add(pnlEasyLevel);
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

            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Controls.Add(lblWelcome);
            pnl.Controls.Add(btnStartGame);
            pnl.Controls.Add(btnHighestScore);
            pnl.Controls.Add(btnHelp);
            pnl.Controls.Add(btnExitGame);

            pnl.Visible = true;
        }

        private void PlayButtonClickSound()
        {
            OnClickSound.Play();
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

        private void Panel_easyGame(ref Panel pnl)
        {
            //setting panel
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Visible = false;

            //setting GameBoard
            TableLayoutPanel pnlGameBoard = new TableLayoutPanel();
            pnlGameBoard.Size = new Size(500, 400);
            pnlGameBoard.Location = new System.Drawing.Point((pnl.Width - pnlGameBoard.Width) / 2,
                (pnl.Height + 40 - pnlGameBoard.Height) / 2);
            pnlGameBoard.BackColor = Color.FromArgb(150, Color.AliceBlue);

            pnlGameBoard.RowCount = 8;
            pnlGameBoard.ColumnCount = 10;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button btnTile = new Button();
                    btnTile.Size = new Size(50, 50);
                    btnTile.Margin = new Padding(0);
                    btnTile.FlatStyle = FlatStyle.Flat;
                    btnTile.Image = Properties.Resources.ButtonBackGround;
                    btnTile.Click += new EventHandler(this.btnTileEvent_Click);
                    pnlGameBoard.Controls.Add(btnTile, i, j);
                }
            }
            pnl.Controls.Add(pnlGameBoard);
        }

        private void btnTileEvent_Click(object sender, EventArgs e)
        {
            Button btnTile = (Button)sender;
            btnTile.Image = Properties.Resources.BackGround;
        }

        private void Panel_HighestScore(ref Panel pnl)
        {
            pnl.Size = this.ClientSize;

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

            //Creating new panel in which we can store the score
            TableLayoutPanel tablePanel = new TableLayoutPanel();
            tablePanel.Size = new Size(600, 400);
            tablePanel.Location = new System.Drawing.Point((pnl.Width - tablePanel.Width) / 2,
                (pnl.Height + 40 - tablePanel.Height) / 2);
            tablePanel.BackColor = Color.FromArgb(150, Color.AliceBlue);

            // Add column styles
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // Rank
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // Score
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // Time
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56));   // Difficulty
            //reading from file
            try
            {
                string[] lines = File.ReadAllLines("HighestScore.txt");
                //sorting fileData from highest score to lowest for score(using bubble sort)
                bool doneSorting;
                do
                {
                    doneSorting = true;
                    int previousScore = 0;
                    string previousLine = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] parts = line.Split(',');
                        int score = int.Parse(parts[0]);
                        if (i == 0)
                        {
                            previousScore = score;
                            previousLine = line;
                            continue;
                        }
                        if (score > previousScore)
                        {
                            lines[i] = previousLine;
                            lines[i - 1] = line;
                            doneSorting = false;
                        }
                        previousScore = score;
                        previousLine = line;
                    }
                } while (!doneSorting);

                // Set up TableLayoutPanel properties
                tablePanel.RowCount = lines.Length;
                tablePanel.ColumnCount = 4;

                // Add headers
                Label rankHeader = new Label();
                rankHeader.Text = "Rank";
                rankHeader.Font = new Font("Times New Roman", 22, FontStyle.Bold);
                rankHeader.ForeColor = Color.Black; // Set the text color
                rankHeader.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                rankHeader.Size = new Size(120, 80);

                Label scoreHeader = new Label();
                scoreHeader.Text = "Score";
                scoreHeader.Font = new Font("Times New Roman", 22, FontStyle.Bold);
                scoreHeader.ForeColor = Color.Black; // Set the text color
                scoreHeader.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                scoreHeader.Size = new Size(120, 80);

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
                tablePanel.Controls.Add(scoreHeader, 1, 0);
                tablePanel.Controls.Add(timeHeader, 2, 0);
                tablePanel.Controls.Add(difficultyHeader, 3, 0);

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

                    Label scoreLabel = new Label();
                    scoreLabel.Text = parts[0];
                    scoreLabel.BackColor = Color.Transparent;
                    scoreLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    scoreLabel.ForeColor = Color.Black; // Set the text color
                    scoreLabel.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                    scoreLabel.Padding = new Padding(25, 0, 0, 0);

                    Label timeLabel = new Label();
                    timeLabel.Text = parts[1];
                    timeLabel.BackColor = Color.Transparent;
                    timeLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    timeLabel.ForeColor = Color.Black; // Set the text color
                    timeLabel.TextAlign = ContentAlignment.MiddleCenter;
                    timeLabel.Size = new Size(125, 30);

                    Label difficultyLabel = new Label();
                    difficultyLabel.Text = parts[2];
                    difficultyLabel.BackColor = Color.Transparent;
                    difficultyLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                    difficultyLabel.ForeColor = Color.Black; // Set the text color
                    difficultyLabel.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
                    difficultyLabel.Size = new Size(125, 30);


                    tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                    tablePanel.Controls.Add(rankLabel, 0, i + 1);
                    tablePanel.Controls.Add(scoreLabel, 1, i + 1);
                    tablePanel.Controls.Add(timeLabel, 2, i + 1);
                    tablePanel.Controls.Add(difficultyLabel, 3, i + 1);
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
                setLabel(error);
                error.Text = ex.Message;
                error.BackColor = Color.Black;
                pnl.Controls.Add(error);
            }



            //adding everything in Highest level panel 
            pnl.BackColor = Color.Transparent;
            pnl.Controls.Add(lblHighestScore);
            pnl.Controls.Add(btnBackButton2);
            pnl.Visible = false;
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

        private void btnBackButtonEvent_Click(object sender, EventArgs e)
        {

            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            pnlMenu.Visible = true;
        }

        private void btnHardLevelEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();

        }

        private void btnMediumLevelEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();

        }

        private void btnEasyLevelEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            pnlEasyLevel.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundMusicPlayer.PlayLooping();
        }

        private void setLabel(Label label)
        {
            label.SetBounds(110, 20, 600, 60);
            label.BackColor = Color.Transparent;
            label.Font = new Font("Times New Roman", 34, FontStyle.Bold);
            label.ForeColor = Color.AntiqueWhite; // Set the text color
            label.TextAlign = ContentAlignment.MiddleCenter; // Set the text alignment
        }

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

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}
