using Game2D.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.OpenGL
{
    public class RenderRectangle
    {
        private VertexBufferObject vbo;
        private uint[] indices;
        private Vertex[] vertices;

        public RenderRectangle()
        {
            vbo = new VertexBufferObject();
            indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };

            vertices = new Vertex[] {
                new Vertex(new Vector3(-1,-1,0), new Vector2(0,1)),
                new Vertex(new Vector3(1,-1,0), new Vector2(1,1)),
                new Vertex(new Vector3(1,1,0), new Vector2(1,0)),
                new Vertex(new Vector3(-1,1,0), new Vector2(0,0))
            };

            vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);

            vbo.BufferData(vertices, Vertex.SizeInBytes);
            vbo.BufferIndices(indices);
        }

        public void Draw(float deltaTime)
        {
            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            vbo.End();
        }
    }
}