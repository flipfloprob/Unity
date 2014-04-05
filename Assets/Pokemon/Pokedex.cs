using UnityEngine;
using System.Collections;

public class Pokedex{
	public enum State {Unknown, Seen, Captured};
	public static State[] states = new State[151];

	public static string PokeDexText(int number){
		switch(number){
		case 1:	return "Bulbasaur is a small, quadruped Pokémon with green or bluish green skin and dark patches. Its thick legs each end with three sharp claws. Its eyes have red irises, while the sclera and pupils are white. Bulbasaur has a pair of small, pointed teeth visible when its mouth is open. It has a bulb on its back, grown from a seed planted there at birth. The bulb provides it with energy through photosynthesis as well as from the nutrient-rich seeds contained within.\n\nstarter Pokémon are raised by Breeders to be distributed to new Trainers. Having been domesticated from birth, Bulbasaur is regarded as both a rare and well-behaved Pokémon. It is known to be extremely loyal even after long-term abandonment. Bulbasaur has also shown itself to be an excellent caretaker or children and pokemon alike.\n\nIt is found in grasslands and forests throughout the Kanto region. However, due to Bulbasaur's status as starter Pokémon, it is hard to come by in the wild and generally found under the ownership of a Trainer. It has been observed that a Bulbasaur's bulb will flash blue when it is ready to evolve. If it does not want to evolve, it must struggle to resist the transformation. It is claimed that many Bulbasaur gather every year in hidden gardens in Kanto to become Ivysaur in ceremonies led by Venusaurs.";
	    case 2: return "When the bulb on its back grows large, it appears to lose the ability to stand on its hind legs.";
        case 3: return "The plant blooms when it is absorbing solar energy. It stays on the move to seek sunlight.";
        case 4: return "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.";
        case 5: return "When it swings its burning tail, it elevates the temperature to unbearably high levels.";
        case 6: return "It spits fire that is hot enough to melt boulders. Known to cause forest fires unintentionally.";
        case 7: return "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth.";
        case 8: return "Often hides in water to stalk unwary prey. For swimming fast, it moves its ears to maintain balance.";
        case 9: return "A brutal Pokémon with pressurized water jets on its shell. They are used for high speed tackles.";
        case 10: return "Its short feet are tipped with suction pads that enable it to tirelessly climb slopes and walls.";
        case 11: return "This Pokémon is vulnerable to attack while its shell is soft, exposing its weak and tender body.";
        case 12: return "In battle, it flaps its wings at high speed, releasing highly toxic dust into the air.";
        case 13: return "Often found in forests, eating leaves. It has a sharp, venomous stinger on its head.";
        case 14: return "Almost incapable of moving, this Pokémon can only harden its shell to protect itself from predators.";
        case 15: return "Flies at high speed and attacks using its large venomous stingers on its forelegs and tail.";
        case 16: return "A common sight in forests and woods. It flaps its wings at ground level to kick up blinding sand.";
        case 17: return "Very protective of its sprawling territory, this Pokémon will fiercely peck at any intruder.";
        case 18: return "When hunting, it skims the surface of water at high speed to pick off unwary prey such as Magikarp.";
        case 19: return "Bites anything when it attacks. Small and very quick, it is a common sight in many places.";
        case 20: return "It uses its whiskers to maintain its balance and will slow down if they are cut off.";
        case 21: return "Eats bugs in grassy areas. It has to flap its short wings at high speed to stay airborne.";
        case 22: return "With its huge and magnificent wings, it can keep aloft without ever having to land for rest.";
        case 23: return "Moves silently and stealthily. Eats the eggs of birds, such as Pidgey and Spearow, whole.";
        case 24: return "It is rumored that the ferocious warning markings on its belly differ from area to area.";
        case 25: return "When several of these Pokémon gather, their electricity could build and cause lightning storms.";
        case 26: return "Its long tail serves as a ground to protect itself from its own high voltage power.";
        case 27: return "Burrows deep underground in arid locations far from water. It only emerges to hunt for food.";
        case 28: return "Curls up into a spiny ball when threatened. It can roll while curled up to attack or escape.";
        case 29: return "Although small, its venomous barbs render this Pokémon dangerous. The female has smaller horns.";
        case 30: return "The female's horn develops slowly. Prefers physical attacks such as clawing and biting.";
        case 31: return "Its hard scales provide strong protection. It uses its hefty bulk to execute powerful moves.";
        case 32: return "Stiffens its ears to sense danger. The larger its horns, the more powerful its secreted venom.";
        case 33: return "An aggressive Pokémon that is quick to attack. The horn on its head secretes a powerful venom.";
        case 34: return "It uses its powerful tail in battle to smash, constrict, then break the prey's bones.";
        case 35: return "Its magical and cute appeal has many admirers. It is rare and found only in certain areas.";
        case 36: return "A timid fairy Pokémon that is rarely seen. It will run and hide the moment it senses people.";
        case 37: return "At the time of birth, it has just one tail. The tail splits from its tip as it grows older.";
        case 38: return "Very smart and very vengeful. Grabbing one of its many tails could result in a 1000-year curse.";
        case 39: return "When its huge eyes light up, it sings a mysteriously soothing melody that lulls its enemies to sleep.";
        case 40: return "The body is soft and rubbery. When angered, it will suck in air and inflate itself to an enormous size.";
        case 41: return "Forms colonies in perpetually dark places. Uses ultrasonic waves to identify and approach targets.";
        case 42: return "Once it strikes, it will not stop draining energy from the victim even if it gets too heavy to fly.";
        case 43: return "During the day, it keeps its face buried in the ground. At night, it wanders around sowing its seeds.";
        case 44: return "The fluid that oozes from its mouth isn't drool. It is a nectar that is used to attract prey.";
        case 45: return "The larger its petals, the more toxic pollen it contains. Its big head is heavy and hard to hold up.";
        case 46: return "Burrows to suck tree roots. The mushrooms on its back grow by drawing nutrients from the bug host.";
		case 47: return "A host-parasite pair in which the parasite mushroom has taken over the host bug. Prefers damp places.";
        case 48: return "Lives in the shadows of tall trees where it eats bugs. It is attracted by light at night.";
        case 49: return "The dustlike scales covering its wings are color-coded to indicate the kinds of poison it has.";
        case 50: return "Lives about one yard underground where it feeds on plant roots. It sometimes appears aboveground.";
        case 51: return "A team of Diglett triplets. It triggers huge earthquakes by burrowing 60 miles underground.";
		}
		return "";
	}
}