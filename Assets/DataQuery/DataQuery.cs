/*
 * Invokable interface to query the database for desirable data.
 * With the primary purpose to serve information into Data.
 * Low cost queries could be called on demand, avoiding complex
 * queries implementations in C#. Although not recommended.
 */

using UnityEngine;
using System.Collections;

public class DataQuery {
	public void GetPokemon(int maxID, int minID) {
		//Placeholder
	}
}
