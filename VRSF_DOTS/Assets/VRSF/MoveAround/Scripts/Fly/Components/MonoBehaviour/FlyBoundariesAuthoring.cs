using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Display lines for the Fly Boundaries in Edit Mode.
    /// 
    /// As it's using OnPostRender and OnDrawGizmos methods, derived from monoBehaviour, 
    /// and it's only a UnityEditor Script, this wasn't refactored to fit the Hybrid System.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(FlyModeAuthoring))]
	public class FlyBoundariesAuthoring : MonoBehaviour
    {
        [Header("Boundaries Parameters")]
        [Tooltip("The minimun local position at which the user can go. Set values to zero for no minimum boundaries.")]
        public Vector3 MinAvatarPosition = new Vector3(0.0f, 0.0f, 0.0f);

        [Tooltip("The maximum local position at which the user can go. Set values to zero for no minimum boundaries.")]
        public Vector3 MaxAvatarPosition = new Vector3(0.0f, 0.0f, 0.0f);

        private void Start()
        {
            if (Application.isPlaying)
                Destroy(this);
        }


#if UNITY_EDITOR
        [Header("Boundaries Material & Color")]
        [Tooltip("Choose the Unlit/Color shader in the Material Settings. You can change the color of the connecting lines through this mat.")]
        public Material LineMat;

        [Tooltip("The color of the Bounding box displayed in the Scene view for this FlyBoundariesAuthoring.")]
        public Color BoundariesLinesColor = Color.green;

        private FlyModeAuthoring _flyComp;

        /// <summary>
        /// To show the lines in the editor
        /// </summary>
        void OnDrawGizmos()
        {
            if (LineMat != null && (MinAvatarPosition != Vector3.zero || MaxAvatarPosition != Vector3.zero))
                DrawConnectingLines();
        }

        /// <summary>
        /// Connect all the points together by getting there coordinates from the FlyingParamertersVariable
        /// </summary>
        void DrawConnectingLines()
        {
            // List of points/vertices
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(MinAvatarPosition.x, MinAvatarPosition.y, MinAvatarPosition.z),
                new Vector3(MinAvatarPosition.x, MaxAvatarPosition.y, MinAvatarPosition.z),
                new Vector3(MinAvatarPosition.x, MaxAvatarPosition.y, MaxAvatarPosition.z),
                new Vector3(MinAvatarPosition.x, MinAvatarPosition.y, MaxAvatarPosition.z),
                new Vector3(MaxAvatarPosition.x, MinAvatarPosition.y, MinAvatarPosition.z),
                new Vector3(MaxAvatarPosition.x, MaxAvatarPosition.y, MinAvatarPosition.z),
                new Vector3(MaxAvatarPosition.x, MaxAvatarPosition.y, MaxAvatarPosition.z),
                new Vector3(MaxAvatarPosition.x, MinAvatarPosition.y, MaxAvatarPosition.z),
            };

            // List of indices/Vector2 between which a line must be made
            Vector2[] indices = new Vector2[12]
            {
                new Vector2(0, 1),
                new Vector2(1, 2),
                new Vector2(2, 3),
                new Vector2(3, 0),

                new Vector2(4, 5),
                new Vector2(5, 6),
                new Vector2(6, 7),
                new Vector2(7, 4),

                new Vector2(0, 4),
                new Vector2(1, 5),
                new Vector2(2, 6),
                new Vector2(3, 7),
            };
            
            // Loop through each indices to connect the points together
            foreach (Vector2 i in indices)
            {
                GL.Begin(GL.LINES);
                LineMat.color = BoundariesLinesColor;
                LineMat.SetPass(0);
                GL.Color(new Color(LineMat.color.r, LineMat.color.g, LineMat.color.b, LineMat.color.a));
                GL.Vertex3(vertices[(int)i.x].x, vertices[(int)i.x].y, vertices[(int)i.x].z);
                GL.Vertex3(vertices[(int)i.y].x, vertices[(int)i.y].y, vertices[(int)i.y].z);
                GL.End();
            }

            Vector3 labelPos = new Vector3(MinAvatarPosition.x, MaxAvatarPosition.y, MinAvatarPosition.z);
            GUIStyle style = new GUIStyle();

            style.normal.textColor = BoundariesLinesColor;

            UnityEditor.Handles.Label(labelPos, "Flying Boundaries", style);
        }
#endif
    }
}