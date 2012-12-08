<Query Kind="Program">
  <Reference Relative="..\bin\Debug\Cuscino.dll">&lt;Personal&gt;\Visual Studio 2010\Projects\Cuscino\bin\Debug\Cuscino.dll</Reference>
</Query>

public class Customer: Cuscino.CouchDoc
{
	public string Name { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string ZipCode { get; set; }
	public string Country { get; set; }
}

void Main()
{
	var client = new Cuscino.CouchClient("https://viva.cloudant.com",
						"oresssiongerarzandeseate", "hORkGBqNxlkCBhOnL6uaMScN");
	
	try
	{
	
	
		//// get document by ID
		//var myEnt = client.GetEntity<Customer>("test", result.InsertedId);
		//Console.WriteLine(myEnt.Name);
		// _id: d0745642ae52144daea5dcb7d0620856
		// _rev: 1-b37cb676b53ef8bf770e01d923825d66

	
		var myEnt = client.GetEntity<Customer>("customers", "d0745642ae52144daea5dcb7d0620856");
		myEnt.Dump();
	
		////apparently down't work
		//var dbs = client.GetAllDocuments("test");
		//foreach (var db in dbs)
		//{
		//    Console.WriteLine(db.ID);
		//}
	
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}

// Define other methods and classes here