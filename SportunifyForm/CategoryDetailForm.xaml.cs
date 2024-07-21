using Repositories.Models;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SongCategoryIdComboBox.ItemsSource = _categoryService.GetAllCategories();
            SongCategoryIdComboBox.DisplayMemberPath = "CategoryName";
            SongCategoryIdComboBox.SelectedValuePath = "CategoryId";
            SongCategoryIdComboBox.SelectedValue = category.CategoryId;
            TitleLabel.Content = category.CategoryName;
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = _songService.SearchSongs(x => x.CategoryId == category.CategoryId);
        }

        private void SongCategoryIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedId = int.Parse(SongCategoryIdComboBox.SelectedValue.ToString());
            category = _categoryService.GetAllCategories().First(cate => cate.CategoryId == selectedId);
            Window_Loaded(sender, e);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Song> songs = _songService.GetAllSongs().Where(song => song.Title.Contains(SongNameTextBox.Text) && song.CategoryId == category.CategoryId).ToList();
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songs;
        }
    }
}
