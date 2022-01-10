namespace Models;

using System.Data;

public class Order
{
    //You can also use DateTime data type for this
    public DateOnly OrderDate { get; set; }
    public int CustomerId { get; set; }
    public int OrderNumber { get; set; }
    public int StoreId { get; set; }
    public int LineItemID { get; set; }
    public decimal Total { get; set; }

    public Order(){}

    public Order(int customerid, int ordernumber, int storeid, int lineitemid){
        this.OrderDate = new DateOnly();
        this.CustomerId = customerid;
        this.OrderNumber = ordernumber;
        this.StoreId = storeid;
        this.LineItemID = lineitemid;
        //this.CalculateTotal();
    }

    public Order(DataRow row){
        this.OrderDate = (DateOnly) row["DateOfOrder"];
        this.CustomerId = (int) row["CustomerID"];
        this.StoreId = (int) row["StoreID"];
        this.Total = (decimal) row["Total"];
        this.LineItemID = (int) row["LineItemID"];

    }
    // public decimal CalculateTotal() {
    //     a method that would go through each lineitem in LineItems property
    //     and sets the total property of the particular order object
    //     decimal total = 0;
    //     if(this.LineItems?.Count > 0)
    //     {
    //         foreach(LineItem lineitem in this.LineItems)
    //         {
    //             //multiply the product's price by how many we're buying
    //             total += lineitem.Item.Price * lineitem.Quantity;
    //         }
    //     }
    //     this.Total = total;
    //     return total;
    // }

    // public void printLineItems(){
    //     foreach(LineItem li in this.LineItems){
    //         Console.WriteLine($"{li.ToString()}");
    //     }
    // }

    public override string ToString()
    {
        return $"Date: {this.OrderDate} \nCustomer ID: {this.CustomerId} \nOrder ID: {this.OrderNumber} \nStore ID: {this.StoreId} \n Total Cost: {this.Total}";
    }

    public void ToDataRow(ref DataRow row){
        this.OrderDate = (DateOnly) row["DateOfOrder"];
        this.CustomerId = (int) row["CustomerID"];
        this.StoreId = (int) row["StoreID"];
        this.Total = (decimal) row["Total"];
        this.LineItemID = (int) row["LineItemID"];
    }
}