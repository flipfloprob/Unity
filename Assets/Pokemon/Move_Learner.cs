using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moves_Learner{

	public void PopulateMoves(Pokemon_Names pokemon, List<Move> moves, int level){
		switch(pokemon){

		case Pokemon_Names.Bulbasaur:
			moves.Add(new Move(MoveNames.Tackle));
			moves.Add(new Move(MoveNames.Growl));
			if (level >= 7)	moves.Add(new Move(MoveNames.LeechSeed));
            if (level >= 13)	moves.Add(new Move(MoveNames.VineWhip));
            if (level >= 20) moves.Add(new Move(MoveNames.PoisonPowder));
            if (level >= 27) moves.Add(new Move(MoveNames.RazorLeaf));
            if (level >= 34) moves.Add(new Move(MoveNames.Growth));
            if (level >= 41) moves.Add(new Move(MoveNames.SleepPowder));
            if (level >= 48) moves.Add(new Move(MoveNames.SolarBeam));
			break;

        case Pokemon_Names.Ivysaur:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.LeechSeed));
            if (level >= 13) moves.Add(new Move(MoveNames.VineWhip));
            if (level >= 22) moves.Add(new Move(MoveNames.PoisonPowder));
            if (level >= 30) moves.Add(new Move(MoveNames.RazorLeaf));
            if (level >= 38) moves.Add(new Move(MoveNames.Growth));
            if (level >= 46) moves.Add(new Move(MoveNames.SleepPowder));
            if (level >= 54) moves.Add(new Move(MoveNames.SolarBeam));
            break;

        case Pokemon_Names.Venusaur:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.LeechSeed));
            moves.Add(new Move(MoveNames.VineWhip));
            if (level >= 22) moves.Add(new Move(MoveNames.PoisonPowder));
            if (level >= 30) moves.Add(new Move(MoveNames.RazorLeaf));
            if (level >= 43) moves.Add(new Move(MoveNames.Growth));
            if (level >= 55) moves.Add(new Move(MoveNames.SleepPowder));
            if (level >= 65) moves.Add(new Move(MoveNames.SolarBeam));
            break;

        //Charmander

		case Pokemon_Names.Charmander:
			moves.Add(new Move(MoveNames.Scratch));
			 moves.Add(new Move(MoveNames.Growl));
			if (level >= 9)	moves.Add(new Move(MoveNames.Ember));
			if (level >= 15) moves.Add(new Move(MoveNames.Leer));
            if (level >= 22) moves.Add(new Move(MoveNames.Rage));
            if (level >= 30) moves.Add(new Move(MoveNames.Slash));
            if (level >= 38) moves.Add(new Move(MoveNames.Flamethrower));
            if (level >= 46) moves.Add(new Move(MoveNames.FireSpin));
			break;

        case Pokemon_Names.Charmeleon:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Ember));
            if (level >= 15) moves.Add(new Move(MoveNames.Leer));
            if (level >= 24) moves.Add(new Move(MoveNames.Rage));
            if (level >= 33) moves.Add(new Move(MoveNames.Slash));
            if (level >= 42) moves.Add(new Move(MoveNames.Flamethrower));
            if (level >= 56) moves.Add(new Move(MoveNames.FireSpin));
            break;

        case Pokemon_Names.Charizard:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Ember));
            moves.Add(new Move(MoveNames.Leer));
            if (level >= 24) moves.Add(new Move(MoveNames.Rage));
            if (level >= 36) moves.Add(new Move(MoveNames.Slash));
            if (level >= 46) moves.Add(new Move(MoveNames.Flamethrower));
            if (level >= 55) moves.Add(new Move(MoveNames.FireSpin));
            break;

        //Squirtle

		case Pokemon_Names.Squirtle:
			moves.Add(new Move(MoveNames.Tackle));
			moves.Add(new Move(MoveNames.TailWhip));
			if (level >= 8)	moves.Add(new Move(MoveNames.Bubble));
			if (level >= 15) moves.Add(new Move(MoveNames.WaterGun));
            if (level >= 22) moves.Add(new Move(MoveNames.Bite));
            if (level >= 28) moves.Add(new Move(MoveNames.Withdraw));
            if (level >= 35) moves.Add(new Move(MoveNames.SkullBash));
            if (level >= 42) moves.Add(new Move(MoveNames.HydroPump));
            break;

        case Pokemon_Names.Wartortle:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Bubble));
            if (level >= 15) moves.Add(new Move(MoveNames.WaterGun));
            if (level >= 24) moves.Add(new Move(MoveNames.Bite));
            if (level >= 31) moves.Add(new Move(MoveNames.Withdraw));
            if (level >= 39) moves.Add(new Move(MoveNames.SkullBash));
            if (level >= 47) moves.Add(new Move(MoveNames.HydroPump));
            break;

        case Pokemon_Names.Blastoise:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Bubble));
            moves.Add(new Move(MoveNames.WaterGun));
            if (level >= 24) moves.Add(new Move(MoveNames.Bite));
            if (level >= 31) moves.Add(new Move(MoveNames.Withdraw));
            if (level >= 42) moves.Add(new Move(MoveNames.SkullBash));
            if (level >= 52) moves.Add(new Move(MoveNames.HydroPump));
            break;

        //Caterpie

        case Pokemon_Names.Caterpie:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.StringShot));
            break;

        case Pokemon_Names.Metapod:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.StringShot));
            moves.Add(new Move(MoveNames.Harden));
            break;

        case Pokemon_Names.Butterfree:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.StringShot));
            moves.Add(new Move(MoveNames.Harden));
            moves.Add(new Move(MoveNames.Confusion));
            if (level >= 13) moves.Add(new Move(MoveNames.PoisonPowder));
            if (level >= 14) moves.Add(new Move(MoveNames.StunSpore));
            if (level >= 15) moves.Add(new Move(MoveNames.SleepPowder));
            if (level >= 18) moves.Add(new Move(MoveNames.Supersonic));
            if (level >= 23) moves.Add(new Move(MoveNames.Whirlwind));
            if (level >= 28) moves.Add(new Move(MoveNames.Gust));
            if (level >= 34) moves.Add(new Move(MoveNames.Psybeam));
            break;

        //Weedle

        case Pokemon_Names.Weedle:
            moves.Add(new Move(MoveNames.PoisonSting));
            moves.Add(new Move(MoveNames.StringShot));
            break;

        case Pokemon_Names.Kakuna:
            moves.Add(new Move(MoveNames.PoisonSting));
            moves.Add(new Move(MoveNames.StringShot));
            moves.Add(new Move(MoveNames.Harden));
            break;

        case Pokemon_Names.Beedrill:
            moves.Add(new Move(MoveNames.PoisonSting));
            moves.Add(new Move(MoveNames.StringShot));
            moves.Add(new Move(MoveNames.Harden));
            moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 16) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 20) moves.Add(new Move(MoveNames.Twineedle));
            if (level >= 25) moves.Add(new Move(MoveNames.Rage));
            if (level >= 30) moves.Add(new Move(MoveNames.PineMissile));
            if (level >= 35) moves.Add(new Move(MoveNames.Agility));
            break;

        //Pidgey

        case Pokemon_Names.Pidgey:
            moves.Add(new Move(MoveNames.Gust));
            if (level >= 5) moves.Add(new Move(MoveNames.SandAttack));
            if (level >= 12) moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 19) moves.Add(new Move(MoveNames.Whirlwind));
            if (level >= 28) moves.Add(new Move(MoveNames.WingAttack));
            if (level >= 36) moves.Add(new Move(MoveNames.Agility));
            if (level >= 44) moves.Add(new Move(MoveNames.MirrorMove));
            break;

        case Pokemon_Names.Pidgeotto:
            moves.Add(new Move(MoveNames.Gust));
            moves.Add(new Move(MoveNames.SandAttack));
            if (level >= 12) moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 21) moves.Add(new Move(MoveNames.Whirlwind));
            if (level >= 31) moves.Add(new Move(MoveNames.WingAttack));
            if (level >= 40) moves.Add(new Move(MoveNames.Agility));
            if (level >= 49) moves.Add(new Move(MoveNames.MirrorMove));
            break;

        case Pokemon_Names.Pidgeot:
            moves.Add(new Move(MoveNames.PoisonSting));
            moves.Add(new Move(MoveNames.StringShot));
            moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 21) moves.Add(new Move(MoveNames.Whirlwind));
            if (level >= 31) moves.Add(new Move(MoveNames.WingAttack));
            if (level >= 44) moves.Add(new Move(MoveNames.Agility));
            if (level >= 54) moves.Add(new Move(MoveNames.MirrorMove));
            break;

        //Rattata

		case Pokemon_Names.Rattata:
			moves.Add(new Move(MoveNames.Tackle));
			moves.Add(new Move(MoveNames.TailWhip));
			if (level >= 7)	moves.Add(new Move(MoveNames.QuickAttack));
			if (level >= 14) moves.Add(new Move(MoveNames.HyperFang));
            if (level >= 23) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 34) moves.Add(new Move(MoveNames.SuperFang));
			break;

        case Pokemon_Names.Raticate:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.TailWhip));
            moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 14) moves.Add(new Move(MoveNames.HyperFang));
            if (level >= 27) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 41) moves.Add(new Move(MoveNames.SuperFang));
            break;

        //Spearow

        case Pokemon_Names.Spearow:
            moves.Add(new Move(MoveNames.Peck));
            moves.Add(new Move(MoveNames.Growl));
            if (level >= 9) moves.Add(new Move(MoveNames.Leer));
            if (level >= 15) moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 22) moves.Add(new Move(MoveNames.MirrorMove));
            if (level >= 29) moves.Add(new Move(MoveNames.DrillPeck));
            if (level >= 36) moves.Add(new Move(MoveNames.Agility));
            break;

        case Pokemon_Names.Fearow:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.TailWhip));
            moves.Add(new Move(MoveNames.Leer));
            if (level >= 15) moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 25) moves.Add(new Move(MoveNames.MirrorMove));
            if (level >= 34) moves.Add(new Move(MoveNames.DrillPeck));
            if (level >= 43) moves.Add(new Move(MoveNames.Agility));
            break;

        //Ekans

        case Pokemon_Names.Ekans:
            moves.Add(new Move(MoveNames.Wrap));
            moves.Add(new Move(MoveNames.Leer));
            if (level >= 10) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 17) moves.Add(new Move(MoveNames.Bite));
            if (level >= 24) moves.Add(new Move(MoveNames.Glare));
            if (level >= 31) moves.Add(new Move(MoveNames.Screech));
            if (level >= 38) moves.Add(new Move(MoveNames.Acid));
            break;

        case Pokemon_Names.Arbok:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.TailWhip));
            moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 17) moves.Add(new Move(MoveNames.Bite));
            if (level >= 27) moves.Add(new Move(MoveNames.Glare));
            if (level >= 36) moves.Add(new Move(MoveNames.Screech));
            if (level >= 47) moves.Add(new Move(MoveNames.Acid));
            break;

        //Pikachu

        case Pokemon_Names.Pikachu:
            moves.Add(new Move(MoveNames.ThunderShock));
            moves.Add(new Move(MoveNames.Growl));
            if (level >= 6) moves.Add(new Move(MoveNames.TailWhip));
            if (level >= 8) moves.Add(new Move(MoveNames.ThunderWave));
            if (level >= 11) moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 15) moves.Add(new Move(MoveNames.DoubleTeam));
            if (level >= 20) moves.Add(new Move(MoveNames.Slam));
            if (level >= 26) moves.Add(new Move(MoveNames.Thunderbolt));
            if (level >= 33) moves.Add(new Move(MoveNames.Agility));
            if (level >= 41) moves.Add(new Move(MoveNames.Thunder));
            if (level >= 50) moves.Add(new Move(MoveNames.LightScreen));
            break;

        case Pokemon_Names.Raichu:
            moves.Add(new Move(MoveNames.ThunderShock));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.TailWhip));
            if (level >= 11) moves.Add(new Move(MoveNames.ThunderWave));
            if (level >= 13) moves.Add(new Move(MoveNames.QuickAttack));
            if (level >= 19) moves.Add(new Move(MoveNames.DoubleTeam));
            if (level >= 24) moves.Add(new Move(MoveNames.Slam));
            if (level >= 30) moves.Add(new Move(MoveNames.Thunderbolt));
            if (level >= 37) moves.Add(new Move(MoveNames.Agility));
            if (level >= 43) moves.Add(new Move(MoveNames.Thunder));
            if (level >= 55) moves.Add(new Move(MoveNames.Swift));
            break;

        //Sandshrew

        case Pokemon_Names.Sandshrew:
            moves.Add(new Move(MoveNames.Scratch));
            if (level >= 10) moves.Add(new Move(MoveNames.SandAttack));
            if (level >= 17) moves.Add(new Move(MoveNames.Slash));
            if (level >= 24) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 31) moves.Add(new Move(MoveNames.Swift));
            if (level >= 38) moves.Add(new Move(MoveNames.FurySwipes));
            break;

        case Pokemon_Names.Sandslash:
            moves.Add(new Move(MoveNames.Scratch));
            moves.Add(new Move(MoveNames.SandAttack));
            if (level >= 17) moves.Add(new Move(MoveNames.Slash));
            if (level >= 27) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 36) moves.Add(new Move(MoveNames.Swift));
            if (level >= 47) moves.Add(new Move(MoveNames.FurySwipes));
            break;

        //NidoranF

        case Pokemon_Names.NidoranF:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            if (level >= 8) moves.Add(new Move(MoveNames.Scratch));
            if (level >= 12) moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 17) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 23) moves.Add(new Move(MoveNames.TailWhip));
            if (level >= 30) moves.Add(new Move(MoveNames.Bite));
            if (level >= 38) moves.Add(new Move(MoveNames.FurySwipes));
            break;

        case Pokemon_Names.Nidorina:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Scratch));
            if (level >= 12) moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 19) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 27) moves.Add(new Move(MoveNames.TailWhip));
            if (level >= 36) moves.Add(new Move(MoveNames.Bite));
            if (level >= 46) moves.Add(new Move(MoveNames.FurySwipes));
			break;

        case Pokemon_Names.Nidoqueen:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Scratch));
            moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 20) moves.Add(new Move(MoveNames.TailWhip));
            if (level >= 32) moves.Add(new Move(MoveNames.Bite));
            if (level >= 39) moves.Add(new Move(MoveNames.FurySwipes));
            if (level >= 46) moves.Add(new Move(MoveNames.BodySlam));
            break;

        //NidoranM

        case Pokemon_Names.NidoranM:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Leer));
            if (level >= 8) moves.Add(new Move(MoveNames.HornAttack));
            if (level >= 12) moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 17) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 23) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 30) moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 38) moves.Add(new Move(MoveNames.HornDrill));
            break;

        case Pokemon_Names.Nidorino:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.HornAttack));
            if (level >= 12) moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 19) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 27) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 36) moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 46) moves.Add(new Move(MoveNames.HornDrill));
			break;

        case Pokemon_Names.Nidoking:
            moves.Add(new Move(MoveNames.Tackle));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Scratch));
            moves.Add(new Move(MoveNames.DoubleKick));
            if (level >= 20) moves.Add(new Move(MoveNames.PoisonSting));
            if (level >= 32) moves.Add(new Move(MoveNames.FocusEnergy));
            if (level >= 39) moves.Add(new Move(MoveNames.FuryAttack));
            if (level >= 46) moves.Add(new Move(MoveNames.Thrash));
            break;

        //Clefairy

        case Pokemon_Names.Clefairy:
            moves.Add(new Move(MoveNames.Pound));
            moves.Add(new Move(MoveNames.Growl));
            if (level >= 13) moves.Add(new Move(MoveNames.Sing));
            if (level >= 18) moves.Add(new Move(MoveNames.DoubleSlap));
            if (level >= 24) moves.Add(new Move(MoveNames.Minimize));
            if (level >= 31) moves.Add(new Move(MoveNames.Metronome));
            if (level >= 39) moves.Add(new Move(MoveNames.DefenseCurl));
            if (level >= 38) moves.Add(new Move(MoveNames.LightScreen));
            break;

        case Pokemon_Names.Clefable:
            moves.Add(new Move(MoveNames.Pound));
            moves.Add(new Move(MoveNames.Growl));
            moves.Add(new Move(MoveNames.Sing));
            if (level >= 20) moves.Add(new Move(MoveNames.DoubleSlap));
            if (level >= 29) moves.Add(new Move(MoveNames.Minimize));
            if (level >= 33) moves.Add(new Move(MoveNames.Metronome));
            if (level >= 41) moves.Add(new Move(MoveNames.DefenseCurl));
            if (level >= 44) moves.Add(new Move(MoveNames.LightScreen));
            break;

		}
	}
}