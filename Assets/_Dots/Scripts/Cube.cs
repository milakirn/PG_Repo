using UnityEngine;

namespace Dots.JobsScripting
{
    public class Cube : MonoBehaviour
    {
        public Vector3[] Closest { get; set; }
        public Vector3 TheLast { get; set; }

        private void OnDrawGizmosSelected()
        {
            if (Closest != null)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < 3; i++)
                {
                    Gizmos.DrawLine(transform.position, Closest[i]);
                }
            }

            Gizmos.color = Color.white;

            Gizmos.DrawLine(transform.position, TheLast);
        }

        public void SetClosestPositions(Vector3[] positions)
        {
            Closest = positions;
        }

        public void SetFarthestPosition(Vector3 position)
        {
            TheLast = position;
        }
    }
}