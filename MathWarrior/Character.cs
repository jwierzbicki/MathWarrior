using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//*Copyright Jacek Wierzbicki*

namespace MathWarrior
{
    /// <summary>
    /// Character entity (player, enemy).
    /// </summary>
    public class Character
    {
        /// <summary> Character texture. </summary>
        public Texture2D texture;
        /// <summary> Character position. </summary>
        public Vector2 position;
        /// <summary> Character previous position. Used for collisions. </summary>
        public Vector2 oldPosition;
        /// <summary> Character walk side. Used for animating correct row of spritesheet. </summary>
        public string walkSide;
        /// <summary> Character boundaries, rectangle around character texture. </summary>
        public Rectangle bounds;
        /// <summary> Number of frames in character sheet. </summary>
        public int frames;
        /// <summary> Character speed value. </summary>
        public float speed;
        /// <summary> Character attack value. </summary>
        public int attack;
        /// <summary> Character armor value. </summary>
        public int armor;
        /// <summary> Character current hp. </summary>
        public int currhp;
        /// <summary> Character maximum hp. </summary>
        public int maxhp;

        /// <summary>
        /// Character constructor.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="walkSide"></param>
        /// <param name="speed"></param>
        /// <param name="maxhp"></param>
        /// <param name="attackValue"></param>
        /// <param name="armorValue"></param>
        /// <param name="frames"></param>
        public Character(Texture2D texture, Vector2 position, string walkSide, float speed, int maxhp, int attackValue, int armorValue, int frames)
        {
            this.texture = texture;
            this.position = position;
            this.walkSide = walkSide;
            this.speed = speed;
            this.maxhp = maxhp;
            currhp = maxhp;
            this.attack = attackValue;
            this.armor = armorValue;
            this.frames = frames;
            SetBounds();
        }
        /// <summary>
        /// Sets rectangle around the character for collisions. Must be called on every position change.
        /// </summary>
        public void SetBounds()
        {
            bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width / frames, texture.Height / 4);
        }

    }
}
