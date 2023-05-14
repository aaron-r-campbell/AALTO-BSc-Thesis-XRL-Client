using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TextureScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Speed of the scrolling

    private Vector2 textureOffset = Vector2.zero;
    private Vector2 maxScrollRatio;

    private XRController leftController;
    private XRController rightController;
    private Dictionary<XRBaseController, bool> hoverStatus = new();


    public void Initialize(GameObject gameObject, Vector2 adjustmentRatio)
    {
        // Define max scroll ratios
        maxScrollRatio = Vector2.one - adjustmentRatio;
        textureOffset.y = maxScrollRatio.y;

        // Setup controllers
        leftController = GameObject.Find("LeftHand Controller").GetComponent<XRController>();
        rightController = GameObject.Find("RightHand Controller").GetComponent<XRController>();

        // Initialize hover values
        hoverStatus.Add(leftController, false);
        hoverStatus.Add(rightController, false);

        // Register to the hover events of the XR Grab Interactable component
        XRSimpleInteractable xrSimpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
        xrSimpleInteractable.hoverEntered.AddListener((args) => OnHoverChanged(args, true));
        xrSimpleInteractable.hoverExited.AddListener((args) => OnHoverChanged(args, false));
    }

    void Update()
    {
        // Initialize an offset
        float offsetX = 0f;
        float offsetY = 0f;

        // Update offset based on controller hover + thubsticks
        Vector2 leftAxes = hoverStatus[leftController] && leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 result) ? result : Vector2.zero;
        Vector2 rightAxes = hoverStatus[rightController] && rightController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out result) ? result : Vector2.zero;
        Vector2 axes = leftAxes + rightAxes;
        offsetX += axes.x;
        offsetY += axes.y;

        // Update offset based on arrow keys
        offsetX -= Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
        offsetX += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        offsetY += Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        offsetY -= Input.GetKey(KeyCode.DownArrow) ? 1 : 0;

        // Update the texture scrolled position clamping to prevent looping
        textureOffset.x = Math.Clamp(textureOffset.x + scrollSpeed * offsetX * Time.deltaTime, 0f, maxScrollRatio.x);
        textureOffset.y = Math.Clamp(textureOffset.y + scrollSpeed * offsetY * Time.deltaTime, 0f, maxScrollRatio.y);

        GetComponent<Renderer>().material.mainTextureOffset = textureOffset;
    }

    public void OnHoverChanged(BaseInteractionEventArgs args, bool hoverEntered)
    {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor && controllerInteractor != null)
        {
            hoverStatus[controllerInteractor.xrController] = hoverEntered;
        }
    }
}