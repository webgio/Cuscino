<Query Kind="Program">
  <Reference Relative="..\bin\Debug\Cuscino.dll">&lt;MyDocuments&gt;\Visual Studio 2010\Projects\Cuscino\bin\Debug\Cuscino.dll</Reference>
  <Reference Relative="..\..\..\Rentals2012\Rentals2012.Domain\bin\Debug\Rentals2012.Domain.dll">&lt;MyDocuments&gt;\Visual Studio 2010\Rentals2012\Rentals2012.Domain\bin\Debug\Rentals2012.Domain.dll</Reference>
  <Namespace>Rentals2012.Domain</Namespace>
</Query>


void Main()
{
	var client = new Cuscino.CouchClient("http://127.0.0.1:5984", "linqtests", "", "");
	
	try
	{
	    client.CreateDatabaseIfNotExists();
		var entity = new Rentals2012.Domain.Entities.Equipment {
			Id = "d0745642ae52144daea5dcb7d0620856", 
			Name = "Qtestinga", 
			Type = "Equipment"};
		var result = client.PostEntity(entity);
		result.Dump();
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}

// Define other methods and classes here