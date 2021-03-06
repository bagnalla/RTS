﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace rts
{
    class Map
    {
        public List<MapTile> Walls = new List<MapTile>();

        MapTile[,] tiles;
        int tileSize = 20;
        int width, height;

        public Map(string mapFilepath)
        {
            loadMap(mapFilepath);
            calculateTileNeighbors();
        }

        void loadMap(string mapFilePath)
        {
            string[] lines = File.ReadAllLines(mapFilePath);

            string[] widthAndHeight = lines[0].Split(' ');
            width = int.Parse(widthAndHeight[0]);
            height = int.Parse(widthAndHeight[1]);

            tiles = new MapTile[height, width];

            for (int i = 1; i < height + 1; i++)
            {
                string[] rowOfTiles = lines[i].Split(' ');

                for (int s = 0; s < width; s++)
                {
                    int typeCode = int.Parse(rowOfTiles[s].Substring(0, 1));
                    int pathingCode = int.Parse(rowOfTiles[s].Substring(1, 1));
                    tiles[i - 1, s] = new MapTile(s, (i - 1), tileSize, tileSize, typeCode, pathingCode);
                    if (pathingCode == 1)
                        Walls.Add(tiles[i - 1, s]);
                }
            }

            Util.SortByX(Walls);
        }

        void calculateTileNeighbors()
        {
            for (int i = 0; i < height; i++)
            {
                for (int s = 0; s < width; s++)
                {
                    MapTile tile = tiles[i, s];

                    if (i - 1 >= 0)
                    {
                        MapTile neighbor = tiles[i - 1, s];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < height)
                    {
                        MapTile neighbor = tiles[i + 1, s];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (s - 1 >= 0)
                    {
                        MapTile neighbor = tiles[i, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (s + 1 < width)
                    {
                        MapTile neighbor = tiles[i, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i - 1 >= 0 && s - 1 >= 0)
                    {
                        MapTile neighbor = tiles[i - 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i - 1 >= 0 && s + 1 < width)
                    {
                        MapTile neighbor = tiles[i - 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < height && s - 1 >= 0)
                    {
                        MapTile neighbor = tiles[i + 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < height && s + 1 < width)
                    {
                        MapTile neighbor = tiles[i + 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                }
            }
        }

        // find centerpoint of nearest walkable tile from given vector
        public Vector2 FindNearestWalkableTile(Vector2 point)
        {
            int y = (int)MathHelper.Clamp(point.Y / tileSize, 0, height - 1);
            int x = (int)MathHelper.Clamp(point.X / tileSize, 0, width - 1);
            MapTile tile = tiles[y, x];

            if (tile.Walkable)
                return tile.CenterPoint;

            MapTile neighbor;

            // find nextdoor neighbor closer to given vector
            float howFarLeft = tile.CenterPoint.X - point.X;
            float howFarRight = point.X - tile.CenterPoint.X;
            float howFarUp = tile.CenterPoint.Y - point.Y;
            float howFarDown = point.Y - tile.CenterPoint.Y;

            float biggest = 0;

            if (howFarLeft > biggest)
                biggest = howFarLeft;
            if (howFarRight > biggest)
                biggest = howFarRight;
            if (howFarUp > biggest)
                biggest = howFarUp;
            if (howFarDown > biggest)
                biggest = howFarDown;

            if (howFarLeft == biggest && x - 1 >= 0)
            {
                neighbor = tiles[y, x - 1];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarRight == biggest && x + 1 < width)
            {
                neighbor = tiles[y, x + 1];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarUp == biggest && y - 1 >= 0)
            {
                neighbor = tiles[y - 1, x];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarDown == biggest && y + 1 < height)
            {
                neighbor = tiles[y + 1, x];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }

            // find next closest neighbor
            for (int i = 0; ; i++)
            {
                if (y - i >= 0)
                {
                    neighbor = tiles[y - i, x];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < height)
                {
                    neighbor = tiles[y + i, x];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (x - i >= 0)
                {
                    neighbor = tiles[y, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (x + i < width)
                {
                    neighbor = tiles[y, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y - i >= 0 && x - i >= 0)
                {
                    neighbor = tiles[y - i, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y - i >= 0 && x + i < width)
                {
                    neighbor = tiles[y - i, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < height && x - i >= 0)
                {
                    neighbor = tiles[y + i, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < height && x + i < width)
                {
                    neighbor = tiles[y + i, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
        }
        public MapTile[,] Tiles
        {
            get
            {
                return tiles;
            }
        }
        public int TileSize
        {
            get
            {
                return tileSize;
            }
        }
    }

    class MapTile : BaseObject
    {
        new public readonly int X, Y;
        public readonly int Type;
        public bool Walkable;
        public readonly float CollisionRadius;
        public bool Visible = false;

        public List<MapTile> Neighbors = new List<MapTile>();

        public MapTile(int x, int y, int width, int height, int typeCode, int pathingCode)
            : base(new Rectangle(x * width, y * height, width, height))
        {
            X = x;
            Y = y;
            Type = typeCode;
            Walkable = (pathingCode == 0);
            CollisionRadius = width / 2f;
        }

        public bool IntersectsUnit(Unit u)
        {
            return Vector2.Distance(centerPoint, u.CenterPoint) < (CollisionRadius + u.Radius);
        }
    }
}
