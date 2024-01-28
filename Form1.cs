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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
namespace MineSweeper
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer backgroundMusicPlayer = new WindowsMediaPlayer();
        private SoundPlayer OnClickSound;
        private Panel pnlMenu = new Panel();
        private Panel pnlDifficultyLevel = new Panel();
        private Panel pnlGameStart = new Panel();
        private Panel pnlGameOver = new Panel();
        private Panel pnlWin = new Panel();
        private Panel pnlHighestScore = new Panel();
        private Button[,] btnEsyLvlTiles = new Button[10,8];
        private Button[,] btnMediumLvlTiles = new Button[18,12];
        private Button[,] btnHardLvlTiles = new Button[24, 15];
        private bool esyLvl = false;
        private bool emediumLvl = false;
        private bool ehardLvl = false;
        private bool firstButtonPressed = true;
        private int unrevealedTiles = 0;
        private int numOfBoomTiles = 0;
        public Form1()
        {
            InitializeComponent();

            //BackGround Music
            backgroundMusicPlayer.URL = "BackgroundMusic.wav"; // Provide the correct path to your music file

            //Button Click Sound
            OnClickSound = new SoundPlayer();
            OnClickSound.SoundLocation = "ButtonClick.wav";

            Panel_Menu(ref pnlMenu);
            Panel_difficultyLevel(ref pnlDifficultyLevel);
            Panel_HighestScore(ref pnlHighestScore);
            Panel_GameOver(ref pnlGameOver,"Game Over");
            Panel_GameOver(ref pnlWin, "You Win");

            Controls.Add(pnlWin);
            Controls.Add(pnlGameOver);
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

        private void Panel_GameStart(ref Panel pnl, Button[,] btnTiles ,
            int btnSize = 50,int BoardWidth = 500,int BoardHeight = 400,int Position = 50
            )
        {
            //setting panel
            pnl.Size = this.ClientSize;
            pnl.BackColor = Color.Transparent;
            pnl.Visible = false;
            TableLayoutPanel tableLayoutPanel = pnl.Controls.OfType<TableLayoutPanel>().FirstOrDefault();

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
            pnl.Controls.Add(btnBackButtonInGame);
            

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

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {                    
                    btnTiles[i, j] = new Button();
                    btnTiles[i, j].Name = ", " + i + " " + j;
                    btnTiles[i, j].Size = new Size(btnSize, btnSize);
                    btnTiles[i, j].Margin = new Padding(0);
                    btnTiles[i, j].FlatStyle = FlatStyle.Flat;
                    btnTiles[i, j].Image = Properties.Resources.ButtonBackGround;
                    btnTiles[i, j].Click += (sender, e) => btnTileEvent_Click(sender, e, btnTiles);
                    btnTiles[i, j].MouseUp += new MouseEventHandler(btnTileEvent_MouseUp);
                    pnlGameBoard.Controls.Add(btnTiles[i, j], i, j);
                }
            }
            pnl.Controls.Add(pnlGameBoard);
            pnl.Visible = true;
        }

        private void btnBackButtonInGameEvent_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlGameStart.Visible = false;
            pnlDifficultyLevel.Visible = true;
        }

        private void btnTileEvent_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                Button btnTile = (Button)sender;
                if (btnTile.Image == Properties.Resources.ButtonBackGround)
                {
                    btnTile.Image = Properties.Resources.BackButton;
                }
                else
                {
                    btnTile.Image = Properties.Resources.ButtonBackGround;
                }
            }
        }

        private void btnHardLevelEvent_Click(object sender, EventArgs e)
        {
            esyLvl = false;
            emediumLvl = false;
            ehardLvl = true;
            firstButtonPressed = true;
            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            unrevealedTiles = 360;
            numOfBoomTiles = 99;
            Panel_GameStart(ref pnlGameStart, btnHardLvlTiles,30,720,450,80);
            pnlGameStart.Visible = true;
        }

        private void btnMediumLevelEvent_Click(object sender, EventArgs e)
        {
            esyLvl = false;
            emediumLvl = true;
            firstButtonPressed = true;
            ehardLvl = false;
            PlayButtonClickSound();
            pnlDifficultyLevel.Visible = false;
            unrevealedTiles = 216;
            numOfBoomTiles = 40;
            Panel_GameStart(ref pnlGameStart, btnMediumLvlTiles,36,648,432,80);
            pnlGameStart.Visible = true;
        }

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
            Panel_GameStart(ref pnlGameStart,btnEsyLvlTiles);
            pnlGameStart.Visible = true;
        }

        bool IsPositionValid(int row, int col, int targetX, int targetY)
        {
            int minDistance = 3;

            int distance = Math.Abs(row - targetX) + Math.Abs(col - targetY);

            return distance >= minDistance;
        }

        private void btnTileEvent_Click(object sender, EventArgs e,Button [,] btnTiles)
        {
            Button btnTile = (Button)sender;

            if (firstButtonPressed)
            {
                string naame = btnTile.Name;
                string[] paarts = naame.Split(' ');
                int xPosition = int.Parse(paarts[2]);
                int yPosition = int.Parse(paarts[1]);
                int Row = btnTiles.GetLength(1);
                int Column = btnTiles.GetLength(0);

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


            
            btnTile.Image = Properties.Resources.BackGround;
            Bitmap bmp = new Bitmap(btnTile.Image);
            String name = btnTile.Name;
            String[] parts = name.Split(' ');
            if (parts[0] == "0")
            {
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

                if (!btnTiles[c, r].Enabled)
                {
                    return;
                }

                btnTiles[c, r].Image = Properties.Resources.BackGround;
                Bitmap bm = new Bitmap(btnTiles[c, r].Image);
                // Create a graphics object from the bitmap
                using (Graphics g = Graphics.FromImage(bm))
                {
                    // Set font and brush for the text
                    Font font = new Font("Arial", 15, FontStyle.Bold);
                    Brush brush = Brushes.Black;
                    float x = (bm.Width - g.MeasureString("1", font).Width) / 2;
                    float y = (bm.Height - g.MeasureString("1", font).Height) / 2;
                    string naame = btnTiles[c, r].Name;
                    string []naaame = naame.Split(' ');
                    if (naaame[0] == "Safe")
                    {
                        naaame[0] = " ";
                    }

                    g.DrawString(naaame[0], font, brush, new PointF(x, y));
                }
                btnTiles[c, r].Image = bm;
                btnTiles[c, r].Enabled = false;
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

            if(unrevealedTiles == numOfBoomTiles)
            {

                pnlWin.Visible = true;
            }
        }

        private void ShowGameOverPanel()
        {

            // Show the game over panel
            pnlGameOver.Visible = true;

        }

        private void Panel_GameOver(ref Panel pnl,string text)
        {
            pnl.Visible = false;
            pnl.SetBounds(100,70,600,400);
            pnl.BackColor = Color.FromArgb(150, Color.AliceBlue);

            Label lblGameOver = new Label();
            setLabel(lblGameOver,10);
            lblGameOver.Text = text;
            lblGameOver.ForeColor = Color.Black;

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

            pnl.Controls.Add(btnPlayAgain);
            pnl.Controls.Add(btnGoHome);
            pnl.Controls.Add(lblGameOver);
        }

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

        private void btnGoHome_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            pnlWin.Visible = false;
            pnlGameOver.Visible = false;
            pnlGameStart.Visible = false;
            pnlMenu.Visible = true;

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
            tablePanel.Location = new Point((pnl.Width - tablePanel.Width) / 2,
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

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundMusicPlayer.settings.setMode("loop", true);
            backgroundMusicPlayer.controls.play();
        }

        private void setLabel(Label label, int x = 110)
        {
            label.SetBounds(x, 20, 600, 60);
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

        private void button1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.Enabled = false;
        }
    }
}
