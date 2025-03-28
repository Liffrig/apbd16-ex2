namespace ex2_containers;

public class CargoShip {
    public List<Container> Cargo { get; } = [];
    public double MaxSpeed { get; }
    public int MaxContainers { get; }
    public double MaxWeightTones { get; }
    
    public double MaxWeightKg => MaxWeightTones * 1000;

    public double CurrentCargoWeight => this.Cargo.Sum(c => c.TotalContainerWeight);
    public int FreeContainerSpace => MaxContainers - Cargo.Count; 
    
    

    public CargoShip(double maxSpeed, int maxContainers, double maxWeightTones) {
        this.MaxSpeed = maxSpeed;
        this.MaxContainers = maxContainers;
        this.MaxWeightTones = maxWeightTones;
        
    }

    public void LoadContainer(Container container) {
        
        if (container.AssignedShip != null) {
            throw new InvalidOperationException("This container is already assigned to another ship.");
        }

        if (CurrentCargoWeight + container.TotalContainerWeight > MaxWeightTones) {
            throw new InvalidOperationException("Maximum allowed weight of cargo will be exceeded.");
        }

        if (FreeContainerSpace == 0 ) {
            throw new InvalidOperationException("Can not load more containers, exceeding ship capacity.");
        }
        
        
        // finally!
        this.Cargo.Add(container);
        container.AssignToShip(this);



    }
    
}