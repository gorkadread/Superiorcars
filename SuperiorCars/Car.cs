using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace SuperiorCars
{
    // ANVÄNDS EJ ÄNNU
    public class Car
    {
        Texture2D _mCarTexture;
        int _mCarHeight;
        int _mCarWidth;
        protected float _mCarRotation;
        protected double _mCarScale;

        public Car()
        {
            _mCarRotation = 0;
            _mCarScale = 1.0;
        }

        public float CarRotation
        {
            get { return _mCarRotation; }
            set { _mCarRotation = value; }
        }

        public double CarScale
        {
            get { return _mCarScale; }
            set { _mCarScale = value; }
        }

        public void CarTexture(Texture2D texture)
        {
            using g as Game1;
            Load
        }
    }
}
