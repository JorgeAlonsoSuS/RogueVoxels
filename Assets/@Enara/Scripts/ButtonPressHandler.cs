using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NewBehaviourScript : MonoBehaviour
{

    public InputActionReference actionReference; // Referencia a la acci�n de entrada que quieres detectar
    [SerializeField]
    GameObject mano;
    public string spell = "laser";



    void OnEnable()
    {
        // Aseg�rate de que la acci�n est� habilitada
        actionReference.action.Enable();
        // Suscribirse al evento de performed (cuando la acci�n se realiza)
        actionReference.action.performed += OnActionPerformed;
    }

    void OnDisable()
    {
        // Desuscribirse del evento cuando se deshabilita el objeto
        actionReference.action.performed -= OnActionPerformed;
    }

    // M�todo que se llama cuando se realiza la acci�n
    void OnActionPerformed(InputAction.CallbackContext context)
    {
        ExecuteYourFunction();
    }

    // Tu funci�n personalizada
    void ExecuteYourFunction()
    {
        Debug.Log("Bot�n pulsado!");
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
