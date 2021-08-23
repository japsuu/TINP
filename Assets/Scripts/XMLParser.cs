using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class XMLParser : MonoBehaviour
{
    [SerializeField] private string documentPath;
    [SerializeField] private Vector3 offset;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();


    private void Start()
    {
        ParseVertsAndTris(documentPath);
        
        Debug.Log("Vertices: " + vertices.Count);
        Debug.Log("Triangles: " + triangles.Count);

        foreach (int triangle in triangles)
        {
            Debug.Log("testing " + triangle + " of " + vertices.Count);
            Debug.Log(vertices[triangle]);
        }
        
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };

        /*
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;*/

        meshFilter.mesh = mesh;
    }
    
    private void ParseVertsAndTris(string path)
    {
        // Open the XML-doc
        XmlDocument xDoc = new XmlDocument();
        try
        {
            xDoc.Load(path);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
        // Get the "Point" and "Face" -nodes.
        XmlNodeList pNodes = xDoc.GetElementsByTagName("P");
        XmlNodeList fNodes = xDoc.GetElementsByTagName("F");
        
        Debug.Log("Points: " + pNodes.Count);
        Debug.Log("Faces: " + fNodes.Count);
        
        foreach (XmlNode node in pNodes)
        {
            string point = node.InnerText;
            string[] parts = point.Split(' ');
    
            if (node.Attributes == null) continue;
            
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);
        
            vertices.Add(new Vector3(x + offset.x, z + offset.y, y + offset.z));
        }
        
        foreach (XmlNode node in fNodes)
        {
            string face = node.InnerText;
            string[] parts = face.Split(' ');
        
            triangles.Add(int.Parse(parts[0]) - 1);
            triangles.Add(int.Parse(parts[1]) - 1);
            triangles.Add(int.Parse(parts[2]) - 1);
        }
    }

    /*
    private static List<Face> ParseFaces(string path)
    {
        Dictionary<int, Vector3> points = new Dictionary<int, Vector3>();
        List<Face> faces = new List<Face>();
    
        // Open the XML-doc
        XmlDocument xDoc = new XmlDocument();
        try
        {
            xDoc.Load(path);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
        // Get the "Point" and "Face" -nodes.
        XmlNodeList pNodes = xDoc.GetElementsByTagName("P");
        XmlNodeList fNodes = xDoc.GetElementsByTagName("F");
        
        
        foreach (XmlNode node in pNodes)
        {
            string point = node.InnerText;
            string[] parts = point.Split(' ');
    
            if (node.Attributes == null) continue;
            
            int id = int.Parse(node.Attributes[0].InnerText);
            
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);
        
            points.Add(id, new Vector3(x, y, z));
        }
        
        foreach (XmlNode node in fNodes)
        {
            string face = node.InnerText;
            string[] parts = face.Split(' ');
        
            int id1 = int.Parse(parts[0]);
            int id2 = int.Parse(parts[1]);
            int id3 = int.Parse(parts[2]);
            Vector3 p1 = points[id1];
            Vector3 p2 = points[id2];
            Vector3 p3 = points[id3];
        
            faces.Add(new Face(p1, p2, p3));
        }
    
        return faces;
    }

    public class Face
    {
        public Vector3 p1 { get; private set; }
        public Vector3 p2 { get; private set; }
        public Vector3 p3 { get; private set; }

        public Face(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public string GetAsString()
        {
            return "p1: " + p1 + ", p2: " + p2 + ", p3: " + p3;
        }
    }*/
}
