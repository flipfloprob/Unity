/*
 * This class binds the database's contents to native C# objects.
 */

using UnityEngine;
using System.Collections;

public class Data
{
	static public bool hasLoaded {get;set;}
	
	static Data() {
		hasLoaded = false;
	}
	
	//Connect to the database, query, record data and then DataQuery is be destroyed
	static public void Init() {
		//Remember to assert the loading of database and use try around the queries
		DataQuery query = new DataQuery();
		hasLoaded = true; //This will go in the final area postceeding try-catch
	}
	
	//Will free upon leaving through the menu.
	static public void Free() {
		hasLoaded = false;
	}
}
