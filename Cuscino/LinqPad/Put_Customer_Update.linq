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
		var customer = client.GetEntity<Customer>("customers", "d0745642ae52144daea5dcb7d0620856");
		customer.Name = "Ciccio Test";
		var result = client.PostEntity("customers", customer);
		Console.WriteLine(result.InsertedId);
	
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}

// Define other methods and classes here