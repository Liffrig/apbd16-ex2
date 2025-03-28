// See https://aka.ms/new-console-template for more information


using ex2_containers;




// Stworzenie kontenera danego typu
var conL = new L_Container(250,2.31,64,28.6,false);
var conG = new G_Container(345,2.56,36,17.8,12.3);
var conC = new C_Container(490,3.33,31,33.3,RefrigeratedProduct.Bananas,6.9);

var conL1 = new L_Container(221,3.1,63,13.3,true);
var conG1 = new G_Container(123,2.17,92,22.3,7);
var conC1 = new C_Container(510,2.56,44,12.3,RefrigeratedProduct.FrozenPizza,-73);


// Załadowanie ładunku do danego kontenera
foreach (var con in Container.GetAllContainers()) {
    
    if (con.GetType() == typeof(C_Container)) {
        var prod = (con as C_Container).RefrigeratedProduct;
        (con as C_Container).Load(ThrowDice() * con.FreeKgCapacity, prod);
    }
    else {
        con.Load(ThrowDice() * con.FreeKgCapacity);
    }
    Console.WriteLine(con);
}
// Załadowanie kontenera na statek
var evergreen1 = new CargoShip(23, 2400, 235);
var evergreen2 = new CargoShip(25, 3000, 300);

foreach (var con in Container.GetAllContainers()) {
    if (ThrowDice() < 0.5) {
        evergreen1.LoadContainer(con);
    }
    else {
        evergreen2.LoadContainer(con);
    }
    
}


//Todo
// Załadowanie listy kontenerów na statek
//     Usunięcie kontenera ze statku
//     Rozładowanie kontenera
//     Zastąpienie kontenera na statku o danym numerze innym kontenerem
// Możliwość przeniesienie kontenera między dwoma statkami
// Wypisanie informacji o danym kontenerze
//     Wypisanie informacji o danym statku i jego ładunk





static double ThrowDice() {
    return Random.Shared.NextDouble();
} 



