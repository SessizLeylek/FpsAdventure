using FpsAdventure.Scripts.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsAdventure.Scripts.Engine
{
    public static class Raycasting
    {
        /// <summary>
        /// Checks if a ray intersects a triangle
        /// </summary>
        /// <param name="ray">Ray</param>
        /// <param name="tri">World.Triangle</param>
        /// <param name="distance">Ray's distance to the hit point</param>
        /// <returns>Whether ray hit or not</returns>
        public static bool IntersectsTriangle(Ray ray, Triangle tri, out float distance)
        {
            distance = 0f;
            if (tri == null) return false;

            // Edge vectors
            Vector3 edge1 = tri.v1 - tri.v0;
            Vector3 edge2 = tri.v2 - tri.v0;

            // Compute the determinant
            Vector3 h = Vector3.Cross(ray.Direction, edge2);
            float det = Vector3.Dot(edge1, h);

            // If the determinant is near zero, the ray is parallel to the triangle
            if (det > -1e-6f && det < 1e-6f) return false;

            float invDet = 1.0f / det;

            // Calculate distance from v0 to ray origin
            Vector3 s = ray.Position - tri.v0;

            // Calculate barycentric coordinate u
            float u = Vector3.Dot(s, h) * invDet;
            if (u < 0.0f || u > 1.0f) return false;

            // Calculate barycentric coordinate v
            Vector3 q = Vector3.Cross(s, edge1);
            float v = Vector3.Dot(ray.Direction, q) * invDet;
            if (v < 0.0f || u + v > 1.0f) return false;

            // Compute intersection distance
            distance = Vector3.Dot(edge2, q) * invDet;
            return distance > 0;
        }

        /// <summary>
        /// Returns the closest triangle intersected by ray
        /// </summary>
        public static Triangle ClosestTriangle(Ray ray, Triangle[] triangles, out float distance, float maxDistance = float.MaxValue)
        {
            Triangle closestTri = null;
            float closestDist = float.MaxValue;
            foreach (Triangle tri in triangles) 
            {
                if(IntersectsTriangle(ray, tri, out float distToTri))
                {
                    if(distToTri < maxDistance && distToTri < closestDist)
                    {
                        closestTri = tri;
                        closestDist = distToTri;
                    }
                }
            }

            distance = closestDist;
            return closestTri;
        }

        /// <summary>
        /// Returns the closest triangle intersected by ray
        /// </summary>
        public static Triangle ClosestTriangle(Ray ray, Triangle[] triangles, float maxDistance = float.MaxValue)
        {
            return ClosestTriangle(ray, triangles, out float _dist, maxDistance);
        }
    }

}
