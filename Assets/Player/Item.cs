using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item{
	public ItemTypes type = ItemTypes.Pokeball;
	public int number = 0;
	public Texture2D icon;

	public Item(ItemTypes type){
		this.type = type;
		this.number = 1;
		icon = (Texture2D)Resources.Load("Icons/"+type.ToString());
	}
	public Item(ItemTypes type, int number){
		this.type = type;
		this.number = number;
		icon = (Texture2D)Resources.Load("Icons/"+type.ToString());
	}

	public void Use(){
		switch(type){
		case ItemTypes.Pokeball:
			//Pokeball.ThrowPokeBall(Player.This.gameObject);
			number--;
			return;
		}
	}

	public static void CombineInventory(List<Item> inventory){
		for(int i=0; i<inventory.Count; i++){
			for(int j=i+1; j<inventory.Count; j++){
				if (inventory[i].type==inventory[j].type){
					inventory[i].number += inventory[j].number;
					inventory.Remove(inventory[j]);
				}
			}
		}
	}

	public static string ItemDescription(ItemTypes type){
		switch(type){
		case ItemTypes.Pokeball:	return "A device for catching wild Pokemon. It's thrown like a ball at a Pokemon, comfortably encapsulating its target.";
		case ItemTypes.Potion:		return "A spray-type medicine for treating wounds. It can be used to restore a small amount of HP to an injured Pokemon.";
		}
		return "";
	}
}

public enum ItemTypes{
	Pokeball,
	Greatball,
	Ultraball,
	Masterball,

	Potion,
	SuperPotion,
	HyperPotion,
	MaxPotion
};