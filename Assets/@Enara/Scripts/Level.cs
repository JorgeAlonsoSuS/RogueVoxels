using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;

public class Level : MonoBehaviour
{
    [SerializeField] private int levelSize = 50;

    public static Level Instance { get; private set; }

    public Material[] materials; //Enara

    public GameObject amulet;

    public InputActionReference actionReference; // Referencia a la acción de entrada que quieres detectar

    public string spell = "laser";

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    GameObject XROrigin;

    [SerializeField]
    GameObject mano;

    [SerializeField]
    Gradient color1;
    [SerializeField]
    Gradient color2;
    [SerializeField]
    Gradient color3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mano.GetComponent<XRInteractorLineVisual>().validColorGradient = color1;
            text.text = "Spell: Laser";
            //DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        GenerateWorld();
        XROrigin = GameObject.Find("XR Origin (XR Rig)");
        XROrigin.transform.position = new Vector3(levelSize/2, levelSize+2, levelSize/2);
        
    }

    public void DestroyVoxelAtPoint(Vector3 point)
    {
        Debug.Log("Llega a level");
        VoxelCreator voxelCreator = GetComponentInChildren<VoxelCreator>();
        if (voxelCreator != null)
        {
            voxelCreator.interactVoxel(point, spell);
        }
    }


    private void GenerateWorld()
    {
        GameObject root = new GameObject("Root");
        root.transform.position = Vector3.zero;
        root.transform.parent = this.transform;
        VoxelCreator voxelCreator = root.AddComponent<VoxelCreator>();
        voxelCreator.Initialize(levelSize, amulet);

    }

    void OnEnable()
    {
        // Asegúrate de que la acción esté habilitada
        actionReference.action.Enable();
        // Suscribirse al evento de performed (cuando la acción se realiza)
        actionReference.action.performed += OnActionPerformed;
    }

    void OnDisable()
    {
        // Desuscribirse del evento cuando se deshabilita el objeto
        actionReference.action.performed -= OnActionPerformed;
    }

    // Método que se llama cuando se realiza la acción
    void OnActionPerformed(InputAction.CallbackContext context)
    {
        ChangeSpell();
    }

    // Tu función personalizada
    void ChangeSpell()
    {
        Debug.Log("Botón pulsado!");
        switch (spell)
        {
            case "laser":
                spell = "water";
                text.text = "Spell: Water";
                mano.GetComponent<XRInteractorLineVisual>().validColorGradient = color2;
                break;
            case "water":
                spell = "fire";
                text.text = "Spell: Fire";
                mano.GetComponent<XRInteractorLineVisual>().validColorGradient = color3;
                break;
            case "fire":
                spell = "laser";
                text.text = "Spell: Laser";
                mano.GetComponent<XRInteractorLineVisual>().validColorGradient = color1;
                break;
            default:
                Debug.LogWarning("Spell desconocido: " + spell);
                spell = "laser";
                text.text = "Spell: Laser";
                mano.GetComponent<XRInteractorLineVisual>().validColorGradient = color1;
                break;
        }

        
    }


}
