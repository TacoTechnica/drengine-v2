﻿using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects
{
    [Serializable]
    public class Transform3D
    {
        private Vector3 _position = Vector3.Zero;

        private Matrix _posMat = Matrix.Identity;
        private Quaternion _rotation = Quaternion.Identity;
        private Matrix _rotMat = Matrix.Identity;
        private Vector3 _scale = Vector3.One;
        private Matrix _scaleMat = Matrix.Identity;

        private bool _cachedInverseUpToDate = false;
        private Matrix _cachedInverse = Matrix.Identity;

        public Transform3D()
        {
            // Do nothing we chilling
        }

        public Transform3D(Vector3 pos, Quaternion rot, Vector3 scale) : this()
        {
            // Start with position
            _position = pos;
            _rotation = rot;
            _scale = scale;
            _posMat = Matrix.CreateWorld(_position, Vector3.Forward, Vector3.Up);
            _rotMat = Matrix.CreateFromQuaternion(_rotation);
            _scaleMat = Matrix.CreateScale(_scale);
            UpdateWorld();
        }

        public Transform3D(Matrix world)
        {
            _position = new Vector3(world.M14, world.M24, world.M34);
            _scale = new Vector3(
                new Vector3(world.M11, world.M21, world.M31).Length(),
                new Vector3(world.M12, world.M22, world.M32).Length(),
                new Vector3(world.M13, world.M23, world.M33).Length()
            );
            _rotMat = new Matrix(
                world.M11 / _scale.X, world.M21 / _scale.Y, world.M31 / _scale.Z, 0,
                world.M12 / _scale.X, world.M22 / _scale.Y, world.M32 / _scale.Z, 0,
                world.M13 / _scale.X, world.M23 / _scale.Y, world.M33 / _scale.Z, 0,
                0, 0, 0, 0
            );

            _rotation = Quaternion.CreateFromRotationMatrix(_rotMat);
            _posMat = Matrix.CreateWorld(_position, Vector3.Forward, Vector3.Up);
            _scaleMat = Matrix.CreateScale(_scale);
        }

        [JsonIgnore] public Matrix Local { get; private set; } = Matrix.Identity;

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z))
                {
                    Debug.LogError("Tried setting null position.");
                    return;
                }
                _position = value;
                _posMat = Matrix.CreateWorld(_position, Vector3.Forward, Vector3.Up);
                UpdateWorld();
            }
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z) || float.IsNaN(value.W))
                {
                    Debug.LogError("Tried setting null rotation.");
                    return;
                }
                _rotation = value;
                _rotation.Normalize();
                _rotMat = Matrix.CreateFromQuaternion(_rotation);
                UpdateWorld();
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z))
                {
                    Debug.LogError("Tried setting null scale`.");
                    return;
                }

                _scale = value;
                _scaleMat = Matrix.CreateScale(_scale);
                UpdateWorld();
            }
        }

        private void UpdateWorld()
        {
            Local = _scaleMat * _rotMat * _posMat;
            _cachedInverseUpToDate = false;
        }

        public Matrix GetInverse()
        {
            if (_cachedInverseUpToDate) return _cachedInverse;
            Matrix invPosMat = Matrix.CreateWorld(-1 * _position, Vector3.Forward, Vector3.Up);
            Matrix invRotMat = Matrix.CreateFromQuaternion(Quaternion.Inverse(_rotation));
            Matrix invScaleMat;
            try
            {
                invScaleMat = Matrix.CreateScale(1.0f / _scale.X, 1.0f / _scale.Y, 1.0f / _scale.Z);
            }
            catch (Exception e)
            {
                _cachedInverse = Matrix.Identity;
                return Matrix.Identity;
            }

            _cachedInverseUpToDate = true;
            _cachedInverse = invPosMat * invRotMat * invScaleMat;
            return _cachedInverse;
        }


        public override string ToString()
        {
            return $"[Transform3D] {{pos={_position} rot={_rotation} scale={_scale}}}";
        }
    }
}