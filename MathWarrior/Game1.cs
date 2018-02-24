using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Data;
using System.IO;

//*Copyright Jacek Wierzbicki*

namespace MathWarrior
{
    /// <summary>
    /// Main class of the game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont defaultFont;
        SpriteFont invasionFont;
        Song backgroundMusic;
        SoundEffect punchSound;

        //Render targets (top and bottom part of the screen)
        RenderTarget2D renderTargetTop;
        RenderTarget2D renderTargetBottom;
        
        //Maps and characters
        private List<TileEngine> tileEngines;
        private Character player;
        private List<Character> enemies;

        //Animations
        private AnimatedTexture spriteTexturePlayer;
        private List<AnimatedTexture> enemiesTexture;
        
        //Textures
        private Texture2D questionBackground;
        private Texture2D questionButtonTexture;
        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D mainMenuButton;
        private Texture2D restartButton;
        private Texture2D helpButton;
        private Texture2D splashScreen;
        private Texture2D winScreen;
        private Texture2D backgroundForest;
        private Texture2D gameOverScreen;

        private Texture2D arrowKeysTexture;
        private Texture2D mouseTexture;

        private Texture2D tileset_dark;
        private Texture2D tileset3;
        private Texture2D roguelike_tileset;
        private Texture2D tileset;
        private Texture2D tileset_dung;

        private Texture2D playerTexture;
        private Texture2D beeTexture;
        private Texture2D batTexture;
        private Texture2D ghostTexture;
        private Texture2D flowerTexture;
        private Texture2D knightTexture;
        private Texture2D hpBar;

        private Rectangle[] levelEndRect;
        //Positions
        private Vector2[] lvlStartPosition;
        private Vector2[] enemyPosition;
        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;
        private Vector2 resumeButtonPosition;
        private Vector2 mainMenuButtonPosition;
        private Vector2 restartButtonPosition;
        private Vector2 helpButtonPosition;
        private Vector2 arrowKeysPosition;
        private Vector2 mouseTexturePosition;

        private KeyboardState kstate;
        private MouseState mouseState;
        private MouseState previousMouseState;
        private Rectangle mouseClickRect;
        
        //Gamestate
        enum GameState { StartMenu, Help, LevelStart, Playing, Fight, Paused, GameOver, Win }
        private GameState gameState;
        private int level = 1;
        private bool lvlstart = false;
        
        //Units vars
        private string walkSidePlayer = "stay";
        private const int framesPlayer = 4;
        private const int framesKnight = 4;
        private const int framesMonster = 3;
        private const int framesPerSec = 2;
        private static float speedPlayer = 150;
        private static float speedEnemy = 100;
        private static int maxHpPlayer = 10;
        private static int maxHpEnemy = 10;
        
        //Question vars
        List<Question> questionList;
        private int answer;
        enum Correct { notSelected, correct, incorrect }
        private Correct isCorrect;
        private int currentQuestion;
        private Vector2 questionLocation;
        private Vector2[] answerLocation;
        private Vector2[] questionTextureLocation;
        private Vector2[] answerMeasure;
        private static Color correctColor = Color.Green;
        private static Color incorrectColor = Color.Red;
        private int correctlyAnswered;
        private int defeated;
        Random random;
        private static int levelChangeDelay = 150;
        private bool wellFull;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 750;
        }

        /// <summary>
        /// Checking mouse button clicks in menus.
        /// </summary>
        private void MouseClicked()
        {
            if (gameState == GameState.StartMenu)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height);
                Rectangle helpButtonRect = new Rectangle((int)helpButtonPosition.X, (int)helpButtonPosition.Y, 250, helpButton.Height);
                if (mouseClickRect.Intersects(startButtonRect))
                    gameState = GameState.Playing;
                else if (mouseClickRect.Intersects(helpButtonRect))
                    gameState = GameState.Help;
                else if (mouseClickRect.Intersects(exitButtonRect))
                    Exit();
            }

            if (gameState == GameState.Playing || gameState == GameState.Fight)
            {
                Rectangle pauseButtonRect = new Rectangle(0, 0, pauseButton.Width, pauseButton.Height);
                if (mouseClickRect.Intersects(pauseButtonRect))
                    gameState = GameState.Paused;
            }

            if (gameState == GameState.Paused)
            {
                Rectangle resumeButtonRect = new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, resumeButton.Width, resumeButton.Height);
                Rectangle mainMenuButtonRect = new Rectangle((int)mainMenuButtonPosition.X, (int)mainMenuButtonPosition.Y, mainMenuButton.Width, mainMenuButton.Height);
                if (mouseClickRect.Intersects(resumeButtonRect))
                    gameState = GameState.Playing;
                else if (mouseClickRect.Intersects(mainMenuButtonRect))
                    Initialize();
            }

            if(gameState == GameState.GameOver)
            {
                Rectangle restartButtonRect = new Rectangle((int)restartButtonPosition.X, (int)restartButtonPosition.Y, restartButton.Width, restartButton.Height);
                if (mouseClickRect.Intersects(restartButtonRect))
                    Initialize();
            }

            if(gameState == GameState.Win)
            {
                Rectangle restartButtonRect = new Rectangle((int)restartButtonPosition.X, (int)restartButtonPosition.Y, restartButton.Width, restartButton.Height);
                if (mouseClickRect.Intersects(restartButtonRect))
                    Initialize();
            }

            if(gameState == GameState.Help)
            {
                Rectangle mainMenuButtonRect = new Rectangle((int)mainMenuButtonPosition.X, (int)mainMenuButtonPosition.Y, mainMenuButton.Width, mainMenuButton.Height);
                if (mouseClickRect.Intersects(mainMenuButtonRect))
                    Initialize();
            }
        }

        /// <summary>
        /// Calculating player position based on Keyboard input and elapsed gameTime.
        /// </summary>
        /// <param name="kstate"/>
        /// <param name="gameTime"/>
        private void PlayerMovement(KeyboardState kstate, GameTime gameTime)
        {
            if (gameState == GameState.Playing)
            {
                player.oldPosition = player.position;

                if (kstate.IsKeyDown(Keys.Left))
                {
                    player.position.X -= player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (player.position.X < 0)
                        player.position.X = 0.0f;
                    player.walkSide = "left";
                }
                if (kstate.IsKeyDown(Keys.Right))
                {
                    player.position.X += player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (player.position.X > ((float)GraphicsDevice.Viewport.Width - (player.texture.Width / 3)))
                        player.position.X = ((float)GraphicsDevice.Viewport.Width - (player.texture.Width / 3));
                    player.walkSide = "right";
                }
                if (kstate.IsKeyDown(Keys.Up))
                {
                    player.position.Y -= player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (player.position.Y < 0)
                        player.position.Y = 0.0f;
                    player.walkSide = "up";
                }
                if (kstate.IsKeyDown(Keys.Down))
                {
                    player.position.Y += player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (player.position.Y > renderTargetTop.Height - (player.texture.Height / 3))
                        player.position.Y = ((float)renderTargetTop.Height - (player.texture.Height / 3));
                    player.walkSide = "down";
                }
                if (kstate.IsKeyUp(Keys.Down) && kstate.IsKeyUp(Keys.Up) && kstate.IsKeyUp(Keys.Left) && kstate.IsKeyUp(Keys.Right))
                {
                    player.walkSide = "stay";
                }
                player.SetBounds();

                //if (level == 0)
                //    Level0Collision();
                if (level == 1)
                    Level1Collision();
                if (level == 2)
                    Level2Collision();
                //if (level == 3)
                //    Level3Collision();
                if (level == 4)
                    Level4Collision();
            }
        }
        /*
        private void Level0Collision() //First
        {

        }*/

        /// <summary>
        /// Player collisions for level 1.
        /// </summary>
        private void Level1Collision() //Water
        {
            //bottom left-mid
            if (player.bounds.Intersects(new Rectangle(0, 380, 585, 150)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(350, 330, 190, 200)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(540, 430, 250, 150)))
                player.position = player.oldPosition;
            //bottom right
            if (player.bounds.Intersects(new Rectangle(850, 430, 150, 150)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(950, 380, 50, 150)))
                player.position = player.oldPosition;
            //top left
            if (player.bounds.Intersects(new Rectangle(0, 0, 40, 150)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(0, 0, 90, 50)))
                player.position = player.oldPosition;
            //top row is made automatically by character texture size
            //top right
            if (player.bounds.Intersects(new Rectangle(700, 0, 300, 50)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(800, 0, 200, 150)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(950, 0, 50, 250)))
                player.position = player.oldPosition;
            //pool
            if (player.bounds.Intersects(new Rectangle(350, 130, 90, 70)))
                player.position = player.oldPosition;
        }
        /// <summary>
        /// Player collisions for level 2.
        /// </summary>
        private void Level2Collision() //Dark
        {
            //top wall without entrance
            if (player.bounds.Intersects(new Rectangle(0, 0, 835, 50)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(900, 0, 200, 50)))
                player.position = player.oldPosition;
            //bottom rectangle
            if (player.bounds.Intersects(new Rectangle(870, 160, 200, 300)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(830, 200, 200, 300)))
                player.position = player.oldPosition;
            //bottom-bottom wall
            if (player.bounds.Intersects(new Rectangle(585, 355, 400, 200)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(585, 260, 30, 200)))
                player.position = player.oldPosition;
            //bottom-mid wall
            if (player.bounds.Intersects(new Rectangle(445, 260, 150, 100)))
                player.position = player.oldPosition;
            //mid-left wall
            if (player.bounds.Intersects(new Rectangle(190, 260, 195, 100)))
                player.position = player.oldPosition;
            //top wall
            if (player.bounds.Intersects(new Rectangle(45, 160, 160, 100)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(0, 70, 60, 120)))
                player.position = player.oldPosition;
        }
        /*
        private void Level3Collision() //Forest
        {
            
        }
        */

        /// <summary>
        /// Player collision for level 4.
        /// </summary>
        private void Level4Collision() //Dungeon
        {
            //top wall
            if (player.bounds.Intersects(new Rectangle(0, 0, 1000, 40)))
                player.position = player.oldPosition;
            //bottom wall
            if (player.bounds.Intersects(new Rectangle(0, 430, 1000, 50)))
                player.position = player.oldPosition;
            //entrance
            if (player.bounds.Intersects(new Rectangle(0, 0, 40, 160)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(0, 275, 40, 200)))
                player.position = player.oldPosition;
            //upper wall
            if (player.bounds.Intersects(new Rectangle(290, 130, 250, 70)))
                player.position = player.oldPosition;
            if (player.bounds.Intersects(new Rectangle(500, 0, 40, 150)))
                player.position = player.oldPosition;
            //right mid wall
            if (player.bounds.Intersects(new Rectangle(690, 225, 50, 270)))
                player.position = player.oldPosition;
            //right outter wall
            if (player.bounds.Intersects(new Rectangle(940, 0, 60, 310)))
                player.position = player.oldPosition;
        }

        /// <summary>
        /// Controls level start transition (player walking in).
        /// </summary>
        /// <param name="gameTime"></param>
        private void OnLevelStart(GameTime gameTime)
        {
            if (player.position.X < lvlStartPosition[level].X)
            {
                player.walkSide = "right";
                player.position.X += player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                player.walkSide = "stay";
                gameState = GameState.Playing;
                lvlstart = false;
            }
        }
        
        /// <summary>
        /// Sets locations for button textures. Called once in <see cref="MathWarrior.Game1.Initialize"/>.
        /// </summary>
        private void SetQuestionButtonLocations()
        {
            answerLocation = new Vector2[4];
            questionTextureLocation = new Vector2[4];
            answerMeasure = new Vector2[4];
            questionLocation = Vector2.Zero;
            questionTextureLocation[0] = new Vector2(renderTargetBottom.Width / 2, 0);
            questionTextureLocation[1] = new Vector2(3 * renderTargetBottom.Width / 4, 0);
            questionTextureLocation[2] = new Vector2(renderTargetBottom.Width / 2, renderTargetBottom.Height / 2);
            questionTextureLocation[3] = new Vector2(3 * renderTargetBottom.Width / 4, renderTargetBottom.Height / 2);
        }
        
        /// <summary>
        /// Sets locations for expression and answers based on text length. Must be called with every question change.
        /// </summary>
        /// <param name="question"></param>
        private void SetQuestionTextLocations(Question question)
        {
            Vector2 textureMeasure = new Vector2(questionButtonTexture.Width, questionButtonTexture.Height) / 2;
            Vector2 questionMeasure = defaultFont.MeasureString(question.QuestionString) / 2;
            questionLocation = new Vector2(renderTargetBottom.Width / 4 - questionMeasure.X,
                renderTargetBottom.Height / 2 - questionMeasure.Y);

            answerMeasure[0] = defaultFont.MeasureString(question.Answers[0]) / 2;
            answerMeasure[1] = defaultFont.MeasureString(question.Answers[1]) / 2;
            answerMeasure[2] = defaultFont.MeasureString(question.Answers[2]) / 2;
            answerMeasure[3] = defaultFont.MeasureString(question.Answers[3]) / 2;

            for (int i = 0; i < question.Answers.Length; i++)
            {
                answerLocation[i] = new Vector2(textureMeasure.X - answerMeasure[i].X, textureMeasure.Y - answerMeasure[i].Y) + questionTextureLocation[i];
            }
        }

        /// <summary>
        /// Checks if mouse clicked on answer and checks if answer is correct. After answer is given, calls <see cref="MathWarrior.Game1.ChangeQuestion"/>
        /// </summary>
        /// <param name="question"/>
        private void UpdateQuestion(Question question)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                answer = CheckIsMouseClickedOverAnOption(question);
                if (answer != -1)
                {
                    punchSound.CreateInstance().Play();
                    if (answer == question.CorrectOption)
                    {
                        //Correct answer 
                        Debug.WriteLine("correct answer");
                        isCorrect = Correct.correct;
                        correctlyAnswered++;

                        if (enemies[level].currhp > 0)
                        {
                            enemies[level].currhp--;
                        }
                        else
                        {
                            defeated++;
                            gameState = GameState.Playing;
                        }
                    }
                    else
                    {
                        //Incorrect answer 
                        Debug.WriteLine("incorrect answer");
                        isCorrect = Correct.incorrect;

                        //Save wrong answered question to to file
                        SaveQuestionToFile();

                        if (player.currhp > 0)
                            player.currhp--;
                        else
                            gameState = GameState.GameOver;
                    }
                    ChangeQuestion();
                }
            }
        }

        //
        /// <summary>
        /// Iterates over buttons positions for answer (mouse button click).
        /// </summary>
        /// <param name="question"></param>
        /// <returns> Answer as int </returns>
        private int CheckIsMouseClickedOverAnOption(Question question)
        {
            Rectangle[] answerRect = new Rectangle[4];
            for (int i = 0; i < question.Answers.Length; i++)
            {
                answerRect[i] = new Rectangle((int)questionTextureLocation[i].X, (int)questionTextureLocation[i].Y + 2 * renderTargetBottom.Height,
                    questionButtonTexture.Width, questionButtonTexture.Height);

                if (mouseClickRect.Intersects(answerRect[i]))
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Draws questions elements (buttons, buttons coloring, question/answers texts).
        /// </summary>
        /// <param name="question"></param>
        private void DrawQuestion(Question question)
        {
            spriteBatch.DrawString(defaultFont, question.QuestionString, questionLocation, Color.Black);
            
            for (int i = 0; i < question.Answers.Length; i++)
            {
                spriteBatch.Draw(questionButtonTexture, questionTextureLocation[i], Color.White);
            }
            if (isCorrect == Correct.correct || isCorrect == Correct.incorrect)
            {
                if(answer != -1)
                {
                    if (isCorrect == Correct.correct)
                        spriteBatch.Draw(questionButtonTexture, questionTextureLocation[answer], correctColor);
                    else if (isCorrect == Correct.incorrect)
                    {
                        spriteBatch.Draw(questionButtonTexture, questionTextureLocation[answer], incorrectColor);
                        spriteBatch.Draw(questionButtonTexture, questionTextureLocation[question.CorrectOption], correctColor);
                    }
                }
            }
            for (int i = 0; i < question.Answers.Length; i++)
            {
                spriteBatch.DrawString(defaultFont, question.Answers[i], answerLocation[i], Color.Black);
            }
        }

        /// <summary>
        /// Changes question after an answer has been given delay.
        /// Adds 1.5sec delay, see <see cref="System.Threading.Tasks.Task.Delay(int)"/>.
        /// </summary>
        private async void ChangeQuestion()
        {
            Debug.WriteLine("Answer input + delay(1500) + generate question");
            GenerateQuestion();
            await Task.Delay(1500);
            isCorrect = Correct.notSelected;
            currentQuestion++;
            Debug.WriteLine("Question changed");
        }

        /// <summary>
        /// Generates random math problem (as two operands and operation) with correct and 3 incorrect answers.
        /// Adds them to <see cref="MathWarrior.Game1.questionList"/>.
        /// </summary>
        private void GenerateQuestion()
        {
            int firstOp;
            int secOp;
            int operation = random.Next(4);
            char oper = '+';
            string expression;
            double result;

            string wrongExp;
            double[] wrongResult = new double[3];
            double[] wrongSpacing = new double[3];
            List<double> wrongList = new List<double>(3);

            switch (operation)
            {
                case 0: oper = '+'; break;
                case 1: oper = '-'; break;
                case 2: oper = '*'; break;
                case 3: oper = '%'; break;
            }

            if(oper == '-')
            {
                do
                {
                    firstOp = random.Next(1, 10);
                    secOp = random.Next(1, 10);
                } while (firstOp <= secOp);
            }
            else if(oper == '*')
            {
                do
                {
                    firstOp = random.Next(1, 10);
                    secOp = random.Next(1, 10);
                } while (firstOp == 1 || secOp == 1);
            }
            //else if(oper == '/')
            //{
            //    bool isInteger;
            //    do
            //    {
            //        firstOp = random.Next(1, 10);
            //        secOp = random.Next(1, 10);
            //        isInteger = (decimal)(firstOp / secOp) == Math.Truncate((decimal)(firstOp / secOp));
            //    } while (!isInteger);
            //}
            else
            {
                firstOp = random.Next(1, 10);
                secOp = random.Next(1, 10);
            }
            expression = firstOp.ToString() + " " + oper.ToString() + " " + secOp.ToString();
            result = Evaluate(expression);
                
            int value;
            int count = 3;
            do
            {
                value = random.Next(1, 6);
                if(!wrongList.Contains(value))
                {
                    wrongList.Add(value);
                    count--;
                }
            } while (count > 0);
            wrongSpacing = wrongList.ToArray();

            int wrongOp = random.Next(2);
            char wrongOper = '+';

            for(int i = 0; i < 3; i++)
            {
                wrongOp = random.Next(2);
                switch (wrongOp)
                {
                    case 0: wrongOper = '+'; break;
                    case 1: wrongOper = '-'; break;
                }
                wrongExp = result.ToString(System.Globalization.CultureInfo.InvariantCulture) + " "
                    + wrongOper.ToString() + " "
                    + wrongSpacing[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
                wrongResult[i] = Evaluate(wrongExp);
                if (wrongResult[i] < 0)
                {
                    wrongResult[i] *= -1;
                    if (wrongResult[i] == result)
                        wrongResult[i]++;
                }
            }
            
            Debug.WriteLine(firstOp + " " + oper + " " + secOp + " = " + result + ", " + wrongResult[0] + ", " + wrongResult[1] + ", " + wrongResult[2]);

            List<double> resultList = new List<double>(4)
            {
                result,
                wrongResult[0],
                wrongResult[1],
                wrongResult[2]
            };

            int correctResult = 0;
            correctResult = Shuffle(resultList);
            string[] resultArray = new string[4];

            for (int i = 0; i < resultList.Capacity; i++)
            {
                resultArray[i] = resultList[i].ToString();
            }
            questionList.Add(new Question(expression, resultArray, correctResult));
        }

        //Evaluate expression.
        /// <summary>
        /// Evaluate <paramref name="expression"/>. Based on <see cref="System.Data.DataTable"/> class.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns> Expression result </returns>
        private double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            var res = double.TryParse((string)row["expression"], out double result);
            return result;
        }
        
        /// <summary>
        /// Shuffles the list and return correct index.
        /// </summary>
        /// <param name="list"></param>
        /// <returns> Correct answer index as integer </returns>
        private int Shuffle(List<double> list)
        {
            int n = list.Count;
            int corr = 0;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                if (k == corr)
                    corr = n;
                double value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return corr;
        }

        /// <summary>
        /// Saves <see cref="MathWarrior.Question.QuestionString"/> parameter to text file. Called after answer is incorrect.
        /// </summary>
        private void SaveQuestionToFile()
        {
            using (StreamWriter sw = new StreamWriter("wrongAnswers.txt", true))
            {
                sw.WriteLine(questionList[currentQuestion].QuestionString);
            }
        }

        /// <summary>
        /// Alows the game to perform any initialization it needs before starting to run (including loading any non-graphic related content).
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            mouseState = Mouse.GetState();
            previousMouseState = mouseState;
            mouseClickRect = Rectangle.Empty;
            IsMouseVisible = true;
            questionList = new List<Question>();
            random = new Random();
            GenerateQuestion();

            //Buttons positions
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 4) - (startButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 160);
            helpButtonPosition = new Vector2(3 * (GraphicsDevice.Viewport.Width / 4) - (helpButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 100);
            exitButtonPosition = new Vector2(3 *(GraphicsDevice.Viewport.Width / 4) - (exitButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 230);
            
            resumeButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - (resumeButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 125);
            mainMenuButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - (mainMenuButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 225);
            restartButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - (restartButton.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) + 225);

            //Load levels
            tileEngines = new List<TileEngine>
            {
                new TileEngine(spriteBatch, tileset, "forest", "level0.txt"),
                new TileEngine(spriteBatch, roguelike_tileset, "roguelike", "level1.txt"),
                new TileEngine(spriteBatch, tileset_dark, "dark", "level2.txt"),
                new TileEngine(spriteBatch, tileset3, "tileset3", "level3.txt"),
                new TileEngine(spriteBatch, tileset_dung, "dung", "level4.txt")
            };

            lvlStartPosition = new Vector2[5];
            lvlStartPosition[0] = new Vector2(50, 190); //First
            lvlStartPosition[1] = new Vector2(50, 300); //Water
            lvlStartPosition[2] = new Vector2(50, 400); //Dark
            lvlStartPosition[3] = new Vector2(50, 270); //Forest
            lvlStartPosition[4] = new Vector2(50, 210); //Dungeon

            levelEndRect = new Rectangle[5];
            levelEndRect[0] = new Rectangle(19 * 50 + 30, 5 * 50 + 25, 50, 2 * 50); //First
            levelEndRect[1] = new Rectangle(19 * 50 + 30, 6 * 50, 50 - 30, 75); //Water
            levelEndRect[2] = new Rectangle(17 * 50, 0, 50, 50); //Dark - have entrace
            levelEndRect[3] = new Rectangle(16 * 50, 0, 50, 50); //Forest - have entrance
            levelEndRect[4] = new Rectangle(19 * 50, 7 * 50, 50, 2 * 50); //Dungeon

            player = new Character(playerTexture, lvlStartPosition[0] - new Vector2(100, 0),
                walkSidePlayer, speedPlayer, maxHpPlayer, 1, 0, framesPlayer);

            enemyPosition = new Vector2[5];
            enemyPosition[0] = new Vector2(400, 200); //First
            enemyPosition[1] = new Vector2(600, 210); //Water
            enemyPosition[2] = new Vector2(550, 150); //Dark
            enemyPosition[3] = new Vector2(500, 170); //Forest
            enemyPosition[4] = new Vector2(460, 300); //Dung

            //Spawn enemies
            enemies = new List<Character>
            {
                new Character(knightTexture, enemyPosition[0], "stay", speedEnemy, maxHpEnemy, 1, 0, framesKnight),
                new Character(beeTexture, enemyPosition[1], "down", speedEnemy, maxHpEnemy, 1, 0, framesMonster),
                new Character(ghostTexture, enemyPosition[2], "down", speedEnemy, maxHpEnemy, 1, 0, framesMonster),
                new Character(flowerTexture, enemyPosition[3], "down", speedEnemy, maxHpEnemy, 1, 0, framesMonster),
                new Character(batTexture, enemyPosition[4], "down", speedEnemy, maxHpEnemy, 1, 0, framesMonster)
            };

            //Animate player texture (update frame from character sheet)
            spriteTexturePlayer = new AnimatedTexture(player.texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, framesPlayer);

            //Animate enemy texture
            enemiesTexture = new List<AnimatedTexture>
            {
                new AnimatedTexture(enemies[0].texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, enemies[0].frames),
                new AnimatedTexture(enemies[1].texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, enemies[1].frames),
                new AnimatedTexture(enemies[2].texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, enemies[2].frames),
                new AnimatedTexture(enemies[3].texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, enemies[3].frames),
                new AnimatedTexture(enemies[4].texture, Vector2.Zero, 0, 1.5f, 0.5f, framesPerSec, enemies[4].frames)
            };
            
            level = 0;
            walkSidePlayer = "stay";
            isCorrect = Correct.notSelected;
            currentQuestion = 0;
            correctlyAnswered = 0;
            defeated = 0;
            wellFull = true;
            lvlstart = true;
            
            SetQuestionButtonLocations();
            gameState = GameState.StartMenu;
        }

        /// <summary>
        /// Place to load all content, method called once per game.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTargetTop = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, 2 * (GraphicsDevice.Viewport.Height / 3));
            renderTargetBottom = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, (GraphicsDevice.Viewport.Height / 3));

            punchSound = Content.Load<SoundEffect>("punchSound");
            backgroundMusic = Content.Load<Song>("Fantasy_Game_Background");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.65f;
            MediaPlayer.Play(backgroundMusic);
            
            roguelike_tileset = Content.Load<Texture2D>("Tilesets/tileset_roguelike");
            tileset = Content.Load<Texture2D>("Tilesets/tileset_forest");
            tileset_dung = Content.Load<Texture2D>("Tilesets/tileset_dung");
            tileset3 = Content.Load<Texture2D>("Tilesets/tileset3");
            tileset_dark = Content.Load<Texture2D>("Tilesets/tileset_dark");
            
            knightTexture = Content.Load<Texture2D>("CharacterSheets/knight");
            beeTexture = Content.Load<Texture2D>("CharacterSheets/bee");
            batTexture = Content.Load<Texture2D>("CharacterSheets/bat");
            ghostTexture = Content.Load<Texture2D>("CharacterSheets/ghost");
            flowerTexture = Content.Load<Texture2D>("CharacterSheets/flower");
            playerTexture = Content.Load<Texture2D>("CharacterSheets/charsheet1");
            
            questionBackground = Content.Load<Texture2D>("questionBackground");
            questionButtonTexture = Content.Load<Texture2D>("Buttons/questionButton");

            hpBar = Content.Load<Texture2D>("hpBar");
            arrowKeysTexture = Content.Load<Texture2D>("arrowKeys");
            mouseTexture = Content.Load<Texture2D>("mouse");

            splashScreen = Content.Load<Texture2D>("Screens/splashScreen");
            backgroundForest = Content.Load<Texture2D>("Screens/backgroundForest");
            winScreen = Content.Load<Texture2D>("Screens/winScreen");
            gameOverScreen = Content.Load<Texture2D>("Screens/gameOverScreen");

            startButton = Content.Load<Texture2D>("Buttons/startButton");
            exitButton = Content.Load<Texture2D>("Buttons/exitButton");
            pauseButton = Content.Load<Texture2D>("Buttons/pauseButton");
            resumeButton = Content.Load<Texture2D>("Buttons/resumeButton");
            mainMenuButton = Content.Load<Texture2D>("Buttons/mainMenuButton");
            restartButton = Content.Load<Texture2D>("Buttons/restartButton");
            helpButton = Content.Load<Texture2D>("Buttons/helpButton");

            defaultFont = Content.Load<SpriteFont>("Arial");
            invasionFont = Content.Load<SpriteFont>("InvasionFont");

            if (File.Exists("wrongAnswers.txt"))
                File.Delete("wrongAnswers.txt");
        }
        
        /// <summary>
        /// Unloads game content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating world, checking for collisions, gathering input. Called 60 times a second.
        /// Run alternately with <see cref="MathWarrior.Game1.Draw(GameTime)"/>.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            kstate = Keyboard.GetState();
            mouseState = Mouse.GetState();
            mouseClickRect = new Rectangle(mouseState.X, mouseState.Y, 5, 5);

            if (kstate.IsKeyDown(Keys.Escape))
                Exit();

            //Keybinds on screen change -- cheat
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D1))
            {
                level = 0;
                lvlstart = true;
                gameState = GameState.Playing;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D2))
            {
                level = 1;
                lvlstart = true;
                gameState = GameState.Playing;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D3))
            {
                level = 2;
                lvlstart = true;
                gameState = GameState.Playing;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D4))
            {
                level = 3;
                lvlstart = true;
                gameState = GameState.Playing;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D5))
            {
                level = 4;
                lvlstart = true;
                gameState = GameState.Playing;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D6))
            {
                gameState = GameState.Win;
            }
            if (kstate.IsKeyDown(Keys.LeftAlt) && kstate.IsKeyDown(Keys.D7))
            {
                gameState = GameState.GameOver;
            }
            //--end of cheats

            if (gameState == GameState.Playing || gameState == GameState.Fight || gameState == GameState.LevelStart)
            {
                if(gameState == GameState.LevelStart)
                {
                    OnLevelStart(gameTime);
                }

                if (gameState == GameState.Playing)
                {
                    PlayerMovement(kstate, gameTime); //player movement

                    //Set player position on beginning of new level
                    if (lvlstart)
                    {
                        player.position = lvlStartPosition[level] - new Vector2(100, 0);
                        gameState = GameState.LevelStart;
                    }

                    //Level changes
                    if(!lvlstart && player.bounds.Intersects(levelEndRect[level]) /*&& enemies[level].currhp == 0*/)
                    {
                        Thread.Sleep(levelChangeDelay);
                        if (level < 4)
                        {
                            level++;
                            lvlstart = true;
                        }
                        else
                            gameState = GameState.Win;
                    }

                    //Well @lvl3 hp+
                    if (level == 3 && player.bounds.Intersects(new Rectangle(11 * 50, 7 * 50, 50, 50)) && player.currhp < player.maxhp && wellFull)
                    {
                        wellFull = false;
                        player.currhp++;
                    }

                    if (player.bounds.Intersects(enemies[level].bounds))
                        gameState = GameState.Fight;
                    
                    //player frame
                    spriteTexturePlayer.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                    //enemy frames
                    enemiesTexture[level].UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                //Question update
                if (gameState == GameState.Fight)
                {
                    player.walkSide = "stay";

                    enemiesTexture[level].UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (player.currhp <= 0)
                        gameState = GameState.GameOver;
                    else if (enemies[level].currhp <= 0)
                        gameState = GameState.Playing;
                    else
                    {
                        SetQuestionTextLocations(questionList[currentQuestion]);
                        UpdateQuestion(questionList[currentQuestion]);
                    }
                    
                }

                /*
                //Add hp on mouse click -- cheat
                if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (mouseClickRect.Intersects(player.bounds))
                    {
                        Debug.WriteLine("Mouse clicked on player");
                        if(player.currhp < 10)
                            player.currhp++;
                    }
                }
                */
            }
            
            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                MouseClicked();
            
            previousMouseState = mouseState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself. Called 60 times a second.
        /// Run alternately with <see cref="MathWarrior.Game1.Update(GameTime)"/>.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(splashScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                spriteBatch.Draw(helpButton, helpButtonPosition, Color.White);
                spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
            }

            if(gameState == GameState.Help)
            {
                arrowKeysPosition = new Vector2(75, 130);
                mouseTexturePosition = new Vector2(75, 250);
                spriteBatch.Draw(backgroundForest, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(mainMenuButton, mainMenuButtonPosition, Color.White);
                spriteBatch.DrawString(invasionFont, "Controls:", new Vector2(50, 50), Color.White);
                spriteBatch.Draw(arrowKeysTexture, new Rectangle((int)arrowKeysPosition.X, (int)arrowKeysPosition.Y, 112, 74), Color.White);
                spriteBatch.DrawString(invasionFont, "to move a character", new Vector2(75 + 112 + 20, 130 + 20), Color.White);
                spriteBatch.Draw(mouseTexture, new Rectangle((int)mouseTexturePosition.X, (int)mouseTexturePosition.Y, 125, 115), Color.White);
                spriteBatch.DrawString(invasionFont, "to select an answer", new Vector2(75 + 125 + 20, 250 + 20), Color.White);
            }
            
            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(splashScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(resumeButton, resumeButtonPosition, Color.White);
                spriteBatch.Draw(mainMenuButton, mainMenuButtonPosition, Color.White);
            }

            if(gameState == GameState.GameOver)
            {
                spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(restartButton, restartButtonPosition, Color.White);
            }

            if(gameState == GameState.Win)
            {
                spriteBatch.Draw(winScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(restartButton, restartButtonPosition, Color.White);

                string winText1 = "Correctly answered questions: " + correctlyAnswered;
                Vector2 winText1pos = new Vector2(20, GraphicsDevice.Viewport.Height / 2 - invasionFont.MeasureString(winText1).Y / 2 + 75);
                string winText2 = "Remaining HP:    " + player.currhp;
                Vector2 winText2pos = new Vector2(GraphicsDevice.Viewport.Width - invasionFont.MeasureString(winText2).X, GraphicsDevice.Viewport.Height / 2 - invasionFont.MeasureString(winText2).Y / 2 + 125);

                spriteBatch.DrawString(invasionFont, winText1, winText1pos, Color.White);
                spriteBatch.DrawString(invasionFont, winText2, winText2pos, Color.White);
            }
            spriteBatch.End();
            
            if (gameState == GameState.Playing || gameState == GameState.Fight ||gameState == GameState.LevelStart)
            {
                //Top part of gamescreen
                GraphicsDevice.SetRenderTarget(renderTargetTop);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                
                tileEngines[level].Draw(spriteBatch);
                
                //well
                if(level == 3)
                {
                    spriteBatch.Draw(tileset3, new Rectangle(11 * 50, 7 * 50, 50, 50), new Rectangle(10 * 32, 0, 32, 32), Color.White);
                    tileEngines[3].DrawTrees();
                }
                
                //Player texture
                if (player.currhp > 0)
                    spriteTexturePlayer.DrawFrame(spriteBatch, player.position, player.walkSide); 
                //Enemy texture
                if (enemies[level].currhp > 0)
                    enemiesTexture[level].DrawFrame(spriteBatch, enemies[level].position, enemies[level].walkSide); 
                
                //Pause button
                spriteBatch.Draw(pauseButton, new Vector2(0, 0), Color.White); 

                if (gameState == GameState.Fight)
                {
                    //Player hp bar
                    if(level == 4)
                        spriteBatch.DrawString(defaultFont, player.currhp.ToString(), new Vector2(90, 13), Color.White);
                    else
                        spriteBatch.DrawString(defaultFont, player.currhp.ToString(), new Vector2(90, 13), Color.Black);
                    spriteBatch.Draw(hpBar, new Rectangle(130, 13, hpBar.Width * player.currhp / player.maxhp, hpBar.Height), Color.White);
                    //Enemy hp bar
                    if(level == 4)
                        spriteBatch.DrawString(defaultFont, enemies[level].currhp.ToString(), new Vector2(900, 13), Color.White);
                    else
                        spriteBatch.DrawString(defaultFont, enemies[level].currhp.ToString(), new Vector2(900, 13), Color.Black);
                    spriteBatch.Draw(hpBar, new Rectangle(870 - hpBar.Width * enemies[level].currhp / enemies[level].maxhp, 13, hpBar.Width * enemies[level].currhp / enemies[level].maxhp, hpBar.Height), Color.White);
                }

                spriteBatch.End();

                //Bottom part of gamescreen.
                GraphicsDevice.SetRenderTarget(renderTargetBottom);
                GraphicsDevice.Clear(Color.Aquamarine);
                spriteBatch.Begin();
                spriteBatch.Draw(questionBackground, new Rectangle(0, 0, renderTargetBottom.Width, renderTargetBottom.Height), Color.White);
                if (gameState == GameState.Playing)
                {
                    string correctlyAnsText = "Correctly answered so far: " + correctlyAnswered.ToString();
                    spriteBatch.DrawString(defaultFont, correctlyAnsText,
                        new Vector2(renderTargetBottom.Width / 2 - defaultFont.MeasureString(correctlyAnsText).X / 2,
                        renderTargetBottom.Height / 2 - defaultFont.MeasureString(correctlyAnsText).Y / 2),
                        Color.Black);
                }
                if(gameState == GameState.Fight)
                    DrawQuestion(questionList[currentQuestion]);
                spriteBatch.End();

                //Drawing renderTargets into screen
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin();
                spriteBatch.Draw(renderTargetTop,
                    new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 2 * (GraphicsDevice.Viewport.Height / 3)),
                    Color.White);
                spriteBatch.Draw(renderTargetBottom,
                    new Rectangle(0, 2 * (GraphicsDevice.Viewport.Height / 3), GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 3),
                    Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
