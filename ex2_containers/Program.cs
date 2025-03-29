using ex2_containers;

// Stworzenie kontenera danego typu
var conL = new L_Container(250,2.31,64,28.6,false);
var conG = new G_Container(345,2.56,36,17.8,12.3);
var conC = new C_Container(490,3.33,31,33.3,RefrigeratedProduct.Bananas,13.3);

var conL1 = new L_Container(127,3.1,63,13.3,true);
var conG1 = new G_Container(123,2.17,92,22.3,7);
var conC1 = new C_Container(510,2.56,44,12.3,RefrigeratedProduct.FrozenPizza,-30);


// Tworzenie statków
var evergreen1 = new CargoShip("EverGreen1",23, 2400, 12);
var evergreen2 = new CargoShip("EverGreen2",25, 3000, 13);

Console.WriteLine("### Załadowanie ładunku do danego kontenera ###");
foreach (var con in Container.GetAllContainers()) {
    
    // metoda Load dla C_Container musi mieć trochę inną sygnaturę :(
    if (con.GetType() == typeof(C_Container)) {
        var prod = (con as C_Container).RefrigeratedProduct;
        (con as C_Container).Load( 
            double.Round(ThrowDice() * con.FreeKgCapacity,2), prod);
    }
    else {
        // randomowa waga ładunku
        con.Load(double.Round(ThrowDice() * con.FreeKgCapacity,2));
    }
    // Wypisanie informacji o danym kontenerze
    Console.WriteLine(con);
}


Console.WriteLine("### Załadowanie kontenera na statek ###");
for (var i = 0; i < Container.GetAllContainers().Count; i++) {
    // ładowanie na zmianę
    var correctShip = i%2 == 0 ? evergreen1 : evergreen2;
    correctShip.LoadContainer(Container.GetAllContainers()[i]);
}
// Wypisanie informacji o danym statku i jego ładunk
Console.WriteLine(evergreen1);
Console.WriteLine(evergreen2);


Console.WriteLine("### Usunięcie kontenera ze statku ###");
foreach (var con in Container.GetAllContainers()) {
    evergreen1.UnloadContainer(con);
    evergreen2.UnloadContainer(con);
}

Console.WriteLine(evergreen1);
Console.WriteLine(evergreen2);

Console.WriteLine("### Załadowanie listy kontenerów na statek ###");
Console.WriteLine(evergreen1);
evergreen1.LoadContainer([conL, conG, conC, conC1, conL1, conG1]);
Console.WriteLine(evergreen1);

Console.WriteLine("### Rozładowanie kontenera. ###");
evergreen1.UnloadContainer(conG);
Console.WriteLine(conG);
conG.Unload();
Console.WriteLine(conG);


Console.WriteLine("### Zastąpienie kontenera na statku o danym numerze innym kontenerem ###");
Console.WriteLine(evergreen1);
ReplaceContainers(conL, conG);
Console.WriteLine(evergreen1);


Console.WriteLine("### Możliwość przeniesienie kontenera między dwoma statkami ###");
// przygotowanie
evergreen2.LoadContainer(conL);
evergreen1.UnloadContainer([conC, conC1, conL1, conG1]);

Console.WriteLine(evergreen1);
Console.WriteLine(evergreen2);
SwapContainers(conL, conG);
Console.WriteLine(evergreen1);
Console.WriteLine(evergreen2);



static void ReplaceContainers(Container toBeReplaced, Container replacement) {
    if (toBeReplaced.AssignedShip != null || replacement.AssignedShip == null) {
        var destination = toBeReplaced.AssignedShip;
        destination.UnloadContainer(toBeReplaced);

        try {
            destination.LoadContainer(replacement);
        }
        catch (InvalidOperationException e) {
            // jeśli nie pykło to ładujemy z powrotem
            Console.WriteLine("An error occured while replacing a container! Operation aborted");
            Console.WriteLine(e.Message);
            destination.LoadContainer(toBeReplaced);
        }
        
    }
    else {
        throw new InvalidOperationException("You can only swap containers between ship and port.");
    }
}


static void SwapContainers(Container leftCon, Container rightCon) {
    var areAllOnShip = leftCon.AssignedShip != null && rightCon.AssignedShip != null;
    var areOnDifferentShips = leftCon.AssignedShip != rightCon.AssignedShip;

    if (areAllOnShip && areOnDifferentShips) {
        
        var leftShip = leftCon.AssignedShip;
        var rightShip = rightCon.AssignedShip;
        
        leftShip.UnloadContainer(leftCon);
        rightShip.UnloadContainer(rightCon);

        if (leftShip.CanContainerBeLoaded(rightCon) && rightShip.CanContainerBeLoaded(leftCon)) {
            leftShip.LoadContainer(rightCon);
            rightShip.LoadContainer(leftCon);
        }
        else {
            leftShip.LoadContainer(leftCon);
            rightShip.LoadContainer(rightCon);
            Console.WriteLine("Operation aborted! Can't load container on the other ship!");
        }
    }
    else {
        Console.WriteLine($"Operation aborted! Containers on ships: {areAllOnShip} / on different ships: {areOnDifferentShips}");
    }
    
}


static double ThrowDice() {
    return Random.Shared.NextDouble();
}

