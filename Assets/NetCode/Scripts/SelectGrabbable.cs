using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectGrabbable : NetworkBehaviour
{
    public void OnSelectGrabbable(SelectEnterEventArgs eventArgs)
    {
        Debug.Log("Trying to pick up for this OwnerClientId " + OwnerClientId);
        Debug.Log("NetworkId " + NetworkObjectId);
        Debug.Log("IsClient: " + IsClient + " IsOwner: " + IsOwner);

        if (IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            Debug.Log("This is the selected object: " + networkObjectSelected);

            if (networkObjectSelected != null)
            {
                RequestGrabbableOwnerShipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnerShipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            if (networkObject.OwnerClientId == newOwnerClientId)
            {
                return;
            }
            Debug.Log("This is the selected object: " + networkObject);
            Debug.Log("And this the client ID: " + newOwnerClientId);
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.Log("Unable to change ownership for: " + newOwnerClientId);
        }
    }
}
