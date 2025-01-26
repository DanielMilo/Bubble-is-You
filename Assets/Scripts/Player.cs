using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera mainCamera;
    private ModifierBubble pickedUpObject;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // On left mouse button down
        {
            if (pickedUpObject == null)
            {
                // Raycast to check if we clicked on an object with a SpriteRenderer
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    ModifierBubble bubble = hit.collider.GetComponent<ModifierBubble>();

                    if (bubble != null)
                    {
                        if (bubble.Attached)
                        {
                            bubble.Pop();
                        }
                        else
                        {
                            pickedUpObject = bubble;
                            pickedUpObject.PickUp();
                            Collider2D pickedUpCollider = hit.collider;
                            pickedUpCollider.enabled = false; // Disable the collider while dragging
                            offset = pickedUpObject.transform.position - GetMouseWorldPosition();
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && pickedUpObject != null) // On left mouse button release
        {
            // Raycast to check if we released on an "Actor" tagged object
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Actor modified = hit.collider.gameObject.GetComponent<Actor>();
                if(modified != null)
                {
                    if (pickedUpObject.ModifierData.Verb != Verb.None)
                    {
                        modified.Modify(pickedUpObject);
                        pickedUpObject.Attach(modified);
                    }
                    else
                    {
                        Debug.Log("Noun bubble dropped on Actor");
                        pickedUpObject.DropReset();
                    }
                }
                else
                {
                    ModifierBubble bubble = hit.collider.gameObject.GetComponent<ModifierBubble>();
                    if (bubble != null)
                    {
                        if(!bubble.Complete)
                        {
                            if (pickedUpObject.ModifierData.Noun != Noun.None)
                            {
                                bubble.CompleteBubble(pickedUpObject);
                            }
                            else
                            {
                                Debug.Log("Noun bubble dropped on Noun bubble");
                                pickedUpObject.DropReset();
                            }
                        }
                        else
                        {
                            Debug.Log("Dropped on complete bubble");
                            pickedUpObject.DropReset();
                        }
                    }
                    else
                    {
                        Debug.Log("Dropped on nothing");
                        pickedUpObject.DropReset();
                    }
                }
            }
            else
            {
                // Dropped elsewhere
                Debug.Log("Dropped outside any Collider.");
                pickedUpObject.DropReset();
            }

            pickedUpObject = null;
        }

        // If an object is picked up, make it follow the mouse
        if (pickedUpObject != null)
        {
            pickedUpObject.transform.position = GetMouseWorldPosition() + offset;
        }
    }

    // Helper function to get mouse world position
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z; // Set z-axis to camera's z distance
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
