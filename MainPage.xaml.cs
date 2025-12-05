using System.Collections.ObjectModel;

namespace Lab3;

public partial class MainPage : ContentPage
{
    
    ObservableCollection<Book> booksCollection = new ObservableCollection<Book>();

    JsonFileService fileService = new JsonFileService();
    string currentFilePath = "";

    public MainPage()
    {
        InitializeComponent();
       
        BooksList.ItemsSource = booksCollection;
    }

   
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var newBook = new Book(); 

        var editPage = new BookEditPage(newBook, (book) =>
        {
            book.Id = booksCollection.Count + 1; 
            booksCollection.Add(book); 
            StatusLabel.Text = $"Книгу '{book.Title}' додано!";
        });

       
        await Navigation.PushModalAsync(editPage);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bookToDelete = button.CommandParameter as Book;

        if (bookToDelete != null)
        {
            bool answer = await DisplayAlert("Видалення", $"Видалити '{bookToDelete.Title}'?", "Так", "Ні");
            if (answer)
            {
                booksCollection.Remove(bookToDelete);
                StatusLabel.Text = "Книгу видалено.";
            }
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bookToEdit = button.CommandParameter as Book;

        if (bookToEdit != null)
        {
            var editPage = new BookEditPage(bookToEdit, (book) =>
            {
                
                int index = booksCollection.IndexOf(book);
                booksCollection[index] = book;

                StatusLabel.Text = $"Книгу '{book.Title}' оновлено!";
            });

            await Navigation.PushModalAsync(editPage);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                currentFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "library.json");
            }

            await fileService.SaveToFileAsync(booksCollection.ToList(), currentFilePath);

            await DisplayAlert("Успіх", $"Збережено у:\n{currentFilePath}", "ОК");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        try
        {
            var jsonFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "json" } },
                });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть файл JSON",
                FileTypes = jsonFileType
            });

            if (result != null)
            {
                string path = result.FullPath;
                var loadedBooks = await fileService.ReadFromFileAsync(path);

                booksCollection.Clear();
                foreach (var book in loadedBooks)
                {
                    booksCollection.Add(book);
                }

                currentFilePath = path;

                StatusLabel.Text = $"Завантажено: {result.FileName}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося відкрити файл: {ex.Message}", "ОК");
        }
    }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        string term = SearchEntry.Text?.ToLower() ?? "";
        bool isNumber = int.TryParse(term, out int searchYear);

        var filteredList = booksCollection.Where(book =>

            book.Title.ToLower().Contains(term) ||
            book.Author.ToLower().Contains(term) ||
            (isNumber && book.Year == searchYear)

        ).ToList();

        BooksList.ItemsSource = filteredList;
        StatusLabel.Text = $"Знайдено: {filteredList.Count}";
    }

    private void OnResetSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
        BooksList.ItemsSource = booksCollection;
        StatusLabel.Text = "Список скинуто.";
    }

   
    private async void OnAboutClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushModalAsync(new AboutPage());
    }
}
