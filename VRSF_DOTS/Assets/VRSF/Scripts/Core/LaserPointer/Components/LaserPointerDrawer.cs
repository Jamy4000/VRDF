using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.LaserPointer
{
    public class LaserPointerDrawer : MonoBehaviour
    {
        public float LineZPosition = 200;
        public Material LineMaterial;

        private EntityManager _entityManager;

        private struct LaserPointerGroup
        {
            public LaserPointerLength PointerLength;
        }

        private void Awake()
        {
            _entityManager = World.Active.EntityManager;
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            // Apply the line material
            LineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            
            GL.Color(new Color(1, 1, 1, 1));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of pointer
            GL.Vertex3(0, 0, LineZPosition);

            GL.End();
            GL.PopMatrix();
        }
    }
}