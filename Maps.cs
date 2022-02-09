using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Snake
{
    class Maps
    {
        public const int mapSize = 11;
        public class MapQuad
        {
            public int[,] map;
            public byte exits;
            public int xOffset, yOffset;
        }
        public Dictionary<(int x, int y), MapQuad> mapDict = new Dictionary<(int x, int y), MapQuad>();
        static Random rand = new Random();
        static byte exitsNum(byte exits)
        {
            return (byte)((exits & 1) + (exits >> 1 & 1) + (exits >> 2 & 1) + (exits >> 3 & 1));
        }
        public MapQuad mapList = new MapQuad();
        public Maps()
        {
            mapList.map = mapInit;
            mapList.exits = 0b1000;
            mapDict.Add((0, 0), mapList);
        }
        public MapQuad addMaps(MapQuad source, byte dest)
        {
            byte orig = (byte)(((dest & 1) << 1) | ((dest >> 1) & 1) | ((dest & 0b100) << 1) | ((dest & 0b1000) >> 1));
            // define o numero de saidas que o mapa vai ter
            int exits = rand.Next(1, 4);

            //// define as direções das saidas
            //int[] dir = new int[exits];
            //List<int> dirMap = new List<int>() { 3, 2, 1, 0 };

            //switch (orig)
            //{
            //    case 0b1000: dirMap.RemoveAt(0); break;
            //    case 0b0100: dirMap.RemoveAt(1); break;
            //    case 0b0010: dirMap.RemoveAt(2); break;
            //    case 0b0001: dirMap.RemoveAt(3); break;
            //}
            //for (int i = 0; i < exits; i++)
            //{
            //randomize:
            //    int index = rand.Next(0, dirMap.Count);
            //    int x, y;
            //    switch (dirMap[index])
            //    {
            //        case 3: x = 0; y = -1; break;
            //        case 2: x = 0; y = 1; break;
            //        case 1: x = -1; y = 0; break;
            //        case 0: x = 1; y = 0; break;
            //        default:
            //            x = y = 0; break;
            //    }
            //    if (mapDict.ContainsKey((source.xOffset + x, source.yOffset + y)))
            //    {
            //        Debug.WriteLine("mapDict contains (x: {0} - y: {1}).", source.xOffset + x, source.yOffset + y);
            //        if (mapDict[(source.xOffset + x, source.yOffset + y)].map[3 + 3 * x, 3 + 3 * y] == 2)
            //        {
            //            Debug.WriteLine("(x: {0} - y: {1}) blocked.", source.xOffset + x, source.yOffset + y);
            //            if (i < exits - 1) i++;
            //            goto randomize;
            //        }
            //    }
            //    dir[i] = dirMap[index];
            //    dirMap.RemoveAt(index);
            //}

            // configura o mapa
            MapQuad temp = new MapQuad();
            temp.map = (int[,])map0.Clone();
            temp.exits = 0;
            temp.xOffset = source.xOffset;
            temp.yOffset = source.yOffset;

            int exitsNum = 1;

            temp.map[0, 5] = temp.map[10, 5] = temp.map[5, 0] = temp.map[5, 10] = 1;
            switch (orig)
            {
                case 0b1000: temp.map[0, 5] = 7; temp.yOffset++; break;
                case 0b0100: temp.map[10, 5] = 7; temp.yOffset--; break;
                case 0b0010: temp.map[5, 0] = 7; temp.xOffset++; break;
                case 0b0001: temp.map[5, 10] = 7; temp.xOffset--; break;
            }
            int origId = (int)Math.Log2(orig);
            temp.exits |= orig;
            int count = 3;
            for (int i = 0; i < 4; i++)
            {
                byte dir = (byte)Math.Pow(2, i);
                if (dir == orig) continue;
                switch (dir)
                {
                    case 0b1000:
                        if (mapDict.ContainsKey((temp.xOffset, temp.yOffset - 1)))
                        {
                            if (mapDict[(temp.xOffset, temp.yOffset - 1)].map[10, 5] == 0) temp.map[0, 5] = 0;
                            temp.exits |= dir;
                        }
                        else
                        {
                            if (rand.Next(0, count) == 0)
                            {
                                temp.map[0, 5] = 0;
                                count = 6;
                                temp.exits |= dir;
                            }
                        }
                        break;
                    case 0b0100:
                        if (mapDict.ContainsKey((temp.xOffset, temp.yOffset + 1)))
                        {
                            if (mapDict[(temp.xOffset, temp.yOffset + 1)].map[0, 5] == 0) temp.map[10, 5] = 0;
                            temp.exits |= dir;
                        }
                        else
                        {
                            if (rand.Next(0, count) == 0)
                            {
                                temp.map[10, 5] = 0;
                                count = 6;
                                temp.exits |= dir;
                            }
                        }
                        break;
                    case 0b0010:
                        if (mapDict.ContainsKey((temp.xOffset - 1, temp.yOffset)))
                        {
                            if (mapDict[(temp.xOffset - 1, temp.yOffset)].map[5, 10] == 0) temp.map[5, 0] = 0;
                            temp.exits |= dir;
                        }
                        else
                        {
                            if (rand.Next(0, count) == 0)
                            {
                                temp.map[5, 0] = 0;
                                count = 6;
                                temp.exits |= dir;
                            }
                        }
                        break;
                    case 0b0001:
                        if (mapDict.ContainsKey((temp.xOffset + 1, temp.yOffset)))
                        {
                            if (mapDict[(temp.xOffset + 1, temp.yOffset)].map[5, 0] == 0) temp.map[5, 10] = 0;
                            temp.exits |= dir;
                        }
                        else
                        {
                            if (rand.Next(0, count) == 0)
                            {
                                temp.map[5, 10] = 0;
                                count = 6;
                                temp.exits |= dir;
                            }
                        }
                        break;
                }
                count--;
            }

            return temp;
        }
        // 0 - livre
        // 1 - muro
        // 2 - food
        // 3 - superposição livre/muro/porta
        // 4+ porta com numero de comidas necessárias para abrir
        static int[,] mapInit =
            new int[,] {                // 1000 udlr
                { 0, 0, 0, 0, 1, 4, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };
        static int[,] map0 =
            new int[,] {                // xxxx udlr
                { 1, 1, 1, 1, 1, 5, 1, 1, 1, 1, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                { 1, 1, 1, 1, 1, 5, 1, 1, 1, 1, 1},
        };
        static int[,] map1 =
            new int[,] {                // 00xx udlr
                { 1, 1, 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1, 1, 1},
                { 1, 0, 0, 0, 0, 0, 1},
                { 5, 0, 0, 0, 0, 0, 5},
                { 1, 0, 0, 0, 0, 0, 1},
                { 1, 1, 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1, 1, 1},
        };
        static int[,] map2 =
            new int[,] {                // xx00 udlr
                { 1, 1, 1, 5, 1, 1, 1},
                { 1, 1, 0, 0, 0, 1, 1},
                { 1, 1, 0, 0, 0, 1, 1},
                { 1, 1, 0, 0, 0, 1, 1},
                { 1, 1, 0, 0, 0, 1, 1},
                { 1, 1, 0, 0, 0, 1, 1},
                { 1, 1, 1, 5, 1, 1, 1},
        };
        static int[,] map3 =
            new int[,] {                // xxxx udlr
                { 1, 1, 1, 5, 1, 1, 1},
                { 1, 1, 1, 4, 1, 1, 1},
                { 1, 1, 1, 4, 1, 1, 1},
                { 5, 4, 4, 0, 4, 4, 5},
                { 1, 1, 1, 4, 1, 1, 1},
                { 1, 1, 1, 4, 1, 1, 1},
                { 1, 1, 1, 5, 1, 1, 1},
        };

    }
}
