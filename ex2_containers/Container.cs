namespace ex2_containers;

// pomocnicze

public interface IHazardNotifier {
    void Notify(string message);
}

public class OverfillException(string message) : Exception(message);

public enum ContainerType {
    L, G, C
}


public enum RefrigeratedProduct {
    Bananas,
    Chocolate,
    Fish,
    Meat,
    IceCream,
    FrozenPizza,
    Cheese,
    Sausages,
    Butter,
    Eggs
}

//


public abstract class Container {
    
    private static int _counter = 1;
    private static readonly List<Container> AllContainers = [];
    
    private ContainerType Type { get; }
    protected double NetWeight { get; set; }
    public double Height { get; private set; }
    public double TareWeight { get; private set; }
    public double Depth { get; private set; }
    public double MaxCapacity { get; private set; }
    public string SerialNo { get; init; }

    
    // wyliczalne właściwości
    public double TotalContainerWeight => NetWeight + TareWeight;
    public double FreeKgCapacity => MaxCapacity - NetWeight;
    
    public static IReadOnlyList<Container> GetAllContainers() => AllContainers.AsReadOnly();
    
    // referencja do statku
    public CargoShip? AssignedShip  { get; private set; }

    
    protected Container(ContainerType containerType, double maxCapacity, double height, double tareWeight, double depth) {
        this.Type = containerType;
        this.NetWeight = 0; // na razie jest pusty
        this.Height = height;
        this.TareWeight = tareWeight;
        this.Depth = depth;
        this.MaxCapacity = maxCapacity;
        this.SerialNo = $"KON-{this.Type.ToString()}-{Container._counter++}"; // od razu inkrementacja licznika

        Container.AllContainers.Add(this);

    }
    
    
    public virtual void Unload() { this.NetWeight = 0; }

    public virtual void Load(double massKg) {
        // można też "doładowywać" w ten sam sposób 
        if (this.FreeKgCapacity < massKg) {
            throw new OverfillException($"Maximum capacity for container {this.SerialNo} exceeded! Operation aborted.");} 
        
        NetWeight += massKg;
    } 
    
    public void AssignToShip(CargoShip ship)
    {
        if (AssignedShip != null)
        {
            throw new InvalidOperationException("This container is already assigned to a ship.");
        }
        AssignedShip = ship;
    }

    public void DeAssignFromShip(CargoShip ship) {
        if (this.AssignedShip != ship){ throw new InvalidOperationException("Ship validation failed. Operation aborted.");}
        
        this.AssignedShip = null;
    }
    
    
    public override string ToString() => $"""
                                         |---------------- Container: {this.SerialNo} ----------------|
                                         Dimesions: {this.Height}x{this.Height}x{this.Depth}         
                                         Cargo weight: {this.NetWeight} Kg / {this.MaxCapacity} Kg
                                         """;
    
}


// klasy dziedziczące
public class L_Container : Container, IHazardNotifier {
    
    // może kiedyś będzie można zmienić te wartości
    private static double MaxCapacityMultipAdr { get; set; } = 0.5;
    private static double MaxCapacityMultipBase { get; set; } = 0.9;
    
    
    public bool IsAdr { get; private set; }

    private double TrueCapacity =>
        IsAdr ?
             double.Round(L_Container.MaxCapacityMultipAdr * this.MaxCapacity, 2) :
             double.Round(L_Container.MaxCapacityMultipBase * this.MaxCapacity, 2);
    

    public L_Container(double maxCapacity, double height, double tareWeight, double depth, bool isADR)
        : base(ContainerType.L, maxCapacity, height, tareWeight, depth) {
        this.IsAdr = isADR;
    }

    public override void Load(double massKg) {
        if (massKg > this.TrueCapacity) {
            Notify($"Maximum safe capacity - {TrueCapacity} / {MaxCapacity} KG - exceeded! Operation aborted!");
            return;
        }
        
        base.Load(massKg);
    }


    public void Notify(string message) {
        Console.WriteLine($"Attention/Atencion/Achtung/Zhuyi - Container {this.SerialNo}");
        Console.WriteLine(message);
    }
}



public class G_Container : Container, IHazardNotifier {
    
    // może kiedyś będzie można zmienić te wartości
    private static double UnloadingLeftover { get; set; } = 0.05;
    
    public double Pressure { get; private set; }

    public G_Container(double maxCapacity, double height, double tareWeight, double depth, double pressure)
        : base(ContainerType.G, maxCapacity, height, tareWeight, depth) {
        this.Pressure = pressure;
    }

    public override void Unload() {
        this.NetWeight = double.Round(G_Container.UnloadingLeftover * this.NetWeight,2);
    }

    public override void Load(double massKg) {
        
        // Wyślij informacje o zagrożeniu
        if ((massKg + this.NetWeight) > this.MaxCapacity) {
            Notify("An attempt has been made to overfill a gas container!");
        }
        
        // Jeśli wysłąno info o zagrożeniu to i tak będzie OverfillException w base
        base.Load(massKg);
    }


    public void Notify(string message) {
        Console.WriteLine($"Attention/Atencion/Achtung/Zhuyi - Container {this.SerialNo}");
        Console.WriteLine(message);
    }
}


public class C_Container : Container {
    
    public RefrigeratedProduct RefrigeratedProduct { get; private set; }
    public double TemperatureKept { get;  private set; }
    
    private static readonly 
        Dictionary<RefrigeratedProduct, double> ExpectedTemperatures = new() {
        { RefrigeratedProduct.Bananas, 13.3 },
        { RefrigeratedProduct.Chocolate, 18.0 },
        { RefrigeratedProduct.Fish, 2.0 },
        { RefrigeratedProduct.Meat, -15.0 },
        { RefrigeratedProduct.IceCream, -18.0 },
        { RefrigeratedProduct.FrozenPizza, -30.0 },
        { RefrigeratedProduct.Cheese, 7.2 },
        { RefrigeratedProduct.Sausages, 5.0 },
        { RefrigeratedProduct.Butter, 20.5 },
        { RefrigeratedProduct.Eggs, 19.0 }
    };

    public C_Container(double maxCapacity, double height, double tareWeight, double depth, RefrigeratedProduct refrigeratedProduct, double temperatureKept )
    :base(ContainerType.C, maxCapacity, height, tareWeight, depth) {
        
        this.RefrigeratedProduct = refrigeratedProduct;
        
        if (Math.Abs(temperatureKept - C_Container.ExpectedTemperatures[RefrigeratedProduct]) > 0.001) {
            throw new ArgumentException($"Temperature of the container is too high: {temperatureKept} / {ExpectedTemperatures[RefrigeratedProduct]}");
        }
        this.TemperatureKept = temperatureKept;
    }

    public void Load(double massKg, RefrigeratedProduct loadedProduct) {
        if (loadedProduct != this.RefrigeratedProduct) {
            throw new ArgumentException($"Cant load different refrigerated products! Expected: {this.RefrigeratedProduct} Loaded: {loadedProduct}");
        }
        base.Load(massKg);
        
    }
    
}











