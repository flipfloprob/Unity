﻿using UnityEngine;
using System.Collections;

public class Move{
	public Move(MoveNames move){
		this.moveType = move;
	}
	public MoveNames moveType = MoveNames.Tackle;
	public int level = 1;
	public float xp = 0;
	public float cooldown = 1;
	
	public float GetPPCost(){
		switch(moveType){
		case MoveNames.Tackle:			return 0.02f;
		case MoveNames.Scratch:			return 0.02f;
		case MoveNames.Growl:			return 0.02f;
		case MoveNames.TailWhip:		return 0.02f;
		}
		return 0;
	}
}

public enum MoveNames{
	Pound,
	KarateChop,
	DoubleSlap,
	CometPunch,
	MegaPunch,
	PayDay,
	FirePunch,
	IcePunch,
	ThunderPunch,
	Scratch,
	ViceGrip,
	Guilotine,
	RazorLeaf,
	SwordsDance,
	Cut,
	Gust,
	WingAttack,
	Whirlwind,
	Fly,
	Bind,
	Slam,
	VineWhip,
	Stomp,
	DoubleKick,
	MegaKick,
	JumpKick,
	RollingKick,
	SandAttack,
	Headbutt,
	HornAttack,
	FuryAttack,
	HornDrill,
	Tackle,
	BodySlam,
	Wrap,
	TakeDown,
	Thrash,
	DoubleEdge,
	TailWhip,
	PoisonSting,
	Twineedle,
	PineMissile,
	Leer,
	Bite,
	Growl,
	Roar,
	Sing,
	Supersonic,
	SonicBoom,
	Diable,
	Acid,
	Ember,
	Flamethrower,
	Mist,
	WaterGun,
	HydroPump,
	Surf,
	IceBeam,
	Blizzard,
	Psybeam,
	BubbleBeam,
	AuroraBeam,
	HyperBeam,
	Peck,
	DrillPeck,
	Submission,
	LowKick,
	Counter,
	SeismicToss,
	Strength,
	Absorb,
	MegaDrain,
	LeechSeed,
	Growth,
	RazorLead,
	SolarBeam,
	PoisonPowder,
	StunSpore,
	SleepPowder,
	PetalDance,
	StringShot,
	DragonRage,
	FireSpin,
	ThunderShock,
	Thunderbolt,
	ThunderWave,
	Thunder,
	RockThrow,
	Earthquake,
	Fissure,
	Dig,
	Toxic,
	Confusion,
	Psychic,
	Hypnosis,
	Meditate,
	Agility,
	QuickAttack,
	Rage,
	Teleport,
	NightShade,
	Mimic,
	Screech,
	DoubleTeam,
	Recover,
	Harden,
	Minimize,
	Smokescreen,
	ConfuseRay,
	Withdraw,
	DefenseCurl,
	Barrier,
	LightScreen,
	Haze,
	Reflect,
	FocusEnergy,
	Bide,
	Metronome,
	MirrorMove,
	SelfDestruct,
	EggBomb,
	Lick,
	Smog,
	Sludge,
	BoneClub,
	FireBlast,
	Waterfall,
	Clamp,
	Swift,
	SkullBash,
	SpikeCannon,
	Constrict,
	Amnesia,
	Kinesis,
	SoftBoiled,
	HighJumpKick,
	Glare,
	DreamEater,
	PoisonGas,
	Barrage,
	LeechLife,
	LovelyKiss,
	SkyAttack,
	Morph,
	Bubble,
	DizzyPunch,
	Spore,
	Flash,
	Psywave,
	Splash,
	AcidArmor,
	Crabhammer,
	Explosion,
	FurySwipes,
	Bonemerang,
	Rest,
	RockSlide,
	HyperFang,
	Sharpen,
	Conversion,
	TriAttack,
	SuperFang,
	Slash,
	Substitute,
	Struggle
}