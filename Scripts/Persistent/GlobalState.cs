using System;
using System.Collections.Generic;

public static class GlobalState
{
	private static bool _observatoryPowered;

	public static bool ObservatoryPowered
	{
		get => _observatoryPowered;
		set

		{
			if( value != _observatoryPowered )
			{
				_observatoryPowered = value;
				onObservatoryPowerToggle.Invoke( value );
			}
		}
	}

	public static Action<bool> onObservatoryPowerToggle;

	public static bool observatoryCrystalInserted = false;
	public static bool observatoryDoorUnlocked = false;

	private static int _waterLevel = 0;
	public static int lastSeenWaterLevel = 0;

	public static int[] ringPositions = new[]
	{
		-1, -1, -1, -1,
	};


	public static int WaterLevel
	{
		get => _waterLevel;
		set { _waterLevel = value; }
	}

	private static readonly HashSet<InventoryItem> Spawned = new HashSet<InventoryItem>();
	private static readonly HashSet<InventoryItem> Despawned = new HashSet<InventoryItem>();

	public static bool HasThisSpawned( InventoryItem item ) { return Spawned.Contains( item ); }

	public static bool HasThisDespawned( InventoryItem item ) { return Despawned.Contains( item ); }

	public static void MakeThisSpawn( InventoryItem item )
	{
		Despawned.Remove( item );
		Spawned.Add( item );
	}

	public static void MakeThisNotSpawn( InventoryItem item )
	{
		Spawned.Remove( item );
		Despawned.Add( item );
	}

	public static void ResetEverything()
	{
		_observatoryPowered        = false;
		observatoryCrystalInserted = false;
		observatoryDoorUnlocked    = false;

		_waterLevel        = 0;
		lastSeenWaterLevel = 0;

		ringPositions = new[]
		{
			-1, -1, -1, -1,
		};

		Spawned.Clear();
		Despawned.Clear();
	}
}
