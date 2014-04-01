using UnityEngine;
using System.Collections;

public class Pokedex{
	public enum State {Unknown, Seen, Captured};
	public static State[] states = new State[151];

	public static string PokeDexText(int number){
		switch(number){
		case 1:	return "Bulbasaur is a small, quadruped Pokémon with green or bluish green skin and dark patches. Its thick legs each end with three sharp claws. Its eyes have red irises, while the sclera and pupils are white. Bulbasaur has a pair of small, pointed teeth visible when its mouth is open. It has a bulb on its back, grown from a seed planted there at birth. The bulb provides it with energy through photosynthesis as well as from the nutrient-rich seeds contained within.\n\nstarter Pokémon are raised by Breeders to be distributed to new Trainers. Having been domesticated from birth, Bulbasaur is regarded as both a rare and well-behaved Pokémon. It is known to be extremely loyal even after long-term abandonment. Bulbasaur has also shown itself to be an excellent caretaker or children and pokemon alike.\n\nIt is found in grasslands and forests throughout the Kanto region. However, due to Bulbasaur's status as starter Pokémon, it is hard to come by in the wild and generally found under the ownership of a Trainer. It has been observed that a Bulbasaur's bulb will flash blue when it is ready to evolve. If it does not want to evolve, it must struggle to resist the transformation. It is claimed that many Bulbasaur gather every year in hidden gardens in Kanto to become Ivysaur in ceremonies led by Venusaurs.";
		}
		return "";
	}
}