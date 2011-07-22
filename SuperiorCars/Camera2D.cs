using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperiorCars
{
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public void IncrementZoom(float amount)
        {
           Zoom = Zoom + amount;
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        Viewport view;

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform = // Thanks to o KB o for this solution
                Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0))*
                Matrix.CreateRotationZ(_rotation)*
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))*
                Matrix.CreateTranslation(new Vector3((graphicsDevice.Viewport.Width * 0.5f) - (_pos.X * _zoom),
                                                     (graphicsDevice.Viewport.Height * 0.5f) - (_pos.Y * _zoom),0));
            return _transform;
        }

    }
}