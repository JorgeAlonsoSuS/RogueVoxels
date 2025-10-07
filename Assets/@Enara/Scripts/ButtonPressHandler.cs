using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NewBehaviourScript : MonoBehaviour
{

    public InputActionReference actionReference; // Referencia a la acción de entrada que quieres detectar
    [SerializeField]
    GameObject mano;
    public string spell = "laser";



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
        ExecuteYourFunction();
    }

    // Tu función personalizada
    void ExecuteYourFunction()
    {
        Debug.Log("Botón pulsado!");
        if(spell == "laser")
        {
            spell = "water";
        }

       if(spell == "water")
        {
            spell = "fire";
        }

       if(spell == "fire")
        {
            spell = "laser";
        }
        Debug.Log("Selected spell:" + spell);
    }
}
