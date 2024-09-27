using System.Collections.ObjectModel;
using KFC_Menu.Models;
using KFC_Menu.Services;
using SQLite;


namespace KFC_Menu;

public partial class OrderPage : ContentPage
{
	private static ObservableCollection<OrderItem> orderItems;
	private User loggedInUser;
	private int quantity;
	public OrderPage()
	{
		InitializeComponent();
		LoadLoggedInUser();
		quantity = 1;
		BindingContext = this;
    }

	private async void LoadLoggedInUser()
	{
		loggedInUser = await DatabaseServices.Instance.getLoggedInUser();
		if (loggedInUser != null)
		{
			LoadOrderItems();
		}
	}
	private async void LoadOrderItems()
	{
		var items = await DatabaseServices.Instance.getOrderItem(loggedInUser.Id);
		BindingContext = items;
		orderItems = new ObservableCollection<OrderItem>(items);
		OrderCollectionView.ItemsSource = orderItems;
		UpdateTotalPrice();
	}

	private void UpdateTotalPrice()
	{
		decimal totalPrice = orderItems.Sum(item => item.Quantity * item.Price);
		TotalPriceLabel.Text = $"${totalPrice}";
	}


    private async void IncreaseQuantityClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var orderItem = button.BindingContext as OrderItem;
		orderItem.Quantity++;
		await DatabaseServices.Instance.updateOrderItem(orderItem);
		UpdateTotalPrice();
	}

	private async void DecreaseQuantityClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var orderItem = button.BindingContext as OrderItem;
		if (orderItem.Quantity > 1)
		{
			orderItem.Quantity--;
			await DatabaseServices.Instance.updateOrderItem(orderItem);
			UpdateTotalPrice();
		}

	}

	private async void RemoveItemClicked(object sender, EventArgs e)
	{
		var button = sender as ImageButton;
		var orderItem = button.BindingContext as OrderItem;
		await DatabaseServices.Instance.removeOrderItem(orderItem.Id);
		orderItems.Remove(orderItem);
		UpdateTotalPrice();
	}

	private async void OrderClicked(object sender, EventArgs e)
	{
		var newOrder = new Order
		{
			UserId = loggedInUser.Id,
			OrderDate = DateTime.Now,
			TotalPrice = orderItems.Sum(item => item.Quantity * item.Price)
		};

		await DatabaseServices.Instance.addOrder(newOrder);


		await DisplayAlert("Order", "Your order has been successfully placed and will be delivered soon.", "OK");

		orderItems.Clear();
		UpdateTotalPrice();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LoadLoggedInUser();
		LoadOrderItems();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    public class OrderItem
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int UserId { get; set; }
		public int OrderId { get; set; }
		public int ItemId { get; set; }
		public DateTime OrderDate { get; set; }
		public string ItemName { get; set; }
		public string ItemImage { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public decimal TotalPrice => Price * Quantity;
	}
}