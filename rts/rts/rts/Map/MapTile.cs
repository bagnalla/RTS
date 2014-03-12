using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rts
{
    public class MapTile : BaseObject
    {
        Map map;
        new public readonly int X, Y;
        public readonly int Type;
        public bool Walkable;
        public readonly float CollisionRadius;
        public bool Visible;
        public bool Revealed;
        public BoundingBox BoundingBox;
        public Texture2D[] TextureSet { get; set; }
        public Texture2D[] HorizontalTextureSet { get; set; }
        public Texture2D[] VerticalTextureSet { get; set; }

        public List<MapTile> Neighbors = new List<MapTile>();

        public MapTile(Map map, int x, int y, int width, int height, int typeCode, int pathingCode)
            : base(new Rectangle(x * width, y * height, width, height))
        {
            this.map = map;
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

        public void RefreshTexture()
        {
            bool topLeft = false, topMiddle = false, topRight = false, middleRight = false, 
                bottomRight = false, bottomMiddle = false, bottomLeft = false, middleLeft = false;

            if (Y - 1 >= 0)
                if (map.Tiles[Y - 1, X].Type == Type)
                    topMiddle = true;
            if (Y + 1 < map.Height)
                if (map.Tiles[Y + 1, X].Type == Type)
                    bottomMiddle = true;
            if (X - 1 >= 0)
                if (map.Tiles[Y, X - 1].Type == Type)
                    middleLeft = true;
            if (X + 1 < map.Width)
                if (map.Tiles[Y, X + 1].Type == Type)
                    middleRight = true;
            if (Y - 1 >= 0 && X - 1 >= 0)
                if (map.Tiles[Y - 1, X - 1].Type == Type)
                    topLeft = true;
            if (Y - 1 >= 0 && X + 1 < map.Width)
                if (map.Tiles[Y - 1, X + 1].Type == Type)
                    topRight = true;
            if (Y + 1 < map.Height && X - 1 >= 0)
                if (map.Tiles[Y + 1, X - 1].Type == Type)
                    bottomLeft = true;
            if (Y + 1 < map.Height && X + 1 < map.Width)
                if (map.Tiles[Y + 1, X + 1].Type == Type)
                    bottomRight = true;

            // edges of map
            if (Y + 1 >= map.Height && !topMiddle && middleRight && middleLeft)
                Texture = TextureSet[1];
            else if (Y - 1 < 0 && !bottomMiddle && middleRight && middleLeft)
                Texture = TextureSet[7];
            else if (X + 1 >= map.Width && !middleLeft && topMiddle && bottomMiddle)
                Texture = TextureSet[3];
            else if (X - 1 < 0 && !middleRight && topMiddle && bottomMiddle)
                Texture = TextureSet[5];

            // main
            else if (!topLeft && !topMiddle && middleRight && bottomRight && bottomMiddle && !middleLeft)
                Texture = TextureSet[0];
            else if (!topMiddle && middleRight && bottomRight && bottomMiddle && bottomLeft && middleLeft)
                Texture = TextureSet[1];
            else if (!topMiddle && !topRight && !middleRight && bottomMiddle && bottomLeft && middleLeft)
                Texture = TextureSet[2];
            else if (topMiddle && topRight && middleRight && bottomRight && bottomMiddle && !middleLeft)
                Texture = TextureSet[3];
            else if (topLeft && topMiddle && topRight && middleRight && bottomRight && bottomMiddle && bottomLeft && middleLeft)
                Texture = TextureSet[4];
            else if (topLeft && topMiddle && !middleRight && bottomMiddle && bottomLeft && middleLeft)
                Texture = TextureSet[5];
            else if (topMiddle && topRight && middleRight && !bottomMiddle && !bottomLeft && !middleLeft)
                Texture = TextureSet[6];
            else if (topLeft && topMiddle && topRight && middleRight && !bottomMiddle && middleLeft)
                Texture = TextureSet[7];
            else if (topLeft && topMiddle && !middleRight && !bottomRight && !bottomMiddle && middleLeft)
                Texture = TextureSet[8];

            // horizontal
            else if (!topLeft && !topMiddle && !topRight && middleRight && !bottomRight && !bottomMiddle && !bottomLeft && !middleLeft)
                Texture = HorizontalTextureSet[0];
            else if (!topLeft && !topMiddle && !topRight && middleRight && !bottomRight && !bottomMiddle && !bottomLeft && middleLeft)
                Texture = HorizontalTextureSet[1];
            else if (!topLeft && !topMiddle && !topRight && !middleRight && !bottomRight && !bottomMiddle && !bottomLeft && middleLeft)
                Texture = HorizontalTextureSet[2];

            // vertical
            else if (!topLeft && !topMiddle && !topRight && !middleRight && !bottomRight && bottomMiddle && !bottomLeft && !middleLeft)
                Texture = VerticalTextureSet[0];
            else if (!topLeft && topMiddle && !topRight && !middleRight && !bottomRight && bottomMiddle && !bottomLeft && !middleLeft)
                Texture = VerticalTextureSet[1];
            else if (!topLeft && topMiddle && !topRight && !middleRight && !bottomRight && !bottomMiddle && !bottomLeft && !middleLeft)
                Texture = VerticalTextureSet[2];

            // alone
            else
                Texture = TextureSet[4];
        }
    }
}
