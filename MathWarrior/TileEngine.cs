using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

//*Copyright Jacek Wierzbicki*

namespace MathWarrior
{
    /// <summary>
    /// Draws map into screen. Loads map from text file.
    /// </summary>
    public class TileEngine
    {
        SpriteBatch spriteBatch;
        Rectangle[] tiles = new Rectangle[50];
        Texture2D tileset;

        /// <summary>
        /// 2D int array storing map.
        /// </summary>
        public int[,] map;
        int mapWidth;
        int mapHeight;
        int tileWidth = 50;
        int tileHeight = 50;
        string tilesetType;

        /// <summary>
        /// TileEngine (map) constructor. Sets tiles from tileset textures.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tileset"></param>
        /// <param name="tilesetType"></param>
        /// <param name="path"></param>
        public TileEngine(SpriteBatch spriteBatch, Texture2D tileset, string tilesetType, string path)
        {
            mapWidth = 20;
            mapHeight = 10;
            this.spriteBatch = spriteBatch;
            this.tileset = tileset;
            this.tilesetType = tilesetType;
            this.map = LoadMap(path);
            SetTiles();
        }
        
        private void SetTiles()
        {
            if (tilesetType == "forest")
            {
                tiles[0] = new Rectangle(0, 0, 32, 32);
                //leaves blanket
                tiles[1] = new Rectangle(32, 0, 32, 32); //1st row
                tiles[2] = new Rectangle(64, 0, 32, 32); //1st row
                tiles[3] = new Rectangle(96, 0, 32, 32); //1st row
                tiles[4] = new Rectangle(32, 32, 32, 32); //2nd row
                tiles[5] = new Rectangle(64, 32, 32, 32); //2nd row
                tiles[6] = new Rectangle(96, 32, 32, 32); //2nd row
                tiles[7] = new Rectangle(32, 64, 32, 32); //3rd row
                tiles[8] = new Rectangle(64, 64, 32, 32); //3rd row
                tiles[9] = new Rectangle(96, 64, 32, 32); //3rd row
                //road
                tiles[10] = new Rectangle(160, 0, 32, 32); //1st row
                tiles[11] = new Rectangle(192, 0, 32, 32); //1st row
                tiles[12] = new Rectangle(224, 0, 32, 32); //1st row
                tiles[13] = new Rectangle(160, 32, 32, 32); //2nd row
                tiles[14] = new Rectangle(192, 32, 32, 32); //2nd row
                tiles[15] = new Rectangle(224, 32, 32, 32); //2nd row
                tiles[16] = new Rectangle(160, 64, 32, 32); //3rd row
                tiles[17] = new Rectangle(192, 64, 32, 32); //3rd row
                tiles[18] = new Rectangle(224, 64, 32, 32); //3rd row
                                                            //krzaki
                tiles[19] = new Rectangle(0, 32, 32, 32);
            }
            else if (tilesetType == "roguelike")
            {
                tiles[0] = new Rectangle(5 * 16 + 6, 0, 14, 14); //grass
                tiles[1] = new Rectangle(9 * 16 + 10, 16 + 2, 14, 14); //rock
                tiles[2] = new Rectangle(2 * 16 + 2, 0, 15, 15); //water top left
                tiles[3] = new Rectangle(3 * 16 + 3, 0, 15, 15); //water top
                tiles[4] = new Rectangle(4 * 16 + 4, 0, 15, 15); //water top right
                tiles[5] = new Rectangle(2 * 16 + 2, 16 + 1, 15, 15); //water left
                tiles[6] = new Rectangle(3 * 16 + 3, 16 + 1, 15, 15); //water mid
                tiles[7] = new Rectangle(4 * 16 + 4, 16 + 1, 15, 15); //water right
                tiles[8] = new Rectangle(2 * 16 + 2, 2 * 16 + 2, 15, 15); //water bottom left
                tiles[9] = new Rectangle(3 * 16 + 3, 2 * 16 + 2, 15, 15); //water bottom
                tiles[10] = new Rectangle(4 * 16 + 4, 2 * 16 + 2, 15, 15); //water bottom right
                tiles[11] = new Rectangle(0, 16 + 1, 15, 15); //water bottom right corner
                tiles[12] = new Rectangle(16 + 1, 16 + 1, 15, 15); //water bottom left corner
                tiles[13] = new Rectangle(0, 2 * 16 + 2, 15, 15); //water top right corner
                tiles[14] = new Rectangle(16 + 1, 2 * 16 + 2, 15, 15); //water top left corner
            }
            else if (tilesetType == "tileset2")
            {
                tiles[0] = new Rectangle(0, 0, 32, 32);
                tiles[1] = new Rectangle(4 * 32, 0, 32, 32); //rock
                tiles[2] = new Rectangle(2 * 32, 0, 32, 32); //pole
                tiles[3] = new Rectangle(3 * 32, 0, 32, 32); //fence
                tiles[4] = new Rectangle(0, 4 * 32, 32, 32); //flowers
                tiles[5] = new Rectangle(4 * 32, 4 * 32, 32, 32); //earth
            }
            else if (tilesetType == "dark")
            {
                tiles[0] = new Rectangle(32, 0, 32, 32); //ground 1
                tiles[1] = new Rectangle(2 * 32, 0, 32, 32); //ground 2
                tiles[2] = new Rectangle(3 * 32, 0, 32, 32); //ground 3
                tiles[3] = new Rectangle(4 * 32, 0, 32, 32); //ground 4
                tiles[4] = new Rectangle(5 * 32, 0, 32, 32); //ground 5
                tiles[5] = new Rectangle(6 * 32, 0, 32, 32); //ground 6
                tiles[6] = new Rectangle(0, 32, 32, 32); //stone top left
                tiles[7] = new Rectangle(32, 32, 32, 32); //stone top
                tiles[8] = new Rectangle(2 * 32, 32, 32, 32); //stone top right
                tiles[9] = new Rectangle(0, 2 * 32, 32, 32); //stone left
                tiles[10] = new Rectangle(32, 2 * 32, 32, 32); //stone mid
                tiles[11] = new Rectangle(2 * 32, 2 * 32, 32, 32); //stone right
                tiles[12] = new Rectangle(0, 3 * 32, 32, 32); //stone bottom left
                tiles[13] = new Rectangle(32, 3 * 32, 32, 32); //stone bottom
                tiles[14] = new Rectangle(2 * 32, 3 * 32, 32, 32); //stone bottom right
                tiles[15] = new Rectangle(4 * 32, 4 * 32, 32, 32); //wall top
                tiles[16] = new Rectangle(4 * 32, 5 * 32, 32, 32); //wall bottom
                tiles[17] = new Rectangle(3 * 32, 4 * 32, 32, 32); //wall left top
                tiles[18] = new Rectangle(3 * 32, 5 * 32, 32, 32); //wall left bottom
                tiles[19] = new Rectangle(5 * 32, 4 * 32, 32, 32); //wall right top
                tiles[20] = new Rectangle(5 * 32, 5 * 32, 32, 32); //wall right bottom
                tiles[21] = new Rectangle(7 * 32, 4 * 32, 32, 32); //entrance top
                tiles[22] = new Rectangle(7 * 32, 5 * 32, 32, 32); //entrance bottom
                tiles[23] = new Rectangle(6 * 32, 3 * 32, 32, 32); //stairs above
                tiles[24] = new Rectangle(6 * 32, 4 * 32, 32, 32); //stairs top
                tiles[25] = new Rectangle(6 * 32, 5 * 32, 32, 32); //stairs bottom
                tiles[26] = new Rectangle(3 * 32, 2 * 32, 32, 32); //hill left
                tiles[27] = new Rectangle(5 * 32, 2 * 32, 32, 32); //hill right
                tiles[28] = new Rectangle(3 * 32, 3 * 32, 32, 32); //hill bottom left
                tiles[29] = new Rectangle(4 * 32, 3 * 32, 32, 32); //hill bottom
                tiles[30] = new Rectangle(5 * 32, 3 * 32, 32, 32); //hill bottom right
                tiles[31] = new Rectangle(7 * 32, 2 * 32, 32, 32); //hill bottom bottom right connector
                tiles[32] = new Rectangle(6 * 32, 2 * 32, 32, 32); //hill bototm bottom left connector
            }
            else if(tilesetType == "tileset3")
            {
                tiles[0] = new Rectangle(0, 0, 32, 32); //grass
                tiles[1] = new Rectangle(2 * 32, 0, 32, 32); //sand small
                tiles[2] = new Rectangle(3 * 32, 0, 32, 32); //sand full
                tiles[3] = new Rectangle(2 * 32, 32, 32, 32); //sand top left
                tiles[4] = new Rectangle(3 * 32, 32, 32, 32); //sand top right
                tiles[5] = new Rectangle(2 * 32, 2 * 32, 32, 32); //sand bottom left
                tiles[6] = new Rectangle(3 * 32, 2 * 32, 32, 32); //sand bottom right
                tiles[7] = new Rectangle(2 * 32, 4 * 32, 32, 32); //grass with sand top left
                tiles[8] = new Rectangle(3 * 32, 4 * 32, 32, 32); //grass with sand top right
                tiles[9] = new Rectangle(2 * 32, 5 * 32, 32, 32); //grass with sand bottom left
                tiles[10] = new Rectangle(3 * 32, 5 * 32, 32, 32); //grass with sand bottom right
                tiles[11] = new Rectangle(32 + 1, 9 * 32, 32 - 1, 32); //grass-grass small
                tiles[12] = new Rectangle(2 * 32, 9 * 32, 32, 32); //grass-grass big
                tiles[13] = new Rectangle(10 * 32, 2 * 32, 32, 32); //entrance
                tiles[14] = new Rectangle(4 * 32 + 1, 0, 32 - 1, 32); //rock small
                tiles[15] = new Rectangle(5 * 32, 3 * 32, 32, 32); //rock full
                tiles[16] = new Rectangle(4 * 32, 4 * 32, 32, 32); //rock with sand top left
                tiles[17] = new Rectangle(5 * 32, 4 * 32, 32, 32); //rock with sandtop right
                tiles[18] = new Rectangle(4 * 32, 5 * 32, 32, 32); //rock with sandbottom left
                tiles[19] = new Rectangle(5 * 32, 5 * 32, 32, 32); //rock with sandbottom right
                tiles[20] = new Rectangle(4 * 32, 32, 32, 32); //rock with grass top left
                tiles[21] = new Rectangle(5 * 32, 32, 32, 32); //rock with grass top right
                tiles[22] = new Rectangle(4 * 32, 2 * 32, 32, 32); //rock with grass bottom left
                tiles[22] = new Rectangle(5 * 32, 2 * 32, 32, 32); //rock with grass bottom right
                tiles[23] = new Rectangle(0, 10 * 32, 32, 32); //grass-grass with sand top left
                tiles[24] = new Rectangle(32, 10 * 32, 32, 32); //grass-grass with sand top right
                tiles[25] = new Rectangle(0, 11 * 32, 32, 32); //grass-grass with sand bottom left
                tiles[26] = new Rectangle(32, 11 * 32, 32, 32); //grass-grass with sand bottom right
                tiles[27] = new Rectangle(4 * 32, 32, 32, 32); //rock with grass top left
                tiles[28] = new Rectangle(5 * 32, 32, 32, 32); //rock with grass top right
                tiles[29] = new Rectangle(4 * 32, 2 * 32, 32, 32); //rock with grass bottom left
                tiles[30] = new Rectangle(5 * 32, 2 * 32, 32, 32); //rock with grass bottom right
            }
            else if(tilesetType == "dung")
            {
                tiles[0] = new Rectangle(16, 32 + 1, 16, 16 - 1); //normal floor
                tiles[1] = new Rectangle(0, 0, 16, 16); //top wall
                tiles[2] = new Rectangle(0, 16, 16, 16); //bottom wall
                tiles[3] = new Rectangle(0, 32, 16, 16); //floor under wall
                tiles[4] = new Rectangle(0, 48, 16, 16); //back side of wall
                tiles[5] = new Rectangle(48, 16, 16, 16); //toxins out of wall
                tiles[6] = new Rectangle(48, 32, 16, 16); //toxins on the floor from wall
                tiles[7] = new Rectangle(0, 112 + 1, 16, 16 - 1); //left top corner of "stone carpet"
                tiles[8] = new Rectangle(16, 112 + 1, 16, 16 - 1); //top
                tiles[9] = new Rectangle(32, 112 + 1, 16, 16 - 1); //right top
                tiles[10] = new Rectangle(0, 128, 16, 16); //left side
                tiles[11] = new Rectangle(0, 144, 16, 16); //left bottom corner
                tiles[12] = new Rectangle(48, 144, 16, 16); //right bottom corner
                tiles[13] = new Rectangle(16, 144, 16, 16); //bottom side
                tiles[14] = new Rectangle(48, 128, 16, 16); //right side
                tiles[15] = new Rectangle(16, 128, 16, 16); //mid1
                tiles[16] = new Rectangle(48, 112 + 1, 16, 16 - 1); //right top corner
                tiles[17] = new Rectangle(240, 144, 16, 16); //empty
                tiles[18] = new Rectangle(3 * 16, 0, 16, 16); //pipe1
                tiles[19] = new Rectangle(4 * 16, 0, 16, 16); //pipe2
                tiles[20] = new Rectangle(4 * 16, 2 * 16, 16, 16); //wall without brick
            }
        }
        
        /// <summary>
        /// Draws tile in corresponding map cell.
        /// </summary>
        /// <param name="spriteBatch"/>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < mapHeight; y++)
                {
                for (int x = 0; x < mapWidth; x++)
                {
                    spriteBatch.Draw(tileset, new Rectangle(x * tileWidth - 1, y * tileHeight - 1, tileWidth + 2, tileHeight + 2),
                        tiles[map[x, y]], Color.White);
                }
            }
        }

        /// <summary>
        /// Loads map from file and parses to int 2D array.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// Loaded map.
        /// </returns>
        public int[,] LoadMap(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                string width = lines[0];
                string height = lines[1];
                if (!int.TryParse(width, out mapWidth)) { throw new Exception("Error parsing the value '" + width + "' as int"); }
                if (!int.TryParse(height, out mapHeight)) { throw new Exception("Error parsing the value '" + height + "' as int"); }
                int[,] loadedMap = new int[mapWidth, mapHeight];
                for (int y = 0; y < mapHeight; y++)
                {
                    string[] columns = lines[y + 2].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for(int x = 0; x < columns.Length; x++)
                    {
                        loadedMap[x, y] = int.Parse(columns[x]);
                    }
                }
                return loadedMap;
            }
            catch(Exception e)
            {
                throw new Exception("Error loading map '" + path + "'.The error was: " + e.ToString());
            }
        }

        /// <summary>
        /// Draw trees and bushes from file tileset3.png @level3
        /// </summary>
        public void DrawTrees()
        {
            Rectangle treeRect = new Rectangle(8 * 32, 3 * 32, 32, 32); //tree from tileset3
            Rectangle twoTreeRect = new Rectangle(10 * 32, 3 * 32 + 1, 32, 32); //two trees from tileset3
            Rectangle bushRect = new Rectangle(6 * 32, 3 * 32, 32, 32); //bush from tileset3
            spriteBatch.Draw(tileset, new Rectangle(3 * 50, 4 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(6 * 50, 6 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(7 * 50, 8 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(4 * 50, 6 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(1 * 50, 0 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(14 * 50, 6 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(12 * 50, 5 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(18 * 50, 8 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(4 * 50, 2 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(0 * 50, 2 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(2 * 50, 8 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(12 * 50, 1 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(16 * 50, 5 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(14 * 50, 8 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(12 * 50, 1 * 50, 50, 50), treeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(8 * 50, 4 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(6 * 50, 0 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(10 * 50, 0 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(0 * 50, 9 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(19 * 50, 4 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(19 * 50, 2 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(17 * 50, 9 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(6 * 50, 3 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(3 * 50, 1 * 50, 50, 50), twoTreeRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(1 * 50, 4 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(4 * 50, 9 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(10 * 50, 9 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(17 * 50, 6 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(17 * 50, 2 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(13 * 50, 2 * 50, 50, 50), bushRect, Color.White);
            spriteBatch.Draw(tileset, new Rectangle(7 * 50, 3 * 50, 50, 50), bushRect, Color.White);
        }
    }
}
