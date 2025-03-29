namespace ex2_containers;

public class CargoShip {
    private List<Container> _cargo  = [];
    
    public string ShipName {get; init;}
    public double MaxSpeed { get; init; }
    public int MaxContainers { get; init; }
    public double MaxWeightTones { get; init; }
    
    public double MaxWeightKg => MaxWeightTones * 1000;

    public double CurrentCargoWeight => double.Round(this._cargo.Sum(c => c.TotalContainerWeight),2);
    public int FreeContainerSpace => MaxContainers - _cargo.Count; 
    
    

    public CargoShip(string shipName, double maxSpeed, int maxContainers, double maxWeightTones) {
        this.ShipName = shipName;
        this.MaxSpeed = maxSpeed;
        this.MaxContainers = maxContainers;
        this.MaxWeightTones = maxWeightTones;
        
    }


    public bool CanContainerBeLoaded(Container container) {
        return
            (container.AssignedShip == null) &&
            (CurrentCargoWeight + container.TotalContainerWeight < MaxWeightKg) &&
            (FreeContainerSpace != 0);

    }
    
    
    public void LoadContainer(Container container) {

        if (CanContainerBeLoaded(container)) {
            this._cargo.Add(container);
            container.AssignToShip(this);
            return;
        }
        
        if (container.AssignedShip != null) {
            throw new InvalidOperationException("This container is already assigned to another ship.");
        }
        if (CurrentCargoWeight + container.TotalContainerWeight > MaxWeightKg) {
            throw new InvalidOperationException("Maximum allowed weight of cargo will be exceeded.");
        }
        if (FreeContainerSpace == 0 ) {
            throw new InvalidOperationException("Can not load more containers, exceeding ship capacity.");
        }
        
      
    }
    
    public void LoadContainer(List<Container> containers) {
        foreach (var con in containers) {
            LoadContainer(con);
        }
    }
    
    
	public void UnloadContainer(Container container) {
        if (container.AssignedShip != this) return;
        this._cargo.Remove(container);
        container.DeAssignFromShip(this);
    }
    public void UnloadContainer(List<Container> containers) {
        foreach (var con in containers) {
            UnloadContainer(con);
        }
    }


    public override string ToString() {
        var shipInfo = $"Cargo Ship {this.ShipName} Info: \n" +
                       $"Cargo: {this.CurrentCargoWeight} / {this.MaxWeightKg} KG " +
                       $"Containers: {this._cargo.Count} / {this.MaxContainers}";

        foreach (var con in _cargo) {
            shipInfo += $"\n{con}";
        }
        return shipInfo + "\n";
    }
    
}