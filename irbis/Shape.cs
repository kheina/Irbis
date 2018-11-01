﻿using Irbis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

public struct Shape
{
    public Line[] Lines
    {
        get { return lines; }
    }
    public Vector2[] Vertices
    {
        get { return vertices; }
        set
        {
            vertices = value;
            outlined = fail = triangulated = false;
        }
    }
    public int NumberOfLines
    {
        get { return lines.Length; }
    }
    public Color ShapeColor
    {
        get { return color; }
        set
        {
            color = value;
            SetColor();
        }
    }
    private Color color;
    private Line[] lines;
    public Vector2[] vertices;
    public bool triangulated;
    private bool fail;
    private bool outlined;
    private int[] ind;
    VertexPositionColor[] vert;
    public Shape(Vector2[] Vertices)
    {
        vertices = new Vector2[Vertices.Length];
        for (int i = 0; i < Vertices.Length; i++)
        {
            vertices[i] = new Vector2(Vertices[i].X - (Irbis.Irbis.halfResolution.X ), -(Vertices[i].Y - (Irbis.Irbis.halfResolution.Y )));
        }
        color = Color.Black;
        lines = new Line[vertices.Length];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            lines[i] = new Line(vertices[i], vertices[i + 1], false);
        }
        if (vertices.Length > 0)
        { lines[lines.Length - 1] = new Line(vertices[vertices.Length - 1], vertices[0], false); }
        ind = new int[0];
        vert = new VertexPositionColor[0];
        triangulated = fail = false;
        outlined = true;
        Irbis.Irbis.WriteLine(this.ToString());
    }
    public Shape(Rectangle referenceRectangle)
    {
        vertices = new Vector2[4];
        vertices[0] = new Vector2(referenceRectangle.Left - (Irbis.Irbis.halfResolution.X ), -(referenceRectangle.Top - (Irbis.Irbis.halfResolution.Y )));
        vertices[1] = new Vector2(referenceRectangle.Right - (Irbis.Irbis.halfResolution.X ), -(referenceRectangle.Top - (Irbis.Irbis.halfResolution.Y )));
        vertices[2] = new Vector2(referenceRectangle.Right - (Irbis.Irbis.halfResolution.X ), -(referenceRectangle.Bottom - (Irbis.Irbis.halfResolution.Y )));
        vertices[3] = new Vector2(referenceRectangle.Left - (Irbis.Irbis.halfResolution.X ), -(referenceRectangle.Bottom - (Irbis.Irbis.halfResolution.Y )));
        color = Color.Black;
        lines = new Line[vertices.Length];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            lines[i] = new Line(vertices[i], vertices[i + 1], false);
        }
        lines[lines.Length - 1] = new Line(vertices[vertices.Length - 1], vertices[0], false);
        ind = new int[0];
        vert = new VertexPositionColor[0];
        outlined = true;
        triangulated = fail = false;

    }
    public void SortVertices()
    {
        List<Vector2> tempVerts = new List<Vector2>(vertices);
        tempVerts.RemoveAt(0);
        tempVerts.RemoveAt(0);

        float closest = float.MaxValue;
        int closestIndex = -1;
        for (int j = 3; j < vertices.Length; j++)
        {
            float distanceSquared = Vector2.DistanceSquared(vertices[2], vertices[j]);
            if (distanceSquared < closest)
            { closestIndex = j; }
        }
        tempVerts[2] = vertices[closestIndex];

        int secondClosestIndex;
        for (int i = 3; i < vertices.Length - 1; i++)
        {
            closest = float.MaxValue;
            secondClosestIndex = closestIndex = i+1;
            for (int j = i + 1; j < vertices.Length; j++)
            {
                float distanceSquared = Vector2.DistanceSquared(vertices[i], vertices[j]);
                if (distanceSquared < closest)
                {
                    secondClosestIndex = closestIndex;
                    closestIndex = j;
                }
            }
            tempVerts[i] = vertices[secondClosestIndex];
        }
    }
    public void CreateLines()
    {
        lines = new Line[vertices.Length];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            lines[i] = new Line(vertices[i], vertices[i + 1], false);
        }
        lines[lines.Length - 1] = new Line(vertices[vertices.Length - 1], vertices[0], false);
        outlined = true;
    }
    public void DrawLines()
    {
        if (!outlined)
        {
            CreateLines();
        }
        foreach (Line l in lines)
        {
            l.Draw();
        }
    }
    public void SetColor()
    {
        for (int i = 0; i < vert.Length; i++)
        {
            vert[i].Color = color;
        }
    }
    public bool Triangulate()
    {
        // vertices[0] = origin;
        vert = new VertexPositionColor[vertices.Length];
        ind = new int[(vertices.Length - 1) * 3];
        //Irbis.Irbis.WriteLine("vert.Length:" + vert.Length);
        //Irbis.Irbis.WriteLine("ind.Length:" + ind.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            vert[i].Position = new Vector3(vertices[i], 000f);
            vert[i].Color = color;
            if (i < vertices.Length - 2)
            {
                ind[i * 3] = 0;
                ind[i * 3 + 1] = i + 1;
                ind[i * 3 + 2] = i + 2;
            }
        }
        //ind[ind.Length - 3] = 0;
        //ind[ind.Length - 2] = vertices.Length - 1;
        //ind[ind.Length - 1] = 1;
        ind[ind.Length - 3] = 0;
        ind[ind.Length - 2] = vertices.Length - 1;
        ind[ind.Length - 1] = 1;

        return triangulated = true;
    }
    public bool TriangleContains(Vector2 TriangleVertexA, Vector2 TriangleVertexB, Vector2 TriangleVertexC, Vector2 TestableVector)
    {
        //Source: http://blackpawn.com/texts/pointinpoly/
        //TriangleVertexA = A
        //TriangleVertexB = B
        //TriangleVertexC = C
        //TestableVector  = P

        //We'll pick the two edges of the triangle that touch A, (C - A) and (B - A).
        //P = A + u * (C - A) + v * (B - A)       // Original equation
        //(P - A) = u * (C - A) + v * (B - A)     // Subtract A from both sides
        //   v2   = u *    v0   + v *    v1       // Substitute v0, v1, v2 for less writing

        //v0 = (C - A)
        //v1 = (B - A)
        //v2 = (P - A)

        Vector2 v0 = TriangleVertexC - TriangleVertexA;
        Vector2 v1 = TriangleVertexB - TriangleVertexA;
        Vector2 v2 = TestableVector - TriangleVertexA;

        //// We have two unknowns (u and v) so we need two equations to solve
        //// for them.  Dot both sides by v0 to get one and dot both sides by
        //// v1 to get a second.
        //(v2) . v0 = (u * v0 + v * v1) . v0
        //(v2) . v1 = (u * v0 + v * v1) . v1

        //// Distribute v0 and v1
        //v2 . v0 = u * (v0 . v0) + v * (v1 . v0)
        //v2 . v1 = u * (v0 . v1) + v * (v1 . v1)

        //// Now we have two equations and two unknowns and can solve one 
        //// equation for one variable and substitute into the other.  Or
        //// if you're lazy like me, fire up Mathematica and save yourself
        //// some handwriting.
        //Solve[v2.v0 == {u(v0.v0) + v(v1.v0), v2.v1 == u(v0.v1) + v(v1.v1)}, {u, v}]
        //u = ( ((v1.v1) * (v2.v0)) - ((v1.v0) * (v2.v1)) ) / ( ((v0.v0) * (v1.v1)) - ((v0.v1) * (v1.v0)) )
        //v = ( ((v0.v0) * (v2.v1)) - ((v0.v1) * (v2.v0)) ) / ( ((v0.v0) * (v1.v1)) - ((v0.v1) * (v1.v0)) )

        float u = ( (Vector2.Dot(v1,v1) * Vector2.Dot(v2,v0)) - (Vector2.Dot(v1,v0) * Vector2.Dot(v2,v1)) ) / ( (Vector2.Dot(v0,v0) * Vector2.Dot(v1,v1)) - (Vector2.Dot(v0,v1) * Vector2.Dot(v1,v0)) );
        float v = ( (Vector2.Dot(v0,v0) * Vector2.Dot(v2,v1)) - (Vector2.Dot(v0,v1) * Vector2.Dot(v2,v0)) ) / ( (Vector2.Dot(v0,v0) * Vector2.Dot(v1,v1)) - (Vector2.Dot(v0,v1) * Vector2.Dot(v1,v0)) );

        //Notice now that if u or v < 0 then we've walked in the wrong direction and must be outside the triangle.
        //Also if u or v > 1 then we've walked too far in a direction and are outside the triangle.
        //Finally if u + v > 1 then we've crossed the edge BC again leaving the triangle.
        if (u <= 0 || v <= 0 ||
            u >= 1 || v >= 1 || 
            u + v >= 1)
        {
            return false;
        }

        return true;
    }
    public bool IsClockwise(Vector2 TriangleVertexA, Vector2 TriangleVertexB, Vector2 TriangleVertexC)
    {
        //sum the edges
        float clockwisetracker
        //edge 1: point A and point B
        //(B.X - A.X) * (B.Y + A.Y)
            = ((TriangleVertexB.X - TriangleVertexA.X) * (TriangleVertexB.Y + TriangleVertexA.Y))
        //edge 2: point B and point C
        //(C.X - B.X) * (C.Y + B.Y)
             + ((TriangleVertexC.X - TriangleVertexB.X) * (TriangleVertexC.Y + TriangleVertexB.Y))
        //edge 3: point C and point A
        //(A.X - C.X) * (A.Y + C.Y)
             + ((TriangleVertexA.X - TriangleVertexC.X) * (TriangleVertexA.Y + TriangleVertexC.Y));

        if (clockwisetracker > 0)
        { return true; }
        return false;
    }
    public void Draw()
    {
        if (vertices.Length > 2)
        {
            if (!triangulated)
            { Triangulate(); }
            Irbis.Irbis.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);
        }
        //DrawLines();
    }
    public override string ToString()
    {
        string returnString = string.Empty;

        returnString = "vertices: " + vertices.Length;
        for (int i = 0; i < vertices.Length; i++)
        {
            returnString += "\nvertex[" + i + "]:" + vertices[i];
        }
        return returnString;
    }
    public string Debug(bool evaluate)
    {
        string returnString = string.Empty;

        returnString = "{";
        returnString += "color:" + color;
        returnString += "\ntriangulated:" + triangulated;
        returnString += "\noutlined:" + outlined;

        returnString += "\nvertices:" + vertices.Length;
        for (int i = 0; i < vertices.Length; i++)
        {
            returnString += "\nvertex[" + i + "]:" + vertices[i];
        }
        returnString += "\nlines:" + lines.Length;
        for (int i = 0; i < lines.Length; i++)
        {
            returnString += "\nline[" + i + "]:" + lines[i];
        }
        returnString += "\nvert:" + vert.Length;
        for (int i = 0; i < vert.Length; i++)
        {
            returnString += "\nvert[" + i + "].Position:" + vert[i].Position;
            returnString += " vert[" + i + "].Color:" + vert[i].Color;
        }
        returnString += "\nind:" + ind.Length;
        for (int i = 0; i < ind.Length; i++)
        {
            returnString += "\nind[" + i + "]:" + ind[i];
        }
        returnString += "}";

        if (evaluate)
        {


            for (int i = 0; i < vertices.Length; i++) // (Vector2 v in vertices)
            {
                bool contains = false;
                foreach (VertexPositionColor v in vert)
                {
                    if (v.Position == (new Vector3(vertices[i], 000f)))
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    returnString += "\nvertex[" + i + "] ("+ vertices[i] + ") not contained in vert";
                }
            }

            if ((ind.Length / 3f) + 2f != (float)(vertices.Length))
            {
                returnString += "\n(ind.Length / 3f) + 2f != (float)(vertices.Length). ind.Length:" + ind.Length + " vertices.Length:" + vertices.Length;
            }
            else
            {
                returnString += "\ntriangles:";
                for (int i = 0; i < ind.Length; i += 3)
                {
                    int ind1 = ind[i + 0];
                    int ind2 = ind[i + 1];
                    int ind3 = ind[i + 2];
                    if (ind1 == ind2 || ind2 == ind3 || ind1 == ind3)
                    {
                        returnString += "\ntriangle contains two of the same points: {ind[" + (i + 0) + "]:" + ind[i + 0] + " ind[" + (i + 1) + "]:" + ind[i + 1] + " ind[" + (i + 2) + "]:" + ind[i + 2] + "}";
                    }
                    else
                    {
                        returnString += "\ntriangle[" + (i/3) + "]:" + "{ind[" + (i + 0) + "]:" + ind[i + 0] + " ind[" + (i + 1) + "]:" + ind[i + 1] + " ind[" + (i + 2) + "]:" + ind[i + 2] + "}"
                            + "{vertex[" + ind[i + 0] + "]:" + vert[ind[i + 0]].Position + " vertex[" + ind[i + 1] + "]:" + vert[ind[i + 1]].Position + " vertex[" + ind[i + 2] + "]:" + vert[ind[i + 2]].Position + "}";
                    }
                }
            }
        }

        return returnString;
    }
    public static int TotalLines(Shape[] Shapes)
    {
        int totalLines = 0;
        foreach (Shape s in Shapes)
        {
            totalLines += s.NumberOfLines;
        }
        return totalLines;
    }
}
