using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace rts
{
    public class Map
    {
        List<KeyValuePair<Point, int>> resources = new List<KeyValuePair<Point, int>>();
        public List<Point> StartingPoints = new List<Point>();

        public MapTile[,] Tiles { get; private set; }
        public BoundingBox[,] BigBoundingBoxes;
        public const int BOUNDING_BOX_SIZE = 8;
        int tileSize = 20;
        public int TileSize { get { return tileSize; } }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(string mapFilepath)
        {
            loadMap(mapFilepath);
            calculateTileNeighbors();
        }

        void loadMap(string mapFilePath)
        {
            string[] lines = File.ReadAllLines(mapFilePath);

            string[] widthAndHeight = lines[0].Split(' ');
            Width = int.Parse(widthAndHeight[0]);
            Height = int.Parse(widthAndHeight[1]);
            int numberOfResources = int.Parse(widthAndHeight[2]);
            int numberOfStartingPoints = int.Parse(widthAndHeight[3]);

            Tiles = new MapTile[Height, Width];

            int endOfTileLines = Height,
                endOfResourceLines = endOfTileLines + numberOfResources,
                endOfStartingPointLines = endOfResourceLines + numberOfStartingPoints;

            for (int i = 1; i <= endOfTileLines; i++)
            {
                string[] rowOfTiles = lines[i].Split(' ');

                for (int s = 0; s < Width; s++)
                {
                    int typeCode = int.Parse(rowOfTiles[s].Substring(0, 1));
                    int pathingCode = int.Parse(rowOfTiles[s].Substring(1, 1));
                    Tiles[i - 1, s] = new MapTile(s, (i - 1), tileSize, tileSize, typeCode, pathingCode);
                    //if (pathingCode == 1)
                    //    Walls.Add(Tiles[i - 1, s]);
                }
            }

            for (int i = endOfTileLines + 1; i <= endOfResourceLines; i++)
            {
                string[] resourceParams = lines[i].Split(' ');

                resources.Add(new KeyValuePair<Point, int>(new Point(int.Parse(resourceParams[1]), int.Parse(resourceParams[2])), int.Parse(resourceParams[0])));
            }

            for (int i = endOfResourceLines + 1; i <= endOfStartingPointLines; i++)
            {
                string[] resourceParams = lines[i].Split(' ');

                StartingPoints.Add(new Point(int.Parse(resourceParams[0]), int.Parse(resourceParams[1])));
            }

            //Util.SortByX(Walls);

            // create bounding boxes
            BigBoundingBoxes = new BoundingBox[Height / BOUNDING_BOX_SIZE, Width / BOUNDING_BOX_SIZE];
            for (int y = 0; y < Height / BOUNDING_BOX_SIZE; y++)
            {
                for (int x = 0; x < Width / BOUNDING_BOX_SIZE; x++)
                {
                    BigBoundingBoxes[y, x] = new BoundingBox(new Rectangle(x * BOUNDING_BOX_SIZE * TileSize, y * BOUNDING_BOX_SIZE * TileSize, TileSize * BOUNDING_BOX_SIZE, TileSize * BOUNDING_BOX_SIZE));
                }
            }

            // link bounding boxes with tiles
            foreach (BoundingBox box in BigBoundingBoxes)
            {
                for (int x = box.Rectangle.X / TileSize; x < box.Rectangle.X / TileSize + Map.BOUNDING_BOX_SIZE; x++)
                {
                    for (int y = box.Rectangle.Y / TileSize; y < box.Rectangle.Y / TileSize + Map.BOUNDING_BOX_SIZE; y++)
                    {
                        MapTile tile = Tiles[y, x];

                        tile.BoundingBox = box;
                        box.Tiles.Add(tile);
                    }
                }
            }
        }

        public void InstantiateMapResources()
        {
            foreach (KeyValuePair<Point, int> resource in resources)
            {
                if (resource.Value == 0)
                    new Roks(resource.Key);
            }
        }

        void calculateTileNeighbors()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int s = 0; s < Width; s++)
                {
                    MapTile tile = Tiles[i, s];

                    if (i - 1 >= 0)
                    {
                        MapTile neighbor = Tiles[i - 1, s];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < Height)
                    {
                        MapTile neighbor = Tiles[i + 1, s];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (s - 1 >= 0)
                    {
                        MapTile neighbor = Tiles[i, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (s + 1 < Width)
                    {
                        MapTile neighbor = Tiles[i, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i - 1 >= 0 && s - 1 >= 0)
                    {
                        MapTile neighbor = Tiles[i - 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i - 1 >= 0 && s + 1 < Width)
                    {
                        MapTile neighbor = Tiles[i - 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < Height && s - 1 >= 0)
                    {
                        MapTile neighbor = Tiles[i + 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                    }
                    if (i + 1 < Height && s + 1 < Width)
                    {
                        MapTile neighbor = Tiles[i + 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                    }
                }
            }
        }

        // find centerpoint of nearest walkable tile from given vector
        public Vector2 FindNearestWalkableTile(Vector2 point)
        {
            int y = (int)MathHelper.Clamp(point.Y / tileSize, 0, Height - 1);
            int x = (int)MathHelper.Clamp(point.X / tileSize, 0, Width - 1);
            MapTile tile = Tiles[y, x];

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
                neighbor = Tiles[y, x - 1];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarRight == biggest && x + 1 < Width)
            {
                neighbor = Tiles[y, x + 1];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarUp == biggest && y - 1 >= 0)
            {
                neighbor = Tiles[y - 1, x];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }
            else if (howFarDown == biggest && y + 1 < Height)
            {
                neighbor = Tiles[y + 1, x];
                if (neighbor.Walkable)
                    return neighbor.CenterPoint;
            }

            // find next closest neighbor
            for (int i = 0; ; i++)
            {
                if (y - i >= 0)
                {
                    neighbor = Tiles[y - i, x];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < Height)
                {
                    neighbor = Tiles[y + i, x];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (x - i >= 0)
                {
                    neighbor = Tiles[y, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (x + i < Width)
                {
                    neighbor = Tiles[y, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y - i >= 0 && x - i >= 0)
                {
                    neighbor = Tiles[y - i, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y - i >= 0 && x + i < Width)
                {
                    neighbor = Tiles[y - i, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < Height && x - i >= 0)
                {
                    neighbor = Tiles[y + i, x - i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
                if (y + i < Height && x + i < Width)
                {
                    neighbor = Tiles[y + i, x + i];
                    if (neighbor.Walkable)
                        return neighbor.CenterPoint;
                }
            }
        }

        public void UpdateBoundingBoxes()
        {
            foreach (BoundingBox box in BigBoundingBoxes)
                box.Update();
        }
    }
}
