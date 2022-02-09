using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Diagnostics;

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

        //  int[,] map = new int[7, 7];
        int delay = 0;
        const int delayOffset = 10;
        const int size = 40;

        const int mapX = 7;
        const int mapY = 7;

        Maps _map = new Maps();

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
            //  int imax = _map.mapList.map.GetLength(0);
            //  int jmax = _map.mapList.map.GetLength(1);
            //  for (int i = 0; i < imax; i++)
            //  {
            //      for (int j = 0; j < jmax; j++)
            //          _map.mapList.map[i, j] = i == 0 || j == 0 || i == imax - 1 || j == jmax - 1 ? 1 : 0;
            //  }
            start();
            base.Initialize();
        }
        void start()
        {
            currLen = 1;
            len[0] = ((int)Maps.mapSize / 2, (int)Maps.mapSize / 2);
            playerDir = nextplayerDir = (0, 0);
        }
        protected override void LoadContent()
        {
            _gm = new GraphicsManager(this.GraphicsDevice);
            _gm.effect = Content.Load<Effect>("block");
            loadMap();
            //  spawnDot();
        }
        string getExits(byte exit)
        {
            return ((exit & 0b1000) != 0 ? "up " : "") +
                    ((exit & 0b100) != 0 ? "down " : "") +
                    ((exit & 0b10) != 0 ? "left " : "") +
                    ((exit & 0b1) != 0 ? "right " : "");
        }
        void loadMap()
        {
            Debug.WriteLine("(x: {0} - y: {1}) exit: {2}.", _map.mapList.xOffset, _map.mapList.yOffset, getExits(_map.mapList.exits));
            for (int i = 0; i < 4; i++)
            {
                // 1000 -> 0100 (up)
                // 0100 -> 1000 (down)
                // 0010 -> 0001 (left)
                // 0001 -> 0010 (right)
                byte dest = (byte)Math.Pow(2, i);
                //if ((_map.mapList.exits & dest) == 0) continue;
                //byte orig = (byte)(((dest & 1) << 1) | ((dest >> 1) & 1) | ((dest & 0b100) << 1) | ((dest & 0b1000) >> 1));                
                if (((_map.mapList.exits >> i) & 1) != 0)
                {
                    switch (i)
                    {
                        case 3:
                            if (!_map.mapDict.ContainsKey((_map.mapList.xOffset, _map.mapList.yOffset - 1)))
                            {
                                Maps.MapQuad temp = _map.addMaps(_map.mapList, dest);
                                _map.mapDict.Add((_map.mapList.xOffset, _map.mapList.yOffset - 1), temp);
                             //   Debug.WriteLine("(x: {0} - y: {1}) added", _map.mapList.xOffset, _map.mapList.yOffset - 1);
                            }
                            break;
                        case 2:
                            if (!_map.mapDict.ContainsKey((_map.mapList.xOffset, _map.mapList.yOffset + 1)))
                            {
                                Maps.MapQuad temp = _map.addMaps(_map.mapList, dest);
                                _map.mapDict.Add((_map.mapList.xOffset, _map.mapList.yOffset + 1), temp);
                             //   Debug.WriteLine("(x: {0} - y: {1}) added", _map.mapList.xOffset, _map.mapList.yOffset + 1);
                            }
                            break;
                        case 1:
                            if (!_map.mapDict.ContainsKey((_map.mapList.xOffset - 1, _map.mapList.yOffset)))
                            {
                                Maps.MapQuad temp = _map.addMaps(_map.mapList, dest);
                                _map.mapDict.Add((_map.mapList.xOffset - 1, _map.mapList.yOffset), temp);
                             //   Debug.WriteLine("(x: {0} - y: {1}) added", _map.mapList.xOffset - 1, _map.mapList.yOffset);
                            }
                            break;
                        case 0:
                            if (!_map.mapDict.ContainsKey((_map.mapList.xOffset + 1, _map.mapList.yOffset)))
                            {
                                Maps.MapQuad temp = _map.addMaps(_map.mapList, dest);
                                _map.mapDict.Add((_map.mapList.xOffset + 1, _map.mapList.yOffset), temp);
                              //  Debug.WriteLine("(x: {0} - y: {1}) added", _map.mapList.xOffset + 1, _map.mapList.yOffset);
                            }
                            break;
                    }
                }
            }
            Debug.WriteLine("current pos (x: {0} - y: {1})", _map.mapList.xOffset, _map.mapList.yOffset);
        }
        void spawnDot()
        {
            _map.mapList.map[rand.Next(1, mapY - 1), rand.Next(1, mapX - 1)] = 2;
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
                if (_map.mapList.map[len[0].y - _map.mapList.yOffset * Maps.mapSize, len[0].x - _map.mapList.xOffset * Maps.mapSize] == 2)
                {
                    _map.mapList.map[len[0].y - _map.mapList.yOffset * Maps.mapSize, len[0].x - _map.mapList.xOffset * Maps.mapSize] = 0;
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

                len[0].x += playerDir.x;
                len[0].y += playerDir.y;

                checkPlayerPos();


                delay += delayOffset;

                if (_map.mapList.map[len[0].y - _map.mapList.yOffset * Maps.mapSize, len[0].x - _map.mapList.xOffset * Maps.mapSize] == 1) start();
            }
            currPlayerPos = new Vector2(len[0].x, len[0].y) * size - (((float)size / delayOffset) * delay) * new Vector2(playerDir.x, playerDir.y);
        }

        void checkPlayerPos()
        {
            if (len[0].x - _map.mapList.xOffset * Maps.mapSize < 0)
            {
                //len[0].x += Maps.mapSize;
                _map.mapList = _map.mapDict[(_map.mapList.xOffset - 1, _map.mapList.yOffset)];//_map.mapList.left;
                goto reload;
            }
            if (len[0].x - _map.mapList.xOffset * Maps.mapSize >= Maps.mapSize)
            {
                //len[0].x -= Maps.mapSize;
                _map.mapList = _map.mapDict[(_map.mapList.xOffset + 1, _map.mapList.yOffset)];// _map.mapList.right;
                goto reload;
            }
            if (len[0].y - _map.mapList.yOffset * Maps.mapSize < 0)
            {
                //len[0].y += Maps.mapSize;
                _map.mapList = _map.mapDict[(_map.mapList.xOffset, _map.mapList.yOffset - 1)];// _map.mapList.up;
                goto reload;
            }
            if (len[0].y - _map.mapList.yOffset * Maps.mapSize >= Maps.mapSize)
            {
                //len[0].y -= Maps.mapSize;
                _map.mapList = _map.mapDict[(_map.mapList.xOffset, _map.mapList.yOffset + 1)];// _map.mapList.down;
                goto reload;
            }
            return;
        reload:
            loadMap();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));
            Vector2 screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //_gm.begin(new Vector2(len[0].x, len[0].y) * size - screenSize / 2, screenSize);
            _gm.begin(currPlayerPos - screenSize / 2, screenSize);
            //_gm.drawBlock(currPlayerPos, playerColor);          

            int imax = _map.mapList.map.GetLength(1);
            int jmax = _map.mapList.map.GetLength(0);

            drawMap((_map.mapList.xOffset, _map.mapList.yOffset), imax, jmax);

            drawMap((_map.mapList.xOffset, _map.mapList.yOffset - 1), imax, jmax);
            drawMap((_map.mapList.xOffset, _map.mapList.yOffset + 1), imax, jmax);
            drawMap((_map.mapList.xOffset - 1, _map.mapList.yOffset), imax, jmax);
            drawMap((_map.mapList.xOffset + 1, _map.mapList.yOffset), imax, jmax);

            drawMap((_map.mapList.xOffset - 1, _map.mapList.yOffset - 1), imax, jmax);
            drawMap((_map.mapList.xOffset - 1, _map.mapList.yOffset + 1), imax, jmax);
            drawMap((_map.mapList.xOffset + 1, _map.mapList.yOffset - 1), imax, jmax);
            drawMap((_map.mapList.xOffset + 1, _map.mapList.yOffset + 1), imax, jmax);

            //          drawMap(_map.mapList.up, imax, jmax);
            //drawMap(_map.mapList.down, imax, jmax);
            //drawMap(_map.mapList.left, imax, jmax);
            //drawMap(_map.mapList.right, imax, jmax);

            for (int i = 0; i < currLen; i++)
            {
                _gm.drawBlock(new Vector2(len[i].x, len[i].y) * size, playerColor);
            }
            _gm.flush();
            base.Draw(gameTime);
        }
        void drawMap((int, int) coord /*Maps.MapQuad map*/, int imax, int jmax)
        {
            //if (map == null) return;            
            if (!_map.mapDict.ContainsKey(coord)) return;
            Maps.MapQuad map = _map.mapDict[coord];
            for (int j = 0; j < jmax; j++)
                for (int i = 0; i < imax; i++)
                {
                    Vector2 pos = (new Vector2(i, j)
                        + new Vector2(map.xOffset, map.yOffset) * Maps.mapSize) * size;
                    //- new Vector2(_map.mapList.xOffset - map.xOffset, _map.mapList.yOffset - 0*map.yOffset) * Maps.mapSize) * size;
                    switch (map.map[j, i])
                    {
                        case 1:
                            _gm.drawBlock(pos, mapColor);
                            break;
                        case 2:
                            _gm.drawBlock(pos, foodColor);
                            break;
                        case 5:
                            _gm.drawBlock(pos, Color.MonoGameOrange);
                            break;
                        case 6:
                            _gm.drawBlock(pos, Color.Aqua);
                            break;
                        case 0: break;
                        default:
                            _gm.drawBlock(pos, Color.Black);
                            break;
                    }
                }
        }
    }
}
