using HotelManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
namespace HotelManagementSystem.Infrastructure;
public static class CatalogSeeder
{
 public static async Task SeedAsync(HmsDbContext db)
 {
  var cities=new[]{"Tbilisi","Batumi","Kutaisi","Kazbegi","Telavi","Mestia","Kobuleti"};
  var styles=new[]{"Garden","Terrace","House","Suites","Grand","View","Boutique","Heritage","Riverside","Sky"};
  foreach(var city in cities){var count=await db.Hotels.CountAsync(x=>x.City==city);for(var i=count+1;i<=20;i++)db.Hotels.Add(new Hotel{Name=$"{city} {styles[(i-1)%styles.Length]} {i}",Rating=3+i%3,Country="Georgia",City=city,Address=$"{10+i} {city} Central Street"});}
  await db.SaveChangesAsync();

  var types=new[]{("Compact Room",1,18,false,75m),("Classic Double",2,23,false,95m),("Breakfast Double",2,25,true,115m),("Deluxe King",2,30,true,145m),("Twin Room",2,28,true,135m),("Junior Suite",3,38,true,180m),("Family Studio",4,44,true,210m),("Family Suite",5,55,true,255m),("Panorama Suite",3,48,true,275m),("Residence",6,70,true,330m)};
  var hotels=await db.Hotels.Include(x=>x.Rooms).ToListAsync();
  foreach(var h in hotels){for(var i=h.Rooms.Count;i<10;i++){var t=types[i];db.Rooms.Add(new Room{Name=t.Item1,Capacity=t.Item2,SizeSquareMeters=t.Item3,BreakfastIncluded=t.Item4,Price=t.Item5+h.Rating*10,HotelId=h.Id});}}
  await db.SaveChangesAsync();

  var cars=new[]{
   ("Toyota","Yaris","Economy",4,2,"Automatic","Hybrid",39m,2024),
   ("Hyundai","Elantra","Compact",5,3,"Automatic","Petrol",49m,2023),
   ("Kia","Sportage","SUV",5,4,"Automatic","Petrol",69m,2024),
   ("Toyota","RAV4","SUV",5,4,"Automatic","Hybrid",79m,2024),
   ("Mercedes","Vito","Van",8,6,"Automatic","Diesel",109m,2023),
   ("Suzuki","Jimny","4x4",4,2,"Manual","Petrol",64m,2024),
   ("Volkswagen","Golf","Compact",5,3,"Manual","Petrol",46m,2023),
   ("BMW","X3","Premium SUV",5,4,"Automatic","Diesel",115m,2023),
   ("Renault","Duster","SUV",5,4,"Manual","Petrol",58m,2024),
   ("Ford","Transit","Van",9,7,"Manual","Diesel",119m,2023)
  };
  foreach(var city in cities)
  {
   var existing=await db.Cars.Where(x=>x.City==city).OrderBy(x=>x.Id).ToListAsync();
   var canonical=new Dictionary<string,Car>(StringComparer.OrdinalIgnoreCase);
   foreach(var group in existing.GroupBy(x=>$"{x.Brand.Trim()}|{x.Model.Trim()}",StringComparer.OrdinalIgnoreCase))
   {
    var keep=group.First();
    canonical[group.Key]=keep;
    foreach(var duplicate in group.Skip(1))
    {
     var rentals=await db.CarRentals.Where(x=>x.CarId==duplicate.Id).ToListAsync();
     foreach(var rental in rentals)rental.CarId=keep.Id;
     db.Cars.Remove(duplicate);
    }
   }
   foreach(var c in cars)
   {
    var key=$"{c.Item1}|{c.Item2}";
    if(!canonical.TryGetValue(key,out var car))
    {
     car=new Car{City=city,Brand=c.Item1,Model=c.Item2};
     db.Cars.Add(car);
     canonical[key]=car;
    }
    car.Category=c.Item3;car.Seats=c.Item4;car.Luggage=c.Item5;car.Transmission=c.Item6;car.Fuel=c.Item7;car.DailyPrice=c.Item8;car.Year=c.Item9;car.AirConditioning=true;car.UnlimitedKilometers=true;
   }
  }
  await db.SaveChangesAsync();
 }
}
