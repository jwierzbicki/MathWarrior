using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//*Copyright Jacek Wierzbicki*

namespace MathWarrior
{ 
    /// <summary>
    /// Animates character textures.
    /// </summary>
    public class AnimatedTexture
    {
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int frame = 0;
        private float totalElapsed;

        private float rotation, scale, depth;
        private Vector2 origin;

        /// <summary>
        /// Animated texture constructor.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="depth"></param>
        /// <param name="framesPerSec"></param>
        /// <param name="frameCount"></param>
        public AnimatedTexture(Texture2D texture, Vector2 origin, float rotation, float scale, float depth, int framesPerSec, int frameCount)
        {
            this.origin = origin;
            this.rotation = rotation;
            this.scale = scale;
            this.depth = depth;
            this.TimePerFrame = (float)1 / framesPerSec;
            this.framecount = frameCount;
            this.totalElapsed = 0;
            this.myTexture = texture;
        }
        
        /// <summary>
        /// Iterate current frame over spritesheet.
        /// </summary>
        /// <param name="elapsed"></param>
        public void UpdateFrame(float elapsed)
        {
            totalElapsed += elapsed;
            if (totalElapsed > TimePerFrame)
            {
                frame++;
                frame = frame % framecount;
                totalElapsed -= TimePerFrame;
            }
        }
        
        /// <summary>
        /// Draws current frame based on walkSide and position of character.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="walkSide"></param>
        public void DrawFrame(SpriteBatch spriteBatch, Vector2 screenPos, string walkSide)
        {
            int frameWidth = myTexture.Width / framecount;
            int frameHeight = myTexture.Height / 4;
            Rectangle sourceRect = Rectangle.Empty;
            if (walkSide == "stay")
            {
                sourceRect = new Rectangle(0, 0, frameWidth, frameHeight);
            }
            if (walkSide == "down")
            {
                sourceRect = new Rectangle(frameWidth * frame, 0, frameWidth, frameHeight);
            }
            if(walkSide == "left")
            {
                sourceRect = new Rectangle(frameWidth * frame, frameHeight, frameWidth, frameHeight);
            }
            if(walkSide == "right")
            {
                sourceRect = new Rectangle(frameWidth * frame, 2 * frameHeight, frameWidth, frameHeight);
            }
            if(walkSide == "up")
            {
                sourceRect = new Rectangle(frameWidth * frame, 3* frameHeight, frameWidth, frameHeight);
            }
            
            spriteBatch.Draw(myTexture, screenPos, sourceRect, Color.White, rotation, origin, scale, SpriteEffects.None, depth);
        }
    }
}
