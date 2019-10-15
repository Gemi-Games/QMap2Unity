/**
 * 
 * MeshStructs.cs and ClipMesh.cs are adapted directly from the pdf file linked below:
 * 
 * https://www.geometrictools.com/Documentation/ClipMesh.pdf
 * 
 * Clipping a Mesh Against a Plane
 * David Eberly, Geometric Tools, Redmond WA 98052
 * https://www.geometrictools.com/
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.To view a copy
 * of this license, visit http://creativecommons.org/licenses/by/4.0/ or send a letter to Creative Commons,
 * PO Box 1866, Mountain View, CA 94042, USA.
 * 
 * Author Created:  February 28, 2002
 * Author Last Modified:  March 1, 2008
 * 
 * Q2U Created:  February 2, 2019
 * Q2U Last Modified:  October 15, 2019
 * 
**/

using System.Collections.Generic;
using UnityEngine;

namespace QMapToUnity
{
    public struct ConvexMeshData
    {
        public UMeshData MeshData;
        public Vector3 DeltaPos;

        public ConvexMeshData(UMeshData lMeshData, Vector3 lDeltaPos)
        {
            MeshData = lMeshData;
            DeltaPos = lDeltaPos;
        }
    }

    public struct UMeshData
    {
        public Vector3[] UVVertices;
        public Vector3[] PosVertices;
        public FaceInfo[] FaceInfos;
        public FaceInfo[][] MeshFaceInfos;

        public UMeshData(Vector3[] lVertices, FaceInfo[] lFaceInfos)
        {
            UVVertices = lVertices;
            PosVertices = new Vector3[lVertices.Length];
            FaceInfos = lFaceInfos;

            MeshFaceInfos = null;
        }
    }

    public struct TextureData
    {
        public string Name;
        public Vector2 Offset;
        public Vector2 Scale;
        public float Angle;

        public TextureData(string lName)
        {
            Name = lName;
            Offset = Vector2.zero;
            Scale = Vector2.one;
            Angle = 0f;
        }

        public TextureData SetValues(Vector2 lTexOffset, Vector2 lTexScale, float lTexAngle)
        {
            Offset = lTexOffset;
            Scale = lTexScale;
            Angle = lTexAngle;

            return this;
        }

        public TextureData SetOffset(Vector2 lTexOffset)
        {
            Offset = lTexOffset;

            return this;
        }

        public TextureData SetScale(Vector2 lTexScale)
        {
            Scale = lTexScale;

            return this;
        }

        public TextureData SetAngle(float lTexAngle)
        {
            Angle = lTexAngle;

            return this;
        }
    }

    public struct Plane
    {
        public Vector3 Normal;
        public float C;

        public TextureData TextureData;

        public Plane(Vector3 lNormal, float lC)
        {
            Normal = lNormal;
            C = lC;

            TextureData = new TextureData("N/A");
        }
    }

    public struct FaceInfo
    {
        public Plane Plane;
        public int[] Indices;
    }

    public struct AMesh
    {
        public AVertex[] V;
        public AEdge[] E;
        public AFace[] F;
    }

    public struct AVertex
    {
        public Vector3 Position;

        public AVertex(Vector3 lPosition)
        {
            Position = lPosition;
        }
    }

    public struct AEdge
    {
        public int[] Vertices;
        public int[] Faces;

        public AEdge(int[] lVertices, int[] lFaces)
        {
            Vertices = lVertices;
            Faces = lFaces;
        }
    }

    public struct AFace
    {
        public int[] Edges;
        public Plane Plane;

        public AFace(int[] lEdges, Plane lPlane)
        {
            Edges = lEdges;
            Plane = lPlane;
        }
    }

    public struct CMesh
    {
        public CVertex[] V;
        public CEdge[] E;
        public CFace[] F;

        public CMesh(AMesh lMesh)
        {
            V = new CVertex[lMesh.V.Length];
            for (int i = 0; i < lMesh.V.Length; i++)
                V[i] = new CVertex(lMesh.V[i].Position);

            E = new CEdge[lMesh.E.Length];
            for (int i = 0; i < lMesh.E.Length; i++)
            {
                int[] lVerts = new int[2] { lMesh.E[i].Vertices[0], lMesh.E[i].Vertices[1] };
                int[] lFaces = new int[2] { lMesh.E[i].Faces[0], lMesh.E[i].Faces[1] };
                E[i] = new CEdge(lVerts, lFaces);
            }

            F = new CFace[lMesh.F.Length];
            for (int i = 0; i < lMesh.F.Length; i++)
            {
                int[] lEdges = new int[lMesh.F[i].Edges.Length];

                for (int j = 0; j < lMesh.F[i].Edges.Length; j++)
                    lEdges[j] = lMesh.F[i].Edges[j];

                F[i] = new CFace(lEdges, lMesh.F[i].Plane);
            }
        }

        public void AppendVertex(CVertex lNewVertex)
        {
            CVertex[] lTemp = new CVertex[V.Length + 1];

            for (int i = 0; i < V.Length; i++)
                lTemp[i] = V[i];

            lTemp[V.Length] = lNewVertex;

            V = lTemp;
        }

        public void AppendEdge(CEdge lNewEdge)
        {
            CEdge[] lTemp = new CEdge[E.Length + 1];

            for (int i = 0; i < E.Length; i++)
                lTemp[i] = E[i];

            lTemp[E.Length] = lNewEdge;

            E = lTemp;
        }

        public void AppendFace(CFace lNewFace)
        {
            CFace[] lTemp = new CFace[F.Length + 1];

            for (int i = 0; i < F.Length; i++)
                lTemp[i] = F[i];

            lTemp[F.Length] = lNewFace;

            F = lTemp;
        }

        public UMeshData GetMeshInformation(Vector3 lPositionDelta)
        {
            List<Vector3> lPoints = new List<Vector3>(V.Length);
            List<int> lVMap = new List<int>(V.Length);

            for (int i = 0; i < V.Length; i++)
                lVMap.Add(-1);

            for (int i = 0; i < V.Length; i++)
            {
                if (V[i].Visible)
                {
                    lVMap[i] = lPoints.Count;
                    lPoints.Add(V[i].Position);
                }
            }

            int[] lFaces = GetOrderedFaces();

            for (int i = 0; i < lFaces.Length;)
            {
                int lNumIndices = lFaces[i];
                i++;

                for (int j = 0; j < lNumIndices; j++)
                {
                    lFaces[i] = lVMap[lFaces[i]];
                    i++;
                }
            }

            int lFaceCount = 0;

            for (int i = 0; i < F.Length; i++)
            {
                if (F[i].Visible)
                    lFaceCount++;
            }

            FaceInfo[] lFaceInfos = new FaceInfo[lFaceCount];
            int lFaceIndex = 0;
            int lVisibleFaceCounter = 0;

            for (int i = 0; i < F.Length; i++)
            {
                if (F[i].Visible)
                {
                    lFaceInfos[lVisibleFaceCounter] = new FaceInfo();

                    lFaceInfos[lVisibleFaceCounter].Plane = F[i].Plane;

                    int lIndicesCount = lFaces[lFaceIndex];

                    int[] lIndices = new int[lIndicesCount];

                    for (int j = 0; j < lIndicesCount; j++)
                        lIndices[j] = lFaces[lFaceIndex + j + 1];

                    lFaceIndex += lIndicesCount + 1;

                    lFaceInfos[lVisibleFaceCounter].Indices = lIndices;

                    lVisibleFaceCounter++;
                }
            }

            UMeshData lMeshData = new UMeshData(lPoints.ToArray(), lFaceInfos);

            for (int v = 0; v < lMeshData.UVVertices.Length; v++)
            {
                lMeshData.PosVertices[v].x = lMeshData.UVVertices[v].x - lPositionDelta.x;
                lMeshData.PosVertices[v].y = lMeshData.UVVertices[v].y - lPositionDelta.y;
                lMeshData.PosVertices[v].z = lMeshData.UVVertices[v].z - lPositionDelta.z;
            }

            return lMeshData;
        }

        private int[] GetOrderedFaces()
        {
            List<int> lFaces = new List<int>();

            for (int i = 0; i < F.Length; i++)
            {
                if (F[i].Visible)
                {
                    int[] lVerts = GetOrderedVertices(F[i]);
                    lFaces.Add(lVerts.Length - 1);

                    if (Vector3.Dot(F[i].Plane.Normal, GetNormal(lVerts)) > 0)
                    {
                        for (int j = lVerts.Length - 2; j >= 0; j--)
                            lFaces.Add(lVerts[j]);
                    }
                    else
                    {
                        for (int j = 0; j <= lVerts.Length - 2; j++)
                            lFaces.Add(lVerts[j]);
                    }
                }
            }

            return lFaces.ToArray();
        }

        private int[] GetOrderedVertices(CFace lFace)
        {
            List<int> lEdges = new List<int>(lFace.Edges.Length);

            for (int j = 0; j < lFace.Edges.Length; j++)
                lEdges.Add(lFace.Edges[j]);

            for (int i0 = 0, i1 = 1, lChoice = 1; i1 < lEdges.Count - 1; i0 = i1, i1++)
            {
                int lCurrent = E[lEdges[i0]].Vertices[lChoice];
                for (int j = i1; j < lEdges.Count; j++)
                {
                    //CEdge lEdge = E[lEdges[j]];
                    if (E[lEdges[j]].Vertices[0] == lCurrent)
                    {
                        int lTemp = lEdges[i1];
                        lEdges[i1] = lEdges[j];
                        lEdges[j] = lTemp;

                        lChoice = 1;
                        break;
                    }
                    if (E[lEdges[j]].Vertices[1] == lCurrent)
                    {
                        int lTemp = lEdges[i1];
                        lEdges[i1] = lEdges[j];
                        lEdges[j] = lTemp;

                        lChoice = 0;
                        break;
                    }
                }
            }

            List<int> lVerts = new List<int>(lEdges.Count + 1);

            lVerts.Add(E[lEdges[0]].Vertices[0]);
            lVerts.Add(E[lEdges[0]].Vertices[1]);

            for (int i = 1; i < lEdges.Count; i++)
            {
                if (E[lEdges[i]].Vertices[0] == lVerts[i])
                    lVerts.Add(E[lEdges[i]].Vertices[1]);
                else
                    lVerts.Add(E[lEdges[i]].Vertices[0]);
            }

            return lVerts.ToArray();
        }

        private Vector3 GetNormal(int[] lVerts)
        {
            Vector3 lNormal = Vector3.zero;

            for (int i = 0; i < lVerts.Length - 1; i++)
                lNormal += Vector3.Cross(V[lVerts[i]].Position, V[lVerts[i + 1]].Position);

            lNormal.Normalize();

            return lNormal;
        }
    }

    public struct CVertex
    {
        public Vector3 Position;
        public float Distance;
        public int Occurs;
        public bool Visible;

        public CVertex(Vector3 lPosition)
        {
            Position = lPosition;

            Distance = 0f;
            Occurs = 0;
            Visible = true;
        }
    }

    public struct CEdge
    {
        public int[] Vertices;
        public int[] Faces;
        public bool Visible;

        public CEdge(int[] lVertices, int[] lFaces)
        {
            Vertices = lVertices;
            Faces = lFaces;

            Visible = true;
        }

        public void AddFace(int lFaceIndex)
        {
            if (Faces == null)
                Faces = new int[0];

            int[] lTemp = new int[Faces.Length + 1];

            for (int i = 0; i < Faces.Length; i++)
                lTemp[i] = Faces[i];

            lTemp[Faces.Length] = lFaceIndex;

            Faces = lTemp;
        }
    }

    public struct CFace
    {
        public int[] Edges;
        public Plane Plane;
        public bool Visible;

        public CFace(int[] lEdges, Plane lPlane)
        {
            Edges = lEdges;
            Plane = lPlane;

            Visible = true;
        }

        public void RemoveEdge(int lEdgeIndex)
        {
            int lFoundIndex = -1;

            for (int i = 0; i < Edges.Length; i++)
            {
                if (Edges[i] == lEdgeIndex)
                {
                    lFoundIndex = i;
                    break;
                }
            }

            if (lFoundIndex >= 0)
            {
                int[] lTemp = new int[Edges.Length - 1];

                if (lTemp.Length == 0)
                {
                    Edges = lTemp;
                    return;
                }

                int lMinusNumber = 0;

                for (int i = 0; i < Edges.Length; i++)
                {
                    if (i != lFoundIndex)
                        lTemp[i - lMinusNumber] = Edges[i];
                    else
                        lMinusNumber = 1;
                }

                Edges = lTemp;
            }
        }

        public void AddEdge(int lEdgeIndex)
        {
            if (Edges == null)
                Edges = new int[0];

            int[] lTemp = new int[Edges.Length + 1];

            for (int i = 0; i < Edges.Length; i++)
                lTemp[i] = Edges[i];

            lTemp[Edges.Length] = lEdgeIndex;

            Edges = lTemp;
        }
    }
}