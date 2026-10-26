using System;
using System.Collections.Generic;

namespace RobotBatterySystem
{
    // ===== Interface: defines the contract, holds no state =====
    public interface IBattery
    {
        int RemainingEnergy { get; }
        void Consume(int amount);
        void Recharge(int amount);
    }

    // ===== Battery implementations =====
    public class LithiumBattery : IBattery
    {
        private int _remainingEnergy;

        public int RemainingEnergy => _remainingEnergy;

        public LithiumBattery(int initialEnergy = 100)
        {
            _remainingEnergy = Math.Clamp(initialEnergy, 0, 100);
        }

        public void Consume(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Max(0, _remainingEnergy - amount);   // never below 0
        }

        public void Recharge(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Min(100, _remainingEnergy + amount); // never above 100
        }
    }

    public class SolarBattery : IBattery
    {
        private int _remainingEnergy;

        public int RemainingEnergy => _remainingEnergy;

        public SolarBattery(int initialEnergy = 100)
        {
            _remainingEnergy = Math.Clamp(initialEnergy, 0, 100);
        }

        public void Consume(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Max(0, _remainingEnergy - amount);
        }

        public void Recharge(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Min(100, _remainingEnergy + amount);
        }
    }

    // ===== Abstract Robot (Dependency Injection via constructor) =====
    public abstract class Robot
    {
        private readonly IBattery _battery;

        public string Name { get; }

        // Reads straight from the injected battery
        public int RemainingEnergy => _battery.RemainingEnergy;

        protected Robot(string name, IBattery battery)
        {
            Name = name;
            _battery = battery ?? throw new ArgumentNullException(nameof(battery));
        }

        public void Recharge(int amount)
        {
            _battery.Recharge(amount);
        }

        protected void UseEnergy(int amount)
        {
            _battery.Consume(amount);
        }

        public abstract void PerformTask();
    }

    // ===== Concrete robots =====
    public class CleaningRobot : Robot
    {
        public CleaningRobot(string name, IBattery battery) : base(name, battery) { }

        public override void PerformTask()
        {
            Console.WriteLine($"{Name} is cleaning... (-10 energy)");
            UseEnergy(10);
        }
    }

    public class DeliveryRobot : Robot
    {
        public DeliveryRobot(string name, IBattery battery) : base(name, battery) { }

        public override void PerformTask()
        {
            Console.WriteLine($"{Name} is delivering a package... (-25 energy)");
            UseEnergy(25);
        }
    }

    // ===== Main =====
    public class Program
    {
        public static void Main(string[] args)
        {
            // CleaningRobot + LithiumBattery, DeliveryRobot + SolarBattery
            Robot cleaner = new CleaningRobot("CleanBot", new LithiumBattery());
            Robot courier = new DeliveryRobot("DeliverBot", new SolarBattery());

            Console.WriteLine("=== Individual demonstration ===");
            Console.WriteLine($"{cleaner.Name} energy: {cleaner.RemainingEnergy}");
            cleaner.PerformTask();
            Console.WriteLine($"{cleaner.Name} energy: {cleaner.RemainingEnergy}");
            cleaner.Recharge(5);
            Console.WriteLine($"{cleaner.Name} after recharge(5): {cleaner.RemainingEnergy}");

            Console.WriteLine();
            Console.WriteLine($"{courier.Name} energy: {courier.RemainingEnergy}");
            courier.PerformTask();
            Console.WriteLine($"{courier.Name} energy: {courier.RemainingEnergy}");
            courier.Recharge(10);
            Console.WriteLine($"{courier.Name} after recharge(10): {courier.RemainingEnergy}");

            // Polymorphism: both robots in a List<Robot>
            Console.WriteLine();
            Console.WriteLine("=== Polymorphism: List<Robot> ===");
            List<Robot> robots = new List<Robot> { cleaner, courier };

            foreach (Robot robot in robots)
            {
                robot.PerformTask();
                Console.WriteLine($"  -> {robot.Name} remaining energy: {robot.RemainingEnergy}");
            }

            // Boundary check: energy never goes below 0 or above 100
            Console.WriteLine();
            Console.WriteLine("=== Boundary check ===");
            for (int i = 0; i < 5; i++) courier.PerformTask();
            Console.WriteLine($"{courier.Name} after many tasks: {courier.RemainingEnergy} (never below 0)");
            courier.Recharge(500);
            Console.WriteLine($"{courier.Name} after recharge(500): {courier.RemainingEnergy} (never above 100)");
        }
    }
}