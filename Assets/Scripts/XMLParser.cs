using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using g3;
using UnityEngine;

public class XMLParser : MonoBehaviour
{
    public Material wireframeMat;
    public Material shadedMat;
    
    [SerializeField] private string documentPath = "./Assets/XXX.xml";
    
    private Vector3d offset;
    private List<Vector3d> vertices = new List<Vector3d>();
    private List<int> triangles = new List<int>();
    private GameObject meshGO;
    private MeshRenderer renderer;

    private void Start()
    {
        #if !UNITY_EDITOR

        documentPath = Application.streamingAssetsPath + "\\data.xml";
        
        #endif
        ParseVertsAndTris(documentPath);
        
        Debug.Log("Vertices: " + vertices.Count);
        Debug.Log("Triangles: " + triangles.Count);

        Debug.Log(vertices[0]);

        Vector3d[] normals = new Vector3d[vertices.Count];
        DMesh3 mesh = DMesh3Builder.Build(vertices, triangles, normals);

        MeshNormals.QuickCompute(mesh);

        meshGO = g3UnityUtils.CreateMeshGO("Model", mesh, Colorf.White);

        renderer = meshGO.GetComponent<MeshRenderer>();

        //IOWriteResult result = StandardMeshWriter.WriteFile(outputPath,
        //    new List<WriteMesh>() { new WriteMesh(mesh) }, WriteOptions.Defaults);
        //
        //Debug.Log(result.message);

        //meshFilter.mesh = mesh;
    }

    private bool isWireframe;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) isWireframe = !isWireframe;

        if (isWireframe)
        {
            renderer.material = wireframeMat;
        }
        else
        {
            renderer.material = shadedMat;
        }
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
            
            double x = double.Parse(parts[0], CultureInfo.InvariantCulture);
            double y = double.Parse(parts[1], CultureInfo.InvariantCulture);
            double z = double.Parse(parts[2], CultureInfo.InvariantCulture);

            if (node == pNodes[0])
            {
                offset.x = -x;
                offset.y = -y;
                offset.z = -z;
            }
        
            vertices.Add(new Vector3d(x + offset.x, z + offset.z, y + offset.y));
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
}
