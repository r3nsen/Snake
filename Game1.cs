using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace Snake
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        GraphicsManager _gm;
        Random rand = new Random();
        Color playerColor = new Color(200, 50, 200);
        Color mapColor = new Color(200, 200, 200);
        Color foodColor = new Color(50, 200, 50);

        //Vector2 playerPos = new Vector2(2, 2);
        Vector2 currPlayerPos = new Vector2(2 * 50, 2 * 50);
        (int x, int y) playerDir = (0, 0);//Vector2.Zero;
        (int x, int y) nextplayerDir = (0, 0);//Vector2.Zero;

        int[,] map = new int[mapY, mapX];
        int delay = 0;
        const int delayOffset = 10;
        const int size = 40;

        const int mapX = 24;
        const int mapY = 14;

        (int x, int y)[] len = new (int x, int y)[mapX * mapY];

        int currLen = 1;

        public Game1()
        {

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            //graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = true;

        }

        protected override void Initialize()
        {
            int imax = map.GetLength(0);
            int jmax = map.GetLength(1);
            for (int i = 0; i < imax; i++)
            {
                for (int j = 0; j < jmax; j++)
                    map[i, j] = i == 0 || j == 0 || i == imax - 1 || j == jmax - 1 ? 1 : 0;
            }
            start();
            base.Initialize();
        }
        void start()
        {
            currLen = 1;
            len[0] = (10, 10);
            playerDir = nextplayerDir = (0, 0);
        }
        protected override void LoadContent()
        {
            _gm = new GraphicsManager(this.GraphicsDevice);
            _gm.effect = Content.Load<Effect>("block");
            spawnDot();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) nextplayerDir = (0, -1);// -Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) nextplayerDir = (0, 1);// Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) nextplayerDir = (-1, 0);// -Vector2.UnitX;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) nextplayerDir = (1, 0);// Vector2.UnitX;

            --delay;

            if (delay < 0)
            {


                if (map[len[0].y, len[0].x] == 2)
                {
                    map[len[0].y, len[0].x] = 0;
                    currLen++;
                    currLen = Math.Clamp(currLen, 0, len.Length);
                    spawnDot();
                }

                for (int i = 1; i < currLen - 1; i++)
                    if (len[0] == len[i]) start();

                for (int i = currLen - 1; i > 0; i--)
                    len[i] = len[i - 1];

                if (nextplayerDir != (-playerDir.x, -playerDir.y))
                    playerDir = nextplayerDir;

                len[0].x += playerDir.x;// playerPos += playerDir;
                len[0].y += playerDir.y;

                delay += delayOffset;

                if (map[len[0].y, len[0].x] == 1) start();
            }

            //currPlayerPos = new Vector2(len[0].x, len[0].y) * size - (((float)size / delayOffset) * delay) * new Vector2(playerDir.x, playerDir.y);
        }
        void spawnDot()
        {
        setspawn:
            int x = rand.Next(2, mapX - 2);
            int y = rand.Next(2, mapY - 2);

            for (int i = 0; i < currLen - 1; i++)
                if ((len[i].x, len[i].y) == (x, y))
                    goto setspawn;

            map[y, x] = 2;
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));
            Vector2 screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _gm.begin(new Vector2(mapX * size / 2 - 20, mapY * size / 2 - 20) - screenSize / 2, screenSize);
            //_gm.drawBlock(currPlayerPos, playerColor);          

            int imax = map.GetLength(1);
            int jmax = map.GetLength(0);
            for (int j = 0; j < jmax; j++)
                for (int i = 0; i < imax; i++)
                    switch (map[j, i])
                    {
                        case 1:
                            _gm.drawBlock(new Vector2(i, j) * size, mapColor);
                            break;
                        case 2:
                            _gm.drawBlock(new Vector2(i, j) * size, foodColor);
                            break;
                    }
            for (int i = 0; i < currLen; i++)
            {
                _gm.drawBlock(new Vector2(len[i].x, len[i].y) * size, playerColor);
            }
            _gm.flush();
            base.Draw(gameTime);
        }
    }
}
