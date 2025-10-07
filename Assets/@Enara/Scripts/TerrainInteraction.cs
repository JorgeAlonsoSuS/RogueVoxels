using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class TerrainInteraction : MonoBehaviour
{
    public RaycastHit hit;

    [SerializeField]
    public AudioSource sound;
    [SerializeField]
    public AudioSource sound2;
    //public void getPointingTriangle()
    //{
    //    interactua();
    //    Debug.Log("Interacción");
    //    if (hit.collider != null)
    //    {
    //        Debug.Log("Hit collider is not null");
    //        if (hit.collider.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
    //        {
    //            Debug.Log("Hit collider has MeshFilter");
    //            Mesh mesh = meshFilter.mesh;
    //            int[] triangles = mesh.triangles;
    //            Vector3[] vertices = mesh.vertices;

    //            int triangleIndex = hit.triangleIndex * 3;
    //            Vector3 v0 = vertices[triangles[triangleIndex]];
    //            Vector3 v1 = vertices[triangles[triangleIndex + 1]];
    //            Vector3 v2 = vertices[triangles[triangleIndex + 2]];

    //            // Convertir los vértices al espacio mundial
    //            v0 = hit.collider.transform.TransformPoint(v0);
    //            v1 = hit.collider.transform.TransformPoint(v1);
    //            v2 = hit.collider.transform.TransformPoint(v2);

    //            // Usar barycentricCoordinate para interpolar el punto exacto
    //            Vector3 collisionPoint = v0 * hit.barycentricCoordinate.x + v1 * hit.barycentricCoordinate.y + v2 * hit.barycentricCoordinate.z;

    //            // Imprimir la información del punto exacto de colisión
    //            Debug.Log($"Punto exacto de colisión en la malla: {collisionPoint}");

    //            // Destruir el voxel en el punto de colisión
    //            Level.Instance.DestroyVoxelAtPoint(collisionPoint);
    //        }
    //    }
    //}

    // Esta función se llama desde el Inspector en el evento Select Entered
    public void getPointingTriangle(SelectEnterEventArgs args)
    {
        Debug.Log("getPointingTriangle called");

        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null)
        {
            Vector3 rayOrigin = interactor.transform.position;
            Vector3 rayDirection = interactor.transform.forward;
            Debug.Log($"Raycast from {rayOrigin} in direction {rayDirection}");

            if (Physics.Raycast(rayOrigin, rayDirection, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                if (hit.collider != null)
                {
                    Debug.Log("Hit collider is not null");
                    if (hit.collider.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                    {
                        Debug.Log("Hit collider has MeshFilter");

                        Mesh mesh = meshFilter.mesh;
                        int[] triangles = mesh.triangles;
                        Vector3[] vertices = mesh.vertices;

                        int triangleIndex = hit.triangleIndex * 3;
                        Vector3 v0 = vertices[triangles[triangleIndex]];
                        Vector3 v1 = vertices[triangles[triangleIndex + 1]];
                        Vector3 v2 = vertices[triangles[triangleIndex + 2]];

                        // Convertir los vértices al espacio mundial
                        v0 = hit.collider.transform.TransformPoint(v0);
                        v1 = hit.collider.transform.TransformPoint(v1);
                        v2 = hit.collider.transform.TransformPoint(v2);

                        // Usar barycentricCoordinate para interpolar el punto exacto
                        Vector3 collisionPoint = v0 * hit.barycentricCoordinate.x + v1 * hit.barycentricCoordinate.y + v2 * hit.barycentricCoordinate.z;

                        // Convertir el punto de colisión a las coordenadas locales del nivel
                        Vector3 localCollisionPoint = collisionPoint - transform.position;
                        Debug.Log($"Local collision point: {localCollisionPoint}");

                        // Destruir el voxel en el punto de colisión
                        Level.Instance.DestroyVoxelAtPoint(localCollisionPoint);
                    }
                    else
                    {
                        Debug.Log("Hit collider does not have MeshFilter.");
                    }
                }
                else
                {
                    Debug.Log("Hit collider is null.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
        else
        {
            Debug.Log("Interactor is null.");
        }
    }

    public void GrabAmulet()
    {
        SceneManager.LoadScene("WinScene");
    }

    public void interactua()
    {
        Debug.Log("Interactua");
        sound2.Play();
    }

}
