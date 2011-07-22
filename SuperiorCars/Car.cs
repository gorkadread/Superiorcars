using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace SuperiorCars
{
    public class Car
    {
        static Random rndGen = new Random();

        public Car(float x, float y)
        {
            X = x;
            Y = y;
            IsActive = false;
            Speed = 0;
            Rotation = 0;
            Scale = 1.0;
            LastProperMove = 0;
            LastProperDirection = 0;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public Texture2D Texture { get; set; }

        public float Rotation { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double Scale { get; set; }

        public float Speed { get; set; }

        public double LastProperMove { get; set; }

        public int LastProperDirection { get; set; }

        public Vector2 StartPosition { get; set; }

        public bool IsActive { get; private set; }


    }
}
