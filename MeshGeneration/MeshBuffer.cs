using UnityEngine;
namespace RobertHoudin.MeshGeneration
{
    public class MeshBuffer
    {
        public Vector3[] vertices;
        public int[] indices;
        
        public MeshBuffer ( ) {}
        public MeshBuffer(int vertexCount, int indexCount)
        {
            vertices = new Vector3[vertexCount];
            indices = new int[indexCount];
        }

        public MeshBuffer(Vector3[] vertices, int[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }
        
        
    }
}