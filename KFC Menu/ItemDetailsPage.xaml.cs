using KFC_Menu.Models;
using KFC_Menu.Services;
using static KFC_Menu.OrderPage;

namespace KFC_Menu;

public partial class ItemDetailsPage : ContentPage
{
	private MenuItem currentItem;
	private int quantity;
	private User loggedInUser;

    public ItemDetailsPage(MenuItem item)
    {
        InitializeComponent();
        currentItem = item;
        quantity = 1;
        BindingContext = currentItem;

        ItemQuantity.Text = quantity.ToString();
        UpdateAddToCartButtonText();
        LoadLoggedInUser();
    }

    public async void LoadLoggedInUser()
	{
		loggedInUser = await DatabaseServices.Instance.getLoggedInUser();
		ItemQuantity.Text = quantity.ToString();
		UpdateFavoriteButtonState();
	}
    private async void BackButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	private async void FavoriteButtonClicked(object sender, EventArgs e)
	{
		try
		{
			loggedInUser = await DatabaseServices.Instance.getLoggedInUser();

			if (loggedInUser != null)
			{
				var favoriteItem = await DatabaseServices.Instance.getFavoriteItem(loggedInUser.Id, currentItem.Id);
				if (favoriteItem == null)
				{
                    var favorite = new Favorite
                    {
                        UserId = loggedInUser.Id,
                        ItemId = currentItem.Id,
                        ItemName = currentItem.Name,
                        ItemPrice = currentItem.Price,
                        ItemImage = currentItem.Image,
                        ItemDescription = currentItem.Description,
                        ItemCategory = currentItem.Category
                    };

                    await DatabaseServices.Instance.addFavorite(loggedInUser.Id, currentItem);
                    await DisplayAlert("Added", $"{currentItem.Name} has been added to favorites", "OK");
                    UpdateFavoriteButtonState();
                }
				else
				{
                    await DatabaseServices.Instance.removeFromFavorites(loggedInUser.Id, currentItem.Id);
                    await DisplayAlert("Removed", $"{currentItem.Name} has been removed from favorites", "OK");
                    UpdateFavoriteButtonState();
                }
            }
			else
			{
				await DisplayAlert("Error", "User is not logged in.", "OK");
            }
        }
		catch (ObjectDisposedException ex)
		{
			await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", "An unexpected error occurred: " + ex.Message, "OK");
        }
		
	}

	private void DecreaseQuantityClicked(object sender, EventArgs e)
	{
		if (quantity > 1)
		{
			quantity--;
			ItemQuantity.Text = quantity.ToString();
            UpdateAddToCartButtonText();
        }
	}

    private void IncreaseQuantityClicked(object sender, EventArgs e)
    {
            quantity++;
            ItemQuantity.Text = quantity.ToString();
			UpdateAddToCartButtonText();
    }

	private void UpdateAddToCartButtonText()
	{
		string priceString = currentItem.Price.Replace("$", "").Trim();
		if (decimal.TryParse(priceString, out decimal price))
		{
			decimal total = quantity * price;
			AddToCartButton.Text = $"Add to cart  ${total}";
		}
	}

	private async void UpdateFavoriteButtonState()
	{
		if (loggedInUser == null) return;

		var favoriteItem = await DatabaseServices.Instance.getFavoriteItem(loggedInUser.Id, currentItem.Id);
		if (favoriteItem == null)
		{
			FavoriteButton.Source = "heart_regular.svg";
		}
		else
		{
			FavoriteButton.Source = "heart_solid.svg";
		}
	}

	private async void AddToCartButtonClicked(object sender, EventArgs e)
	{
		loggedInUser = await DatabaseServices.Instance.getLoggedInUser();
		if (loggedInUser == null)
		{
			await DisplayAlert("Error", "You need to be logged in to add items to the cart.", "OK");
			return;
		}

		var orderItem = new OrderItem
		{
			UserId = loggedInUser.Id,
			Id = currentItem.Id,
			OrderDate = DateTime.Now,
			ItemName = currentItem.Name,
			ItemImage = currentItem.Image,
			Price = decimal.Parse(currentItem.Price.Replace("$", "").Trim()),
			Quantity = quantity
		};
		await DatabaseServices.Instance.addOrderItem(orderItem);

		await DisplayAlert("Added", $"{currentItem.Name} has been added to your order", "OK");
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadLoggedInUser();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}