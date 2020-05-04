using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorageScript : InitScript {
	public List<ResourceScript> resources;


    public Vector3[] slotPositions;
    public ResourceScript[] slots;
	//public string[] disallowedResourceNames;
	public Dictionary<ResourceData, int> totals;
	public Dictionary<ResourceData, int> maxima;
	private void Start() {
        
    }

    public override void Init() {
		totals = new Dictionary<ResourceData, int>();
		maxima = new Dictionary<ResourceData, int>();
		ResourceMaximum[] maximaComponents = GetComponents<ResourceMaximum>();
		foreach (ResourceMaximum maximum in maximaComponents) maxima[maximum.resource] = maximum.maximum;
		List<Vector3> positionsList = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag("DummyPos")) {
                positionsList.Add(transform.GetChild(i).localPosition);
            }
        }
        slotPositions = positionsList.ToArray();
        slots = new ResourceScript[slotPositions.Length];
    }

	public int GetCount(ResourceData resource) {
		if (!totals.ContainsKey(resource)) return 0;
		return totals[resource];
	}

	public int GetSpaceFor(ResourceData resource) {
		int c = 0;
		for (int i = 0; i < slots.Length; i++) {
			if (slots[i] == null) c += resource.stackSize;
			else if (slots[i].data == resource) c += resource.stackSize - slots[i].count;
		}
		if(maxima.ContainsKey(resource)) {
			if (c + GetCount(resource) > maxima[resource]) c = maxima[resource] - GetCount(resource);
		}
		return c;
	}

	public void AddResource(ResourceData resource, int count) {
		//this doesn't work if there isn't space in the storage
		if (totals.ContainsKey(resource)) { totals[resource] += count; } else { totals[resource] = count; }
		//First go through filling up existing stacks
		for (int i = 0; i < slots.Length; i++) {
            if(slots[i] != null && slots[i].data == resource) {
                if(count <= resource.stackSize - slots[i].count) {
                    slots[i].AddCount(count);
                    return;
                } else {
                    count -= resource.stackSize - slots[i].count;
                    slots[i].SetCount(resource.stackSize);
                }
            }
        }
        //Now add new stacks
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i] == null) {
                GameObject newStack = new GameObject();
				//if (GetComponent<GridObjectRendererScript>() != null) newStack.transform.SetParent(GetComponent<GridObjectRendererScript>().spriteObject.transform, false);
				if (GetComponent<GridObjectRendererScript>() != null) newStack.transform.SetParent(transform, false);
				else newStack.transform.SetParent(transform, false);
				//newStack.transform.localPosition = slotPositions[i];
                newStack.AddComponent<ResourceScript>().Init(resource, slotPositions[i]);
                slots[i] = newStack.GetComponent<ResourceScript>();
                if (count <= resource.stackSize) {
                    newStack.GetComponent<ResourceScript>().SetCount(count);
                    return;
                } else {
                    newStack.GetComponent<ResourceScript>().SetCount(resource.stackSize);
                    count -= resource.stackSize;
                }
            }
        }
	}

	public void RemoveResource(ResourceData resource, int count) {
		//Debug.Log("Taking " + count.ToString() + " " + name + " from destination."
		if(totals[resource] < count) {
			Debug.Log("Tried to take more resources than exist from a stack, reducing instead");
			count = totals[resource];
		}
		totals[resource] -= count;
        if (count > resource.stackSize) Debug.LogError("Taking more than a stack? Unlikely");
        for (int i = slots.Length - 1; i >= 0; i--) {
            if (slots[i] != null && slots[i].data == resource) {
                if (slots[i].count > count) {
                    slots[i].RemoveCount(count);
                    count = 0;
                    break;
                } else if (slots[i].count == count) {
                    Destroy(slots[i].gameObject);
                    slots[i] = null;
                    count = 0;
                    break;
                } else {
                    count -= slots[i].count;
                    Destroy(slots[i].gameObject);
                    slots[i] = null;
                }
            }
        }
    }

	


	public void TransferResource(ResourceData resource, int count, ResourceStorageScript other){
		//Debug.Log("Transfering " + count.ToString() + " " + name + " between storages.");
		if (totals[resource] < count) {
			Debug.Log("Tried to take more resources than exist from a stack, reducing instead");
			count = totals[resource];
		}
		totals[resource] -= count;
		if (count > resource.stackSize) Debug.LogError("Taking more than a stack? Unlikely");
		int removedCount = 0;
        for (int i = slots.Length - 1; i >= 0; i--) {
            if(slots[i] != null && slots[i].data == resource) {
                if (slots[i].count > count) { //Take rest to be removed, job done
                    slots[i].RemoveCount(count);
					removedCount += count;
                    count = 0;
                    break;
                } else if (slots[i].count == count) { //Take entire stack, job done
					removedCount += count;
					Destroy(slots[i].gameObject);
                    slots[i] = null;
                    count = 0;
                    break;
                } else { //Take all in stack, move to next
					removedCount += slots[i].count;
					count -= slots[i].count;
                    Destroy(slots[i].gameObject);
                    slots[i] = null;
                }
            }
        }
        if(count > 0) { Debug.LogError("Tried to take more resources than exist from a stack"); }
		other.AddResource(resource, removedCount);
        /*
        //ResourceScript stack = resources.Find(r => r.resourceName == name);
        if(stack.count < count) {
            
            return null;
        } else {
            GameObject newStack = Instantiate(stack.gameObject, parent.transform);
            newStack.GetComponent<ResourceScript>().SetCount(count);
            if(stack.count == count){
                resources.Remove(stack);
                Destroy(stack.gameObject);
            } else {
                stack.RemoveCount(count);
            }
            return newStack;
        }
        */
    }
}
