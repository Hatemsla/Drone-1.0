using UnityEngine;

namespace CodeMonkey.Utils
{
    public static class MeshUtils
    {
        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new(0, -1);
        private static readonly Vector3 normal2D = new(0, 0, -1f);


        private static Quaternion[] cachedQuaternionEulerArr;

        private static void CacheQuaternionEuler()
        {
            if (cachedQuaternionEulerArr != null) return;
            cachedQuaternionEulerArr = new Quaternion[360];
            for (var i = 0; i < 360; i++) cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
        }

        private static Quaternion GetQuaternionEuler(float rotFloat)
        {
            var rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;
            //if (rot >= 360) rot -= 360;
            if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
            return cachedQuaternionEulerArr[rot];
        }


        private static Quaternion[] cachedQuaternionEulerXZArr;

        private static void CacheQuaternionEulerXZ()
        {
            if (cachedQuaternionEulerXZArr != null) return;
            cachedQuaternionEulerXZArr = new Quaternion[360];
            for (var i = 0; i < 360; i++) cachedQuaternionEulerXZArr[i] = Quaternion.Euler(0, i, 0);
        }

        private static Quaternion GetQuaternionEulerXZ(float rotFloat)
        {
            var rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;

            if (cachedQuaternionEulerXZArr == null) CacheQuaternionEulerXZ();
            return cachedQuaternionEulerXZArr[rot];
        }


        public static Mesh CreateEmptyMesh()
        {
            var mesh = new Mesh();
            mesh.vertices = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            return mesh;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs,
            out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            return AddToMesh(null, pos, rot, baseSize, uv00, uv11);
        }

        public static Mesh CreateMeshQuad(float size)
        {
            return AddToMesh(null, Vector3.zero, 0f, new Vector3(1, 1) * size, Vector2.zero, Vector2.one);
        }

        public static Mesh CreateMesh(Vector3 pos01, Vector3 pos00, Vector3 pos10, Vector3 pos11)
        {
            var mesh = CreateMeshQuad(1f);

            SetVertexPosition(mesh, 0, pos01);
            SetVertexPosition(mesh, 1, pos00);
            SetVertexPosition(mesh, 2, pos10);
            SetVertexPosition(mesh, 3, pos11);

            return mesh;
        }

        public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            if (mesh == null) mesh = CreateEmptyMesh();
            var vertices = new Vector3[4 + mesh.vertices.Length];
            var uvs = new Vector2[4 + mesh.uv.Length];
            var triangles = new int[6 + mesh.triangles.Length];

            mesh.vertices.CopyTo(vertices, 0);
            mesh.uv.CopyTo(uvs, 0);
            mesh.triangles.CopyTo(triangles, 0);

            var index = vertices.Length / 4 - 1;
            //Relocate vertices
            var vIndex = index * 4;
            var vIndex0 = vIndex;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            baseSize *= .5f;

            var skewed = baseSize.x != baseSize.y;
            if (skewed)
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            }
            else
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            var tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            //mesh.bounds = bounds;

            return mesh;
        }

        public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos,
            float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            //Relocate vertices
            var vIndex = index * 4;
            var vIndex0 = vIndex;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            baseSize *= .5f;

            var skewed = baseSize.x != baseSize.y;
            if (skewed)
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            }
            else
            {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            var tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }

        public static void AddToMeshArraysXZ(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos,
            float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            //Relocate vertices
            var vIndex = index * 4;
            var vIndex0 = vIndex;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            baseSize *= .5f;

            var skewed = baseSize.x != baseSize.z;
            if (skewed)
            {
                vertices[vIndex0] = pos + GetQuaternionEulerXZ(rot) * new Vector3(-baseSize.x, 0, baseSize.z);
                vertices[vIndex1] = pos + GetQuaternionEulerXZ(rot) * new Vector3(-baseSize.x, 0, -baseSize.z);
                vertices[vIndex2] = pos + GetQuaternionEulerXZ(rot) * new Vector3(baseSize.x, 0, -baseSize.z);
                vertices[vIndex3] = pos + GetQuaternionEulerXZ(rot) * baseSize;
            }
            else
            {
                vertices[vIndex0] = pos + GetQuaternionEulerXZ(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEulerXZ(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEulerXZ(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEulerXZ(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            var tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }


        public static Mesh CreateLineMesh(Vector3 pointA, Vector3 pointB, float width)
        {
            return CreateLineMesh(pointA, pointB, normal2D, width);
        }

        public static Mesh CreateLineMesh(Vector3 pointA, Vector3 pointB, Vector3 normal, float width)
        {
            // Creates a Mesh with a Line segment going from pointA to pointB with width
            var mesh = new Mesh();

            var vertices = new Vector3[4]; // 2 vertices per point, one "left" one "right"
            var uv = new Vector2[4];
            var triangles = new int[6]; // 6 triangles to make 2 polygons

            var widthHalf = width * .5f;

            var dirAToB = (pointB - pointA).normalized;
            var vertexALeft = pointA + Vector3.Cross(dirAToB, normal * +1f) * widthHalf;
            var vertexARight = pointA + Vector3.Cross(dirAToB, normal * -1f) * widthHalf;

            var vertexBLeft = pointB + Vector3.Cross(dirAToB, normal * +1f) * widthHalf;
            var vertexBRight = pointB + Vector3.Cross(dirAToB, normal * -1f) * widthHalf;

            var index = 0;
            // Relocate vertices
            var vIndex = index * 4;
            var vIndex0 = vIndex;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            vertices[vIndex0] = vertexALeft;
            vertices[vIndex1] = vertexARight;
            vertices[vIndex2] = vertexBLeft;
            vertices[vIndex3] = vertexBRight;


            // Relocate UVs
            uv[vIndex0] = new Vector2(0, 0);
            uv[vIndex1] = new Vector2(1, 0);
            uv[vIndex2] = new Vector2(0, 1);
            uv[vIndex3] = new Vector2(1, 1);

            // Create triangles
            var tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex2;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            return mesh;
        }

        public static void AddLinePoint(Mesh mesh, Vector3 pointB, float width)
        {
            AddLinePoint(mesh, pointB, normal2D, width);
        }

        public static void AddLinePoint(Mesh mesh, Vector3 pointB, Vector3 normal, float width)
        {
            var lastVertexLeft = mesh.vertices[mesh.vertices.Length - 2];
            var lastVertexRight = mesh.vertices[mesh.vertices.Length - 1];

            var halfDirLastLeftToRight = (lastVertexRight - lastVertexLeft) * .5f;
            var lastPoint = lastVertexLeft + halfDirLastLeftToRight;

            var dirAToB = (pointB - lastPoint).normalized;

            AddLinePoint(mesh, pointB, dirAToB, normal, width);
        }

        public static void AddLinePointForward(Mesh mesh, Vector3 pointB, Vector3 pointBforward, float width)
        {
            AddLinePoint(mesh, pointB, pointBforward, normal2D, width);
        }

        public static void AddLinePoint(Mesh mesh, Vector3 pointB, Vector3 pointBforward, Vector3 normal, float width)
        {
            var vertices = new Vector3[mesh.vertices.Length + 2]; // Add 2 more for pointB Left/Right
            var uv = new Vector2[mesh.uv.Length + 2];
            var triangles = new int[mesh.triangles.Length + 6];

            mesh.vertices.CopyTo(vertices, 0);
            mesh.uv.CopyTo(uv, 0);
            mesh.triangles.CopyTo(triangles, 0);

            var vIndex = vertices.Length - 4;
            var vIndex0 = vIndex + 0;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            /*
            Vector3 lastVertexLeft = vertices[vIndex0];
            Vector3 lastVertexRight = vertices[vIndex1];
    
            Vector3 halfDirLastLeftToRight = (lastVertexRight - lastVertexLeft) * .5f;
            Vector3 lastPoint = lastVertexLeft + halfDirLastLeftToRight;
    
            Vector3 dirAToB = (pointB - lastPoint).normalized;
            */

            var widthHalf = width * .5f;

            var vertexBLeft = pointB + Vector3.Cross(pointBforward, normal * +1f) * widthHalf;
            var vertexBRight = pointB + Vector3.Cross(pointBforward, normal * -1f) * widthHalf;

            vertices[vIndex2] = vertexBLeft;
            vertices[vIndex3] = vertexBRight;

            // Calculate Total Line Length
            var totalLengthUnits = 0f;
            var lastVertexPosition = vertices[0];
            for (var i = 0; i <= vIndex3; i += 2)
            {
                // +=4 to always skip to the same Vertex on the next Quad
                totalLengthUnits += Vector3.Distance(lastVertexPosition, vertices[i]);
                lastVertexPosition = vertices[i];
            }

            // Update UVs based on Total Line Length
            var thisLengthUnits = 0f;
            lastVertexPosition = vertices[0];
            var lastUVy = 0f;
            for (var i = 0; i <= vIndex3; i += 2)
            {
                // +=4 to always skip to the same Vertex on the next Quad
                thisLengthUnits += Vector3.Distance(lastVertexPosition, vertices[i]);
                // Relocate UVs
                var thisUVy = thisLengthUnits / totalLengthUnits;
                uv[i + 0] = new Vector2(0, thisUVy);
                uv[i + 1] = new Vector2(1, thisUVy);
                /*
                uv[i + 0] = new Vector2(0, lastUVy);
                uv[i + 1] = new Vector2(1, lastUVy);
                uv[i + 2] = new Vector2(0, thisUVy);
                uv[i + 3] = new Vector2(1, thisUVy);
                */

                lastVertexPosition = vertices[i];
                lastUVy = thisUVy;
            }


            // Create triangles
            var tIndex = triangles.Length - 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex2;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
        }

        public static void SetVertexPosition(Mesh mesh, int vIndex, Vector3 position)
        {
            var vertices = new Vector3[mesh.vertices.Length];

            mesh.vertices.CopyTo(vertices, 0);

            vertices[vIndex] = position;

            mesh.vertices = vertices;
        }
    }
}