using UnityEngine;

namespace QMapToUnity
{
    public class UMeshCreator
    {
        private enum AutoUVDirection
        {
            XPos,
            XNeg,
            YPos,
            YNeg,
            ZPos,
            ZNeg,
        }

        private static Vector3[] s_Directions = new Vector3[6]
        {
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down,
        Vector3.forward,
        Vector3.back,
        };

        private static float[] s_DirectionTolerance = new float[6]
        {
        0.001f,
        0.001f,
        0.002f,
        0.002f,
        0f,
        0f,
        };

        public static Mesh[] ConvertToMeshes(UMeshData lMeshData)
        {
            Vector3[] lPosVerts = lMeshData.PosVertices;
            Vector3[] lUVVerts = lMeshData.UVVertices;

            int lMeshCount = lMeshData.MeshFaceInfos.Length;
            Mesh[] lMeshes = new Mesh[lMeshCount];

            for (int m = 0; m < lMeshCount; m++)
            {
                Mesh lMesh = new Mesh();

                FaceInfo[] lFaceInfos = lMeshData.MeshFaceInfos[m];

                int lVertCount = 0;
                int lTriCount = 0;

                for (int f = 0; f < lFaceInfos.Length; f++)
                {
                    lVertCount += lFaceInfos[f].Indices.Length;

                    if (lFaceInfos[f].Indices.Length > 2)
                        lTriCount += lFaceInfos[f].Indices.Length - 2;
                }

                int[] lTriangles = new int[lTriCount * 3];

                int lTriIndex = 0;

                if (m == 0)
                {
                    for (int i = 0; i < lFaceInfos.Length; i++)
                    {
                        for (int j = 0; j < lFaceInfos[i].Indices.Length - 2; j++)
                        {
                            lTriangles[lTriIndex++] = lFaceInfos[i].Indices[0];
                            lTriangles[lTriIndex++] = lFaceInfos[i].Indices[j + 1];
                            lTriangles[lTriIndex++] = lFaceInfos[i].Indices[j + 2];
                        }
                    }

                    lMesh.vertices = lMeshData.PosVertices;
                    lMesh.triangles = lTriangles;

                    lMesh.RecalculateNormals();
                    lMesh.RecalculateTangents();
                    lMesh.RecalculateBounds();

                    lMesh.name = "Collider Mesh";

                    lMeshes[0] = lMesh;
                }
                else
                {
                    Vector3[] lNewVerts = new Vector3[lVertCount];
                    Vector3[] lNormals = new Vector3[lVertCount];
                    Vector2[] lUVs = new Vector2[lVertCount];

                    int lVertIndex = 0;

                    int[][] lNewIndices = new int[lFaceInfos.Length][];

                    for (int f = 0; f < lFaceInfos.Length; f++)
                    {
                        FaceInfo lFaceInfo = lFaceInfos[f];
                        Plane lPlane = lFaceInfo.Plane;
                        TextureData lTexData = lPlane.TextureData;
                        int[] lIndices = lFaceInfo.Indices;

                        float lMinAngle = 360f;
                        AutoUVDirection lDir = AutoUVDirection.XPos;

                        for (int j = 0; j < s_Directions.Length; j++)
                        {
                            float lAngle = Vector3.Angle(-lPlane.Normal, s_Directions[j]) - s_DirectionTolerance[j];

                            if (lMinAngle > lAngle)
                            {
                                lMinAngle = lAngle;
                                lDir = (AutoUVDirection)j;
                            }
                        }

                        lNewIndices[f] = new int[lIndices.Length];

                        for (int j = 0; j < lIndices.Length; j++)
                        {
                            Vector3 lVertPos = lPosVerts[lIndices[j]];
                            Vector3 lVertUV = lUVVerts[lIndices[j]];

                            //lVertPos.x = (int)((lVertPos.x + 0.5f) * 64f + 0f) / 64f;
                            //lVertPos.y = (int)((lVertPos.y + 0.5f) * 64f + 0f) / 64f;
                            //lVertPos.z = (int)((lVertPos.z + 0.5f) * 64f + 0f) / 64f;

                            lNewVerts[lVertIndex] = lVertPos;
                            lNormals[lVertIndex] = -lPlane.Normal;

                            lUVs[lVertIndex] = GetProjectionCoord(lDir, lVertUV);

                            lUVs[lVertIndex] = Quaternion.Euler(0f, 0f, -lTexData.Angle) * lUVs[lVertIndex];

                            lUVs[lVertIndex] *= lTexData.Scale;

                            lUVs[lVertIndex] += lTexData.Offset;

                            lNewIndices[f][j] = lVertIndex;

                            lVertIndex++;
                        }

                        for (int j = 0; j < lNewIndices[f].Length - 2; j++)
                        {
                            lTriangles[lTriIndex++] = lNewIndices[f][0];
                            lTriangles[lTriIndex++] = lNewIndices[f][j + 1];
                            lTriangles[lTriIndex++] = lNewIndices[f][j + 2];
                        }
                    }

                    if (lVertIndex >= 3)
                    {
                        lMesh.vertices = lNewVerts;
                        lMesh.triangles = lTriangles;
                        lMesh.normals = lNormals;
                        lMesh.uv = lUVs;

                        lMesh.RecalculateTangents();
                        lMesh.RecalculateBounds();

                        lMesh.name = "Visible Mesh";

                        lMeshes[m] = lMesh;
                    }
                    else
                    {
                        lMeshes[m] = null;
                    }
                }
            }

            return lMeshes;
        }

        private static Vector2 GetProjectionCoord(AutoUVDirection lDir, Vector3 lVertPos)
        {
            if (lDir == AutoUVDirection.XPos || lDir == AutoUVDirection.XNeg)
            {
                return new Vector2(lVertPos.z, lVertPos.y);
            }
            else if (lDir == AutoUVDirection.YPos || lDir == AutoUVDirection.YNeg)
            {
                return new Vector2(lVertPos.x, lVertPos.z);
            }
            else if (lDir == AutoUVDirection.ZPos || lDir == AutoUVDirection.ZNeg)
            {
                return new Vector2(lVertPos.x, lVertPos.y);
            }

            return Vector2.zero;
        }
    }
}