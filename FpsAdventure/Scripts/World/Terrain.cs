using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsAdventure.Scripts.World
{
    public static class Terrain
    {
        public static Triangle[] groundTriangles = new Triangle[]
        {
            new Triangle(new Vector3(-100, 0, -50), new Vector3(100, 0, -50), new Vector3(0, 0, 50)),
            new Triangle(new Vector3(-10, 0, 5), new Vector3(10, 0, 5), new Vector3(0, 5, 15))
        };

    }

    public class Triangle
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;

        public Triangle(Vector3 _v0, Vector3 _v1, Vector3 _v2)
        {
            this.v0 = _v0;
            this.v1 = _v1;
            this.v2 = _v2;
        }
    }
}
