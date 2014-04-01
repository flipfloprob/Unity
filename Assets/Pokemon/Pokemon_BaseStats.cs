using UnityEngine;
using System.Collections;

public class Pokemon_BaseStats{
	public static int Health(Pokemon_Names pokemon){
		switch(pokemon){
		case Pokemon_Names.Bulbasaur:		return 45;
		case Pokemon_Names.Charmander:		return 39;
		case Pokemon_Names.Squirtle:		return 44;
		case Pokemon_Names.Pidgey:			return 40;
		case Pokemon_Names.Rattata:			return 30;
		}
		return 50;
	}

	public static int Attack(Pokemon_Names pokemon){
		switch(pokemon){
		case Pokemon_Names.Bulbasaur:		return 49;
		case Pokemon_Names.Charmander:		return 52;
		case Pokemon_Names.Squirtle:		return 48;
		case Pokemon_Names.Pidgey:			return 45;
		case Pokemon_Names.Rattata:			return 56;
		}
		return 50;
	}

	public static int Defence(Pokemon_Names pokemon){
		switch(pokemon){
		case Pokemon_Names.Bulbasaur:		return 49;
		case Pokemon_Names.Charmander:		return 43;
		case Pokemon_Names.Squirtle:		return 65;
		case Pokemon_Names.Pidgey:			return 40;
		case Pokemon_Names.Rattata:			return 35;
		}
		return 50;
	}

	public static int Speed(Pokemon_Names pokemon){
		switch(pokemon){
		case Pokemon_Names.Bulbasaur:	return 45;
		case Pokemon_Names.Charmander:	return 65;
		case Pokemon_Names.Squirtle:	return 43;
		case Pokemon_Names.Pidgey:	return 56;
		case Pokemon_Names.Rattata:	return 72;
		}
		return 50;
	}
}