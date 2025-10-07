using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoxelPoolManager : MonoBehaviour
{
    public static VoxelPoolManager Instance { get; private set; }

    public List<GameObject> waterVoxels = new List<GameObject>();
    public List<GameObject> grassVoxels = new List<GameObject>();
    public List<GameObject> rockVoxels = new List<GameObject>();

    private int currentWaterVoxel = 0;
    private int currentGrassVoxel = 0;
    private int currentRockVoxel = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowWaterVoxel(Vector3 _pos)
    {
        if (currentWaterVoxel >= waterVoxels.Count)
            return;

        waterVoxels[currentWaterVoxel].SetActive(true);
        waterVoxels[currentWaterVoxel].transform.position = _pos;
        currentWaterVoxel++;
    }

    public void ShowGrassVoxel(Vector3 _pos)
    {
        if (currentGrassVoxel >= grassVoxels.Count)
            return;

        grassVoxels[currentGrassVoxel].SetActive(true);
        grassVoxels[currentGrassVoxel].transform.position = _pos;
        currentGrassVoxel++;
    }

    public void ShowRockVoxel(Vector3 _pos)
    {
        if (currentRockVoxel >= rockVoxels.Count)
            return;

        rockVoxels[currentRockVoxel].SetActive(true);
        rockVoxels[currentRockVoxel].transform.position = _pos;
        currentRockVoxel++;
    }
}
