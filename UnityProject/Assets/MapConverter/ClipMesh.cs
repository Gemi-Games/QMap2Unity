/*
 * 
ClipMeshStructs.cs and ClipMesh.cs are adapted directly from the pdf file linked below:

https://www.geometrictools.com/Documentation/ClipMesh.pdf

Clipping a Mesh Against a Plane
David Eberly, Geometric Tools, Redmond WA 98052
https://www.geometrictools.com/

This work is licensed under the Creative Commons Attribution 4.0 International License.To view a copy
of this license, visit http://creativecommons.org/licenses/by/4.0/ or send a letter to Creative Commons,
PO Box 1866, Mountain View, CA 94042, USA.

Created:  February 28, 2002
Last Modified:  March 1, 2008

*/

using UnityEngine;

namespace QMapToUnity
{
    public enum ClipOperationResult
    {
        AllPositive,
        AllNegative,
        Mixed,
    }

    public static class ClipMesh
    {
        private static float s_Epsilon = 0.001f;

        public static CMesh CreateConvexPolyhedron(Plane[] lPlanes)
        {
            return CreateConvexPolyhedron(lPlanes, Vector3.zero, Vector3.one * 8191f);
        }

        public static CMesh CreateConvexPolyhedron(Plane[] lPlanes, Vector3 lCentre, Vector3 lSize)
        {
            AMesh lMeshToClip = CreateAMeshBox(lCentre, lSize);

            CMesh lClippedMesh = new CMesh(lMeshToClip);

            for (int i = 0; i < lPlanes.Length; i++)
            {
                ClipOperationResult lResult = Clip(ref lClippedMesh, lPlanes[i]);

                if (lResult == ClipOperationResult.AllNegative)
                    break;
            }

            return lClippedMesh;
        }

        private static AMesh CreateAMeshBox(Vector3 lCentre, Vector3 lSize)
        {
            Vector3 lHalfSize = lSize * 0.5f;
            Vector3 lMin = lCentre - lHalfSize;
            Vector3 lMax = lCentre + lHalfSize;

            AVertex[] lVerts = new AVertex[8];
            lVerts[0] = new AVertex(new Vector3(lMin.x, lMax.y, lMin.z));
            lVerts[1] = new AVertex(new Vector3(lMax.x, lMax.y, lMin.z));
            lVerts[2] = new AVertex(new Vector3(lMax.x, lMin.y, lMin.z));
            lVerts[3] = new AVertex(new Vector3(lMin.x, lMin.y, lMin.z));

            lVerts[4] = new AVertex(new Vector3(lMin.x, lMax.y, lMax.z));
            lVerts[5] = new AVertex(new Vector3(lMax.x, lMax.y, lMax.z));
            lVerts[6] = new AVertex(new Vector3(lMax.x, lMin.y, lMax.z));
            lVerts[7] = new AVertex(new Vector3(lMin.x, lMin.y, lMax.z));

            AEdge[] lEdges = new AEdge[12];
            lEdges[0] = new AEdge(new int[2] { 0, 1 }, new int[2] { 0, 1 });
            lEdges[1] = new AEdge(new int[2] { 1, 2 }, new int[2] { 0, 2 });
            lEdges[2] = new AEdge(new int[2] { 2, 3 }, new int[2] { 0, 3 });
            lEdges[3] = new AEdge(new int[2] { 3, 0 }, new int[2] { 0, 4 });

            lEdges[4] = new AEdge(new int[2] { 0, 4 }, new int[2] { 4, 1 });
            lEdges[5] = new AEdge(new int[2] { 1, 5 }, new int[2] { 1, 2 });
            lEdges[6] = new AEdge(new int[2] { 2, 6 }, new int[2] { 2, 3 });
            lEdges[7] = new AEdge(new int[2] { 3, 7 }, new int[2] { 3, 4 });

            lEdges[8] = new AEdge(new int[2] { 4, 5 }, new int[2] { 1, 5 });
            lEdges[9] = new AEdge(new int[2] { 5, 6 }, new int[2] { 2, 5 });
            lEdges[10] = new AEdge(new int[2] { 6, 7 }, new int[2] { 3, 5 });
            lEdges[11] = new AEdge(new int[2] { 7, 4 }, new int[2] { 4, 5 });

            AFace[] lFaces = new AFace[6];
            lFaces[0] = new AFace(new int[4] { 0, 1, 2, 3 }, new Plane(Vector3.forward, lMin.z));
            lFaces[1] = new AFace(new int[4] { 0, 4, 8, 5 }, new Plane(Vector3.down, -lMax.y));
            lFaces[2] = new AFace(new int[4] { 1, 6, 9, 5 }, new Plane(Vector3.left, -lMax.x));
            lFaces[3] = new AFace(new int[4] { 2, 7, 10, 6 }, new Plane(Vector3.up, lMin.y));
            lFaces[4] = new AFace(new int[4] { 3, 7, 11, 4 }, new Plane(Vector3.right, lMin.x));
            lFaces[5] = new AFace(new int[4] { 8, 9, 10, 11 }, new Plane(Vector3.back, -lMax.z));

            AMesh lMesh = new AMesh();

            lMesh.V = lVerts;
            lMesh.E = lEdges;
            lMesh.F = lFaces;

            return lMesh;
        }

        private static ClipOperationResult Clip(ref CMesh lCMesh, Plane lPlane)
        {
            ClipOperationResult lResult = ProcessVertices(ref lCMesh, lPlane);

            if (lResult != ClipOperationResult.Mixed)
                return lResult;

            ProcessEdges(ref lCMesh, lPlane);

            ProcessFaces(ref lCMesh, lPlane);

            return lResult;
        }

        private static ClipOperationResult ProcessVertices(ref CMesh lCMesh, Plane lPlane)
        {
            int lPosCount = 0;
            int lNegCount = 0;

            for (int i = 0; i < lCMesh.V.Length; i++)
            {
                CVertex lV = lCMesh.V[i];

                if (lV.Visible)
                {
                    lV.Distance = Vector3.Dot(lPlane.Normal, lCMesh.V[i].Position) - lPlane.C;

                    if (lV.Distance >= s_Epsilon)
                    {
                        lPosCount++;
                    }
                    else if (lV.Distance <= -s_Epsilon)
                    {
                        lNegCount++;
                        lV.Visible = false;
                    }
                    else
                    {
                        lV.Distance = 0f;
                    }

                    lCMesh.V[i] = lV;
                }
            }

            if (lNegCount == 0)
                return ClipOperationResult.AllPositive;

            if (lPosCount == 0)
                return ClipOperationResult.AllNegative;

            return ClipOperationResult.Mixed;
        }

        private static void ProcessEdges(ref CMesh lCMesh, Plane lPlane)
        {
            for (int i = 0; i < lCMesh.E.Length; i++)
            {
                CEdge lE = lCMesh.E[i];

                if (lE.Visible)
                {
                    float lD0 = lCMesh.V[lE.Vertices[0]].Distance;
                    float lD1 = lCMesh.V[lE.Vertices[1]].Distance;

                    if (lD0 <= 0f && lD1 <= 0f)
                    {
                        for (int j = 0; j < lE.Faces.Length; j++)
                        {
                            CFace lF = lCMesh.F[lE.Faces[j]];
                            lF.RemoveEdge(i);
                            if (lF.Edges.Length == 0)
                                lF.Visible = false;

                            lCMesh.F[lE.Faces[j]] = lF;
                        }

                        lE.Visible = false;
                        lCMesh.E[i] = lE;

                        continue;
                    }

                    if (lD0 >= 0f && lD1 >= 0f)
                    {
                        lCMesh.E[i] = lE;
                        continue;
                    }

                    float lT = lD0 / (lD0 - lD1);
                    Vector3 lDelta0 = (1f - lT) * lCMesh.V[lE.Vertices[0]].Position;
                    Vector3 lDelta1 = lT * lCMesh.V[lE.Vertices[1]].Position;
                    Vector3 lIntersect = lDelta0 + lDelta1;

                    int lIndex = lCMesh.V.Length;

                    CVertex lNewVertex = new CVertex(lIntersect);

                    lCMesh.AppendVertex(lNewVertex);

                    if (lD0 > 0)
                        lE.Vertices[1] = lIndex;
                    else
                        lE.Vertices[0] = lIndex;

                    lCMesh.E[i] = lE;
                }
            }
        }

        private static void ProcessFaces(ref CMesh lCMesh, Plane lPlane)
        {
            CFace lNewFace = new CFace();

            lNewFace.Plane = lPlane;
            lNewFace.Visible = true;

            int lNewFaceIndex = lCMesh.F.Length;

            for (int i = 0; i < lCMesh.F.Length; i++)
            {
                CFace lF = lCMesh.F[i];

                if (lF.Visible)
                {
                    for (int j = 0; j < lF.Edges.Length; j++)
                    {
                        int lEdgeIndex = lF.Edges[j];

                        lCMesh.V[lCMesh.E[lEdgeIndex].Vertices[0]].Occurs = 0;
                        lCMesh.V[lCMesh.E[lEdgeIndex].Vertices[1]].Occurs = 0;
                    }

                    int lStart, lEnd;
                    if (GetOpenPolyline(lCMesh, lF, out lStart, out lEnd))
                    {
                        CEdge lNewEdge = new CEdge();
                        int lNewEdgeIndex = lCMesh.E.Length;

                        lNewEdge.Vertices = new int[2];
                        lNewEdge.Vertices[0] = lStart;
                        lNewEdge.Vertices[1] = lEnd;

                        lNewEdge.AddFace(i);
                        lNewEdge.AddFace(lNewFaceIndex);

                        lNewEdge.Visible = true;

                        lF.AddEdge(lNewEdgeIndex);
                        lNewFace.AddEdge(lNewEdgeIndex);

                        lCMesh.AppendEdge(lNewEdge);
                    }

                    lCMesh.F[i] = lF;
                }
            }

            lCMesh.AppendFace(lNewFace);
        }

        private static bool GetOpenPolyline(CMesh lCMesh, CFace lF, out int lStart, out int lEnd)
        {
            for (int i = 0; i < lF.Edges.Length; i++)
            {
                int lEdgeIndex = lF.Edges[i];

                lCMesh.V[lCMesh.E[lEdgeIndex].Vertices[0]].Occurs++;
                lCMesh.V[lCMesh.E[lEdgeIndex].Vertices[1]].Occurs++;
            }

            lStart = -1;
            lEnd = -1;

            for (int i = 0; i < lF.Edges.Length; i++)
            {
                int lEdgeIndex = lF.Edges[i];

                int lI0 = lCMesh.E[lEdgeIndex].Vertices[0];
                int lI1 = lCMesh.E[lEdgeIndex].Vertices[1];
                if (lCMesh.V[lI0].Occurs == 1)
                {
                    if (lStart == -1)
                        lStart = lI0;
                    else if (lEnd == -1)
                        lEnd = lI0;
                }

                if (lCMesh.V[lI1].Occurs == 1)
                {
                    if (lStart == -1)
                        lStart = lI1;
                    else if (lEnd == -1)
                        lEnd = lI1;
                }
            }

            return lStart != -1;
        }
    }
}