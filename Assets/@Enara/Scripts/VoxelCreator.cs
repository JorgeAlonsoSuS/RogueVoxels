using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;
using System.Linq;
using System.ComponentModel;
using UnityEngine.XR.Interaction.Toolkit;

public class VoxelCreator : MonoBehaviour
{
    private Voxel[,,] voxels;

    public Dictionary<Voxel.VoxelType, Material> voxelMaterials;
    private int levelSize;

    private Dictionary<int, List<List<Vector3>>> vertices = new Dictionary<int, List<List<Vector3>>>();
    private Dictionary<int, List<List<int>>> triangles = new Dictionary<int, List<List<int>>>();
    private Dictionary<int, List<List<Vector2>>> uvs = new Dictionary<int, List<List<Vector2>>>();

    private List<Vector3> currentVertices;
    private List<int> currentTriangles;
    private List<Vector2> currentUVs;

    //private List<List<Voxel>> voxelList = new List<List<Voxel>>();

    private int currentIndex = -1;

    void Start()
    {

        IterateVoxels(); // Make sure this processes all voxels

        // Call this to generate the chunk mesh
        GenerateMesh();
    }

    private void InitializeVoxels()
    {
        int seed = Random.Range(1, 500000);
        for (int x = 0; x < levelSize; x++)
        {
            for (int y = 0; y < levelSize; y++)
            {
                for (int z = 0; z < levelSize; z++)
                {
                    // Use world coordinates for noise sampling
                    Vector3 worldPos = transform.position + new Vector3(x, y, z);
                    Voxel.VoxelType type = DetermineVoxelType(worldPos.x, worldPos.y, worldPos.z, seed);
                    voxels[x, y, z] = new Voxel(worldPos, type, type != Voxel.VoxelType.Air);
                }
            }
        }
    }

    private Voxel.VoxelType DetermineVoxelType(float x, float y, float z, int seed)
    {
        

        float noiseValue = Noise.CalcPixel3D((int)x + seed, (int)y + seed, (int)z + seed, 0.1f);

        float threshold = 125f; // The threshold for determining solid/air

        //Debug.Log(noiseValue);

        if (noiseValue > threshold)
        {
            if (noiseValue < 140)
                return Voxel.VoxelType.Grass; // Solid voxel
            else if (noiseValue < 180)
                return Voxel.VoxelType.Stone;
            else if (noiseValue < 185)
                return Voxel.VoxelType.Metal;
            else if (noiseValue < 190)
                return Voxel.VoxelType.Lava;
            else if (noiseValue < 195)
                return Voxel.VoxelType.Uranium;

            else
                return Voxel.VoxelType.Water;
        }
        else
            return Voxel.VoxelType.Air; // Air voxel
    }

    // New method to iterate through the voxel data
    public void IterateVoxels()
    {
        //Primero vamos a dividir 
        for (int x = 0; x < levelSize; x++)
        {
            for (int y = 0; y < levelSize; y++)
            {
                for (int z = 0; z < levelSize; z++)
                {
                    ProcessVoxel(x, y, z);
                }
            }
        }
    }

    private void ProcessVoxel(int x, int y, int z)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (voxels == null || x < 0 || x >= voxels.GetLength(0) ||
            y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }

        Voxel voxel = voxels[x, y, z];

        //switch (voxel.type)
        //{
        //    case Voxel.VoxelType.Grass:
        //        voxelList[0].Add(voxel);
        //        break;
        //    case Voxel.VoxelType.Stone:
        //        voxelList[1].Add(voxel);
        //        break;
        //    case Voxel.VoxelType.Water:
        //        voxelList[1].Add(voxel);
        //        break;
        //}

        int dictionaryKey = 0;
        switch (voxel.type)
        {
            case Voxel.VoxelType.Grass:
                dictionaryKey = 0;
                break;
            case Voxel.VoxelType.Stone:
                dictionaryKey = 1;
                break;
            case Voxel.VoxelType.Water:
                dictionaryKey = 2;
                break;
            case Voxel.VoxelType.Lava:
                dictionaryKey = 3;
                break;
            case Voxel.VoxelType.Uranium:
                dictionaryKey = 4;
                break;
            case Voxel.VoxelType.Metal:
                dictionaryKey = 5;
                break;
        }



        //ENARA: los voxels están en un array de 3 dimensiones, no hace falta hacer lista
        //aquí es donde tenemos que crear nuevas listas: 

        currentIndex = vertices[dictionaryKey].Count - 1;
        if (currentIndex == -1 || vertices[dictionaryKey].Count == 0 || vertices[dictionaryKey][currentIndex].Count + 24 > 65536)
        {
            currentVertices = new List<Vector3>();
            currentUVs = new List<Vector2>();
            currentTriangles = new List<int>();

            vertices[dictionaryKey].Add(currentVertices);
            uvs[dictionaryKey].Add(currentUVs);
            triangles[dictionaryKey].Add(currentTriangles);

            currentIndex++;
        }
        else
        {
            currentVertices = vertices[dictionaryKey][currentIndex];
            currentUVs = uvs[dictionaryKey][currentIndex];
            currentTriangles = triangles[dictionaryKey][currentIndex];
        }
        // 

        if (voxel.isActive)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i); // Method to add mesh data for the visible face
            }
        }
    }

    private bool IsFaceVisible(int x, int y, int z)
    {
        // Check if the neighboring voxel is inactive or out of bounds in the current chunk
        return IsVoxelHiddenInChunk(x, y, z);
    }

    private bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= levelSize || y < 0 || y >= levelSize || z < 0 || z >= levelSize)
            return true; // Face is at the boundary of the chunk
        return !voxels[x, y, z].isActive;
    }

    public bool IsVoxelActiveAt(Vector3 localPosition)
    {
        // Round the local position to get the nearest voxel index
        int x = Mathf.RoundToInt(localPosition.x);
        int y = Mathf.RoundToInt(localPosition.y);
        int z = Mathf.RoundToInt(localPosition.z);

        // Check if the indices are within the bounds of the voxel array
        if (x >= 0 && x < levelSize && y >= 0 && y < levelSize && z >= 0 && z < levelSize)
        {
            // Return the active state of the voxel at these indices
            return voxels[x, y, z].isActive;
        }

        // If out of bounds, consider the voxel inactive
        return false;
    }

    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs


        if (faceIndex == 0) // Top Face
        {
            currentVertices.Add(new Vector3(x, y + 1, z));
            currentVertices.Add(new Vector3(x, y + 1, z + 1));
            currentVertices.Add(new Vector3(x + 1, y + 1, z + 1));
            currentVertices.Add(new Vector3(x + 1, y + 1, z));
            currentUVs.Add(new Vector2(0, 0));
            currentUVs.Add(new Vector2(1, 0));
            currentUVs.Add(new Vector2(1, 1));
            currentUVs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            currentVertices.Add(new Vector3(x, y, z));
            currentVertices.Add(new Vector3(x + 1, y, z));
            currentVertices.Add(new Vector3(x + 1, y, z + 1));
            currentVertices.Add(new Vector3(x, y, z + 1));
            currentUVs.Add(new Vector2(0, 0));
            currentUVs.Add(new Vector2(0, 1));
            currentUVs.Add(new Vector2(1, 1));
            currentUVs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            currentVertices.Add(new Vector3(x, y, z));
            currentVertices.Add(new Vector3(x, y, z + 1));
            currentVertices.Add(new Vector3(x, y + 1, z + 1));
            currentVertices.Add(new Vector3(x, y + 1, z));
            currentUVs.Add(new Vector2(0, 0));
            currentUVs.Add(new Vector2(0, 0));
            currentUVs.Add(new Vector2(0, 1));
            currentUVs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            currentVertices.Add(new Vector3(x + 1, y, z + 1));
            currentVertices.Add(new Vector3(x + 1, y, z));
            currentVertices.Add(new Vector3(x + 1, y + 1, z));
            currentVertices.Add(new Vector3(x + 1, y + 1, z + 1));
            currentUVs.Add(new Vector2(1, 0));
            currentUVs.Add(new Vector2(1, 1));
            currentUVs.Add(new Vector2(1, 1));
            currentUVs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            currentVertices.Add(new Vector3(x, y, z + 1));
            currentVertices.Add(new Vector3(x + 1, y, z + 1));
            currentVertices.Add(new Vector3(x + 1, y + 1, z + 1));
            currentVertices.Add(new Vector3(x, y + 1, z + 1));
            currentUVs.Add(new Vector2(0, 1));
            currentUVs.Add(new Vector2(0, 1));
            currentUVs.Add(new Vector2(1, 1));
            currentUVs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            currentVertices.Add(new Vector3(x + 1, y, z));
            currentVertices.Add(new Vector3(x, y, z));
            currentVertices.Add(new Vector3(x, y + 1, z));
            currentVertices.Add(new Vector3(x + 1, y + 1, z));
            currentUVs.Add(new Vector2(0, 0));
            currentUVs.Add(new Vector2(1, 0));
            currentUVs.Add(new Vector2(1, 0));
            currentUVs.Add(new Vector2(0, 0));

        }
        AddTriangleIndices();
    }

    private void AddTriangleIndices()
    {
        int vertCount = currentVertices.Count;

        // First triangle
        currentTriangles.Add(vertCount - 4);
        currentTriangles.Add(vertCount - 3);
        currentTriangles.Add(vertCount - 2);

        // Second triangle
        currentTriangles.Add(vertCount - 4);
        currentTriangles.Add(vertCount - 2);
        currentTriangles.Add(vertCount - 1);
    }

    private void GenerateMesh()
    {
        Debug.Log("Generating mesh");
        Material mat;

        for (int i = 0; i < voxelMaterials.Count; i++)
        {
            mat = voxelMaterials[(Voxel.VoxelType)i];

            for (int j = 0; j < vertices[i].Count; j++)
            {
                GameObject go = new GameObject("Mesh " + i + ":" + j);
                go.transform.position = Vector3.zero;
                go.transform.parent = this.transform;

                if (i == 0)
                {
                    go.layer = LayerMask.NameToLayer("Grass");
                } 
                
                else if (i == 1)
                {
                    go.layer = LayerMask.NameToLayer("Stone");
                } 
                
                else if (i == 2)
                {
                    go.layer = LayerMask.NameToLayer("WaterV");
                }
                
                else if (i == 3)
                {
                    go.layer = LayerMask.NameToLayer("Lava");
                }

                else if (i == 4)
                {
                    go.layer = LayerMask.NameToLayer("Uranium");
                }

                else if (i == 5)
                {
                    go.layer = LayerMask.NameToLayer("Metal");
                }

                MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                MeshCollider meshCollider = go.AddComponent<MeshCollider>();

                Mesh mesh = new Mesh();
                mesh.vertices = vertices[i][j].ToArray();
                mesh.triangles = triangles[i][j].ToArray();
                mesh.uv = uvs[i][j].ToArray();

                mesh.RecalculateNormals();

                meshFilter.mesh = mesh;
                meshCollider.sharedMesh = mesh;

                meshRenderer.material = mat;

                XRSimpleInteractable interactable = go.AddComponent<XRSimpleInteractable>();
            }
        }
    }

    /*public void PlaceItemInLowestAirSpace(GameObject amulet)
    {   
        // Intenta encontrar un espacio de aire desde el nivel más bajo posible
        for (int z = 0; z < levelSize; z++)
        {
            for (int x = 0; x < levelSize; x++)
            {
                for (int y = 0; y < levelSize; y++)
                {
                    // Comprueba si el voxel en esta posición es aire
                    if (voxels[x, y, z].type == Voxel.VoxelType.Air)
                    {
                        // Comprueba si la posición está realmente libre (no obstruida por otros objetos)
                        Vector3 worldPosition = transform.position + new Vector3(x, y, z);
                        if (IsPositionFree(worldPosition))
                        {
                            // Coloca el objeto
                            Instantiate(amulet, worldPosition, Quaternion.identity);
                            return;  // Sale de la función una vez colocado el objeto
                        }
                    }
                }
            }
        }
    }

    private bool IsPositionFree(Vector3 position)
    {
        // Realiza una comprobación de colisión en el punto para asegurarte de que está libre
        float checkRadius = 0.5f; // Asegúrate de que este radio sea adecuado para el tamaño del objeto
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
        return colliders.Length == 0; // Retorna verdadero si no hay colisionadores en este punto
    }*/

    public void PlaceItemInPreferablyCentralAirSpace(GameObject amulet)
    {
        List<Vector3> possiblePositions = new List<Vector3>();

        // Margen para evitar la generación justo en el borde
        int margin = 2;

        for (int x = margin; x < levelSize - margin; x++)
        {
            for (int y = margin; y < levelSize - margin; y++)
            {
                for (int z = margin; z < levelSize - margin; z++)
                {
                    // Verifica que el voxel actual y un pequeño buffer alrededor sean de aire
                    if (IsAirWithBuffer(x, y, z))
                    {
                        Vector3 worldPosition = transform.position + new Vector3(x, y, z);
                        if (IsPositionFree(worldPosition))
                        {
                            possiblePositions.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }
        }

        if (possiblePositions.Count > 0)
        {
            Vector3 selectedPos = GetWeightedRandomPosition(possiblePositions);
            Instantiate(amulet, transform.position + selectedPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No suitable location found for placing the object.");
        }
    }

    private bool IsAirWithBuffer(int x, int y, int z)
    {
        // Verifica si el voxel y sus vecinos son de tipo 'Air' para asegurarse de que hay espacio
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (voxels[x + i, y + j, z + k].type != Voxel.VoxelType.Air)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private Vector3 GetWeightedRandomPosition(List<Vector3> positions)
    {
        // Ponderación por distancia al centro para favorecer posiciones centrales
        List<float> weights = new List<float>();
        Vector3 center = new Vector3(levelSize / 2, levelSize / 2, levelSize / 2);

        foreach (var pos in positions)
        {
            float weight = 1 / (center - pos).magnitude; // Mayor peso cuanto más cerca del centro
            weights.Add(weight);
        }

        float weightSum = weights.Sum();
        float randomValue = Random.Range(0, weightSum);
        float cumulativeWeight = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return positions[i];
            }
        }

        return positions[0]; // Fallback seguro
    }


    private bool IsPositionFree(Vector3 worldPosition)
    {
        Collider[] hitColliders = Physics.OverlapSphere(worldPosition, 0.5f);
        return hitColliders.Length == 0;  // Retorna true si no hay colisionadores en este punto.
    }


    public void DestroyVoxelAt(Vector3 point)
    {
        Debug.Log("Destroy voxel at");
        int x = Mathf.FloorToInt(point.x); // Usar FloorToInt para evitar problemas de redondeo
        int y = Mathf.FloorToInt(point.y);
        int z = Mathf.FloorToInt(point.z);

        // Ajustar las coordenadas para que estén dentro de los límites del nivel
        x = Mathf.Clamp(x, 0, levelSize - 1);
        y = Mathf.Clamp(y, 0, levelSize - 1);
        z = Mathf.Clamp(z, 0, levelSize - 1);

        Debug.Log($"Voxel coordinates after clamping: ({x}, {y}, {z})");

        if (x >= 0 && x < levelSize && y >= 0 && y < levelSize && z >= 0 && z < levelSize)
        {
            Voxel voxel = voxels[x, y, z];
            if (voxel.type != Voxel.VoxelType.Air)
            {
                voxels[x, y, z].isActive = false;
                voxels[x, y, z].type = Voxel.VoxelType.Air; // Cambiar el tipo a aire para indicar que está destruido
                Debug.Log($"Voxel at ({x}, {y}, {z}) set to Air and inactive");
                UpdateVoxelMesh(x, y, z);
            }
            else
            {
                Debug.Log($"Voxel at ({x}, {y}, {z}) is already Air");
            }
        }
        else
        {
            Debug.Log($"Voxel coordinates ({x}, {y}, {z}) are out of bounds");
        }
    }

    public void interactVoxel(Vector3 point, string spell)
    {
        Debug.Log("Replace voxel at");
        int x = Mathf.FloorToInt(point.x); // Usar FloorToInt para evitar problemas de redondeo
        int y = Mathf.FloorToInt(point.y);
        int z = Mathf.FloorToInt(point.z);

        // Ajustar las coordenadas para que estén dentro de los límites del nivel
        x = Mathf.Clamp(x, 0, levelSize - 1);
        y = Mathf.Clamp(y, 0, levelSize - 1);
        z = Mathf.Clamp(z, 0, levelSize - 1);

        Debug.Log($"Voxel coordinates after clamping: ({x}, {y}, {z})");

        if (x >= 0 && x < levelSize && y >= 0 && y < levelSize && z >= 0 && z < levelSize)
        {
            Voxel voxel = voxels[x, y, z];
            if (voxel.type != Voxel.VoxelType.Air)
            {
                switch (spell)
                {
                    case "laser":
                        if(voxel.type == Voxel.VoxelType.Stone)
                        {
                            voxels[x, y, z].isActive = false;
                            voxels[x, y, z].type = Voxel.VoxelType.Air; // Cambiar el tipo a aire para indicar que está destruido
                            Debug.Log($"Voxel at ({x}, {y}, {z}) set to Air and inactive");
                            UpdateVoxelMesh(x, y, z);
                        }
                        break;
                    case "water":
                        if(voxel.type == Voxel.VoxelType.Lava)
                        {
                            voxels[x, y, z].type = Voxel.VoxelType.Stone; // Cambiar el tipo a piedra
                            Debug.Log($"Voxel at ({x}, {y}, {z}) set to stone");
                            UpdateVoxelMesh(x, y, z);
                        }
                        break;
                    case "fire":
                        if(voxel.type == Voxel.VoxelType.Grass)
                        {
                            voxels[x, y, z].isActive = false;
                            voxels[x, y, z].type = Voxel.VoxelType.Air; // Cambiar el tipo a aire para indicar que está destruido
                            Debug.Log($"Voxel at ({x}, {y}, {z}) set to Air and inactive");
                            UpdateVoxelMesh(x, y, z);
                        }
                        break;
                    default:
                        Debug.LogWarning("Spell desconocido: " + spell);
                        spell = "laser";
                        break;
                }

            }
            else
            {
                Debug.Log($"Voxel at ({x}, {y}, {z}) is already Air");
            }
        }
        else
        {
            Debug.Log($"Voxel coordinates ({x}, {y}, {z}) are out of bounds");
        }
    }

    private void UpdateVoxelMesh(int x, int y, int z)
    {
        Debug.Log("Update voxel mesh");

        // Limpiar referencias activas y eliminar eventos antes de destruir objetos
        foreach (Transform child in transform)
        {
            XRSimpleInteractable interactable = child.GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.RemoveAllListeners();
            }
            Destroy(child.gameObject);
        }

        // Clear previous mesh data
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int i = 0; i < voxelMaterials.Count; i++)
        {
            vertices.Add(i, new List<List<Vector3>>());
            uvs.Add(i, new List<List<Vector2>>());
            triangles.Add(i, new List<List<int>>());
        }

        // Reprocess voxels
        IterateVoxels();

        // Regenerate the mesh
        GenerateMesh();
    }




    public void Initialize(int size, GameObject amulet)
    {
        this.levelSize = size;
        voxels = new Voxel[size, size, size];

        voxelMaterials = new Dictionary<Voxel.VoxelType, Material>()
        {
            { Voxel.VoxelType.Grass, Level.Instance.materials[0] },
            { Voxel.VoxelType.Stone, Level.Instance.materials[1]},
            { Voxel.VoxelType.Water, Level.Instance.materials[2]},
            { Voxel.VoxelType.Lava, Level.Instance.materials[3]},
            { Voxel.VoxelType.Uranium, Level.Instance.materials[4]},
            { Voxel.VoxelType.Metal, Level.Instance.materials[5]},
            // Add more types and their corresponding materials
        };

        for (int i = 0; i < voxelMaterials.Count; i++)
        {
            vertices.Add(i, new List<List<Vector3>>());
            uvs.Add(i, new List<List<Vector2>>());
            triangles.Add(i, new List<List<int>>());
            //voxelList.Add(new List<Voxel>());
        }

        InitializeVoxels();

        PlaceItemInPreferablyCentralAirSpace(amulet);
    }
}
