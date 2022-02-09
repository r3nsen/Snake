﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    class GraphicsManager
    {
        GraphicsDevice graphicsD;
        public Effect effect;

        Matrix view;
        Matrix projection;

        VertexPositionColorTexture[] vertex = new VertexPositionColorTexture[1];
        short[] index = new short[1];
        int vertexCount = 0, indexCount = 0;

        public GraphicsManager(GraphicsDevice graphicsDevice)
        {
            graphicsD = graphicsDevice;

            vertex = new VertexPositionColorTexture[4];
            index = new short[6];            
        }

        public void begin(Vector2 pos, Vector2 size)
        {
            Vector3 orig = new Vector3(0, 0, 3);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            Matrix.CreateLookAt(ref orig, ref target, ref up, out view);
            Matrix.CreateOrthographicOffCenter(pos.X, pos.X + size.X, pos.Y + size.Y, pos.Y, -100, 100, out projection);
            effect.Parameters["WorldViewProjection"].SetValue(view * projection);
        }
        public void drawBlock(Vector2 pos, Color cor)
        {
            EnsureSpace(6, 4);

            index[indexCount++] = (short)(vertexCount + 0);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 2);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 3);
            index[indexCount++] = (short)(vertexCount + 2);

            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), cor, new Vector2(0, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), cor, new Vector2(1, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), cor, new Vector2(0, 1));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), cor, new Vector2(1, 1));

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                //* Matrix.CreateRotationZ(MathHelper.PiOver2 * flipX)
                * Matrix.CreateScale(new Vector3(40, 40, 1))
                * Matrix.CreateTranslation(new Vector3(pos, 0));

            for (int i = vertexCount - 4; i < vertexCount; i++)
                Vector3.Transform(ref vertex[i].Position, ref world, out vertex[i].Position);
        }
        public void flush()
        {
            RasterizerState rast = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.Solid };
            graphicsD.RasterizerState = rast;
            graphicsD.BlendState = BlendState.NonPremultiplied;

            graphicsD.SetRenderTarget(null);
           // graphicsD.Clear(new Color(20, 20, 20));// LevelData.Black);

            if (vertexCount == 0) return;

            effect.CurrentTechnique.Passes[0].Apply();

            graphicsD.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);
            vertexCount = indexCount = 0;

        }
        private void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (indexCount + indexSpace >= index.Length)
                Array.Resize(ref index, Math.Max(indexCount + indexSpace, index.Length * 2));
            if (vertexCount + vertexSpace >= vertex.Length)
                Array.Resize(ref vertex, Math.Max(vertexCount + vertexSpace, vertex.Length * 2));
        }
    }
}
