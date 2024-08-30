using Repositories.Models;
using Services.Services;
using System.Windows;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for CategoryDetailForm.xaml
    /// </summary>
    public partial class CategoryDetailForm : Window
    {
        public Category category;
        private SongService _songService = new();
        private CategoryService _categoryService = new();
        public CategoryDetailForm()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCategoryForm createForm = new();
            this.Visibility = System.Windows.Visibility.Hidden;
            createForm.ShowDialog();
            this.Visibility = System.Windows.Visibility.Visible;
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CategoryComboBoxLoad()
        {
            SongCategoryIdComboBox.ItemsSource = _categoryService.GetAllCategories();
            SongCategoryIdComboBox.DisplayMemberPath = "CategoryName";
            SongCategoryIdComboBox.SelectedValuePath = "CategoryId";
            SongCategoryIdComboBox.SelectedValue = category.CategoryId.ToString();
        }
        private void DataGirdLoad()
        {
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = _songService.SearchSongs(x => x.CategoryId == category.CategoryId);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataGirdLoad();
            TitleLabel.Content = category.CategoryName;
            CategoryComboBoxLoad();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Song> songs = _songService.GetAllSongs().Where(song => song.Title.Contains(SongNameTextBox.Text) && song.CategoryId == category.CategoryId).ToList();
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songs;
        }

        private async void SearchCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedId = int.Parse(SongCategoryIdComboBox.SelectedValue.ToString());
            category = await Task.Run(() => _categoryService.GetCategoryById(selectedId));
            DataGirdLoad();
            TitleLabel.Content = category.CategoryName;
            CategoryComboBoxLoad();
        }
    }
}
