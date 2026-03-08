using DevNote.ViewModels;

namespace DevNote.Pages;

public partial class CategoryDetailPage : ContentPage
{
    public CategoryDetailPage(CategoryDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
