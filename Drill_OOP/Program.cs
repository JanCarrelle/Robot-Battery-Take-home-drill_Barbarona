using System;
using System.Collections.Generic;
using System.Globalization;

// Base class 
class Vehicle
{
    protected int VehicleId;
    protected string Brand;
    protected double DailyRate;

    public Vehicle(int vehicleId, string brand, double dailyRate)
    {
        VehicleId = vehicleId;
        Brand = brand;
        DailyRate = dailyRate;
    }

    public virtual double CalculateRentalCost(int days)
    {
        return DailyRate * days;
    }

    public virtual void DisplayInfo()
    {
        Console.Write($"Vehicle [ID={VehicleId}, Brand={Brand}]");
    }

    // Get a short label for the vehicle
    public string GetLabel()
    {
        return $"{Brand} {GetType().Name}";
    }
}
// Inheritance: Car IS-A Vehicle 
class Car : Vehicle
{
    private int numSeats;

    public Car(int vehicleId, string brand, double dailyRate, int numSeats)
        : base(vehicleId, brand, dailyRate)
    {
        this.numSeats = numSeats;
    }

    public override double CalculateRentalCost(int days)
    {
        return base.CalculateRentalCost(days) * 1.10; // 10% insurance surcharge
    }

    public override void DisplayInfo()
    {
        Console.Write($"Car [ID={VehicleId}, Brand={Brand}, Seats={numSeats}]");
    }
}
//  Inheritance: Motorcycle IS-A Vehicle 
class Motorcycle : Vehicle
{
    private int engineCapacity;

    public Motorcycle(int vehicleId, string brand, double dailyRate, int engineCapacity)
        : base(vehicleId, brand, dailyRate)
    {
        this.engineCapacity = engineCapacity;
    }

    public override double CalculateRentalCost(int days)
    {
        return base.CalculateRentalCost(days) * 0.95; // 5% discount
    }

    public override void DisplayInfo()
    {
        Console.Write($"Motorcycle [ID={VehicleId}, Brand={Brand}, CC={engineCapacity}]");
    }
}

// Composition: Customer HAS-A Vehicle 
class Customer
{
    private string name;
    private string licenseNumber;
    private Vehicle? rentedVehicle;

    public Customer(string name, string licenseNumber)
    {
        this.name = name;
        this.licenseNumber = licenseNumber;
    }

    public void RentVehicle(Vehicle vehicle)
    {
        rentedVehicle = vehicle;
    }

    public void ReturnVehicle()
    {
        rentedVehicle = null;
    }

    public void PrintRentalSummary(int days)
    {
        Console.WriteLine($"Customer: {name} (License: {licenseNumber})");
        if (rentedVehicle == null)
        {
            Console.WriteLine("No vehicle currently rented.");
            return;
        }
        Console.WriteLine();
        Console.WriteLine($"Rented: {rentedVehicle.GetLabel()}");
        // Polymorphic call: runtime type decides which override runs
        double cost = rentedVehicle.CalculateRentalCost(days);
        Console.WriteLine($"Total Cost for {days} days: ${cost.ToString("F2", CultureInfo.InvariantCulture)}");
    }
}

// Demo 
class Program
{
    static void Main()
    {
        int days = 3;        
        int rentalDays = 5;  

        // Polymorphism
        List<Vehicle> vehicles = new List<Vehicle>
        {
            new Car(101, "Toyota", 100.00, 5),
            new Motorcycle(102, "Yamaha", 50.00, 150)
        };

        Console.WriteLine("=== Vehicle Rental Costs ===");
        foreach (Vehicle v in vehicles)
        {
            v.DisplayInfo();
            double cost = v.CalculateRentalCost(days);
            Console.WriteLine($" -> Cost for {days} days: ${cost.ToString("F2", CultureInfo.InvariantCulture)}");
        }

        // Composition
        Customer alice = new Customer("Alice", "AB1234");
        alice.RentVehicle(vehicles[0]);

        Console.WriteLine("=== Customer Rental Summary ===");
        alice.PrintRentalSummary(rentalDays);
    }
}