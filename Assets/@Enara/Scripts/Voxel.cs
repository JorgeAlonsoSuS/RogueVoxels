using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a simple Voxel struct
public struct Voxel
{
    public Vector3 position;
    public bool isActive;

    public VoxelType type; // Using the VoxelType enum
    public enum VoxelType
    {
        Air = -1,    // Represents empty space
        Grass = 0,  // Represents grass block
        Stone = 1,  // Represents stone block
        Water = 2,        // Add more types as needed
        Lava = 3,
        Uranium = 4,
        Metal = 5,
    }
    public Voxel(Vector3 position, VoxelType type, bool isActive = true)
    {
        this.position = position;
        this.type = type;
        this.isActive = isActive;
    }
}

